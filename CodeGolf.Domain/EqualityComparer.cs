namespace CodeGolf.Domain
{
    using System;

    public static class EqualityComparer
    {
        private static string Normalise(string s) => s.Replace("\r", string.Empty);

        public static bool Equal<T>(T a, T b)
        {
            if (typeof(T) == typeof(string))
            {
                return string.Equals(Normalise(a.ToString()), Normalise(b.ToString()), StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                return a.Equals(b);
            }
        }
    }
}
