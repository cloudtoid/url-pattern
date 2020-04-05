namespace Cloudtoid.UrlPattern
{
    internal abstract class PatternValidatorBase : PatternNodeVisitor
    {
        internal void Validate(PatternNode pattern) => Visit(pattern);
    }
}
