using FluentAssertions;
using Xunit;

namespace CodeGolf.Unit.Test.SyntaxTreeModification
{
    public class CancellationTokenInjectorShould
    {
        [Fact]
        public void AddCancellationTokenParam()
        {
            var code = @"public class HelloWorld
    {
        public string Main()
        {
            return ""Hello World"";
        }
    }";

            var newSource = CompilationTooling.Transform(code);

            var expect = @"public class HelloWorld
    {
public string Main(System.Threading.CancellationToken cancellationToken)
{
    return ""Hello World"";
}    }";
            newSource.ToFullString().Should().BeEquivalentTo(expect);
        }

        [Fact(Skip = "Can only get one transform to run at a time")]
        public void AddCancellationTokenChecker()
        {
            var code = @"public class HelloWorld
    {
        public string Main()
        {
            while(true)
            {}
            return ""Hello World"";
        }
    }";

            var newSource = CompilationTooling.Transform(code);

            var expect = @"public class HelloWorld
    {
public string Main(System.Threading.CancellationToken cancellationToken)
{
            while(true)
            {
                if (token.IsCancellationRequested)
                {
                    throw new System.Threading.Tasks.TaskCanceledException();
                }
}
    return ""Hello World"";
}    }";
            newSource.ToFullString().Should().BeEquivalentTo(expect);
        }
    }
}