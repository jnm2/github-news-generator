using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubNewsGenerator
{
    public static class Program
    {
        public static async Task Main()
        {
            var minDate = new DateTime(2019, 01, 31);

            Console.WriteLine($"Starting date: {minDate:d}");
            Console.Write("GitHub access token (needs public_repo): ");
            var accessToken = Console.ReadLine();

            Console.WriteLine();
            Console.Write("Querying GitHub...");

            var today = DateTime.Today;
            var info = await Queries.GetNewsInfoAsync(accessToken, organization: "nunit", docsRepo: "docs", minDate);

            Console.WriteLine(" done.");
            Console.WriteLine();

            var title = $"NUnit GitHub Activity {minDate:MMM d} to {today:MMM d}";

            switch (ConsoleUtils.Choose("Would you like the results to go to the [c]lipboard, a [f]ile, or [v]iew them here?"))
            {
                case 'c':
                    var builder = new StringWriter();
                    FormatNewsInfo(builder, title, info);
                    Clipboard.SetText(builder.ToString());

                    Console.WriteLine("Results copied to the clipboard.");
                    break;

                case 'f':
                    var filename = title + ".md";
                    using (var file = File.CreateText(filename))
                        FormatNewsInfo(file, title, info);

                    Console.WriteLine($"Results saved to file {filename} in the current directory.");
                    break;

                case 'v':
                    Console.WriteLine();
                    Console.WriteLine();
                    FormatNewsInfo(Console.Out, title, info);
                    break;
            }
        }

        private static void FormatNewsInfo(TextWriter writer, string title, GitHubNewsInfo info)
        {
            writer.WriteLine("# " + title);
            writer.WriteLine();

            writer.WriteLine("## Releases (also check VS extension gallery)");
            writer.WriteLine();

            if (info.Releases.Any())
            {
                foreach (var release in info.Releases.OrderBy(r => r.PublishedAt))
                {
                    writer.WriteLine($"- **{release.Repo}** '{release.Name}'" + (release.IsPrerelease ? " (prerelease)" : null) + $" by {release.AuthorName ?? release.Author}  ");
                    writer.WriteLine($"  Published at {release.PublishedAt}  ");
                    writer.WriteLine($"  {release.Url}");
                    writer.WriteLine();
                }
            }
            else
            {
                writer.WriteLine("(none)");
            }

            writer.WriteLine();
            writer.WriteLine("## Merged pull requests");
            writer.WriteLine();

            if (info.PullRequests.Any())
            {
                foreach (var pullRequest in info.PullRequests.OrderBy(r => r.MergedAt))
                {
                    writer.WriteLine($"- **{pullRequest.Repo}** '{pullRequest.Title}' by {pullRequest.AuthorName ?? pullRequest.Author}  ");
                    writer.WriteLine($"  Merged at {pullRequest.MergedAt}  ");
                    writer.WriteLine($"  {pullRequest.Url}");
                    writer.WriteLine();
                }
            }
            else
            {
                writer.WriteLine("(none)");
            }

            writer.WriteLine();
            writer.WriteLine("## Closed docs issues");
            writer.WriteLine();

            if (info.DocsIssues.Any())
            {
                foreach (var issue in info.DocsIssues.OrderBy(r => r.ClosedAt))
                {
                    writer.WriteLine($"- '{issue.Title}'  ");
                    writer.WriteLine($"  Closed at {issue.ClosedAt}  ");
                    writer.WriteLine($"  {issue.Url}");
                    writer.WriteLine();
                }
            }
            else
            {
                writer.WriteLine("(none)");
            }
        }
    }
}
