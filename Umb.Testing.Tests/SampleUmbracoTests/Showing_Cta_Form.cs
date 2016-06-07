using System;
using System.Linq;
using System.Web;
using Moq;
using NUnit.Framework;
using Umb.Testing.Web.Controllers;
using Umbraco.Core;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
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
    }
}
