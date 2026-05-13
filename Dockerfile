# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["BudgetFlow.API/BudgetFlow.API.csproj", "BudgetFlow.API/"] 
COPY ["BudgetFlow.Application/BudgetFlow.Application.csproj", "BudgetFlow.Application/"] 
COPY ["BudgetFlow.Infrastructure/BudgetFlow.Infrastructure.csproj", "BudgetFlow.Infrastructure/"]
COPY ["BudgetFlow.Domain/BudgetFlow.Domain.csproj", "BudgetFlow.Domain/"]

RUN dotnet restore "BudgetFlow.API/BudgetFlow.API.csproj"

COPY . .

WORKDIR "/src/BudgetFlow.API"
RUN dotnet build "BudgetFlow.API.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "BudgetFlow.API.csproj" -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BudgetFlow.API.dll"]