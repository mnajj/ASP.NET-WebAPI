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
		public IEnumerable<ItemDto> GetItems()
		{
			return repo.GetItems().Select(i => i.AsDto());
		}

		[HttpGet("{id}")]
		public ActionResult<ItemDto> GetItem(Guid id)
		{
			var item = repo.GetItem(id);
			if(item == null)
			{
				return NotFound();
			}
			return item.AsDto();
		}

		[HttpPost]
		public ActionResult<ItemDto> CreateItem(CreateItemDto itemDto)
		{
			Item item = new Item()
			{
				Id = new Guid(),
				Name = itemDto.Name,
				Price = itemDto.Price,
				CreatedDate = DateTimeOffset.UtcNow
			};
			repo.CreateItem(item);
			return CreatedAtAction(nameof(GetItem), new {id = item.Id}, item.AsDto());
		}

		[HttpPut("{id}")]
		public ActionResult UpdateItem(Guid id, UpdateItemDto itemDto)
		{
			var existingItem = repo.GetItem(id);
			if (existingItem == null)
			{
				return NotFound();
			}
			Item updatedItem = existingItem with
			{
				Name = itemDto.Name,
				Price = itemDto.Price
			};
			repo.UpdateItem(updatedItem);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public ActionResult DeleteItem(Guid id)
		{
			var existingItem = repo.GetItem(id);
			if (existingItem == null)
			{
				return NotFound();
			}
			repo.DeleteItem(id);
			return NoContent();
		}
	}
}
