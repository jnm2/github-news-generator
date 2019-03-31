using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GitHubNewsGenerator
{

    public sealed class GitHubRestClient : GitHubClient
    {
        public GitHubRestClient(string userAgent, string accessToken)
            : base(baseAddress: "https://api.github.com", userAgent, accessToken)
        {
        }

        public async Task<Uri> CreateGistAsync(
            string description,
            bool isPublic,
            IReadOnlyDictionary<string, string> contentByFileName,
            CancellationToken cancellationToken)
        {
            if (contentByFileName is null) throw new ArgumentNullException(nameof(contentByFileName));

            using (var reader = new JsonTextReader(await PostAsync(
                "/gists",
                buffer =>
                {
                    using (var writer = new JsonTextWriter(buffer))
                    {
                        writer.WriteStartObject();

                        if (description != null)
                        {
                            writer.WritePropertyName("description", escape: false);
                            writer.WriteValue(description);
                        }

                        writer.WritePropertyName("public", escape: false);
                        writer.WriteValue(isPublic);

                        writer.WritePropertyName("files", escape: false);
                        writer.WriteStartObject();

                        foreach (var (fileName, content) in contentByFileName)
                        {
                            writer.WritePropertyName(fileName);
                            writer.WriteStartObject();
                            writer.WritePropertyName("content", escape: false);
                            writer.WriteValue(content);
                            writer.WriteEndObject();
                        }

                        writer.WriteEndObject();

                        writer.WriteEndObject();
                    }
                },
                cancellationToken).ConfigureAwait(false)))
            {
                await reader.ReadStartObjectAsync(cancellationToken).ConfigureAwait(false);

                while (true)
                {
                    var propertyName = await reader.ReadPropertyNameUnlessEndClassAsync(cancellationToken).ConfigureAwait(false);
                    if (propertyName == "html_url") break;
                    if (propertyName is null) throw new InvalidDataException("Expected \"html_url\" property.");

                    await reader.SkipAsync(cancellationToken).ConfigureAwait(false);
                }

                return new Uri(await reader.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
            }
        }
    }
}
