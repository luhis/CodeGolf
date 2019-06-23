using System;
using CodeGolf.Domain;

namespace CodeGolf.Persistence.Static
{
    public static class Challenges
    {
        public static readonly ChallengeSet<string> HelloWorld = new ChallengeSet<string>(
            "Hello World",
            "Write a function that returns *'Hello World'*",
            new ParamDescription[] { },
            new[] { new Challenge<string>(new object[0], "Hello World") });

        public static readonly ChallengeSet<string> AlienSpeak = new ChallengeSet<string>(
            "English to Alien translator",
            @"Take a string and output the alien's language equivalent. The translation works as such:


Swap all the vowels in the word with the corresponding:


| Vowel | With |
|---|----|
| a | obo |
| e | unu |
| i | ini |
| o | api |
| u | iki |",
            new[] { new ParamDescription(typeof(string), "s") },
            new[]
                {
                    new Challenge<string>(new object[] { "Shaun" }, "Shoboikin"),
                    new Challenge<string>(new object[] { "Java" }, "Jobovobo"),
                    new Challenge<string>(new object[] { "Hello, World!" }, "Hunullapi, Wapirld!"),
                    new Challenge<string>(new object[] { "Alan" }, "Obolobon"),
                    new Challenge<string>(new object[] { "Australia" }, "Oboikistroboliniobo"),
                });

        public static readonly ChallengeSet<string> Calculator = new ChallengeSet<string>(
            "Basic Calculator",
            @"You must write a program to evaluate a string that would be entered into a calculator.",
            new[] { new ParamDescription(typeof(string), "s") },
            new[]
                {
                    new Challenge<string>(new object[] { "-4 + 5" }, "1"),
                    new Challenge<string>(new object[] { "-7.5 / 2.5" }, "-3"),
                    new Challenge<string>(new object[] { "-2 + 6 / 2 * 8 - 1 / 2.5 - 18" }, "-12"),
                });

        public static readonly ChallengeSetArray<string> FizzBuzz = new ChallengeSetArray<string>(
            "FizzBuzz",
            "Write a program that prints the integer numbers from 1 to i inclusive. But for multiples of three print \"Fizz\" instead of the number and for the multiples of five print \"Buzz\". For numbers which are multiples of both three and five print \"FizzBuzz\".",
            new[] { new ParamDescription(typeof(int), "i") },
            new[]
                {
                    new ChallengeArray<string>(
                        new object[] { 10 },
                        new[] { "1", "2", "Fizz", "4", "Buzz", "Fizz", "7", "8", "Fizz", "Buzz", }),
                    new ChallengeArray<string>(
                        new object[] { 20 },
                        new[]
                            {
                                "1", "2", "Fizz", "4", "Buzz", "Fizz", "7", "8", "Fizz", "Buzz", "11", "Fizz", "13",
                                "14", "FizzBuzz", "16", "17", "Fizz", "19", "Buzz",
                            })
                });
    }
}