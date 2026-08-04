# Playlist API

An ASP.NET Core Web API that allows users to create playlists and add, update, or remove songs within them. Built as a backend technical assessment.

## Overview

The API implements playlist creation, song management, and retrieval, using a layered Controller → Service → Repository architecture on top of Entity Framework Core and SQL Server. There is no authentication system — user identity is passed via route parameters (e.g. `/api/users/1/playlists`), and two demo users are seeded automatically so the API is testable out of the box.

## Features

- Create a playlist for a user
- Add a song to a playlist
- Fetch all playlists for a user (summary view)
- Fetch a single playlist with full song details
- Update or delete a playlist (deleting cascades to its songs)
- Update or delete a song
- Swagger/OpenAPI documentation
- Global exception handling with consistent error responses
- Unit tests (service layer, mocked repositories)
- Integration tests (real HTTP requests, in-memory SQLite)

## Technology Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core Web API, .NET 10 |
| ORM | Entity Framework Core 10 |
| Database | SQL Server 2022 (via Docker Compose) |
| API Docs | Swagger / OpenAPI |
| Unit Testing | xUnit, Moq |
| Integration Testing | xUnit, `WebApplicationFactory`, SQLite (in-memory) |

## Architecture

Controller → Service → Repository → Entity Framework Core → SQL Server


- **Controllers** handle HTTP concerns only — routing, model binding, status codes.
- **Services** contain business logic (e.g. verifying a parent resource exists) and map entities to DTOs.
- **Repositories** are the only layer that talks to `PlaylistDbContext`, one repository per entity.
- Services depend on repository interfaces, not concrete classes, which is what makes the unit tests possible without a real database.

## Database

**SQL Server** was chosen because the data is relational (users → playlists → songs, one-to-many throughout), foreign keys enforce referential integrity, EF Core has mature SQL Server support, and Docker Compose lets it run identically on any machine.

Full schema, ER diagram, and verification steps are in [`docs/DATABASE.md`](docs/DATABASE.md).

## API Endpoints

| Method | Route | Description |
|---|---|---|
| POST | `/api/users/{userId}/playlists` | Create a playlist for a user |
| GET | `/api/users/{userId}/playlists` | Get all playlists for a user (summary) |
| GET | `/api/playlists/{playlistId}` | Get one playlist with its songs |
| PUT | `/api/playlists/{playlistId}` | Update a playlist |
| DELETE | `/api/playlists/{playlistId}` | Delete a playlist (and its songs) |
| POST | `/api/playlists/{playlistId}/songs` | Add a song to a playlist |
| PUT | `/api/playlists/{playlistId}/songs/{songId}` | Update a song |
| DELETE | `/api/playlists/{playlistId}/songs/{songId}` | Delete a song |

> **Note on wording:** the original assignment says "update and delete events endpoints," interpreted here as update and delete endpoints for playlists and songs.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Docker Desktop with WSL 2 (Windows) or Docker Engine (macOS/Linux)
- Git

## Setup and Run

Clone the repository:

```bash
git clone [<your-repo-url>](https://github.com/Ahmed-Shoeib/PlaylistApi.git)
cd PlaylistApi
```

Start SQL Server:

```bash
docker compose up -d
docker compose ps
```

Wait until the status shows `(healthy)` before continuing.

Restore dependencies and apply migrations:

```bash
dotnet restore
dotnet ef database update --project src/PlaylistApi
```

Run the API:

```bash
dotnet run --project src/PlaylistApi
```

The console output shows the listening URL, e.g. `https://localhost:7123`.

## Swagger

With the API running, open:

https://localhost:<port>/swagger


(replace `<port>` with the port shown in your terminal). All endpoints can be tested there directly.

## Running Tests

```bash
dotnet test tests/PlaylistApi.UnitTests
dotnet test tests/PlaylistApi.IntegrationTests
dotnet test
```

## Testing Status

- **Unit tests** (Moq-mocked repositories): all passing.
- **Integration tests** (`WebApplicationFactory` + in-memory SQLite): all passing.
- **Manual verification against the real SQL Server database** (via Docker Compose and Swagger): completed. All create, get, update, delete, validation, and not-found scenarios were tested manually and worked as expected. See [`docs/MANUAL_API_TESTS.md`](docs/MANUAL_API_TESTS.md) for the full record.

## Assumptions

- No authentication/authorization; user identity is passed via route parameters.
- "Update and delete events endpoints" was interpreted as update/delete endpoints for playlists and songs.
- A song belongs to exactly one playlist.
- Integer primary keys are used; no requirement for GUIDs was identified.

## Known Limitations

- No authentication or authorization.
- No pagination on the "get user playlists" endpoint.
- Integration tests run against SQLite rather than SQL Server directly (see `docs/DATABASE.md` for the rationale).

## AI Usage

AI assistance was used during development, guided step by step and verified at each stage (build, run, manual test, automated test). Full disclosure is in [`docs/AI_USAGE.md`](docs/AI_USAGE.md); the conversation record is in [`docs/AI_CHAT_CONTEXT.md`](docs/AI_CHAT_CONTEXT.md).

## Troubleshooting

| Symptom | Fix |
|---|---|
| `docker compose up -d` fails with "port already allocated" | Something else is using port 1433. Stop it or remap the port in `docker-compose.yml`. |
| `dotnet ef database update` fails to connect | SQL Server isn't healthy yet — check `docker compose ps` and wait for `(healthy)`. |
| `Login failed for user 'sa'` | The password in `docker-compose.yml` and `appsettings.Development.json` must match exactly. |
| Swagger page doesn't load | Confirm you're running via `dotnet run` (Development environment). |
| `dotnet ef` not found | Run `dotnet tool install --global dotnet-ef`. |
