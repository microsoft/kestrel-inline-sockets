// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging
{
    public interface IConnectionLogger : ILogger
    {
        void ConnectionAborting(string connectionId, ConnectionAbortedException abortReason);

        void ConnectionClosing(string connectionId);

        void ConnectionClosed(string connectionId);

        void ConnectionDisposed(string connectionId, bool isAsync);

        void PipeReaderComplete(string connectionId, Exception error);

        void PipeWriterComplete(string connectionId, Exception error);

        void ReadStarting(string connectionId, int bufferLength);

        void ReadSucceeded(string connectionId, int bytesRead);

        void ReadFailed(string connectionId, Exception error);

        void PendingReadCanceling(string connectionId);

        void ReadCanceled(string connectionId);

        void WriteStarting(string connectionId, int bufferLength);

        void WriteSucceeded(string connectionId, int bytesWritten);

        void WriteFailed(string connectionId, Exception error);

        void PendingWriteCanceling(string connectionId);

        void WriteCanceled(string connectionId);
    }
}
