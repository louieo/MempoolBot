#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MempoolBot.Web/MempoolBot.Web.csproj", "MempoolBot.Web/"]
COPY ["MempoolBot.Lib/MempoolBot.Lib.csproj", "MempoolBot.Lib/"]
RUN dotnet restore "MempoolBot.Web/MempoolBot.Web.csproj"
COPY . .
WORKDIR "/src/MempoolBot.Web"
RUN dotnet build "MempoolBot.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MempoolBot.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MempoolBot.Web.dll"]
