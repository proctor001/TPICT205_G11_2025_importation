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