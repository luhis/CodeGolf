using CodeGolf.Persistence.Static;
using CodeGolf.Service;
using CodeGolf.Web.Pages;
using Moq;
using Xunit;

namespace CodeGolf.Unit.Test.Pages
{
    public class DemoModelShould
    {
        private readonly MockRepository mockRepository;
        private readonly DemoModel demoModel;

        public DemoModelShould()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            var cgs = this.mockRepository.Create<ICodeGolfService>();
            cgs.Setup(a => a.GetDemoChallenge()).Returns(Challenges.HelloWorld);
            this.demoModel = new DemoModel(cgs.Object);
        }

        [Fact]
        public void Initial()
        {
            this.demoModel.OnGet();
            this.mockRepository.VerifyAll();
        }
    }
}
