# System Integration 

Welcome to my repository showcasing my work from the **System Integration** elective during my AP Degree in Computer Science (Datamatiker).

This repository serves as a practical exploration of how different software systems communicate, integrate, and share data using various architectural patterns.


## 🚧 Current Progress
This is a work in progress. Currently, the repository covers the first few lessons:

- [**Lesson 1: Docker & RabbitMQ Basics**](./SystemIntegrationLektion01/README.md) - *Setting up a local development environment using Docker to run a RabbitMQ instance. Includes basic C# console applications demonstrating fundamental message sending and receiving utilizing the `RabbitMQ.Client` package.*
- [**Lesson 2: Web APIs, JSON & Competing Consumers**](./SystemIntegrationLektion02/README.md) - *Building an ASP.NET Core Web API to generate and publish JSON-serialized messages to RabbitMQ. Also explores the Competing Consumers pattern to scale message processing efficiently across multiple listeners.*
- [**Lesson 3: RESTful Web APIs, CRUD & Dependency Injection**](./SystemIntegrationLektion03/README.md) - *Developed complete controller-based Web APIs, implementing full CRUD operations and data validation. Focus is placed on clean architecture by separating business logic into services using Dependency Injection, and utilizing Entity Framework Core with an In-Memory database for data persistence.*

*More lessons and assignments will be added as the course progresses.*

## 🛠️ Tech Stack & Tools
*Based on the current lessons, the projects primarily utilize:*
- **C# / .NET**
- **ASP.NET Core Web API** (RESTful services, Entity Framework Core)
- **RabbitMQ** (Message brokering & Asynchronous communication)
- **Docker** (Containerization)
- **JetBrains Rider** (Primary IDE)
