using FileUpload.Middleware;
using FileUpload.Models;
using FileUpload.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; // Get the configuration object

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<FileDataService>();
builder.Services.AddTransient<IManageImage, ManageImage>();

// Add PostgreSQL database context
builder.Services.AddDbContext<FilesContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("PostgreConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// File size validation middleware
string[] allowedFileTypes = configuration.GetSection("AllowedFileTypes").Get<string[]>();
int maxFileSizeMB = configuration.GetValue<int>("MaxFileSizeMB");
int minFileSizeMB = configuration.GetValue<int>("MinFileSizeMB");
app.UseMiddleware<FileFilterMiddleware>(allowedFileTypes, maxFileSizeMB, minFileSizeMB);

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
