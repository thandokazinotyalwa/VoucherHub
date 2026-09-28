# VoucherHub

A .NET Modular Monolith for creating, managing, and tracking grocery and essential-item vouchers.

## Overview

VoucherHub is an MVP application designed to help users create and manage vouchers for groceries and essential items.

The project is being developed as a practical software engineering project using modern .NET development practices.

## Technologies

* C#
* .NET
* ASP.NET Core
* REST APIs
* In-Memory Data Storage
* Git & GitHub

## Architecture

VoucherHub follows a **Modular Monolith** architecture with separation of responsibilities within the application.

The current voucher module is organised into:

```text
Modules/
└── Vouchers/
    ├── API/
    ├── Application/
    ├── Domain/
    └── Infrastructure/
```

### Layers

* **API** – Defines the HTTP endpoints.
* **Application** – Contains DTOs, services, and application interfaces.
* **Domain** – Contains the core voucher entities and business concepts.
* **Infrastructure** – Contains implementations such as repositories and data storage.

## Current Features

* Create vouchers
* Update vouchers
* Manage voucher status
* Retrieve voucher information
* In-memory voucher repository

## Getting Started

### Prerequisites

Make sure you have:

* .NET SDK installed
* Git installed
* A code editor such as VS Code

## Future Improvements

Planned improvements include:

* Persistent database storage
* Authentication and authorization
* Voucher redemption
* Voucher expiry
* Transaction history
* Angular frontend
* Automated testing
* Cloud deployment

## License

This project is currently for learning and development purposes.
