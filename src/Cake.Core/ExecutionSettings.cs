// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cake.Core
{
    /// <summary>
    /// Contains settings related to execution of the script.
    /// </summary>
    public sealed class ExecutionSettings
    {
        /// <summary>
        /// Gets the targets to be executed.
        /// </summary>
        public IEnumerable<string> Targets { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not to use the target exclusively.
        /// </summary>
        public bool Exclusive { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionSettings"/> class.
        /// </summary>
        public ExecutionSettings()
        {
            Targets = Array.Empty<string>();
            Exclusive = false;
        }

        /// <summary>
        /// Sets the target to be executed.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>The same <see cref="ExecutionSettings"/> instance so that multiple calls can be chained.</returns>
        /// <remarks>Targets consisting of whitespace only will be ignored.</remarks>
        public ExecutionSettings SetTarget(string target)
        {
            Targets = string.IsNullOrWhiteSpace(target) ? Array.Empty<string>() : new string[] { target };
            return this;
        }

        /// <summary>
        /// Sets the targets to be executed.
        /// </summary>
        /// <param name="targets">The targets.</param>
        /// <returns>The same <see cref="ExecutionSettings"/> instance so that multiple calls can be chained.</returns>
        /// <remarks>Targets consisting of whitespace only will be ignored.</remarks>
        public ExecutionSettings SetTargets(IEnumerable<string> targets)
        {
            Targets = targets?.ToArray().Where(s => !string.IsNullOrWhiteSpace(s)) ?? Array.Empty<string>();
            return this;
        }

        /// <summary>
        /// Whether or not to use the target exclusively.
        /// </summary>
        /// <returns>The same <see cref="ExecutionSettings"/> instance so that multiple calls can be chained.</returns>
        public ExecutionSettings UseExclusiveTarget()
        {
            Exclusive = true;
            return this;
        }
    }
}