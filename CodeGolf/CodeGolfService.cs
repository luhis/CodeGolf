using System.Collections.Generic;
using System.Linq;
using CodeGolf.Dtos;
using OneOf;

namespace CodeGolf
{
    public class CodeGolfService : ICodeGolfService
    {
        private readonly Runner runner = new Runner();
        private readonly Scorer scorer = new Scorer();

        public OneOf<int, IReadOnlyList<string>> Score<T>(string code, Challenge<T> challenge)
        {
            var result = this.runner.Compile<T>(code)(challenge.Args);
            return result.Match(success =>
            {
                if (!challenge.Validator(success))
                {
                    return (OneOf<int, IReadOnlyList<string>>) new List<string>() {"Return value incorrect"};
                }
                else
                {
                    return this.scorer.Score(code);
                }

            }, a => a.ToList());
        }
    }
}