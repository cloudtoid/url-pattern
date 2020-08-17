using static Cloudtoid.Contract;

namespace Cloudtoid.UrlPattern
{
    /// <summary>
    /// Represents a variable in the pattern
    /// </summary>
    internal sealed class VariableNode : LeafNode
    {
        internal VariableNode(string name)
        {
            Name = CheckValue(name, nameof(name));
        }

        public string Name { get; }

        public override string ToString() => ":" + Name;

        internal override void Accept(PatternNodeVisitor visitor)
            => visitor.VisitVariable(this);
    }
}
