using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Tests.TestHelpers;
using Umbraco.Web;

namespace Umb.Testing.Tests.SampleUmbracoTests
{
    [TestFixture]
    [DatabaseTestBehavior(DatabaseBehavior.NewDbFileAndSchemaPerTest)]
    public class Working_With_Database : BaseDatabaseFactoryTest
    {
        private IDataTypeService dataTypeService;

        [SetUp]
        public override void Initialize()
        {
            base.Initialize();

            dataTypeService = ApplicationContext.Services.DataTypeService;
        }

        [Test]
        public void Persisting_DataType()
        {
            var dataType = CreateAndSaveDataType();
            var id = dataType.Id;
            var persisted = dataTypeService.GetDataTypeDefinitionById(id);
            Assert.AreNotSame(dataType, persisted);
            Assert.AreEqual(dataType.Name, persisted.Name);
        }

        [Test]
        public void Updating_DataType()
        {
            const string expectedName = "An edited fancy datatype";
            var id = CreateAndSaveDataType().Id;

            var toUpdate = dataTypeService.GetDataTypeDefinitionById(id);
            toUpdate.Name = expectedName;
            dataTypeService.Save(toUpdate);

            var updated = dataTypeService.GetDataTypeDefinitionById(id);
            Assert.AreEqual(expectedName, updated.Name);
        }

        [Test]
        public void Deleting_DataType()
        {
            var dataType = CreateAndSaveDataType();
            dataTypeService.Delete(dataType);
            var lost = dataTypeService.GetDataTypeDefinitionById(dataType.Id);
            Assert.IsNull(lost);
        }

        private DataTypeDefinition CreateAndSaveDataType()
        {
            var dataType = new DataTypeDefinition(-1, "a-definition");
            dataType.Name = "Fancy datatype";
            dataTypeService.Save(dataType);
            return dataType;
        }
    }
}
