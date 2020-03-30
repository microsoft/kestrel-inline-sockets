// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.AspNetCore.Connections.Features
{
    /// <summary>
    /// Feature interface providing flow control for connection output.
    /// </summary>
    public interface IConnectionOutputControlFeature
    {
        /// <summary>
        /// Gets true if <see cref="Suspend"/> has been called more times than <see cref="Resume"/>.
        /// </summary>
        bool IsSuspended { get; }

        /// <summary>
        /// Causes the connection to stop writing data to the output pipeline. Each
        /// call to Suspend must be matched by exactly one subsequent call to Resume. All
        /// buffered data will be sent to the output pipeline when each call to Suspend
        /// has been matched with exactly one corresponding call to Resume.
        /// </summary>
        void Suspend();

        /// <summary>
        /// Causes the contection to continue writing data to the output pipeline. Each
        /// call to Resume must match exactly one earlier call to Suspend. All
        /// buffered data will be sent to the output pipeline when each call to Suspend
        /// has been matched with exactly one corresponding call to Resume.
        /// </summary>
        void Resume();
    }
}
