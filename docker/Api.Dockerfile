FROM das-secure-message-service-build AS build
WORKDIR /src
ENV PROJECT_PATH=SFA.DAS.SecureMessageService.Api/SFA.DAS.SecureMessageService.Api.csproj
RUN dotnet publish $PROJECT_PATH -c Release --no-build -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SFA.DAS.SecureMessageService.Api.dll"]
