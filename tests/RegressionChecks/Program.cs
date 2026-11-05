using System.ComponentModel.DataAnnotations;
using AuthService.Data;
using AuthService.DTOs;
using AuthService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProductService.DTOs;

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

static bool IsValid(object value) =>
    Validator.TryValidateObject(value, new ValidationContext(value), new List<ValidationResult>(), true);

Check(!IsValid(new ProductFilterDto { PageSize = 0 }), "PageSize=0 must be rejected");
Check(!IsValid(new ProductFilterDto { Page = -1 }), "Negative page must be rejected");
Check(!IsValid(new ProductFilterDto { PageSize = 101 }), "Unbounded pages must be rejected");
Check(!IsValid(new ProductFilterDto { MinPrice = 20, MaxPrice = 10 }), "Inverted price ranges must be rejected");
Check(IsValid(new ProductFilterDto()), "Default filters should be valid");
Check(!IsValid(new UserRegisterDto()), "Empty registration must be rejected");

var options = new DbContextOptionsBuilder<AuthDbContext>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
using var db = new AuthDbContext(options);
var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Jwt:Secret"] = "regression-test-only-key-at-least-32-bytes"
}).Build();
var service = new AuthServiceClass(db, config);
service.Register(new UserRegisterDto
{
    Username = "newclient", FirstName = "Test", LastName = "User",
    Email = "test@example.com", Password = "test-password", Role = "Admin"
});
var user = db.Users.Single();
Check(user.Role == "Cliente", "Public registration must not accept privileged roles");
Check(BCrypt.Net.BCrypt.Verify("test-password", user.PasswordHash), "Password must be hashed");
Check(service.Login(new AuthService.Models.LoginRequest { Username = "newclient", Password = "wrong" }) is null,
    "Invalid password must not produce a token");
Console.WriteLine("All regression checks passed.");
