FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and restore dependencies
COPY ["Personal_Finance_Tracker.API/Personal_Finance_Tracker.API.csproj","Personal_Finance_Tracker.API/"]
COPY ["Repository/Repository.csproj","Repository/"]
COPY ["Core/Core.csproj","Core/"]
COPY ["Services/Services.csproj","Services/"]

# Restore dependencies
RUN dotnet restore "Personal_Finance_Tracker.API/Personal_Finance_Tracker.API.csproj"
COPY . ../


WORKDIR /Personal_Finance_Tracker.API
RUN pwd
RUN dotnet build "Personal_Finance_Tracker.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Personal_Finance_Tracker.API.dll"]
