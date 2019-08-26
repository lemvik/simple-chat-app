FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app
COPY . .
RUN chmod a+w /tmp 
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN apt install -y npm nodejs

RUN dotnet build -c Release 
RUN dotnet publish -c Release ./chat-service/chat-service.csproj -o out

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS chat-service
WORKDIR /app
COPY --from=build-env /app/chat-service/out .
ENTRYPOINT ["dotnet", "chat-service.dll"]
EXPOSE 80/tcp