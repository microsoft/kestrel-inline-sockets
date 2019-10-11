// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if NETSTANDARD2_0
using System;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public partial class Connection : IConnection
    {
        private readonly object _synchronizeCompletion = new object();
        private bool _pipeWriterComplete;
        private bool _pipeReaderComplete;

        void IConnection.OnPipeReaderComplete(Exception exception)
        {
            OnPipeComplete(pipeReaderComplete: true);
        }

        void IConnection.OnPipeWriterComplete(Exception exception)
        {
            OnPipeComplete(pipeWriterComplete: true);
        }

        private void OnPipeComplete(
            bool pipeReaderComplete = false,
            bool pipeWriterComplete = false)
        {
            var connectionClosed = false;
            var readerRemaining = false;
            lock (_synchronizeCompletion)
            {
                if (pipeReaderComplete)
                {
                    _pipeReaderComplete = true;
                }

                if (pipeWriterComplete)
                {
                    _pipeWriterComplete = true;
                }

                connectionClosed = _pipeReaderComplete && _pipeWriterComplete;
                readerRemaining = (_pipeReaderComplete == false) && _pipeWriterComplete;
            }

            if (connectionClosed)
            {
                // signal all tranceiving is complete
                FireConnectionClosed();
            }
            else if (readerRemaining)
            {
                // this is necessary for Kestrel to realize the connection has ended
                CancelPendingRead();
            }
        }
    }
}
#endif
