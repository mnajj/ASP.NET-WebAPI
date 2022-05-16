using Moq;
using Xunit;
using System;
using WebAPI.Repositories;
using WebAPI.Model;
using WebAPI.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using static WebAPI.Dtos;
using System.Collections.Generic;

namespace WebAPIUnitTests
{
	public class ItemsControllerTests
	{
		private readonly Mock<IItemsRepo> _repoStub = new Mock<IItemsRepo>();
		private readonly Mock<ILogger<ItemsController>> _logger =
			new Mock<ILogger<ItemsController>>();
		private readonly Random _random = new();

		[Fact]
		public async void GetItemAsync_WithUnexistingItem_ReturnsNotFound()
		{
			// Arrange
			_repoStub.Setup(r => r.GetItemAsync(It.IsAny<Guid>()))
				.ReturnsAsync((Item)null);
			var controller = new ItemsController(_repoStub.Object, _logger.Object);

			// Act
			var result = await controller.GetItemAsync(Guid.NewGuid());

			// Assert
			result.Result.Should().BeOfType<NotFoundResult>();
		}

		[Fact]
		public async void GetItemAsync_WithexistingItem_ReturnsExpectedItem()
		{
			// Arrange
			var expectedItem = CreateRandomItem();
			_repoStub.Setup(r => r.GetItemAsync(It.IsAny<Guid>()))
				.ReturnsAsync(expectedItem);
			var controller = new ItemsController(_repoStub.Object, _logger.Object);

			// Act
			var result = await controller.GetItemAsync(Guid.NewGuid());

			// Assert
			result.Value.Should().BeEquivalentTo(expectedItem);
		}

		[Fact]
		public async void GetItemsAsync_WithExistingItems_ReturnsAllItems()
		{
			// Arrange
			var expectedItems = new[]
			{
				CreateRandomItem(),
				CreateRandomItem(),
				CreateRandomItem()
			};
			_repoStub.Setup(r => r.GetItemsAsync())
				.ReturnsAsync(expectedItems);
			var controller = new ItemsController(_repoStub.Object, _logger.Object);

			// Act
			var actualItems = await controller.GetItemsAsync();

			// Assert
			actualItems.Should().BeEquivalentTo(expectedItems);
		}

		[Fact]
		public async void CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
		{
			// Arrange
			var itemToCreate = new CreateItemDto(
				Guid.NewGuid().ToString(),
				Guid.NewGuid().ToString(),
				_random.Next(1000));
			var controller = new ItemsController(_repoStub.Object, _logger.Object);

			// Act
			var result = await controller.CreateItemAsync(itemToCreate);

			// Assert
			var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
			itemToCreate.Should().BeEquivalentTo(
				createdItem,
				opts => opts.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
				);
			// createdItem.Id.Should().NotBeEmpty();
			createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(1000));
		}

		[Fact]
		public async void UpdateItemAsync_WithExistingItem_ReturnsNoContent()
		{
			// Arrange
			var existingItem = CreateRandomItem();
			_repoStub.Setup(r => r.GetItemAsync(It.IsAny<Guid>()))
				.ReturnsAsync(existingItem);
			var itemId = existingItem.Id;
			var itemToUpdate = new UpdateItemDto(
				Guid.NewGuid().ToString(),
				Guid.NewGuid().ToString(),
				existingItem.Price + 3);
			var controller = new ItemsController(_repoStub.Object, _logger.Object);

			// Act
			var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

			// Assert
			result.Should().BeOfType<NoContentResult>();
		}

		[Fact]
		public async void DeleteItemAsync_WithExistingItem_ReturnsNoContent()
		{
			// Arrange
			var existingItem = CreateRandomItem();
			_repoStub.Setup(r => r.GetItemAsync(It.IsAny<Guid>()))
				.ReturnsAsync(existingItem);
			var controller = new ItemsController(_repoStub.Object, _logger.Object);

			// Act
			var result = await controller.DeleteItemAsync(existingItem.Id);

			// Assert
			result.Should().BeOfType<NoContentResult>();
		}

		[Fact]
		public async void GetItemsAsync_WithMatchingItems_ReturnsMatchingItems()
		{
			// Arrange
			var allItems = new[]
			{
				new Item() {Name = "Potion"},
				new Item() {Name = "Antidote"},
				new Item() {Name = "Hi-Potion"}
			};
			string nameToMatch = "Potion";
			_repoStub.Setup(r => r.GetItemsAsync())
				.ReturnsAsync(allItems);
			var controller = new ItemsController(_repoStub.Object, _logger.Object);

			// Act
			IEnumerable<ItemDto> foundItems = await controller.GetItemsAsync(nameToMatch);

			// Assert
			foundItems.Should().OnlyContain(
				i => i.Name == allItems[0].Name || i.Name == allItems[2].Name
				);
		}


		private Item CreateRandomItem() =>
		new()
		{
			Id = Guid.NewGuid(),
			Name = Guid.NewGuid().ToString(),
			Price = _random.Next(1000),
			CreatedDate = DateTimeOffset.UtcNow
		};
	}
}