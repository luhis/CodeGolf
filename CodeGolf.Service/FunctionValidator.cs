namespace CodeGolf.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using CodeGolf.Domain;

    public class FunctionValidator
    {
        private readonly string functionName;

        public FunctionValidator(string functionName)
        {
            this.functionName = functionName;
        }

        public ErrorSet ValidateCompiledFunction(
            MethodInfo fun,
            Type expectedReturn,
            IReadOnlyCollection<Type> paramTypes)
        {
            if (fun == null)
            {
                return new ErrorSet($"Function '{this.functionName}' missing");
            }

            var compiledParams = fun.GetParameters().Take(fun.GetParameters().Length - 1);

            if (compiledParams.Count() != paramTypes.Count)
            {
                return new ErrorSet($"Incorrect parameter count expected {paramTypes.Count}");
            }

            if (expectedReturn != fun.ReturnType)
            {
                return new ErrorSet($"Return type incorrect expected {expectedReturn}");
            }

            var missMatches = compiledParams.Select(a => a.ParameterType).Zip(paramTypes, ValueTuple.Create)
                .Where(a => a.Item1 != a.Item2);
            if (missMatches.Any())
            {
                return new ErrorSet("Parameter type mismatch");
            }

            return new ErrorSet();
        }
    }
}
