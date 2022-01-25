FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HookHook.Backend/HookHook.Backend.csproj", "HookHook.Backend/"]
RUN dotnet restore "HookHook.Backend/HookHook.Backend.csproj"
COPY . .
WORKDIR "/src/HookHook.Backend"
RUN dotnet build "HookHook.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HookHook.Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HookHook.Backend.dll"]