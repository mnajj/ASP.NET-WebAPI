using WebAPI.Model;

namespace WebAPI.Repositories
{
	public class ItemsRepo : IItemsRepo
	{
		private readonly List<Item> items = new()
		{
			new Item
			{
				Id = Guid.NewGuid(),
				Name = "Potion",
				Price = 9,
				CreatedDate = DateTimeOffset.UtcNow
			},
			new Item
			{
				Id = Guid.NewGuid(),
				Name = "Iron Sword",
				Price = 15,
				CreatedDate = DateTimeOffset.UtcNow
			},
			new Item
			{
				Id = Guid.NewGuid(),
				Name = "Bronze Shield",
				Price = 30,
				CreatedDate = DateTimeOffset.UtcNow
			},
		};

		public IEnumerable<Item> GetItems() => items;

		public Item GetItem(Guid id) => items.Where(i => i.Id == id).SingleOrDefault();

		public void CreateItem(Item item) => items.Add(item);

		public void UpdateItem(Item item)
		{
			var index = items.FindIndex(i => i.Id == item.Id);
			items[index] = item;
		}

		public void DeleteItem(Guid id)
		{
			var index = items.FindIndex(i => i.Id == id);
			items.RemoveAt(index);
		}
	}
}
