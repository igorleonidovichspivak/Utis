# Utis

docker-compose should start application in Docker.

Contains 2 services: 
    1) task-service
    2) processing-service

Task service use RestApi to manage tasks

    POST /api/tasks - creating new task

    GET /api/tasks/{id} - get task by id

    PUT /api/tasks/{id} - full update of the task
    
    DELETE /api/tasks/{id} - delete task

    GET /api/tasks - list of task by status (with pagination)

Logs stored in folder ..\Utis.Tasks.WebApi\Logs\

Data stores in PostgreSQL.

Services can be run by  
 
    docker-compose up -d

\init-scripts should contains initial scripts for creating db







