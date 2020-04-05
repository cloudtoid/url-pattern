namespace Cloudtoid.UrlPattern.UnitTests
{
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PatternTypeResolverTests
    {
        private readonly IPatternTypeResolver resolver;

        public PatternTypeResolverTests()
        {
            var services = new ServiceCollection().AddUrlPattern();
            var serviceProvider = services.BuildServiceProvider();
            resolver = serviceProvider.GetRequiredService<IPatternTypeResolver>();
        }

        [TestMethod]
        public void TypeResolutionTests()
        {
            Validate("default", "default", PatternType.PrefixMatch);
            Validate("prefix: /product/", "/product/", PatternType.PrefixMatch);
            Validate("exact: /product/", "/product/", PatternType.ExactMatch);
            Validate("regex: product", "product", PatternType.Regex);
            Validate("REGEX: product", "product", PatternType.Regex);
            Validate("regex:product", "regex:product", PatternType.PrefixMatch);
        }

        private void Validate(string pattern, string expectedPattern, PatternType expectedType)
        {
            var result = resolver.Resolve(pattern);
            result.Pattern.Should().Be(expectedPattern);
            result.Type.Should().Be(expectedType);
        }
    }
}
