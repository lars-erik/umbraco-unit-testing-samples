using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Umbraco.UnitTesting.Adapter
{
    public class FakeDataTypeService : IDataTypeService
    {
        private List<IDataType> dataTypes = new List<IDataType>();

        public IReadOnlyDictionary<Udi, IEnumerable<string>> GetReferences(int id)
        {
            throw new NotImplementedException();
        }

        public Attempt<OperationResult<OperationResultType, EntityContainer>> CreateContainer(int parentId, string name, int userId = -1)
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

        public IEnumerable<EntityContainer> GetContainers(string folderName, int level)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EntityContainer> GetContainers(IDataType dataType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EntityContainer> GetContainers(int[] containerIds)
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

        public IDataType GetDataType(string name)
        {
            return dataTypes.Find(dt => dt.Name == name);
        }

        public IDataType GetDataType(int id)
        {
            return dataTypes.Find(dt => dt.Id == id);
        }

        public IDataType GetDataType(Guid id)
        {
            return dataTypes.Find(dt => dt.Key == id);
        }

        public IEnumerable<IDataType> GetAll(params int[] ids)
        {
            return dataTypes.Where(dt => ids.Contains(dt.Id));
        }

        public void Save(IDataType dataType, int userId = -1)
        {
            dataTypes.Add(dataType);
        }

        public void Save(IEnumerable<IDataType> dataTypeDefinitions, int userId = -1)
        {
            foreach(var dataType in dataTypeDefinitions)
            { 
                dataTypes.Add(dataType);
            }
        }

        public void Save(IEnumerable<IDataType> dataTypeDefinitions, int userId, bool raiseEvents)
        {
            foreach (var dataType in dataTypeDefinitions)
            {
                dataTypes.Add(dataType);
            }
        }

        public void Delete(IDataType dataType, int userId = -1)
        {
            dataTypes.Remove(dataType);
        }

        public IEnumerable<IDataType> GetByEditorAlias(string propertyEditorAlias)
        {
            throw new NotImplementedException();
        }

        public Attempt<OperationResult<MoveOperationStatusType>> Move(IDataType toMove, int parentId)
        {
            throw new NotImplementedException();
        }
    }
}