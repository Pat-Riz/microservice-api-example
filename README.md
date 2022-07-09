# Unnamed Project

Miniproject mostly to practice creating a microservice API.

Microservice architecture with Dotnet 6 minimal API with 2 services that can talk to each other with gRPC and via a pub/sub on a RabbitMQ bus.  
Using kubernetes to setup all neccesarry network configuration.

A "Dummy" frontend created with React CRA mostly to practice with Tailwind CSS and not using a existing component library.

Functionality wise all it does is allow a user to see and create commands for diffrent platforms (Docker, kubernetes etc).
In development it uses an in memory database otherwise it uses a MSSQL database with a persistant storage in kubernetes.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- .net 6
- yarn
- Docker
- Kubernetes

### Installing

#### Running in dev without kubernetes (except rabbitmq)

Starting backend

```
cd backend
kubectl apply -f rabbitmq-depl.yaml
dotnet run --project .\PlatformService\PlatformService.csproj
dotnet run --project .\CommandsService\CommandsService.csproj
```

Starting frontend

```
cd frontend
yarn install
yarn start
```

####

## Built With

- [React](https://reactjs.org/docs/getting-started.html)
- [ASP.Net Minimal API](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0)
- [Docker](https://www.docker.com/)
- [Kubernetes](https://kubernetes.io/docs/home/)
- [gRPC](https://grpc.io/docs/)
- [RabbitMQ](https://www.rabbitmq.com/documentation.html)

## Roadmap

- [x] Create a Readme
- [ ] Create a proper Docker container to run the frontend
- [ ] Learn and use Helm charts to organize Kubernetes for the backend
- [ ] Create a powershellscript in the root of repo to start both frontend and backend

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
