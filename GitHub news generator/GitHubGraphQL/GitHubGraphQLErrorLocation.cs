using System.Diagnostics;

namespace GitHubNewsGenerator.GitHubGraphQL
{
    [DebuggerDisplay("Line = {Line}, Column = {Column}")]
    public readonly struct GitHubGraphQLErrorLocation
    {
        public GitHubGraphQLErrorLocation(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public int Line { get; }
        public int Column { get; }

        public override string ToString() => $"[{Line}, {Column}]";
    }
}
