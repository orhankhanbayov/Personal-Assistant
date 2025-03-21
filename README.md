# Personal Assistant

## Overview

The **Personal Assistant** project is a .NET-based application that integrates **ChatGPT API** and **Twilio API** to provide task and event management, as well as access to ChatGPT through a Twilio phone number. The project serves as a hands-on learning experience for improving .NET skills while implementing various software development best practices.

## Features

- **Event & Task Management**: Users can create and manage events and tasks via a Twilio phone number.
- **ChatGPT Access**: Provides AI-powered responses from ChatGPT through Twilio SMS or calls.
- **Entity Framework & SQL Database**: Utilizes **Entity Framework** as the ORM and **Microsoft SQL Server** as the database.
- **Logging & Caching**: Implements structured logging and caching for better performance.

- **Timed Notifications**: Uses **Hangfire and Worker Service** for scheduling and managing background jobs.
- **Service-Oriented Architecture**: Built with a modular service structure with gRPC used for inter-service communication:
  - `AssistantService` for handling core assistant functionalities.
  - `SchedulerService` for background processing tasks.
  - `CacheService` for managing caching operations.
  - `ChatGPTService` for handling AI interactions.
  - `TwilioService` for managing SMS and call communications.
  - `DatabaseService` for handling storing and creating data. 

## Technologies Used

- **.NET 9.0**
- **ChatGPT API**
- **gRPC**
- **Twilio API**
- **Entity Framework Core**
- **Microsoft SQL Server**
- **Hangfire** 
- **Logging & Caching**





