FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY *.slnx ./
COPY src/AuctionSystem.Core/*.csproj src/AuctionSystem.Core/
COPY src/AuctionSystem.Application/*.csproj src/AuctionSystem.Application/
COPY src/AuctionSystem.Infrastructure/*.csproj src/AuctionSystem.Infrastructure/
COPY src/AuctionSystem.Api/*.csproj src/AuctionSystem.Api/
RUN dotnet restore src/AuctionSystem.Api/AuctionSystem.Api.csproj

COPY src/ src/
RUN dotnet publish src/AuctionSystem.Api/AuctionSystem.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "AuctionSystem.Api.dll"]
