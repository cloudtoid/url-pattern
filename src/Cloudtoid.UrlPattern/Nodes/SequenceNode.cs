using System.Collections.Generic;
using System.Linq;
using static Cloudtoid.Contract;

namespace Cloudtoid.UrlPattern
{
    /// <summary>
    /// Represents a sequence of pattern nodes
    /// </summary>
    internal sealed class SequenceNode : PatternNode
    {
        internal SequenceNode(IEnumerable<PatternNode> nodes)
        {
            CheckValue(nodes, nameof(nodes));

            Nodes = CheckNonEmpty(
                Flatten(nodes).AsReadOnlyList(),
                nameof(nodes));
        }

        internal SequenceNode(params PatternNode[] nodes)
            : this((IEnumerable<PatternNode>)nodes)
        {
        }

        public IReadOnlyList<PatternNode> Nodes { get; }

        public override string ToString() => string.Join(null, Nodes);

        internal override void Accept(PatternNodeVisitor visitor)
            => visitor.VisitSequence(this);

        private static IEnumerable<PatternNode> Flatten(IEnumerable<PatternNode> nodes)
        {
            var result = Enumerable.Empty<PatternNode>();

            foreach (var node in nodes)
            {
                result = node is SequenceNode seq
                    ? result.Concat(seq.Nodes)
                    : result.Concat(node);
            }

            return result;
        }
    }
}
