using System;
using System.Collections.Generic;
using System.Linq;
using OneOf;

namespace CodeGolf
{
    public class CodeGolfService : ICodeGolfService
    {
        private readonly Runner runner = new Runner();
        private readonly Scorer scorer = new Scorer();

        public OneOf<int, IReadOnlyList<string>> Score<T>(string code, Func<T, bool> validator)
        {
            var result = this.runner.Execute<T>(code, new object[] { });
            return result.Match(success =>
            {
                if (!validator(success))
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