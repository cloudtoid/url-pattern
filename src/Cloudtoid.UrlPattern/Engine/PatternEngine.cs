using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Http;
using static Cloudtoid.Contract;

namespace Cloudtoid.UrlPattern
{
    public sealed class PatternEngine : IPatternEngine
    {
        private readonly IPatternCompiler compiler;
        private readonly IPatternMatcher matcher;
        private readonly ConcurrentDictionary<string, CompiledPatternInfo> compiledPatterns =
            new ConcurrentDictionary<string, CompiledPatternInfo>(StringComparer.Ordinal);

        public PatternEngine()
        {
            compiler = new PatternCompiler(new PatternTypeResolver(), new PatternParser(), new PatternValidator());
            matcher = new PatternMatcher();
        }

        // This constructor is used by the dependency injection engine
        public PatternEngine(IPatternCompiler compiler, IPatternMatcher matcher)
        {
            this.compiler = CheckValue(compiler, nameof(compiler));
            this.matcher = CheckValue(matcher, nameof(matcher));
        }

        public bool TryMatch(
            string pattern,
            PathString path,
            [NotNullWhen(true)] out PatternMatchResult? match,
            [NotNullWhen(false)] out string? why)
        {
            return TryMatchCore(pattern, path, out match, out why);
        }

        public PatternMatchResult Match(string pattern, PathString path)
        {
            if (TryMatchCore(pattern, path, out var match, out var why))
                return match;

            throw new PatternException(why);
        }

        private bool TryMatchCore(
            string pattern,
            PathString path,
            [NotNullWhen(true)] out PatternMatchResult? match,
            [NotNullWhen(false)] out string? why)
        {
            CheckValue(pattern, nameof(pattern));
            CheckParam(path.HasValue, nameof(path));

            if (!TryCompile(pattern, out var compiledPattern, out why))
            {
                match = null;
                return false;
            }

            return TryMatch(compiledPattern, path, out match, out why);
        }

        private bool TryCompile(
            string pattern,
            [NotNullWhen(true)] out CompiledPattern? compiledPattern,
            [NotNullWhen(false)] out string? error)
        {
            if (compiledPatterns.TryGetValue(pattern, out var info))
            {
                error = info.Error;
                if (error is null)
                {
                    compiledPattern = CheckValue(info.CompiledPattern, nameof(info.CompiledPattern));
                    return true;
                }
                else
                {
                    compiledPattern = null;
                    return false;
                }
            }

            if (!compiler.TryCompile(pattern, out compiledPattern, out var errors))
            {
                error = GetCompileErrorMessage(errors);
                compiledPatterns.TryAdd(pattern, new CompiledPatternInfo(error));
                error += Environment.NewLine + "// not from cache";
                return false;
            }

            error = null;
            compiledPatterns.TryAdd(pattern, new CompiledPatternInfo(compiledPattern));
            return true;
        }

        public bool TryMatch(
            CompiledPattern compiledPattern,
            PathString path,
            [NotNullWhen(true)] out PatternMatchResult? match,
            [NotNullWhen(false)] out string? why)
        {
            return matcher.TryMatch(compiledPattern, path, out match, out why);
        }

        public bool TryCompile(
            string pattern,
            [NotNullWhen(true)] out CompiledPattern? compiledPattern,
            [NotNullWhen(false)] out IReadOnlyList<PatternCompilerError>? errors)
        {
            return compiler.TryCompile(pattern, out compiledPattern, out errors);
        }

        private static string GetCompileErrorMessage(IReadOnlyList<PatternCompilerError> errors)
        {
            var builder = new StringBuilder("Failed to compile the pattern with the following errors:");
            foreach (var error in errors)
                builder.AppendLine().Append(error.ToString());

            return builder.ToString();
        }

        private sealed class CompiledPatternInfo
        {
            internal CompiledPatternInfo(CompiledPattern compiledPattern)
            {
                CompiledPattern = compiledPattern;
            }

            internal CompiledPatternInfo(string error)
            {
                Error = error;
            }

            internal CompiledPattern? CompiledPattern { get; }

            internal string? Error { get; }
        }
    }
}
