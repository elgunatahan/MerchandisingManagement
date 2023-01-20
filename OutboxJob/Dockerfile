#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 82
EXPOSE 444

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OutboxJob/OutboxJob.csproj", "OutboxJob/"]
RUN dotnet restore "OutboxJob/OutboxJob.csproj"
COPY . .
WORKDIR "/src/OutboxJob"
RUN dotnet build "OutboxJob.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OutboxJob.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OutboxJob.dll"]