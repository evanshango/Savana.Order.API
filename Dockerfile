FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Savana.Order.API.csproj", "Savana.Order.API/"]
RUN dotnet restore "Savana.Order.API/Savana.Order.API.csproj"
WORKDIR "/src/Savana.Order.API"
COPY . .
RUN dotnet build "Savana.Order.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Savana.Order.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Savana.Order.API.dll"]
