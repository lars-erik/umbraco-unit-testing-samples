using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using umbraco;
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
        private HttpContextBase httpContext;
        private CtaFormSelection selection;

        [SetUp]
        public void Setup()
        {
            httpContext = Mock.Of<HttpContextBase>();
            selection = new CtaFormSelection(httpContext);
        }

        [Test]
        public void For_Anonymous_User()
        {
            Mock.Get(httpContext)
                .Setup(c => c.User)
                .Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));

            var contentModel = new ContentModel(Mock.Of<IPublishedContent>());

            selection.SelectForm(contentModel);

            Assert.AreEqual("AnonymousForm", contentModel.CtaForm);
        }

        [Test]
        public void For_Authenticated_User()
        {
            Mock.Get(httpContext)
                .Setup(c => c.User)
                .Returns(new GenericPrincipal(new GenericIdentity("user"), new string[0]));


            var contentModel = new ContentModel(Mock.Of<IPublishedContent>());

            selection.SelectForm(contentModel);

            Assert.AreEqual("AuthenticatedForm", contentModel.CtaForm);
        }
    }
}
