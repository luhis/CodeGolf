namespace CodeGolf.Web.Tooling
{
    using System.Linq;

    using CodeGolf.Domain;
    using CodeGolf.Service;

    public static class ErrorSetSerialiser
    {
        public static string Serialise(ErrorSet es)
        {
            return string.Join(
                ",",
                es.Errors.Select(ErrorMessageParser.Parse)
                    .Select(e => e.Match(some => $"{some.Line}:{some.Col}", () => string.Empty)));
        }
    }
}