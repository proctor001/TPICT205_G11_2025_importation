using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEnd_TimeManagement.Data;
using BackEnd_TimeManagement.Models;
using System.Security.Cryptography;
using System.Text;

namespace BackEnd_TimeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtudiantsController(ApplicationDbContext context) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Etudiants etudiant)
        {
            if (await context.Etudiants.AnyAsync(e => e.Email == etudiant.Email))
            {
                return BadRequest("Email déjà utilisé");
            }

            etudiant.MotDePasse = HashPassword(etudiant.MotDePasse);
            context.Etudiants.Add(etudiant);
            await context.SaveChangesAsync();

            return Ok("Inscription réussie");
        }

        private static string HashPassword(string password)
        {
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}