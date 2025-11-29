# Firmeza Project

Firmeza is a sales management system built with a .NET 8 Web API backend and a Vue 3 frontend. It features user authentication, a product catalog, a shopping cart, and automated email notifications.

## Project Structure

- **Firmeza.Api**: Backend REST API (ASP.NET Core 8).
- **Firmeza.Client**: Frontend application (Vue 3 + Vite).
- **Firmeza.Web**: Legacy web application.

## Prerequisites

- .NET SDK 8.0 or later
- Node.js 18.0 or later
- PostgreSQL Database

## Setup and Configuration

### 1. Database
Ensure you have a PostgreSQL database running. Update the connection string in the API configuration file if necessary.

### 2. Email Configuration (SMTP)
To enable email receipts, you must configure SMTP credentials in `Firmeza.Api/appsettings.json`.

Example configuration structure:
```json
"Smtp": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "Username": "your-email@gmail.com",
  "Password": "your-app-password",
  "FromName": "Firmeza Store"
}
```
Note: For Gmail, use an App Password.

## Running the Application

### Backend (API)
1. Open a terminal in the `Firmeza.Api` directory.
2. Run the following command:
   ```bash
   dotnet run
   ```
3. The API will start on `http://localhost:5081`.

### Frontend (Client)
1. Open a terminal in the `Firmeza.Client` directory.
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the development server:
   ```bash
   npm run dev
   ```
4. Access the application at `http://localhost:5173`.

## Key Features

- **Authentication**: Secure login and registration with JWT.
- **Catalog**: Product browsing with stock management.
- **Cart**: Slide-out shopping cart with real-time updates.
- **Profile**: User purchase history and account details.
- **Receipts**: Digital receipts available in the profile and sent via email.

## Troubleshooting

- **Sales not loading**: Ensure the API is running and the database connection is valid.
- **Emails not sending**: Verify the SMTP credentials in `appsettings.json`.
