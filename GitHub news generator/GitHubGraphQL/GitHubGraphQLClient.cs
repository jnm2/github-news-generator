using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GitHubNewsGenerator.GitHubGraphQL
{
    public sealed class GitHubGraphQLClient : GitHubClient
    {
        public GitHubGraphQLClient(string userAgent, string accessToken)
            : base(baseAddress: "https://api.github.com/graphql", userAgent, accessToken)
        {
        }

        public Task<StreamReader> GetStreamReaderAsync(string query, CancellationToken cancellationToken)
        {
            return GetStreamReaderAsync(query, variables: null, cancellationToken);
        }

        public async Task<StreamReader> GetStreamReaderAsync(string query, IReadOnlyDictionary<string, object> variables, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query must be specified.", nameof(query));

            return await PostAsync(
                buffer => WriteBody(buffer, query, variables),
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<JToken> GetDataAsync(string query, IReadOnlyDictionary<string, object> variables, CancellationToken cancellationToken)
        {
            var streamReader = await GetStreamReaderAsync(query, variables, cancellationToken).ConfigureAwait(false);

            var jsonReader = new JsonTextReader(streamReader);

            await jsonReader.ReadStartObjectAsync(cancellationToken).ConfigureAwait(false);

            var data = (JToken)null;

            while (true)
            {
                switch (await jsonReader.ReadPropertyNameUnlessEndClassAsync(cancellationToken).ConfigureAwait(false))
                {
                    case null:
                        return data ?? throw new InvalidDataException("No top-level \"data\" or \"errors\" property found.");
                    case "data":
                        await jsonReader.ReadAsync(cancellationToken).ConfigureAwait(false);
                        data = await JToken.LoadAsync(jsonReader, cancellationToken).ConfigureAwait(false);
                        break;
                    case "errors":
                        await jsonReader.ReadAsync(cancellationToken).ConfigureAwait(false);
                        var errors = await JToken.LoadAsync(jsonReader, cancellationToken).ConfigureAwait(false);

                        throw new GitHubGraphQLException(new JsonSerializer().Deserialize<ImmutableArray<GitHubGraphQLError>>(errors.CreateReader()));
                    default:
                        await jsonReader.SkipAsync(cancellationToken).ConfigureAwait(false);
                        break;
                }
            }
        }

        private static void WriteBody(TextWriter writer, string query, IReadOnlyDictionary<string, object> variables)
        {
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("query", escape: false);
                jsonWriter.WriteValue(query);

                if (variables != null && variables.Any())
                {
                    jsonWriter.WritePropertyName("variables", escape: false);
                    new JsonSerializer().Serialize(jsonWriter, variables);
                }

                jsonWriter.WriteEndObject();
            }
        }
    }
}
