namespace Cloudtoid.UrlPattern
{
    internal abstract class PatternNodeVisitor
    {
        protected virtual void Visit(PatternNode node)
        {
            node.Accept(this);
        }

        protected internal virtual void VisitLeaf(LeafNode node)
        {
        }

        protected internal virtual void VisitMatch(MatchNode node)
        {
            VisitLeaf(node);
        }

        protected internal virtual void VisitVariable(VariableNode node)
        {
            VisitLeaf(node);
        }

        protected internal virtual void VisitSegmentStart(SegmentStartNode node)
        {
            VisitLeaf(node);
        }

        protected internal virtual void VisitWildcard(WildcardNode node)
        {
            VisitLeaf(node);
        }

        protected internal virtual void VisitOptional(OptionalNode node)
        {
            Visit(node.Node);
        }

        protected internal virtual void VisitSequence(SequenceNode node)
        {
            foreach (var child in node.Nodes)
                Visit(child);
        }
    }
}
