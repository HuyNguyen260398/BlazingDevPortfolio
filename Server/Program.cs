using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options => {
    options.AddPolicy(
        "CorsPolicy",
        buidler => buidler.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());                  
});

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["jwt:Issuer"],
                        ValidAudience = builder.Configuration["jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(DtoMappings));
builder.Services.AddSwaggerGen();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var serviceScope = app.Services.CreateScope())
    {
        // Access injected services via serviceScope.ServiceProvider.
        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        SeedAdminRolesAndUsers.Seed(roleManager, userManager).Wait();
    }

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Blazing Dev Portfolio API");
        options.RoutePrefix = String.Empty;
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
