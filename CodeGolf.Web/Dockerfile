FROM mcr.microsoft.com/dotnet/core/sdk:3.0.101-buster AS build-env
WORKDIR /src
# Copy csproj and restore as distinct layers
COPY ["CodeGolf.Web/CodeGolf.Web.csproj", "CodeGolf.Web/"]
COPY ["CodeGolf.Persistence/CodeGolf.Persistence.csproj", "CodeGolf.Persistence/"]
COPY ["CodeGolf.Domain/CodeGolf.Domain.csproj", "CodeGolf.Domain/"]
COPY ["CodeGolf.Persistence.Static/CodeGolf.Persistence.Static.csproj", "CodeGolf.Persistence.Static/"]
COPY ["CodeGolf.Service/CodeGolf.Service.csproj", "CodeGolf.Service/"]
COPY ["CodeGolf.ServiceInterfaces/CodeGolf.ServiceInterfaces.csproj", "CodeGolf.ServiceInterfaces/"]
COPY ["CodeGolf.Recapcha/CodeGolf.Recaptcha.csproj", "CodeGolf.Recapcha/"]
RUN dotnet restore "CodeGolf.Web/CodeGolf.Web.csproj"
# Setup NodeJs
RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_10.x | bash - && \
    apt-get install -y build-essential nodejs
RUN npm install -g yarn
# Copy everything else and build
COPY . ./
WORKDIR "/src/CodeGolf.Web"
RUN dotnet publish "CodeGolf.Web.csproj" -c Release -o /app/publish
# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeGolf.Web.dll"]