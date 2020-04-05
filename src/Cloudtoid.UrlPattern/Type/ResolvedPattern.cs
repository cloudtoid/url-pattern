namespace Cloudtoid.UrlPattern
{
    internal sealed class ResolvedPattern
    {
        internal ResolvedPattern(string pattern, PatternType type)
        {
            Pattern = pattern;
            Type = type;
        }

        internal string Pattern { get; }

        internal PatternType Type { get; }
    }
}
