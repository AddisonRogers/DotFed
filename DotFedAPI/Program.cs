var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
using var db = new Db(new DbContextOptionsBuilder<Db>().UseInMemoryDatabase("test").Options);
app.AddRoutes(db);

app.Run();