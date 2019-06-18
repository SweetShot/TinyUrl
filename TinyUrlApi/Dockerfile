FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["TinyUrlApi/TinyUrlApi.csproj", "TinyUrlApi/"]
RUN dotnet restore "TinyUrlApi/TinyUrlApi.csproj"
COPY . .
WORKDIR "/src/TinyUrlApi"
RUN dotnet build "TinyUrlApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TinyUrlApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TinyUrlApi.dll"]