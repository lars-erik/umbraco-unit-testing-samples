using System.Linq;
using NUnit.Framework;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Tests.TestHelpers;

namespace Umb.Testing.Tests.SampleUmbracoTests
{
    /* NOTE: These tests currently fail.
     * They don't even execute.
     * Research might be done, also it might not.
     * Feel free to contribute a fix. :) */

    /* NOTE: This is the only test in the repo not using UmbracoSupport.
     * This is due to the fact that we do full DB integration here.
     * The UmbracoSupport class is intended for use with stubbed data.      */
    [TestFixture]
    [DatabaseTestBehavior(DatabaseBehavior.NewDbFileAndSchemaPerFixture)]
    public class Working_With_Database : BaseDatabaseFactoryTest
    {
        private IDataTypeService dataTypeService;

        [SetUp]
        public override void Initialize()
        {
            base.Initialize();

            dataTypeService = ApplicationContext.Services.DataTypeService;
            //ApplicationContext.DatabaseContext.Database.BeginTransaction();
        }

        [TearDown]
        public override void TearDown()
        {
            //ApplicationContext.DatabaseContext.Database.AbortTransaction();
            base.TearDown();
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

        [Test]
        public void Transaction_Works()
        {
            Assert.AreEqual(24, dataTypeService.GetAllDataTypeDefinitions().Count());
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
