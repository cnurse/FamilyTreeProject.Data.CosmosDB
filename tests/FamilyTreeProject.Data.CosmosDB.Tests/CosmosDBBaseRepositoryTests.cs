using System;
using System.Collections.Generic;
using FamilyTreeProject.Core.Common;
using FamilyTreeProject.Data.Common.Mapping;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Moq;
using NUnit.Framework;
// ReSharper disable UnusedVariable
// ReSharper disable ExpressionIsAlwaysNull

namespace FamilyTreeProject.Data.CosmosDB.Tests
{
    public abstract class CosmosDBBaseRepositoryTests<TModel> : CosmosDBTestBase where TModel : Entity, new()
    {
        [Test]
        public void Constructor_Throws_On_Null_DocumentClient()
        {
            //Arrange
            DocumentClient client = null;

            //Act

            //Assert
            // ReSharper disable once ExpressionIsAlwaysNull
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CosmosDBRepository<TModel>(client, It.IsAny<string>(), It.IsAny<string>()));
        }
        
        [Test]
        public void Constructor_Throws_On_Empty_DatabaseId()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();

            //Act

            //Assert
            // ReSharper disable once ExpressionIsAlwaysNull
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new CosmosDBRepository<TModel>(mockClient.Object, String.Empty, It.IsAny<string>()));
        }

        [Test]
        public void Constructor_Throws_On_Empty_CollectionId()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();

            //Act

            //Assert
            // ReSharper disable once ExpressionIsAlwaysNull
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new CosmosDBRepository<TModel>(mockClient.Object, It.IsAny<string>(), String.Empty));
        }
        
        [Test]
        public void Constructor_Checks_If_Database_Exists()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();

            //Act
            var repository = new CosmosDBRepository<TModel>(mockClient.Object, CosmosDB_DatabaseId, CosmosDB_CollectionId);

            //Assert
            mockClient.Verify(c => c.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(CosmosDB_DatabaseId), null), Times.Once());
        }

        [Test]
        public void Constructor_Checks_If_Collection_Exists()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();

            //Act
            var repository = new CosmosDBRepository<TModel>(mockClient.Object, CosmosDB_DatabaseId, CosmosDB_CollectionId);

            //Assert
            mockClient.Verify(c => c.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(CosmosDB_DatabaseId, CosmosDB_CollectionId), null), Times.Once());
        }

        [Test]
        public void Add_Throws_On_Null_Entity()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();

            //Act
            var repository = new CosmosDBRepository<TModel>(mockClient.Object, CosmosDB_DatabaseId, CosmosDB_CollectionId);

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => repository.Add(null));
        }

        [Test]
        public void AddAsync_Throws_On_Null_Entity()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();
            TModel model = null;

            //Act
            var repository = new CosmosDBRepository<TModel>(mockClient.Object, CosmosDB_DatabaseId, CosmosDB_CollectionId);

            //Act, Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddAsync(model));
        }

        [Test]
        public void AddAsync_Overload_Throws_On_Null_Collection()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();
            IEnumerable<TModel> items = null;

            //Act
            var repository = new CosmosDBRepository<TModel>(mockClient.Object, CosmosDB_DatabaseId, CosmosDB_CollectionId);

            //Act, Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddAsync(items));
        }

        [Test]
        public void AddAsync_Overload_Throws_On_Null_Item_In_Collection()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();
            IEnumerable<TModel> items = new List<TModel> { new TModel(), null};

            //Act
            var repository = new CosmosDBRepository<TModel>(mockClient.Object, CosmosDB_DatabaseId, CosmosDB_CollectionId);

            //Act, Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddAsync(items));
        }

        [Test]
        public void Delete_Throws_On_Null_Entity()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();

            //Act
            var repository = new CosmosDBRepository<TModel>(mockClient.Object, CosmosDB_DatabaseId, CosmosDB_CollectionId);

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => repository.Delete(null));
        }

        [Test]
        public void DeleteAsync_Throws_On_Null_Id()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();
            string id = null;

            //Act
            var repository = new CosmosDBRepository<TModel>(mockClient.Object, CosmosDB_DatabaseId, CosmosDB_CollectionId);

            //Act, Assert
            Assert.ThrowsAsync<ArgumentException>(() => repository.DeleteAsync(id));
        }
        
        [Test]
        public void DeleteAsync_Throws_On_Empty_Id()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();
            string id = String.Empty;

            //Act
            var repository = new CosmosDBRepository<TModel>(mockClient.Object, CosmosDB_DatabaseId, CosmosDB_CollectionId);

            //Act, Assert
            Assert.ThrowsAsync<ArgumentException>(() => repository.DeleteAsync(id));
        }
        
        [Test]
        public void Update_Throws_On_Null_Entity()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();

            //Act
            var repository = new CosmosDBRepository<TModel>(mockClient.Object, CosmosDB_DatabaseId, CosmosDB_CollectionId);

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => repository.Update(null));
        }

        [Test]
        public void UpdateAsync_Throws_On_Null_Entity()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();
            TModel model = null;

            //Act
            var repository = new CosmosDBRepository<TModel>(mockClient.Object, CosmosDB_DatabaseId, CosmosDB_CollectionId);

            //Act, Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => repository.UpdateAsync(model));
        }

    }
}