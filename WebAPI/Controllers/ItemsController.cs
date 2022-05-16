using Microsoft.AspNetCore.Mvc;
using WebAPI.Model;
using WebAPI.Repositories;
using static WebAPI.Dtos;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("items")]
	public class ItemsController : Controller
	{
		private readonly IItemsRepo repo;
		private readonly ILogger<ItemsController> _logger;

		public ItemsController(IItemsRepo repo, ILogger<ItemsController> logger)
		{
			this.repo = repo;
			this._logger = logger;
		}

		[HttpGet]
		public async Task<IEnumerable<ItemDto>> GetItemsAsync(string nameToMatch = null)
		{
			var items = (await repo.GetItemsAsync()).Select(i => i.AsDto());
			if (!String.IsNullOrWhiteSpace(nameToMatch))
			{
				items = items.Where(i => i.Name.Contains(
					nameToMatch, StringComparison.OrdinalIgnoreCase));
			}
			return items;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
		{
			var item = await repo.GetItemAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			return item.AsDto();
		}

		[HttpPost]
		public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
		{
			Item item = new Item()
			{
				Id = new Guid(),
				Name = itemDto.Name,
				Description = itemDto.Description,
				Price = itemDto.Price,
				CreatedDate = DateTimeOffset.UtcNow
			};
			await repo.CreateItemAsync(item);
			return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
		{
			var existingItem = await repo.GetItemAsync(id);
			if (existingItem == null)
			{
				return NotFound();
			}
			existingItem.Name = itemDto.Name;
			existingItem.Price = itemDto.Price;
			await repo.UpdateItemAsync(existingItem);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteItemAsync(Guid id)
		{
			var existingItem = await repo.GetItemAsync(id);
			if (existingItem == null)
			{
				return NotFound();
			}
			await repo.DeleteItemAsync(id);
			return NoContent();
		}
	}
}
