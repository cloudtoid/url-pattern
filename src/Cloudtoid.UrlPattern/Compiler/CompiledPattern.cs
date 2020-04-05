namespace Cloudtoid.UrlPattern
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using static Contract;

    public sealed class CompiledPattern
    {
        internal CompiledPattern(
            string pattern,
            PatternType type,
            Regex regex,
            ISet<string> variableNames)
        {
            Pattern = CheckValue(pattern, nameof(pattern));
            Type = type;
            Regex = CheckValue(regex, nameof(regex));
            VariableNames = CheckValue(variableNames, nameof(variableNames));
        }

        public string Pattern { get; }

        public PatternType Type { get; }

        public Regex Regex { get; }

        public ISet<string> VariableNames { get; }
    }
}
