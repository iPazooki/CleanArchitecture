FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Libraries/Ca.Core/Ca.Core.csproj", "Libraries/Ca.Core/"]
COPY ["Libraries/Ca.Data/Ca.Data.csproj", "Libraries/Ca.Data/"]
COPY ["Libraries/Ca.Services/Ca.Services.csproj", "Libraries/Ca.Services/"]
COPY ["Libraries/Ca.SharedKernel/Ca.SharedKernel.csproj", "Libraries/Ca.SharedKernel/"]
COPY ["Presentation/Ca.WebApi/Ca.WebApi.csproj", "Presentation/Ca.WebApi/"]
RUN dotnet restore "./Presentation/Ca.WebApi/Ca.WebApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./Presentation/Ca.WebApi/Ca.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Presentation/Ca.WebApi/Ca.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ca.WebApi.dll"]