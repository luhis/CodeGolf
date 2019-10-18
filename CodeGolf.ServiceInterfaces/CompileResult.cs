namespace CodeGolf.ServiceInterfaces
{
    public class CompileResult
    {
        public CompileResult(byte[] dll, byte[] pdb)
        {
            this.Dll = dll;
            this.Pdb = pdb;
        }

        public CompileResult()
        {
        }

        public byte[] Dll { get; set; }

        public byte[] Pdb { get; set; }
    }
}
