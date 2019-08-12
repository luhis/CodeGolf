using System;
using System.Threading.Tasks;
using CodeGolf.ServiceInterfaces;
using JKang.IpcServiceFramework;

namespace CodeGolf.Service
{
    public class ExecutionProxy : IExecutionService
    {
        private readonly IpcServiceClient<IExecutionService> svc;

        public ExecutionProxy(IpcServiceClient<IExecutionService> svc)
        {
            this.svc = svc;
        }

        Task<ValueTuple<T, string>[]> IExecutionService.Execute<T>(
            CompileResult compileResult,
            string className,
            string funcName,
            object[][] args,
            Type[] paramTypes)
        {
            return this.svc.InvokeAsync(a => a.Execute<T>(compileResult, className, funcName, args, paramTypes));
        }

        Task IExecutionService.Ping()
        {
            return this.svc.InvokeAsync(a => a.Ping());
        }
    }
}