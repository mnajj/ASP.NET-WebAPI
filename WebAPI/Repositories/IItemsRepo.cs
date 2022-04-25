using WebAPI.Model;

namespace WebAPI.Repositories
{
	public interface IItemsRepo
	{
		Task<Item> GetItemAsync(Guid id);
		Task<IEnumerable<Item>> GetItemsAsync();
		Task CreateItemAsync (Item item);
		Task UpdateItemAsync (Item item);
		Task DeleteItemAsync (Guid id);
	}
}