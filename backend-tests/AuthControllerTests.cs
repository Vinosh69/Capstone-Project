using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentAPlace.Models;
using RentAplace.Controllers;

namespace RentAplace.Tests;

public class AuthControllerTests
{
    [Fact]
    public async Task Register_ReturnsBadRequest_WhenEmailExists()
    {
        await using var db = TestHelpers.CreateInMemoryDb(nameof(Register_ReturnsBadRequest_WhenEmailExists));
        db.Users.Add(new User { Name = "A", Email = "a@test.com", PasswordHash = "p", Role = "User" });
        await db.SaveChangesAsync();

        var controller = new AuthController(db, TestHelpers.CreateJwtConfig());
        var result = await controller.Register(new User { Name = "B", Email = "a@test.com", PasswordHash = "p2", Role = "Owner" });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Email already exists", bad.Value);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenInvalidCredentials()
    {
        await using var db = TestHelpers.CreateInMemoryDb(nameof(Login_ReturnsUnauthorized_WhenInvalidCredentials));
        db.Users.Add(new User { Name = "A", Email = "a@test.com", PasswordHash = "correct", Role = "User" });
        await db.SaveChangesAsync();

        var controller = new AuthController(db, TestHelpers.CreateJwtConfig());
        var result = await controller.Login(new LoginRequest { Email = "a@test.com", Password = "wrong" });

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid credentials", unauthorized.Value);
    }

    [Fact]
    public async Task Login_ReturnsToken_WhenValidCredentials()
    {
        await using var db = TestHelpers.CreateInMemoryDb(nameof(Login_ReturnsToken_WhenValidCredentials));
        db.Users.Add(new User { Name = "A", Email = "a@test.com", PasswordHash = "p", Role = "User" });
        await db.SaveChangesAsync();

        var controller = new AuthController(db, TestHelpers.CreateJwtConfig());
        var result = await controller.Login(new LoginRequest { Email = "a@test.com", Password = "p" });

        var ok = Assert.IsType<OkObjectResult>(result);
        var tokenProp = ok.Value!.GetType().GetProperty("token");
        Assert.NotNull(tokenProp);
        var token = tokenProp!.GetValue(ok.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Contains(".", token); // JWT looks like header.payload.signature
    }
}

