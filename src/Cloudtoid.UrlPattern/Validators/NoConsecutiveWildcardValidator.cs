namespace Cloudtoid.UrlPattern
{
    /// <summary>
    /// Validates that there are no consecutive <see cref="Constants.Wildcard"/> characters.
    /// </summary>
    internal sealed class NoConsecutiveWildcardValidator : PatternValidatorBase
    {
        private bool fail;

        protected internal override void VisitLeaf(LeafNode node)
        {
            base.VisitLeaf(node);
            fail = false;
        }

        protected internal override void VisitWildcard(WildcardNode node)
        {
            if (fail)
                throw new PatternException($"Found consecutive '{Constants.Wildcard}' which is invalid.");

            fail = true;
        }
    }
}
