// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging
{
    public class ConnectionLogger : ForwardingLogger, IConnectionLogger
    {
        private static readonly LoggerMessage<string> _logConnectionAborting = (LogLevel.Information, nameof(ConnectionAborting), "Connection \"{ConnectionId}\" aborted");
        private static readonly LoggerMessage<string> _logConnectionClosed = (LogLevel.Trace, nameof(ConnectionClosed), "Connection \"{ConnectionId}\" closed");
        private static readonly LoggerMessage<string> _logConnectionClosing = (LogLevel.Trace, nameof(ConnectionClosing), "Close of connection \"{ConnectionId}\" requested");
        private static readonly LoggerMessage<string, string> _logConnectionDisposed = (LogLevel.Information, nameof(ConnectionDisposed), "Connection \"{ConnectionId}\" disposed {AsyncMode}");
        private static readonly LoggerMessage<string> _logPendingReadCanceling = (LogLevel.Trace, nameof(PendingReadCanceling), "Canceling pending reads on connection \"{ConnectionId}\"");
        private static readonly LoggerMessage<string> _logPendingWriteCanceling = (LogLevel.Trace, nameof(PendingWriteCanceling), "Canceling pending writes on connection \"{ConnectionId}\"");
        private static readonly LoggerMessage<string> _logPipeReaderComplete = (LogLevel.Trace, nameof(PipeReaderComplete), "All reading on connection \"{ConnectionId}\" is complete");
        private static readonly LoggerMessage<string> _logPipeWriterComplete = (LogLevel.Trace, nameof(PipeWriterComplete), "All writing on connection \"{ConnectionId}\" is complete");
        private static readonly LoggerMessage<string> _logReadCanceled = (LogLevel.Trace, nameof(ReadCanceled), "Read on connection \"{ConnectionId}\" was canceled");
        private static readonly LoggerMessage<string> _logReadFailed = (LogLevel.Information, nameof(ReadFailed), "Failed to read on connection \"{ConnectionId}\"");
        private static readonly LoggerMessage<string, int> _logReadStarting = (LogLevel.Trace, nameof(ReadStarting), "Starting read on connection \"{ConnectionId}\" for {BufferLength} bytes");
        private static readonly LoggerMessage<string, int> _logReadSucceeded = (LogLevel.Debug, nameof(ReadSucceeded), "Read on connection \"{ConnectionId}\" received {BytesRead} bytes");
        private static readonly LoggerMessage<string> _logWriteCanceled = (LogLevel.Trace, nameof(WriteCanceled), "Write on connection \"{ConnectionId}\" was canceled");
        private static readonly LoggerMessage<string> _logWriteFailed = (LogLevel.Information, nameof(WriteFailed), "Failed to write on connection \"{ConnectionId}\"");
        private static readonly LoggerMessage<string, int> _logWriteStarting = (LogLevel.Trace, nameof(WriteStarting), "Starting write on connection \"{ConnectionId}\" for {BufferLength} bytes");
        private static readonly LoggerMessage<string, int> _logWriteSucceeded = (LogLevel.Debug, nameof(WriteSucceeded), "Write on connection \"{ConnectionId}\" send {BytesWritten} bytes");
        private static readonly LoggerMessage<string, int> _logPipeWriterSuspended = (LogLevel.Trace, nameof(PipeWriterSuspended), "Writing on connection \"{ConnectionId}\" suspended; {SuspendCount} calls to resume are expected");
        private static readonly LoggerMessage<string, int> _logPipeWriterResumed = (LogLevel.Trace, nameof(PipeWriterResumed), "Writing on connection \"{ConnectionId}\" resumed; {SuspendCount} more calls to resume are expected");

        public ConnectionLogger(ILoggerFactory loggerFactory)
            : base(loggerFactory, "Microsoft.Bing.AspNetCore.Connections.InlineSocket.Connection")
        {
        }

        public virtual void ConnectionAborting(string connectionId, ConnectionAbortedException abortReason) => _logConnectionAborting.Log(this, connectionId, abortReason);

        public virtual void ConnectionClosed(string connectionId) => _logConnectionClosed.Log(this, connectionId, null);

        public virtual void ConnectionClosing(string connectionId) => _logConnectionClosing.Log(this, connectionId, null);

        public virtual void ConnectionDisposed(string connectionId, bool isAsync) => _logConnectionDisposed.Log(this, connectionId, isAsync ? "asynchronously" : "synchronously", null);

        public virtual void PendingReadCanceling(string connectionId) => _logPendingReadCanceling.Log(this, connectionId, null);

        public virtual void PendingWriteCanceling(string connectionId) => _logPendingWriteCanceling.Log(this, connectionId, null);

        public virtual void PipeReaderComplete(string connectionId, Exception error) => _logPipeReaderComplete.Log(this, connectionId, error);

        public virtual void PipeWriterComplete(string connectionId, Exception error) => _logPipeWriterComplete.Log(this, connectionId, error);

        public virtual void ReadCanceled(string connectionId) => _logReadCanceled.Log(this, connectionId, null);

        public virtual void ReadFailed(string connectionId, Exception error) => _logReadFailed.Log(this, connectionId, error);

        public virtual void ReadStarting(string connectionId, int bufferLength) => _logReadStarting.Log(this, connectionId, bufferLength, null);

        public virtual void ReadSucceeded(string connectionId, int bytesRead) => _logReadSucceeded.Log(this, connectionId, bytesRead, null);

        public virtual void WriteCanceled(string connectionId) => _logWriteCanceled.Log(this, connectionId, null);

        public virtual void WriteFailed(string connectionId, Exception error) => _logWriteFailed.Log(this, connectionId, error);

        public virtual void WriteStarting(string connectionId, int bufferLength) => _logWriteStarting.Log(this, connectionId, bufferLength, null);

        public virtual void WriteSucceeded(string connectionId, int bytesWritten) => _logWriteSucceeded.Log(this, connectionId, bytesWritten, null);

        public virtual void PipeWriterSuspended(string connectionId, int suspendCount) => _logPipeWriterSuspended.Log(this, connectionId, suspendCount, null);

        public virtual void PipeWriterResumed(string connectionId, int suspendCount) => _logPipeWriterResumed.Log(this, connectionId, suspendCount, null);
    }
}
