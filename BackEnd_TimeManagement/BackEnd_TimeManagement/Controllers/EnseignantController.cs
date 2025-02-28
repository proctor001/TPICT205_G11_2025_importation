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