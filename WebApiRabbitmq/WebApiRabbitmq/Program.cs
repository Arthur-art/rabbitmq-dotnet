using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();

var startInfo = new ProcessStartInfo();
startInfo.FileName = "http://localhost:5000/swagger/index.html";
startInfo.UseShellExecute = true;
Process.Start(startInfo);

app.UseAuthorization();

app.MapControllers();

app.Run();
