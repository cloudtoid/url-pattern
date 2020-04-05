namespace Cloudtoid.UrlPattern
{
    using System;
    using static Contract;

    public sealed class PatternException : Exception
    {
        public PatternException(string message, Exception? innerException = null)
            : base(CheckValue(message, nameof(message)), innerException)
        {
        }
    }
}