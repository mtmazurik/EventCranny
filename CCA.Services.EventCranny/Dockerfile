#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.2.0-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2.0-aspnetcore-runtime AS build
WORKDIR /src
COPY ["CCA.Services.EventCranny/CCA.Services.EventCranny.csproj", "CCA.Services.EventCranny/"]
COPY ["CCA.Services.EventCranny.DAL/CCA.Services.EventCranny.DAL.csproj", "CCA.Services.EventCranny.DAL/"]
RUN dotnet restore "CCA.Services.EventCranny/CCA.Services.EventCranny.csproj"
COPY . .
WORKDIR "/src/CCA.Services.EventCranny"
RUN dotnet build "CCA.Services.EventCranny.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CCA.Services.EventCranny.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "eca.services.EventCranny.dll"]
