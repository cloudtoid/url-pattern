namespace Cloudtoid.UrlPattern
{
    internal interface IPatternTypeResolver
    {
        ResolvedPattern Resolve(string pattern);
    }
}
