# Используем официальный образ .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы проекта
COPY *.csproj ./
RUN dotnet restore

# Копируем все файлы
COPY . ./

# Собираем приложение
RUN dotnet publish -c Release -o /app/publish

# Используем образ .NET Runtime для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Устанавливаем инструменты для миграций БД
RUN apt-get update && apt-get install -y sqlite3 libsqlite3-dev

# Копируем собранное приложение
COPY --from=build /app/publish ./

# Создаем директорию для БД
RUN mkdir -p /app/data

# Делаем app пользователем
USER app

# Открываем порт
EXPOSE 8080

# Запускаем приложение
ENTRYPOINT ["dotnet", "booksite.dll"]