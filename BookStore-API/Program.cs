using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.Mappings;
using BookStore_API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddRazorPages();

builder.Services.AddAutoMapper(typeof(Maps));

builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Book Store API",
        Version = "v1",
        Description = "Educational API for a Book Store"
    });
    var xfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xpath = Path.Combine(AppContext.BaseDirectory, xfile);
    c.IncludeXmlComments(xpath);
});

builder.Services.AddSingleton<ILoggerService, LoggerService>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddCors(o =>
{
    o.AddPolicy("Cors Policy", b =>
    {
        b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Stor API");
    c.RoutePrefix = "";
});

app.UseCors("Cors Policy");

app.UseHttpsRedirection();
//app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
