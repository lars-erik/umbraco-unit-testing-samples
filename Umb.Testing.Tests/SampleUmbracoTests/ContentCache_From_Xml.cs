using System.Web;
using NUnit.Framework;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors.ValueConverters;
using Umbraco.UnitTesting.Adapter.Support;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedCache;

namespace Umb.Testing.Tests.SampleUmbracoTests
{
    [TestFixture]
    [Description(@"
    The BaseDatabaseFactoryTest class from Umbraco.Tests has a GetXmlContent method.
    It's used to stub out the content cache we otherwise have in App_Data/umbraco.config.
    It's also what's used by the ContentCache on the UmbracoContext.
    We can exploit that to quickly set up content relevant for our tests.
    ")]
    public class ContentCache_From_Xml
    {
        // Note: Setup and privates are below the tests. ;)

        [Test]
        [Description(@"
        An actual XmlPublishedContent is returned with all the built-in primitive data.
        However, properties uses content type information and the content type does not exist.
        This super-simple setup is ideal for testing menus, sitemaps and other functionality 
        using the content hierarchy. Other good matches are content finders, url providers etc.
        ")]
        public void Returns_Empty_Documents()
        {
            var page1 = contentCache.GetById(Page1Id);

            Assert.That(page1, Is
                .Not.Null
                .And
                .InstanceOf<PublishedContentWithKeyBase>()
                .And
                .Property("Name").EqualTo("Page 1")
                .And
                .Matches<IPublishedContent>(c => c["title"] == null)
                .And
                .Property("Parent")
                    .Property("Children")
                        .With.Count.EqualTo(2)
            );
        }

        [Test]
        [Description(@"
        The UmbracoSupport class contains a helper method to stub up content types.
        By doing this, the XmlPublishedContent can retrieve property values from the stubbed XML.
        ")]
        public void With_DocumentTypes_Setup_Returns_Full_Blown_Documents()
        {
            SetupContentType();

            var page1 = contentCache.GetById(Page1Id);

            Assert.Multiple(() =>
            {
                Assert.That(page1["title"], Is.EqualTo("Welcome!"));
                Assert.That(page1["excerpt"], Is.EqualTo("Lorem ipsum dolor..."));
                Assert.That(page1["body"].ToString(), Is.EqualTo("<p>Lorem ipsum dolor...</p>"));
                Assert.That(page1["image"], Is.EqualTo(123));
            });
        }

        [Test]
        [Description(@"
        The UmbracoSupport class contains a fake instance for the model factory.
        You can register types by calling register with the alias and a lambda with the constructor.
        Property value converter types need to be registered in setup before SetupUmbraco.
        ")]
        public void With_DocumentTypes_And_Models_Setup_Returns_Fully_Functional_Typed_Content()
        {
            SetupContentType();

            umbracoSupport.ModelFactory.Register("page", c => new Page(c));

            var page1 = contentCache.GetById(Page1Id);

            Assert.That(page1, Is
                .InstanceOf<Page>()
                .And.Property("Body")
                    .Matches<IHtmlString>(s => 
                        s.ToString() == "<p>Lorem ipsum dolor...</p>"
                    )
            );
        }

        private void SetupContentType()
        {
            umbracoSupport.SetupContentType("page", new[]
            {
                new PropertyType("textstring", DataTypeDatabaseType.Nvarchar, "title"),
                new PropertyType("textarea", DataTypeDatabaseType.Nvarchar, "excerpt"),
                new PropertyType("Umbraco.TinyMCEv3", DataTypeDatabaseType.Nvarchar, "body"),
                new PropertyType("media", DataTypeDatabaseType.Integer, "image")
            });
        }

        [SetUp]
        public void Setup()
        {
            umbracoSupport = new UmbracoSupport();

            // Converter types need to be added before setup
            umbracoSupport.ConverterTypes.Add(typeof(TinyMceValueConverter));

            umbracoSupport.SetupUmbraco();

            contentCache = umbracoSupport.UmbracoContext.ContentCache;

            // This XML is what the ContentCache will represent
            umbracoSupport.ContentCacheXml = @"
                <?xml version=""1.0"" encoding=""utf-8""?>
                <!DOCTYPE root [<!ELEMENT contentBase ANY>
                <!ELEMENT home ANY>
                <!ATTLIST home id ID #REQUIRED>
                <!ELEMENT page ANY>
                <!ATTLIST page id ID #REQUIRED>
                ]>
                <root id=""-1"">
                  <home id=""1103"" key=""156f1933-e327-4dce-b665-110d62720d03"" parentID=""-1"" level=""1"" creatorID=""0"" sortOrder=""0"" createDate=""2017-10-17T20:25:12"" updateDate=""2017-10-17T20:25:17"" nodeName=""Home"" urlName=""home"" path=""-1,1103"" isDoc="""" nodeType=""1093"" creatorName=""Admin"" writerName=""Admin"" writerID=""0"" template=""1064"" nodeTypeAlias=""home"">
                    <title>Welcome!</title>
                    <excerptCount>4</excerptCount>
                    <page id=""1122"" key=""1cb33e0a-400a-4938-9547-b05a35739b8b"" parentID=""1103"" level=""2"" creatorID=""0"" sortOrder=""0"" createDate=""2017-10-17T20:25:12"" updateDate=""2017-10-17T20:25:17"" nodeName=""Page 1"" urlName=""page1"" path=""-1,1103,1122"" isDoc="""" nodeType=""1095"" creatorName=""Admin"" writerName=""Admin"" writerID=""0"" template=""1060"" nodeTypeAlias=""page"">
                      <title>Welcome!</title>
                      <excerpt><![CDATA[Lorem ipsum dolor...]]></excerpt>
                      <body><![CDATA[<p>Lorem ipsum dolor...</p>]]></body>
                      <image>123</image>
                    </page>
                    <page id=""1123"" key=""242928f6-a1cf-4cd3-ac34-f3ddf3526b2e"" parentID=""1103"" level=""2"" creatorID=""0"" sortOrder=""1"" createDate=""2017-10-17T20:25:12"" updateDate=""2017-10-17T20:25:17"" nodeName=""Page 2"" urlName=""page2"" path=""-1,1103,1123"" isDoc="""" nodeType=""1095"" creatorName=""Admin"" writerName=""Admin"" writerID=""0"" template=""1060"" nodeTypeAlias=""page"">
                      <title>More welcome!</title>
                      <excerpt><![CDATA[More lorem ipsum dolor...]]></excerpt>
                      <body><![CDATA[Even more lorem ipsum dolor...]]></body>
                      <image>234</image>
                    </page>
                  </home>
                </root>
            ".Trim();
        }

        [TearDown]
        public void TearDown()
        {
            umbracoSupport.DisposeUmbraco();
        }

        private const int Page1Id = 1122;

        private UmbracoSupport umbracoSupport;
        private ContextualPublishedContentCache contentCache;

        public class Page : PublishedContentModel
        {
            public Page(IPublishedContent content) : base((IPublishedContentWithKey)content)
            {
            }

            public string Title => Content.GetPropertyValue<string>("title");
            public string Excerpt => Content.GetPropertyValue<string>("excerpt");
            public IHtmlString Body => Content.GetPropertyValue<IHtmlString>("body");
            public int Image => Content.GetPropertyValue<int>("image");
        }
    }
}
