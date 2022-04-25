using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Model;
using WebAPI.Repositories;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("items")]
	public class ItemsController : Controller
	{
		private readonly IItemsRepo repo;

		public ItemsController(IItemsRepo repo)
		{
			this.repo = repo; 
		}

		[HttpGet]
		public async Task<IEnumerable<ItemDto>> GetItemsAsync()
		{
			return (await repo.GetItemsAsync()).Select(i => i.AsDto());
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
		{
			var item = await repo.GetItemAsync(id);
			if(item == null)
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
				Price = itemDto.Price,
				CreatedDate = DateTimeOffset.UtcNow
			};
			await repo.CreateItemAsync(item);
			return CreatedAtAction(nameof(GetItemAsync), new {id = item.Id}, item.AsDto());
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
		{
			var existingItem = await repo.GetItemAsync(id);
			if (existingItem == null)
			{
				return NotFound();
			}
			Item updatedItem = existingItem with
			{
				Name = itemDto.Name,
				Price = itemDto.Price
			};
			await repo.UpdateItemAsync(updatedItem);
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
