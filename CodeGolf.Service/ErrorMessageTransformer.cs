namespace CodeGolf.Service
{
    using CodeGolf.Service.Dtos;

    public class ErrorMessageTransformer : IErrorMessageTransformer
    {
        CompileErrorMessage IErrorMessageTransformer.Transform(CompileErrorMessage msg)
        {
            return new CompileErrorMessage(msg.Line, msg.Col, msg.EndCol, msg.Message);
        }
    }
}
