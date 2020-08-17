using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Cloudtoid.UrlPattern
{
    public interface IPatternMatcher
    {
        /// <summary>
        /// Matches the <paramref name="path"/> against the <paramref name="compiledPattern"/>.
        /// If a match could not be made, <paramref name="why"/> specifies the reason for it. It is typically because
        /// <paramref name="path"/> is not a match, but sometimes it could be that the processing timed out.
        /// </summary>
        bool TryMatch(
            CompiledPattern compiledPattern,
            PathString path,
            [NotNullWhen(true)] out PatternMatchResult? match,
            [NotNullWhen(false)] out string? why);
    }
}
