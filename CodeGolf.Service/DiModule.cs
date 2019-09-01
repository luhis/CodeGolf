namespace CodeGolf.Service
{
    using CodeGolf.ServiceInterfaces;
    using Microsoft.Extensions.DependencyInjection;

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
            collection.AddTransient<IAdminService, AdminService>();
            collection.AddTransient<IBestAttemptsService, BestAttemptsService>();
            collection.AddTransient<IResultsService, ResultsService>();
            collection.AddTransient<CancellationTokenInjector>();
        }
    }
}
