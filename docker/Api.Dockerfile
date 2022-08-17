FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build

ENV PROJECT_PATH=SFA.DAS.SecureMessageService.sln
COPY ./src ./src
WORKDIR /src

RUN dotnet restore $PROJECT_PATH
RUN dotnet build $PROJECT_PATH -c Release --no-restore
RUN dotnet test -c Release --no-restore --no-build

WORKDIR /src
ENV PROJECT_PATH=SFA.DAS.SecureMessageService.Api/SFA.DAS.SecureMessageService.Api.csproj
RUN dotnet publish $PROJECT_PATH -c Release --no-build -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SFA.DAS.SecureMessageService.Api.dll"]
