FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BIK.AuthService.sln", "./"]
COPY ["BIK.AuthService.API/BIK.AuthService.API.csproj", "BIK.AuthService.API/"]
COPY ["BIK.AuthService.Application/BIK.AuthService.Application.csproj", "BIK.AuthService.Application/"]
COPY ["BIK.AuthService.Domain/BIK.AuthService.Domain.csproj", "BIK.AuthService.Domain/"]
COPY ["BIK.AuthService.Infrastructure/BIK.AuthService.Infrastructure.csproj", "BIK.AuthService.Infrastructure/"]
RUN dotnet restore

COPY . .
WORKDIR "/src/BIK.AuthService.API"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "BIK.AuthService.API.dll"]