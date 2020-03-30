// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;

namespace Microsoft.AspNetCore.Connections.Features
{
    /// <summary>
    /// Extension methods for <seealso cref="IConnectionOutputControlFeature"/>.
    /// </summary>
    public static class ConnectionOutputControlExtensions
    {
        /// <summary>
        /// Calls <seealso cref="IConnectionOutputControlFeature.Suspend"/> and returns
        /// an <seealso cref="IDisposable"/> scope object. When <seealso cref="IDisposable"/>
        /// is called a corresponding call to <seealso cref="IConnectionOutputControlFeature.Resume"/>
        /// is made automatically.
        /// </summary>
        /// <param name="outputControl">An <seealso cref="IConnectionOutputControlFeature"/> interface
        /// to Suspend.</param>
        /// <returns>A <seealso cref="ConnectionOutputControlScope"/> object.</returns>
        public static ConnectionOutputControlScope SuspendScope(this IConnectionOutputControlFeature outputControl)
        {
            return ConnectionOutputControlScope.Create(outputControl);
        }
    }
}
