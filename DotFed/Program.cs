using DotFed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();
using var db = new Db(new DbContextOptionsBuilder<Db>().UseInMemoryDatabase("test").Options);
app.AddRoutes(db);
app.Run();