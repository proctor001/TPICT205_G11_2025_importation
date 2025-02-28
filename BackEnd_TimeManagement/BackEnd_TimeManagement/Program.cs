using BackEnd_TimeManagement.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -----------------
// Configuration des services
// -----------------

// 1. Configurer le DbContext (MySQL ici)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"), 
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")))
);

// 2. Configurer l'authentification JWT (pour usage futur si nécessaire)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidateLifetime = true,
             ValidateIssuerSigningKey = true,
             ValidIssuer = builder.Configuration["Jwt:Issuer"],
             ValidAudience = builder.Configuration["Jwt:Audience"],
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
         };
    });

// 3. Configurer CORS pour autoriser les requêtes depuis le front-end (http://localhost:5202)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy => policy.WithOrigins("http://localhost:5202")
                         .AllowAnyMethod()
                         .AllowAnyHeader());
});

// 4. Ajouter les contrôleurs et Swagger (pour la documentation)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Forcer l'application à écouter sur le port 5063
app.Urls.Add("http://localhost:5063");

// -----------------
// Configuration du pipeline HTTP
// -----------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Rediriger en HTTPS dès le début
app.UseHttpsRedirection();

// Activer CORS AVANT l'authentification et l'autorisation
app.UseCors("AllowBlazor");

app.UseAuthentication();
app.UseAuthorization();

// Mapper les contrôleurs
app.MapControllers();

app.Run();
    