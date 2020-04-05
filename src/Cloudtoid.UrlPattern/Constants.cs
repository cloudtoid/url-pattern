namespace Cloudtoid.UrlPattern
{
    using System.Collections.Generic;

    internal static class Constants
    {
        internal const string EscapeSequence = @"\\";
        internal const char EscapeSequenceStart = '\\';
        internal const char Slash = '/';
        internal const char VariableStart = ':';
        internal const char OptionalStart = '(';
        internal const char OptionalEnd = ')';
        internal const char Wildcard = '*';

        internal static readonly HashSet<char> Escapable = new HashSet<char>
        {
            VariableStart,
            OptionalStart,
            OptionalEnd,
            Wildcard
        };
    }
}
