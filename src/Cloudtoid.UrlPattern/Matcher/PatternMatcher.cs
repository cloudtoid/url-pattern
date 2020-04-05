namespace Cloudtoid.UrlPattern
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;
    using Microsoft.AspNetCore.Http;
    using static Contract;

    internal sealed class PatternMatcher : IPatternMatcher
    {
        /// <inheritdoc/>
        public bool TryMatch(
            CompiledPattern compiledPattern,
            PathString path,
            [NotNullWhen(true)] out PatternMatchResult? match,
            [NotNullWhen(false)] out string? why)
        {
            CheckValue(compiledPattern, nameof(compiledPattern));
            CheckParam(path.HasValue, nameof(path));

            var pathValue = path.Value;
            Match regexMatch;
            try
            {
                regexMatch = compiledPattern.Regex.Match(pathValue);
            }
            catch (RegexMatchTimeoutException)
            {
                why = $"The attempt to match path '{pathValue}' with pattern '{compiledPattern.Pattern}' timed out.";
                match = null;
                return false;
            }

            if (!regexMatch.Success)
            {
                why = $"The path '{pathValue}' is not a match for pattern '{compiledPattern.Pattern}'";
                match = null;
                return false;
            }

            var variables = GetVariables(regexMatch, compiledPattern.VariableNames);
            var pathSuffix = GetPathSuffix(regexMatch, pathValue);

            why = null;
            match = new PatternMatchResult(pathSuffix, variables);
            return true;
        }

        private static IReadOnlyDictionary<string, string> GetVariables(Match match, ISet<string> variableNames)
        {
            var groups = match.Groups;
            Dictionary<string, string>? result = null;

            foreach (var variableName in variableNames)
            {
                var group = groups[variableName];
                if (!group.Success)
                    continue;

                if (result is null)
                    result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                result.Add(variableName, group.Value);
            }

            return result is null
                ? ImmutableDictionary<string, string>.Empty
                : (IReadOnlyDictionary<string, string>)result;
        }

        private static string GetPathSuffix(Match regexMatch, string path)
           => path.Substring(regexMatch.Length);
    }
}
