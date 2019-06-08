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

        public Task<bool> IsAlive()
        {
            return this.svc.InvokeAsync(a => a.IsAlive());
        }

        public Task<T> Execute<T>(byte[] assembly, string className, string funcName, object[] args, Type[] paramTypes)
        {
            return this.svc.InvokeAsync(
                a => a.Execute<T>(assembly, className, funcName, args, paramTypes));
        }
    }
}