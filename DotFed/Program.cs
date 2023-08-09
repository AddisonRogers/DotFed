using DotFed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();
var db = new Db(new DbContextOptionsBuilder<Db>().UseNpgsql(
    connectionString: "Server=localhost;Port=5432;Database=postgres;User Id=706b6e9e;Password=pog;"
).Options);
app.AddRoutes(db);
app.Run();