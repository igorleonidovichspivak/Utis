INSERT INTO public."__EFMigrationsHistory" (migration_id,product_version) 
SELECT '20251010173419_InitialCreate', '8.0.11'
WHERE NOT EXISTS (SELECT 1 FROM public."__EFMigrationsHistory" WHERE migration_id = '20251010173419_InitialCreate');

INSERT INTO public."__EFMigrationsHistory" (migration_id,product_version) 
SELECT '20251012155753_FixColumnNames', '8.0.11'
WHERE NOT EXISTS (SELECT 1 FROM public."__EFMigrationsHistory" WHERE migration_id = '20251012155753_FixColumnNames');
