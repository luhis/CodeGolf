using System;
using System.Collections.Generic;
using CodeGolf.Service;
using CodeGolf.Unit.Test.Tooling;
using FluentAssertions;
using Xunit;

namespace CodeGolf.Unit.Test.Services
{
    public class RunnerShould
    {
        private readonly IRunner runner = new Runner(new SyntaxTreeTransformer(new CancellationTokenInjector()));

        [Fact]
        public void ReturnHelloWorld()
        {
            var r = this.runner.Compile<string>("public string Main(string s){ return s;}", new[] { typeof(string) }).ExtractSuccess()(new[] { "Hello world" }).Result;
            r.ExtractSuccess().Should().BeEquivalentTo("Hello world");
        }

        [Fact]
        public void ReturnNotHelloWorld()
        {
            var r = this.runner.Compile<string>("public string Main(string s){ return \"not \" + s;}", new[] { typeof(string) }).ExtractSuccess()(new[]
                {"Hello world"}).Result;
            r.ExtractSuccess().Should().BeEquivalentTo("not Hello world");
        }

        [Fact]
        public void FailToCompileInvalidApplication()
        {
            this.runner.Compile<string>("abc", new Type[] { }).ExtractErrors()
                .Should().BeEquivalentTo("(7,1): error CS1519: Invalid token '}' in class, struct, or interface member declaration");
        }

        [Fact]
        public void FailCleanlyWhenFunctionMisnamed()
        {
            this.runner.Compile<string>("public string MainXXX(string s){ return \"not \" + s;}", new[] { typeof(string) }).ExtractErrors().Should().BeEquivalentTo("Public function 'Main' missing");
        }

        [Fact]
        public void FailWhenWrongNumberOfParameters()
        {
            this.runner.Compile<string>("public string Main(string s){ return \"not \" + s;}", new[] { typeof(string), typeof(int) }).ExtractErrors().Should().BeEquivalentTo("Incorrect parameter count expected 2");
        }

        [Fact]
        public void FailWhenReturnTypeIsUnexpected()
        {
            this.runner.Compile<string>("public int Main(string s){ return 42;}", new[] { typeof(string) }).ExtractErrors().Should().BeEquivalentTo("Return type incorrect expected System.String");
        }

        [Fact]
        public void FailWhenParametersHaveWrongType()
        {
            this.runner.Compile<string>("public string Main(int i){ return \"not \";}", new[] { typeof(string) }).ExtractErrors().Should().BeEquivalentTo("Parameter type mismatch");
        }

        [Fact]
        public void SupportPrivateFunctionCalls()
        {
            var code = @"
        private int X() => 42;
        public string Main(string s){ return s + X();}";
            this.runner.Compile<string>(code, new[] { typeof(string) }).ExtractSuccess()(new object[] {"Hello world"}).Result.
                ExtractSuccess().Should().BeEquivalentTo("Hello world42");
        }

        [Fact]
        public void DealWithRunTimeErrors()
        {
            var code = @"
        public string Main(string s){ 
            var a = (new [] {1})[1];
            return s;
        }";
            this.runner.Compile<string>(code, new[] { typeof(string) }).ExtractSuccess()(new object[] { "Hello world" }).Result
                .ExtractErrors().Should().BeEquivalentTo("One or more errors occurred. (Exception has been thrown by the target of an invocation.)");
        }

        [Fact]
        public void DealWithInfiniteLoops()
        {
            var code = @"
        public string Main(string s){ 
            while(true)
            {
            }
            return s;
        }";
            this.runner.Compile<string>(code, new[] { typeof(string) }).ExtractErrors().Should().BeEquivalentTo("(14,6): warning CS0162: Unreachable code detected");
        }

        [Fact]
        public void SupportLinq()
        {
            var code = "public IEnumerable<int> Main(int[] a) { return a.Select(b => b+1); }";
            var r = this.runner.Compile<IEnumerable<int>>(code, new[] { typeof(int[]) }).ExtractSuccess()(new[] { new[] {1, 2} }).Result;
            r.ExtractSuccess().Should().BeEquivalentTo(new[] {2, 3});
        }
    }
}
