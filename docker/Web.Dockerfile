FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine

ENV PROJECT_PATH=SFA.DAS.SecureMessageService.sln
COPY ./src ./src
WORKDIR /src

RUN dotnet restore $PROJECT_PATH
RUN dotnet build $PROJECT_PATH -c Release --no-restore
RUN dotnet test -c Release --no-restore --no-build

ENV PROJECT_PATH=SFA.DAS.SecureMessageService.Web/SFA.DAS.SecureMessageService.Web.csproj
RUN dotnet publish $PROJECT_PATH -c Release --no-build -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SFA.DAS.SecureMessageService.Web.dll"]
