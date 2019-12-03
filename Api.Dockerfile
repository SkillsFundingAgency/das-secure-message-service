FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine AS runtime
WORKDIR /src
COPY /src/SFA.DAS.SecureMessageService.Api/publish ./
ENTRYPOINT ["dotnet", "SFA.DAS.SecureMessageService.Api.dll"]
