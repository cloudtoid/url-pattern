namespace Cloudtoid.UrlPattern
{
    /// <summary>
    /// Validates that the wild-card character <see cref="Constants.Wildcard"/> cannot not follow a variable.
    /// </summary>
    internal sealed class NoVariableFollowedByWildcardValidator : PatternValidatorBase
    {
        private bool fail;

        protected internal override void VisitLeaf(LeafNode node)
        {
            base.VisitLeaf(node);
            fail = false;
        }

        protected internal override void VisitVariable(VariableNode node)
            => fail = true;

        protected internal override void VisitWildcard(WildcardNode node)
        {
            if (fail)
                throw new PatternException($"The wild-card character '{Constants.Wildcard}' cannot not follow a variable.");

            base.VisitWildcard(node);
        }
    }
}
