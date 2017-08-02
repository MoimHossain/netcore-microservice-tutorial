FROM microsoft/aspnetcore:1.0.1


COPY ./bin/Release/eisen-pub/ /app
WORKDIR /app

ENV ASPNETCORE_URLS http://+:5000

EXPOSE 5000
ENTRYPOINT ["dotnet", "api.dll"]
