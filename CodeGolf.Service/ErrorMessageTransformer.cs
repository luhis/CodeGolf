namespace CodeGolf.Service
{
    using CodeGolf.Service.Dtos;

    public class ErrorMessageTransformer : IErrorMessageTransformer
    {
        CompileErrorMessage IErrorMessageTransformer.Transform(CompileErrorMessage msg)
        {
            return new CompileErrorMessage(msg.Line - 6, msg.Col - 4, msg.EndCol - 4, msg.Message);
        }
    }
}