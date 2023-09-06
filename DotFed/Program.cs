using DotFed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();
<<<<<<< HEAD
=======
//var db = new Db(new DbContextOptionsBuilder<Db>().UseInMemoryDatabase("DotFed").Options);

>>>>>>> 170192b3902b2a72fe7a53763c367d0760cdbf70
app.AddRoutes();
app.Run();