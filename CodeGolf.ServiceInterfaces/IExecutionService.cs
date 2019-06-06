using System;
using System.Threading.Tasks;

namespace CodeGolf.ServiceInterfaces
{
    public interface IExecutionService
    {
        Task<bool> IsAlive();

        Task<object> Execute(byte[] assembly, string className, string funcName, object[] args,
            Type[] paramTypes);

        Task<object[]> ExecuteArr(byte[] assembly, string className, string funcName, object[] args,
            Type[] paramTypes);
    }
}