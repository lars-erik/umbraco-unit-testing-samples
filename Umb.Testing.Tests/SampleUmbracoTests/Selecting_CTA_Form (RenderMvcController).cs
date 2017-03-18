using System;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using Umb.Testing.Tests.Support;
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
    public class Selecting_Cta_Form
    {
        private UmbracoContext umbracoContext;
        private ContentController contentController;
        private IPublishedContent content;

        readonly UmbracoSupport umbracoSupport = new UmbracoSupport();

        [SetUp]
        public void Setup()
        {
            umbracoSupport.SetupUmbraco();
            umbracoContext = umbracoSupport.UmbracoContext;
            content = umbracoSupport.CurrentPage;

            contentController = new ContentController(umbracoContext);
            umbracoSupport.PrepareController(contentController);

            // ViewEngine mock necessary for template evaluation in base RenderMvcController
            var viewEngineMock = new Mock<IViewEngine>();
            viewEngineMock
                .Setup(e => e.FindView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new ViewEngineResult(Mock.Of<IView>(), viewEngineMock.Object));
            // Clearing necessary to avoid MVC dependency hell
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(viewEngineMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            umbracoSupport.DisposeUmbraco();
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

            // Example: Not specifying culture here blows up the setup method
            var result = (ViewResult)contentController.Index(new RenderModel(content));

            var resultModel = (RenderModel<ContentModel>)result.Model;
            Assert.AreEqual("AuthenticatedForm", resultModel.Content.CtaForm);
        }
    }
}
