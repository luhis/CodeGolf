namespace CodeGolf.Service.Dtos
{
    public class ErrorMessage
    {
        public ErrorMessage(int line, int col, string message)
        {
            this.Line = line;
            this.Col = col;
            this.Message = message;
        }

        public int Line { get; }

        public int Col { get; }

        public string Message { get; }

        public string GetString() => $"({this.Line},{this.Col}): {this.Message}";
    }
}