namespace CodeGolf.ServiceInterfaces
{
    using System;
    using System.Threading.Tasks;
    using Optional;

    public interface IExecutionService
    {
        Task<Option<T, string>[]> Execute<T>(
            CompileResult compileResult,
            string className,
            string funcName,
            object[][] args,
            Type[] paramTypes);

        Task Ping();
    }
}
