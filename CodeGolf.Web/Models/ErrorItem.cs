namespace CodeGolf.Web.Models
{
    public class ErrorItem
    {
        public ErrorItem(int line, int ch, int endCh)
        {
            this.line = line;
            this.ch = ch;
            this.endCh = endCh;
        }

        public int line { get; }

        public int ch { get; }

        public int endCh { get; }
    }
}