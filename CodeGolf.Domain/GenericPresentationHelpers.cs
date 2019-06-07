using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeGolf.Domain
{
    public static class GenericPresentationHelpers
    {
        private static string ToCommaSep<T>(T[] arr) => arr == null ? "null": string.Join(", ", arr.Select(a => a == null ? "null" : a.ToString()));

        private static string WrapIfString(object o, Type t) => t == typeof(string) ? $"\"{o}\"" : o.ToString();

        public static string WrapIfArray(object o, Type t)
        {
            if (t.IsArray)
            {
                if (o is object[] objArr)
                {
                    return $"[{ToCommaSep(objArr)}]";
                }
                else
                {
                    return $"[{ToCommaSep((int[])o)}]";
                }
            }
            else
            {
                return WrapIfString(o, t);
            }
        }

        public static string DisplayFunction(IChallenge challenge, IReadOnlyList<Type> paramTypes, Type returnType)
        {
            var zipped = challenge.Args.Zip(paramTypes, Tuple.Create).Select(a => WrapIfString(a.Item1, a.Item2));
            return $"({string.Join(", ", zipped)}) => {WrapIfArray(challenge.ExpectedResult, returnType)}";
        }
    }
}