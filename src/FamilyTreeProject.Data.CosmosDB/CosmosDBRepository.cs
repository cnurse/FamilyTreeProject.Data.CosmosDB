using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyTreeProject.Core.Collections;
using FamilyTreeProject.Core.Common;
using FamilyTreeProject.Core.Contracts;
using FamilyTreeProject.Core.Data;
using Microsoft.Azure.Documents.Client;
using FamilyTreeProject.Data.Common.Mapping;
using Microsoft.Azure.Documents;

namespace FamilyTreeProject.Data.CosmosDB
{
    public class CosmosDBRepository<TModel> : IRepository<TModel>, IAsyncRepository<TModel> where TModel : Entity
    {
        private readonly IDocumentClient _db;
        private readonly string _databaseId;
        private readonly string _collectionId;
        private readonly Uri _databaseUri;
        private readonly Uri _documentCollectionUri;
        
        public CosmosDBRepository(IDocumentClient db, string databaseId) :this(db, databaseId, typeof(TModel).Name)
        {
        }

        public CosmosDBRepository(IDocumentClient db, string databaseId, string collectionId)
        {            
            Requires.NotNull(db);
            Requires.NotNullOrEmpty("databaseId", databaseId);
            Requires.NotNullOrEmpty("collectionId", collectionId);

            _db = db;
            _databaseId = databaseId;
            _collectionId = collectionId;
            _databaseUri = UriFactory.CreateDatabaseUri(databaseId);
            _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
            
            Initialize();
        }
        
        private void Initialize()
        {
            Util.CreateDatabaseIfNotExistsAsync(_db, _databaseId).Wait();
            Util.CreateCollectionIfNotExistsAsync(_db, _databaseId, _collectionId).Wait();
        }
        
        public bool SupportsAggregates => true;

        /// <summary>
        /// Add an Item into the repository
        /// </summary>
        /// <param name="item">The item to be added</param>
        public void Add(TModel item)
        {
            Requires.NotNull(item);

            AddAsync(item).Wait();
        }
        
        /// <summary>
        /// Add an Item into the repository
        /// </summary>
        /// <param name="item">The item to be added</param>
        public async Task AddAsync(TModel item)
        {
            Requires.NotNull(item);
            
            await _db.CreateDocumentAsync(_documentCollectionUri, item.ToDataModel(true));
        }

        /// <summary>
        /// Add a collection of Items into the repository
        /// </summary>
        /// <param name="item">The items to be added</param>
        public async Task AddAsync(IEnumerable<TModel> items)
        {
            Requires.NotNull(items);
            
            foreach (var item in items)
            {
                await AddAsync(item);
            }
        }

        /// <summary>
        /// Determines whether an item exists in the repository
        /// </summary>
        /// <param name="id">The id of the item</param>
        /// <returns>True if the item exists, false if it does not.</returns>
        public async Task<bool> AnyAsync(string id)
        {
            return _db.CreateDocumentQuery<TModel>(_documentCollectionUri).Any(x => x.UniqueId == id);
        }

        /// <summary>
        /// Delete an Item from the repository
        /// </summary>
        /// <param name="item">The item to be deleted</param>
        public void Delete(TModel item)
        {
            Requires.NotNull(item);

            DeleteAsync(item.UniqueId).Wait();
        }

        /// <summary>
        /// Delete an Item from the repository
        /// </summary>
        /// <param name="id">The id of the item to be deleted</param>
        public async Task DeleteAsync(string id)
        {
            Requires.NotNullOrEmpty("id", id);

            await _db.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
        }

        /// <summary>
        /// Find items from the repository based on a Linq predicate
        /// </summary>
        /// <param name="predicate">The Linq predicate"</param>
        /// <returns>A list of items</returns>
        public IEnumerable<TModel> Find(Func<TModel, bool> predicate)
        {
            return FindAsync(predicate).Result;
        }

        /// <summary>
        /// Find a Page of items from the repository based on a Linq predicate
        /// </summary>
        /// <param name="pageIndex">The page Index to fetch</param>
        /// <param name="pageSize">The size of the page to fetch</param>
        /// <param name="predicate">The Linq predicate"</param>
        /// <returns>A list of items</returns>
        public IPagedList<TModel> Find(int pageIndex, int pageSize, Func<TModel, bool> predicate)
        {
            return FindAsync(pageIndex, pageSize, predicate).Result;
        }

        /// <summary>
        /// Find items from the repository based on a Linq predicate
        /// </summary>
        /// <param name="predicate">The Linq predicate"</param>
        /// <returns>A list of items</returns>
        public async Task<IEnumerable<TModel>> FindAsync(Func<TModel, bool> predicate)
        {
            return _db.CreateDocumentQuery<TModel>(_documentCollectionUri).Where(predicate);
        }

        /// <summary>
        /// Find a Page of items from the repository based on a Linq predicate
        /// </summary>
        /// <param name="pageIndex">The page Index to fetch</param>
        /// <param name="pageSize">The size of the page to fetch</param>
        /// <param name="predicate">The Linq predicate"</param>
        /// <returns>A list of items</returns>
        public async Task<IPagedList<TModel>> FindAsync(int pageIndex, int pageSize, Func<TModel, bool> predicate)
        {
            return _db.CreateDocumentQuery<TModel>(_documentCollectionUri).Where(predicate).InPagesOf(pageSize).GetPage(pageIndex);
        }

        /// <summary>
        /// Returns all the items in the repository as an enumerable list
        /// </summary>
        /// <returns>The list of items</returns>
        public IEnumerable<TModel> GetAll()
        {
            return GetAllAsync().Result;
        }

        /// <summary>
        /// Returns all the items in the repository as an enumerable list
        /// </summary>
        /// <returns>The list of items</returns>
        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            return _db.CreateDocumentQuery<TModel>(_documentCollectionUri);
        }

        /// <summary>
        /// returns a single item from the repository
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The item</returns>
        public async Task<TModel> GetAsync(string id)
        {
            return _db.CreateDocumentQuery<TModel>(_documentCollectionUri).SingleOrDefault(i => i.UniqueId == id);
        }

        /// <summary>
        /// Gets a list of items from the repository
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TModel>> GetAsync(IEnumerable<string> ids)
        {
            return _db.CreateDocumentQuery<TModel>(_documentCollectionUri).Where(i => ids.Contains(i.UniqueId));
        }

        /// <summary>
        /// Returns a page of items in the repository as a paged list
        /// </summary>
        /// <param name="pageIndex">The page Index to fetch</param>
        /// <param name="pageSize">The size of the page to fetch</param>
        /// <returns>The list of items</returns>
        public IPagedList<TModel> GetPage(int pageIndex, int pageSize)
        {
            return GetAll().InPagesOf(pageSize).GetPage(pageIndex);
        }

        /// <summary>
        /// Updates an Item in the repository
        /// </summary>
        /// <param name="item">The item to be updated</param>
        public void Update(TModel item)
        {
            Requires.NotNull(item);

            UpdateAsync(item).Wait();
        }

        /// <summary>
        /// Upserts an Item in the repository
        /// </summary>
        /// <param name="item">The item to be updated or created</param>
        public async Task UpdateAsync(TModel item)
        {
            Requires.NotNull(item);

            await _db.UpsertDocumentAsync(_documentCollectionUri, item.ToDataModel(true));
        }
    }
}