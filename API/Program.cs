using API.Data;
using API.Entities;
using API.Extensions;
using API.Middleware;
using API.Services;
using API.signalR;
using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


builder.Services.AddAppServices(builder.Configuration);

builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(builder => builder.AllowAnyHeader().AllowCredentials().AllowAnyMethod().WithOrigins("https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization(); 

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");
using var scope = app.Services.CreateScope();
var service = scope.ServiceProvider;
try {
    var dataContext = service.GetRequiredService<DataContext>();
    var userManager = service.GetRequiredService<UserManager<AppUser>>(); //
    var roleManager = service.GetRequiredService<RoleManager<AppRole>>(); //<--
    await dataContext.Database.MigrateAsync();
    await Seed.SeedUsers(userManager, roleManager); //<-
    await dataContext.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]");
}
catch (System.Exception e)
{
    var log = service.GetRequiredService<ILogger<Program>>();
    log.LogError(e, "an error occurred during migration !!");
}

app.Run();