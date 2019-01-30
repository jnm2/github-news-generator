using System;
using System.Diagnostics;

namespace GitHubNewsGenerator
{
    [DebuggerDisplay("{Title}")]
    public readonly struct GitHubDocsIssue
    {
        public GitHubDocsIssue(string url, string title, DateTime closedAt)
        {
            Url = url;
            Title = title;
            ClosedAt = closedAt;
        }

        public string Url { get; }
        public string Title { get; }
        public DateTime ClosedAt { get; }
    }
}
