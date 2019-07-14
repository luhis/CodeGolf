using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.ServiceInterfaces;

namespace CodeGolf.ExecutionServer
{
    using System.Diagnostics;

    public class ExecutionService : IExecutionService
    {
        private const int ExecutionTimeoutMilliseconds = 1000;

        public async Task<ValueTuple<T, string>[]> Execute<T>(
            byte[] dll,
            byte[] pdb,
            string className,
            string funcName,
            object[][] argSets,
            Type[] paramTypes)
        {
            var obj = Assembly.Load(dll, pdb);
            var type = obj.GetType(className);
            var inst = Activator.CreateInstance(type);
            var fun = GetMethod(funcName, type);
            return (await Task.WhenAll(
                        argSets.Select(
                            async a =>
                                {
                                    var castArgs = CastArgs(a, paramTypes);
                                    var source = new CancellationTokenSource();
                                    source.CancelAfter(TimeSpan.FromMilliseconds(ExecutionTimeoutMilliseconds));
                                    try
                                    {
                                        return ValueTuple.Create<T, string>(
                                            await Task<T>.Factory.StartNew(
                                                () => (T)fun.Invoke(
                                                    inst,
                                                    BindingFlags.Default | BindingFlags.InvokeMethod,
                                                    null,
                                                    castArgs.Append(source.Token).ToArray(),
                                                    CultureInfo.InvariantCulture),
                                                source.Token),
                                            null);
                                    }
                                    catch (Exception e)
                                    {
                                        var final = GetFinalException(e);
                                        var line = (new StackTrace(final, true)).GetFrame(0).GetFileLineNumber();
                                        return ValueTuple.Create(
                                            default(T),
                                            $"Runtime Error line {line} - {final.Message}");
                                    }
                                }))).ToArray();
        }

        private static Exception GetFinalException(Exception e) => e.InnerException == null ? e : GetFinalException(e.InnerException);

        private static MethodInfo GetMethod(string funcName, IReflect type)
        {
            return type.GetMethod(funcName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static IEnumerable<object> CastArgs(IEnumerable<object> args, IEnumerable<Type> paramTypes)
        {
            return paramTypes.Zip(args, ValueTuple.Create).Select(a => Convert.ChangeType(a.Item2, a.Item1));
        }
    }
}