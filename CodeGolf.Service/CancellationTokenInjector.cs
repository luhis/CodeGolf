using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeGolf.Service
{
    public class CancellationTokenInjector : CSharpSyntaxRewriter
    { 
        private const string TokenName = "cancellationToken";

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var updated = node.AddParameterListParameters(
                Parameter(
                        Identifier(TokenName))
                    .WithType(ParseTypeName(typeof(CancellationToken).FullName))).NormalizeWhitespace();

            return base.VisitMethodDeclaration(node.ReplaceNode(node, updated));
        }

        public override SyntaxNode VisitWhileStatement(WhileStatementSyntax node)
        {
            var throwIfCancelled =
                ParseStatement(@"if (" + TokenName + @".IsCancellationRequested)
                {
                    throw new System.Threading.Tasks.TaskCanceledException();
                }");
            var statement = node.Statement;
            return base.VisitWhileStatement(node.ReplaceNode(statement, Block(throwIfCancelled).NormalizeWhitespace()));
        }
    }
}