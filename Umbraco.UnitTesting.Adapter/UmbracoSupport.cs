using System;
using System.Linq;
using System.Threading.Tasks;
using CSharpTest.Net.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.PublishedCache;
using Umbraco.Cms.Infrastructure.Serialization;
using Umbraco.Cms.Tests.Common;
using Umbraco.Cms.Tests.Common.Builders;
using Umbraco.Cms.Tests.Common.Builders.Extensions;
using Umbraco.Cms.Tests.Common.Testing;
using Umbraco.Cms.Tests.Integration.Testing;
using Umbraco.Cms.Web.Common.UmbracoContext;
using Umbraco.Extensions;

namespace Umbraco.UnitTesting.Adapter
{
    [UmbracoTest(Database = UmbracoTestOptions.Database.None)]
    public class UmbracoSupport : UmbracoIntegrationTest
    {
        private readonly BPlusTree<int, ContentNodeKit> localDb;
        private IPublishedSnapshot publishedSnapshot;
        private IPublishedSnapshotAccessor publishedSnapshotAccessor;
        private FakeDataTypeService dataTypeService;
        private FakeContentTypeService contentTypeService;
        private ContentStore contentStore;
        private TestVariationContextAccessor variationContextAccessor;

        public UmbracoSupport(BPlusTree<int, ContentNodeKit> localDb = null)
        {
            this.localDb = localDb;
        }

        public override void Setup()
        {
            base.Setup();
            SetupRequest();
        }

        public T GetService<T>()
        {
            return GetRequiredService<T>();
        }

        public void SetupContentCache()
        {
            var publishedModelFactory = GetRequiredService<IPublishedModelFactory>();
            contentStore = new ContentStore(
                publishedSnapshotAccessor,
                variationContextAccessor,
                Mock.Of<ILogger>(),
                Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole()),
                publishedModelFactory, // TODO: Use the real model factory?
                localDb
            );

            var publishedContentTypeFactory = GetRequiredService<IPublishedContentTypeFactory>();
            using (contentStore.GetScopedWriteLock(GetRequiredService<IScopeProvider>()))
            {
                //contentStore.UpdateDataTypesLocked(dataTypeService.GetAll()));
                contentStore.UpdateContentTypesLocked(contentTypeService.GetAll()
                    .Select(x => publishedContentTypeFactory.CreateContentType(x)));
                contentStore.SetAllLocked(localDb.Values);
            }

            var storeSnapshot = contentStore.CreateSnapshot();

            var cache = new ContentCache(
                false,
                storeSnapshot,
                NoAppCache.Instance,
                NoAppCache.Instance,
                Mock.Of<IDomainCache>(),
                new OptionsWrapper<GlobalSettings>(GlobalSettings),
                variationContextAccessor
            );

            Mock.Get(publishedSnapshot).Setup(x => x.Content).Returns(cache);
        }

        // TODO: Customizable URL
        private void SetupRequest()
        {
            var httpContext = GetRequiredService<IHttpContextAccessor>().HttpContext;
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("localhost", 443);
            httpContext.Request.PathBase = new PathString();
            httpContext.Request.Path = new PathString("/");
            httpContext.Request.QueryString = QueryString.Empty;
            var url = httpContext.Request.GetEncodedUrl();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.RemoveAll(x => x.ServiceType == typeof(IDataTypeService));
            dataTypeService = new FakeDataTypeService();
            services.AddSingleton<IDataTypeService>(dataTypeService);

            services.RemoveAll(x => x.ServiceType == typeof(IContentTypeService));
            contentTypeService = new FakeContentTypeService();
            services.AddSingleton<IContentTypeService>(contentTypeService);

            publishedSnapshot = Mock.Of<IPublishedSnapshot>();
            Mock.Get(publishedSnapshot).Setup(x => x.SnapshotCache).Returns(NoAppCache.Instance);
            services.AddSingleton(publishedSnapshot);

            publishedSnapshotAccessor = Mock.Of<IPublishedSnapshotAccessor>();
            publishedSnapshotAccessor.PublishedSnapshot = publishedSnapshot;
            services.AddSingleton(publishedSnapshotAccessor);

            variationContextAccessor = new TestVariationContextAccessor();
            variationContextAccessor.VariationContext = new VariationContext();
            services.AddSingleton<IVariationContextAccessor>(variationContextAccessor);

            var snapshotService = new FakePublishedSnapshotService(publishedSnapshot);
            services.AddSingleton<IPublishedSnapshotService>(snapshotService);
        }

        public IUmbracoContext GetUmbracoContext()
        {
            var ctxFact = GetRequiredService<IUmbracoContextFactory>();
            var ctxRef = ctxFact.EnsureUmbracoContext();
            var ctx = ctxRef.UmbracoContext;
            return ctx;
        }


        public void TearDownUmbraco()
        {
            base.TearDown_Logging();
            Task.Run(async () => await base.TearDownAsync()).Wait();
        }

        #region Old code
        //private readonly BPlusTree<int, ContentNodeKit> localDb;
        //private TestObjects.TestDataTypeService dataTypeService;
        //private PublishedContentTypeFactory publishedContentTypeFactory;
        //private IPublishedSnapshot snapshot;
        //private DictionaryAppCache appCache;
        //private IPublishedSnapshotAccessor snapshotAccessor;
        //private ContentStore contentStore;
        //private Action<Composition> compose;
        //private TestVariationContextAccessor testVariationContextAccessor;
        //private IPublishedSnapshotService snapshotService;
        //private List<IPublishedContentType> contentTypes = new List<IPublishedContentType>();

        //public UmbracoSupport(BPlusTree<int, ContentNodeKit> localDb = null)
        //{
        //    this.localDb = localDb;

