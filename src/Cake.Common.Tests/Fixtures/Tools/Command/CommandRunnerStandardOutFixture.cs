namespace Cake.Common.Tests.Fixtures.Tools.Command
{
    internal class CommandRunnerStandardOutputFixture : CommandRunnerFixture
    {
        public int ExitCode { get; protected set; }
        public string StandardOutput { get; protected set; }

        public CommandRunnerStandardOutputFixture GivenStandardOutput(params string[] standardOutput)
        {
            ProcessRunner.Process.SetStandardOutput(standardOutput);
            return this;
        }

        protected override void RunTool()
        {
            ExitCode = GetRunner().RunCommand(Arguments, out var standardOutput);
            StandardOutput = standardOutput;
        }
    }

    internal class CommandRunnerStandardErrorFixture : CommandRunnerStandardOutputFixture
    {
        public string StandardError { get; private set; }

        public new CommandRunnerStandardErrorFixture GivenStandardOutput(params string[] standardOutput)
        {
            base.GivenStandardOutput(standardOutput);
            return this;
        }

        public CommandRunnerStandardErrorFixture GivenStandardError(params string[] standardError)
        {
            ProcessRunner.Process.SetStandardError(standardError);
            return this;
        }

        protected override void RunTool()
        {
            ExitCode = GetRunner().RunCommand(Arguments, out var standardOutput, out var standardError);
            StandardOutput = standardOutput;
            StandardError = standardError;
        }
    }
}
