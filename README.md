# Clean Architecture
 
A starter template for **ASP.Net API (5.0)**, In this project, you will see loosely-coupled, dependency-inverted architecture based on [Mircosoft .Net 5.0](https://dotnet.microsoft.com/download/dotnet/5.0).
 
 
## Getting Started
To use this project, please follow these steps:
 
 - Install [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/) and [SQL Server Express](https://www.microsoft.com/en-gb/sql-server/sql-server-downloads)
 - Clone repository.
 - Check connection string in the `appsettings.json` file to make sure it's correct.
 - Build the solution and then run **Update-Database** in the **Package Manager Colsole**.
![Package Manager Console](https://user-images.githubusercontent.com/1321544/134480682-3c99363b-ff8d-4cd7-ae4e-3292e7b47aee.jpg)
 - Set **Ca.WebApi** as a start up project and then run the project.
![Clean Architecture](https://user-images.githubusercontent.com/1321544/134480311-10537133-9f75-46c3-8dbb-21ad9aca45c9.jpg)
 
### Using Serilog & Seq
This project is using [Serilog](https://serilog.net/) and [Seq](https://datalust.co/seq) for a more user-friendly UI. Please download [latest version](https://datalust.co/download) based on your prefered platform.
 
### Using Docker
To use [Docker](https://www.docker.com/) for this project, please first open the `docker-compose.yml` file and the change `ConnectionStrings__Default`, `SEQ_URL` base on your prefered configuration and then just run `Create-Docker.ps1` in the **PowerShell** console.

#### Technologies and libraries

This project has used these technologies and libraries:

 - .NET 5
 - TDD, SOLID, Dependancy Injection, Repository.
 - EF Core, EF Migration, AutoMapper, FluentValidation.
 - NUnit, xUnit, GenFu.
 - Serilog, Seq, Swagger.
 
 
