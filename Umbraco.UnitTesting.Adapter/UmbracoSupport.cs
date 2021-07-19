using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Xml.XPath;
using CSharpTest.Net.Collections;
using Moq;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Xml;
using Umbraco.Tests.PublishedContent;
using Umbraco.Tests.TestHelpers;
using Umbraco.Tests.Testing;
using Umbraco.Tests.Testing.Objects.Accessors;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.PublishedCache.NuCache;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;
using Current = Umbraco.Web.Composing.Current;

namespace Umbraco.UnitTesting.Adapter
{
    using CacheKeys = Umbraco.Web.PublishedCache.NuCache.CacheKeys;

    public class UmbracoSupport : BaseWebTest
    {
        private readonly BPlusTree<int, ContentNodeKit> localDb;
        private TestObjects.TestDataTypeService dataTypeService;
        private PublishedContentTypeFactory publishedContentTypeFactory;
        private IPublishedSnapshot snapshot;
        private DictionaryAppCache appCache;
        private IPublishedSnapshotAccessor snapshotAccessor;
        private ContentStore contentStore;
        private Action<Composition> compose;
        private TestVariationContextAccessor testVariationContextAccessor;
        private IPublishedSnapshotService snapshotService;
        private List<IPublishedContentType> contentTypes = new List<IPublishedContentType>();

        public UmbracoSupport(BPlusTree<int, ContentNodeKit> localDb = null)
        {
            this.localDb = localDb;

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
        }

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

        public new UmbracoContext GetUmbracoContext(string url, int templateId = 1234, RouteData routeData = null, bool setSingleton = false, IUmbracoSettingsSection umbracoSettings = null, IEnumerable<IUrlProvider> urlProviders = null, IEnumerable<IMediaUrlProvider> mediaUrlProviders = null, IGlobalSettings globalSettings = null, IPublishedSnapshotService snapshotService = null)
        {
            var service = snapshotService ?? this.snapshotService;
            var httpContext = GetHttpContextFactory(url, routeData).HttpContext;

            var umbracoContext = new UmbracoContext(
                httpContext,
                service,
                new WebSecurity(httpContext, Factory.GetInstance<IUserService>(),
                    Factory.GetInstance<IGlobalSettings>()),
                umbracoSettings ?? Factory.GetInstance<IUmbracoSettingsSection>(),
                urlProviders ?? Enumerable.Empty<IUrlProvider>(),
                mediaUrlProviders ?? Enumerable.Empty<IMediaUrlProvider>(),
                globalSettings ?? Factory.GetInstance<IGlobalSettings>(),
                new TestVariationContextAccessor());

            if (setSingleton)
                Umbraco.Web.Composing.Current.UmbracoContextAccessor.UmbracoContext = umbracoContext;

            return umbracoContext;
        }

        /// <summary>
        /// Initializes a stubbed Umbraco request context. Generally called from [SetUp] methods.
        /// Remember to call UmbracoSupport.DisposeUmbraco from your [TearDown].
        /// </summary>
        public void SetupUmbraco(Action<Composition> compose = null)
        {
            this.compose = compose;

            appCache = new DictionaryAppCache();
            appCache.Items.AddOrUpdate(CacheKeys.ProfileName(-1), "User", (s, o) => "User");

            //RegisterForTesting(this);
            base.SetUp();
        }

        protected override void Compose()
        {
            base.Compose();

            compose?.Invoke(Composition);
        }

        protected override IPublishedSnapshotService CreatePublishedSnapshotService()
        {
            // Base sets private ContentTypesCache
            base.CreatePublishedSnapshotService();

            snapshotService = Mock.Of<IPublishedSnapshotService>();
            snapshotAccessor = Mock.Of<IPublishedSnapshotAccessor>();
            snapshot = Mock.Of<IPublishedSnapshot>();
            snapshotAccessor.PublishedSnapshot = snapshot;

            testVariationContextAccessor = new TestVariationContextAccessor();

            contentStore = new ContentStore(
                snapshotAccessor,
                testVariationContextAccessor,
                Current.Factory.GetInstance<ILogger>(),
                localDb
            );

            using (contentStore.GetScopedWriteLock(Current.Factory.GetInstance<IScopeProvider>()))
            {
                contentStore.SetAllContentTypesLocked(contentTypes);
                contentStore.SetAllLocked(localDb.Values);
            }

            var storeSnapshot = contentStore.CreateSnapshot();
            //new ContentStore.Snapshot(contentStore, 1, Current.Logger);

            var anotherCache = new ContentCache(
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

            Mock.Get(snapshotService).Setup(x => x.CreatePublishedSnapshot(It.IsAny<string>())).Returns(() =>
            {
                return snapshot;
            });

            return snapshotService;
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

        public void CreateContentType(int id, string alias,
            Func<IPublishedContentType, IEnumerable<IPublishedPropertyType>> propertyFactory)
        {
            var publishedContentType = publishedContentTypeFactory.CreateContentType(Guid.NewGuid(), id, alias, propertyFactory);
            contentTypes.Add(publishedContentType);
        }
    }
}
