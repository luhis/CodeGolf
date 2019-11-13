namespace CodeGolf.Service
{
    using System;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using CodeGolf.ExecutionService;
    using CodeGolf.ServiceInterfaces;
    using Google.Protobuf;
    using Grpc.Net.Client;
    using Optional;

    public class ExecutionProxy : IExecutionService
    {
        private readonly string host = "localhost";
        private readonly Executer.ExecuterClient client;

        public ExecutionProxy()
        {
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(
                $"http://{this.host}:{SharedSettings.PortNumber}");
            this.client = new Executer.ExecuterClient(channel);
        }

        async Task<Option<T, string>[]> IExecutionService.Execute<T>(
            ServiceInterfaces.CompileResult compileResult,
            string className,
            string funcName,
            object[][] args,
            Type[] paramTypes)
        {
            var rx = await this.client.ExecuteAsync(new ExecuteRequest
            {
                CompileResult = new ExecutionService.CompileResult()
                {
                    Dll = ByteString.CopyFrom(compileResult.Dll),
                    Pdb = ByteString.CopyFrom(compileResult.Pdb)
                },
                ClassName = className,
                FuncName = funcName,
                Args = { args.Select(a => new ArgSet() { Arg = { a.Select(b => b.ToString()) } }) }, // todo
                ParamTypes = { paramTypes.Select(a => a.FullName) }
            });

            return rx.Result.Select(a => a.Result_ != null ? Option.Some<T, string>(JsonSerializer.Deserialize<T>(a.Result_)) : Option.None<T, string>(a.Error)).ToArray();
        }

        async Task IExecutionService.Ping()
        {
            await this.client.PingAsync(new PingRequest());
        }
    }
}
