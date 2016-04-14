using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Moq;
using NUnit.Framework;
using Umb.Testing.Web.Controllers;
using Umb.Testing.Web.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Profiling;
using Umbraco.Core.Services;
using Umbraco.Tests.TestHelpers;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace Umb.Testing.Tests.MocksOnly
{
    [TestFixture]
    public class Adding_Numbers
    {
        private UmbracoContext umbracoContext;

        [SetUp]
        public void Setup()
        {
            var settings = SettingsForTests.GenerateMockSettings();
            SettingsForTests.ConfigureSettings(settings);

            var logger = Mock.Of<ILogger>();

            var applicationContext = ApplicationContext.EnsureContext(
                new DatabaseContext(
                    Mock.Of<IDatabaseFactory>(),
                    logger,
                    new SqlSyntaxProviders(new ISqlSyntaxProvider[0])
                    ),
                new ServiceContext(
                    contentService: Mock.Of<IContentService>() //, ...
                    ),
                CacheHelper.CreateDisabledCacheHelper(),
                new ProfilingLogger(logger, Mock.Of<IProfiler>()),
                true
            );

            var httpContext = Mock.Of<HttpContextBase>();

            umbracoContext = UmbracoContext.EnsureContext(
                httpContext,
                applicationContext,
                new WebSecurity(httpContext, applicationContext),
                settings,
                new List<IUrlProvider>(),
                true,
                false
            );
        }

        [Test]
        public void Returns_Sum()
        {
            var controller = new SimpleSurfaceController(umbracoContext, new UmbracoHelper(umbracoContext));

            var model = new AdditionModel
            {
                IsPosted = true,
                X = 3,
                Y = 5
            };
            var result = (AdditionModel)controller.AddForm(model).Model;
            Assert.AreEqual(8, result.Sum);
        }
    }
}
