using System;
using System.Collections.Generic;
using System.Linq;
using CodeGolf.Domain.ChallengeInterfaces;

namespace CodeGolf.Domain
{
    public static class GenericPresentationHelpers
    {
        private static string ToCommaSep<T>(T[] arr) =>
            arr == null ? "null" : string.Join(", ", arr.Select(a => a == null ? "null" : WrapIfString(a, typeof(T))));

        private static string WrapIfString(object o, Type t) => t == typeof(string) ? $"\"{o}\"" : o.ToString();

        public static string WrapIfArray(object o, Type t)
        {
            if (o == null)
            {
                return "null";
            }
            else if (t.IsArray)
            {
                if (o is string[] stringArr)
                {
                    return $"[{ToCommaSep(stringArr)}]";
                }

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

        public static string DisplayFunction(IChallenge challenge, IReadOnlyList<ParamDescription> paramTypes, Type returnType)
        {
            var zipped = challenge.Args.Zip(paramTypes, ValueTuple.Create).Select(a => WrapIfString(a.Item1, a.Item2.Type));
            return $"({string.Join(", ", zipped)}) => {WrapIfArray(challenge.ExpectedResult, returnType)}";
        }

        private static readonly IReadOnlyDictionary<Type, string> Aliases =
            new Dictionary<Type, string>()
                {
                    { typeof(string), "string" }, { typeof(string[]), "string[]" }, { typeof(int), "int" }
                };

        public static string GetAlias(Type t)
        {
            if (Aliases.ContainsKey(t))
            {
                return Aliases[t];
            }
            else
            {
                return t.Name;
            }
        }

        public static string GetFuncTemplate(IChallengeSet set)
        {
            var paramPairs = set.Params
                .Select(t => $"{GetAlias(t.Type)} {t.SuggestedName}");
            return $"{GetAlias(set.ReturnType)} Main({string.Join(", ", paramPairs)}) {{ return ... ; }}";
        }
    }
}