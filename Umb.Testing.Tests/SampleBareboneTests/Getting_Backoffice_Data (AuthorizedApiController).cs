using System;
using NUnit.Framework;
using Umb.Testing.Web.Controllers;
using Umbraco.Tests.TestHelpers;
using Umbraco.Web;

namespace Umb.Testing.Tests.SampleBareboneTests
{
    [TestFixture]
    public class Getting_Backoffice_Data : BaseWebTest
    {
        private SimpleAuthorizedController controller;

        [SetUp]
        public void Setup()
        {
            GetUmbracoContext("http://localhost", -1, null, true);
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
            var x = UmbracoContext.Current.Security.CurrentUser;
            var result = controller.GetUserInfo();
            Console.WriteLine(result);
        }
    }
}
