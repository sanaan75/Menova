using System;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

Console.WriteLine("Init Empty Database");

const string connectionString = "Data Source=localhost;Initial Catalog=Menova-Empty;uid=sa; pwd=S@naan51!@;TrustServerCertificate=True";

var builder = new DbContextOptionsBuilder<AppDbContext>();
builder.LogTo(Console.WriteLine);
builder.UseSqlServer(connectionString);

using var db = new AppDbContext(builder.Options);
db.Database.EnsureDeleted();
db.Database.EnsureCreated();
db.Database.Migrate();

Console.WriteLine("Finished");