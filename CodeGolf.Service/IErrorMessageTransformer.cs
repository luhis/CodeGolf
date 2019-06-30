namespace CodeGolf.Service
{
    using CodeGolf.Service.Dtos;

    public interface IErrorMessageTransformer
    {
        ErrorMessage Transform(ErrorMessage msg);
    }
}