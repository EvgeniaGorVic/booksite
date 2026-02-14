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
                    ImageUrl = "/images/book2.jpg",
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
                    ImageUrl = "/images/book4.jpg",
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
                    ImageUrl = "/images/book1.jpg",
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
                    ImageUrl = "/images/book3.jpg",
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
                },
                new Book
                {
                    Title = "Убить пересмешника",
                    Author = "Харпер Ли",
                    Description = "Роман о расовой несправедливости в американском Юге",
                    Price = 459,
                    ImageUrl = "/images/book7.jpg",
                    Category = "Классическая литература",
                    Rating = 4.7m,
                    Pages = 376,
                    Year = 1960,
                    IsBestseller = true
                },
                new Book
                {
                    Title = "Властелин колец",
                    Author = "Дж. Р. Р. Толкин",
                    Description = "Эпическая фэнтези-сага о Средиземье",
                    Price = 799,
                    ImageUrl = "/images/book8.jpg",
                    Category = "Фэнтези",
                    Rating = 4.9m,
                    Pages = 1178,
                    Year = 1954,
                    IsFeatured = true
                },
                new Book
                {
                    Title = "Гордость и предубеждение",
                    Author = "Джейн Остин",
                    Description = "Роман о любви и социальном статусе в Англии XIX века",
                    Price = 329,
                    ImageUrl = "/images/book9.jpg",
                    Category = "Роман",
                    Rating = 4.6m,
                    Pages = 432,
                    Year = 1813,
                    IsFeatured = true
                },
                new Book
                {
                    Title = "Три товарища",
                    Author = "Эрих Мария Ремарк",
                    Description = "Роман о дружбе и любви в послевоенной Германии",
                    Price = 379,
                    ImageUrl = "/images/book10.jpg",
                    Category = "Классическая литература",
                    Rating = 4.8m,
                    Pages = 480,
                    Year = 1936,
                    IsBestseller = true
                },
                new Book
                {
                    Title = "Атлант расправил плечи",
                    Author = "Айн Рэнд",
                    Description = "Философский роман об объективизме и капитализме",
                    Price = 899,
                    ImageUrl = "/images/book11.jpg",
                    Category = "Философский роман",
                    Rating = 4.5m,
                    Pages = 1168,
                    Year = 1957,
                    IsFeatured = true
                },
                new Book
                {
                    Title = "Сто лет одиночества",
                    Author = "Габриэль Гарсиа Маркес",
                    Description = "Магический реализм о семье Буэндиа",
                    Price = 429,
                    ImageUrl = "/images/book12.jpg",
                    Category = "Магический реализм",
                    Rating = 4.7m,
                    Pages = 448,
                    Year = 1967,
                    IsBestseller = true
                },
                new Book
                {
                    Title = "Шантарам",
                    Author = "Грегори Дэвид Робертс",
                    Description = "Роман о жизни в индийских трущобах",
                    Price = 699,
                    ImageUrl = "/images/book13.jpg",
                    Category = "Приключения",
                    Rating = 4.6m,
                    Pages = 936,
                    Year = 2003,
                    IsFeatured = true
                },
                new Book
                {
                    Title = "Код да Винчи",
                    Author = "Дэн Браун",
                    Description = "Детектив-триллер о религиозных тайнах",
                    Price = 399,
                    ImageUrl = "/images/book14.jpg",
                    Category = "Триллер",
                    Rating = 4.3m,
                    Pages = 489,
                    Year = 2003,
                    IsBestseller = true
                },
                new Book
                {
                    Title = "Портрет Дориана Грея",
                    Author = "Оскар Уайльд",
                    Description = "Роман о красоте, морали и вечной молодости",
                    Price = 289,
                    ImageUrl = "/images/book15.jpg",
                    Category = "Классическая литература",
                    Rating = 4.5m,
                    Pages = 254,
                    Year = 1890,
                    IsFeatured = true
                },
                new Book
                {
                    Title = "Метро 2033",
                    Author = "Дмитрий Глуховский",
                    Description = "Постапокалиптический роман о жизни в московском метро",
                    Price = 349,
                    ImageUrl = "/images/book16.jpg",
                    Category = "Постапокалипсис",
                    Rating = 4.4m,
                    Pages = 384,
                    Year = 2005,
                    IsBestseller = true
                },
                new Book
                {
                    Title = "Алхимик",
                    Author = "Пауло Коэльо",
                    Description = "Философская притча о поиске своей судьбы",
                    Price = 319,
                    ImageUrl = "/images/book17.jpg",
                    Category = "Философская проза",
                    Rating = 4.2m,
                    Pages = 208,
                    Year = 1988,
                    IsFeatured = true
                },
                new Book
                {
                    Title = "451° по Фаренгейту",
                    Author = "Рэй Брэдбери",
                    Description = "Антиутопия о обществе, где книги запрещены",
                    Price = 329,
                    ImageUrl = "/images/book18.jpg",
                    Category = "Антиутопия",
                    Rating = 4.6m,
                    Pages = 256,
                    Year = 1953,
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
                Password = HashPassword("test123"), // Пароль: test123
                FirstName = "Тестовый",
                LastName = "Пользователь",
                Phone = "+79001234567",
                IsActive = true,
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                AvatarUrl = "/images/default-avatar.png"
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

// Функция хеширования пароля (должна совпадать с методом в AccountController)
static string HashPassword(string password)
{
    using var sha256 = System.Security.Cryptography.SHA256.Create();
    var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(bytes);
}