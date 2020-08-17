using System.Diagnostics.CodeAnalysis;

namespace Cloudtoid.UrlPattern
{
    internal interface IPatternParser
    {
        bool TryParse(
            string pattern,
            PatternCompilerErrorsSink errorsSink,
            [NotNullWhen(true)] out PatternNode? parsedPattern);
    }
}
