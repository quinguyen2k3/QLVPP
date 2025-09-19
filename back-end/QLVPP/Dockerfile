# -------- BUILD STAGE --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy file csproj và restore packages
COPY *.csproj ./
RUN dotnet restore

# copy toàn bộ source code
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# -------- RUNTIME STAGE --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# expose port cho API
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "QLVPP.dll"]
