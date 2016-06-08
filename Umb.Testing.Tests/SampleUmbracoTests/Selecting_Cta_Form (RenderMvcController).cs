using System;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using Umb.Testing.Web.Controllers;
using Umb.Testing.Web.Models;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Tests.TestHelpers;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Routing;

namespace Umb.Testing.Tests.SampleUmbracoTests
{
    [TestFixture]
    public class Selecting_Cta_Form : BaseRoutingTest
    {
        private Mock<ControllerContext> controllerContextMock;
        private UmbracoContext umbracoContext;
        private ContentController contentController;
        private IPublishedContent content;

        [SetUp]
        public void Setup()
        {
            // Mocked settings are now necessary
            SettingsForTests.ConfigureSettings(SettingsForTests.GenerateMockSettings());

            // Routing context necessary for published content request
            var routeData = new RouteData();
            var routingContext = GetRoutingContext("http://localhost", 1, routeData, true, UmbracoConfig.For.UmbracoSettings());
            umbracoContext = routingContext.UmbracoContext;

            // Published content request necessary for rendermodel simple ctor (avoid culture)
            umbracoContext.PublishedContentRequest = new PublishedContentRequest(
                new Uri("http://localhost"),
                routingContext,
                UmbracoConfig.For.UmbracoSettings().WebRouting,
                s => new string[0]
                );

            // ViewEngine mock necessary for template evaluation in base RenderMvcController
            var viewEngineMock = new Mock<IViewEngine>();
            viewEngineMock
                .Setup(e => e.FindView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new ViewEngineResult(Mock.Of<IView>(), viewEngineMock.Object));
            // Clearing necessary to avoid MVC dependency hell
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(viewEngineMock.Object);

            // ControllerContext mock also necessary for HttpContext and template evaluation in base RenderMvcController
            controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.Setup(c => c.RouteData).Returns(routeData);
            controllerContextMock.Setup(c => c.HttpContext).Returns(umbracoContext.HttpContext);

            routeData.Values.Add("controller", "Content");
            routeData.Values.Add("action", "Content");

            // Current page
            content = Mock.Of<IPublishedContent>();

            // And controller
            contentController = new ContentController(umbracoContext)
            {
                ControllerContext = controllerContextMock.Object
            };
        }

        [Test]
        public void For_Anonymous_User()
        {
            Mock.Get(umbracoContext.HttpContext)
                .Setup(c => c.User)
                .Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));

            var result = (ViewResult)contentController.Index(new RenderModel(content));

            var resultModel = (RenderModel<ContentModel>)result.Model;
            Assert.AreEqual("AnonymousForm", resultModel.Content.CtaForm);
        }

        [Test]
        public void For_Authenticated_User()
        {
            Mock.Get(umbracoContext.HttpContext)
                .Setup(c => c.User)
                .Returns(new GenericPrincipal(new GenericIdentity("user"), new string[0]));

            var result = (ViewResult)contentController.Index(new RenderModel(content));

            var resultModel = (RenderModel<ContentModel>)result.Model;
            Assert.AreEqual("AuthenticatedForm", resultModel.Content.CtaForm);
        }
    }
}
