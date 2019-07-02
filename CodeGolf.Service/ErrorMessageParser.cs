namespace CodeGolf.Service
{
    using System.Text.RegularExpressions;

    using CodeGolf.Service.Dtos;

    using Optional;

    public static class ErrorMessageParser
    {
        public static Option<ErrorMessage> Parse(string s)
        {
            var match = Regex.Match(s, @"\((?<line>-?\d+),(?<col>-?\d+)\): (?<message>.*...)");

            return match.Success
                       ? Option.Some(
                           new ErrorMessage(
                               int.Parse(match.Groups[1].Value),
                               int.Parse(match.Groups[2].Value),
                               match.Groups[3].Value))
                       : Option.None<ErrorMessage>();
        }
    }
}