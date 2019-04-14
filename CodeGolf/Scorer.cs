namespace CodeGolf
{
    public class Scorer
    {
        private static string CleanSource(string s) => s.Replace("\n", "");

        public int Score(string code)
        {
            return CleanSource(code).Length;
        }
    }
}