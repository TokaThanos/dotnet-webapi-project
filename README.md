### Comprehensive CI/CD Steps for a .NET Web API with SQL Server and Docker

This document outlines the step-by-step implementation of a CI/CD pipeline for a .NET Web API that uses SQL Server as its database. The pipeline uses GitHub Actions to automate the build, test, and deployment process, ensuring a streamlined workflow.

---

### **1. Prerequisites**

1. **Development Environment**:
   - .NET SDK installed (latest version recommended).
   - Docker and Docker Compose installed.

2. **Source Code**:
   - A .NET Web API project configured to use SQL Server.

3. **GitHub Repository**:
   - Code pushed to a GitHub repository.

4. **Docker Hub**:
   - Docker Hub account with a repository created to store your Docker images.

5. **Unit and Integration Tests**:
   - Write unit tests for business logic.
   - Write integration tests to ensure the API communicates correctly with SQL Server.

6. **SQL Server Setup**:
   - Use SQL Server Docker image for the database.

---

### **2. Prepare the Project for Docker**

#### **Dockerfile**
Create a `Dockerfile` for the Web API:

```dockerfile
# Use the .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy and restore dependencies
COPY *.sln .
COPY MyWebApi/*.csproj ./MyWebApi/
RUN dotnet restore

# Copy and build the app
COPY MyWebApi/. ./MyWebApi/
WORKDIR /app/MyWebApi
RUN dotnet publish -c Release -o /publish

# Use a runtime image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "MyWebApi.dll"]
```

#### **Docker Compose**
Create a `docker-compose.yml` to orchestrate the Web API and SQL Server:

```yaml
version: "3.8"

services:
  webapi:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "5000:80"
    depends_on:
      - sqlserver
    environment:
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=MyWebApiDb;User Id=sa;Password=YourPassword123;

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ports:
      - "1433:1433"
    environment:
      ACCEPT_EULA: "Y"
      SA_PASSWORD: "YourPassword123"
```

---

### **3. Configure GitHub Actions Workflow**

Create a `.github/workflows/ci-cd.yml` file in your repository:

```yaml
name: CI/CD for .NET Web API with Docker

on:
  push:
    branches:
      - main

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
      # Step 1: Checkout the repository
      - name: Checkout Code
        uses: actions/checkout@v3

      # Step 2: Set up .NET
      - name: Set up .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: "7.0"

      # Step 3: Restore dependencies
      - name: Restore Dependencies
        run: dotnet restore

      # Step 4: Build the application
      - name: Build Application
        run: dotnet build --configuration Release

      # Step 5: Run unit tests
      - name: Run Unit Tests
        run: dotnet test --no-build --configuration Release

      # Step 6: Build Docker image
      - name: Build Docker Image
        run: docker build -t mywebapi:latest .

      # Step 7: Start services with Docker Compose
      - name: Start Services with Docker Compose
        run: docker-compose up -d

      # Step 8: Test the Web API
      - name: Test API Endpoint
        run: |
          curl -f http://localhost:5000/health || exit 1

      # Step 9: Push Docker Image to Docker Hub
      - name: Push Docker Image
        env:
          DOCKER_USERNAME: ${{ secrets.DOCKER_USERNAME }}
          DOCKER_PASSWORD: ${{ secrets.DOCKER_PASSWORD }}
        run: |
          echo "${DOCKER_PASSWORD}" | docker login -u "${DOCKER_USERNAME}" --password-stdin
          docker tag mywebapi:latest dockerhubusername/mywebapi:latest
          docker push dockerhubusername/mywebapi:latest
```

---

### **4. Workflow Details**

1. **Trigger**: The workflow runs every time you push to the `main` branch.

2. **Steps**:
   - **Checkout Code**: Fetches the latest code.
   - **Setup .NET**: Configures the GitHub Actions runner to use .NET.
   - **Restore, Build, and Test**: Ensures the application builds and all tests pass.
   - **Build Docker Image**: Creates a Docker image for the Web API.
   - **Docker Compose**: Starts both the Web API and SQL Server containers.
   - **Integration Test**: Tests the API endpoint to ensure the app and database work together.
   - **Push Docker Image**: Publishes the Docker image to Docker Hub.

---

### **5. Deployment (Optional)**

To deploy the application, you can:

1. Use **Docker Compose** to run the containers on a server or cloud provider.
2. Push the Docker image to a cloud platform (e.g., AWS ECS, Azure App Services).

---

### **6. Cost Analysis**

- **GitHub Actions**: Free for 2,000 minutes/month (Linux runners).
- **Docker Hub**: Free for public repositories.
- **SQL Server**: Free Developer Edition for testing.

For deployment, hosting costs depend on the chosen platform (e.g., AWS, Azure). Free tiers are available for small-scale use.

---

### **Conclusion**
This CI/CD pipeline automates the build, test, and deployment process for your .NET Web API, ensuring seamless integration with SQL Server. The use of Docker ensures a consistent environment across development, testing, and production.

