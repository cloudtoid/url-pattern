namespace Cloudtoid.UrlPattern
{
    using System.Runtime.CompilerServices;

    public static class PatternVariables
    {
        private static readonly int[] CharToIndex = new int[123];

        static PatternVariables()
        {
            for (int i = 0; i < 123; i++)
                CharToIndex[i] = -1;

            var len = ValidVariableCharacters.Length;
            for (int i = 0; i < len; i++)
            {
                char c = ValidVariableCharacters[i];
                int index = char.ToUpperInvariant(c);
                CharToIndex[index] = i;

                index = char.ToLowerInvariant(c);
                CharToIndex[index] = i;
            }
        }

        public static char[] ValidVariableCharacters { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_".ToCharArray();

        /// <summary>
        /// It returns the index of the character <paramref name="c"/> in the <see cref="ValidVariableCharacters"/> array.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetIndex(char c, out int index)
        {
            int v = c;
            if (v >= CharToIndex.Length)
            {
                index = -1;
                return false;
            }

            index = CharToIndex[c];
            return index != -1;
        }

        /// <summary>
        /// Checks that a <paramref name="c"/> is a valid character that can be used in a variable name.
        /// The valid characters are [a..zA..Z0..9_]. However, the very first character cannot be a number.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidVariableChar(char c, bool isFirstChar)
            => IsValidVariableChar((int)c, isFirstChar);

        /// <summary>
        /// Checks that a <paramref name="c"/> is the integer code of a valid character that can be used in a variable name.
        /// The valid characters are [a..zA..Z0..9_]. However, the very first character cannot be a number.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidVariableChar(int c, bool isFirstChar)
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
