using System;
using System.Collections.Generic;
using Optional;

namespace CodeGolf.Unit.Test
{
    public static class TestTooling
    {
        public static IReadOnlyList<string> ExtractErrors<T>(this Option<T, IReadOnlyList<string>> input)
        {
            return input.Match(_ => throw new Exception("Option contains success"), a => a);
        }

        public static T ExtractSuccess<T>(this Option<T, IReadOnlyList<string>> input)
        {
            return input.Match(a => a, _ => throw new Exception("Option contains error"));
        }
    }
}