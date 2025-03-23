📝 Notes App - Full Stack Project
A simple, full-stack Notes App that allows users to register, log in, and manage their personal notes.

🚀 Tech Stack
Frontend: Vue 3, TypeScript, TailwindCSS, Vite

Backend: ASP.NET Core Web API (.NET 9), Dapper ORM, JWT Authentication

Database: SQL Server

✅ Prerequisites
Backend:
.NET SDK 9.0 (or latest stable version)

Frontend:
Node.js 18+ and npm

Database:
SQL Server 
SQL Server Management Studio (SSMS) 

⚙️ How to Run Locally

1. Clone the repo
   
git clone https://github.com/odomdara168/my-notes-apps.git

2. Setup SQL Server Database

  -Set up SQL Server
   
  -Open SSMS
  
  -Run the scripts in /backend/Database/:init.sql (Creates DB & tables)

3. Backend API
 - cd my-notes-apps\NotesApp-Backend\NotesAPI
 - (**appsettings.json*- **Pls Change Connection String in appsettings.json according to your SQL server setup**)
 -dotnet restore
 -dotnet build
 -dotnet run


4. Frontend App
-cd my-notes-apps\notes-app-frontend
-npm install
-npm run dev


🔒 Authentication
Passwords hashed with BCrypt.
JWT tokens for secure API access.

Author: Odom DARA
