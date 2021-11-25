using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cake.Commands.Init
{
    public class InitCommandSettings : CommandSettings
    {
        private const string DefaultSource = "cake-init";

        [CommandArgument(0, "[TEMPLATE]")]
        [Description("The the template to use.")]
        public string Template { get; set; }

        [CommandOption("--list|-l")]
        [Description("Lists templates containing the specified template name. If no name is specified, lists all templates.")]
        public bool List { get; set; }

        [CommandOption("--source|-s <SOURCE_ORG_OR_USER>")]
        [Description("Specifies the GitHub organization or user containing templates. Defaults to [grey]" + DefaultSource + "[/]")]
        public string Source { get; set; } = DefaultSource;
    }
}