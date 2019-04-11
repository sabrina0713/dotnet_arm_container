FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["ManagedServiceIdentityTest.csproj", ""]
RUN dotnet restore "ManagedServiceIdentityTest.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "ManagedServiceIdentityTest.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ManagedServiceIdentityTest.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ManagedServiceIdentityTest.dll"]