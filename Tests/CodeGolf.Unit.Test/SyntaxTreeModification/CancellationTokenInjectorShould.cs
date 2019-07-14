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
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    throw new System.Threading.Tasks.TaskCanceledException();
                                }
                                return ""Hello World"";
                            }
                        }";
            newSource.ToFullString().Should().BeEquivalentToIgnoreWS(expect);
        }

        [Fact]
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
            if (cancellationToken.IsCancellationRequested)
            {
                throw new System.Threading.Tasks.TaskCanceledException();
            }
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new System.Threading.Tasks.TaskCanceledException();
                }
            }    
            return ""Hello World"";
        }    
    }";
            newSource.ToFullString().Should().BeEquivalentToIgnoreWS(expect);
        }

        [Fact]
        public void NotLoseLoopBody()
        {
            var code = @"public class HelloWorld
    {
        public string Main()
        {
            var i = 0;
            while(true)
            {
                i++;
            }
            return ""Hello World"";
        }
    }";

            var newSource = CompilationTooling.Transform(code);

            var expect = @"public class HelloWorld
    {
        public string Main(System.Threading.CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new System.Threading.Tasks.TaskCanceledException();
            }
            var i = 0;
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new System.Threading.Tasks.TaskCanceledException();
                }
                i++;
            }    
            return ""Hello World"";
        }
    }";
            newSource.ToFullString().Should().BeEquivalentToIgnoreWS(expect);
        }

        [Fact]
        public void WorkWithPrivateFunctions()
        {
            var code = @"public class HelloWorld
    {
        private int X() => 42;

        public string Main(string s)
        {
            return s + X();
        }
    }";

            var newSource = CompilationTooling.Transform(code);

            var expect = @"public class HelloWorld
    {
        private int X(System.Threading.CancellationToken cancellationToken){
            if (cancellationToken.IsCancellationRequested)
                {
                    throw new System.Threading.Tasks.TaskCanceledException();
                }
            return 42;
        }

        public string Main(string s, System.Threading.CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new System.Threading.Tasks.TaskCanceledException();
            }
            return s + X(cancellationToken);
        }
    }";
            newSource.ToFullString().Should().BeEquivalentToIgnoreWS(expect);
        }

        [Fact]
        public void WorkWithGotoStatements()
        {
            var code = @"public string Main(string s){
            forever:
                goto forever;
            return s;
        }";

            var newSource = CompilationTooling.Transform(code);

            var expect = @"public string Main(string s, System.Threading.CancellationToken cancellationToken){
            if (cancellationToken.IsCancellationRequested)
                {
                    throw new System.Threading.Tasks.TaskCanceledException();
                }
            forever:
                {if (cancellationToken.IsCancellationRequested)
                {
                    throw new System.Threading.Tasks.TaskCanceledException();
                }
                goto forever;}
            return s;
        }";
            newSource.ToFullString().Should().BeEquivalentToIgnoreWS(expect);
        }

        [Fact]
        public void WorkWithForEachStatements()
        {
            var code = @"public string Main(string s){
            foreach(var x in new[] {1, 2, 3})
                x++;
            return s;
        }";

            var newSource = CompilationTooling.Transform(code);

            var expect = @"public string Main(string s, System.Threading.CancellationToken cancellationToken){
            if (cancellationToken.IsCancellationRequested)
                {
                    throw new System.Threading.Tasks.TaskCanceledException();
                }
            foreach(var x in new[] {1, 2, 3})
                {
if (cancellationToken.IsCancellationRequested)
                {
                    throw new System.Threading.Tasks.TaskCanceledException();
                }
                x++;
                }
            return s;
        }";
            newSource.ToFullString().Should().BeEquivalentToIgnoreWS(expect);
        }
    }
}