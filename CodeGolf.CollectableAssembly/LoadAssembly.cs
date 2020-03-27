namespace CodeGolf.CollectableAssembly
{
    using System;
    using System.IO;
    using System.Reflection;
    using CodeGolf.ServiceInterfaces;

    public class LoadAssembly : ILoadAssembly
    {
        T ILoadAssembly.LoadAndFunc<T>(MemoryStream dll, Func<Assembly, T> func)
        {
            var context = new CollectibleAssemblyLoadContext();
            var obj = context.LoadFromStream(dll);

            var res = func(obj);
            context.Unload();
            return res;
        }
    }
}
