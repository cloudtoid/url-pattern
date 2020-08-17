using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudtoid.UrlPattern.UnitTests
{
    [TestClass]
    public sealed class PatternParserTests
    {
        [TestMethod]
        public void TryParse_WhenEmptyRoute_ReturnsEmptyMatchAndNoError()
        {
            var pattern = ParseAndValidate(string.Empty);
            pattern.Should().Be(MatchNode.Empty);
        }

        [TestMethod]
        public void TryParse_WhenValidSingleCharMatchRoute_ReturnsMatchNodeAndNoError()
        {
            var pattern = ParseAndValidate("a");
            pattern.Should().BeEquivalentTo(
                new MatchNode("a"),
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenValidLongMatchRoute_ReturnsMatchNodeAndNoError()
        {
            var pattern = ParseAndValidate("valid-value");
            pattern.Should().BeEquivalentTo(
                new MatchNode("valid-value"),
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenSegmentChar_ReturnsSegmentNode()
        {
            var pattern = ParseAndValidate("/");
            pattern.Should().BeEquivalentTo(
                SegmentStartNode.Instance,
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenWildcardChar_ReturnsWildcardNode()
        {
            var pattern = ParseAndValidate("*");
            pattern.Should().BeEquivalentTo(
                WildcardNode.Instance,
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenOptionalMatch_ReturnsOptionalWithMatchNode()
        {
            var pattern = ParseAndValidate("(value)");
            pattern.Should().BeEquivalentTo(
                new OptionalNode(new MatchNode("value")),
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenOptionalWild_ReturnsOptionalWithWildNode()
        {
            var pattern = ParseAndValidate("(*)");
            pattern.Should().BeEquivalentTo(
                new OptionalNode(WildcardNode.Instance),
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenEmptyOptional_ReturnsNullPatternAndError()
        {
            ParseAndValidate("()", 1, "empty or invalid", 1);
        }

        [TestMethod]
        public void TryParse_WhenOptionalStartOnly_ReturnsNullPatternAndError()
        {
            ParseAndValidate("(", 2, "There is a missing ')'");
        }

        [TestMethod]
        public void TryParse_WhenOptionalEndOnly_ReturnsNullPatternAndError()
        {
            ParseAndValidate(")", 1, "There is an unexpected ')'", 0);
        }

        [TestMethod]
        public void TryParse_WhenVariableNmeStartsWithNumber_Fails()
        {
            ParseAndValidate(":0variable", 1, "There is a variable with an empty or invalid name.", 1);
        }

        [TestMethod]
        public void TryParse_WhenVariableNmeStartsWithSpace_Fails()
        {
            ParseAndValidate(": variable", 1, "There is a variable with an empty or invalid name.", 1);
        }

        [TestMethod]
        public void TryParse_WhenSimpleVariable_ReturnsVariableNode()
        {
            var pattern = ParseAndValidate(":variable");
            pattern.Should().BeEquivalentTo(
                new VariableNode("variable"),
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenVariableWithNumber_ReturnsVariableNode()
        {
            var pattern = ParseAndValidate(":variable0");
            pattern.Should().BeEquivalentTo(
                new VariableNode("variable0"),
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenVariableNameStops_ReturnsVariableNodeAndMatch()
        {
            var pattern = ParseAndValidate(":variable-placeholder");
            pattern.Should().BeEquivalentTo(
                new VariableNode("variable") + new MatchNode("-placeholder"),
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenEmptyVariable_ReturnsNullPatternAndError()
        {
            ParseAndValidate(":-----", 1, "invalid name", 1);
        }

        [TestMethod]
        public void TryParse_WhenSegmentIsVariable_Success()
        {
            var pattern = ParseAndValidate("/api/v:version/");

            pattern.Should().BeEquivalentTo(
                new SequenceNode(
                    SegmentStartNode.Instance,
                    new MatchNode("api"),
                    SegmentStartNode.Instance,
                    new MatchNode("v"),
                    new VariableNode("version"),
                    SegmentStartNode.Instance),
                o => o.RespectingRuntimeTypes());

            pattern.Should().BeEquivalentTo(
                SegmentStartNode.Instance
                + new MatchNode("api")
                + SegmentStartNode.Instance
                + new MatchNode("v")
                + new VariableNode("version")
                + SegmentStartNode.Instance,
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenSegmentIsVariableExtended_Success()
        {
            var pattern = ParseAndValidate("/api/v:version");

            pattern.Should().BeEquivalentTo(
                new SequenceNode(
                    SegmentStartNode.Instance,
                    new MatchNode("api"),
                    SegmentStartNode.Instance,
                    new MatchNode("v"),
                    new VariableNode("version")),
                o => o.RespectingRuntimeTypes());

            pattern.Should().BeEquivalentTo(
                SegmentStartNode.Instance
                + new MatchNode("api")
                + SegmentStartNode.Instance
                + new MatchNode("v")
                + new VariableNode("version"),
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenMultipleVariables_Success()
        {
            var pattern = ParseAndValidate("/api/v:version/product/:id/");

            pattern.Should().BeEquivalentTo(
                SegmentStartNode.Instance
                + new MatchNode("api")
                + SegmentStartNode.Instance
                + new MatchNode("v")
                + new VariableNode("version")
                + SegmentStartNode.Instance
                + new MatchNode("product")
                + SegmentStartNode.Instance
                + new VariableNode("id")
                + SegmentStartNode.Instance,
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenMultipleVariablesExtended_Success()
        {
            var pattern = ParseAndValidate("/api/v:version/product/:id");

            pattern.Should().BeEquivalentTo(
                SegmentStartNode.Instance
                + new MatchNode("api")
                + SegmentStartNode.Instance
                + new MatchNode("v")
                + new VariableNode("version")
                + SegmentStartNode.Instance
                + new MatchNode("product")
                + SegmentStartNode.Instance
                + new VariableNode("id"),
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenOptionalSegment_Success()
        {
            var pattern = ParseAndValidate("/api(/v1.0)/product/:id");

            pattern.Should().BeEquivalentTo(
                SegmentStartNode.Instance
                + new MatchNode("api")
                + new OptionalNode((SegmentStartNode.Instance + new MatchNode("v1.0"))!)
                + SegmentStartNode.Instance
                + new MatchNode("product")
                + SegmentStartNode.Instance
                + new VariableNode("id"),
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenMultipleOptionlSegmentsWithVariables_Success()
        {
            var pattern = ParseAndValidate("/api(/v:version)/product(/:id)");

            pattern.Should().BeEquivalentTo(
                SegmentStartNode.Instance
                + new MatchNode("api")
                + new OptionalNode((SegmentStartNode.Instance + new MatchNode("v") + new VariableNode("version"))!)
                + SegmentStartNode.Instance
                + new MatchNode("product")
                + new OptionalNode((SegmentStartNode.Instance + new VariableNode("id"))!),
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenMultipleOptionlSegmentsWithVariablesExtended_Success()
        {
            var pattern = ParseAndValidate("/api(/v:version)/product/(:id)");

            pattern.Should().BeEquivalentTo(
                SegmentStartNode.Instance
                + new MatchNode("api")
                + new OptionalNode((SegmentStartNode.Instance + new MatchNode("v") + new VariableNode("version"))!)
                + SegmentStartNode.Instance
                + new MatchNode("product")
                + SegmentStartNode.Instance
                + new OptionalNode(new VariableNode("id")),
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenComplexOptional_Success()
        {
            var pattern = ParseAndValidate("/(api/v:version/)product/");

            pattern.Should().BeEquivalentTo(
                SegmentStartNode.Instance
                + new OptionalNode(
                    (new MatchNode("api")
                    + SegmentStartNode.Instance
                    + new MatchNode("v")
                    + new VariableNode("version")
                    + SegmentStartNode.Instance)!)
                + new MatchNode("product")
                + SegmentStartNode.Instance,
                o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenUsingEscapeCharButNotFollowedByEscapableChar_EscapeCharsShouldBeMatched()
        {
            var values = new[]
            {
                @"\",
                @"\\",
                @"\\a",
                @"\\value",
                @"value\",
                @"value\\",
                @"value\\a",
                @"some\\value",
                @"\\a\",
                @"\\a\\",
                @"\\a\\b",
                @"\\some\\value"
            };

            foreach (var value in values)
                ExpectMatch(value);

            values = new[]
            {
                @":variable\",
                @":variable\\",
                @":variable\\a",
                @":variable\\value",
                @":variable-value\",
                @":variable-value\\",
                @":variable-value\\a",
                @":variable-some\\value",
                @":variable\\a\",
                @":variable\\a\\",
                @":variable\\a\\b",
                @":variable\\some\\value",
                @"\:variable",
                @"\\a:variable",
                @"\\value:variable",
                @"-value\:variable",
                @"-value\\a:variable",
                @"-some\\value:variable",
                @"\\a\:variable",
                @"\\a\\b:variable",
                @"\\some\\value:variable"
            };

            foreach (var value in values)
                ExpectVariableAndMatch(value);

            values = new[]
            {
                @"\:variable",
                @"\\a:variable",
                @"\\value:variable",
                @"-value\:variable",
                @"-value\\a:variable",
                @"-some\\value:variable",
                @"\\a\:variable",
                @"\\a\\b:variable",
                @"\\some\\value:variable"
            };

            foreach (var value in values)
                ExpectEndVariableAndMatch(value);
        }

        private static void ExpectMatch(string value)
        {
            var pattern = ParseAndValidate(value);
            pattern.Should().BeEquivalentTo(new MatchNode(value), o => o.RespectingRuntimeTypes());
        }

        private static void ExpectVariableAndMatch(string value)
        {
            var pattern = ParseAndValidate(value);
            pattern.Should()
                .BeEquivalentTo(
                    new VariableNode("variable") + new MatchNode(value.ReplaceOrdinal(":variable", string.Empty)),
                    o => o.RespectingRuntimeTypes());
        }

        private static void ExpectEndVariableAndMatch(string value)
        {
            var pattern = ParseAndValidate(value);
            pattern.Should()
                    .BeEquivalentTo(
                        new MatchNode(value.ReplaceOrdinal(":variable", string.Empty)) + new VariableNode("variable"),
                        o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenEscapedAtBegining_SpecialCharIsEscaped()
        {
            var values = new[]
            {
                @"\\:var",
                @"\\*",
                @"\\(",
                @"\\)",
                @"\\*value",
                @"\\(value",
                @"\\)value",
            };

            foreach (var value in values)
                ExpectEscapedAtBeginingMatch(value);
        }

        private static void ExpectEscapedAtBeginingMatch(string value)
        {
            var pattern = ParseAndValidate(value);
            pattern.Should().BeEquivalentTo(new MatchNode(value.Substring(2)), o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenEscapedInMiddle_SpecialCharIsEscaped()
        {
            var values = new[]
            {
                @"a\\:var",
                @"a\\*",
                @"a\\(",
                @"a\\)",
                @"a\\*value",
                @"a\\(value",
                @"a\\)value",
            };

            foreach (var value in values)
                ExpectEscapedInMiddleMatch(value);
        }

        private static void ExpectEscapedInMiddleMatch(string value)
        {
            var pattern = ParseAndValidate(value);
            pattern.Should().BeEquivalentTo(new MatchNode("a") + new MatchNode(value.Substring(3)), o => o.RespectingRuntimeTypes());
        }

        [TestMethod]
        public void TryParse_WhenWildcardPlusMatchPlusVariableVariableNameStops_Success()
        {
            var pattern = ParseAndValidate("*placeholder:variable/");
            pattern.Should().BeEquivalentTo(
                WildcardNode.Instance
                + new MatchNode("placeholder")
                + new VariableNode("variable")
                + SegmentStartNode.Instance,
                o => o.RespectingRuntimeTypes());
        }

        private static PatternNode ParseAndValidate(string pattern)
        {
            var parser = new PatternParser();
            var errorsSink = new PatternCompilerErrorsSink();
            parser.TryParse(pattern, errorsSink, out var parsedPattern).Should().BeTrue();
            errorsSink.Errors.Should().HaveCount(0);
            errorsSink.HasErrors.Should().BeFalse();
            parsedPattern.Should().NotBeNull();
            return parsedPattern!;
        }

        private static void ParseAndValidate(
            string pattern,
            int count,
            string messageContains,
            int? location = null)
        {
            var parser = new PatternParser();
            var errorsSink = new PatternCompilerErrorsSink();
            parser.TryParse(pattern, errorsSink, out var parsedPattern).Should().BeFalse();

            parsedPattern.Should().BeNull();
            errorsSink.HasErrors.Should().BeTrue();
            errorsSink.Errors.Should().HaveCount(count);
            var error = errorsSink.Errors[0];
            error.Location.Should().Be(location);
            error.Message.Should().Contain(messageContains);
        }

        // /api/v:version/
        // /api/v:version/product/:id/
        // /api/v:version/product/:id
        // /api(/v1.0)/product/:id
        // /api(/v:version)/product(/:id)
        // /api(/v:version)/product/(:id)
        // /(api/v:version/)product/
        // \
        // \\
        // \\a
        // \\value
        // value\
        // value\\
        // value\\a
        // some\\value
        // \\a\
        // \\a\\
        // \\a\\b
        // \\some\\value
    }
}
