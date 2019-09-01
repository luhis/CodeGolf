namespace CodeGolf.Service
{
    using Microsoft.CodeAnalysis;

    public interface ISyntaxTreeTransformer
    {
        SyntaxTree Transform(SyntaxTree syntaxTree);
    }
}
