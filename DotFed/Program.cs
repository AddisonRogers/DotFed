using DotFed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();
app.AddRoutes();
app.Run();