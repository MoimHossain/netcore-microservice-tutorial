## Caution: **Work in progress** - handle with care!


## Builds status
|    |Windows|
|:--:|:--:   |
|Build|[![Build status](http://petabridge-ci.cloudapp.net/app/rest/builds/buildType:AkkaNet_AkkaNetWindowsBuild/statusIcon)](http://petabridge-ci.cloudapp.net/viewType.html?buildTypeId=AkkaNet_AkkaNetWindowsBuild&guest=1)|
|Issues|[![Stories in Ready](https://badge.waffle.io/MoimHossain/netcore-microservice-tutorial.svg?label=ready&title=Ready)](http://waffle.io/MoimHossain/netcore-microservice-tutorial)|




# netcore-microservice-tutorial
This repo contains some very basic code sample that demonstrates one way of designing  micro-services, using .net core web api supported by akka.net and running on a Docker Swarm cluster, traffic controlled by an nginx proxy.

### Steps

- Docker swarm automation template (Azure RM templates)
- Azure Container registry
- Docker stack deployment
- VS Online build integration (CD)
- Akka.net Cluster 
- Sticky seed nodes on swarm masters
- CQRS with Actors
- EventStore (GetEventStore on .NET)
- .NET Core web API
- NGINX proxies (HA and Reverse proxies) to web APIs
- ReactJS Front End application

### Purpose
This demo is prepared for a personal talk. But my intention is to take it to a good shape overtime that can illustrate/visualize micro-service architectures running on container clusters. Any remarks..? Please reach out! Thanks.