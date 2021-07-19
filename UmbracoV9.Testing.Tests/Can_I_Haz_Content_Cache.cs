using System;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Umbraco.UnitTesting.Adapter;

namespace UmbracoV9.Testing.Tests
{
    [TestFixture]
    public class Can_I_Haz_Content_Cache
    {

        [Test]
        [Explicit]
        public void Write_All_Content_Kits()
        {
            var allContentKits = new ContentCacheSupport().GetFromCacheFile("./../../../../UmbracoV9.Testing.Web/umbraco/Data/TEMP/NuCache/NuCache.Content.db");
            Console.WriteLine(JsonConvert.SerializeObject(allContentKits.Select(x => new { x.Key, x.Value.ContentTypeId, x.Value.PublishedData.Name }), Formatting.Indented));
        }
    }
}
