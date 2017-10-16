using System;
using System.Security.Claims;
using System.Security.Principal;
using Moq;
using NUnit.Framework;
using Umb.Testing.Web.Controllers;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Profiling;
using Umbraco.Core.Security;
using Umbraco.Core.Services;
using Umbraco.Tests.TestHelpers;
using Umbraco.UnitTesting.Adapter.Support;
using Umbraco.Web;

namespace Umb.Testing.Tests.SampleUmbracoTests
{
    [TestFixture]
    public class Getting_Backoffice_Data
    {
        private const int StubbedUserId = 1;
        private SimpleAuthorizedController controller;
        private ServiceContext serviceContext;

        private UmbracoContext umbracoContext;

        private UmbracoSupport umbracoSupport = new UmbracoSupport();

        [SetUp]
        public void Initialize()
        {
            umbracoSupport.SetupUmbraco();
            umbracoContext = umbracoSupport.UmbracoContext;
            serviceContext = umbracoSupport.ServiceContext;

            controller = new SimpleAuthorizedController();
        }

        [TearDown]
        public void TearDown()
        {
            umbracoSupport.DisposeUmbraco();
        }

        [Test]
        public void Via_Authorized_Api_Controller()
        {
            Assert.That(controller.List(), Is.EquivalentTo(new[] {"A", "B"}));
        }

        [Test]
        public void Via_Auth_That_Checks_User()
        {
            const string expectedName = "expected name";
            Mock.Get(umbracoContext.HttpContext).Setup(ctx => ctx.User).Returns(new ClaimsPrincipal(CreateIdentity(StubbedUserId)));

            var user = Mock.Of<IUser>();
            Mock.Get(user).Setup(u => u.Name).Returns(expectedName);

            Mock.Get(serviceContext.UserService).Setup(ctx => ctx.GetUserById(StubbedUserId)).Returns(user);

            var result = controller.GetUserInfo();
            
            Console.WriteLine(result);
            Assert.AreEqual(expectedName, result);
        }

        //protected override ApplicationContext CreateApplicationContext()
        //{
        //    serviceContext = MockHelper.GetMockedServiceContext();
        //    return new ApplicationContext(
        //        new DatabaseContext(new Mock<IDatabaseFactory>().Object, Mock.Of<ILogger>(), Mock.Of<ISqlSyntaxProvider>(), "test"),
        //        serviceContext,
        //        CacheHelper.CreateDisabledCacheHelper(),
        //        new ProfilingLogger(Mock.Of<ILogger>(), Mock.Of<IProfiler>())
        //    );
        //}

        private UmbracoBackOfficeIdentity CreateIdentity(int userId)
        {
            var sessionId = Guid.NewGuid().ToString();
            var userData = new UserData(sessionId)
            {
                AllowedApplications = new[] { "content", "media" },
                Culture = "en-us",
                Id = userId,
                RealName = "hello world",
                Roles = new[] { "admin" },
                StartContentNodes = new[] { -1 },
                StartMediaNodes = new [] { 654 },
                Username = "testing"
            };

            var identity = new UmbracoBackOfficeIdentity(userData);
            return identity;
        }
    }
}
