namespace CodeGolf.ExecutionServer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.ServiceInterfaces;
    using Optional;
    using CompileResult = CodeGolf.ServiceInterfaces.CompileResult;

    // todo this should be split out to allow GRPC bypass without importing in all the other crap
    public class ExecutionService : IExecutionService
    {
        private const int ExecutionTimeoutMilliseconds = 2_000;

        async Task<Option<T, string>[]> IExecutionService.Execute<T>(CompileResult compileResult, string className, string funcName, object[][] args, Type[] paramTypes)
        {
            using (var dll = new MemoryStream(compileResult.Dll))
            using (var pdb = new MemoryStream(compileResult.Pdb))
            {
                var obj = AssemblyLoadContext.Default.LoadFromStream(dll, pdb);
                var type = obj.GetType(className);
                var inst = Activator.CreateInstance(type);
                var fun = GetMethod(funcName, type);
                return await Task.WhenAll(
                    args.Select(
                        async a =>
                        {
                            var castArgs = CastArgs(a, paramTypes).ToArray();
                            using var source = new CancellationTokenSource();
                            source.CancelAfter(TimeSpan.FromMilliseconds(ExecutionTimeoutMilliseconds));
                            try
                            {
                                return Optional.Option.Some<T, string>(
                                    await Task<T>.Factory.StartNew(
                                        () => (T)fun.Invoke(
                                            inst,
                                            BindingFlags.Default | BindingFlags.InvokeMethod,
                                            null,
                                            castArgs.Append(source.Token).ToArray(),
                                            CultureInfo.InvariantCulture),
                                        source.Token,
                                        TaskCreationOptions.None,
                                        TaskScheduler.Current));
                            }
                            catch (Exception e)
                            {
                                var final = GetFinalException(e);
                                var line = new StackTrace(final, true).GetFrame(0).GetFileLineNumber();
                                return Optional.Option.None<T, string>(
                                    $"Runtime Error line {line} - {final.Message}");
                            }
                        }));
            }
        }

        Task IExecutionService.Ping()
        {
            return Task.CompletedTask;
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
