ok , je vais t'envoyer le code , fichier par fichier

contenu du dossier controllers

AccountController.cs

// Fichier : Controllers/AccountController.cs
using BackEnd_TimeManagement.Data;
using BackEnd_TimeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// using BCrypt.Net;

namespace BackEnd_TimeManagement.Controllers
{
[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Endpoint de création d'utilisateur (Admin seulement)
        [HttpPost("create")]
        [Authorize(Roles = "Admin")] // Seul l'Admin peut créer des comptes
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
        {
            if (_context.Utilisateurs.Any(u => u.Email == model.Email))
            {
                return BadRequest("L'email existe déjà.");
            }

            // var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.MotDePasse);
            var hashedPassword = model.MotDePasse;

            var utilisateur = new Utilisateur
            {
                Email = model.Email,
                MotDePasse = hashedPassword,
                Role = model.Role
            };

            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Utilisateur créé avec succès !" });
        }
    }

    // ✅ Modèle pour la création d'un utilisateur
    public class CreateUserModel
    {
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public string Role { get; set; } // Peut être Admin, Enseignant, ou Étudiant
    }
}

AdminController

// Fichier : Controllers/AdminController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd_TimeManagement.Controllers
{
[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
// ✅ Endpoint accessible uniquement par un Admin
[HttpGet("dashboard")]
[Authorize(Roles = "Admin")] // Accès uniquement pour les utilisateurs ayant le rôle Admin
public IActionResult GetAdminDashboard()
{
// Logic de l'admin pour accéder au dashboard
return Ok("Bienvenue sur le dashboard Admin !");
}
}
}

AuthController

using BackEnd_TimeManagement.Data;
using BackEnd_TimeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackEnd_TimeManagement.Controllers
{
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
private readonly ApplicationDbContext _context;
private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ✅ Endpoint de connexion : /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            //if (utilisateur == null || !BCrypt.Net.BCrypt.Verify(model.MotDePasse, utilisateur.MotDePasse))
            if (utilisateur == null || model.MotDePasse != utilisateur.MotDePasse)
            {
                return Unauthorized(new { message = "Identifiants incorrects" });
            }

            var token = GenerateJwtToken(utilisateur);
            return Ok(new { token, role = utilisateur.Role });
        }

        // 🔐 Génération du JWT Token
        private string GenerateJwtToken(Utilisateur utilisateur)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, utilisateur.Id.ToString()),
                new Claim(ClaimTypes.Email, utilisateur.Email),
                new Claim(ClaimTypes.Role, utilisateur.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // ✅ Modèle pour la connexion
    public class LoginModel
    {
        public string Email { get; set; }
        public string MotDePasse { get; set; }
    }
}

EnseignantController

// Fichier : Controllers/EnseignantController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd_TimeManagement.Controllers
{
[Route("api/[controller]")]
[ApiController]
public class EnseignantController : ControllerBase
{
// ✅ Endpoint accessible uniquement par un Enseignant
[HttpGet("dashboard")]
[Authorize(Roles = "Enseignant")] // Accès uniquement pour les utilisateurs ayant le rôle Enseignant
public IActionResult GetEnseignantDashboard()
{
// Logic de l'enseignant pour accéder au dashboard
return Ok("Bienvenue sur le dashboard Enseignant !");
}
}
}

