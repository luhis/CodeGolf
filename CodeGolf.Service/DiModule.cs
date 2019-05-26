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
            collection.AddTransient<ISyntaxTreeTransformer, SyntaxTreeTransformer>();
        }
    }
}