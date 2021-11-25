using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Cake.Cli;
using Cake.Commands.Init.Extensions;
using Cake.Commands.Init.Models;
using Cake.Common.Text;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Text;
using Spectre.Console;
using Spectre.Console.Cli;
using Index = Cake.Commands.Init.Models.Index;
using Path = Cake.Core.IO.Path;

namespace Cake.Commands.Init
{
    public sealed class InitCommand : AsyncCommand<InitCommandSettings>
    {
        private readonly ICakeLog _log;
        private readonly ICakeEnvironment _environment;
        private readonly IFileSystem _fileSystem;
        private readonly IVersionResolver _versionResolver;
        private readonly Func<string, HttpClient> _createClient;

        public override async Task<int> ExecuteAsync(CommandContext context, InitCommandSettings settings)
        {
            if (settings.List)
            {
                return await ListTemplates(settings);
            }

            if (string.IsNullOrWhiteSpace(settings.Template))
            {
                settings.Template = await PickTemplate(settings);
            }

            using (var client = _createClient(nameof(InitCommand)))
            {
                var zipUri = $"https://github.com/{settings.Source}/{settings.Template}/archive/refs/heads/main.zip";
                _log.Information("Fetching available template from {0}...", zipUri);
                using (var stream = await client.GetStreamAsync(zipUri))
                {
                    using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
                    {
                        var rootPath = DirectoryPath.FromString($"{settings.Template}-main");
                        var entriesLookup = GetEntriesLookup(zip, rootPath);

                        var templateJsonPath = rootPath.CombineWithFilePath("template.json");
                        var templateJsonEntry = entriesLookup[templateJsonPath].FirstOrDefault()
                                                ?? throw new FileNotFoundException(
                                                    $"Template json not found for template {settings.Template}", templateJsonPath.FullPath);

                        var (repository, template) = await GetRepositoryTemplate(templateJsonEntry.Entry);

                        IEnumerable<KeyValuePair<string, object>> tokens = PickAnswers(repository, template)
                            .Union(GetStandardTokens())
                            .ToArray()
                            ?? Array.Empty<KeyValuePair<string, object>>();

                        var templateRootPath = DirectoryPath.FromString($"{settings.Template}-main/{template.Name}");
                        var templateAbsoluteRootPath = templateRootPath.MakeAbsolute(_environment);

                        foreach (var entry in entriesLookup[templateRootPath]
                                                            .OrderBy(entry => entry.IsFile)
                                                            .ThenBy(entry => entry.Path.FullPath.Length))
                        {
                            if (entry.Path is DirectoryPath directoryPath)
                            {
                                var targetDirectory = _fileSystem.GetDirectory(_environment
                                    .WorkingDirectory
                                    .Combine(templateAbsoluteRootPath.GetRelativePath(directoryPath)));

                                if (targetDirectory.Exists)
                                {
                                    _log.Information("Directory: {0} Skipped (Exists)",
                                        targetDirectory.Path);
                                    continue;
                                }

                                targetDirectory.Create();

                                _log.Information("Directory: {0} Created",
                                    targetDirectory.Path);
                            }
                            else if (entry.Path is FilePath filePath)
                            {
                                var targetFile = _fileSystem.GetFile(_environment
                                    .WorkingDirectory
                                    .CombineWithFilePath(templateAbsoluteRootPath.GetRelativePath(filePath)));

                                if (targetFile.Exists)
                                {
                                    _log.Information("File: {0} Skipped (Exists)",
                                        targetFile.Path);
                                    continue;
                                }

                                using (Stream
                                    source = entry.Entry.Open(),
                                    target = targetFile.OpenWrite())
                                {
                                    if (IsKnownText(targetFile))
                                    {
                                        Transform(source, target, tokens);
                                    }
                                    else
                                    {
                                        await source.CopyToAsync(target);
                                    }
                                }

                                _log.Information("File: {0} Created",
                                    targetFile.Path);
                            }
                        }

                        return 0;
                    }
                }
            }
        }

        private IEnumerable<KeyValuePair<string, object>> GetStandardTokens()
        {
            yield return new KeyValuePair<string, object>("cake_version", _versionResolver.GetVersion());
            yield return new KeyValuePair<string, object>("cake_product_version", _versionResolver.GetProductVersion());
        }

        private static IEnumerable<KeyValuePair<string, object>> PickAnswers(Repository repository, RepositoryTemplate template)
        {
            return repository
                .Questions
                .Union(template
                    .Questions)
                .Select(question => AnsiConsole.Prompt(
                        new SelectionPrompt<RepositoryAnswers>()
                            .Title(question.Description)
                            .PageSize(10)
                            .MoreChoicesText("[grey](Move up and down to reveal more answers)[/]")
                            .AddChoices(question.Answers)
                            .UseConverter(t => t.Description))
                    .Values)
                .SelectMany(kv => kv);
        }

