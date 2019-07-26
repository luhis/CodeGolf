using Microsoft.CodeAnalysis;

namespace CodeGolf.Service
{
    public class SyntaxTreeTransformer : ISyntaxTreeTransformer
    {
        SyntaxTree ISyntaxTreeTransformer.Transform(SyntaxTree syntaxTree)
        {
            var cancellationTokenInjector = new CancellationTokenInjector(syntaxTree.GetRoot());
            return cancellationTokenInjector.Visit(syntaxTree.GetRoot()).NormalizeWhitespace().SyntaxTree;
        }
    }
}