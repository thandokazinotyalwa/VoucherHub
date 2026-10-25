using VoucherHub.Modules.Vouchers.Application.Interfaces;
using VoucherHub.Modules.Vouchers.Application.Services;
using VoucherHub.Modules.Vouchers.Infrastructure.Repositories;
using VoucherHub.Modules.Vouchers.API;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using VoucherHub.Modules.Vouchers.Infrastructure.Persistence;
using Microsoft.Extensions.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddScoped<IVoucherRepository, PostgresVoucherRepository>();
builder.Services.AddScoped<VoucherService>();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<VoucherHubDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("VoucherHub")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapVoucherEndpoints();

app.Run();


public partial class Program { }
