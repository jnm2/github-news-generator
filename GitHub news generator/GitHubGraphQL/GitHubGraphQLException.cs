using System;
using System.Collections.Immutable;

namespace GitHubNewsGenerator.GitHubGraphQL
{
    public sealed class GitHubGraphQLException : Exception
    {
        public ImmutableArray<GitHubGraphQLError> Errors { get; }

        public GitHubGraphQLException(ImmutableArray<GitHubGraphQLError> errors)
            : base(string.Join(Environment.NewLine, errors))
        {
            Errors = errors;
        }
    }
}
