FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5234

ENV ASPNETCORE_URLS=http://+:5234

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS build 
ARG configuration=Release
WORKDIR /src
COPY ["meter-api.csproj", "./"]
RUN dotnet restore "meter-api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "meter-api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "meter-api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "meter-api.dll"]
