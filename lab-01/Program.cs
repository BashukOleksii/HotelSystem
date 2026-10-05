using FluentValidation;
using lab_01.Data;
using lab_01.ExceptionHandling;
using lab_01.Models.Entities;
using lab_01.Repositories.Implementations;
using lab_01.Repositories.Interfaces;
using lab_01.Services.Implementations;
using lab_01.Services.Interfaces;
using lab_01.Validators.Hotel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped(
    typeof(IRepository<>),
    typeof(Repository<>)
);
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IImageStorageService,ImageStorageService>();

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(Program).Assembly
);

builder.Services.AddValidatorsFromAssemblyContaining<HotelCreateDtoValidator>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddIdentity<User, IdentityRole>(options => 
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 8;

        options.User.RequireUniqueEmail = true;
    }
)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath =
        "/Account/Login";

    options.AccessDeniedPath =
        "/Account/AccessDenied";
});

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<
    GlobalExceptionHandler
>();

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.InitializeAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
    app.UseHsts();


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
)
.WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Hotels}/{action=Index}/{id?}"
)
.WithStaticAssets();


app.Run();
