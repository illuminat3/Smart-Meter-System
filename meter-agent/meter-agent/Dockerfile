FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5234

ENV ASPNETCORE_URLS=http://+:5234

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build 
ARG configuration=Release
WORKDIR /src
COPY ["meter-agent.csproj", "./"]
RUN dotnet restore "meter-agent.csproj"
COPY . .
RUN dotnet publish "meter-agent.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "meter-agent.dll"]
