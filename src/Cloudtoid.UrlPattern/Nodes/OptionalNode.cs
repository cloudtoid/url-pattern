namespace Cloudtoid.UrlPattern
{
    using static Contract;

    /// <summary>
    /// This node wraps other nodes and turns them optional
    /// </summary>
    internal sealed class OptionalNode : PatternNode
    {
        internal OptionalNode(PatternNode node)
        {
            CheckValue(node, nameof(node));

            Node = node is OptionalNode optionalNode
                ? optionalNode.Node
                : node;
        }

        public PatternNode Node { get; }

        public override string ToString() => $"({Node})";

        internal override void Accept(PatternNodeVisitor visitor)
            => visitor.VisitOptional(this);
    }
}
