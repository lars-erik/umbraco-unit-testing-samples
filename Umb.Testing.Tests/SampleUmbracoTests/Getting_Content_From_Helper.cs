using NUnit.Framework;
using Umbraco.UnitTesting.Adapter.Support;

namespace Umb.Testing.Tests.SampleUmbracoTests
{
    [TestFixture]
    public class Getting_Content_From_Helper
    {
        readonly UmbracoSupport support = new UmbracoSupport();

        [SetUp]
        public void Setup()
        {
            support.SetupUmbraco();
        }

        [TearDown]
        public void TearDown()
        {
            support.DisposeUmbraco();
        }

        /* NOTE: To read properties from the content read from this XML
         * you'll have to stub up the content type and all its properties on the IContentTypeService.
         * Instead, you should just inject the IPublishedContentCache to your controller
         * and forget you ever met the UmbracoHelper. ;) */
        [Test]
        public void Goes_Through_ContentCache_InnerCache_Which_Is_Tightly_Bound_To_GetXmlContent()
        {
            support.ContentCacheXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE root [ 
<!ELEMENT content ANY>
<!ATTLIST content id ID #REQUIRED>

]>
<root id=""-1"">
  <content id=""1"" key=""17838480-c495-4da9-bc3b-098a71577a18"" parentID=""-1"" level=""1"" creatorID=""0"" sortOrder=""0"" createDate=""2016-04-03T18:21:14"" updateDate=""2016-04-03T18:21:14"" nodeName=""Testpage"" urlName=""testpage"" path=""-1,1"" isDoc="""" nodeType=""1051"" creatorName=""Admin"" writerName=""Admin"" writerID=""0"" template=""1050"" nodeTypeAlias=""content"" />
</root>
";

            var content = support.UmbracoHelper.TypedContent(1);

            Assert.That(content, Is.Not.Null);
        }
    }
}
