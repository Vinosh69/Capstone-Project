var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ✅ Custom error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error.html"); // we will put error.html in wwwroot
    app.UseHsts();
}

// ✅ Force HTTPS
app.UseHttpsRedirection();

// ✅ Logging middleware (Request + Response)
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next(); // call next middleware
    Console.WriteLine($"Response Status: {context.Response.StatusCode}");
});

// ✅ Basic Security: Content Security Policy (protect from XSS etc.)
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
    await next();
});

// ✅ Enable Static Files from wwwroot
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Your MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();