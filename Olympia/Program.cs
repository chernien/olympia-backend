using HelpDesk.Repository;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Olympia.Interfaces;
using Olympia.Models;
using Olympia.Repositories;
using Olympia.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowOrigin",
        builder =>
        {
            builder.WithOrigins("*","https://olympiaclub.olympiacompany.com", "http://localhost:4200", "https://storeolympia.com",
                "https://www.storeolympia.com", "http://197.13.2.117:89", "*")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowAnyOrigin();
        });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers().AddJsonOptions(x =>
                                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ERP210OLYMPIA_FContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DataBase"));
});
builder.Services.AddScoped<IArtRepository, ArtRepository>();
builder.Services.AddScoped<ITarRepository, TarRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IRemiseRepository, RemiseRepository>();
builder.Services.AddScoped<ITarRemiseRepository, TarRemiseRepository>();
builder.Services.AddScoped<IGuestRepository, GuestRepository>();
builder.Services.AddHttpClient<Olympia.Services.SmsService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapFallbackToController("Index", "Fallback");
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthorization();
app.UseCors("AllowOrigin");
app.MapControllers();
app.Run();
