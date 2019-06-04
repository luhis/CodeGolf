using System;
using System.Collections.Generic;
using System.Linq;
using CodeGolf.Domain;

namespace CodeGolf.Web
{
    public static class GenericPresentationHelpers
    {
        private static string WrapIfString(object o, Type t) => t == typeof(string) ? $"\"{o}\"" : o.ToString();

        public static string DisplayFunction(IChallenge challenge, IReadOnlyList<Type> paramTypes, Type returnType)
        {
            var zipped = challenge.Args.Zip(paramTypes, Tuple.Create).Select(a => WrapIfString(a.Item1, a.Item2));
            return $"({string.Join(", ", zipped)}) => {WrapIfString(challenge.ExpectedResult, returnType)}";
        }
    }
}