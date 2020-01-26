using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using GitHubNewsGenerator.GitHubGraphQL;
using Newtonsoft.Json.Linq;

namespace GitHubNewsGenerator
{
    internal static class Queries
    {
        public static async Task<GitHubNewsInfo> GetNewsInfoAsync(string accessToken, string organization, string docsRepo, DateTime minDate)
        {
            using (var client = new GitHubGraphQLClient("GitHubGraphQLClient", accessToken))
            {
                var data = await client.GetDataAsync(
                    Properties.Resources.GitHubNewsQuery,
                    new Dictionary<string, object>
                    {
                        ["Org"] = organization,
                        ["DocsRepo"] = docsRepo
                    },
                    CancellationToken.None).ConfigureAwait(false);

                return ParseData(data, minDate);
            }
        }

        private static GitHubNewsInfo ParseData(JToken data, DateTime minDate)
        {
            var org = data["organization"];

            var releases = ImmutableArray.CreateBuilder<GitHubRelease>();
            var pullRequests = ImmutableArray.CreateBuilder<GitHubPullRequest>();
            var docsIssues = ImmutableArray.CreateBuilder<GitHubDocsIssue>();

            foreach (var repo in org["repositories"]["nodes"])
            {
                var repoName = (string)repo["name"];

                ParseReleases(minDate, releases, repo, repoName);
                ParsePullRequests(minDate, pullRequests, repo, repoName);
            }

            var docsRepo = org["repository"];

            ParseDocsIssues(minDate, docsIssues, docsRepo);

            return new GitHubNewsInfo(
                releases.ToImmutable(),
                pullRequests.ToImmutable(),
                docsIssues.ToImmutable());
        }

        private static void ParseReleases(DateTime minDate, ImmutableArray<GitHubRelease>.Builder builder, JToken repo, string repoName)
        {
            var foundOlder = false;

            var releases = (JArray)repo["releases"]["nodes"];
            foreach (var release in releases)
            {
                if (!release.HasValues) continue;

                var publishedAt = (DateTime)release["publishedAt"];
                if (publishedAt < minDate)
                {
                    foundOlder = true;
                    continue;
                }

                if ((bool)release["isDraft"]) continue;

                var author = release["author"];

                builder.Add(new GitHubRelease(
                    repoName,
                    (string)release["url"],
                    (string)release["name"],
                    (string)author["login"],
                    (string)author["name"],
                    publishedAt,
                    (bool)release["isPrerelease"]));
            }

            if (!foundOlder && releases.Count >= 2)
                throw new NotImplementedException("Potential missing data");
        }

        private static void ParsePullRequests(DateTime minDate, ImmutableArray<GitHubPullRequest>.Builder builder, JToken repo, string repoName)
        {
            var foundOlder = false;

            var pullRequests = (JArray)repo["pullRequests"]["nodes"];
            foreach (var pullRequest in pullRequests)
            {
                var mergedAt = (DateTime)pullRequest["mergedAt"];
                if (mergedAt < minDate)
                {
                    foundOlder = true;
                    continue;
                }

                var author = pullRequest["author"];

                builder.Add(new GitHubPullRequest(
                    repoName,
                    (string)pullRequest["url"],
                    (string)pullRequest["title"],
                    (string)author["login"],
                    (string)author["name"],
                    mergedAt));
            }

            if (!foundOlder && pullRequests.Count >= 100)
                throw new NotImplementedException("Potential missing data");
        }

        private static void ParseDocsIssues(DateTime minDate, ImmutableArray<GitHubDocsIssue>.Builder builder, JToken repo)
        {
            var foundOlder = false;

            var issues = (JArray)repo["issues"]["nodes"];
            foreach (var issue in issues)
            {
                var mergedAt = (DateTime)issue["closedAt"];
                if (mergedAt < minDate)
                {
                    foundOlder = true;
                    continue;
                }

                builder.Add(new GitHubDocsIssue(
                    (string)issue["url"],
                    (string)issue["title"],
                    mergedAt));
            }

            if (!foundOlder && issues.Count >= 100)
                throw new NotImplementedException("Potential missing data");
        }
    }
}
