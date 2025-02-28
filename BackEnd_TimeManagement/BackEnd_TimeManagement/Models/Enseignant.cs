using System; // Importation de l'espace de noms de base, notamment pour la gestion de types de données comme 'DateTime'
using System.ComponentModel.DataAnnotations; // Importation des annotations de validation, utilisées pour valider les propriétés de la classe
using BackEnd_TimeManagement.Models;

public class Enseignant
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }
    public string MotDePasse { get; set; }
    public string Role { get; set; } = "enseignant"; // Rôle par défaut
}