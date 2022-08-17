using System;
using Cake.Common.Tests.Fixtures.Tools.Command;
using Xunit;

namespace Cake.Common.Tests.Unit.Tools.Command
{
    public sealed class CommandRunnerTests
    {
        public sealed class TheRunCommandMethod
        {
            [Fact]
            public void Should_Throw_If_Arguments_Was_Null()
            {
                // Given
                var fixture = new CommandRunnerFixture
                {
                    Arguments = null
                };

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                AssertEx.IsArgumentNullException(result, "arguments");
            }

            [Fact]
            public void Should_Throw_If_Settings_Was_Null()
            {
                // Given
                var fixture = new CommandRunnerFixture
                {
                    Settings = null
                };

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                AssertEx.IsArgumentNullException(result, "settings");
            }

            [Fact]
            public void Should_Throw_If_ToolName_Was_Null()
            {
                // Given
                var fixture = new CommandRunnerFixture
                {
                    ToolName = null
                };

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                AssertEx.IsArgumentNullException(result, "ToolName");
            }
        }

        [Fact]
        public void Should_Throw_If_ToolExecutableNames_Was_Null()
        {
            // Given
            var fixture = new CommandRunnerFixture
            {
                ToolExecutableNames = null
            };

            // When
            var result = Record.Exception(() => fixture.Run());

            // Then
            AssertEx.IsArgumentNullException(result, "ToolExecutableNames");
        }

        [Fact]
        public void Should_Throw_If_ToolExecutableNames_Was_Empty()
        {
            // Given
            var fixture = new CommandRunnerFixture
            {
                ToolExecutableNames = Array.Empty<string>()
            };

            // When
            var result = Record.Exception(() => fixture.Run());

            // Then
            AssertEx.IsArgumentNullException(result, "ToolExecutableNames");
        }

        [Fact]
        public void Should_Call_Settings_PostAction()
        {
            // Given
            var called = false;
            var fixture = new CommandRunnerFixture
            {
                Settings = { PostAction = _ => called = true }
            };

            // When
            var result = fixture.Run();

            // Then
            Assert.True(called, "Settings PostAction not called");
        }

        [Fact]
        public void Should_Return_StandardOutput()
        {
            // Given
            const string expectStandard = "LINE1";
            const int expectExitCode = 0;

            var fixture = new CommandRunnerStandardOutputFixture()
                            .GivenStandardOutput(expectStandard);

            // When
            fixture.Run();

            // Then
            Assert.Equal(expectStandard, fixture.StandardOutput);
            Assert.Equal(expectExitCode, fixture.ExitCode);
        }

        [Fact]
        public void Should_Return_StandardOutput_ExitCode()
        {
            // Given
            const string expectStandard = "LINE1";
            const int expectExitCode = 1337;

            var fixture = new CommandRunnerStandardOutputFixture
                            {
                                Settings =
                                                {
                                                    HandleExitCode = exitCode => exitCode == expectExitCode
                                                }
                            }
                            .GivenStandardOutput(expectStandard);

            fixture.ProcessRunner.Process.SetExitCode(expectExitCode);

            // When
            fixture.Run();

            // Then
            Assert.Equal(expectStandard, fixture.StandardOutput);
            Assert.Equal(expectExitCode, fixture.ExitCode);
        }

        [Fact]
        public void Should_Return_StandardError()
        {
            // Given
            const string expectStandard = "LINE1";
            const string expectError = "ERRORLINE1";
            const int expectExitCode = 0;

            var fixture = new CommandRunnerStandardErrorFixture()
                            .GivenStandardError(expectError)
                            .GivenStandardOutput(expectStandard);

            // When
            fixture.Run();

            // Then
            Assert.Equal(expectStandard, fixture.StandardOutput);
            Assert.Equal(expectError, fixture.StandardError);
            Assert.Equal(expectExitCode, fixture.ExitCode);
        }

        [Fact]
        public void Should_Return_StandardError_ExitCode()
        {
            // Given
            const string expectStandard = "LINE1";
            const string expectError = "ERRORLINE1";
            const int expectExitCode = 1337;

            var fixture = new CommandRunnerStandardErrorFixture
                            {
                                Settings =
                                {
                                    HandleExitCode = exitCode => exitCode == expectExitCode
                                }
                            }
                            .GivenStandardError(expectError)
                            .GivenStandardOutput(expectStandard);

            fixture.ProcessRunner.Process.SetExitCode(expectExitCode);

            // When
            fixture.Run();

            // Then
            Assert.Equal(expectStandard, fixture.StandardOutput);
            Assert.Equal(expectError, fixture.StandardError);
            Assert.Equal(expectExitCode, fixture.ExitCode);
        }
    }
}
