namespace Cloudtoid.UrlPattern
{
    using System;
    using System.Text.RegularExpressions;

    internal static class RegexFactory
    {
        private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(1);
        private static readonly RegexOptions Options =
            RegexOptions.IgnoreCase
            | RegexOptions.Singleline
            | RegexOptions.ExplicitCapture
            | RegexOptions.Compiled
            | RegexOptions.CultureInvariant;

        internal static Regex Create(string pattern)
            => new Regex(pattern, Options, MatchTimeout);
    }
}
