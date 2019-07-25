using System;
using System.Threading.Tasks;

namespace CodeGolf.ServiceInterfaces
{
    using System.IO;

    public interface IExecutionService
    {
        Task<ValueTuple<T, string>[]> Execute<T>(
            CompileResult compileResult, 
            string className,
            string funcName,
            object[][] args,
            Type[] paramTypes);
    }
}