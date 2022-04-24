using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
	public record UpdateItemDto
	{
		[Required]
		public string Name { get; init; }
		[Required]
		[Range(1, 1000)]
		public decimal Price { get; init; }
	}
}
