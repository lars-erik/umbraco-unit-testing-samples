using Moq;
using NUnit.Framework;
using Umb.Testing.Web.Controllers;
using Umb.Testing.Web.Models;
using Umbraco.UnitTesting.Adapter.Support;

namespace Umb.Testing.Tests.SampleUmbracoTests
{
    [TestFixture]
    public class CTA_Form_Controller
    {
        private CtaFormController controller;
        private UmbracoSupport support = new UmbracoSupport();

        private IMailGateway mailGateway;

        [SetUp]
        public void Setup()
        {
            support.SetupUmbraco();

            mailGateway = Mock.Of<IMailGateway>();

            controller = new CtaFormController(support.UmbracoContext, support.UmbracoHelper, mailGateway);
            support.PrepareController(controller);
        }

        [TearDown]
        public void TearDown()
        {
            support.DisposeUmbraco();
        }

        [Test]
        public void Shows_Specified_View()
        {
            const string expectedViewName = "NewLead";

            var result = controller.Form(expectedViewName);

            Assert.AreEqual(expectedViewName, result.ViewName);
        }

        [Test]
        public void Adds_Page_Title_To_Model()
        {
            const string expectedPageName = "Page name";

            Mock.Get(support.CurrentPage).Setup(c => c.Name).Returns(expectedPageName);

            var result = controller.Form("view");
            var model = (FormModel)result.Model;

            Assert.AreEqual(expectedPageName, model.PageName);
        }

        [Test]
        [TestCase("my@email.com", "A page")]
        [TestCase("my-other@email.com", "Another page")]
        public void Sends_Mail_When_Posted(string expectedEmail, string expectedPage)
        {
            var model = new FormModel
            {
                PageName = expectedPage,
                Email = expectedEmail
            };

            controller.Post(model);

            Mock.Get(mailGateway).Verify(gateway => gateway.Send(
                expectedEmail + ", hardcoded@email.com",
                "Welcome to our newsletter",
                "Hi, you subscribed with " + expectedEmail + " via " + expectedPage
            ));
        }
    }
}
