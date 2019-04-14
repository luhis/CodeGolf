using System.Collections.Generic;
using CodeGolf.Dtos;
using OneOf;

namespace CodeGolf
{
    public interface ICodeGolfService
    {
        OneOf<int, IReadOnlyList<string>> Score<T>(string code, Challenge<T> challenge);
    }
}