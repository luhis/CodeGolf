namespace CodeGolf.Web.Models
{
    public class ErrorItem
    {
        public ErrorItem(int line, int ch)
        {
            this.line = line;
            this.ch = ch;
        }

        public int line { get; }

        public int ch { get; }
    }
}