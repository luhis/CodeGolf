using System.Linq;

namespace CodeGolf.Service
{
    public class Scorer : IScorer
    {
        private static string CleanSource(string s) => string.Concat(s.Where(a => !char.IsWhiteSpace(a)));

        int IScorer.Score(string code)
        {
            return CleanSource(code).Length;
        }
    }
}
