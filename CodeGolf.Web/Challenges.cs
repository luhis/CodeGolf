using CodeGolf.Dtos;

namespace CodeGolf.Web
{
    public static class Challenges
    {
        public static readonly Challenge<string> HelloWorld = new Challenge<string>(new object[0], "Hello World");
    }
}