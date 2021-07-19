using CSharpTest.Net.Collections;
using Umbraco.Web.PublishedCache.NuCache;
using Umbraco.Web.PublishedCache.NuCache.DataSource;

namespace Umbraco.UnitTesting.Adapter
{
    public class ContentCacheSupport
    {
        public BPlusTree<int, ContentNodeKit> GetFromCacheFile(string path)
        {
            var allContent = BTree.GetTree(path, true);
            return allContent;
        }
    }
}
