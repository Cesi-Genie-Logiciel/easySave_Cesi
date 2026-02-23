using LogCentralizer.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var logDirectory = builder.Configuration.GetValue<string>("LogDirectory") ?? "/app/logs";
builder.Services.AddSingleton<ILogRepository>(new FileLogRepository(logDirectory));

var app = builder.Build();

app.MapControllers();

app.Run();
