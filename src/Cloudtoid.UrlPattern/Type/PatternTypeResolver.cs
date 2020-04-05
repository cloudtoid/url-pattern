namespace Cloudtoid.UrlPattern
{
    using static Contract;

    internal sealed class PatternTypeResolver : IPatternTypeResolver
    {
        private const string Exact = "exact: ";
        private const string Prefix = "prefix: ";
        private const string Regex = "regex: ";

        public ResolvedPattern Resolve(string pattern)
        {
            CheckValue(pattern, nameof(pattern));

            if (pattern.StartsWithOrdinalIgnoreCase(Exact))
                return new ResolvedPattern(pattern.Substring(Exact.Length), PatternType.ExactMatch);

            if (pattern.StartsWithOrdinalIgnoreCase(Regex))
                return new ResolvedPattern(pattern.Substring(Regex.Length), PatternType.Regex);

            if (pattern.StartsWithOrdinalIgnoreCase(Prefix))
                return new ResolvedPattern(pattern.Substring(Prefix.Length), PatternType.PrefixMatch);

            return new ResolvedPattern(pattern, PatternType.PrefixMatch);
        }
    }
}
