using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using RentAPlace.Controllers;
using RentAPlace.Models;
using RentAPlace.Models.DTOs;

namespace RentAplace.Tests;

public class PropertyControllerTests
{
    private sealed class FakeEnv : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Test";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = Path.GetTempPath();
        public string EnvironmentName { get; set; } = "Development";
        public string ContentRootPath { get; set; } = Path.GetTempPath();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    [Fact]
    public async Task GetAllProperties_ReturnsPropertyResponseDto_WithoutSensitiveOwnerFields()
    {
        await using var db = TestHelpers.CreateInMemoryDb(nameof(GetAllProperties_ReturnsPropertyResponseDto_WithoutSensitiveOwnerFields));

        var owner = new User { Name = "Owner", Email = "o@test.com", PasswordHash = "secret", Role = "Owner" };
        db.Users.Add(owner);
        await db.SaveChangesAsync();

        var prop = new Property
        {
            Title = "Luxury",
            Location = "Goa",
            PropertyType = "Villa",
            PricePerNight = 7000,
            OwnerId = owner.Id,
            Images = new List<PropertyImage>
            {
                new PropertyImage { ImageUrl = "/images/properties/x.jpg" }
            }
        };
        db.Properties.Add(prop);
        await db.SaveChangesAsync();

        var controller = new PropertyController(db, new FakeEnv());
        var result = await controller.GetAllProperties();
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<PropertyResponseDto>>(ok.Value);

        Assert.Single(list);
        Assert.Equal("Luxury", list[0].Title);
        Assert.NotNull(list[0].Owner);
        Assert.Equal(owner.Email, list[0].Owner!.Email);
        Assert.NotEmpty(list[0].Images);

        // DTO should not expose password hash: only OwnerSummaryDto is returned
        Assert.Null(list[0].Owner!.GetType().GetProperty("PasswordHash"));
    }
}

