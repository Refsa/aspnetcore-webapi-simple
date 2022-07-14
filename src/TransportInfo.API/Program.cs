using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using TransportInfo.Data;
using TransportInfo.Models;
using TransportInfo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services.AddDbContext<ParkingRegistryContext>(opts =>
{
    opts.UseSqlite(builder.Configuration.GetConnectionString("ParkingRegistry"));
});

builder.Services
    .AddScoped<IParkingRegistryContext, ParkingRegistryContext>()
    .AddScoped<IParkeringsRegisteretAdapter, ParkeringsRegisteretAdapter>();

builder.Services
    .AddHostedService<ParkeringsRegisteretService>();

builder.Services
    .AddHealthChecks()
    .AddSqlite(builder.Configuration.GetConnectionString("ParkingRegistry"));

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.GrafanaLoki("http://localhost:3100")
    .CreateLogger();
builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ParkingRegistryContext>();
    context.Database.EnsureCreated();
    ParkingRegistrySeed.Initialize(context);
}

app.UseSerilogRequestLogging();

app.UseHealthChecksPrometheusExporter("/metrics");

app.UseHttpsRedirection()
   .UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }