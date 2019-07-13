using System;
using System.Collections.Generic;
using CodeGolf.Domain;
using Optional;
using Optional.Unsafe;

namespace CodeGolf.Unit.Test.Tooling
{
    using CodeGolf.Service.Dtos;

    public static class TestTooling
    {
        public static IReadOnlyList<string> ExtractErrors<T>(this Option<T, ErrorSet> input)
        {
            return input.Match(_ => throw new Exception("Option contains success"), a => a.Errors);
        }

        public static string ExtractErrors<T>(this Option<T, string> input)
        {
            return input.Match(_ => throw new Exception("Option contains success"), a => a);
        }

        public static IReadOnlyList<ChallengeResult> ExtractErrors<T>(this Option<T, IReadOnlyList<ChallengeResult>> input)
        {
            return input.Match(_ => throw new Exception("Option contains success"), a => a);
        }

        public static IReadOnlyList<CompileErrorMessage> ExtractErrors<T>(this Option<T, IReadOnlyList<CompileErrorMessage>> input)
        {
            return input.Match(_ => throw new Exception("Option contains success"), a => a);
        }

        public static T ExtractSuccess<T>(this Option<T, ErrorSet> input)
        {
            return input.Match(a => a, errors => throw new Exception($"Option contains error: {string.Join(", ", errors.Errors)}"));
        }

        public static T ExtractSuccess<T, TT>(this Option<T, TT> input)
        {
            return input.ValueOrFailure();
        }
    }
}