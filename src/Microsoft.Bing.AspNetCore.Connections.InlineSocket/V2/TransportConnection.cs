// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if NETSTANDARD2_0
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public class TransportConnection : Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal.TransportConnection
    {
        private readonly IConnection _connection;
        private FeatureReference<IConnectionItemsFeature> _connectionItemsFeature;
        private FeatureReference<IConnectionTransportFeature> _connectionTransportFeature;
        private FeatureReference<IConnectionLifetimeFeature> _connectionLifetimeFeature;
        private FeatureReference<IMemoryPoolFeature> _memoryPoolFeature;
        private FeatureReference<IConnectionLifetimeNotificationFeature> _connectionLifetimeNotificationFeature;

        public TransportConnection(IConnection connection)
        {
            _connection = connection;

            // hoist ConnectionClosedRequested signal
            ConnectionClosedRequested.Register(() => _connectionLifetimeNotificationFeature.Fetch(Features).RequestClose());

            ConnectionClosedRequested = _connectionLifetimeNotificationFeature.Fetch(Features).ConnectionClosedRequested;
            ConnectionClosed = _connectionLifetimeFeature.Fetch(Features).ConnectionClosed;
        }

        public override IFeatureCollection Features => _connection.Features;

        public override string ConnectionId
        {
            get => _connection.ConnectionId;
            set => _connection.ConnectionId = value;
        }

        public override IDuplexPipe Transport
        {
            get => _connectionTransportFeature.Fetch(Features).Transport;
            set { }
        }

        public override MemoryPool<byte> MemoryPool => _memoryPoolFeature.Fetch(Features).MemoryPool;

        public override PipeScheduler InputWriterScheduler => PipeScheduler.Inline;

        public override PipeScheduler OutputReaderScheduler => PipeScheduler.Inline;

        public override IDictionary<object, object> Items
        {
            get => _connectionItemsFeature.Fetch(Features).Items;
            set => _connectionItemsFeature.Fetch(Features).Items = value;
        }

        public override void Abort()
        {
            _connectionLifetimeFeature.Fetch(Features).Abort();
        }

        public override void Abort(ConnectionAbortedException abortReason)
        {
            _connection.Abort(abortReason);
        }
    }
}
#endif

