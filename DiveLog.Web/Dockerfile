FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["DiveLog.Web/DiveLog.Web.csproj", "DiveLog.Web/"]
COPY ["DiveLog.Parser/DiveLog.Parser.csproj", "DiveLog.Parser/"]
COPY ["DiveLog.DTO/DiveLog.DTO.csproj", "DiveLog.DTO/"]
RUN dotnet restore "DiveLog.Web/DiveLog.Web.csproj"
COPY . .
WORKDIR "/src/DiveLog.Web"
RUN dotnet build "DiveLog.Web.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "DiveLog.Web.csproj" -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DiveLog.Web.dll"]