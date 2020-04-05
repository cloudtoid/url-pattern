namespace Cloudtoid.UrlPattern
{
    using System.Diagnostics.CodeAnalysis;
    using static Contract;

    internal sealed class PatternParser : IPatternParser
    {
        public bool TryParse(
            string pattern,
            PatternCompilerErrorsSink errorsSink,
            [NotNullWhen(true)] out PatternNode? parsedPattern)
        {
            CheckValue(pattern, nameof(pattern));

            pattern = pattern.Trim();
            parsedPattern = new Parser(pattern, errorsSink).Parse();
            return parsedPattern != null;
        }

        private struct Parser
        {
            private readonly string pattern;
            private readonly PatternCompilerErrorsSink errorsSink;

            public Parser(string pattern, PatternCompilerErrorsSink errorsSink)
            {
                this.pattern = pattern;
                this.errorsSink = errorsSink;
            }

            internal PatternNode? Parse()
            {
                using (var reader = new SeekableStringReader(pattern))
                    return ReadNode(reader);
            }

            private PatternNode? ReadNode(SeekableStringReader reader, char? stopChar = null)
            {
                PatternNode? node = null;
                int c, len, start;

                Reset();

                void Reset()
                {
                    len = 0;
                    start = reader.NextPosition;
                }

                PatternNode? AppendMatch(string pattern)
                {
                    if (len > 0)
                        node += new MatchNode(pattern.Substring(start, len));

                    return node;
                }

                while ((c = reader.Read()) > -1)
                {
                    if (c == stopChar)
                        return AppendMatch(pattern);

                    PatternNode? next;
                    switch (c)
                    {
                        case Constants.Slash:
                            next = SegmentStartNode.Instance;
                            break;

                        case Constants.Wildcard:
                            next = WildcardNode.Instance;
                            break;

                        case Constants.VariableStart:
                            next = ReadVariableNode(reader);
                            break;

                        case Constants.OptionalStart:
                            next = ReadOptionalNode(reader);
                            break;

                        case Constants.OptionalEnd:
                            errorsSink.AddError($"There is an unexpected '{(char)c}'.", reader.NextPosition - 1);
                            return null;

                        case Constants.EscapeSequenceStart:
                            if (!ShouldEscape(reader))
                            {
                                len++;
                                continue;
                            }

                            AppendMatch(pattern);
                            reader.NextPosition += Constants.EscapeSequence.Length;
                            start = reader.NextPosition - 1;
                            len = 1;
                            continue;

                        default:
                            len++;
                            continue;
                    }

                    // failed to parse!
                    if (next is null)
                        return null;

                    if (len > 0)
                        node += new MatchNode(pattern.Substring(start, len));

                    node += next;
                    Reset();
                }

                // expected an end char but didn't find it
                if (stopChar.HasValue)
                {
                    errorsSink.AddError($"There is a missing '{stopChar}'.");
                    return null;
                }

                node += len == 0
                    ? MatchNode.Empty
                    : new MatchNode(pattern.Substring(reader.NextPosition - len, len));

                return node;
            }

            private VariableNode? ReadVariableNode(SeekableStringReader reader)
            {
                int start = reader.NextPosition;
                int c, len = 0;
                while ((c = reader.Peek()) > -1 && IsValidVariableChar(c, isFirstChar: len == 0))
                {
                    reader.Read();
                    len++;
                }

                if (len == 0)
                {
                    errorsSink.AddError(
                        "There is a variable with an empty or invalid name. The valid characters are 'a-zA-Z0-9_' and the first character cannot be a number.",
                        start);

                    return null;
                }

                return new VariableNode(pattern.Substring(start, len));
            }

            private OptionalNode? ReadOptionalNode(SeekableStringReader reader)
            {
                var start = reader.NextPosition;
                var node = ReadNode(reader, Constants.OptionalEnd);

                if (node == null)
                {
                    errorsSink.AddError("There is an optional element that is either empty or invalid.", start);
                    return null;
                }

                return new OptionalNode(node);
            }

            // returns true, if escaped an escapable char
            private static bool ShouldEscape(SeekableStringReader reader)
            {
                var value = reader.Value;
                var start = reader.NextPosition - 1;
                var len = Constants.EscapeSequence.Length;
                if (value.Length - reader.NextPosition < len)
                    return false;

                for (int i = 1; i < len; i++)
                {
                    if (Constants.EscapeSequence[i] != value[start + i])
                        return false;
                }

                return Constants.Escapable.Contains(value[start + len]);
            }

            /// <summary>
            /// The valid characters are [a..zA..Z0..9_]. However, the very first character cannot be a number.
            /// </summary>
            private static bool IsValidVariableChar(int c, bool isFirstChar)
            {
                if ((c > 64 && c < 91) || (c > 96 && c < 123) || c == 95)
                    return true;

                // numbers are only allowed if not the first character in the variable name
                if (!isFirstChar && c > 47 && c < 58)
                    return true;

                return false;
            }
        }
    }
}
