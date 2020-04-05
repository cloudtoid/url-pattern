namespace Cloudtoid.UrlPattern
{
    internal interface IPatternValidator
    {
        bool Validate(
            PatternNode pattern,
            PatternCompilerErrorsSink errorsSink);
    }
}
