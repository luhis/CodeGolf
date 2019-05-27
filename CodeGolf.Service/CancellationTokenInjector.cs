using System.Collections.Generic;
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
        private static readonly StatementSyntax ThrowIfCancelled = ParseStatement(@"if (" + TokenName + @".IsCancellationRequested)
                {
                    throw new System.Threading.Tasks.TaskCanceledException();
                }");

        private IList<string> ModifiedFuncs { get; } = new List<string>();

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var updated = node.AddParameterListParameters(
                Parameter(
                        Identifier(TokenName))
                    .WithType(ParseTypeName(typeof(CancellationToken).FullName))).NormalizeWhitespace();
            this.ModifiedFuncs.Add(node.Identifier.ValueText);
            return base.VisitMethodDeclaration(node.ReplaceNode(node, updated));
        }

        public override SyntaxNode VisitWhileStatement(WhileStatementSyntax node)
        {
            var statement = (BlockSyntax) node.Statement;
            var updated = statement.AddStatements(ThrowIfCancelled).NormalizeWhitespace();
            return base.VisitWhileStatement(node.ReplaceNode(statement, updated));
        }

        public override SyntaxNode VisitForStatement(ForStatementSyntax node)
        {
            var statement = (BlockSyntax)node.Statement;
            var updated = statement.AddStatements(ThrowIfCancelled).NormalizeWhitespace();
            return base.VisitForStatement(node.ReplaceNode(statement, updated));
        }

        public override SyntaxNode VisitDoStatement(DoStatementSyntax node)
        {
            var statement = (BlockSyntax)node.Statement;
            var updated = statement.AddStatements(ThrowIfCancelled).NormalizeWhitespace();
            return base.VisitDoStatement(node.ReplaceNode(statement, updated));
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is IdentifierNameSyntax ins && this.ModifiedFuncs.Contains(ins.Identifier.ValueText))
            {
                var updated = node.AddArgumentListArguments(Argument(
                    IdentifierName(TokenName)));
                return base.VisitInvocationExpression(node.ReplaceNode(node, updated).NormalizeWhitespace());
            }
            else
            {
                return base.VisitInvocationExpression(node);
            }
        }
    }
}