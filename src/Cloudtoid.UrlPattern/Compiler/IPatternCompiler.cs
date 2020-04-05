namespace Cloudtoid.UrlPattern
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public interface IPatternCompiler
    {
        /// <summary>
        /// Parses and compiles the URL pattern specified by the <paramref name="pattern"/> parameter.
        /// </summary>
        bool TryCompile(
            string pattern,
            [NotNullWhen(true)] out CompiledPattern? compiledPattern,
            [NotNullWhen(false)] out IReadOnlyList<PatternCompilerError>? errors);
    }
}
