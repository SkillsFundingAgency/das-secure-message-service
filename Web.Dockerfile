FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine AS runtime
WORKDIR /app
COPY /publish/SFA.DAS.SecureMessageService.Web ./
ENTRYPOINT ["dotnet", "SFA.DAS.SecureMessageService.Web.dll"]
