FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["DiveLog.GUI/DiveLog.GUI.csproj", "DiveLog.GUI/"]
RUN dotnet restore "DiveLog.GUI/DiveLog.GUI.csproj"
COPY . .
WORKDIR "/src/DiveLog.GUI"
RUN dotnet build "DiveLog.GUI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DiveLog.GUI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DiveLog.GUI.dll"]