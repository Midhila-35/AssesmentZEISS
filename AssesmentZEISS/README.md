# AssesmentZEISS

This project is a .NET 8-based application for managing products, including features like product ID generation, stock adjustment, and CRUD operations.

## Features
- Product CRUD operations.
- Unique product ID generation.
- Stock adjustment (increment/decrement).
- Unit tests for critical functionalities.

## Prerequisites
- .NET 8 SDK
- SQL Server (or any configured database)
- Visual Studio 2022 (or any IDE supporting .NET 8)

## Setup Instructions
1. **Clone the Repository**:
2. **Configure Database**:
   - Open `appsettings.json` and update the connection string:
3. **Run EF Core Migrations**:
   - Open a terminal in the project directory and run:
4. **Run the Application**:
   - Start the application:
- The API will be available at `https://localhost:5001` or `http://localhost:5000`.

git remote add origin <repository-url>
git branch -M main
git push -u origin main
git init
git add .
git commit -m "Initial commit"
## Dependencies
- **xUnit**: For unit testing.
- **Moq**: For mocking dependencies in tests.
- **Entity Framework Core**: For database operations.

## Notes
- Ensure the database server is running before starting the application.
- Use Swagger (enabled by default) to explore the API at `https://localhost:5001/swagger`.
## API Endpoints
### Product Endpoints
1. **GET** `/api/Product` - Get all products.
2. **GET** `/api/Product/{id}` - Get a product by ID.
3. **POST** `/api/Product` - Create a new product.
4. **PUT** `/api/Product/{id}` - Update a product.
5. **DELETE** `/api/Product/{id}` - Delete a product.
6. **PUT** `/api/products/decrement-stock/{id}/{quantity}` - Decrement stock.
7. **PUT** `/api/products/add-to-stock/{id}/{quantity}` - Add to stock.

## Running Locally
1. Clone the repository.
2. Configure the database in `appsettings.json`.
3. Run EF Core migrations.
4. Start the application using `dotnet run`.
5. Use tools like Postman or Swagger to test the API.

## Running Tests
1. Navigate to the test project directory.
2. Run the tests using:
5. **Run Unit Tests**:
- Navigate to the test project directory and run:
