using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeGolf.Domain
{
    public static class GenericPresentationHelpers
    {
        private static string WrapIfString(object o, Type t) => t == typeof(string) ? $"\"{o}\"" : o.ToString();
        public static string WrapIfArray(object o, Type t)
        {
            if (t.IsArray)
            {
                var objArr = o as object[];
                if (objArr != null)
                {
                    return $"[{string.Join(", ", objArr.Select(a => a.ToString()))}]";
                }
                else
                {
                    return $"[{string.Join(", ", ((int[])o).Select(a => a.ToString()))}]";
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