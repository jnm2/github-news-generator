using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GitHubNewsGenerator
{
    internal static class Extensions
    {
        public static async Task<StreamReader> ReadAsStreamReaderAsync(this HttpContent content)
        {
            if (content is null) throw new ArgumentNullException(nameof(content));

            var stream = await content.ReadAsStreamAsync().ConfigureAwait(false);

            // Reference: https://github.com/dotnet/corefx/blob/v2.2.1/src/System.Net.Http/src/System/Net/Http/HttpContent.cs

            return new StreamReader(
                stream,
                TryDetectEncoding(content.Headers) ?? Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true);
        }

        private static Encoding TryDetectEncoding(HttpContentHeaders headers)
        {
            // Reference: https://github.com/dotnet/corefx/blob/v2.2.1/src/System.Net.Http/src/System/Net/Http/HttpContent.cs

            var charset = headers.ContentType?.CharSet;

            try
            {
                return Encoding.GetEncoding(charset);
            }
            catch (ArgumentException e)
            {
                throw new InvalidOperationException("Invalid charset in Content-Type header.", e);
            }
        }
    }
}
