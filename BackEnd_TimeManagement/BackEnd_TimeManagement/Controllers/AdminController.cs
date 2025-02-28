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