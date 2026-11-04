using ClinicAppManager.Authorization;
using ClinicAppManager.Extensions;
using ClinicAppManagerBusinessLayer;
using ClinicAppManagerBusinessLayer.Appointments;
using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerBusinessLayer.Payments;
using ClinicAppManagerDataLayer;
using ClinicAppManagerDataLayer.Appoinments;
using ClinicAppManagerDataLayer.DataLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;
using ClinicAppManagerDataLayer.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("AuthLimiter", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenAuth();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ClinicAppManager")));

builder.Services.AddScoped<IPersonData, PersonData>();
builder.Services.AddScoped<IDataData, DataData>();
builder.Services.AddScoped<IDataBusiness, DataBusiness>();
builder.Services.AddScoped<IPatientData, PatientData>();
builder.Services.AddScoped<IPatientBusiness, PatientBuiness>();
builder.Services.AddScoped<IDoctorData, DoctorData>();
builder.Services.AddScoped<IDoctorBusiness, DoctorBuiness>();
builder.Services.AddScoped<IUserData, UserData>();
builder.Services.AddScoped<IUserBusiness, UserBusiness>();
builder.Services.AddScoped<IAppointmentStatusData, AppointmentStatusData>();
builder.Services.AddScoped<IAppointmentStatusBusiness, AppointmentStatusBusiness>();
builder.Services.AddScoped<IAppointmentData, AppointmentData>();
builder.Services.AddScoped<IAppointmentBusiness, AppointmentBusiness>();
builder.Services.AddScoped<IMedicalRecordData, MedicalRecordData>();
builder.Services.AddScoped<IMedicalRecordBusiness, MedicalRecordBusiness>();
builder.Services.AddScoped<IPaymentMethodData, PaymentMethodData>();
builder.Services.AddScoped<IPaymentMethodBusiness, PaymentMethodBusiness>();
builder.Services.AddScoped<IPaymentData, PaymentData>();
builder.Services.AddScoped<IPaymentBusiness, PaymentBusiness>();
builder.Services.AddScoped<IPrescribedMedicationData, PrescribedMedicationData>();
builder.Services.AddScoped<IPrescribedMedicationBusiness, PrescribedMedicationBusiness>();

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddCustomJWTAuth(builder.Configuration);

builder.Services.AddSingleton<IAuthorizationHandler, OwnerOrAdminHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OwnerOrAdminPolicy", policy =>
        policy.Requirements.Add(new OwnerOrAdminRequirement()));
});


var app = builder.Build();

await SeedAdminUserAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
    {
        await context.Response.WriteAsync("Too many login attempts. Please try again later.");
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task SeedAdminUserAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var configuration = services.GetRequiredService<IConfiguration>();

    string[] adminRole = ["admin", "user"];
    string? adminEmail = configuration["AdminSeed:Email"];
    string? adminPassword = configuration["AdminSeed:Password"];

    if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
    {
        Console.WriteLine("Admin seeding skipped. Configure AdminSeed:Email and AdminSeed:Password to create an administrator.");
        return;
    }

    foreach (var role in adminRole)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
    if (existingAdmin == null)
    {
        var adminUser = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createAdminResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (!createAdminResult.Succeeded)
        {
            var errors = string.Join(", ", createAdminResult.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Failed to seed admin user: {errors}");
        }

        existingAdmin = adminUser;

       
    }

    IUserBusiness userBusiness = services.GetRequiredService<IUserBusiness>();

    if (!await userBusiness.CheckUserExistByAspNetUserId(existingAdmin.Id))
    {
        userBusiness.Initialize(new UserDTO
        {
            AspNetUserId = existingAdmin.Id,
            UserName = existingAdmin.UserName,
            IsActive = true,
            Person = new PersonDTO
            {
                FirstName = "System",
                LastName = "Administrator",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Address = "Not specified",
                Gender = 'M'

            }
        });

        if (!await userBusiness.Save())
        {
            throw new InvalidOperationException("Failed to save admin user data to the database.");
        }
    }
  
    if (!await userManager.IsInRoleAsync(existingAdmin, adminRole[0]))
    {
        await userManager.AddToRoleAsync(existingAdmin, adminRole[0]);
    }
    if (!await userManager.IsInRoleAsync(existingAdmin, adminRole[1]))
    {
        await userManager.AddToRoleAsync(existingAdmin, adminRole[1]);
    }

    


}

