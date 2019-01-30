using System;

namespace GitHubNewsGenerator
{
    public readonly struct GitHubRelease
    {
        public GitHubRelease(string repo, string url, string name, string author, string authorName, DateTime publishedAt, bool isPrerelease)
        {
            Repo = repo;
            Url = url;
            Name = name;
            Author = author;
            AuthorName = authorName;
            PublishedAt = publishedAt;
            IsPrerelease = isPrerelease;
        }

        public string Repo { get; }
        public string Url { get; }
        public string Name { get; }
        public string Author { get; }
        public string AuthorName { get; }
        public DateTime PublishedAt { get; }
        public bool IsPrerelease { get; }
    }
}
