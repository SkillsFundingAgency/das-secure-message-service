FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine AS runtime
WORKDIR /src
COPY /src/SFA.DAS.SecureMessageService.Web/publish ./
ENTRYPOINT ["dotnet", "SFA.DAS.SecureMessageService.Web.dll"]