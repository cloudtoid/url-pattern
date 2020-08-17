using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudtoid.UrlPattern.UnitTests
{
    [TestClass]
    public sealed class PatternValidatorTests
    {
        private readonly IPatternValidator validator;

        public PatternValidatorTests()
        {
            var services = new ServiceCollection().AddUrlPattern();
            var serviceProvider = services.BuildServiceProvider();
            validator = serviceProvider.GetRequiredService<IPatternValidator>();
        }

        [TestMethod]
        public void Validate_WhenConsecutiveSegmentStart_Fails()
        {
            Validate("a//b", $"Found consecutive '{Constants.Slash}' which is invalid.");
        }

        [TestMethod]
        public void Validate_WhenConsecutiveSegmentStartOneOptional_Fails()
        {
            Validate("a/(/)b", $"Found consecutive '{Constants.Slash}' which is invalid.");
        }

        [TestMethod]
        public void Validate_WhenConsecutiveWildcard_Fails()
        {
            Validate("**", $"Found consecutive '{Constants.Wildcard}' which is invalid.");
        }

        [TestMethod]
        public void Validate_WhenConsecutiveWildcardOneOptional_Fails()
        {
            Validate("*(*)", $"Found consecutive '{Constants.Wildcard}' which is invalid.");
        }

        [TestMethod]
        public void Validate_WhenTwoVariablesInSegment_Fails()
        {
            Validate(":var1:var2", "Each URL segment can only include a single variable definition.");
        }

        [TestMethod]
        public void Validate_WhenMultiSegmentsAndTwoVariablesInSegment_Fails()
        {
            Validate(":var0/:var1:var2", "Each URL segment can only include a single variable definition.");
        }

        [TestMethod]
        public void Validate_WhenMultiSegmentsButSingleVariableInEach_NoFailure()
        {
            Validate(":var0/:var1/:var2");
        }

        [TestMethod]
        public void Validate_WhenWildcardFollowsVariable_Fail()
        {
            Validate(":var0*/", $"The wild-card character '{Constants.Wildcard}' cannot not follow a variable.");
        }

        [TestMethod]
        public void Validate_WhenWildcardFollowsVariableButNotImmediately_NoFailure()
        {
            Validate(":var0/*");
        }

        [TestMethod]
        public void Validate_WhenDuplicateVariableName_Fail()
        {
            Validate(":var0/:var0", "The variable name 'var0' has already been used. Variable names must be unique.");
        }

        private void Validate(string pattern)
        {
            var parser = new PatternParser();
            var errorsSink = new PatternCompilerErrorsSink();
            parser.TryParse(pattern, errorsSink, out var parsedPattern).Should().BeTrue();
            parsedPattern.Should().NotBeNull();
            errorsSink.HasErrors.Should().BeFalse();

            validator.Validate(parsedPattern!, errorsSink).Should().BeTrue();
            errorsSink.HasErrors.Should().BeFalse();
        }

        private void Validate(string pattern, string errorContainsText)
        {
            var parser = new PatternParser();
            var errorsSink = new PatternCompilerErrorsSink();
            parser.TryParse(pattern, errorsSink, out var parsedPattern).Should().BeTrue();
            parsedPattern.Should().NotBeNull();
            errorsSink.HasErrors.Should().BeFalse();

            validator.Validate(parsedPattern!, errorsSink).Should().BeFalse();
            errorsSink.HasErrors.Should().BeTrue();
            errorsSink.Errors.Any(e => e.Message.ContainsOrdinalIgnoreCase(errorContainsText)).Should().BeTrue();
        }
    }
}
