namespace CodeGolf.ServiceInterfaces
{
    public class CompileResult
    {
        public CompileResult(byte[] dll, byte[] pdb)
        {
            this.Dll = dll;
            this.Pdb = pdb;
        }

        public byte[] Dll { get; }

        public byte[] Pdb { get; }
    }
}
