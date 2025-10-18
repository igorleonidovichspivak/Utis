# Utis

docker-compose up -d --build 

    should start application in Docker.

Contains 2 services: 

    1) task-service
    
    2) processing-service
    

Task service use RestApi to manage tasks

    http://localhost:5001/swagger

    POST /api/tasks - creating new task

    GET /api/tasks/{id} - get task by id

    PUT /api/tasks/{id} - full update of the task
    
    DELETE /api/tasks/{id} - delete task

    GET /api/tasks - list of task by status (with pagination)

    GET /api/currency

Logs stored in folders

    \logs\tasks-service\
    \logs\processing-service\

Data stores in PostgreSQL

    localhost:7432/
        admin/Admin123!
        db: maindb
        table: tasks

Initial scripts for creating db

    \init-scripts 
    
Volume for db 

    \postgres_data


RabbitMQ used for sending messages.

    localhost:17672/
    guest/guest
    
Overdue tasks should be appeared in

    'expired-tasks' queue

Volumes for RabbitMQ 

    \rabbitmq_data
    \rabbitmq_logs



 
    









