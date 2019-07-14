using System;
using System.Threading.Tasks;

namespace CodeGolf.ServiceInterfaces
{
    public interface IExecutionService
    {
        Task<ValueTuple<T, string>[]> Execute<T>(
            byte[] dll,
            byte[] pdb,
            string className,
            string funcName,
            object[][] args,
            Type[] paramTypes);
    }
}