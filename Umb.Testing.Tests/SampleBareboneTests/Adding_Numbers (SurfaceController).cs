using NUnit.Framework;
using Umb.Testing.Web.Controllers;
using Umb.Testing.Web.Models;
using Umbraco.Tests.TestHelpers;

namespace Umb.Testing.Tests.SampleBareboneTests
{
    [TestFixture]
    public class Adding_Numbers : BaseWebTest
    {
        [SetUp]
        public void Setup()
        {
            GetUmbracoContext("http://localhost", -1, null, true);

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
