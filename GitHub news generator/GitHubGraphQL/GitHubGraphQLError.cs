using System.Collections.Immutable;
using System.Diagnostics;

namespace GitHubNewsGenerator.GitHubGraphQL
{
    [DebuggerDisplay("{ToString(),nq}")]
    public readonly struct GitHubGraphQLError
    {
        public GitHubGraphQLError(string message, ImmutableArray<GitHubGraphQLErrorLocation> locations)
        {
            Message = message;
            Locations = locations;
        }

        public string Message { get; }
        public ImmutableArray<GitHubGraphQLErrorLocation> Locations { get; }

        public override string ToString() => Message;
    }
}
