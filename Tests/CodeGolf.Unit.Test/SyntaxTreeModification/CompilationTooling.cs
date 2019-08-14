namespace CodeGolf.Unit.Test.SyntaxTreeModification
{
    using CodeGolf.Service;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public static class CompilationTooling
    {
        public static SyntaxNode Transform(string source)
        {
            var tree = CSharpSyntaxTree.ParseText(source);

            var rewriter = new CancellationTokenInjector(tree.GetRoot());
            return rewriter.Visit(tree.GetRoot());
        }
    }
}