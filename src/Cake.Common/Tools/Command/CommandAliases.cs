// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.Common.Tools.Command
{
    /// <summary>
    /// <para>Contains generic functionality for simplifying the execution tools with no dedicated alias available yet.</para>
    /// </summary>
    [CakeAliasCategory("Command")]
    public static class CommandAliases
    {
        /// <summary>
        /// Executes a generic command based on arguments and settings.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="arguments"/>, <paramref name="context"/> or <paramref name="settings"/> is null.</exception>
        [CakeMethodAlias]
        [CakeAliasCategory("Command")]
        public static void Command(
            this ICakeContext context,
            ProcessArgumentBuilder arguments,
            CommandSettings settings)
        {
            var runner = GetRunner(context, settings);

            runner.RunCommand(arguments);
        }

        /// <summary>
        /// Executes a generic command based on arguments and settings.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="standardOutput">The standard output.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="arguments"/>, <paramref name="context"/> or <paramref name="settings"/> is null.</exception>
        /// <returns>The exit code.</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Command")]
        public static int Command(
            this ICakeContext context,
            ProcessArgumentBuilder arguments,
            CommandSettings settings,
            out string standardOutput)
        {
            var runner = GetRunner(context, settings);

            return runner.RunCommand(arguments, out standardOutput);
        }

        /// <summary>
        /// Executes a generic command based on arguments and settings.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="standardOutput">The standard output.</param>
        /// <param name="standardError">The standard error.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="arguments"/>, <paramref name="context"/> or <paramref name="settings"/> is null.</exception>
        /// <returns>The exit code.</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Command")]
        public static int Command(
            this ICakeContext context,
            ProcessArgumentBuilder arguments,
            CommandSettings settings,
            out string standardOutput,
            out string standardError)
        {
            var runner = GetRunner(context, settings);

            return runner.RunCommand(arguments, out standardOutput, out standardError);
        }

        private static CommandRunner GetRunner(ICakeContext context, CommandSettings settings)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var runner = new CommandRunner(
                settings,
                context.FileSystem,
                context.Environment,
                context.ProcessRunner,
                context.Tools);

            return runner;
        }
    }
}
