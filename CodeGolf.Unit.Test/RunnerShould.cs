using System;
using FluentAssertions;
using Xunit;

namespace CodeGolf.Unit.Test
{
    public class RunnerShould
    {
        private readonly Runner runner = new Runner();

        [Fact]
        public void ReturnHelloWorld()
        {
            var r = this.runner.Execute<string>("public string Main(string s){ return s;}", new []{"Hello world"});
            r.Should().BeEquivalentTo("Hello world");
        }

        [Fact]
        public void ReturnNotHelloWorld()
        {
            var r = this.runner.Execute<string>("public string Main(string s){ return \"not \" + s;}", new[] { "Hello world" });
            r.Should().BeEquivalentTo("not Hello world");
        }

        [Fact]
        public void FailToCompileInvalidApplication()
        {
            Action a = () => this.runner.Execute<string>("abc", new[] { "Hello world" });
            a.Should().Throw<Exception>().WithMessage("(1,39): error CS1519: Invalid token '}' in class, struct, or interface member declaration");
        }

        [Fact]
        public void FailCleanlyWhenFunctionMisnamed()
        {
            Action a = () => this.runner.Execute<string>("public string MainXXX(string s){ return \"not \" + s;}", new[] { "Hello world" });
            a.Should().Throw<Exception>().WithMessage("Function 'Main' missing");
        }

        [Fact]
        public void FailWhenWrongNumberOfParameters()
        {
            Action a = () => this.runner.Execute<string>("public string Main(string s){ return \"not \" + s;}", new object[] { "Hello world", 1 });
            a.Should().Throw<Exception>().WithMessage("Incorrect parameter count expected 2");
        }

        [Fact]
        public void FailWhenReturnTypeIsUnexpected()
        {
            Action a = () => this.runner.Execute<string>("public int Main(string s){ return 42;}", new object[] { "Hello world" });
            a.Should().Throw<Exception>().WithMessage("Return type incorrect expected System.String");
        }

        [Fact]
        public void FailWhenParametersHaveWrongType()
        {
            Action a = () => this.runner.Execute<string>("public string Main(int i){ return \"not \";}", new object[] { "Hello world" });
            a.Should().Throw<Exception>().WithMessage("Parameter type mismatch");
        }
    }
}
