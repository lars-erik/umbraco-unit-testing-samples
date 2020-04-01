using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using CSharpTest.Net.Collections;
using Moq;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Xml;
using Umbraco.Tests.PublishedContent;
using Umbraco.Tests.TestHelpers;
using Umbraco.Tests.Testing;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.PublishedCache.NuCache;
using Current = Umbraco.Web.Composing.Current;

namespace Umbraco.UnitTesting.Adapter
{
    using CacheKeys = Umbraco.Web.PublishedCache.NuCache.CacheKeys;

    //[UmbracoTest(TypeLoader = UmbracoTestOptions.TypeLoader.PerFixture, WithApplication = true)]
    public class UmbracoSupport : PublishedContentSnapshotTestBase
    {
        private readonly BPlusTree<int, ContentNodeKit> localDb;
        private TestObjects.TestDataTypeService dataTypeService;
        private PublishedContentTypeFactory publishedContentTypeFactory;
        private IPublishedSnapshot snapshot;
        private DictionaryAppCache appCache;
        private IPublishedContentCache fakeContentCache;
        private IPublishedSnapshotAccessor snapshotAccessor;
        private ContentStore contentStore;
        private Action<Composition> compose;

        public UmbracoSupport(BPlusTree<int, ContentNodeKit> localDb = null)
        {
            this.localDb = localDb;
        }

        public IPublishedContentCache ContentCache => fakeContentCache;

        public static void RegisterForTesting<TFromAssembly>(TFromAssembly instance = null)
            where TFromAssembly : class
        {
            var type = typeof(TFromAssembly);
            var asm = type.Assembly;
            if (TestOptionAttributeBase.ScanAssemblies.All(x => x != asm))
            { 
                TestOptionAttributeBase.ScanAssemblies.Add(asm);
            }
        }

        public UmbracoContext GetUmbracoContext(string url)
        {
            return base.GetUmbracoContext(url, -1, setSingleton: true);
        }

        /// <summary>
        /// Initializes a stubbed Umbraco request context. Generally called from [SetUp] methods.
        /// Remember to call UmbracoSupport.DisposeUmbraco from your [TearDown].
        /// </summary>
        public void SetupUmbraco(Action<Composition> compose = null)
        {
            this.compose = compose;

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
            appCache.Items.AddOrUpdate(CacheKeys.ProfileName(-1), "User", (s, o) => "User");


        }

        protected override void Compose()
        {
            base.Compose();

            compose?.Invoke(Composition);
        }
        
        public void SetupSnapshot()
        {
            snapshotAccessor = Current.Factory.GetInstance<IPublishedSnapshotAccessor>();
            snapshot = Mock.Of<IPublishedSnapshot>();
            snapshotAccessor.PublishedSnapshot = snapshot;

            // TODO: This should really be a real content cache...
            fakeContentCache = new FakePublishedContentCache();

            contentStore = new ContentStore(
                snapshotAccessor,
                Current.Factory.GetInstance<IVariationContextAccessor>(),
                Current.Factory.GetInstance<ILogger>(),
                localDb
            );
            var storeSnapshot = contentStore.CreateSnapshot();
            //new ContentStore.Snapshot(contentStore, 1, Current.Logger);

            var anotherCache = new Umbraco.Web.PublishedCache.NuCache.ContentCache(
                false,
                storeSnapshot,
                appCache,
                null,
                Mock.Of<IDomainCache>(),
                Current.Configs.Global(),
                Current.VariationContextAccessor
            );

            Mock.Get(snapshot).Setup(x => x.SnapshotCache).Returns(appCache);
            Mock.Get(snapshot).Setup(x => x.Content).Returns(anotherCache);

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

            ((FakePublishedContentCache)fakeContentCache).Add(published);

            return published;
        }

        public override void PopulateCache(PublishedContentTypeFactory factory, SolidPublishedContentCache cache)
        {
        }
    }


    class FakePublishedContentCache : PublishedCacheBase, IPublishedContentCache, IPublishedMediaCache
    {
        private readonly Dictionary<int, IPublishedContent> _content = new Dictionary<int, IPublishedContent>();

        public FakePublishedContentCache()
            : base(false)
        { }

        public void Add(IPublishedContent content)
        {
            _content[content.Id] = PublishedContentExtensionsForModels.CreateModel(content);
        }

        public void Clear()
        {
            _content.Clear();
        }

        public IPublishedContent GetByRoute(bool preview, string route, bool? hideTopLevelNode = null, string culture = null)
        {
            throw new NotImplementedException();
        }

        public IPublishedContent GetByRoute(string route, bool? hideTopLevelNode = null, string culture = null)
        {
            throw new NotImplementedException();
        }

        public string GetRouteById(bool preview, int contentId, string culture = null)
        {
            throw new NotImplementedException();
        }

        public string GetRouteById(int contentId, string culture = null)
        {
            throw new NotImplementedException();
        }

        public override IPublishedContent GetById(bool preview, int contentId)
        {
            return _content.ContainsKey(contentId) ? _content[contentId] : null;
        }

        public override IPublishedContent GetById(bool preview, Guid contentId)
        {
            throw new NotImplementedException();
        }

        public override IPublishedContent GetById(bool preview, Udi nodeId)
            => throw new NotSupportedException();

        public override bool HasById(bool preview, int contentId)
        {
            return _content.ContainsKey(contentId);
        }

        public override IEnumerable<IPublishedContent> GetAtRoot(bool preview, string culture = null)
        {
            return _content.Values.Where(x => x.Parent == null);
        }

        public override IPublishedContent GetSingleByXPath(bool preview, string xpath, Core.Xml.XPathVariable[] vars)
        {
            throw new NotImplementedException();
        }

        public override IPublishedContent GetSingleByXPath(bool preview, System.Xml.XPath.XPathExpression xpath, Core.Xml.XPathVariable[] vars)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IPublishedContent> GetByXPath(bool preview, string xpath, Core.Xml.XPathVariable[] vars)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IPublishedContent> GetByXPath(bool preview, System.Xml.XPath.XPathExpression xpath, Core.Xml.XPathVariable[] vars)
        {
            throw new NotImplementedException();
        }

        public override System.Xml.XPath.XPathNavigator CreateNavigator(bool preview)
        {
            throw new NotImplementedException();
        }

        public override System.Xml.XPath.XPathNavigator CreateNodeNavigator(int id, bool preview)
        {
            throw new NotImplementedException();
        }

        public override bool HasContent(bool preview)
        {
            return _content.Count > 0;
        }

        public override IPublishedContentType GetContentType(int id)
        {
            throw new NotImplementedException();
        }

        public override IPublishedContentType GetContentType(string alias)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IPublishedContent> GetByContentType(IPublishedContentType contentType)
        {
            throw new NotImplementedException();
        }
    }
}
