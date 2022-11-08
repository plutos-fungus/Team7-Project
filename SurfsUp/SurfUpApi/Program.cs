using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurfUpApi.Data;
using Microsoft.AspNetCore.Identity;
using SurfUpApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SurfUpApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SurfUpApiContext") ?? throw new InvalidOperationException("Connection string 'SurfUpApiContext' not found.")));


builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<SurfUpApiContext>().AddDefaultUI()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>(TokenOptions.DefaultProvider);
builder.Services.AddAuthorization(options => options.AddPolicy("RequiredAdminRole", policy => policy.RequireRole("Admin")));


//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<SurfUpApiContext>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins().AllowAnyHeader().AllowAnyOrigin()));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllers();

app.Run();
