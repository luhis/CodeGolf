namespace CodeGolf.ExecutionServer
{
    using System.Reflection;
    using System.Runtime.Loader;

    public class CollectibleAssemblyLoadContext : AssemblyLoadContext
    {
        public CollectibleAssemblyLoadContext()
            : base(true)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
