// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests.Fixtures
{
    public class AppFixture : IHttpApplication<AppFixture.MessageContext>, IDisposable
    {
        public Func<MessageContext, Task> OnRequest { get; set; } = context => Task.CompletedTask;

        public MessageContext CreateContext(IFeatureCollection contextFeatures)
        {
            return new MessageContext { Features = contextFeatures };
        }

        public void DisposeContext(MessageContext context, Exception exception)
        {
        }

        public Task ProcessRequestAsync(MessageContext context) => OnRequest(context);

        public void Dispose()
        {
        }

        public class MessageContext
        {
            public IFeatureCollection Features { get; set; }

            public TFeature Get<TFeature>() => Features.Get<TFeature>();

            public Stream ResponseStream
            {
                get
                {
                    return Get<IHttpResponseBodyFeature>().Stream;
                }
            }
        }
    }
}
