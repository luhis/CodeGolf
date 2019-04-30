namespace CodeGolf
{
    public class Scorer : IScorer
    {
        private static string CleanSource(string s) => s.Replace("\n", "");

        int IScorer.Score(string code)
        {
            return CleanSource(code).Length;
        }
    }
}