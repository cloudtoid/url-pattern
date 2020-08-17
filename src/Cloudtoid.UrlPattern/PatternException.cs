using System;
using static Cloudtoid.Contract;

namespace Cloudtoid.UrlPattern
{
    public sealed class PatternException : Exception
    {
        public PatternException(string message, Exception? innerException = null)
            : base(CheckValue(message, nameof(message)), innerException)
        {
        }
    }
}