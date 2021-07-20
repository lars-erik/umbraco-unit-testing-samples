using System;
using System.Linq;
using CSharpTest.Net.Collections;
using Newtonsoft.Json;
using NUnit.Framework;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.PublishedCache;
using Umbraco.Cms.Tests.Common.Builders;
using Umbraco.Cms.Tests.Common.Builders.Extensions;
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

            SetupContentTypes();

            support.SetupContentCache();

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

        private void SetupContentTypes()
        {
            var dataType = new DataTypeBuilder()
                .WithId(1)
                .WithDatabaseType(ValueStorageType.Ntext)
                .WithName("Title")
                .Build();
            support.GetService<IDataTypeService>().Save(dataType);

            var contentTypeBuilder = new ContentTypeBuilder();
            var homeType = contentTypeBuilder
                .WithAlias("home")
                .WithId(1055)
                .Build();
            var pageType = contentTypeBuilder
                .WithAlias("page")
                .WithId(1056)
                .Build();
            support.GetService<IContentTypeService>().Save(homeType);
            support.GetService<IContentTypeService>().Save(pageType);
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
