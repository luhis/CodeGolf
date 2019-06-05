using System.Collections.Generic;
using System.Threading;
using CodeGolf.Domain;
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
            var r = this.runner.Compile("public string Main(string s){ return s;}", new[] {typeof(string)},
                typeof(string),
                CancellationToken.None).ExtractSuccess()(new[] {"Hello world"}).Result;
            r.ExtractSuccess().Should().BeEquivalentTo("Hello world");
        }

        [Fact]
        public void ReturnNotHelloWorld()
        {
            var r = this.runner.Compile("public string Main(string s){ return \"not \" + s;}",
                new[] {typeof(string)}, typeof(string), CancellationToken.None).ExtractSuccess()(new[]
                {"Hello world"}).Result;
            r.ExtractSuccess().Should().BeEquivalentTo("not Hello world");
        }

        [Fact]
        public void FailCleanlyWhenFunctionMisnamed()
        {
            this.runner.Compile("public string MainXXX(string s){ return \"not \" + s;}",
                    new[] {typeof(string)}, typeof(string), CancellationToken.None).ExtractErrors().Should()
                .BeEquivalentTo("Function 'Main' missing");
        }

        [Fact]
        public void FailWhenWrongNumberOfParameters()
        {
            this.runner.Compile("public string Main(string s){ return \"not \" + s;}",
                    new[] {typeof(string), typeof(int)}, typeof(string), CancellationToken.None).ExtractErrors()
                .Should()
                .BeEquivalentTo("Incorrect parameter count expected 2");
        }

        [Fact]
        public void FailWhenReturnTypeIsUnexpected()
        {
            this.runner
                .Compile("public int Main(string s){ return 42;}", new[] {typeof(string)}, typeof(string),
                    CancellationToken.None).ExtractErrors().Should()
                .BeEquivalentTo("Return type incorrect expected System.String");
        }

        [Fact]
        public void FailWhenParametersHaveWrongType()
        {
            this.runner.Compile("public string Main(int i){ return \"not \";}", new[] {typeof(string)}, typeof(string),
                CancellationToken.None).ExtractErrors().Should().BeEquivalentTo("Parameter type mismatch");
        }

        [Fact]
        public void SupportPrivateFunctionCalls()
        {
            var code = @"
        private int X() => 42;
        public string Main(string s){ return s + X();}";
            this.runner.Compile(code, new[] {typeof(string)}, typeof(string), CancellationToken.None).ExtractSuccess()(
                    new object[] {"Hello world"}).Result.ExtractSuccess().Should()
                .BeEquivalentTo("Hello world42");
        }

        [Fact]
        public void DealWithRunTimeErrors()
        {
            var code = @"
        public string Main(string s){ 
            var a = (new [] {1})[1];
            return s;
        }";
            this.runner.Compile(code, new[] {typeof(string)}, typeof(string), CancellationToken.None).ExtractSuccess()(
                    new object[] {"Hello world"}).Result
                .ExtractErrors().Should().BeEquivalentTo("Index was outside the bounds of the array.");
        }

        [Fact]
        public void DealWithInfiniteWhileLoops()
        {
            var code = @"
        public string Main(string s){ 
            while(true)
            {
            }
            return s;
        }";
            this.runner.Compile(code, new[] {typeof(string)}, typeof(string), CancellationToken.None).ExtractSuccess()(
                    new object[] {"Hello world"}).Result
                .ExtractErrors().Should().BeEquivalentTo("A task was canceled.");
        }

        [Fact]
        public void DealWithInfiniteForLoops()
        {
            var code = @"
        public string Main(string s){ 
            for(var i = 0; i > -1;)
            {
            }
            return s;
        }";
            this.runner.Compile(code, new[] {typeof(string)}, typeof(string), CancellationToken.None).ExtractSuccess()(
                    new object[] {"Hello world"}).Result
                .ExtractErrors().Should().BeEquivalentTo("A task was canceled.");
        }

        [Fact]
        public void DealWithInfiniteDoWhileLoops()
        {
            var code = @"
        public string Main(string s){
            do
            {
            } while(true);
            return s;
        }";
            this.runner.Compile(code, new[] {typeof(string)}, typeof(string), CancellationToken.None).ExtractSuccess()(
                    new object[] {"Hello world"}).Result
                .ExtractErrors().Should().BeEquivalentTo("A task was canceled.");
        }

        [Fact]
        public void DealWithInfiniteGotoLoops()
        {
            var code = @"
        public string Main(string s){
            forever:
                goto forever;
            return s;
        }";
            this.runner.Compile(code, new[] {typeof(string)}, typeof(string), CancellationToken.None).ExtractSuccess()(
                    new object[] {"Hello world"}).Result
                .ExtractErrors().Should().BeEquivalentTo("A task was canceled.");
        }

        [Fact(Skip = "soon")]
        public void DealWithInfiniteRecursion()
        {
            var code = @"
        public string Main(string s){
            return Main(s);
        }";
            this.runner.Compile(code, new[] {typeof(string)}, typeof(string), CancellationToken.None).ExtractSuccess()(
                    new object[] {"Hello world"}).Result
                .ExtractErrors().Should().BeEquivalentTo("A task was canceled.");
        }

        [Fact]
        public void DealWithThreadSleep()
        {
            var code = @"
        public string Main(string s){ 
            System.Threading.Thread.Sleep(1000);
            return s;
        }";
            this.runner.Compile(code, new[] {typeof(string)}, typeof(string), CancellationToken.None)
                .ExtractErrors().Should()
                .BeEquivalentTo("(9,34): error CS0122: 'Thread' is inaccessible due to its protection level");
        }

        [Fact(Skip = "One day")]
        public void DealWithTaskSleep()
        {
            var code = @"
        public string Main(string s){ 
            System.Threading.Tasks.Task.Delay(100000).Wait();
            return s;
        }";
            this.runner.Compile(code, new[] {typeof(string)}, typeof(string), CancellationToken.None).ExtractSuccess()(
                    new[]
                        {"aaa"}).Result
                .ExtractErrors().Should()
                .BeEquivalentTo("(8,22): error CS0122: 'Thread' is inaccessible due to its protection level");
        }

        [Fact]
        public void SupportLinq()
        {
            var code = "public IEnumerable<int> Main(int[] a) { return a.Select(b => b+1); }";
            var r = this.runner.Compile(code, new[] {typeof(int[])}, typeof(IEnumerable<int>), CancellationToken.None)
                .ExtractSuccess()(new[] {new[] {1, 2}}).Result;
            r.ExtractSuccess().Should().BeEquivalentTo(new[] {2, 3});
        }
    }
}
