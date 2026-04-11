using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Features.Auth.Commands.Register;
using BudgetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;
using ValidationException = BudgetFlow.Application.Common.Exceptions.ValidationException;

namespace BudgetFlow.Tests.Features.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IJwtService> _jwtServiceMock;

    public RegisterCommandHandlerTests()
    {
        _jwtServiceMock = new Mock<IJwtService>();
        _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<AppUser>()))
            .Returns("fake-jwt-token");
        _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("fake-refresh-token");
    }

    private static TestDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var context = new TestDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsTokenAndIds()
    {
        // Arrange
        var context = CreateContext(Guid.NewGuid().ToString());
        var handler = new RegisterCommandHandler(context, _jwtServiceMock.Object);
        var command = new RegisterCommand(
            TenantName: "Acme Corp",
            Subdomain: "acme",
            FirstName: "John",
            LastName: "Doe",
            Email: "john@acme.com",
            Password: "Password123"
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TenantId.ShouldNotBe(Guid.Empty);
        result.UserId.ShouldNotBe(Guid.Empty);
        result.Token.ShouldBe("fake-jwt-token");
        result.RefreshToken.ShouldBe("fake-refresh-token");
    }

    [Fact]
    public async Task Handle_DuplicateSubdomain_ThrowsValidationException()
    {
        // Arrange 
        var context = CreateContext("DuplicateSubdomainTest_" + Guid.NewGuid());

        context.Tenants.Add(new BudgetFlow.Domain.Entities.Tenant
        {
            Name = "Acme Corp",
            Subdomain = "acme",
            IsActive = true,
            MaxDepartments = 2,
            MaxUsers = 5
        });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var handler = new RegisterCommandHandler(context, _jwtServiceMock.Object);
        var command = new RegisterCommand(
            TenantName: "Another Corp",
            Subdomain: "acme",
            FirstName: "Jane",
            LastName: "Doe",
            Email: "jane@another.com",
            Password: "Password123"
        );

        // Assert 
        await Should.ThrowAsync<ValidationException>(
            () => handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_SavesTenantWithFreePlanDefaults()
    {
        // Arrange
        var context = CreateContext("FreePlanTest_" + Guid.NewGuid());
        var handler = new RegisterCommandHandler(context, _jwtServiceMock.Object);
        var command = new RegisterCommand(
            TenantName: "Acme Corp",
            Subdomain: "acme",
            FirstName: "John",
            LastName: "Doe",
            Email: "john@acme.com",
            Password: "Password123"
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var tenant = await context.Tenants.FindAsync(result.TenantId);
        tenant.ShouldNotBeNull();
        tenant!.Subdomain.ShouldBe("acme");
        tenant.MaxDepartments.ShouldBe(2);
        tenant.MaxUsers.ShouldBe(5);
        tenant.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ValidCommand_SavesUserWithCorrectRole()
    {
        // Arrange
        var context = CreateContext("UserRoleTest");
        var handler = new RegisterCommandHandler(context, _jwtServiceMock.Object);
        var command = new RegisterCommand(
            TenantName: "Acme Corp",
            Subdomain: "acme",
            FirstName: "John",
            LastName: "Doe",
            Email: "john@acme.com",
            Password: "Password123"
        );

        // Act
        await handler.Handle(command, CancellationToken.None);
        context.ChangeTracker.Clear();

        // Assert
        var user = await context.Users.FirstOrDefaultAsync();
        user.ShouldNotBeNull();
        user!.Email.ShouldBe("john@acme.com");
        user.Role.ShouldBe(Domain.Enums.UserRole.TenantAdmin);
        user.PasswordHash.ShouldNotBe("Password123");
    }
}