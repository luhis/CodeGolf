using System;
using System.Threading.Tasks;

namespace CodeGolf.ServiceInterfaces
{
    public interface IExecutionService
    {
        Task<bool> IsAlive();

        Task<T> Execute<T>(byte[] assembly, string className, string funcName, object[] args,
            Type[] paramTypes);
    }
}