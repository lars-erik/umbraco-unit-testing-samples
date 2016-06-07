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
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace Umb.Testing.Tests.SampleUmbracoTests
{
    [TestFixture]
    public class Showing_Cta_Form
    {
        [SetUp]
        public void SetUp()
        {
            var applicationContext = new ApplicationContext(
                CacheHelper.CreateDisabledCacheHelper(),
                new ProfilingLogger(Mock.Of<ILogger>(), Mock.Of<IProfiler>())
            );

            UmbracoContext.EnsureContext(
                Mock.Of<HttpContextBase>(),
                applicationContext,
                new WebSecurity(Mock.Of<HttpContextBase>(), applicationContext),
                Mock.Of<IUmbracoSettingsSection>(),
                Enumerable.Empty<IUrlProvider>(),
                true
            );
        }

        [Test]
        public void Should_Return_The_Specified_View()
        {
            const string expectedViewName = "AnonymousVisitor";
            var controller = new CtaFormController();
            var result = controller.Form(expectedViewName);
            Assert.AreEqual(expectedViewName, result.ViewName);
        }

        [Test]
        public void Adds_The_Name_Of_The_Current_Page_To_The_Form()
        {
            const string expectedPageName = "A page";
            var contentMock = new Mock<IPublishedContent>();
            contentMock.Setup(c => c.Name).Returns(expectedPageName);

            var helper = new UmbracoHelper(UmbracoContext.Current, contentMock.Object);

            var controller = new CtaFormController(UmbracoContext.Current, helper);
            var result = controller.Form("view");
            var model = (FormModel) result.Model;

            Assert.AreEqual(expectedPageName, model.PageName);
        }
    }
}
