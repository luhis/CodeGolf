using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Domain;
using FluentAssertions;
using Optional;
using Xunit;

namespace CodeGolf.Unit.Test.Domain
{
    public class ChallengeSetArrayShould
    {
        [Fact]
        public void ReturnFalseResultWhenIncorrectWithArray()
        {
            var a = (IChallengeSet)new ChallengeSetArray<string>("a", "b", new[] { typeof(string) },
                new[] { new ChallengeArr<string>(new object[] { "test" }, new string[] { "a" }) });
            var r = a.GetResults(o => Task.FromResult(Option.Some<object, string>(new[] { "testXX" }))).Result;
            r.Single().Error
                .HasValue.Should().BeTrue();
        }

        [Fact]
        public void ReturnFalseResultWhenIncorrectWithEmptyArray()
        {
            var a = (IChallengeSet)new ChallengeSetArray<string>("a", "b", new[] { typeof(string) },
                new[] { new ChallengeArr<string>(new object[] { "test" }, new string[] { "a" }),  });
            var r = a.GetResults(o => Task.FromResult(Option.Some<object, string>(new string[] { }))).Result;
            r.Single().Error
                .HasValue.Should().BeTrue();
        }

        [Fact]
        public void ReturnFalseResultWhenIncorrectWithNullInArray()
        {
            var a = (IChallengeSet)new ChallengeSetArray<string>("a", "b", new[] { typeof(string) },
                new[] { new ChallengeArr<string>(new object[] { "test" }, new string[] { "a" }),  });
            var r = a.GetResults(o => Task.FromResult(Option.Some<object, string>(new string[] { null }))).Result;
            r.Single().Error
                .HasValue.Should().BeTrue();
        }

        [Fact]
        public void ReturnFalseResultWhenIncorrectWithNull()
        {
            var a = (IChallengeSet)new ChallengeSetArray<string>("a", "b", new[] { typeof(string) },
                new[] { new ChallengeArr<string>(new object[] { "test" }, new string[] { "a" }),  });
            var r = a.GetResults(o => Task.FromResult(Option.Some<object, string>(null))).Result;
            r.Single().Error
                .HasValue.Should().BeTrue();
        }
    }
}