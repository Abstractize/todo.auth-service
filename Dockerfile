# Stage 1: Base runtime image (minimal & secure)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app

ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

# Labels for image metadata (useful for CI/CD and Helm)
ARG VERSION
LABEL org.opencontainers.image.source="https://github.com/Abstractize/todo.auth-service"
LABEL org.opencontainers.image.version=$VERSION

# Stage 2: Build and publish
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG GITHUB_USERNAME
WORKDIR /src

COPY src .

WORKDIR /src/API
RUN --mount=type=secret,id=GITHUB_TOKEN bash -c '\
    TOKEN=$(cat /run/secrets/GITHUB_TOKEN) && \
    echo "<?xml version=\"1.0\" encoding=\"utf-8\"?>" > nuget.config && \
    echo "<configuration>" >> nuget.config && \
    echo "  <packageSources>" >> nuget.config && \
    echo "    <add key=\"nuget.org\" value=\"https://api.nuget.org/v3/index.json\" />" >> nuget.config && \
    echo "    <add key=\"github\" value=\"https://nuget.pkg.github.com/'$GITHUB_USERNAME'/index.json\" />" >> nuget.config && \
    echo "  </packageSources>" >> nuget.config && \
    echo "  <packageSourceCredentials>" >> nuget.config && \
    echo "    <github>" >> nuget.config && \
    echo "      <add key=\"Username\" value=\"'$GITHUB_USERNAME'\" />" >> nuget.config && \
    echo "      <add key=\"ClearTextPassword\" value=\"$TOKEN\" />" >> nuget.config && \
    echo "    </github>" >> nuget.config && \
    echo "  </packageSourceCredentials>" >> nuget.config && \
    echo "</configuration>" >> nuget.config \
    '

RUN dotnet restore --configfile /src/API/nuget.config
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Final runtime image
FROM base AS final
WORKDIR /app

RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "API.dll"]