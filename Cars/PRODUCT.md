# AUTODOK product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Stack

C# with ASP.NET Core Razor Pages on .NET 10. The user selected Razor Pages over the earlier Windows Forms version.

## Users

Customers browse the available fleet, inspect vehicle details, and book a rental. An administrator manages cars, customers, and returns for a school project demonstration.

## Product Purpose

Demonstrate a working car rental transaction from vehicle browsing through price calculation, booking, return, and completed revenue, while making C# classes, collections, and event-driven web interactions visible to the presenters.

## Operating Context

Three students need a presentation-ready project with about five hours to prepare. It runs locally in an IDE and needs no external service during use.

## Capabilities and Constraints

- No SQL, database, ORM, API, or internet requirement. Data stays in C# `List<T>` collections for the current application run.
- Customer catalog, details, booking, confirmation, and a rental lookup by phone.
- Admin login; car and customer CRUD; bookings; reservations with return and cancellation; in-memory payment records; and dashboard statistics.
- Vehicle add/edit forms accept an uploaded PNG, JPEG, or WebP photo up to 2 MB. Photos are stored with the in-memory car records.
- Payment method is a recorded demo choice. No payment processing is performed.
- Hardcoded classroom admin credentials are `admin` / `admin123`.
- Vehicle names and illustrative Philippine peso prices follow the supplied screenshot.

## Brand Commitments

The user supplied a screenshot of a dark automotive catalog with orange accents, four columns of car photo cards on desktop, short descriptions, peso-per-day prices, and orange View Details buttons. The project is named AUTODOK and uses the supplied AUTODOK logo.

## Evidence on Hand

Reference images: `cars.jpg`, `login ui cars.jpg`, `payments.jpg`, `reservation.jpg`, `vehicle.jpg`, `booking.jpg`, and `dashboard.jpg` in `C:\Users\Xander Calz\Downloads`. The bundled eight-car photo sheet and login fleet image are generated for this project and are illustrative, not verified photos of exact listed models.

## Product Principles

- Make the complete rental sequence easy to show live.
- Keep every visible action functional.
- Keep the code simple enough for students to explain.
- Keep the application self-contained for an offline classroom demo.
