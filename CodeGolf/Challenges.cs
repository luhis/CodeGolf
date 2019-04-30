using System;
using CodeGolf.Dtos;

namespace CodeGolf
{
    public static class Challenges
    {
        public static readonly ChallengeSet<string> HelloWorld = new ChallengeSet<string>("Hello World",
            "Write a function that returns 'Hello World'",
            new Type[]{}, 
            new[] {new Challenge<string>(new object[0], "Hello World")});
    }
}