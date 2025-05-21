# OrderingApp
This is an order management system built with asp.net 8.

The api contains 4 different endpoints:
1. POST "api.order" to create a new draft order
2. GET "api.order/{orderId}" to get an existing order
3. PUT "api.order" to transition an order state (You can only confirm or cancel a draft order).
4. GET "api/analytics" to view order analytics (with response caching for 5mn)


 # How to run
1. If you're using Visual studio the app should run without issues (it'll use sqlserver localdb as DB).
2. if not using VS, you can run an instance of sqlserver, then update the connection string in appsettings.development.json.
3. You need Docker to run integration tests.

# Other things
For simplicity few customer entities were seeded. There are 3 with ids 1,2 and 3.
You can update the seeded data in the **ApplicationDbContext.OnModelCreating** method.

# make it better
1. Add docker-compose support
2. Improve logging (use Seq)
3. Better project structure (separate domain from Application logic)
4. use MediatR to better enhance business logic isolation, and simplify the service classes logic by introducing the request pattern.
