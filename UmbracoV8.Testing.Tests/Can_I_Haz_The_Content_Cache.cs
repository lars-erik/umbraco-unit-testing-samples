using System;
using System.Web;
using NUnit.Framework;
using Umbraco.Web.Composing;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Tests.PublishedContent;
using Umbraco.Tests.TestHelpers;
using Umbraco.Tests.Testing;
using Umbraco.Web;

namespace UmbracoV8.Testing.Tests
{
    [TestFixture]
    [UmbracoTest(TypeLoader = UmbracoTestOptions.TypeLoader.PerFixture)]
    public class Can_I_Haz_The_Content_Cache : PublishedContentSnapshotTestBase
    {
        public Can_I_Haz_The_Content_Cache()
        {
            // TODO: Move to onetime setup
            TestOptionAttributeBase.ScanAssemblies.Clear();
            TestOptionAttributeBase.ScanAssemblies.Add(GetType().Assembly.FullName);
        }

        [Test]
        public void Yes_I_Can()
        {
            var content = Current.UmbracoContext.ContentCache.GetById(2);

            Console.WriteLine(content.Value<IHtmlString>("body"));
            Assert.AreEqual(
                "<p>Lorem ipsum dolor etc. what do I know.</p>", 
                content.Value<IHtmlString>("body").ToHtmlString()
            );
        }

        protected override void PopulateCache(PublishedContentTypeFactory factory, SolidPublishedContentCache cache)
        {
            var titleProp = factory.CreatePropertyType("title", 1);
            var bodyProp = factory.CreatePropertyType("body", 1);

            var contentType = factory.CreateContentType(1, "page", new[] {titleProp, bodyProp});

            var content = new SolidPublishedContent(contentType)
            {
                Name = "My content",
                Id = 2,
                Key = new Guid("EFD46B40-662C-4A06-9C77-0C2CE9F22828"),
                Properties = new []
                {
                    new SolidPublishedProperty
                    {
                        Alias = "title",
                        PropertyType = titleProp,
                        SolidSourceValue = "<h1>Hi there!</h1>",
                        SolidValue = new HtmlString("<h1>Hi there!</h1>")
                    }, 
                    new SolidPublishedProperty
                    {
                        Alias = "body",
                        PropertyType = titleProp,
                        SolidSourceValue = "<p>Lorem ipsum dolor etc. what do I know.</p>",
                        SolidValue = new HtmlString("<p>Lorem ipsum dolor etc. what do I know.</p>")
                    }, 
                }
            };

            cache.Add(content);
        }
    }
}
