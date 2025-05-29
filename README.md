
# 🏥 Clinic Management Backend Web API

This is a **.NET 9 Web API** for managing a clinic's appointments and users, built with **RESTful endpoints** and **JWT authentication**.

---

## 🚀 Features

### 🔹 Slots Module
- **GET /Slots/doctors/{docId}/slots**  
  Get available appointment slots for a specific doctor.

- **POST /Slots/book**  
  Book an appointment slot.

- **DELETE /Slots/{id}**  
  Delete an appointment slot by ID.

- **GET /Slots/{id}/slotinfo**  
  Get detailed information about a specific slot.

---

### 🔹 Users Module
- **GET /user**  
  Retrieve a list of all users.

- **GET /user/doctors/{id}**  
  Get information about a specific doctor.

- **POST /users/register**  
  Register a new user (patient or doctor).

- **POST /users/login**  
  Authenticate and obtain a **JWT token** for secure access.

---

## 🔐 Authentication
- The API uses **JWT (JSON Web Token)** for secure user authentication.
- After login, use the token in the `Authorization` header as `Bearer {token}`.

---

## 🛠️ Technologies
- **.NET 9**
- **ASP.NET Core Web API**
- **Entity Framework Core** (with SQLite or any relational DB)
- **JWT Authentication**
- **Docker-ready** (for deployment)

---

## 🐳 Docker Support
- A `Dockerfile` is included to containerize the API.
- To build and run:
  ```bash
  docker build -t clinic-api .
  docker run -p 5000:5000 clinic-api


* Update your connection strings if using SQLite with Docker volume.

---

## 📂 Project Structure

```
ClinicAPI/
├── Controllers/
│   ├── SlotsController.cs
│   └── UsersController.cs
├── Authorization/
├── Dtos/
|   ├── Users
|   └── FilterParams
├── Entities
├── Services
├── Helpers
├── Program.cs
├── Startup.cs
├── appsettings.json
├── Dockerfile
└── README.md
```

---

## 🔗 Endpoints Overview

| Module | Endpoint                     | Method |
| ------ | ---------------------------- | ------ |
| Slots  | /Slots/doctors/{docId}/slots | GET    |
|        | /Slots/book                  | POST   |
|        | /Slots/{id}                  | DELETE |
|        | /Slots/{id}/slotinfo         | GET    |
| Users  | /user                        | GET    |
|        | /user/doctors/{id}           | GET    |
|        | /users/register              | POST   |
|        | /users/login                 | POST   |

---

## 📝 Setup Instructions

1. **Clone the repo**

   ```bash
   git clone https://github.com/yourusername/clinic-api.git
   cd clinic-api
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Run the app**

   ```bash
   dotnet run
   ```

4. **Use Postman or Swagger** to test the API endpoints.

---

## 📌 Notes

* This project is a work in progress; feel free to contribute!
* Supports **Docker** for easy deployment and **SQLite** for lightweight DB.

---

## 📢 License

This project is licensed under the MIT License.

---

Enjoy managing your clinic with this lightweight API! 😊

