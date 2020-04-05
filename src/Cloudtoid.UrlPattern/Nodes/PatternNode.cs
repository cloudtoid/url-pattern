namespace Cloudtoid.UrlPattern
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// This is the base class for all the pattern nodes
    /// </summary>
    internal abstract class PatternNode
    {
        [return: NotNullIfNotNull("left")]
        [return: NotNullIfNotNull("right")]
        public static PatternNode? operator +(PatternNode? left, PatternNode? right)
        {
            if (left is null)
                return right;

            if (right is null)
                return left;

            if (Equals(left, MatchNode.Empty))
                return right;

            if (Equals(right, MatchNode.Empty))
                return left;

            return new SequenceNode(left, right);
        }

        internal abstract void Accept(PatternNodeVisitor visitor);
    }
}
