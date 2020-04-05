namespace Cloudtoid.UrlPattern
{
    /// <summary>
    /// Represents a forward slash in the pattern
    /// </summary>
    internal sealed class SegmentStartNode : LeafNode
    {
        private static readonly string Value = Constants.Slash.ToString();

        private SegmentStartNode()
        {
        }

        internal static SegmentStartNode Instance { get; } = new SegmentStartNode();

        public override string ToString() => Value;

        internal override void Accept(PatternNodeVisitor visitor)
            => visitor.VisitSegmentStart(this);
    }
}
