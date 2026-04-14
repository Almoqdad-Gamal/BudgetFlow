using BudgetFlow.API.Middleware;
using BudgetFlow.API.Services;
using BudgetFlow.Application;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Infrastructure.Extentions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "BudgetFlow API V1");
        options.RoutePrefix = "swagger";
    });
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();



app.Run();


