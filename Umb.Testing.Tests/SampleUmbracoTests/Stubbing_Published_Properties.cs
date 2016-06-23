using Moq;
using NUnit.Framework;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Umb.Testing.Tests.SampleUmbracoTests
{
    [TestFixture]
    public class Stubbing_Published_Properties
    {
        [Test]
        public void For_GetPropertyValueOfType()
        {
            var contentStub = new Mock<IPublishedContent>();
            var propertyStub = new Mock<IPublishedProperty>();

            propertyStub.Setup(p => p.Value).Returns(new ValueType("a text", 15));

            // GetPropertyValue<T> calls GetProperty and checks whether source is of T, so just skip converters.
            contentStub.Setup(c => c.GetProperty("test", false)).Returns(propertyStub.Object);

            var content = contentStub.Object;
            var propertyValue = content.GetPropertyValue<ValueType>("test");

            Assert.AreEqual("a text", propertyValue.Text);
            Assert.AreEqual(15, propertyValue.Number);
        }
    }

    public class ValueType
    {
        public string Text { get; set; }
        public int Number { get; set; }

        public ValueType(string text, int number)
        {
            Text = text;
            Number = number;
        }
    }
}
