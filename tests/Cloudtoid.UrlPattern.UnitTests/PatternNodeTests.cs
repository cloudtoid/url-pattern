using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudtoid.UrlPattern.UnitTests
{
    [TestClass]
    public class PatternNodeTests
    {
        [TestMethod]
        public void ToStringTest()
        {
            var pattern = new SequenceNode(
                SegmentStartNode.Instance,
                new MatchNode("category"),
                SegmentStartNode.Instance,
                new VariableNode("category"),
                SegmentStartNode.Instance,
                new OptionalNode((new MatchNode("p_") + WildcardNode.Instance)!));

            pattern.ToString().Should().Be("/category/:category/(p_*)");
        }

        [TestMethod]
        public void AdditionOperatorTests()
        {
            var node = null + (PatternNode?)null;
            node.Should().BeNull();

            node = WildcardNode.Instance + null;
            node.Should().Be(WildcardNode.Instance);

            node = WildcardNode.Instance + MatchNode.Empty;
            node.Should().Be(WildcardNode.Instance);

            node = MatchNode.Empty + WildcardNode.Instance;
            node.Should().Be(WildcardNode.Instance);

            node = SegmentStartNode.Instance + WildcardNode.Instance;
            node.Should().BeEquivalentTo(
                new SequenceNode(
                    SegmentStartNode.Instance,
                    WildcardNode.Instance));
        }

        [TestMethod]
        public void SequenceInSequenceTests()
        {
            var node = new SequenceNode(
                new SequenceNode(SegmentStartNode.Instance, WildcardNode.Instance),
                new SequenceNode(SegmentStartNode.Instance, WildcardNode.Instance));

            node.Should().BeEquivalentTo(
                new SequenceNode(
                    SegmentStartNode.Instance,
                    WildcardNode.Instance,
                    SegmentStartNode.Instance,
                    WildcardNode.Instance));
        }

        [TestMethod]
        public void OptionalInOptionalTests()
        {
            var node = new OptionalNode(
                new OptionalNode(SegmentStartNode.Instance));

            node.Should().BeEquivalentTo(
                new OptionalNode(
                    SegmentStartNode.Instance));
        }
    }
}
