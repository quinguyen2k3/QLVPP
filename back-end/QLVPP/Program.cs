using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Repositories;
using QLVPP.Repositories.Implementations;
using QLVPP.Services;
using QLVPP.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// 🔗 Add DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔗 Add UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 🔗 Add Services
builder.Services.AddScoped<ICategoryService, CategoryService>();

// 🔗 Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
