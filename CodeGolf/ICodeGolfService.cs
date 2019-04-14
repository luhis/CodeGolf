using System.Collections.Generic;
using CodeGolf.Dtos;
using Optional;

namespace CodeGolf
{
    public interface ICodeGolfService
    {
        Option<int, IReadOnlyList<string>> Score<T>(string code, Challenge<T> challenge);
    }
}