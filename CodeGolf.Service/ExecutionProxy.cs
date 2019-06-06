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
            return this.svc.InvokeAsync(
                a => a.IsAlive());
        }

        public Task<object> Execute(byte[] assembly, string className, string funcName, object[] args, Type[] paramTypes)
        {
            return this.svc.InvokeAsync(
                a => a.Execute(assembly, className, funcName, args, paramTypes));
        }

        public Task<object[]> ExecuteArr(byte[] assembly, string className, string funcName, object[] args, Type[] paramTypes)
        {
            return this.svc.InvokeAsync(
                a => a.ExecuteArr(assembly, className, funcName, args, paramTypes));
        }
    }
}