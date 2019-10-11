using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests.Fixtures;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.TestHelpers.Fixtures
{
    public class ClientFixture : IDisposable
    {
        private readonly EndPointFixture _endPoint;
        private readonly TimeoutFixture _timeout;
        private readonly HttpClient _httpClient;

        public ClientFixture(EndPointFixture endPoint, TimeoutFixture timeout)
        {
            _endPoint = endPoint;
            _timeout = timeout;

#if NETSTANDARD2_0
            HttpMessageHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, errors) => RemoteCertificateValidation(certificate, chain, errors),
            };
#else
            HttpMessageHandler handler = new SocketsHttpHandler
            {
                SslOptions =
                {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => RemoteCertificateValidation(certificate, chain, errors),
                }
            };
#endif

            _httpClient = new HttpClient(handler, disposeHandler: true);
        }

        public Func<X509Certificate, X509Chain, SslPolicyErrors, bool> RemoteCertificateValidation { get; set; } = (certificate, chain, errors) => true;

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public async Task<HttpResponseMessage> GetAsync(string path)
        {
            return await _httpClient.GetAsync(_endPoint.Address + path, HttpCompletionOption.ResponseHeadersRead, _timeout.Token);
        }

        public async Task<(HttpResponseMessage response, string content)> GetStringAsync(string path)
        {
            var response = await GetAsync(path);
            return (response, await response.Content.ReadAsStringAsync());
        }

        public async Task<HttpResponseMessage> PostAsync(string path, HttpContent content)
        {
            return await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, _endPoint.Address + path) { Content = content }, HttpCompletionOption.ResponseHeadersRead, _timeout.Token);
        }

        public async Task<(HttpResponseMessage response, string content)> PostStringAsync(string path, HttpContent content)
        {
            var response = await PostAsync(path, content);
            return (response, await response.Content.ReadAsStringAsync());
        }

        public async Task<(HttpResponseMessage response, byte[] content)> PostBytesAsync(string path, HttpContent content)
        {
            var response = await PostAsync(path, content);
            return (response, await response.Content.ReadAsByteArrayAsync());
        }
    }
}
