namespace CodeGolf.Persistence.Static
{
    using System;
    using CodeGolf.Domain;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:Parameter should not span multiple lines", Justification = "Pre formatted text")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1137:Elements should have the same indentation", Justification = "Pre formatted text")]
    public static class Challenges
    {
        public static readonly ChallengeSet<string> HelloWorld = new ChallengeSet<string>(
            Guid.Parse("5ccbb74c-1972-47cd-9c5c-f2f512ad95e5"),
            "Hello World",
            "Write a function that returns *'Hello World'*",
            new ParamDescription[] { },
            new[] { new Challenge<string>(new object[0], "Hello World") });

        public static readonly ChallengeSet<string> AlienSpeak = new ChallengeSet<string>(
            Guid.Parse("d44ee76a-ccde-4006-aa83-86578296a886"),
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
            Guid.Parse("08d16a48-4dbb-4f93-9c69-41ff0ab5a417"),
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
            Guid.Parse("74e2c07b-4d14-413f-8efa-d8befcae0510"),
            "FizzBuzz",
            "Write a program that prints the integer numbers from 1 to i inclusive. But for multiples of three print \"Fizz\" instead of the number and for the multiples of five print \"Buzz\". For numbers which are multiples of both three and five print \"FizzBuzz\".",
            new[] { new ParamDescription(typeof(int), "i") },
            new[]
                {
                    new Challenge<string[]>(
                        new object[] { 3 },
                        new[] { "1", "2", "Fizz", }),
                    new Challenge<string[]>(
                        new object[] { 10 },
                        new[] { "1", "2", "Fizz", "4", "Buzz", "Fizz", "7", "8", "Fizz", "Buzz", }),
                    new Challenge<string[]>(
                        new object[] { 20 },
                        new[]
                            {
                                "1", "2", "Fizz", "4", "Buzz", "Fizz", "7", "8", "Fizz", "Buzz", "11", "Fizz", "13",
                                "14", "FizzBuzz", "16", "17", "Fizz", "19", "Buzz",
                            })
                });

        public static readonly ChallengeSet<string> RocketScience = new ChallengeSet<string>(
            Guid.Parse("08d16a48-4dbb-4f93-9c69-41ff0ab5a417"),
            "This isn't rocket science",
            @"Write a program or function that takes in a single-line string. You can assume it only contains printable ASCII. Print or return a string of an ASCII art rocket such as.",
            new[] { new ParamDescription(typeof(string), "s") },
            new[]
                {
                    new Challenge<string>(
                        new object[] { string.Empty },
                        NormaliseNewLines(@"  |
 /_\
 |_|
/___\
 VvV")),
                    new Challenge<string>(
                        new object[] { "a" },
                        NormaliseNewLines(@"  |
 /_\
 |a|
 |_|
/___\
 VvV")),
                    new Challenge<string>(new object[] { "Earth" }, NormaliseNewLines(@"  |
 /_\
 |E|
 |a|
 |r|
 |t|
 |h|
 |_|
/___\
 VvV")),
                });

        public static readonly ChallengeSet<string> SpiralNumbers = new ChallengeSet<string>(
            Guid.Parse("08d16a48-4dbb-4f93-9c69-41ff0ab5a418"),
            "Spiral Numbers",
            @"Write a program or function that takes in a square side length, and generated a square with the numbers spiraling around the cells",
            new[] { new ParamDescription(typeof(int), "i") },
            new[]
            {
                new Challenge<string>(
                    new object[] { 1 },
                    "| 1|"),
                new Challenge<string>(
                    new object[] { 2 },
                    "| 1| 2|\n| 4| 3|"),
                new Challenge<string>(
                    new object[] { 3 },
                    "| 1| 2| 3|\n| 8| 9| 4|\n| 7| 6| 5|"),
            });

        private static string NormaliseNewLines(string s) => s.Replace("\r\n", "\n");
    }
}
