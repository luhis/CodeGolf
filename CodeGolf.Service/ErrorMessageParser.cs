namespace CodeGolf.Service
{
    using System.Text.RegularExpressions;

    using CodeGolf.Service.Dtos;

    public static class ErrorMessageParser
    {
        public static ErrorMessage Parse(string s)
        {
           var match = Regex.Match(s, @"\((?<line>\d+),(?<col>\d+)\): (?<message>.*...)");

           return new ErrorMessage(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), match.Groups[3].Value);
        }
    }
}