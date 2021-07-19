using System;
using CSharpTest.Net.Collections;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Infrastructure.PublishedCache;
using Umbraco.Cms.Infrastructure.PublishedCache.DataSource;

namespace Umbraco.UnitTesting.Adapter
{
    public class ContentCacheSupport
    {
        public BPlusTree<int, ContentNodeKit> GetFromCacheFile(string path)
        {
            var allContent = BTree.GetTree(path, true, new NuCacheSettings());
            return allContent;
        }
    }
}
