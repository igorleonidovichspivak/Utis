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

    GET /api/currency

Logs stored in folder 
    \tasks-service\Utis.Tasks.WebApi\Logs\
    \processing-service\Utis.Processing.Service\Logs\

Data stores in PostgreSQL.

Initial scripts for creating db
    \init-scripts 
Volume for db 
    \postgres_data


RabbitMQ used for sending messages.

Volumes for RabbitMQ 
    \rabbitmq_data
    \rabbitmq_logs


Services can be run by  
 
    docker-compose up -d --build









