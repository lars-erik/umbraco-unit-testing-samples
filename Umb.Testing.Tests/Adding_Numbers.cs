using System.Web.Mvc;
using NUnit.Framework;
using Umb.Testing.Web.Controllers;
using Umb.Testing.Web.Models;
using Umbraco.Tests.TestHelpers;
using Umbraco.Web;

namespace Umb.Testing.Tests
{
    [TestFixture]
    public class Adding_Numbers : BaseWebTest
    {
        [SetUp]
        public void Setup()
        {
            GetUmbracoContext("http://localhost", -1, null, true);
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
