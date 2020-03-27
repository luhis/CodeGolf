namespace CodeGolf.ServiceInterfaces
{
    using System;
    using System.IO;
    using System.Reflection;

    public interface ILoadAssembly
    {
        T LoadAndFunc<T>(MemoryStream ms, Func<Assembly, T> func);
    }
}
