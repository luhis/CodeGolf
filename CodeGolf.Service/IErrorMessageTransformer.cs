namespace CodeGolf.Service
{
    using CodeGolf.Service.Dtos;

    public interface IErrorMessageTransformer
    {
        CompileErrorMessage Transform(CompileErrorMessage msg);
    }
}
