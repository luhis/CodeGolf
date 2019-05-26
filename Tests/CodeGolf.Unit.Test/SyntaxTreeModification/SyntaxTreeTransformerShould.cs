using CodeGolf.Service;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace CodeGolf.Unit.Test.SyntaxTreeModification
{
    public class SyntaxTreeTransformerShould
    {
        private readonly ISyntaxTreeTransformer syntaxTreeTransformer = new SyntaxTreeTransformer();

        [Fact(Skip = "later")]
        public void Test()
        {
            var code = System.IO.File.ReadAllText("../../../SyntaxTreeModification/HelloWorld.cs");
            var tree = CSharpSyntaxTree.ParseText(code);
            var r = this.syntaxTreeTransformer.Transform(tree);
            r.Should().NotBeNull();
        }
    }
}
