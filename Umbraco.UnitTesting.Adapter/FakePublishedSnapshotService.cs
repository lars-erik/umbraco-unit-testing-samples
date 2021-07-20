using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.PublishedCache;

namespace Umbraco.UnitTesting.Adapter
{
    public class FakePublishedSnapshotService : IPublishedSnapshotService
    {
        private readonly IPublishedSnapshot snapshot;

        public FakePublishedSnapshotService(IPublishedSnapshot snapshot)
        {
            this.snapshot = snapshot;
        }
        
        public void Dispose()
        {
        }

        public IPublishedSnapshot CreatePublishedSnapshot(string previewToken)
        {
            return snapshot;
        }

        public void Rebuild(IReadOnlyCollection<int> contentTypeIds = null, IReadOnlyCollection<int> mediaTypeIds = null,
            IReadOnlyCollection<int> memberTypeIds = null)
        {
        }

        public void Notify(ContentCacheRefresher.JsonPayload[] payloads, out bool draftChanged, out bool publishedChanged)
        {
            draftChanged = false;
            publishedChanged = false;
        }

        public void Notify(MediaCacheRefresher.JsonPayload[] payloads, out bool anythingChanged)
        {
            anythingChanged = false;
        }

        public void Notify(ContentTypeCacheRefresher.JsonPayload[] payloads)
        {
        }

        public void Notify(DataTypeCacheRefresher.JsonPayload[] payloads)
        {
        }

        public void Notify(DomainCacheRefresher.JsonPayload[] payloads)
        {
        }

        public Task CollectAsync()
        {
            return Task.CompletedTask;
        }
    }
}
