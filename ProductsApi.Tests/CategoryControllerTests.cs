using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductsApi.Controllers;
using ProductsApi.CQRS.Queries;
using ProductsApi.DTOs;
using ProductsApi.Services;

namespace ProductsApi.Tests;

public class CategoryControllerTests
{
    [Fact]
    public async Task GetById_ReturnsOkWithCategory_WhenCategoryExists()
    {
        var category = new CategoryReadDto
        {
            Id = 1,
            Name = "Laptops",
            Slug = "laptops"
        };

        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(
                It.Is<GetCategoryByIdQuery>(q => q.Id == category.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var controller = new CategoryController(Mock.Of<ICategoryService>(), mediator.Object);

        var result = await controller.GetById(category.Id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<CategoryReadDto>(okResult.Value);

        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(category.Id, actual.Id);
        Assert.Equal(category.Name, actual.Name);
        Assert.Equal(category.Slug, actual.Slug);
        mediator.Verify(m => m.Send(
            It.Is<GetCategoryByIdQuery>(q => q.Id == category.Id),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        const int categoryId = 99;

        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(
                It.Is<GetCategoryByIdQuery>(q => q.Id == categoryId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CategoryReadDto?)null);

        var controller = new CategoryController(Mock.Of<ICategoryService>(), mediator.Object);

        var result = await controller.GetById(categoryId);

        Assert.IsType<NotFoundResult>(result);
        mediator.Verify(m => m.Send(
            It.Is<GetCategoryByIdQuery>(q => q.Id == categoryId),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
