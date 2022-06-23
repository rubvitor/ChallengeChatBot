# Description
ChatBot able to start chat with multiples users using WebSocket and RabbitMQ

#Configurations
All the available users are into the file appsettings section Configuration.UserModels (you may pick one to use in the front-end)
The project use SqlServer inMemory. All the passwords are Encrypted
The project API uses Bearer Token. It's necessary to Login using the method Login into the controller Security

Please install Docker in your machine. Some commands and link below:

docker pull rabbitmq:3-management
docker run -d -p 15672:15672 -p 5672:5672 --name rabbit-test-for-medium rabbitmq:3-management

Source: https://code.imaginesoftware.it/rabbitmq-with-docker-on-windows-in-30-minutes-172e88bb0808