namespace CodeGolf.Service
{
    using System.Linq;

    public class Scorer : IScorer
    {
        int IScorer.Score(string code)
        {
            return CleanSource(code).Length;
        }

        private static string CleanSource(string s) => string.Concat(s.Where(a => !char.IsWhiteSpace(a)));
    }
}
