# Personal Assistant

## Overview

The **Personal Assistant** project is a .NET-based application that integrates **ChatGPT API** and **Twilio API** to provide task and event management, as well as access to ChatGPT through a Twilio phone number. The project serves as a hands-on learning experience for improving .NET skills while implementing various software development best practices.

## Features

- **Event & Task Management**: Users can create and manage events and tasks via a Twilio phone number.
- **ChatGPT Access**: Provides AI-powered responses from ChatGPT through Twilio SMS or calls.
- **Entity Framework & SQL Database**: Utilizes **Entity Framework** as the ORM and **Microsoft SQL Server** as the database.
- **Logging & Caching**: Implements structured logging and caching for better performance.
- **Timed Notifications**: Uses **Hangfire** for scheduling and managing background jobs.
- **Service-Oriented Architecture**: Built with a modular service structure:
  - `AssistantService` for handling core assistant functionalities.
  - `BackgroundService` for background processing tasks.
  - `CacheService` for managing caching operations.
  - `ChatGPTService` for handling AI interactions.
  - `TwilioService` for managing SMS and call communications.

## Technologies Used

- **.NET 9.0**
- **ChatGPT API**
- **Twilio API**
- **Entity Framework Core**
- **Microsoft SQL Server**
- **Hangfire** for background job scheduling
- **Logging & Caching Mechanisms**

## Required Configuration Files

This project requires the following configuration files:

### `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Initial Catalog=PersonalAssistantDatabase;User ID=sa;Password=;TrustServerCertificate=True;"
  },
  "Twilio": {
    "AccountSid": "",
    "AuthToken": ""
  },
  "OpenAI": {
    "APIKey": ""
  }
}
```

## Setup Instructions

### Prerequisites

- .NET 9.0 SDK installed
- Microsoft SQL Server
- Twilio Account & API Credentials
- OpenAI API Key
- Hangfire configured for background job management

### Installation Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/your-repo/personal-assistant.git
   cd personal-assistant
   ```
2. Configure **appsettings.json** and **appsettings.Development.json** with Twilio, OpenAI, and Hangfire settings.
3. Apply database migrations:
   ```bash
   dotnet ef database update
   ```
4. Run the application:
   ```bash
   dotnet run
   ```

## Twilio Integration

The project has a single controller that needs to be pointed to from the **Twilio API** for handling SMS and call interactions. Ensure the Twilio webhook is configured to send requests to your deployed API endpoint.



