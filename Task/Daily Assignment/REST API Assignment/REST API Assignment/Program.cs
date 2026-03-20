using Microsoft.EntityFrameworkCore;
using REST_API_Assignment.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ✅ ADD CORS (FIXES "Failed to fetch")
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// ✅ KEEP YOUR DbContext (AppDbContext)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("MovieCatalogDb"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ ENABLE CORS
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();