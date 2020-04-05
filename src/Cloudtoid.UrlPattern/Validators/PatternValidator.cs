namespace Cloudtoid.UrlPattern
{
    internal sealed class PatternValidator : IPatternValidator
    {
        public bool Validate(
            PatternNode pattern,
            PatternCompilerErrorsSink errorsSink)
        {
            try
            {
                Validate<NoConsecutiveSegmentStartValidator>(pattern);
                Validate<NoConsecutiveWildcardValidator>(pattern);
                Validate<OneVariablePerSegmentValidator>(pattern);
                Validate<NoVariableFollowedByWildcardValidator>(pattern);
                Validate<VariableNameValidator>(pattern);
                return true;
            }
            catch (PatternException pe)
            {
                errorsSink.AddError(pe.Message);
                return false;
            }
        }

        private static void Validate<TValidator>(PatternNode pattern)
            where TValidator : PatternValidatorBase, new()
        {
            new TValidator().Validate(pattern);
        }
    }
}
