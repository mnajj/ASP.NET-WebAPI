using MongoDB.Bson;
using MongoDB.Driver;
using WebAPI.Model;

namespace WebAPI.Repositories
{
	public class MongoDbItemsRepo : IItemsRepo
	{
		private const string databaseName = "Catalog";
		private readonly IMongoCollection<Item> _itemsCollection;
		private const string collectionName = "Items";
		private readonly FilterDefinitionBuilder<Item> _filterBuilder = Builders<Item>.Filter;

		public MongoDbItemsRepo(IMongoClient mongoClient)
		{
			IMongoDatabase database = mongoClient.GetDatabase(databaseName);
			_itemsCollection = database.GetCollection<Item>(collectionName);
		}

		public async Task CreateItemAsync(Item item)
		{
			await _itemsCollection.InsertOneAsync(item);
		}

		public async Task<Item> GetItemAsync(Guid id)
		{
			var filter = _filterBuilder.Eq(i => i.Id, id);
			return await _itemsCollection.Find(filter).SingleOrDefaultAsync();
		}

		public async Task<IEnumerable<Item>> GetItemsAsync() =>
			await _itemsCollection.Find(new BsonDocument()).ToListAsync();

		public async Task UpdateItemAsync(Item item)
		{
			var filter = _filterBuilder.Eq(i => i.Id, item.Id);
			await _itemsCollection.ReplaceOneAsync(filter, item);
		}

		public async Task DeleteItemAsync(Guid id)
		{
			var filter = _filterBuilder.Eq(i => i.Id, id);
			await _itemsCollection.DeleteOneAsync(filter);
		}
	}
}
