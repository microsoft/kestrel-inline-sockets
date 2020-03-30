// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;

namespace Microsoft.AspNetCore.Connections.Features
{
    /// <summary>
    /// Zero allocation utility enables application to have using guarentee
    /// that each call to Suspend is matched with exactly one call to Resume.
    /// Not intended to be used directly this utility object is created by calling
    /// <see cref="ConnectionOutputControlExtensions.SuspendScope(IConnectionOutputControlFeature)"/>
    /// </summary>
    public struct ConnectionOutputControlScope : IDisposable
    {
        private IConnectionOutputControlFeature _feature;

        /// <summary>
        /// Calls <see cref="IConnectionOutputControlFeature.Suspend"/> when
        /// a scope is created.
        /// </summary>
        /// <param name="feature">The <seealso cref="IConnectionOutputControlFeature"/> to suspend.</param>
        /// <returns>The new <seealso cref="ConnectionOutputControlScope"/>.</returns>
        public static ConnectionOutputControlScope Create(IConnectionOutputControlFeature feature)
        {
            feature.Suspend();
            return new ConnectionOutputControlScope
            {
                _feature = feature
            };
        }

        /// <summary>
        /// Calls <seealso cref="IConnectionOutputControlFeature.Resume"/> when
        /// the scope is disposed.
        /// </summary>
        public void Dispose()
        {
            _feature.Resume();
        }
    }
}
