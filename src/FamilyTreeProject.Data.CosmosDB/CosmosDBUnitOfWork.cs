using System.Threading.Tasks;
using FamilyTreeProject.Common.Data;
using FamilyTreeProject.Common.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Naif.Core.Contracts;

namespace FamilyTreeProject.Data.CosmosDB
{
    public class CosmosDBUnitOfWork: IUnitOfWork
    {
        private readonly IDocumentClient _db;
        private readonly string _databaseId;
        private readonly string _collectionId;
      
        public CosmosDBUnitOfWork(IDocumentClient db, string databaseId, string collectionId)
        {
            Requires.NotNull(db);
            Requires.NotNullOrEmpty("databaseId", databaseId);
            Requires.NotNullOrEmpty("collectionId", collectionId);
            
            _db = db;
            _databaseId = databaseId;
            _collectionId = collectionId;

            Initialize();
        }
        
        public void Dispose()
        {
        }

        public void Commit()
        {
        }

        public IRepository<T> GetRepository<T>() where T : Entity
        {
            return new CosmosDBRepository<T>(_db, _databaseId, _collectionId);
        }

        private void Initialize()
        {
            Util.CreateDatabaseIfNotExistsAsync(_db, _databaseId).Wait();
            Util.CreateCollectionIfNotExistsAsync(_db, _databaseId, _collectionId).Wait();
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await _db.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _db.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(_databaseId),
                        new DocumentCollection
                        {
                            Id = _collectionId
                        },
                        new RequestOptions { OfferThroughput = 400 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}