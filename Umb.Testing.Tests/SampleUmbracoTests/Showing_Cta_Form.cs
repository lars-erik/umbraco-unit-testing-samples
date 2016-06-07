using NUnit.Framework;
using Umb.Testing.Web.Controllers;

namespace Umb.Testing.Tests.SampleUmbracoTests
{
    [TestFixture]
    public class Showing_Cta_Form
    {
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
