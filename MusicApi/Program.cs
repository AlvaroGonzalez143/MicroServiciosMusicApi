using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourProject.Endpoints;
var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("DefaultConnection");

var serverVersion = ServerVersion.AutoDetect(connection);
builder.Services.AddAntiforgery();
builder.Services.AddDbContext<SongBD>(options =>
{
    options.UseMySql(connection, serverVersion, options => options.EnableRetryOnFailure()).EnableSensitiveDataLogging();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//Descarga del archivo
app.Crear();
app.Consultar();
app.Descargar();



app.UseHttpsRedirection();
app.Run();
