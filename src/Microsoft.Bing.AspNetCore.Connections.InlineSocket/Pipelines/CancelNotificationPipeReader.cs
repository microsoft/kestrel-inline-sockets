// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Pipelines
{
    public class CancelNotificationPipeReader : PipeReader
    {
        public CancelNotificationPipeReader(
            PipeReader inner,
            Action cancelPendingReadCallback)
        {
            Inner = inner;
            CancelPendingReadCallback = cancelPendingReadCallback;
        }

        public PipeReader Inner { get; }

        public Action CancelPendingReadCallback { get; }

        public override bool TryRead(out ReadResult result) => Inner.TryRead(out result);

        public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default) => Inner.ReadAsync(cancellationToken);

        public override void CancelPendingRead()
        {
            try
            {
                Inner.CancelPendingRead();
            }
            finally
            {
                CancelPendingReadCallback();
            }
        }

        public override void AdvanceTo(SequencePosition consumed) => Inner.AdvanceTo(consumed);

        public override void AdvanceTo(SequencePosition consumed, SequencePosition examined) => Inner.AdvanceTo(consumed, examined);

        public override void Complete(Exception exception = null) => Inner.Complete(exception);

        [Obsolete]
        public override void OnWriterCompleted(Action<Exception, object> callback, object state) => Inner.OnWriterCompleted(callback, state);
    }
}
