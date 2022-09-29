FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CKLunch/CKLunch.csproj", "CKLunch/"]
RUN dotnet restore "CKLunch/CKLunch.csproj"
COPY . .
WORKDIR "/src/CKLunch"
RUN dotnet build "CKLunch.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CKLunch.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CKLunch.dll"]
