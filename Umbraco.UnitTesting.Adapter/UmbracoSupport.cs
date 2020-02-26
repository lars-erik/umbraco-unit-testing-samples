using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Tests.PublishedContent;
using Umbraco.Tests.TestHelpers;
using Umbraco.Tests.Testing;
using Umbraco.Web.Composing;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.PublishedCache.NuCache;

namespace Umbraco.UnitTesting.Adapter
{
    using CacheKeys = Umbraco.Web.PublishedCache.NuCache.CacheKeys;

    //[UmbracoTest(TypeLoader = UmbracoTestOptions.TypeLoader.PerFixture, WithApplication = true)]
    public class UmbracoSupport : PublishedContentSnapshotTestBase
    {
        private TestObjects.TestDataTypeService dataTypeService;
        private PublishedContentTypeFactory publishedContentTypeFactory;
        private IPublishedSnapshot snapshot;
        private DictionaryAppCache appCache;
        private SolidPublishedContentCache fakeContentCache;
        private IPublishedSnapshotAccessor snapshotAccessor;

        public static void RegisterForTesting<TFromAssembly>(TFromAssembly instance = null)
            where TFromAssembly : class
        {
            var asm = typeof(TFromAssembly).Assembly;
            if (TestOptionAttributeBase.ScanAssemblies.All(x => x != asm))
            { 
                TestOptionAttributeBase.ScanAssemblies.Add(asm);
            }
        }

        /// <summary>
        /// Initializes a stubbed Umbraco request context. Generally called from [SetUp] methods.
        /// Remember to call UmbracoSupport.DisposeUmbraco from your [TearDown].
        /// </summary>
        public void SetupUmbraco()
        {
            //RegisterForTesting(this);
            base.SetUp();

            dataTypeService = new TestObjects.TestDataTypeService(
                new DataType(new VoidEditor(Mock.Of<ILogger>())) { Id = 1 }
                );
            
            publishedContentTypeFactory = new PublishedContentTypeFactory(
                Mock.Of<IPublishedModelFactory>(), 
                new PropertyValueConverterCollection(
                    Array.Empty<IPropertyValueConverter>()
                ), 
                dataTypeService
            );

            appCache = new DictionaryAppCache();

            // TODO: This should really be a real content cache...
            fakeContentCache = new SolidPublishedContentCache();

            snapshot = Mock.Of<IPublishedSnapshot>();
            Mock.Get(snapshot).Setup(x => x.SnapshotCache).Returns(appCache);
            Mock.Get(snapshot).Setup(x => x.Content).Returns(fakeContentCache);

            snapshotAccessor = Current.Factory.GetInstance<IPublishedSnapshotAccessor>();
            snapshotAccessor.PublishedSnapshot = snapshot;

            appCache.Items.AddOrUpdate(CacheKeys.ProfileName(-1), "User", (s, o) => "User");
        }

        /// <summary>
        /// Cleans up the stubbed Umbraco request context. Generally called from [TearDown] methods.
        /// Must be called before another UmbracoSupport.SetupUmbraco.
        /// </summary>
        public void DisposeUmbraco()
        {
            base.TearDown();
        }

        public IPublishedPropertyType CreatePropertyType(string alias, int type)
        {
            return publishedContentTypeFactory.CreatePropertyType(alias, type);
        }

        public IPublishedContentType CreateContentType(int id, string alias,
            Func<IPublishedContentType, IEnumerable<IPublishedPropertyType>> propertyFactory)
        {
            return publishedContentTypeFactory.CreateContentType(id, alias, propertyFactory);
        }

        public IPublishedContent CreatePublishedContent(ContentNodeKit contentNode, IPublishedContentType contentType)
        {
            contentNode.Node.ContentType = contentType;

            var published = new PublishedContent(
                contentNode.Node,
                contentNode.PublishedData,
                snapshotAccessor,
                Current.Factory.GetInstance<IVariationContextAccessor>()
            );

            return published;
        }

        public override void PopulateCache(PublishedContentTypeFactory factory, SolidPublishedContentCache cache)
        {
        }
    }
}
