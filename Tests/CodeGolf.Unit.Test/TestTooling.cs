using System;
using System.Collections.Generic;
using CodeGolf.Service.Dtos;
using Optional;

namespace CodeGolf.Unit.Test
{
    public static class TestTooling
    {
        public static IReadOnlyList<string> ExtractErrors<T>(this Option<T, ErrorSet> input)
        {
            return input.Match(_ => throw new Exception("Option contains success"), a => a.Errors);
        }

        public static T ExtractSuccess<T>(this Option<T, ErrorSet> input)
        {
            return input.Match(a => a, errors => throw new Exception($"Option contains error: {string.Join(", ", errors)}"));
        }
    }
}