        private static async Task<(Repository repository, RepositoryTemplate template)> GetRepositoryTemplate(ZipArchiveEntry entry)
        {
            using (var templateJsonStream = entry.Open())
            {
                var repository = await JsonSerializer.DeserializeAsync<Repository>(templateJsonStream);
                if (repository?.Templates?.Any() != true)
                {
                    throw new CakeException("No templates found");
                }

                var template = AnsiConsole.Prompt(
                               new SelectionPrompt<RepositoryTemplate>()
                                   .Title("Pick template")
                                   .PageSize(10)
                                   .MoreChoicesText("[grey](Move up and down to reveal more templates)[/]")
                                   .AddChoices(repository.Templates)
                                   .UseConverter(t => string.Concat(t.Name, " - ", t.Description)))
                           ?? throw new NullReferenceException("Failed to find template");

                return (repository, template);
            }
        }


        private ILookup<Path, ZipEntryPath> GetEntriesLookup(ZipArchive zip, DirectoryPath rootPath)
        {
            var entriesLookup = zip
                .Entries
                .ToLookup(
                    key => GetRootPath(rootPath, FilePath.FromString(key.FullName)),
                    entry =>
                    {
                        var isFile = entry.IsFile();
                        return new ZipEntryPath(entry, isFile, isFile
                            ? FilePath
                                .FromString(entry.FullName)
                                .MakeAbsolute(_environment) as Path
                            : DirectoryPath
                                .FromString(entry.FullName)
                                .MakeAbsolute(_environment));
                    },
                    PathComparer.Default);
            return entriesLookup;
        }

        private async Task<string> PickTemplate(InitCommandSettings settings)
        {
            var index = await GetTemplates(settings);
            string template = AnsiConsole.Prompt(
                                      new SelectionPrompt<IndexRepository>()
                                          .Title("Pick template repository")
                                          .PageSize(10)
                                          .MoreChoicesText("[grey](Move up and down to reveal more repositories)[/]")
                                          .AddChoices(index?.Repositories ?? Array.Empty<IndexRepository>())
                                          .UseConverter(t => string.Concat(t.Name, " - ", t.Description)))
                                  ?.Name
                              ?? throw new NullReferenceException("Failed to find repository");
            return template;
        }

        private static bool IsKnownText(IFile targetFile)
            => targetFile
                    ?.Path
                    ?.GetExtension()
                    ?.ToLowerInvariant()
                switch
                {
                    ".json" => true,
                    ".yml" => true,
                    ".config" => true,
                    ".sh" => true,
                    ".cmd" => true,
                    ".ps1" => true,
                    ".cake" => true,
                    _ => false
                };

        private readonly Tuple<string, string> _placeholder = new Tuple<string, string>("<%", "%>");
        private void Transform(Stream source, Stream target, IEnumerable<KeyValuePair<string, object>> tokens)
        {
            // Read the content of the file.
            using (var reader = new StreamReader(source, Encoding.UTF8, true, 1024, true))
            {
                var textTransformation = new TextTransformation<TextTransformationTemplate>(
                    _fileSystem, _environment,
                    new TextTransformationTemplate(reader.ReadToEnd(), _placeholder));
                textTransformation.WithTokens(tokens);
                textTransformation.Save(target);
            }
        }

        private Path GetRootPath(DirectoryPath rootPath, FilePath filePath)
        {
            var path = filePath.GetDirectory();
            if (rootPath == path || string.IsNullOrWhiteSpace(path.FullPath))
            {
                return filePath;
            }

            var parentPath = path;
            do
            {
                path = parentPath;
                parentPath = parentPath
                    .Combine("../")
                    .Collapse();
            }
            while (rootPath != parentPath);

            return path;
        }

        private async Task<int> ListTemplates(InitCommandSettings settings)
        {
            var index = await GetTemplates(settings);

            var templateFilter = string.IsNullOrWhiteSpace(settings.Template)
                ? new Func<IndexRepository, bool>(_ => true)
                : template => template?.Name?.StartsWith(settings.Template, StringComparison.OrdinalIgnoreCase) == true;

            var table = new Table()
                .AddColumn("Name")
                .AddColumn("Description");

            foreach (var template in index?.Repositories?.Where(templateFilter) ?? Array.Empty<IndexRepository>())
            {
                table.AddRow(template.Name, template.Description);
            }

            AnsiConsole.Write(table);
            return 0;
        }

        private async Task<Index> GetTemplates(InitCommandSettings settings)
        {
            var indexUri = $"https://raw.githubusercontent.com/{settings.Source}/Home/main/index.json";
            _log.Information("Fetching available templates from {0}...", indexUri);
            Index index;
            using (var client = _createClient(nameof(ListTemplates)))
            {
                using (var stream = await client.GetStreamAsync(indexUri))
                {
                    index = await JsonSerializer.DeserializeAsync<Index>(stream);
                }
            }

            return index;
        }

        public InitCommand(
            ICakeLog log,
            ICakeEnvironment environment,
            IFileSystem fileSystem,
            IVersionResolver versionResolver) : this(
            log,
            environment,
            fileSystem,
            versionResolver,
            _ => new HttpClient())
        {
        }

        internal InitCommand(
            ICakeLog log,
            ICakeEnvironment environment,
            IFileSystem fileSystem,
            IVersionResolver versionResolver,
            Func<string, HttpClient> createClient)
        {
            _log = log;
            _environment = environment;
            _fileSystem = fileSystem;
            _versionResolver = versionResolver;
            _createClient = createClient;
        }
    }
}