EtudiantsController

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackEnd_TimeManagement.Data;
using BackEnd_TimeManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd_TimeManagement.Controllers
{
[Route("api/[controller]")] // URL de base : api/etudiants
[ApiController] // Indique que ce contrôleur est une API
public class EtudiantsController : ControllerBase
{
private readonly ApplicationDbContext _context;

        // Injection du contexte de la base de données dans le contrôleur
        public EtudiantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET : api/etudiants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Etudiants>>> GetEtudiants()
        {
            return await _context.Etudiants.ToListAsync();
        }

        // GET : api/etudiants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Etudiants>> GetEtudiant(int id)
        {
            var etudiant = await _context.Etudiants.FindAsync(id);

            if (etudiant == null)
            {
                return NotFound();
            }

            return etudiant;
        }

        // POST : api/etudiants
        [HttpPost]
        public async Task<ActionResult<Etudiants>> PostEtudiant(Etudiants etudiant)
        {
            // Ajoute l'étudiant à la base de données
            _context.Etudiants.Add(etudiant);
            await _context.SaveChangesAsync();

            // Retourne l'objet créé avec son ID
            return CreatedAtAction(nameof(GetEtudiant), new { id = etudiant.Id }, etudiant);
        }

        // PUT : api/etudiants/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEtudiant(int id, Etudiants etudiant)
        {
            if (id != etudiant.Id)
            {
                return BadRequest();
            }

            _context.Entry(etudiant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EtudiantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE : api/etudiants/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEtudiant(int id)
        {
            var etudiant = await _context.Etudiants.FindAsync(id);
            if (etudiant == null)
            {
                return NotFound();
            }

            _context.Etudiants.Remove(etudiant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Vérifie si un étudiant existe dans la base de données
        private bool EtudiantExists(int id)
        {
            return _context.Etudiants.Any(e => e.Id == id);
        }

        // ✅ Endpoint accessible uniquement par un Étudiant
        [HttpGet("dashboard")]
        [Authorize(Roles = "Etudiant")] // Accès uniquement pour les utilisateurs ayant le rôle Etudiant
        public IActionResult GetEtudiantDashboard()
        {
            // Logique de l'étudiant pour accéder au dashboard
            return Ok("Bienvenue sur le dashboard Étudiant !");
        }
    }
}

UtilisateursController

using BackEnd_TimeManagement.Data;
using BackEnd_TimeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using BCrypt.Net;

namespace BackEnd_TimeManagement.Controllers
{
[Route("api/utilisateurs")]
[ApiController]
public class UtilisateursController : ControllerBase
{
private readonly ApplicationDbContext _context;

        public UtilisateursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ 1. Lister tous les utilisateurs (Admin uniquement)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Utilisateur>>> GetUtilisateurs()
        {
            return await _context.Utilisateurs.ToListAsync();
        }

        // ✅ 2. Ajouter un enseignant (Admin uniquement)
        [HttpPost("ajouter-enseignant")]
        public async Task<ActionResult<Utilisateur>> AjouterEnseignant(Utilisateur utilisateur)
        {
            // Vérifier si l'email existe déjà
            if (await _context.Utilisateurs.AnyAsync(u => u.Email == utilisateur.Email))
            {
                return BadRequest(new { message = "Cet email est déjà utilisé." });
            }

            // Définir le rôle et hacher le mot de passe
            utilisateur.Role = "Enseignant";
            utilisateur.MotDePasse = utilisateur.MotDePasse;
            //utilisateur.MotDePasse = BCrypt.Net.BCrypt.HashPassword(utilisateur.MotDePasse);

            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUtilisateurs), new { id = utilisateur.Id }, utilisateur);
        }
    }
}

contenu du dossier Data

ApplicationDbContext

// Importation des namespaces nécessaires
using BackEnd_TimeManagement.Models; // Contient les classes d'entités (ex: Etudiant)
using Microsoft.EntityFrameworkCore; // Fournit les classes pour Entity Framework Core

namespace BackEnd_TimeManagement.Data
{
// La classe ApplicationDbContext hérite de DbContext, qui est la classe de base pour interagir avec la base de données
public class ApplicationDbContext : DbContext
{
// Constructeur qui prend les options de configuration pour EF Core
// Ces options incluent généralement la chaîne de connexion à la base de données
public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
{
// Le constructeur de base (DbContext) est appelé avec les options fournies
}

        // DbSet<Etudiants> représente la table "Etudiants" dans la base de données
        // Cette propriété permet d'accéder et de manipuler les enregistrements de la table Etudiants
        public DbSet<Etudiants> Etudiants { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; } // Nouvelle table pour gérer les utilisateurs
        
        // Méthode OnModelCreating : utilisée pour configurer le modèle de données
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Appel de la méthode de base pour s'assurer que la configuration par défaut est appliquée
            base.OnModelCreating(modelBuilder);

            // Configuration personnalisée pour l'entité Etudiant
            modelBuilder.Entity<Etudiants>()
                .HasIndex(e => e.Email) // Crée un index sur la colonne Email
                .IsUnique(); // Garantit que l'email est unique dans la table Etudiants
            
            // Assurer l'unicité de l'email pour les utilisateurs (Admin, Enseignants, Étudiants)
            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Création d'un compte admin par défaut
            modelBuilder.Entity<Utilisateur>().HasData(new Utilisateur
            {
                Id = 1,
                Nom = "Admin Université",
                Email = "admin@univ.com",
                MotDePasse = "Admin", // Hachage du mot de passe
                // MotDePasse = BCrypt.Net.BCrypt.HashPassword("Admin"), // Hachage du mot de passe
                Role = "Admin"
            });
        }
    }
}

contenu du dossier Migrations

20250217165736_InitialCreate

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd_TimeManagement.Migrations
{
/// <inheritdoc />
public partial class InitialCreate : Migration
{
/// <inheritdoc />
protected override void Up(MigrationBuilder migrationBuilder)
{
migrationBuilder.AlterDatabase()
.Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Etudiants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nom = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prenom = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MotDePasse = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Departement = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Filiere = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Niveau = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etudiants", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Etudiants_Email",
                table: "Etudiants",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Etudiants");
        }
    }
}

20250217165736_InitialCreate.Designer

// <auto-generated />
using BackEnd_TimeManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BackEnd_TimeManagement.Migrations
{
[DbContext(typeof(ApplicationDbContext))]
[Migration("20250217165736_InitialCreate")]
partial class InitialCreate
{
/// <inheritdoc />
protected override void BuildTargetModel(ModelBuilder modelBuilder)
{
#pragma warning disable 612, 618
modelBuilder
.HasAnnotation("ProductVersion", "9.0.2")
.HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("BackEnd_TimeManagement.Models.Etudiants", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Departement")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Filiere")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("MotDePasse")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Niveau")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Prenom")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Etudiants");
                });
#pragma warning restore 612, 618
}
}
}

