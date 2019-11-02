﻿namespace CodeGolf.ExecutionServer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.ServiceInterfaces;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class ExecutionController : ControllerBase
    {
        private const int ExecutionTimeoutMilliseconds = 2_000;

        [HttpPost("[action]")]
        public async Task<ValueTuple<object, string>[]> Execute(
            ExecutionParams p)
        {
            using (StreamReader reader = new StreamReader(this.Request.Body, Encoding.UTF8))
            {
                var a = await reader.ReadToEndAsync();
            }

            using (var dll = new MemoryStream(p.CompileResult.Dll))
            using (var pdb = new MemoryStream(p.CompileResult.Pdb))
            {
                var obj = AssemblyLoadContext.Default.LoadFromStream(dll, pdb);
                var type = obj.GetType(p.ClassName);
                var inst = Activator.CreateInstance(type);
                var fun = GetMethod(p.FuncName, type);
                return await Task.WhenAll(
                    p.ArgSets.Select(
                        async a =>
                        {
                            var castArgs = CastArgs(a, p.ParamTypes.Select(Type.GetType).ToArray()).ToArray();
                            using var source = new CancellationTokenSource();
                            source.CancelAfter(TimeSpan.FromMilliseconds(ExecutionTimeoutMilliseconds));
                            try
                            {
                                return ValueTuple.Create<object, string>(
                                    await Task<object>.Factory.StartNew(
                                        () => fun.Invoke(
                                            inst,
                                            BindingFlags.Default | BindingFlags.InvokeMethod,
                                            null,
                                            castArgs.Append(source.Token).ToArray(),
                                            CultureInfo.InvariantCulture),
                                        source.Token,
                                        TaskCreationOptions.None,
                                        TaskScheduler.Current),
                                    null);
                            }
                            catch (Exception e)
                            {
                                var final = GetFinalException(e);
                                var line = new StackTrace(final, true).GetFrame(0).GetFileLineNumber();
                                return ValueTuple.Create(
                                    default(object),
                                    $"Runtime Error line {line} - {final.Message}");
                            }
                        }));
            }
        }

        [HttpGet("[action]")]
        public Task Ping()
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
