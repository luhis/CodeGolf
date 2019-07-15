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
                                #line -2
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
            #line -2
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new System.Threading.Tasks.TaskCanceledException();
                }
                #line 0
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
            #line -2
            var i = 0;
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new System.Threading.Tasks.TaskCanceledException();
                }
                #line 1
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
            #line -3
            return 42;
        }

        public string Main(string s, System.Threading.CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new System.Threading.Tasks.TaskCanceledException();
            }
            #line 0
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
                #line -5
            forever:
                {if (cancellationToken.IsCancellationRequested)
                {
                    throw new System.Threading.Tasks.TaskCanceledException();
                }
                #line -1
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
                #line -5
            foreach(var x in new[] {1, 2, 3})
                {
if (cancellationToken.IsCancellationRequested)
                {
                    throw new System.Threading.Tasks.TaskCanceledException();
                }
                #line -5
                x++;
                }
            return s;
        }";
            newSource.ToFullString().Should().BeEquivalentToIgnoreWS(expect);
        }
    }
}