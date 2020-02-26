using System;
using System.IO;
using System.Linq;
using System.Web;
using CSharpTest.Net.Collections;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Umbraco.Web.Composing;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Tests.PublishedContent;
using Umbraco.Tests.TestHelpers;
using Umbraco.Tests.Testing;
using Umbraco.UnitTesting.Adapter;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.PublishedCache.NuCache;

namespace UmbracoV8.Testing.Tests
{

    [TestFixture]
    [UmbracoTest(TypeLoader = UmbracoTestOptions.TypeLoader.PerFixture, WithApplication = true)]
    public class Can_I_Haz_The_Content_Cache //: PublishedContentSnapshotTestBase
    {
        private UmbracoSupport support;

        private IPublishedContentType contentType;
        private IPublishedPropertyType titleProp;
        private IPublishedPropertyType bodyProp;
        private SolidPublishedContent solidContent;
        private BPlusTree<int, ContentNodeKit> allContentKits;
        private ContentCacheSupport contentCacheSupport;

        public Can_I_Haz_The_Content_Cache()
        {
            UmbracoSupport.RegisterForTesting(this);
            
            contentCacheSupport = new ContentCacheSupport();
            allContentKits = contentCacheSupport.GetFromCacheFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\UmbracoV8.Testing.Web\App_Data\TEMP\NuCache\NuCache.Content.db"));

            support = new UmbracoSupport(allContentKits);
        }

        [SetUp]
        public void SetUp()
        {
            // TODO: Move to onetime setup

            support.SetupUmbraco();
            support.SetupSnapshot();

            SetupContentType();
            //StubContentBySolidPublishedContent();
        }

        //[Test]
        //public void From_Stubbed_SolidContent()
        //{
        //    var content = Current.UmbracoContext.ContentCache.GetById(2);

        //    Console.WriteLine(content.Value<IHtmlString>("body"));
        //    Assert.AreEqual(
        //        "<p>Lorem ipsum dolor etc. what do I know.</p>", 
        //        content.Value<IHtmlString>("body").ToHtmlString()
        //    );
        //}

        [TearDown]
        public void TearDown()
        {
            support.DisposeUmbraco();
        }

        [Test]
        public void From_Json_Serialized_Data_Via_The_NuCache_File()
        {
            var homeNode = allContentKits.First().Value;

            var firstPublished = support.CreatePublishedContent(homeNode, contentType);

            Console.WriteLine(firstPublished.Value("bodyText"));

            Console.WriteLine(JsonConvert.SerializeObject(homeNode, Formatting.Indented));

            Console.WriteLine(JsonConvert.SerializeObject(firstPublished, Formatting.Indented));
        }

        [Test]
        public void The_Whole_List()
        {
            var allPublished = allContentKits.Select(x => support.CreatePublishedContent(x.Value, contentType));
            Console.WriteLine(JsonConvert.SerializeObject(allContentKits, Formatting.Indented));
            Console.WriteLine(JsonConvert.SerializeObject(allPublished, Formatting.Indented));
        }

        [Test]
        public void Do_We_Have_Children()
        {
            var allPublished = allContentKits.Select(x => support.CreatePublishedContent(x.Value, contentType));
            var home = allPublished.First();
            var children = home.Children();
            Console.WriteLine(JsonConvert.SerializeObject(children, Formatting.Indented));
        }

        //private void StubContentBySolidPublishedContent()
        //{
        //    solidContent = new SolidPublishedContent(contentType)
        //    {
        //        Name = "My content",
        //        Id = 2,
        //        Key = new Guid("EFD46B40-662C-4A06-9C77-0C2CE9F22828"),
        //        Properties = new[]
        //        {
        //            new SolidPublishedProperty
        //            {
        //                Alias = "title",
        //                PropertyType = titleProp,
        //                SolidSourceValue = "<h1>Hi there!</h1>",
        //                SolidValue = new HtmlString("<h1>Hi there!</h1>")
        //            },
        //            new SolidPublishedProperty
        //            {
        //                Alias = "body",
        //                PropertyType = bodyProp,
        //                SolidSourceValue = "<p>Lorem ipsum dolor etc. what do I know.</p>",
        //                SolidValue = new HtmlString("<p>Lorem ipsum dolor etc. what do I know.</p>")
        //            },
        //        }
        //    };

        //    fakeContentCache.Add(solidContent);
        //}

        private void SetupContentType()
        {
            titleProp = support.CreatePropertyType("pageTitle", 1);
            bodyProp = support.CreatePropertyType("bodyText", 1);
            contentType = support.CreateContentType(1, "page", ct => new[] {titleProp, bodyProp});
        }

        //public override void PopulateCache(PublishedContentTypeFactory factory, SolidPublishedContentCache cache)
        //{
        //    return;
        //}
    }
}
