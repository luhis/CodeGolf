namespace CodeGolf.Service.Handlers
{
    using System.Collections.Generic;
    using CodeGolf.Service.Dtos;
    using MediatR;

    public class FinalScores : IRequest<IReadOnlyList<ResultDto>>
    {
    }
}
