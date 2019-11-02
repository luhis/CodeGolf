namespace CodeGolf.Domain
{
    public class Error
    {
        public Error(string message, string found)
        {
            this.Message = message;
            this.Found = found;
        }

        public Error(string message)
        {
            this.Message = message;
            this.Found = string.Empty;
        }

        public string Message { get; }

        public string Found { get; }
    }
}
