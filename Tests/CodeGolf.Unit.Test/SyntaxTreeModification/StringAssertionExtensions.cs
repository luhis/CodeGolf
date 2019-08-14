namespace CodeGolf.Unit.Test.SyntaxTreeModification
{
    using FluentAssertions;
    using FluentAssertions.Primitives;

    public static class StringAssertionExtensions
    {
        public static void BeEquivalentToIgnoreWS(this StringAssertions sa, string expect)
        {
            Clean(sa.Subject).Should().BeEquivalentTo(Clean(expect));
        }

        private static string Clean(string s)
        {
            return s.Trim().Replace("\n", string.Empty).Replace(" ", string.Empty).Replace("\r", string.Empty);
        }
    }
}