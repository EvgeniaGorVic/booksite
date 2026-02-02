using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using booksite.Data;
using booksite.Models;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация сервисов
builder.Services.AddControllersWithViews();

// База данных
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Аутентификация
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Home/Error";
    });

// Сессии
builder.Services.AddSession();

var app = builder.Build();

// Health check endpoint
app.MapGet("/health", () =>
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var canConnect = dbContext.Database.CanConnect();

        return Results.Json(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            database = canConnect ? "connected" : "disconnected",
            environment = app.Environment.EnvironmentName
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            status = "unhealthy",
            timestamp = DateTime.UtcNow,
            error = ex.Message,
            environment = app.Environment.EnvironmentName
        }, statusCode: 500);
    }
});

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Инициализация базы данных
InitializeDatabase(app);

app.Run();

static void InitializeDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        context.Database.EnsureCreated();
        Console.WriteLine("База данных проверена/создана");

        if (!context.Books.Any())
        {
            Console.WriteLine("Инициализация базы данных с книгами...");

            var books = new List<Book>
            {
                new Book
                {
                    Title = "Мастер и Маргарита",
                    Author = "Михаил Булгаков",
                    Description = "Классический роман о добре и зле, любви и предательстве",
                    Price = 499,
                    ImageUrl = "/images/book1.jpg",
                    Category = "Русская классика",
                    Rating = 4.8m,
                    Pages = 384,
                    Year = 1966,
                    IsFeatured = true,
                    IsBestseller = true
                },
                new Book
                {
                    Title = "Преступление и наказание",
                    Author = "Фёдор Достоевский",
                    Description = "Философский роман о морали и преступлении",
                    Price = 399,
                    ImageUrl = "/images/book2.jpg",
                    Category = "Русская классика",
                    Rating = 4.7m,
                    Pages = 608,
                    Year = 1866,
                    IsFeatured = true
                },
                new Book
                {
                    Title = "1984",
                    Author = "Джордж Оруэлл",
                    Description = "Антиутопия о тоталитарном обществе",
                    Price = 349,
                    ImageUrl = "/images/book3.jpg",
                    Category = "Антиутопия",
                    Rating = 4.6m,
                    Pages = 328,
                    Year = 1949,
                    IsBestseller = true
                },
                new Book
                {
                    Title = "Война и мир",
                    Author = "Лев Толстой",
                    Description = "Эпопея о русском обществе во время Наполеоновских войн",
                    Price = 599,
                    ImageUrl = "/images/book4.jpg",
                    Category = "Русская классика",
                    Rating = 4.9m,
                    Pages = 1225,
                    Year = 1869,
                    IsBestseller = true
                },
                new Book
                {
                    Title = "Гарри Поттер и философский камень",
                    Author = "Джоан Роулинг",
                    Description = "Первая книга о приключениях юного волшебника",
                    Price = 449,
                    ImageUrl = "/images/book5.jpg",
                    Category = "Фэнтези",
                    Rating = 4.8m,
                    Pages = 432,
                    Year = 1997,
                    IsFeatured = true
                },
                new Book
                {
                    Title = "Маленький принц",
                    Author = "Антуан де Сент-Экзюпери",
                    Description = "Философская сказка для детей и взрослых",
                    Price = 299,
                    ImageUrl = "/images/book6.jpg",
                    Category = "Детская литература",
                    Rating = 4.9m,
                    Pages = 96,
                    Year = 1943,
                    IsBestseller = true
                }
            };

            context.Books.AddRange(books);
            context.SaveChanges();
            Console.WriteLine($"Добавлено {books.Count} книг");
        }

        if (!context.Users.Any())
        {
            var testUser = new User
            {
                Username = "test",
                Email = "test@example.com",
                Password = "AQAAAAIAAYagAAAAEN8YHcVJgdqkknGy08jG1fY3fPPj/LLqkQvWYUXr3vYJptOcm9nQR8d+TRHmWYJYpQ==",
                FirstName = "Тестовый",
                LastName = "Пользователь",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow
            };

            context.Users.Add(testUser);
            context.SaveChanges();
            Console.WriteLine("Тестовый пользователь создан");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка инициализации БД: {ex.Message}");
    }
}