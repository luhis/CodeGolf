using CodeGolf.Service;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeGolf.Unit.Test.SyntaxTreeModification
{
    public static class CompilationTooling
    {
        public static SyntaxNode Transform(string source)
        {
            var tree = CSharpSyntaxTree.ParseText(source);

            var rewriter = new CancellationTokenInjector();
            return rewriter.Visit(tree.GetRoot());
        }
    }
}