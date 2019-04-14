using System.Collections.Generic;
using CodeGolf.Dtos;
using Optional;

namespace CodeGolf
{
    public class CodeGolfService : ICodeGolfService
    {
        private readonly Runner runner = new Runner();
        private readonly Scorer scorer = new Scorer();

        public Option<int, IReadOnlyList<string>> Score<T>(string code, Challenge<T> challenge)
        {
            var compileResult = this.runner.Compile<T>(code);
            return compileResult.FlatMap(compiled =>
            {
                var result = compiled(challenge.Args);
                return result.FlatMap(success =>
                {
                    if (!challenge.Validator(success))
                    {
                        return Option.None<int, IReadOnlyList<string>>(new List<string>() {"Return value incorrect"});
                    }
                    else
                    {
                        return Option.Some<int, IReadOnlyList<string>>(this.scorer.Score(code));
                    }

                });
            });
        }
    }
}