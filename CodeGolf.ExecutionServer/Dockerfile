FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 55000

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["CodeGolf.ExecutionServer/CodeGolf.ExecutionServer.csproj", "CodeGolf.ExecutionServer/"]
COPY ["CodeGolf.ServiceInterfaces/CodeGolf.ServiceInterfaces.csproj", "CodeGolf.ServiceInterfaces/"]
RUN dotnet restore "CodeGolf.ExecutionServer/CodeGolf.ExecutionServer.csproj"
COPY . .
WORKDIR "/src/CodeGolf.ExecutionServer"
RUN dotnet build "CodeGolf.ExecutionServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeGolf.ExecutionServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeGolf.ExecutionServer.dll", "http://0.0.0.0:55000"]