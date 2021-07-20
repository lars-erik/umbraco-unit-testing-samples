using System;
using System.Linq;
using CSharpTest.Net.Collections;
using Newtonsoft.Json;
using NUnit.Framework;
using Umbraco.Cms.Infrastructure.PublishedCache;
using Umbraco.Cms.Tests.Common.Testing;
using Umbraco.UnitTesting.Adapter;

namespace UmbracoV9.Testing.Tests
{
    [TestFixture]
    [SingleThreaded]
    [NonParallelizable]
    public class Can_I_Haz_Content_Cache
    {
        private UmbracoSupport support;
        private BPlusTree<int, ContentNodeKit> allContentKits;

        public Can_I_Haz_Content_Cache()
        {
            TestOptionAttributeBase.ScanAssemblies.Add(typeof(Can_I_Haz_Content_Cache).Assembly);
        }

        [SetUp]
        public void Setup()
        {
            allContentKits = new ContentCacheSupport().GetFromCacheFile("./../../../../UmbracoV9.Testing.Web/umbraco/Data/TEMP/NuCache/NuCache.Content.db");

            support = new UmbracoSupport(allContentKits);
            support.Setup();
        }

        [Test]
        public void Write_All_Published_Content()
        {
            var ctx = support.GetUmbracoContext();

            Console.WriteLine(JsonConvert.SerializeObject(ctx.Content.GetAtRoot(), new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }

        [TearDown]
        public void TearDown()
        {
            support.TearDownUmbraco();
        }

        [Test]
        [Explicit]
        public void Write_All_Content_Kits()
        {
            var contentNodeKits = allContentKits;
            Console.WriteLine(JsonConvert.SerializeObject(contentNodeKits.Select(x => new { x.Key, x.Value.ContentTypeId, x.Value.PublishedData.Name }), Formatting.Indented));
        }
    }
}
