using MongoDB.Driver;
using WebAPI.Model;

namespace WebAPI.Repositories
{
	public class MongoDbItemsRepo : IItemsRepo
	{
		private const string databaseName = "Catalog";
		private readonly IMongoCollection<Item> _itemsCollection;
		private const string collectionName = "Items";


		public MongoDbItemsRepo(IMongoClient mongoClient)
		{
			IMongoDatabase database = mongoClient.GetDatabase(databaseName);
			_itemsCollection = database.GetCollection<Item>(collectionName);
		}

		public void CreateItem(Item item)
		{
			_itemsCollection.InsertOne(item);
		}

		public void DeleteItem(Guid id)
		{
			throw new NotImplementedException();
		}

		public Item GetItem(Guid id)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Item> GetItems()
		{
			throw new NotImplementedException();
		}

		public void UpdateItem(Item item)
		{
			throw new NotImplementedException();
		}
	}
}
