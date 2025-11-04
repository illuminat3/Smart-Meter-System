FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5234

ENV ASPNETCORE_URLS=http://+:5234

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build 
ARG configuration=Release
WORKDIR /src
COPY ["meter-api.csproj", "./"]
RUN dotnet restore "meter-api.csproj"
COPY . .
RUN dotnet publish "meter-api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "meter-api.dll"]
