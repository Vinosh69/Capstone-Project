using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RentAPlace.Data;
using System.Security.Claims;

namespace RentAplace.Tests;

public static class TestHelpers
{
    public static ApplicationDbContext CreateInMemoryDb(string name)
    {
        var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(name)
            .EnableSensitiveDataLogging()
            .Options;
        return new ApplicationDbContext(opts);
    }

    public static IConfiguration CreateJwtConfig()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "TestJwtKey_123456789012345678901234567890",
            ["Jwt:Issuer"] = "TestIssuer",
            ["Jwt:Audience"] = "TestAudience"
        };
        return new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
    }

    public static void SetUser(ControllerBase controller, string email, string role = "User", string name = "Test")
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Name, name)
        }, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }
}

