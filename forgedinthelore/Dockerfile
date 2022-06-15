FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["forgedinthelore-auth-api.csproj", "forgedinthelore/"]
WORKDIR "/src/forgedinthelore/"
RUN dotnet restore "forgedinthelore-auth-api.csproj"
COPY . .
RUN dotnet build "forgedinthelore-auth-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "forgedinthelore-auth-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "forgedinthelore-auth-api.dll"]
