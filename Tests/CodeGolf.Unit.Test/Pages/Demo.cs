using CodeGolf.Service;
using CodeGolf.Web.Pages;
using FluentAssertions;
using Moq;
using Xunit;

namespace CodeGolf.Unit.Test.Pages
{
    public class DemoShould
    {
        private readonly MockRepository mockRepository;
        private readonly DemoModel demoModel;

        public DemoShould()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.demoModel = new DemoModel(this.mockRepository.Create<ICodeGolfService>().Object);
        }

        [Fact]
        public void Initial()
        {
            this.demoModel.OnGet();
        }
    }
}
