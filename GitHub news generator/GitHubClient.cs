using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GitHubNewsGenerator
{
    public abstract class GitHubClient : IDisposable
    {
        private readonly HttpClient client;

        public GitHubClient(string baseAddress, string userAgent, string accessToken)
        {
            client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = ~DecompressionMethods.None
            })
            {
                BaseAddress = new Uri(baseAddress),
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("bearer", accessToken),
                    UserAgent = { ProductInfoHeaderValue.Parse(userAgent) }
                }
            };
        }

        public void Dispose()
        {
            client.Dispose();
        }

        protected Task<StreamReader> PostAsync(Action<TextWriter> writeToBuffer, CancellationToken cancellationToken)
        {
            return PostAsync(url: null, writeToBuffer, cancellationToken);
        }

        protected async Task<StreamReader> PostAsync(string url, Action<TextWriter> writeToBuffer, CancellationToken cancellationToken)
        {
            HttpResponseMessage response;

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), bufferSize: 4096, leaveOpen: true))
                {
                    writeToBuffer.Invoke(writer);
                }

                memoryStream.Position = 0;

                response = await client.SendAsync(
                   new HttpRequestMessage(HttpMethod.Post, string.Empty)
                   {
                       RequestUri = url is null ? null : new Uri(url, UriKind.Relative),
                       Content = new StreamContent(memoryStream)
                   },
                   HttpCompletionOption.ResponseHeadersRead,
                   cancellationToken).ConfigureAwait(false);
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamReaderAsync().ConfigureAwait(false);
        }
    }
}
