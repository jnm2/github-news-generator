using System.Collections.Immutable;

namespace GitHubNewsGenerator
{
    public readonly struct GitHubNewsInfo
    {
        public GitHubNewsInfo(ImmutableArray<GitHubRelease> releases, ImmutableArray<GitHubPullRequest> pullRequests, ImmutableArray<GitHubDocsIssue> docsIssues)
        {
            Releases = releases;
            PullRequests = pullRequests;
            DocsIssues = docsIssues;
        }

        public ImmutableArray<GitHubRelease> Releases { get; }
        public ImmutableArray<GitHubPullRequest> PullRequests { get; }
        public ImmutableArray<GitHubDocsIssue> DocsIssues { get; }
    }
}
