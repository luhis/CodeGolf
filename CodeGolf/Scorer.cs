namespace CodeGolf
{
    public class Scorer
    {
        public int Score(string code)
        {
            return code.Replace("\n", "").Length;
        }
    }
}