FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /build
COPY /src ./src
COPY *.sln ./
RUN dotnet publish src/Distance.Service -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /build/out ./

EXPOSE 5000

ENTRYPOINT [ "dotnet", "Distance.Service.dll" ]
