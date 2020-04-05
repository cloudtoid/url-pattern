namespace Cloudtoid.UrlPattern
{
    /// <summary>
    /// Validates that each URL segment only includes a maximum of one variable
    /// </summary>
    internal sealed class OneVariablePerSegmentValidator : PatternValidatorBase
    {
        private bool fail;

        protected internal override void VisitVariable(VariableNode node)
        {
            if (fail)
                throw new PatternException("Each URL segment can only include a single variable definition.");

            fail = true;
        }

        protected internal override void VisitSegmentStart(SegmentStartNode node)
        {
            base.VisitSegmentStart(node);
            fail = false;
        }
    }
}
