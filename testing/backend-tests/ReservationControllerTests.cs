using Microsoft.AspNetCore.Mvc;
using RentAPlace.Controllers;
using RentAPlace.Models;
using RentAPlace.Models.DTOs;

namespace RentAplace.Tests;

public class ReservationControllerTests
{
    [Fact]
    public async Task CreateReservation_ReturnsBadRequest_WhenDatesMissing()
    {
        await using var db = TestHelpers.CreateInMemoryDb(nameof(CreateReservation_ReturnsBadRequest_WhenDatesMissing));
        db.Users.Add(new User { Name = "Renter", Email = "r@test.com", PasswordHash = "p", Role = "User" });
        db.Users.Add(new User { Name = "Owner", Email = "o@test.com", PasswordHash = "p", Role = "Owner" });
        await db.SaveChangesAsync();

        db.Properties.Add(new Property { Title = "P", OwnerId = db.Users.Single(u => u.Email == "o@test.com").Id, Location = "Goa", PropertyType = "Villa", PricePerNight = 1000 });
        await db.SaveChangesAsync();

        var controller = new ReservationController(db, TestHelpers.CreateJwtConfig());
        TestHelpers.SetUser(controller, "r@test.com", "User");

        var res = new Reservation { PropertyId = db.Properties.Single().Id };
        var result = await controller.CreateReservation(res);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Check-in and check-out dates are required.", bad.Value);
    }

    [Fact]
    public async Task CreateReservation_ReturnsBadRequest_WhenOverlapsExisting()
    {
        await using var db = TestHelpers.CreateInMemoryDb(nameof(CreateReservation_ReturnsBadRequest_WhenOverlapsExisting));

        var owner = new User { Name = "Owner", Email = "o@test.com", PasswordHash = "p", Role = "Owner" };
        var renter = new User { Name = "Renter", Email = "r@test.com", PasswordHash = "p", Role = "User" };
        db.Users.AddRange(owner, renter);
        await db.SaveChangesAsync();

        var prop = new Property { Title = "P", OwnerId = owner.Id, Location = "Goa", PropertyType = "Villa", PricePerNight = 1000 };
        db.Properties.Add(prop);
        await db.SaveChangesAsync();

        db.Reservations.Add(new Reservation
        {
            PropertyId = prop.Id,
            RenterId = renter.Id,
            Status = "Confirmed",
            CheckInDate = new DateTime(2026, 3, 20),
            CheckOutDate = new DateTime(2026, 3, 22)
        });
        await db.SaveChangesAsync();

        var controller = new ReservationController(db, TestHelpers.CreateJwtConfig());
        TestHelpers.SetUser(controller, "r@test.com", "User");

        var request = new Reservation
        {
            PropertyId = prop.Id,
            CheckInDate = new DateTime(2026, 3, 21),
            CheckOutDate = new DateTime(2026, 3, 23)
        };

        var result = await controller.CreateReservation(request);
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Property is not available for the selected dates.", bad.Value);
    }

    [Fact]
    public async Task CreateReservation_ReturnsOk_WhenAvailable()
    {
        await using var db = TestHelpers.CreateInMemoryDb(nameof(CreateReservation_ReturnsOk_WhenAvailable));

        var owner = new User { Name = "Owner", Email = "o@test.com", PasswordHash = "p", Role = "Owner" };
        var renter = new User { Name = "Renter", Email = "r@test.com", PasswordHash = "p", Role = "User" };
        db.Users.AddRange(owner, renter);
        await db.SaveChangesAsync();

        var prop = new Property { Title = "P", OwnerId = owner.Id, Location = "Goa", PropertyType = "Villa", PricePerNight = 1000 };
        db.Properties.Add(prop);
        await db.SaveChangesAsync();

        var controller = new ReservationController(db, TestHelpers.CreateJwtConfig());
        TestHelpers.SetUser(controller, "r@test.com", "User");

        var request = new Reservation
        {
            PropertyId = prop.Id,
            CheckInDate = new DateTime(2026, 3, 25),
            CheckOutDate = new DateTime(2026, 3, 27)
        };

        var result = await controller.CreateReservation(request);
        var ok = Assert.IsType<OkObjectResult>(result);
        var saved = Assert.IsType<ReservationResponseDto>(ok.Value);
        Assert.Equal("Pending", saved.Status);
        Assert.Equal(renter.Id, saved.RenterId);
    }
}

