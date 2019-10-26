namespace CodeGolf.Domain
{
    public class Error
    {
        public Error(string message, string expected, string found)
        {
            this.Message = message;
            this.Expected = expected;
            this.Found = found;
        }

        public Error(string message)
        {
            this.Message = message;
            this.Expected = string.Empty;
            this.Found = string.Empty;
        }

        public string Message { get; }

        public string Expected { get; }

        public string Found { get; }
    }
}
