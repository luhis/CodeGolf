using System.Linq;

namespace CodeGolf.Service
{
    public class Scorer : IScorer
    {
        private static string TrimLines(string s)
        {
            return string.Join("\n", s.Split('\n').Select(a => a.Trim()));
        }

        private static string CleanSource(string s) => TrimLines(s).Replace("\n", "").Replace(" ", "");

        int IScorer.Score(string code)
        {
            return CleanSource(code).Length;
        }
    }
}
