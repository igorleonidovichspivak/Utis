-- Создаем базу данных если не существует
SELECT 'CREATE DATABASE maindb'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'maindb')\gexec

-- Подключаемся к новой базе
\c maindb;

-- Создаем расширения если нужны
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Создаем таблицы или выполняем другие скрипты
-- CREATE TABLE IF NOT EXISTS tasks (...);