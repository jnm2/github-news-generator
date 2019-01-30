using System;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubNewsGenerator
{
    public static class Program
    {
        public static async Task Main()
        {
            Console.Write("GitHub access token (needs public_repo): ");
            var accessToken = Console.ReadLine();

            var info = await Queries.GetNewsInfoAsync(
                accessToken,
                organization: "nunit",
                docsRepo: "docs",
                minDate: new DateTime(2018, 12, 31));

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("## Releases (also check VS extension gallery)");
            Console.WriteLine();

            if (info.Releases.Any())
            {
                foreach (var release in info.Releases.OrderBy(r => r.PublishedAt))
                {
                    Console.WriteLine($"- **{release.Repo}** '{release.Name}'" + (release.IsPrerelease ? " (prerelease)" : null) + $" by {release.AuthorName ?? release.Author}");
                    Console.WriteLine($"  Published at {release.PublishedAt}");
                    Console.WriteLine($"  {release.Url}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("(none)");
            }

            Console.WriteLine();
            Console.WriteLine("## Merged pull requests");
            Console.WriteLine();

            if (info.PullRequests.Any())
            {
                foreach (var pullRequest in info.PullRequests.OrderBy(r => r.MergedAt))
                {
                    Console.WriteLine($"- **{pullRequest.Repo}** '{pullRequest.Title}' by {pullRequest.AuthorName ?? pullRequest.Author}");
                    Console.WriteLine($"  Merged at {pullRequest.MergedAt}");
                    Console.WriteLine($"  {pullRequest.Url}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("(none)");
            }

            Console.WriteLine();
            Console.WriteLine("## Closed docs issues");
            Console.WriteLine();

            if (info.DocsIssues.Any())
            {
                foreach (var issue in info.DocsIssues.OrderBy(r => r.ClosedAt))
                {
                    Console.WriteLine($"- '{issue.Title}'");
                    Console.WriteLine($"  Closed at {issue.ClosedAt}");
                    Console.WriteLine($"  {issue.Url}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("(none)");
            }
        }
    }
}
