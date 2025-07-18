using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities.Identity;
using ParkNet.Entities.Enums;
using ParkNet.Entities.Types;
using ParkNet.Repositories;
using ParkNet.Services.Contracts;
using ParkNet.Services.Implementations;
using ParkNet.Repositories.Contracts;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ParkNetDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ParkNetDbContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Injeção de dependências (Repositories & Services)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IVehicleTypeService, VehicleTypeService>();
builder.Services.AddScoped<IBuildingImportService, BuildingImportService>();

builder.Services.AddScoped<IParkingSlotRepository, ParkingSlotRepository>();
builder.Services.AddScoped<IVehicleTypeRepository, VehicleTypeRepository>();
builder.Services.AddScoped<IFloorRepository, FloorRepository>();

builder.Services.AddRazorPages();

var app = builder.Build();

async Task SeedDataAsync(IServiceProvider services, ILogger logger)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var db = services.GetRequiredService<ParkNetDbContext>();

    string adminRole = UserRole.Admin.ToString();
    string adminEmail = "admin@parknet.com";
    string adminPassword = "Admin123!";

     if (!await roleManager.RoleExistsAsync(adminRole))
    {
        var result = await roleManager.CreateAsync(new IdentityRole(adminRole));
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                logger.LogError("Erro a criar role: {0} - {1}", error.Code, error.Description);
        }
    }

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
            logger.LogInformation("Admin criado com sucesso.");
        }
        else
        {
            foreach (var error in result.Errors)
                logger.LogError("Erro a criar admin: {0} - {1}", error.Code, error.Description);
        }
    }
    else
    {
        logger.LogInformation("Admin já existe.");
        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
            logger.LogInformation("Admin adicionado à role.");
        }
    }

    if (!db.VehicleTypes.Any())
    {
        db.VehicleTypes.AddRange(
            new VehicleType { Code = "C", Designation = "Carro" },
            new VehicleType { Code = "M", Designation = "Moto" }
        );
        await db.SaveChangesAsync();
        logger.LogInformation("Tipos de veículo seedados.");
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        await SeedDataAsync(services, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao executar seed de dados.");
    }
}

app.Run();
