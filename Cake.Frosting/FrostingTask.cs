﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Cake.Core;
using Cake.Frosting.Internal;

namespace Cake.Frosting
{
    /// <summary>
    /// Base class for a Frosting task using the standard context.
    /// </summary>
    /// <seealso cref="ICakeContext" />
    public abstract class FrostingTask : FrostingTask<ICakeContext>
    {
    }

    /// <summary>
    /// Base class for a Frosting task using a custom context.
    /// </summary>
    /// <typeparam name="T">The context type.</typeparam>
    /// <seealso cref="IFrostingTask" />
    public abstract class FrostingTask<T> : IFrostingTask
        where T : ICakeContext
    {
        /// <summary>
        /// Runs the task using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Run(T context)
        {
        }

        /// <summary>
        /// Gets whether or not the task should be run.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if the task should run; otherwise <c>false</c>.
        /// </returns>
        public virtual bool ShouldRun(T context)
        {
            return true;
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Explicit implementation.")]
        void IFrostingTask.Run(ICakeContext context)
        {
            Guard.ArgumentNotNull(context, nameof(context));

            Run((T)context);
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Explicit implementation.")]
        bool IFrostingTask.ShouldRun(ICakeContext context)
        {
            Guard.ArgumentNotNull(context, nameof(context));

            return ShouldRun((T)context);
        }
    }
}
