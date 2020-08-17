using System;
using System.Collections.Generic;

namespace Cloudtoid.UrlPattern
{
    /// <summary>
    /// Validates that all variable names are unique
    /// </summary>
    internal sealed class VariableNameValidator : PatternValidatorBase
    {
        private readonly ISet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        protected internal override void VisitVariable(VariableNode node)
        {
            if (!names.Add(node.Name))
                throw new PatternException($"The variable name '{node.Name}' has already been used. Variable names must be unique.");
        }
    }
}
