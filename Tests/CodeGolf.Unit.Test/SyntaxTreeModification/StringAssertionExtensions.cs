using FluentAssertions;
using FluentAssertions.Primitives;

namespace CodeGolf.Unit.Test.SyntaxTreeModification
{
    public static class StringAssertionExtensions
    {
        private static string Clean(string s)
        {
            return s.Trim().Replace("\n", "").Replace(" ", "").Replace("\r", "");
        }

        public static void BeEquivalentToIgnoreWS(this StringAssertions sa, string expect)
        {
            Clean(sa.Subject).Should().BeEquivalentTo(Clean(expect));
        }
    }
}