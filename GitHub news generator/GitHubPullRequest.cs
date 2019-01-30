using System;
using System.Diagnostics;

namespace GitHubNewsGenerator
{
    [DebuggerDisplay("{Title} by {AuthorName,nq} ({Author,nq}) in {Repo,nq}")]
    public readonly struct GitHubPullRequest
    {
        public GitHubPullRequest(string repo, string url, string title, string author, string authorName, DateTime mergedAt)
        {
            Repo = repo;
            Url = url;
            Title = title;
            Author = author;
            AuthorName = authorName;
            MergedAt = mergedAt;
        }

        public string Repo { get; }
        public string Url { get; }
        public string Title { get; }
        public string Author { get; }
        public string AuthorName { get; }
        public DateTime MergedAt { get; }
    }
}
