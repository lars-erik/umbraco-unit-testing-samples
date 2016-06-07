using System;
using System.Linq;
using System.Web;
using Moq;
using NUnit.Framework;
using Umb.Testing.Web.Controllers;
using Umb.Testing.Web.Models;
using Umbraco.Core;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Profiling;
using Umbraco.Tests.TestHelpers;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace Umb.Testing.Tests.SampleUmbracoTests
{
    [TestFixture]
    public class Showing_Cta_Form : BaseRoutingTest
    {
        private Mock<IPublishedContent> contentMock;
        private CtaFormController controller;

        [SetUp]
        public void SetUp()
        {
            contentMock = new Mock<IPublishedContent>();

            var settings = SettingsForTests.GenerateMockSettings();
            var routingContext = GetRoutingContext("http://localhost", 0, umbracoSettings:settings);
            var helper = new UmbracoHelper(routingContext.UmbracoContext, contentMock.Object);

            controller = new CtaFormController(routingContext.UmbracoContext, helper);
        }

        [Test]
        public void Should_Return_The_Specified_View()
        {
            const string expectedViewName = "AnonymousVisitor";

            var result = controller.Form(expectedViewName);

            Assert.AreEqual(expectedViewName, result.ViewName);
        }

        [Test]
        public void Adds_The_Name_Of_The_Current_Page_To_The_Form()
        {
            const string expectedPageName = "A page";
            contentMock.Setup(c => c.Name).Returns(expectedPageName);

            var result = controller.Form("view");
            var model = (FormModel) result.Model;

            Assert.AreEqual(expectedPageName, model.PageName);
        }
    }
}
