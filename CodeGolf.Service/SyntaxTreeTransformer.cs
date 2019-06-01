using Microsoft.CodeAnalysis;

namespace CodeGolf.Service
{
    public class SyntaxTreeTransformer : ISyntaxTreeTransformer
    {
        private readonly CancellationTokenInjector cancellationTokenInjector;

        public SyntaxTreeTransformer(CancellationTokenInjector cancellationTokenInjector)
        {
            this.cancellationTokenInjector = cancellationTokenInjector;
        }

        SyntaxTree ISyntaxTreeTransformer.Transform(SyntaxTree syntaxTree)
        {
            return this.cancellationTokenInjector.Visit(syntaxTree.GetRoot()).NormalizeWhitespace().SyntaxTree;
        }
    }
}