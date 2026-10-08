# AUTODOK — Car Rental Management System

This is a C# ASP.NET Core Razor Pages project for a school presentation. The customer catalog follows the supplied eight-car reference image. Admin login, dashboard, vehicles, booking, reservations, and payments follow the six supplied UI screenshots with their colors changed to charcoal and orange. **There is no SQL, database, Entity Framework, API, or internet dependency.** C# `List<T>` collections hold data only while the app is running.

## Open in your IDE

1. Install the **.NET 10 SDK** on a Windows computer. Use a Visual Studio version that supports .NET 10, or VS Code with C# Dev Kit.
2. Open the `Cars` folder or `AUTODOK.csproj`.
3. In a terminal inside the `Cars` folder, run `dotnet run`.
4. Open the localhost address printed by the terminal. The included run profile uses `http://localhost:5214` for the classroom demo.
5. Browse the eight cars on the customer home page. Click **Admin** and log in with `admin` / `admin123`.

No NuGet packages need to be added to this project. The eight-car photo sheet, login photo, and supplied AUTODOK logo are bundled in `wwwroot/images` so the website works without internet access. Admin can upload a PNG, JPEG, or WebP vehicle photo up to 2 MB; it stays in memory and resets with the car data when the app restarts.

## File structure

```text
Cars/
├── AUTODOK.csproj
├── Program.cs
├── Models/
│   ├── Car.cs
│   ├── Customer.cs
│   ├── Rental.cs
│   └── Payment.cs
├── Data/
│   └── DataStore.cs
├── Services/
│   └── VehiclePhoto.cs              validates vehicle uploads
├── Pages/
│   ├── Index.cshtml(.cs)             catalog
│   ├── Cars/Details.cshtml(.cs)      car details
│   ├── Bookings/Create.cshtml(.cs)   booking + live total
│   ├── Bookings/Confirmation.cshtml(.cs)
│   ├── Bookings/MyRentals.cshtml(.cs)
│   ├── Admin/Login.cshtml(.cs)
│   ├── Admin/Logout.cshtml(.cs)
│   ├── Admin/Index.cshtml(.cs)       dashboard
│   ├── Admin/Bookings.cshtml(.cs)    five-step booking
│   ├── Admin/Cars.cshtml(.cs)        vehicle CRUD
│   ├── Admin/Customers.cshtml(.cs)   customer CRUD
│   ├── Admin/Reservations.cshtml(.cs) return + history
│   ├── Admin/Payments.cshtml(.cs)    payment records
│   ├── Admin/Receipt.cshtml(.cs)     print / save receipt
│   └── Shared/                     site and admin layouts
└── wwwroot/
    ├── css/site.css, admin.css
    ├── js/site.js
    └── images/car-sheet.png, login-fleet.png, autodok-logo.png
```

## How the data flows

`Program.cs` calls `DataStore.Seed()` at startup. The customer pages and admin pages call `DataStore` methods to read or change the same in-memory lists. Booking checks availability, calculates `(return date − pickup date) × price per day + optional ₱500 insurance`, creates a `Rental` and a pending `Payment`, and changes the car to **Rented**. Admin can mark a payment paid or refunded as a demo record; no money moves. Returning changes the rental to **Completed**, records the actual return time, and changes the car to **Available**. Dashboard revenue sums completed rentals only.

## Five-hour checklist

| Time | Check |
|---|---|
| 0:00–0:30 | Open the folder, run `dotnet run`, and visit the catalog |
| 0:30–1:15 | Review `Car`, `Customer`, `Rental`, and `DataStore` |
| 1:15–2:00 | Test catalog cards, search, category filter, and details |
| 2:00–2:50 | Test dates, live total, insurance, booking, and confirmation |
| 2:50–3:40 | Test admin login, vehicle table, photo upload, add/edit drawer, customer CRUD, and search |
| 3:40–4:20 | Test admin booking steps, reservation detail, return, payment status, and revenue |
| 4:20–5:00 | Rehearse the five-minute presentation on the actual computer |

## Five-minute presentation script

**0:00–0:40 — Intro.** “AUTODOK is a car rental system built in C# with ASP.NET Core Razor Pages. It keeps data in classes and in-memory lists, with no database.”

**0:40–1:30 — Customer catalog.** Show the dark/orange eight-car grid, search/filter, and a details page. “Every View Details button opens the corresponding vehicle.”

**1:30–2:30 — Booking.** Select an available car. Enter customer details, pickup and return dates, locations, optional insurance, and payment choice. Change dates and show the live total. Confirm the booking. “The server recalculates the total and checks availability before saving it.”

**2:30–3:20 — Admin CRUD.** Log in through the split-screen page. Open Vehicles and Customers. Add a vehicle with a photo, edit it, search for it, and explain deletion is prevented when it would break rental history.

**3:20–4:15 — Return and payment.** Open Reservations. Show the active booking and return it. Visit the catalog: the car is available again. Open Payments and mark the pending payment paid. Show the completed reservation filter and dashboard revenue.

**4:15–5:00 — Concepts.** “Models show classes, properties, and objects. DataStore demonstrates lists, methods, conditions, loops, LINQ, calculations, and exception handling. Razor Page handlers process form events and validate uploaded images.”

## Classroom limits

- Data resets when the app restarts.
- Admin credentials are hardcoded for demonstration, not production security.
- Payment method is recorded only; no money is charged.
- The bundled car photos and login photo are AI-generated illustrative images inspired by the supplied screenshots. They are not verified photos of exact car models.
- A car becomes unavailable when booked, even when the pickup date is in the future. This keeps availability simple for the classroom demo.
