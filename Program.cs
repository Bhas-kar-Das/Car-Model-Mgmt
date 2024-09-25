using DemoAppAdo.Data;
using DemoAppAdo.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUserRepository, UserRepository>(provider =>
    new UserRepository(configuration.GetConnectionString("DemoDB")));

builder.Services.AddScoped<CarModelRepository>(provider =>
    new CarModelRepository(
        builder.Configuration.GetConnectionString("DemoDB"),
        provider.GetRequiredService<ClassRepository>(),
        provider.GetRequiredService<BrandRepository>()));
builder.Services.AddScoped<BrandRepository>(provider =>
    new BrandRepository(configuration.GetConnectionString("DemoDB")));
builder.Services.AddScoped<ClassRepository>(provider =>
    new ClassRepository(configuration.GetConnectionString("DemoDB")));
builder.Services.AddScoped<SalesRecordRepository>(provider =>
    new SalesRecordRepository(configuration.GetConnectionString("DemoDB")));

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Configure CORS
app.UseCors(options =>
{
    options.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

// Serve static files from "wwwroot" and from a custom directory "iages"
app.UseStaticFiles(); // Serve files from wwwroot

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "images")),
    RequestPath = "/Files"
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo API V1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();
app.Run();
