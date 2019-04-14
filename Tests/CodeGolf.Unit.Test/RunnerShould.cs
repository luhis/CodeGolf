using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.OneOf;
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
            r.Should().Be<string>().And.Should().BeEquivalentTo("Hello world");
        }

        [Fact]
        public void ReturnNotHelloWorld()
        {
            var r = this.runner.Execute<string>("public string Main(string s){ return \"not \" + s;}", new[] { "Hello world" });
            r.Should().Be<string>().And.Should().BeEquivalentTo("not Hello world");
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
            var r = this.runner.Execute<string>("public string MainXXX(string s){ return \"not \" + s;}", new[] { "Hello world" });
            r.Should().Be<IReadOnlyList<string>>().And.Should().BeEquivalentTo("Function 'Main' missing");
        }

        [Fact]
        public void FailWhenWrongNumberOfParameters()
        {
            var r = this.runner.Execute<string>("public string Main(string s){ return \"not \" + s;}", new object[] { "Hello world", 1 });
            r.Should().Be<IReadOnlyList<string>>().And.Should().BeEquivalentTo("Incorrect parameter count expected 2");
        }

        [Fact]
        public void FailWhenReturnTypeIsUnexpected()
        {
            var r = this.runner.Execute<string>("public int Main(string s){ return 42;}", new object[] { "Hello world" });
            r.Should().Be<IReadOnlyList<string>>().And.Should().BeEquivalentTo("Return type incorrect expected System.String");
        }

        [Fact]
        public void FailWhenParametersHaveWrongType()
        {
            var r = this.runner.Execute<string>("public string Main(int i){ return \"not \";}", new object[] { "Hello world" });
            r.Should().Be<IReadOnlyList<string>>().And.Should().BeEquivalentTo("Parameter type mismatch");
        }

        [Fact]
        public void SupportPrivateFunctionCalls()
        {
            var code = @"
private int X() => 42;
public string Main(string s){ return s + X();}";
            var r = this.runner.Execute<string>(code, new[] { "Hello world" });
            r.Should().Be<string>().And.Should().BeEquivalentTo("Hello world42");
        }
    }
}
