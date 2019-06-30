namespace CodeGolf.Service
{
    using CodeGolf.Service.Dtos;

    public class ErrorMessageTransformer : IErrorMessageTransformer
    {
        ErrorMessage IErrorMessageTransformer.Transform(ErrorMessage msg)
        {
            return new ErrorMessage(msg.Line - 6, msg.Col - 5, msg.Message);
        }
    }
}