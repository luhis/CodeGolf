using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace CodeGolf
{
    public class Runner
    {
        public bool IsCorrect(string code, Func<int, bool> f)
        {
            var provider = new CSharpCodeProvider();

            // Build the parameters for source compilation.
            var cp = new CompilerParameters();

            // Add an assembly reference.
            cp.ReferencedAssemblies.Add("System.dll");

            // Generate an executable instead of
            // a class library.
            cp.GenerateExecutable = true;

            // Set the assembly file name to generate.
           // cp.OutputAssembly = exeFile;

            // Save the assembly as a physical file.
            cp.GenerateInMemory = false;

            // Invoke compilation.
            var cr = provider.CompileAssemblyFromSource(cp, code);

            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                Console.WriteLine("Errors building {0} into {1}",
                    code, cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Source {0} built into {1} successfully.",
                    code, cr.PathToAssembly);
            }

            return f(1);
        }
    }
}
