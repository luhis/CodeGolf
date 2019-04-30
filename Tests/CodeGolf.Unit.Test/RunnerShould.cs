using System;
using FluentAssertions;
using Xunit;

namespace CodeGolf.Unit.Test
{
    public class RunnerShould
    {
        private readonly IRunner runner = new Runner();

        [Fact]
        public void ReturnHelloWorld()
        {
            var r = this.runner.Compile<string>("public string Main(string s){ return s;}", new[] { typeof(string) }).ExtractSuccess()(new[] { "Hello world" });
            r.ExtractSuccess().Should().BeEquivalentTo("Hello world");
        }

        [Fact]
        public void ReturnNotHelloWorld()
        {
            var r = this.runner.Compile<string>("public string Main(string s){ return \"not \" + s;}", new[] { typeof(string) }).ExtractSuccess()(new[]
                {"Hello world"});
            r.ExtractSuccess().Should().BeEquivalentTo("not Hello world");
        }

        [Fact]
        public void FailToCompileInvalidApplication()
        {
            this.runner.Compile<string>("abc", new Type[]{}).ExtractErrors()
                .Should().BeEquivalentTo("(1,39): error CS1519: Invalid token '}' in class, struct, or interface member declaration");
        }

        //[Fact]
        //public void FailCleanlyWhenFunctionMisnamed()
        //{
        //    var r = this.runner.Compile<string>("public string MainXXX(string s){ return \"not \" + s;}").ExtractSuccess()(new[]
        //        {"Hello world"});
        //    r.ExtractErrors().Should().BeEquivalentTo("Function 'Main' missing");
        //}

        [Fact]
        public void FailWhenWrongNumberOfParameters()
        {
            var r = this.runner.Compile<string>("public string Main(string s){ return \"not \" + s;}", new[] { typeof(string), typeof(int) }).ExtractSuccess()(new object[]
                {"Hello world", 1});
            r.ExtractErrors().Should().BeEquivalentTo("Incorrect parameter count expected 2");
        }

        [Fact]
        public void FailWhenReturnTypeIsUnexpected()
        {
            var r = this.runner.Compile<string>("public int Main(string s){ return 42;}", new[] { typeof(string) }).ExtractSuccess()(new object[] { "Hello world" });
            r.ExtractErrors().Should().BeEquivalentTo("Return type incorrect expected System.String");
        }

        [Fact]
        public void FailWhenParametersHaveWrongType()
        {
            var r = this.runner.Compile<string>("public string Main(int i){ return \"not \";}", new [] {typeof(string)}).ExtractSuccess()(new object[]
                {"Hello world"});
            r.ExtractErrors().Should().BeEquivalentTo("Parameter type mismatch");
        }

        [Fact]
        public void SupportPrivateFunctionCalls()
        {
            var code = @"
private int X() => 42;
public string Main(string s){ return s + X();}";
            var r = this.runner.Compile<string>(code, new []{typeof(string)}).ExtractSuccess()(new[] {"Hello world"});
            r.ExtractSuccess().Should().BeEquivalentTo("Hello world42");
        }
    }
}
