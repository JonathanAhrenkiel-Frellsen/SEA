# How to Start the Project

This section provides a step-by-step guide to setting up and running the project, including initializing the database and starting both the frontend and backend services using Docker.

## Docker Setup

The project uses Docker to simplify setup and ensure consistency across development and production environments. Docker containers are used for the backend (.NET), frontend (React), and the PostgreSQL database.

1. **Install Docker and Docker Compose**  
   Ensure you have Docker and Docker Compose installed on your system. You can download them from [https://docs.docker.com/get-docker/](https://docs.docker.com/get-docker/).

2. **Clone the Repository**  
   Clone the project repository to your local machine:
   ```bash
   git clone https://github.com/JonathanAhrenkiel-Frellsen/SEA
   cd SEA
   ```

# Database Initialization and Migration
The backend uses PostgreSQL as its database. Database migrations are handled via the .NET backend.

1. **Start the Database Container**<br>
If using Docker Compose, the database container will be started automatically. Otherwise, you can start it manually: 
```bash
docker-compose -f .\docker-compose.db.yml up -d
```
2. **Run Database Migrations** <br>
   After the database is running, apply the initial schema and any migrations. From the backend directory, run from the root of the SEA project:
```bash
cd .\backend\Survey.Infrastructure\
dotnet ef database update
```

# Running the Application (Frontend and Backend)
1. **Start All Services with Docker Compose** <br>
The recommended way to run the entire stack is with Docker Compose (remember to run this in the root of the SEA folder):
```bash
docker-compose up --build
```
This will build and start the backend, frontend, and database containers.

2. **Accessing the Application** <br>
Once all containers are running:
* The frontend (React app) is typically available at http://localhost:3000
* The backend API (.NET) is typically available at http://localhost:5000 or another configured port
* The database (Postgres) is available on the configured port (default: 5432)

3. **Stopping the Application** <br>
To stop all running containers:
```bash
docker-compose down
```
