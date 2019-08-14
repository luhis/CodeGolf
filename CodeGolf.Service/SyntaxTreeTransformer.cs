namespace CodeGolf.Service
{
    using Microsoft.CodeAnalysis;

    public class SyntaxTreeTransformer : ISyntaxTreeTransformer
    {
        SyntaxTree ISyntaxTreeTransformer.Transform(SyntaxTree syntaxTree)
        {
            var cancellationTokenInjector = new CancellationTokenInjector(syntaxTree.GetRoot());
            return cancellationTokenInjector.Visit(syntaxTree.GetRoot()).NormalizeWhitespace().SyntaxTree;
        }
    }
}