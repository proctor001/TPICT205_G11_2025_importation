using System;
using System.ComponentModel.DataAnnotations;

namespace BackEnd_TimeManagement.Models
{
    public class Etudiants
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        public string Prenom { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string MotDePasse { get; set; }

        [Required]
        public string Filiere { get; set; }

        [Required]
        public string Niveau { get; set; }

        public string Role { get; set; } = "etudiant";
    }
}