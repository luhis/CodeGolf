using CodeGolf.ServiceInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGolf.Service
{
    public static class DiModule
    {
        public static void Add(IServiceCollection collection)
        {
            collection.AddTransient<ICodeGolfService, CodeGolfService>();
            collection.AddTransient<IScorer, Scorer>();
            collection.AddTransient<IRunner, Runner>();
            collection.AddTransient<IGameService, GameService>();
            collection.AddTransient<IDashboardService, DashboardService>();
            collection.AddTransient<IExecutionService, ExecutionProxy>();
            collection.AddTransient<ISyntaxTreeTransformer, SyntaxTreeTransformer>();
            collection.AddTransient<IErrorMessageTransformer, ErrorMessageTransformer>();
            collection.AddTransient<CancellationTokenInjector>();
        }
    }
}