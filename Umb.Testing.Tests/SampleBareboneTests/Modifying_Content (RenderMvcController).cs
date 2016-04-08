using System;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using Umb.Testing.Web.Controllers;
using Umb.Testing.Web.Models;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Tests.TestHelpers;
using Umbraco.Web.Models;
using Umbraco.Web.Routing;

namespace Umb.Testing.Tests.SampleBareboneTests
{
    [TestFixture]
    public class Modifying_Content : BaseRoutingTest
    {
        private Mock<ControllerContext> controllerContextMock;

        [SetUp]
        public void Setup()
        {
            // Mocked settings are now necessary
            SettingsForTests.ConfigureSettings(SettingsForTests.GenerateMockSettings());

            // Routing context necessary for published content request
            var routeData = new RouteData();
            var routingContext = GetRoutingContext("http://localhost", 1, routeData, true, UmbracoConfig.For.UmbracoSettings());
            var ctx = routingContext.UmbracoContext;

            // Published content request necessary for rendermodel simple ctor (avoid culture)
            ctx.PublishedContentRequest = new PublishedContentRequest(
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

            // ControllerContext mock also necessary for template evaluation in base RenderMvcController
            controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.Setup(c => c.RouteData).Returns(routeData);

            routeData.Values.Add("controller", "Content");
            routeData.Values.Add("action", "Content");
        }

        [Test]
        public void From_RenderMvcController()
        {
            var content = Mock.Of<IPublishedContent>();

            var controller = new ContentController
            {
                ControllerContext = controllerContextMock.Object
            };

            // Example: Not specifying culture here blows up the setup method
            var result = (ViewResult)controller.Index(new RenderModel(content));

            var resultModel = (RenderModel<ContentModel>)result.Model;
            Assert.AreEqual("Hello from controller", resultModel.Content.MessageFromController);
        }
    }
}
