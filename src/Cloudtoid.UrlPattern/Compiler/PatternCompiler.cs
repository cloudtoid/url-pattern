using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using static Cloudtoid.Contract;

namespace Cloudtoid.UrlPattern
{
    internal sealed class PatternCompiler : IPatternCompiler
    {
        private readonly IPatternTypeResolver resolver;
        private readonly IPatternParser parser;
        private readonly IPatternValidator validator;

        public PatternCompiler(
            IPatternTypeResolver resolver,
            IPatternParser parser,
            IPatternValidator validator)
        {
            this.resolver = CheckValue(resolver, nameof(resolver));
            this.parser = CheckValue(parser, nameof(parser));
            this.validator = CheckValue(validator, nameof(validator));
        }

        public bool TryCompile(
            string pattern,
            [NotNullWhen(true)] out CompiledPattern? compiledPattern,
            [NotNullWhen(false)] out IReadOnlyList<PatternCompilerError>? errors)
        {
            CheckValue(pattern, nameof(pattern));

            var errorsSink = new PatternCompilerErrorsSink();

            // 1- Resolve the type of the pattern
            var resolvedPattern = resolver.Resolve(pattern);
            var type = resolvedPattern.Type;
            pattern = resolvedPattern.Pattern;

            Regex regex;
            ISet<string> variables;

            if (type == PatternType.Regex)
            {
                // 2- Build regex
                regex = RegexFactory.Create(pattern);

                // 3- Get variable names
                var names = regex.GetGroupNames().Where(n => !short.TryParse(n, NumberStyles.None, null, out var _));
                variables = new HashSet<string>(names, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                // 2- Parse
                if (!parser.TryParse(pattern, errorsSink, out var parsedPattern))
                {
                    compiledPattern = null;
                    errors = errorsSink.Errors;
                    return false;
                }

                // 3- Validate
                if (!validator.Validate(parsedPattern, errorsSink))
                {
                    compiledPattern = null;
                    errors = errorsSink.Errors;
                    return false;
                }

                // 4- Build regex
                regex = PatternRegexBuilder.Build(parsedPattern, type == PatternType.ExactMatch);

                // 5- Get variable names
                variables = VariableNamesExtractor.Extract(parsedPattern);
            }

            compiledPattern = new CompiledPattern(
                pattern,
                type,
                regex,
                variables);

            errors = null;
            return true;
        }

        private sealed class VariableNamesExtractor : PatternNodeVisitor
        {
            private readonly ISet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            private VariableNamesExtractor()
            {
            }

            internal static ISet<string> Extract(PatternNode node)
                => new VariableNamesExtractor().ExtractCore(node);

            private ISet<string> ExtractCore(PatternNode node)
            {
                Visit(node);
                return names;
            }

            protected internal override void VisitVariable(VariableNode node) => names.Add(node.Name);
        }
    }
}
