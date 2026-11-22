# Inventory & Order Management Database

A SQL Server portfolio project demonstrating relational database design and reporting for products, customers, inventory and orders.

## Features

- Normalized tables with primary and foreign keys.
- Constraints to protect data integrity.
- Order header and order-line modelling.
- Inventory tracking and low-stock reporting.
- Stored procedures and views for reusable data access.
- Analytical queries using JOIN, GROUP BY, subqueries and aggregate functions.

## Schema

```text
Customers 1 ---- * Orders 1 ---- * OrderItems * ---- 1 Products
                                      |
                                      |
                                  Products
```

The starter schema and example queries are in [schema.sql](./sql/schema.sql) and [reports.sql](./sql/reports.sql).

## Run

1. Open SQL Server Management Studio.
2. Connect to a local development SQL Server instance.
3. Run `sql/schema.sql`.
4. Run `sql/reports.sql` to explore sample reporting queries.

The scripts create a dedicated `InventoryOrderDemo` database. Review the script before running it on any shared SQL Server.

## Possible extensions

- Add inventory transaction history instead of storing only an on-hand quantity.
- Add row-version concurrency handling for stock updates.
- Add indexes based on actual query plans.
- Add automated database tests and a .NET API.