        //    dataTypeService = new TestObjects.TestDataTypeService(
        //        new DataType(new VoidEditor("", new DataValueEditorFactory())) { Id = 1 }
        //    );

        //    publishedContentTypeFactory = new PublishedContentTypeFactory(
        //        Mock.Of<IPublishedModelFactory>(),
        //        new PropertyValueConverterCollection(
        //            Array.Empty<IPropertyValueConverter>()
        //        ),
        //        dataTypeService
        //    );
        //}

        //public static void RegisterForTesting<TFromAssembly>(TFromAssembly instance = null)
        //    where TFromAssembly : class
        //{
        //    var type = typeof(TFromAssembly);
        //    var asm = type.Assembly;
        //    if (TestOptionAttributeBase.ScanAssemblies.All(x => x != asm))
        //    {
        //        TestOptionAttributeBase.ScanAssemblies.Add(asm);
        //    }
        //}

        //public new UmbracoContext GetUmbracoContext(string url, int templateId = 1234, RouteData routeData = null, bool setSingleton = false, IUmbracoSettingsSection umbracoSettings = null, IEnumerable<IUrlProvider> urlProviders = null, IEnumerable<IMediaUrlProvider> mediaUrlProviders = null, IGlobalSettings globalSettings = null, IPublishedSnapshotService snapshotService = null)
        //{
        //    var service = snapshotService ?? this.snapshotService;
        //    var httpContext = GetHttpContextFactory(url, routeData).HttpContext;

        //    var umbracoContext = new UmbracoContext(
        //        httpContext,
        //        service,
        //        new WebSecurity(httpContext, Factory.GetInstance<IUserService>(),
        //            Factory.GetInstance<IGlobalSettings>()),
        //        umbracoSettings ?? Factory.GetInstance<IUmbracoSettingsSection>(),
        //        urlProviders ?? Enumerable.Empty<IUrlProvider>(),
        //        mediaUrlProviders ?? Enumerable.Empty<IMediaUrlProvider>(),
        //        globalSettings ?? Factory.GetInstance<IGlobalSettings>(),
        //        new TestVariationContextAccessor());

        //    if (setSingleton)
        //        Umbraco.Web.Composing.Current.UmbracoContextAccessor.UmbracoContext = umbracoContext;

        //    return umbracoContext;
        //}

        ///// <summary>
        ///// Initializes a stubbed Umbraco request context. Generally called from [SetUp] methods.
        ///// Remember to call UmbracoSupport.DisposeUmbraco from your [TearDown].
        ///// </summary>
        //public void SetupUmbraco(Action<Composition> compose = null)
        //{
        //    this.compose = compose;

        //    appCache = new DictionaryAppCache();
        //    appCache.Items.AddOrUpdate(CacheKeys.ProfileName(-1), "User", (s, o) => "User");

        //    //RegisterForTesting(this);
        //    base.SetUp();
        //}

        //protected override void Compose()
        //{
        //    base.Compose();

        //    compose?.Invoke(Composition);
        //}

        //protected override IPublishedSnapshotService CreatePublishedSnapshotService()
        //{
        //    // Base sets private ContentTypesCache
        //    base.CreatePublishedSnapshotService();

        //    snapshotService = Mock.Of<IPublishedSnapshotService>();
        //    snapshotAccessor = Mock.Of<IPublishedSnapshotAccessor>();
        //    snapshot = Mock.Of<IPublishedSnapshot>();
        //    snapshotAccessor.PublishedSnapshot = snapshot;

        //    testVariationContextAccessor = new TestVariationContextAccessor();

        //    contentStore = new ContentStore(
        //        snapshotAccessor,
        //        testVariationContextAccessor,
        //        Current.Factory.GetInstance<ILogger>(),
        //        localDb
        //    );

        //    using (contentStore.GetScopedWriteLock(Current.Factory.GetInstance<IScopeProvider>()))
        //    {
        //        contentStore.SetAllContentTypesLocked(contentTypes);
        //        contentStore.SetAllLocked(localDb.Values);
        //    }

        //    var storeSnapshot = contentStore.CreateSnapshot();
        //    //new ContentStore.Snapshot(contentStore, 1, Current.Logger);

        //    var anotherCache = new ContentCache(
        //        false,
        //        storeSnapshot,
        //        appCache,
        //        null,
        //        Mock.Of<IDomainCache>(),
        //        Current.Configs.Global(),
        //        Current.VariationContextAccessor
        //    );

        //    Mock.Get(snapshot).Setup(x => x.SnapshotCache).Returns(appCache);
        //    Mock.Get(snapshot).Setup(x => x.Content).Returns(anotherCache);

        //    Mock.Get(snapshotService).Setup(x => x.CreatePublishedSnapshot(It.IsAny<string>())).Returns(() =>
        //    {
        //        return snapshot;
        //    });

        //    return snapshotService;
        //}

        ///// <summary>
        ///// Cleans up the stubbed Umbraco request context. Generally called from [TearDown] methods.
        ///// Must be called before another UmbracoSupport.SetupUmbraco.
        ///// </summary>
        //public void DisposeUmbraco()
        //{
        //    base.TearDown();
        //}

        //public IPublishedPropertyType CreatePropertyType(string alias, int type)
        //{
        //    return publishedContentTypeFactory.CreatePropertyType(alias, type);
        //}

        //public void CreateContentType(int id, string alias,
        //    Func<IPublishedContentType, IEnumerable<IPublishedPropertyType>> propertyFactory)
        //{
        //    contentTypes.Add(publishedContentType);
        //    var publishedContentType = publishedContentTypeFactory.CreateContentType(Guid.NewGuid(), id, alias, propertyFactory);
        //}

        #endregion
    }
}
