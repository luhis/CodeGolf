using Microsoft.CodeAnalysis;

namespace CodeGolf.Service
{
    public interface ISyntaxTreeTransformer
    {
        SyntaxTree Transform(SyntaxTree syntaxTree);
    }
}