namespace CodeGolf.ServiceInterfaces
{
    public class ExecutionParams
    {
        public ExecutionParams(
            CompileResult compileResult,
            string className,
            string funcName,
            object[][] argSets,
            string[] paramTypes)
        {
            this.CompileResult = compileResult;
            this.ClassName = className;
            this.FuncName = funcName;
            this.ArgSets = argSets;
            this.ParamTypes = paramTypes;
        }

        public ExecutionParams()
        {
        }

        public CompileResult CompileResult { get; set; }

        public string ClassName { get; set; }

        public string FuncName { get; set; }

        public object[][] ArgSets { get; set; }

        public string[] ParamTypes { get; set; }
    }
}
