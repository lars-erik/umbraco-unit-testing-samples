using Moq;
using NUnit.Framework;
using Umb.Testing.Web.Controllers;
using Umb.Testing.Web.Models;
using Umbraco.Core.Models;
using Umbraco.Tests.TestHelpers;
using Umbraco.Web;

namespace Umb.Testing.Tests.SampleUmbracoTests
{
    [TestFixture]
    public class CTA_Form_Controller : BaseRoutingTest
    {
        private CtaFormController controller;
        private UmbracoContext umbracoContext;

        #region step 2
        private UmbracoHelper helper;
        private IPublishedContent content;

        #region step 3
        private IMailGateway mailGateway;
        #endregion
        #endregion

        [SetUp]
        public void Setup()
        {
            // Step 1 - running dependencyless logic
            umbracoContext = GetUmbracoContext("http://localhost", -1);

            #region step 2
            // Step 2 - be able to create an UmbracoHelper
            var routingContext = GetRoutingContext("http://localhost", -1, umbracoSettings:SettingsForTests.GenerateMockSettings());
            umbracoContext = routingContext.UmbracoContext;

            #region step 3
            // Step 3 - stub content
            content = Mock.Of<IPublishedContent>();
            helper = new UmbracoHelper(umbracoContext, content);

            #region step 4
            // Step 4 - create mail gateway mock
            mailGateway = Mock.Of<IMailGateway>();

            #endregion
            #endregion
            #endregion

            // Finally - create controller
            controller = new CtaFormController(umbracoContext, helper, mailGateway);
        }

        [Test]
        public void Shows_Specified_View()
        {
            const string expectedViewName = "NewLead";

            var result = controller.Form(expectedViewName);

            Assert.AreEqual(expectedViewName, result.ViewName);
        }

        #region step 2

        [Test]
        public void Adds_Page_Title_To_Model()
        {
            const string expectedPageName = "Page name";

            Mock.Get(content).Setup(c => c.Name).Returns(expectedPageName);

            var result = controller.Form("view");
            var model = (FormModel)result.Model;

            Assert.AreEqual(expectedPageName, model.PageName);
        }

        #region step 3

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

        #endregion
        #endregion
    }
}
