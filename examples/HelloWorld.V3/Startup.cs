// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HelloWorld
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInlineSocketsTransport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(next => async context =>
            {
                var outputControl = context.Features.Get<IConnectionOutputControlFeature>();
                var httpResponse = context.Features.Get<IHttpResponseFeature>();

                if (outputControl != null && httpResponse != null)
                {
                    outputControl.Suspend();
                    httpResponse.OnCompleted(async state => ((IConnectionOutputControlFeature)state).Resume(), outputControl);
                }

                await next(context);
            });

            app.Run(async (context) =>
            {
                Console.WriteLine("Request received");
                await context.Response.WriteAsync("Hello World!");
                Console.WriteLine("Response sent");
            });
        }
    }
}
