# Sport Reservation System

A web-based system built with ASP.NET Core MVC to manage users, sport spaces, and reservations for a sports complex.

---

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- MySQL server running locally
- `dotnet-ef` tool installed:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## How to run the project

**1. Clone the repository**
```bash
git clone https://github.com/your-username/SportReservation.git
cd SportReservation
```

**2. Set your database password**

Open `appsettings.json` and update the connection string:
```json
"ConnectionStrings": {
  "Default": "server=localhost;database=sportreservation;user=root;password=YOUR_PASSWORD_HERE"
}
```

**3. Restore packages**
```bash
dotnet restore
```

**4. Create the database tables**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**5. Run the project**
```bash
dotnet run
```

Open your browser at: `http://localhost:5000`

---

## Project structure

```
SportReservation/
├── Controllers/
│   ├── UserController.cs         → Manage users (list, create, edit)
│   ├── SportSpaceController.cs   → Manage sport spaces
│   └── ReservationController.cs  → Manage reservations
├── Data/
│   └── MysqlDbContext.cs         → Database configuration
├── Models/
│   ├── User.cs                   → User data (name, document, phone, email)
│   ├── SportSpace.cs             → Sport space (name, type, capacity)
│   ├── Reservation.cs            → Reservation (user, space, date, time, status)
│   └── ReservationStatus.cs      → Enum: Active, Cancelled, Finished
├── Responses/
│   └── ServiceResponse.cs        → Shared response wrapper
├── Services/
│   ├── UserService.cs            → User business logic
│   ├── SportSpaceService.cs      → Sport space business logic
│   └── ReservationService.cs     → Reservation business logic + validations
├── Views/
│   ├── User/
│   ├── SportSpace/
│   └── Reservation/
├── Program.cs
└── appsettings.json
```

---

## Main features

### Users
- Register a new user (name, document ID, phone, email)
- Edit user information
- Validate that no two users share the same document or email
- List all registered users

### Sport Spaces
- Register a sport space (name, type, capacity)
- Edit space information
- Validate no duplicate spaces
- List and filter spaces by type

### Reservations
- Create a reservation (user + space + date + start time + end time)
- Validate there are no overlapping reservations for the same space
- Validate a user does not have two reservations at the same time
- Validate end time is after start time
- Validate reservation is not in the past
- Cancel a reservation (status → Cancelled)
- List reservations by user or by space

---

## Business rules

- A user cannot have two active reservations in the same time range
- A sport space cannot have two reservations in overlapping time ranges
- End time must be greater than start time
- Reservations cannot be created for past dates or times
- Reservation status: **Active → Cancelled** or **Active → Finished**

---

## Diagrams

See the `/docs` folder for:
- `class-diagram.png` — class diagram
- `use-case-diagram.png` — use case diagram