ApplicationDbContextModelSnapshot

// <auto-generated />
using BackEnd_TimeManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BackEnd_TimeManagement.Migrations
{
[DbContext(typeof(ApplicationDbContext))]
partial class ApplicationDbContextModelSnapshot : ModelSnapshot
{
protected override void BuildModel(ModelBuilder modelBuilder)
{
#pragma warning disable 612, 618
modelBuilder
.HasAnnotation("ProductVersion", "9.0.2")
.HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("BackEnd_TimeManagement.Models.Etudiants", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Departement")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Filiere")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("MotDePasse")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Niveau")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Prenom")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Etudiants");
                });
#pragma warning restore 612, 618
}
}
}

contenu du dossier Models

Etudiants

    using System; // Importation de l'espace de noms de base, notamment pour la gestion de types de données comme 'DateTime'
using System.ComponentModel.DataAnnotations; // Importation des annotations de validation, utilisées pour valider les propriétés de la classe

namespace BackEnd_TimeManagement.Models
{
// La classe 'Etudiants' représente un étudiant dans le système de gestion des étudiants
// Elle contient des propriétés qui correspondent aux informations des étudiants dans la base de données
public class Etudiants
{
// 🔹 1. Propriété Id - Clé primaire de l'étudiant
[Key] // L'annotation [Key] indique que cette propriété est la clé primaire dans la base de données
public int Id { get; set; } // Identifiant unique de l'étudiant

        // 🔹 2. Propriété Nom - Nom de l'étudiant
        [Required] // L'annotation [Required] signifie que cette propriété est obligatoire
        public string Nom { get; set; } // Le nom de l'étudiant

        // 🔹 3. Propriété Prenom - Prénom de l'étudiant
        [Required] // La propriété Prénom est également obligatoire
        public string Prenom { get; set; } // Le prénom de l'étudiant

        // 🔹 4. Propriété Email - Adresse e-mail de l'étudiant
        [Required] // Cette propriété est obligatoire
        [EmailAddress] // L'annotation [EmailAddress] permet de valider si l'email est bien formaté
        public string Email { get; set; } // L'adresse email de l'étudiant

        // 🔹 5. Propriété MotDePasse - Mot de passe de l'étudiant
        [Required] // L'annotation [Required] garantit que la propriété est remplie
        public string MotDePasse { get; set; } // Le mot de passe de l'étudiant

        // 🔹 6. Propriété Departement - Département dans lequel l'étudiant est inscrit
        [Required] // Le département est obligatoire
        public string Departement { get; set; } // Le département (ex : Informatique, Mathématiques, etc.)

        // 🔹 7. Propriété Filiere - Filière de l'étudiant
        [Required] // La filière est obligatoire
        public string Filiere { get; set; } // La filière de l'étudiant (ex : Développement Web, Analyse de données, etc.)

        // 🔹 8. Propriété Niveau - Niveau d'études de l'étudiant (ex : L1, L2, M1, M2, etc.)
        [Required] // Le niveau est également requis
        public string Niveau { get; set; } // Le niveau d'études de l'étudiant
    }
}

Utilisateur

using System.ComponentModel.DataAnnotations;

namespace BackEnd_TimeManagement.Models
{
public class Utilisateur
{
[Key] // Clé primaire
public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required]
        [EmailAddress] // Vérifie que c'est un email valide
        public string Email { get; set; }

        [Required]
        public string MotDePasse { get; set; } // On va le hacher plus tard

        [Required]
        public string Role { get; set; } // "Etudiant", "Enseignant", "Admin"
    }
}

A la racine

appsettings

{
"Logging": {
"LogLevel": {
"Default": "Information",
"Microsoft.AspNetCore": "Warning"
}
},
"AllowedHosts": "*",
"ConnectionStrings": {
"DefaultConnection": "server=localhost;database=TP205_TimeSchool_Etudiants;user=root;password=root;"
},
"Jwt": {
"Key": "MaSuperCleSecreteQuiDoitEtreLongue123456890",
"Issuer": "backend_time_management",
"Audience": "frontend_application"
}

}

appsettings.Development

{
"Logging": {
"LogLevel": {
"Default": "Information",
"Microsoft.AspNetCore": "Warning"
}
}
}

Program.cs

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BackEnd_TimeManagement.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// 🔹 1. Configuration de la chaîne de connexion MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 🔹 2. Ajout des services nécessaires pour l'API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

// 🔹 3. Configuration de l'authentification JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
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
IssuerSigningKey = new SymmetricSecurityKey(key)
};
});

var app = builder.Build();

// 🔹 4. Configuration des middlewares pour l'environnement de développement
if (app.Environment.IsDevelopment())
{
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();
}

// 🔹 5. Ajout des middlewares essentiels
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(builder =>
builder.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader());

app.UseAuthentication(); // Activation de l'authentification JWT
app.UseAuthorization();  // Activation de la gestion des autorisations

// 🔹 6. Mapping des routes des contrôleurs
app.MapControllers();

// 🔹 7. Lancement de l'application
app.Run();