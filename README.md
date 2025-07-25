# TODO.AuthService

This project is the **Authentication Service** for the TODO application. It provides JWT-based authentication and refresh token support.

---

## 🧩 Tech Stack

- .NET 9
- Minimal APIs (endpoint-based routing)
- JWT Authentication
- Entity Framework Core (PostgreSQL)
- Clean Architecture (Domain, Application, Infrastructure, API layers)

---

## 📘 API Overview

All routes are prefixed by `/api/auth` and typically routed through the API Gateway.

### Auth Operations

- `POST /api/auth/login` — Login with email and password  
- `POST /api/auth/register` — Register a new user  
- `POST /api/auth/refresh-token` — Get a new access token using a refresh token  
- `POST /api/auth/logout` — Invalidate the current refresh token  

---

## 🧠 Auth Flow

1. **Login** returns an access token (JWT) and a refresh token.
2. The **access token** is used in `Authorization: Bearer <token>` headers for authenticated requests.
3. When the access token expires, the **refresh-token** endpoint can be used to get a new one.
4. On logout, the refresh token is revoked and cannot be reused.

---

## 📁 Project Structure

- `API/` – Entry point with mapped endpoints  
- `Managers/` – Auth manager implementing business logic  
- `Services/` – Token generation, hashing, and identity abstraction  
- `Data/` – EF Core entities and repositories

---

## 📄 License

MIT — see `LICENSE` file.