using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Umbraco.UnitTesting.Adapter
{
    public class FakeContentTypeService : IContentTypeService
    {
        private List<IContentType> contentTypes = new List<IContentType>();

        IContentTypeComposition IContentTypeBaseService.Get(int id)
        {
            return Get(id);
        }

        public IContentType Get(Guid key)
        {
            return contentTypes.Find(x => x.Key == key);
        }

        public IContentType Get(string alias)
        {
            return contentTypes.Find(x => x.Alias == alias);
        }

        public int Count()
        {
            return contentTypes.Count;
        }

        public bool HasContentNodes(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IContentType> GetAll(params int[] ids)
        {
            return contentTypes.Where(x => ids.Length == 0 || ids.Contains(x.Id));
        }

        public IEnumerable<IContentType> GetAll(IEnumerable<Guid> ids)
        {
            return contentTypes.Where(x => !ids.Any() || ids.Contains(x.Key));
        }

        public IEnumerable<IContentType> GetDescendants(int id, bool andSelf)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IContentType> GetComposedOf(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IContentType> GetChildren(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IContentType> GetChildren(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool HasChildren(int id)
        {
            throw new NotImplementedException();
        }

        public bool HasChildren(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Save(IContentType item, int userId = -1)
        {
            contentTypes.Add(item);
        }

        public void Save(IEnumerable<IContentType> items, int userId = -1)
        {
            foreach (var item in items)
            {
                contentTypes.Add(item);
            }
        }

        public void Delete(IContentType item, int userId = -1)
        {
            contentTypes.Remove(item);
        }

        public void Delete(IEnumerable<IContentType> item, int userId = -1)
        {
            contentTypes.RemoveAll(item.Contains);
        }

        public Attempt<string[]> ValidateComposition(IContentType compo)
        {
            throw new NotImplementedException();
        }

        public bool HasContainerInPath(string contentPath)
        {
            throw new NotImplementedException();
        }

        public bool HasContainerInPath(params int[] ids)
        {
            throw new NotImplementedException();
        }

        public Attempt<OperationResult<OperationResultType, EntityContainer>> CreateContainer(int parentContainerId, string name, int userId = -1)
        {
            throw new NotImplementedException();
        }

        public Attempt<OperationResult> SaveContainer(EntityContainer container, int userId = -1)
        {
            throw new NotImplementedException();
        }

        public EntityContainer GetContainer(int containerId)
        {
            throw new NotImplementedException();
        }

        public EntityContainer GetContainer(Guid containerId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EntityContainer> GetContainers(int[] containerIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EntityContainer> GetContainers(IContentType contentType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EntityContainer> GetContainers(string folderName, int level)
        {
            throw new NotImplementedException();
        }

        public Attempt<OperationResult> DeleteContainer(int containerId, int userId = -1)
        {
            throw new NotImplementedException();
        }

        public Attempt<OperationResult<OperationResultType, EntityContainer>> RenameContainer(int id, string name, int userId = -1)
        {
            throw new NotImplementedException();
        }

        public Attempt<OperationResult<MoveOperationStatusType>> Move(IContentType moving, int containerId)
        {
            throw new NotImplementedException();
        }

        public Attempt<OperationResult<MoveOperationStatusType, IContentType>> Copy(IContentType copying, int containerId)
        {
            throw new NotImplementedException();
        }

        public IContentType Copy(IContentType original, string alias, string name, int parentId = -1)
        {
            throw new NotImplementedException();
        }

        public IContentType Copy(IContentType original, string alias, string name, IContentType parent)
        {
            throw new NotImplementedException();
        }

        public IContentType Get(int id)
        {
            return contentTypes.Find(x => x.Id == id);
        }

        public IEnumerable<string> GetAllPropertyTypeAliases()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllContentTypeAliases(params Guid[] objectTypes)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetAllContentTypeIds(string[] aliases)
        {
            throw new NotImplementedException();
        }
    }
}
