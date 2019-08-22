namespace CodeGolf.Unit.Test.Services
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.ExecutionServer;
    using CodeGolf.Service;
    using CodeGolf.Service.Dtos;
    using CodeGolf.Unit.Test.Tooling;

    using FluentAssertions;

    using Xunit;

    public class RunnerShould
    {
        private readonly IRunner runner = new Runner(
            new SyntaxTreeTransformer(),
            new ExecutionService(),
            new ErrorMessageTransformer());

        [Fact]
        public async Task ReturnHelloWorld()
        {
            var r = await this.runner
                        .Compile(
                            "public string Main(string s){ return s;}",
                            new[] { typeof(string) },
                            typeof(string),
                            CancellationToken.None).ExtractSuccess().Func(new[] { new[] { "Hello world" } });
            r.First().ExtractSuccess().Should().BeEquivalentTo("Hello world");
        }

        [Fact]
        public async Task ReturnNotHelloWorld()
        {
            var r = await this.runner
                        .Compile(
                            "public string Main(string s){ return \"not \" + s;}",
                            new[] { typeof(string) },
                            typeof(string),
                            CancellationToken.None).ExtractSuccess().Func(new[] { new[] { "Hello world" } });
            r.First().ExtractSuccess().Should().BeEquivalentTo("not Hello world");
        }

        [Fact]
        public void FailCleanlyWhenFunctionMisnamed()
        {
            this.runner.Compile(
                    "public string MainXXX(string s){ return \"not \" + s;}",
                    new[] { typeof(string) },
                    typeof(string),
                    CancellationToken.None).ExtractErrors().Should()
                .BeEquivalentTo(new CompileErrorMessage("Function 'Main' missing"));
        }

        [Fact]
        public void FailWhenWrongNumberOfParameters()
        {
            this.runner.Compile(
                    "public string Main(string s){ return \"not \" + s;}",
                    new[] { typeof(string), typeof(int) },
                    typeof(string),
                    CancellationToken.None).ExtractErrors().Should()
                .BeEquivalentTo(new CompileErrorMessage("Incorrect parameter count expected 2"));
        }

        [Fact]
        public void FailWhenReturnTypeIsUnexpected()
        {
            this.runner.Compile(
                "public int Main(string s){ return 42;}",
                new[] { typeof(string) },
                typeof(string),
                CancellationToken.None).ExtractErrors().Should().BeEquivalentTo(
                new CompileErrorMessage("Return type incorrect expected System.String"));
        }

        [Fact]
        public void FailWhenParametersHaveWrongType()
        {
            this.runner.Compile(
                    "public string Main(int i){ return \"not \";}",
                    new[] { typeof(string) },
                    typeof(string),
                    CancellationToken.None).ExtractErrors().Should()
                .BeEquivalentTo(new CompileErrorMessage("Parameter type mismatch"));
        }

        [Fact]
        public async Task SupportPrivateFunctionCalls()
        {
            var code = @"
        private int X() => 42;
        public string Main(string s){ return s + X();}";
            var r = await this.runner.Compile(code, new[] { typeof(string) }, typeof(string), CancellationToken.None)
                        .ExtractSuccess().Func(new[] { new object[] { "Hello world" } });

            r.First().ExtractSuccess().Should().BeEquivalentTo("Hello world42");
        }

        [Fact]
        public async Task DealWithRunTimeErrors()
        {
            var code = @"string Main(string s){ 
            var a = (new [] {1})[1];
            return s;
        }";
            var r = await this.runner.Compile(code, new[] { typeof(string) }, typeof(string), CancellationToken.None)
                        .ExtractSuccess().Func(new[] { new object[] { "Hello world" } });
            r.First().ExtractErrors().Should()
                .BeEquivalentTo("Runtime Error line 0 - Index was outside the bounds of the array.");
        }

        [Fact]
        public async Task DealWithInfiniteWhileLoops()
        {
            var code = @"
        public string Main(string s){ 
            while(true)
            {
            }
            return s;
        }";
            var r = await this.runner.Compile(code, new[] { typeof(string) }, typeof(string), CancellationToken.None).ExtractSuccess()
                .Func(new[] { new object[] { "Hello world" } });
            r.First().ExtractErrors().Should()
                .BeEquivalentTo("Runtime Error line 0 - A task was canceled.");
        }

        [Fact]
        public async Task DealWithInfiniteForLoops()
        {
            var code = @"
        public string Main(string s){ 
            for(var i = 0; i > -1;)
            {
            }
            return s;
        }";
            var r = await this.runner.Compile(code, new[] { typeof(string) }, typeof(string), CancellationToken.None)
                        .ExtractSuccess().Func(new[] { new object[] { "Hello world" } });
            r.First().ExtractErrors().Should().BeEquivalentTo("Runtime Error line 0 - A task was canceled.");
        }

        [Fact]
        public async Task DealWithInfiniteDoWhileLoops()
        {
            var code = @"
        public string Main(string s){
            do
            {
            } while(true);
            return s;
        }";
            var r = await this.runner.Compile(code, new[] { typeof(string) }, typeof(string), CancellationToken.None).ExtractSuccess()
                .Func(new[] { new object[] { "Hello world" } });
            r.First().ExtractErrors().Should()
                .BeEquivalentTo("Runtime Error line 0 - A task was canceled.");
        }

        [Fact]
        public async Task DealWithInfiniteGotoLoops()
        {
            var code = @"
        public string Main(string s){
            forever:
                goto forever;
            return s;
        }";
            var r = await this.runner.Compile(code, new[] { typeof(string) }, typeof(string), CancellationToken.None).ExtractSuccess()
                .Func(new[] { new object[] { "Hello world" } });
            r.First().ExtractErrors().Should()
                .BeEquivalentTo("Runtime Error line 0 - A task was canceled.");
        }

        [Fact(Skip = "soon")]
        public async Task DealWithInfiniteRecursion()
        {
            var code = @"
        public string Main(string s){
            return Main(s);
        }";
            var r = await this.runner.Compile(code, new[] { typeof(string) }, typeof(string), CancellationToken.None)
                        .ExtractSuccess().Func(new[] { new object[] { "Hello world" } });
            r.First().ExtractErrors().Should().BeEquivalentTo("A task was canceled.");
        }

        [Fact]
        public void DealWithThreadSleep()
        {
            var code = @"
        public string Main(string s){ 
            System.Threading.Thread.Sleep(1000);
            return s;
        }";
            this.runner.Compile(code, new[] { typeof(string) }, typeof(string), CancellationToken.None).ExtractErrors()
                .Should().BeEquivalentTo(
                    new CompileErrorMessage(3, 29, 35, "'Thread' is inaccessible due to its protection level"));
        }

        [Fact(Skip = "One day")]
        public async Task DealWithTaskSleep()
        {
            var code = @"
        public string Main(string s){ 
            System.Threading.Tasks.Task.Delay(100000).Wait();
            return s;
        }";
            var r = await this.runner.Compile(code, new[] { typeof(string) }, typeof(string), CancellationToken.None)
                        .ExtractSuccess().Func(new[] { new[] { "aaa" } });
            r.First().ExtractErrors().Should().BeEquivalentTo(
                "(8,22): error CS0122: 'Thread' is inaccessible due to its protection level");
        }

        [Fact]
        public async Task SupportLinq()
        {
            var code = "public int[] Main(int[] a) { return a.Select(b => b+1).ToArray(); }";
            var r = await this.runner.Compile(code, new[] { typeof(int[]) }, typeof(int[]), CancellationToken.None)
                        .ExtractSuccess().Func(new[] { new[] { new int[] { 1, 2 } } });
            r.First().ExtractSuccess().Should().BeEquivalentTo(new[] { 2, 3 });
        }
    }
}
