using System.Text;
using System.Text.RegularExpressions;

namespace Cloudtoid.UrlPattern
{
    internal sealed class PatternRegexBuilder : PatternNodeVisitor
    {
        private static readonly string SegmentStart = Regex.Escape(@"/");
        private static readonly string Wildcard = $"[^{SegmentStart}]*";  // [^/]*
        private static readonly string Start = @"\A";
        private readonly StringBuilder builder = new StringBuilder(Start);

        private PatternRegexBuilder()
        {
        }

        internal static Regex Build(PatternNode pattern, bool exactMatch)
            => new PatternRegexBuilder().BuildCore(pattern, exactMatch);

        private Regex BuildCore(PatternNode pattern, bool exactMatch)
        {
            Visit(pattern);

            if (exactMatch)
                builder.AppendDollar();

            return RegexFactory.Create(builder.ToString());
        }

        protected internal override void VisitMatch(MatchNode node)
            => builder.Append(Regex.Escape(node.Value));

        protected internal override void VisitVariable(VariableNode node)
        {
            // - Generates a regex capture with the name of the variable:  (?<variable>[^/]+)
            // - Variable name does not need to be escaped or validated. The PatternParser ensures that it only contains 'a-zA-Z0-9_'
            //   and the first character is not a number.
            builder.Append($"(?<{node.Name}>[^{SegmentStart}]+)");
        }

        protected internal override void VisitSegmentStart(SegmentStartNode node)
            => builder.Append(SegmentStart);

        protected internal override void VisitWildcard(WildcardNode node)
            => builder.Append(Wildcard);

        protected internal override void VisitOptional(OptionalNode node)
        {
            // regex: (node)?
            // Note that RegexOptions.ExplicitCapture is on so no need to use "(?:node)?".

            builder.AppendOpenParentheses();
            base.VisitOptional(node);
            builder.Append(")?");
        }
    }
}
