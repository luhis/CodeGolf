namespace CodeGolf.ExecutionServer
{
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using CodeGolf.ExecutionService;
    using CodeGolf.ServiceInterfaces;
    using Grpc.Core;
    using Type = System.Type;

    public class ExecutionServiceGrpcController : Executer.ExecuterBase
    {
        private readonly IExecutionService executionService;

        public ExecutionServiceGrpcController(IExecutionService executionService)
        {
            this.executionService = executionService;
        }

        public override async Task<PingResponse> Ping(PingRequest request, ServerCallContext context)
        {
            await this.executionService.Ping();
            return new PingResponse();
        }

        public override async Task<ExecuteResponse> Execute(ExecuteRequest request, ServerCallContext context)
        {
            var result = await this.executionService.Execute<object>(
                new ServiceInterfaces.CompileResult(request.CompileResult.Dll.ToByteArray(), request.CompileResult.Pdb.ToByteArray()),
                request.ClassName,
                request.FuncName,
                request.Args.Select(a => a.Arg.ToArray()).ToArray(),
                request.ParamTypes.Select(Type.GetType).ToArray());
            var z = result.Select(a =>
                a.Match(some => new Result() { Result_ = JsonSerializer.Serialize(some) }, none => new Result() { Error = none }));
            return new ExecuteResponse() { Result = { z } };
        }
    }
}
