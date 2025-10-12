#!/bin/bash

set -e

echo "Waiting for database..."

# Простая проверка доступности базы (адаптируйте под вашу БД)
timeout 30s bash -c 'until nc -z $DB_HOST 5432; do sleep 1; done'

echo "Database is ready. Applying migrations..."

# Применяем миграции
dotnet ef database update --project Utis.Tasks.Infrastructure --connection "$ConnectionStrings__DefaultConnection"

echo "Migrations applied successfully. Starting application..."

# Запускаем приложение
exec dotnet Utis.Tasks.WebApi.dll