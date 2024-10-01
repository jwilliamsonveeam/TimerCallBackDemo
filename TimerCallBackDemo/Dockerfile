#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# These ARGs allow for swapping out the base used to make the final image when debugging from VS
ARG LAUNCHING_FROM_VS
# This sets the base image for final, but only if LAUNCHING_FROM_VS has been defined
ARG FINAL_BASE_IMAGE=${LAUNCHING_FROM_VS:+aotdebug}

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TimerCallBackDemo/TimerCallBackDemo.csproj", "TimerCallBackDemo/"]
RUN apt-get update && apt-get install -y --no-install-recommends gdb strace procps
RUN dotnet restore "./TimerCallBackDemo/TimerCallBackDemo.csproj"
COPY . .
WORKDIR "/src/TimerCallBackDemo"
RUN dotnet build "./TimerCallBackDemo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TimerCallBackDemo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used as the base for the final stage when launching from VS to support debugging in regular mode (Default when not using the Debug configuration)
FROM build as aotdebug
USER root
# Install GDB to support native debugging
RUN apt-get update && apt-get install -y --no-install-recommends gdb

FROM ${FINAL_BASE_IMAGE:-mcr.microsoft.com/dotnet/runtime-deps:8.0} AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimerCallBackDemo.dll"]