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
using Umbraco.Web;

namespace Umb.Testing.Tests.SampleUmbracoTests
{
    [TestFixture]
    public class Getting_Backoffice_Data : BaseDatabaseFactoryTest
    {
        private SimpleAuthorizedController controller;
        private ServiceContext serviceContextMock;
        private UmbracoContext umbracoContext;

        public override void Initialize()
        {
            base.Initialize();
            umbracoContext = GetUmbracoContext("http://localhost", -1, null, true);
            controller = new SimpleAuthorizedController();
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
            var user = Mock.Of<IUser>();

            Mock.Get(user).Setup(u => u.Name).Returns(expectedName);
            Mock.Get(serviceContextMock.UserService).Setup(ctx => ctx.GetUserById(1)).Returns(user);
            Mock.Get(umbracoContext.HttpContext).Setup(ctx => ctx.User).Returns(new ClaimsPrincipal(CreateIdentity()));

            var result = controller.GetUserInfo();
            Console.WriteLine(result);
            Assert.AreEqual(expectedName, result);
        }

        protected override ApplicationContext CreateApplicationContext()
        {
            serviceContextMock = MockHelper.GetMockedServiceContext();
            return new ApplicationContext(
                new DatabaseContext(new Mock<IDatabaseFactory>().Object, Mock.Of<ILogger>(), Mock.Of<ISqlSyntaxProvider>(), "test"),
                serviceContextMock,
                CacheHelper.CreateDisabledCacheHelper(),
                new ProfilingLogger(Mock.Of<ILogger>(), Mock.Of<IProfiler>())
            );
        }

        private UmbracoBackOfficeIdentity CreateIdentity()
        {
            var sessionId = Guid.NewGuid().ToString();
            var userData = new UserData(sessionId)
            {
                AllowedApplications = new[] { "content", "media" },
                Culture = "en-us",
                Id = 1,
                RealName = "hello world",
                Roles = new[] { "admin" },
                StartContentNode = -1,
                StartMediaNode = 654,
                Username = "testing"
            };

            var identity = new UmbracoBackOfficeIdentity(userData);
            return identity;
        }
    }
}
