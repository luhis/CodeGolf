using System;
using System.Threading.Tasks;

namespace CodeGolf.ServiceInterfaces
{
    public interface IExecutionService
    {
        Task<ValueTuple<T, string>[]> Execute<T>(byte[] assembly, string className, string funcName, object[][] args,
            Type[] paramTypes);
    }
}