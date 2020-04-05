namespace Cloudtoid.UrlPattern
{
    using static Contract;

    /// <summary>
    /// This node represents a full text match.
    /// </summary>
    internal sealed class MatchNode : LeafNode
    {
        private MatchNode()
        {
            Value = string.Empty;
        }

        internal MatchNode(string value)
        {
            Value = CheckNonEmpty(value, nameof(value));
        }

        public string Value { get; }

        internal static MatchNode Empty { get; } = new MatchNode();

        public override string ToString() => Value;

        internal override void Accept(PatternNodeVisitor visitor)
            => visitor.VisitMatch(this);
    }
}
