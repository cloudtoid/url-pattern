using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudtoid.UrlPattern.UnitTests
{
    [TestClass]
    public class PatternCompilerTests
    {
        private readonly IPatternCompiler compiler;

        public PatternCompilerTests()
        {
            var services = new ServiceCollection().AddUrlPattern();
            var serviceProvider = services.BuildServiceProvider();
            compiler = serviceProvider.GetRequiredService<IPatternCompiler>();
        }

        [TestMethod]
        public void CompileExactMatch()
        {
            CompileAndValidate("exact: /product/", @"\A/product/$", PatternType.ExactMatch);
        }

        [TestMethod]
        public void CompileImplicitPrefixMatch()
        {
            CompileAndValidate("/product/", @"\A/product/", PatternType.PrefixMatch);
        }

        [TestMethod]
        public void CompileExplicitPrefixMatch()
        {
            CompileAndValidate("prefix: /product/", @"\A/product/", PatternType.PrefixMatch);
        }

        [TestMethod]
        public void CompileRegexMatch()
        {
            CompileAndValidate(@"regex: \A/product(/)?", @"\A/product(/)?", PatternType.Regex);
        }

        [TestMethod]
        public void CompileRegexWithVariableMatch()
        {
            CompileAndValidate(@"regex: \A/product(/(?<id>.+))?", @"\A/product(/(?<id>.+))?", PatternType.Regex, "id");
        }

        [TestMethod]
        public void CompileWithParseError()
        {
            CompileAndVerifyErrors(
                "/: variable",
                @"(location:2) There is a variable with an empty or invalid name. The valid characters are 'a-zA-Z0-9_' and the first character cannot be a number.");
        }

        [TestMethod]
        public void CompileWithValidationErrorError()
        {
            CompileAndVerifyErrors(
                "/:id/:id",
                @"The variable name 'id' has already been used. Variable names must be unique.");
        }

        private void CompileAndVerifyErrors(string pattern, params string[] expectedErrors)
        {
            compiler.TryCompile(pattern, out var compiledPattern, out var errors).Should().BeFalse();
            compiledPattern.Should().BeNull();
            errors.Should().NotBeNull();
            errors!.Select(i => i.ToString()).Should().BeEquivalentTo(expectedErrors);
        }

        private void CompileAndValidate(string pattern, string expectedRegex, PatternType expectedType, params string[] variables)
        {
            compiler.TryCompile(pattern, out var compiledPattern, out var errors).Should().BeTrue();
            errors.Should().BeNull();
            compiledPattern.Should().NotBeNull();
            compiledPattern!.Type.Should().Be(expectedType);
            compiledPattern.Regex.ToString().Should().Be(expectedRegex);
            compiledPattern.VariableNames.Should().BeEquivalentTo(variables);
        }
    }
}
