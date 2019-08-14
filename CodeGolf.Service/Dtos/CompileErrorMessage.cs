namespace CodeGolf.Service.Dtos
{
    using EnsureThat;

    public class CompileErrorMessage
    {
        public CompileErrorMessage(int line, int col, int endCol, string message)
        {
            this.Line = line;
            this.Col = col;
            this.EndCol = endCol;
            this.Message = EnsureArg.IsNotNull(message, nameof(message));
        }

        public CompileErrorMessage(string message)
        {
            this.Line = -1;
            this.Col = -1;
            this.EndCol = -1;
            this.Message = EnsureArg.IsNotNull(message, nameof(message));
        }

        public int Line { get; }

        public int Col { get; }

        public int EndCol { get; }

        public string Message { get; }
    }
}