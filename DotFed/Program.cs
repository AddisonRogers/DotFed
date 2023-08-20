using DotFed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();
//var db = new Db(new DbContextOptionsBuilder<Db>().UseInMemoryDatabase("DotFed").Options);
app.UseHttpsRedirection();
app.UseStaticFiles();

app.AddRoutes();
app.Run();