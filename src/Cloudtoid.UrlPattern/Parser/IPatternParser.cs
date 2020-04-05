namespace Cloudtoid.UrlPattern
{
    using System.Diagnostics.CodeAnalysis;

    internal interface IPatternParser
    {
        bool TryParse(
            string pattern,
            PatternCompilerErrorsSink errorsSink,
            [NotNullWhen(true)] out PatternNode? parsedPattern);
    }
}
