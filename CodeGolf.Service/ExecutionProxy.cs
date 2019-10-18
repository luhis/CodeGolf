namespace CodeGolf.Service
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using CodeGolf.ServiceInterfaces;

    public class ExecutionProxy : IExecutionService
    {
        private readonly HttpClient httpClient;
        private readonly string host = "localhost";

        public ExecutionProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        async Task<ValueTuple<T, string>[]> IExecutionService.Execute<T>(
            CompileResult compileResult,
            string className,
            string funcName,
            object[][] args,
            Type[] paramTypes)
        {
            var r = await this.httpClient.PostAsJsonAsync(
                $"http://{this.host}:{SharedSettings.PortNumber}/execution/execute",
                new ExecutionParams(compileResult, className, funcName, args, paramTypes.Select(a => a.FullName).ToArray()));
            if (!r.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to call API: {r.ReasonPhrase}");
            }

            return JsonSerializer.Deserialize<ValueTuple<T, string>[]>(await r.Content.ReadAsStringAsync());
        }

        Task IExecutionService.Ping()
        {
            return this.httpClient.GetAsync($"http://{this.host}:{SharedSettings.PortNumber}/execution/ping");
        }
    }
}
