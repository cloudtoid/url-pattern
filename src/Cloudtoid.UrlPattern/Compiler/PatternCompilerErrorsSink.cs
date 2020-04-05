namespace Cloudtoid.UrlPattern
{
    using System;
    using System.Collections.Generic;

    internal sealed class PatternCompilerErrorsSink
    {
        private List<PatternCompilerError>? errors;

        internal IReadOnlyList<PatternCompilerError> Errors
            => ((IReadOnlyList<PatternCompilerError>?)errors) ?? Array.Empty<PatternCompilerError>();

        internal bool HasErrors => !errors.IsNullOrEmpty();

        internal void AddError(string message, int? location = null)
        {
            if (errors is null)
                errors = new List<PatternCompilerError>();

            errors.Add(new PatternCompilerError(message, location));
        }
    }
}
