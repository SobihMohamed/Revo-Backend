using FluentAssertions;
using Revo.API.Resposes;
using Revo.Application.Common.Pagination;
using Revo.Application.Features.PortfolioItems.Dto;
using Revo.IntegrationTests.Infrastructre;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace Revo.IntegrationTests.Features.Portfolio
{
    [Collection("SharedTestCollection")]
    public class GetPortfolioItemsTests : BaseIntegrationTest
    {
        public GetPortfolioItemsTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        // 1. Get By Id - Happy Path
        [Fact]
        public async Task GetById_WhenItemExists_ShouldReturn200OK_WithDetails()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Domain.Entities.Category { Id = categoryId, NameAr = "تصنيف", NameEn = "Cat" };
            await _context.Categories.AddAsync(category);

            var portfolioId = Guid.NewGuid();
            var portfolioItem = new Domain.Entities.PortfolioItem
            {
                Id = portfolioId,
                CategoryId = categoryId,
                CaptionAr = "تفاصيل المشروع",
                CaptionEn = "Item Details",
                OrderIndex = 1
            };
            await _context.PortfolioItems.AddAsync(portfolioItem);
            await _context.SaveChangesAsync();

            // Act
            var response = await HttpClient.GetAsync($"api/portfolioitems/{portfolioId}");

            // Assert
            var responseText = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Expected 200 OK : {responseText}");

            var responseData = await response.Content.ReadFromJsonAsync<ApiResponse<PortfolioItemDetailsDto>>();
            responseData.Should().NotBeNull();
            responseData!.IsSuccess.Should().BeTrue();
            responseData.Data.Should().NotBeNull();
            responseData.Data!.Id.Should().Be(portfolioId);
            responseData.Data.CaptionEn.Should().Be("Item Details");
        }

        // 2. Get By Id - Negative Path (Not Found)
        [Fact]
        public async Task GetById_WhenItemDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var randomId = Guid.NewGuid();

            // Act
            var response = await HttpClient.GetAsync($"api/portfolioitems/{randomId}");

            // Assert
            var responseText = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            responseText.Should().Contain("PortfolioItemNotFound");
        }

        // 3. Get All - Testing Filters (Category & Search)
        [Fact]
        public async Task GetAll_WhenApplyingFilters_ShouldReturnCorrectPaginatedData()
        {
            var cat1 = new Domain.Entities.Category { Id = Guid.NewGuid(), NameAr = "تصنيف 1", NameEn = "Cat 1" };
            var cat2 = new Domain.Entities.Category { Id = Guid.NewGuid(), NameAr = "تصنيف 2", NameEn = "Cat 2" };
            await _context.Categories.AddRangeAsync(cat1, cat2);

            var item1 = new Domain.Entities.PortfolioItem { Id = Guid.NewGuid(), CategoryId = cat1.Id, CaptionEn = "Web Application", CaptionAr = "تطبيق ويب" };
            var item2 = new Domain.Entities.PortfolioItem { Id = Guid.NewGuid(), CategoryId = cat1.Id, CaptionEn = "Mobile App", CaptionAr = "تطبيق موبايل" };
            var item3 = new Domain.Entities.PortfolioItem { Id = Guid.NewGuid(), CategoryId = cat2.Id, CaptionEn = "Desktop System", CaptionAr = "نظام سطح مكتب" };
            await _context.PortfolioItems.AddRangeAsync(item1, item2, item3);

            await _context.SaveChangesAsync();

            // Act 1 filter by category (cat1) get 2 projects only
            var responseByCat = await HttpClient.GetAsync($"api/portfolioitems?categoryId={cat1.Id}&pageIndex=1&pageSize=10");
            responseByCat.StatusCode.Should().Be(HttpStatusCode.OK);
            var dataByCat = await responseByCat.Content.ReadFromJsonAsync<ApiResponse<PaginationResponse<PortfolioItemListDto>>>();
            dataByCat!.Data!.Data.Should().OnlyContain(i => i.CaptionEn == "Web Application" || i.CaptionEn == "Mobile App");
            dataByCat.Data.TotalCount.Should().Be(2);

            // Act 2 filter by search term (Mobile) get 1 project only
            var responseBySearch = await HttpClient.GetAsync($"api/portfolioitems?search=Mobile&pageIndex=1&pageSize=10");
            responseBySearch.StatusCode.Should().Be(HttpStatusCode.OK);
            var dataBySearch = await responseBySearch.Content.ReadFromJsonAsync<ApiResponse<PaginationResponse<PortfolioItemListDto>>>();
            dataBySearch!.Data!.Data.Should().ContainSingle(i => i.CaptionEn == "Mobile App");

            // Act 3 filter by search term that doesn't exist (NonsenseWord) get 0 projects
            var responseEmpty = await HttpClient.GetAsync($"api/portfolioitems?search=NonsenseWord&pageIndex=1&pageSize=10");
            responseEmpty.StatusCode.Should().Be(HttpStatusCode.OK);
            var dataEmpty = await responseEmpty.Content.ReadFromJsonAsync<ApiResponse<PaginationResponse<PortfolioItemListDto>>>();
            dataEmpty!.Data!.TotalCount.Should().Be(0);
            dataEmpty.Data.Data.Should().BeEmpty();
        }
    }
}