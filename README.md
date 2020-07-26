# HOW TO RUN

* RUN `.\DockerCompose.ps1`
* Browse to http://localhost:5189/weatherforecast
*Watch the console, messages will be send and received. At this points its easy to add more services to the messagebus and let them chat*

## Points of interest

* [Rabbitmq folder](https://github.com/PBeckerr/DotnetDockerComposeRabbitMq/tree/master/API1/RabbitMq) - For a baseline how its working
  *  `WeatherForecastReceiveService : BasicReceiveService<WeatherForecast>` Receives messages of type `WeatherForecast`
  *  `SendSendMessageService` Sends messages to the queue
* [docker-compose.yml](https://github.com/PBeckerr/DotnetDockerComposeRabbitMq/blob/master/docker-compose.yml) - luckily thanks to docker magic its easy to setup rabbitmq

## Disclaimer
Rabbitmq is a powerful tool and this demo only demonstrates simple sending and receiving of messages from the bus and hopefully showcases how easy it is to add a messagequeue to your api or any other executable for that matter.
