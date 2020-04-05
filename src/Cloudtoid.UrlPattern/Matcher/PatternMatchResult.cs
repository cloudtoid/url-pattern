namespace Cloudtoid.UrlPattern
{
    using System.Collections.Generic;
    using static Contract;

    public sealed class PatternMatchResult
    {
        internal PatternMatchResult(
            string pathSuffix,
            IReadOnlyDictionary<string, string> variables)
        {
            PathSuffix = CheckValue(pathSuffix, nameof(pathSuffix));
            Variables = CheckValue(variables, nameof(variables));
        }

        /// <summary>
        /// Gets the suffix portion of the URL path that was not matched to the pattern.
        /// </summary>
        public string PathSuffix { get; }

        /// <summary>
        /// Gets the variables and their values extracted from the route pattern and the inbound URL path respectively.
        /// </summary>
        public IReadOnlyDictionary<string, string> Variables { get; }
    }
}
