using NUnit.Framework;
using Umb.Testing.Web.Controllers;
using Umb.Testing.Web.Models;
using Umbraco.Tests.TestHelpers;
using Umbraco.UnitTesting.Adapter.Support;

namespace Umb.Testing.Tests.SampleUmbracoTests
{
    [TestFixture]
    public class Adding_Numbers
    {
        private UmbracoSupport support = new UmbracoSupport();

        [SetUp]
        public void Setup()
        {
            support.SetupUmbraco();

            /*
            Stubbing the UmbracoContext:

            var appCtx = new ApplicationContext(
                CacheHelper.CreateDisabledCacheHelper(),
                new ProfilingLogger(Mock.Of<ILogger>(), Mock.Of<IProfiler>()));

            var umbCtx = UmbracoContext.EnsureContext(
                new Mock<HttpContextBase>().Object,
                appCtx,
                new Mock<WebSecurity>(null, null).Object,
                Mock.Of<IUmbracoSettingsSection>(),
                Enumerable.Empty<IUrlProvider>(),
                true);

            var ctrl = new SimpleSurfaceController(umbCtx);
            */
        }

        [TearDown]
        public void TearDown()
        {
            support.DisposeUmbraco();
        }

        [Test]
        public void Posting_AddModel_Calculates_Result()
        {
            const int expectedSum = 3;
            var model = new AdditionModel
            {
                X = 1,
                Y = 2,
                IsPosted = true
            };

            var controller = new SimpleSurfaceController();
            var result = controller.AddForm(model);
            var resultModel = (AdditionModel)result.Model;

            Assert.AreEqual(expectedSum, resultModel.Sum);
        }
    }


}
