using Microsoft.CodeAnalysis;

namespace CodeGolf.Service
{
    public class SyntaxTreeTransformer : ISyntaxTreeTransformer
    {
        SyntaxTree ISyntaxTreeTransformer.Transform(SyntaxTree syntaxTree)
        {
            return syntaxTree;
        }
    }
}