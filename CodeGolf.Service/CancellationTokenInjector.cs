using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeGolf.Service
{
    using System;

    public class CancellationTokenInjector : CSharpSyntaxRewriter
    {
        private const string TokenName = "cancellationToken";

        private static readonly StatementSyntax ThrowIfCancelled = IfStatement(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(TokenName),
                IdentifierName("IsCancellationRequested")),
            Block(
                SingletonList<StatementSyntax>(
                    ThrowStatement(
                        ObjectCreationExpression(
                                QualifiedName(
                                    QualifiedName(
                                        QualifiedName(
                                            IdentifierName("System"),
                                            IdentifierName("Threading")),
                                        IdentifierName("Tasks")),
                                    IdentifierName("TaskCanceledException")))
                            .WithArgumentList(
                                ArgumentList()))))).NormalizeWhitespace();

        private IList<string> ModifiedFuncs { get; } = new List<string>();

        private static int GetLineNumber(CSharpSyntaxNode n) => n.GetLocation().GetLineSpan().StartLinePosition.Line - 5;

        private static SyntaxTrivia GetLineDirective(int line) =>
            Trivia(LineDirectiveTrivia(Literal(line), true));

        public override SyntaxNode VisitLabeledStatement(LabeledStatementSyntax node)
        {
            var statement = (GotoStatementSyntax)node.Statement;
            var line = GetLineNumber(statement);
            var updated = Block(ThrowIfCancelled.WithTrailingTrivia(GetLineDirective(line)), statement);
            return base.VisitLabeledStatement(node.ReplaceNode(node.Statement, updated));
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var statements = node.Body != null
                                 ? node.Body.Statements.ToArray()
                                 : new[] { ReturnStatement(node.ExpressionBody.Expression) };
            var line = GetLineNumber(node.Body != null
                                         ? node.Body
                                         : (CSharpSyntaxNode)node.ExpressionBody);
            var ps = node.AddParameterListParameters(
                Parameter(Identifier(TokenName)).WithType(ParseTypeName(typeof(CancellationToken).FullName)));
            var updated = (statements.Length > 1
                              ? ps.WithBody(
                                      Block(
                                          new[]
                                              {
                                                  ThrowIfCancelled,
                                                  statements.First().WithLeadingTrivia(
                                                      statements.First().GetLeadingTrivia()
                                                          .Concat(new[] { GetLineDirective(line) }))
                                              }.Concat(statements.Skip(1)))).WithExpressionBody(null).WithoutTrivia()
                                  .WithSemicolonToken(Token(SyntaxKind.None))
                              : ps.WithBody(
                                      Block(
                                          new[] { ThrowIfCancelled.WithTrailingTrivia(GetLineDirective(line)) }.Concat(
                                              statements))).WithExpressionBody(null).WithoutTrivia()
                                  .WithSemicolonToken(Token(SyntaxKind.None))).WithLeadingTrivia(node.GetLeadingTrivia());

            this.ModifiedFuncs.Add(node.Identifier.ValueText);
            return base.VisitMethodDeclaration(node.ReplaceNode(node, updated));
        }

        public override SyntaxNode VisitWhileStatement(WhileStatementSyntax node)
        {
            var statement = (BlockSyntax)node.Statement;
            var line = GetLineNumber(statement);
            var updated = Block(new[] { ThrowIfCancelled.WithTrailingTrivia(GetLineDirective(line)) }.Concat(statement.Statements));
            return base.VisitWhileStatement(node.ReplaceNode(statement, updated));
        }

        public override SyntaxNode VisitForStatement(ForStatementSyntax node)
        {
            var statement = (BlockSyntax)node.Statement;
            var line = GetLineNumber(statement);
            var updated = Block(new[] { ThrowIfCancelled.WithTrailingTrivia(GetLineDirective(line)) }.Concat(statement.Statements));
            return base.VisitForStatement(node.ReplaceNode(statement, updated));
        }

        public override SyntaxNode VisitDoStatement(DoStatementSyntax node)
        {
            var statement = (BlockSyntax)node.Statement;
            var line = GetLineNumber(statement);
            var updated = Block(new[] { ThrowIfCancelled.WithTrailingTrivia(GetLineDirective(line)) }.Concat(statement.Statements));
            return base.VisitDoStatement(node.ReplaceNode(statement, updated));
        }

        public override SyntaxNode VisitForEachStatement(ForEachStatementSyntax node)
        {
            var statement = GetStatementsAsBlock(node);
            var line = GetLineNumber(statement);
            var updated = Block(new[] { ThrowIfCancelled.WithTrailingTrivia(GetLineDirective(line)) }.Concat(statement.Statements));
            return base.VisitForEachStatement(node.ReplaceNode(node.Statement, updated));
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is IdentifierNameSyntax ins && this.ModifiedFuncs.Contains(ins.Identifier.ValueText))
            {
                var updated = node.AddArgumentListArguments(Argument(IdentifierName(TokenName)));
                return base.VisitInvocationExpression(node.ReplaceNode(node, updated).NormalizeWhitespace());
            }
            else
            {
                return base.VisitInvocationExpression(node);
            }
        }

        private static BlockSyntax GetStatementsAsBlock(CommonForEachStatementSyntax node)
        {
            if (node.Statement is BlockSyntax syntax)
            {
                return syntax;
            }
            else if (node.Statement is ExpressionStatementSyntax)
            {
                return Block(node.Statement);
            }

            throw new Exception($"Unknown body type {node.Statement.GetType()}");
        }
    }
}