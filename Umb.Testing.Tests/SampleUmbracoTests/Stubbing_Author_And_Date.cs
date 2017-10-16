using System;
using Moq;
using NUnit.Framework;
using Umb.Testing.Web.Controllers;
using Umb.Testing.Web.Models;
using Umbraco.Core.Models;
using Umbraco.Tests.TestHelpers;
using Umbraco.UnitTesting.Adapter.Support;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace Umb.Testing.Tests.SampleUmbracoTests
{
    [TestFixture]
    public class Stubbing_Author_And_Date
    {
        private IPublishedContent content;
        private BylineController bylineController;

        readonly UmbracoSupport umbracoSupport = new UmbracoSupport();

        [SetUp]
        public void SetUp()
        {
            umbracoSupport.SetupUmbraco();
            content = umbracoSupport.CurrentPage;

            bylineController = new BylineController(umbracoSupport.UmbracoContext, umbracoSupport.UmbracoHelper);
        }

        [TearDown]
        public void TearDown()
        {
            umbracoSupport.DisposeUmbraco();
        }

        [Test]
        public void Built_In_Properties_Are_Used_By_Default()
        {
            const string expectedAuthor = "An author";
            const string expectedDate = "2015-12-31";
            StubContent(expectedAuthor, expectedDate);
            var result = bylineController.Byline();
            var model = (BylineModel) result.Model;

            Assert.AreEqual(expectedAuthor, model.Author);
            Assert.AreEqual(expectedDate, model.Date.Value.ToString("yyyy-MM-dd"));
        }

        [Test]
        public void Content_Properties_Override_Bultin_Properties()
        {
            const string expectedAuthor = "An author";
            const string expectedDate = "2015-12-31";

            StubContent("Another author", "2016-01-01");
            StubProperty("author", expectedAuthor);
            StubProperty("date", expectedDate);

            var result = bylineController.Byline();
            var model = (BylineModel) result.Model;

            Assert.AreEqual(expectedAuthor, model.Author);
            Assert.AreEqual(expectedDate, model.Date.Value.ToString("yyyy-MM-dd"));
        }

        private void StubContent(string expectedAuthor, string expectedDate)
        {
            var contentMock = Mock.Get(content);
            contentMock.Setup(c => c.WriterName).Returns(expectedAuthor);
            contentMock.Setup(c => c.CreateDate).Returns(DateTime.Parse(expectedDate));
        }

        private void StubProperty<T>(string alias, T value)
        {
            var prop = new Mock<IPublishedProperty>();
            prop.Setup(p => p.Value).Returns(value);
            Mock.Get(content).Setup(c => c.GetProperty(alias, false)).Returns(prop.Object);
        }
    }
}

