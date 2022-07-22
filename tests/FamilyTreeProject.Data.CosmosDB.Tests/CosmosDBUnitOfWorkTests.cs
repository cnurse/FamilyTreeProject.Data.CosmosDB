using System;
using FamilyTreeProject.Common.Data;
using FamilyTreeProject.Common.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Moq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace FamilyTreeProject.Data.CosmosDB.Tests
{
    [TestFixture]
    public class CosmosDBUnitOfWorkTests : CosmosDBTestBase
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
            Assert.Throws<ArgumentNullException>(() => new CosmosDBUnitOfWork(client, It.IsAny<string>(), It.IsAny<string>()));
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
            Assert.Throws<ArgumentException>(() => new CosmosDBUnitOfWork(mockClient.Object, String.Empty, It.IsAny<string>()));
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
            Assert.Throws<ArgumentException>(() => new CosmosDBUnitOfWork(mockClient.Object, It.IsAny<string>(), String.Empty));
        }
        
        [Test]
        public void Constructor_Checks_If_Database_Exists()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();

            //Act
            var unitOfWork = new CosmosDBUnitOfWork(mockClient.Object, CosmosDB_DatabaseId, CosmosDB_CollectionId);

            //Assert
            mockClient.Verify(c => c.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(CosmosDB_DatabaseId), null), Times.Once());
        }

        [Test]
        public void Constructor_Checks_If_Collection_Exists()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();

            //Act
            var unitOfWork = new CosmosDBUnitOfWork(mockClient.Object, CosmosDB_DatabaseId, CosmosDB_CollectionId);

            //Assert
            mockClient.Verify(c => c.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(CosmosDB_DatabaseId, CosmosDB_CollectionId), null), Times.Once());
        }
        
        [Test]
        public void GetRepository_Returns_Repository()
        {
            //Arrange
            var mockClient = new Mock<IDocumentClient>();
            var unitOfWork = new CosmosDBUnitOfWork(mockClient.Object, CosmosDB_DatabaseId, CosmosDB_CollectionId);

            //Act
            var rep = unitOfWork.GetRepository<Individual>();

            //Assert
            Assert.IsInstanceOf<IRepository<Individual>>(rep);
        }
    }
}