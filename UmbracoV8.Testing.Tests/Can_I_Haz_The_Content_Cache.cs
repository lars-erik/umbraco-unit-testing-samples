using System;
using System.IO;
using System.Linq;
using CSharpTest.Net.Collections;
using Newtonsoft.Json;
using NUnit.Framework;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Tests.Testing;
using Umbraco.UnitTesting.Adapter;
using Umbraco.Web;
using Umbraco.Web.PublishedCache.NuCache;

namespace UmbracoV8.Testing.Tests
{

    [TestFixture]
    [UmbracoTest(TypeLoader = UmbracoTestOptions.TypeLoader.PerFixture, WithApplication = true)]
    public class Can_I_Haz_The_Content_Cache //: PublishedContentSnapshotTestBase
    {
        private UmbracoSupport support;

        private BPlusTree<int, ContentNodeKit> allContentKits;
        private ContentCacheSupport contentCacheSupport;

        public Can_I_Haz_The_Content_Cache()
        {
            UmbracoSupport.RegisterForTesting(this);
        }

        [SetUp]
        public void SetUp()
        {
            contentCacheSupport = new ContentCacheSupport();
            allContentKits = contentCacheSupport.GetFromCacheFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\UmbracoV8.Testing.Web\App_Data\TEMP\NuCache\NuCache.Content.db"));

            support = new UmbracoSupport(allContentKits);
            SetupContentTypes();
            support.SetupUmbraco();
        }

        [TearDown]
        public void TearDown()
        {
            support.DisposeUmbraco();
            allContentKits.Dispose();
        }

        [Test]
        public void Write_Home_Node_From_Content_Cache()
        {
            var ctx = support.GetUmbracoContext("/", setSingleton:true);
            var root = ctx.Content.GetById(1104);

            Console.WriteLine(JsonConvert.SerializeObject(root, 
                new JsonSerializerSettings { 
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented
                }));
        }

        [Test]
        public void Write_First_Child_Node_From_Content_Cache()
        {
            var ctx = support.GetUmbracoContext("/", setSingleton:true);
            var firstChild = ctx.Content.GetById(1104).FirstChild();

            Console.WriteLine(JsonConvert.SerializeObject(firstChild, 
                new JsonSerializerSettings { 
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented
                }));
        }

        [Test]
        [Explicit]
        public void Write_All_Content_Kits()
        {
            Console.WriteLine(JsonConvert.SerializeObject(allContentKits.Select(x => new{x.Key, x.Value.ContentTypeId, x.Value.PublishedData.Name}), Formatting.Indented));
        }

        private void SetupContentTypes()
        {
            var noProps = Enumerable.Empty<IPublishedPropertyType>();

            var titleProp = support.CreatePropertyType("pageTitle", 1);
            var bodyProp = support.CreatePropertyType("bodyText", 1);
            var contentBaseProps = new[] { titleProp, bodyProp };
            //contentType = support.CreateContentType(1, "page", ct => contentBaseProps);

            // Compositions
            // 1092 Content Base
            // 1099 Feature
            // 1093 Navigation Base
            //
            // Content
            // 1094 Products
            // 1095 Product
            // 1096 Person
            // 1097 People
            // 1098 Home
            // 1100 Content Page
            // 1101 Contact
            // 1102 Blogpost
            // 1103 Blog

            support.CreateContentType(1092, "contentBase", ct => contentBaseProps);
            support.CreateContentType(1099, "feature", ct => noProps);
            support.CreateContentType(1093, "navigationBase", ct => noProps);
            
            support.CreateContentType(1094, "products", ct => noProps);
            support.CreateContentType(1095, "product", ct => noProps);
            support.CreateContentType(1096, "person", ct => noProps);
            support.CreateContentType(1097, "people", ct => noProps);
            support.CreateContentType(1098, "home", ct => noProps);
            support.CreateContentType(1100, "contentPage", ct => noProps);
            support.CreateContentType(1101, "contact", ct => noProps);
            support.CreateContentType(1102, "blogpost", ct => noProps);
            support.CreateContentType(1103, "blog", ct => noProps);

        }
    }
}
