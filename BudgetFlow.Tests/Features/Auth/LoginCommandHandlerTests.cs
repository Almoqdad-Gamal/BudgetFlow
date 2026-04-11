using BudgetFlow.Application.Common.Exceptions;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Features.Auth.Commands.Login;
using BudgetFlow.Application.Features.Auth.Commands.Register;
using BudgetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace BudgetFlow.Tests.Features.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IJwtService> _jwtServiceMock;

    public LoginCommandHandlerTests()
    {
        _jwtServiceMock = new Mock<IJwtService>();
        _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<AppUser>()))
            .Returns("fake-jwt-token");
        _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("fake-refresh-token");
    }

    private async Task<TestDbContext> CreateContextWithTenant(string dbName)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var context = new TestDbContext(options);
        context.Database.EnsureCreated();

        var tenant = new BudgetFlow.Domain.Entities.Tenant
        {
            Name = "Acme Corp",
            Subdomain = "acme",
            IsActive = true,
            MaxDepartments = 2,
            MaxUsers = 5
        };
        context.Tenants.Add(tenant);

        var user = new BudgetFlow.Domain.Entities.AppUser
        {
            TenantId = tenant.Id,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@acme.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
            Role = BudgetFlow.Domain.Enums.UserRole.TenantAdmin,
            IsActive = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        return context;
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsLoginResult()
    {
        // Arrange
        var context = await CreateContextWithTenant("LoginValidTest");
        var handler = new LoginCommandHandler(context, _jwtServiceMock.Object);
        var command = new LoginCommand(
            Email: "john@acme.com",
            Password: "Password123",
            Subdomain: "acme"
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Token.ShouldBe("fake-jwt-token");
        result.FullName.ShouldBe("John Doe");
        result.Role.ShouldBe("TenantAdmin");
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsValidationException()
    {
        // Arrange
        var context = await CreateContextWithTenant("LoginWrongPasswordTest");
        var handler = new LoginCommandHandler(context, _jwtServiceMock.Object);
        var command = new LoginCommand(
            Email: "john@acme.com",
            Password: "WrongPassword123",
            Subdomain: "acme"
        );

        // Act & Assert
        await Should.ThrowAsync<ValidationException>(
            () => handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_WrongSubdomain_ThrowsNotFoundException()
    {
        // Arrange
        var context = await CreateContextWithTenant("LoginWrongSubdomainTest");
        var handler = new LoginCommandHandler(context, _jwtServiceMock.Object);
        var command = new LoginCommand(
            Email: "john@acme.com",
            Password: "Password123",
            Subdomain: "wrong-subdomain"
        );

        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(
            () => handler.Handle(command, CancellationToken.None)
        );
    }
}