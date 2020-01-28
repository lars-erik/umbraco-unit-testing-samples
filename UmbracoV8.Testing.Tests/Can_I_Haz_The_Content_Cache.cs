using System;
using System.IO;
using System.Linq;
using System.Web;
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
    using CacheKeys = Umbraco.Web.PublishedCache.NuCache.CacheKeys;

    [TestFixture]
    [UmbracoTest(TypeLoader = UmbracoTestOptions.TypeLoader.PerFixture)]
    public class Can_I_Haz_The_Content_Cache : PublishedContentSnapshotTestBase
    {
        private IPublishedContentType contentType;
        private PublishedContentTypeFactory publishedContentTypeFactory;
        private IPublishedPropertyType titleProp;
        private IPublishedPropertyType bodyProp;
        private SolidPublishedContent solidContent;
        private IPublishedSnapshot snapshot;
        private DictionaryAppCache appCache;
        private SolidPublishedContentCache fakeContentCache;

        public Can_I_Haz_The_Content_Cache()
        {
            // TODO: Move to onetime setup
            TestOptionAttributeBase.ScanAssemblies.Clear();
            TestOptionAttributeBase.ScanAssemblies.Add(GetType().Assembly);

            var dataTypeService = new TestObjects.TestDataTypeService(
                new DataType(new VoidEditor(Mock.Of<ILogger>())) { Id = 1 });

            publishedContentTypeFactory = new PublishedContentTypeFactory(Mock.Of<IPublishedModelFactory>(), new PropertyValueConverterCollection(Array.Empty<IPropertyValueConverter>()), dataTypeService);
            
            appCache = new DictionaryAppCache();
            fakeContentCache = new SolidPublishedContentCache();

            snapshot = Mock.Of<IPublishedSnapshot>();
            Mock.Get(snapshot).Setup(x => x.SnapshotCache).Returns(appCache);
            Mock.Get(snapshot).Setup(x => x.Content).Returns(fakeContentCache);
        }

        public override void SetUp()
        {
            base.SetUp();

            SetupContentType();
            StubContentBySolidPublishedContent();
        }

        [Test]
        public void From_Stubbed_SolidContent()
        {
            var content = Current.UmbracoContext.ContentCache.GetById(2);

            Console.WriteLine(content.Value<IHtmlString>("body"));
            Assert.AreEqual(
                "<p>Lorem ipsum dolor etc. what do I know.</p>", 
                content.Value<IHtmlString>("body").ToHtmlString()
            );
        }

        [Test]
        public void From_Json_Serialized_Data_Via_The_NuCache_File()
        {

            var publishedSnapshotAccessor = Current.Factory.GetInstance<IPublishedSnapshotAccessor>();
            publishedSnapshotAccessor.PublishedSnapshot = snapshot;
            
            appCache.Items.AddOrUpdate(CacheKeys.ProfileName(-1), "User", (s, o) => "User");

            var support = new ContentCacheSupport();
            var allContent = support.GetFromCacheFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\UmbracoV8.Testing.Web\App_Data\TEMP\NuCache\NuCache.Content.db"));
            var homeNode = allContent.First().Value;
            homeNode.Node.ContentType = contentType;

            var firstPublished = new PublishedContent(
                homeNode.Node, 
                homeNode.PublishedData,
                publishedSnapshotAccessor,
                Current.Factory.GetInstance<IVariationContextAccessor>()
            );

            Console.WriteLine(JsonConvert.SerializeObject(homeNode, Formatting.Indented));

            Console.WriteLine(JsonConvert.SerializeObject(firstPublished, Formatting.Indented));
        }

        private void StubContentBySolidPublishedContent()
        {
            solidContent = new SolidPublishedContent(contentType)
            {
                Name = "My content",
                Id = 2,
                Key = new Guid("EFD46B40-662C-4A06-9C77-0C2CE9F22828"),
                Properties = new[]
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
                        PropertyType = bodyProp,
                        SolidSourceValue = "<p>Lorem ipsum dolor etc. what do I know.</p>",
                        SolidValue = new HtmlString("<p>Lorem ipsum dolor etc. what do I know.</p>")
                    },
                }
            };

            fakeContentCache.Add(solidContent);
        }

        private void SetupContentType()
        {
            titleProp = publishedContentTypeFactory.CreatePropertyType("title", 1);
            bodyProp = publishedContentTypeFactory.CreatePropertyType("body", 1);

            contentType = publishedContentTypeFactory.CreateContentType(1, "page", ct => new[] {titleProp, bodyProp});
        }

        public override void PopulateCache(PublishedContentTypeFactory publishedContentTypeFactory, SolidPublishedContentCache cache)
        {
            // We hijack all the stuff for now.
            // Might climb the inheritance hierarchy and get rid of it
        }
    }
}
