namespace Cloudtoid.UrlPattern
{
    public sealed class PatternCompilerError
    {
        internal PatternCompilerError(string message, int? location)
        {
            Message = message;
            Location = location;
        }

        public string Message { get; }

        public int? Location { get; }

        public override string ToString()
            => Location is null ? Message : $"(location:{Location}) {Message}";
    }
}
