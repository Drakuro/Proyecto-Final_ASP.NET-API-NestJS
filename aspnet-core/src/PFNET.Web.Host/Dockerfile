FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/PFNET.Web.Host/PFNET.Web.Host.csproj", "src/PFNET.Web.Host/"]
COPY ["src/PFNET.Web.Core/PFNET.Web.Core.csproj", "src/PFNET.Web.Core/"]
COPY ["src/PFNET.Application/PFNET.Application.csproj", "src/PFNET.Application/"]
COPY ["src/PFNET.Core/PFNET.Core.csproj", "src/PFNET.Core/"]
COPY ["src/PFNET.EntityFrameworkCore/PFNET.EntityFrameworkCore.csproj", "src/PFNET.EntityFrameworkCore/"]
WORKDIR "/src/src/PFNET.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/PFNET.Web.Host", "src/PFNET.Web.Host"]
COPY ["src/PFNET.Web.Core", "src/PFNET.Web.Core"]
COPY ["src/PFNET.Application", "src/PFNET.Application"]
COPY ["src/PFNET.Core", "src/PFNET.Core"]
COPY ["src/PFNET.EntityFrameworkCore", "src/PFNET.EntityFrameworkCore"]
WORKDIR "/src/src/PFNET.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "PFNET.Web.Host.dll"]
