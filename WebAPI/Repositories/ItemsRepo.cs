using WebAPI.Model;

namespace WebAPI.Repositories
{
	public class ItemsRepo
	{
		private readonly List<Item> Items = new()
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

		public IEnumerable<Item> GetItems() => Items;

		public Item GetItem(Guid id) => Items.Where(i => i.Id == id).SingleOrDefault();
	}
}
