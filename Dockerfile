FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы проекта
COPY booksite.csproj ./
RUN dotnet restore booksite.csproj

# Копируем все файлы
COPY . ./

# Собираем приложение
RUN dotnet publish booksite.csproj -c Release -o /app/publish

# Используем образ .NET Runtime для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Устанавливаем SQLite (опционально, но рекомендуется)
RUN apt-get update && apt-get install -y sqlite3 libsqlite3-dev

# Копируем собранное приложение
COPY --from=build /app/publish ./

# Открываем порт
EXPOSE 8080

# Запускаем приложение (без создания отдельного пользователя)
ENTRYPOINT ["dotnet", "booksite.dll"]