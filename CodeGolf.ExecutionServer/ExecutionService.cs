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
    public class ExecutionService : IExecutionService
    {
        private const int ExecutionTimeoutMilliseconds = 1000;

        public async Task<Tuple<T, string>> Execute<T>(byte[] assembly, string className, string funcName, object[] args,
            Type[] paramTypes)
        {
            var castArgs = CastArgs(args, paramTypes);
            var obj = Assembly.Load(assembly);
            var type = obj.GetType(className);
            var inst = Activator.CreateInstance(type);
            var fun = GetMethod(funcName, type);
            var source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromMilliseconds(ExecutionTimeoutMilliseconds));
            try
            {
                return Tuple.Create<T, string>(await Task<T>.Factory.StartNew(() => (T) fun.Invoke(inst,
                    BindingFlags.Default | BindingFlags.InvokeMethod,
                    null, castArgs.Append(source.Token).ToArray(), CultureInfo.InvariantCulture), source.Token), null);
            }
            catch (Exception e)
            {
                return Tuple.Create(default(T), e.InnerException != null ? e.InnerException.Message : e.Message);
            }
        }

        private static MethodInfo GetMethod(string funcName, IReflect type)
        {
            return type.GetMethod(funcName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static IEnumerable<object> CastArgs(IEnumerable<object> args, IEnumerable<Type> paramTypes)
        {
            return paramTypes.Zip(args, Tuple.Create).Select(a => Convert.ChangeType(a.Item2, a.Item1));
        }
    }
}