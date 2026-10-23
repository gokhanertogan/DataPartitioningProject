# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY DataPartitioningProject.API/DataPartitioningProject.API.csproj DataPartitioningProject.API/
RUN dotnet restore DataPartitioningProject.API/DataPartitioningProject.API.csproj
COPY DataPartitioningProject.API/ DataPartitioningProject.API/
RUN dotnet publish DataPartitioningProject.API/DataPartitioningProject.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "DataPartitioningProject.API.dll"]
