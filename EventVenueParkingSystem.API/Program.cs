using EventParkingReservationSystem.API.Configuration;
using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.Helpers;
using EventParkingReservationSystem.API.Repositories.Implementations;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using EventParkingReservationSystem.API.Services.Implementations;
using EventParkingReservationSystem.API.Services.Interfaces;
using EventVenueParkingSystem.API.Repositories.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// Controllers
// =====================================================
builder.Services.AddControllers();


// =====================================================
// Database
// =====================================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


// =====================================================
// Configuration
// =====================================================
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));


// =====================================================
// Helpers
// =====================================================
builder.Services.AddScoped<PasswordHelper>();
builder.Services.AddScoped<TokenHelper>();
builder.Services.AddScoped<JwtHelper>();


// =====================================================
// Repositories
// =====================================================

// Member 1 - Customer
builder.Services.AddScoped<
    ICustomerRepository,
    CustomerRepository>();

// Member 2 - Venue
builder.Services.AddScoped<
    IVenueRepository,
    VenueRepository>();

// Member 2 - Event Category
builder.Services.AddScoped<
    IEventCategoryRepository,
    EventCategoryRepository>();

// Member 2 - Event
builder.Services.AddScoped<
    IEventRepository,
    EventRepository>();

// Member 3 - Seat
builder.Services.AddScoped<
    ISeatRepository,
    SeatRepository>();

// Member 3 - Parking Slot
builder.Services.AddScoped<
    IParkingSlotRepository,
    ParkingSlotRepository>();


// =====================================================
// Services
// =====================================================

// Member 1 - Authentication
builder.Services.AddScoped<
    IAuthService,
    AuthService>();

// Member 1 - Customer
builder.Services.AddScoped<
    ICustomerService,
    CustomerService>();

// Member 1 - Email
builder.Services.AddScoped<
    IEmailService,
    EmailService>();

// Member 2 - Venue
builder.Services.AddScoped<
    IVenueService,
    VenueService>();

// Member 2 - Event Category
builder.Services.AddScoped<
    IEventCategoryService,
    EventCategoryService>();

// Member 2 - Event
builder.Services.AddScoped<
    IEventService,
    EventService>();

// Member 3 - Seat
builder.Services.AddScoped<
    ISeatService,
    SeatService>();

// Member 3 - Parking Slot
builder.Services.AddScoped<
    IParkingSlotService,
    ParkingSlotService>();


// =====================================================
// JWT Settings
// =====================================================
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>();

if (jwtSettings == null)
{
    throw new InvalidOperationException(
        "JwtSettings configuration is missing.");
}

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException(
        "JwtSettings Key is missing.");
}


// =====================================================
// JWT Authentication
// =====================================================
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings.Key)),

                ClockSkew = TimeSpan.Zero
            };
    });


// =====================================================
// Authorization
// =====================================================
builder.Services.AddAuthorization();


// =====================================================
// OpenAPI
// =====================================================
builder.Services.AddOpenApi();


// =====================================================
// Build Application
// =====================================================
var app = builder.Build();


// =====================================================
// Database Initialization
// =====================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context =
        services.GetRequiredService<ApplicationDbContext>();

    var passwordHelper =
        services.GetRequiredService<PasswordHelper>();

    await DbInitializer.InitializeAsync(
        context,
        passwordHelper);
}


// =====================================================
// OpenAPI + Swagger UI
// =====================================================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Event Venue Parking System API v1");

        options.RoutePrefix = "swagger";
    });
}


// =====================================================
// HTTP Request Pipeline
// =====================================================
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


// =====================================================
// Controllers
// =====================================================
app.MapControllers();


// =====================================================
// Run
// =====================================================
app.Run();