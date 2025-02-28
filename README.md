# **TP205_TimeSchool - Gestion des Emplois de Temps**

## **Introduction**
Le projet **TP205_TimeSchool** est une application de gestion des **emplois du temps** pour la faculté de sciences. Le backend est développé en **ASP.NET Core** et le frontend utilise **Blazor Server**. Le projet permet de gérer les inscriptions, connexions et redirections en fonction des rôles des utilisateurs, tout en offrant une gestion des emplois du temps.

---

## **Pages Principales**

### **Page d'Inscription**
- **Route** : `/inscriptionEtudiant`
- Permet aux utilisateurs (étudiants) de s'inscrire en remplissant un formulaire de données personnelles.
- Envoi des données au backend pour validation et inscription.

### **Page de Connexion**
- **Route** : `/login`
- Permet aux utilisateurs de se connecter en utilisant leur email et mot de passe.
- Validation des informations par le backend et redirection vers le tableau de bord approprié.

### **Dashboard (Tableau de Bord)**
- **Route** : `/DashboardEtudiant` ou `/DashboardEnseignant`
- Après une connexion réussie, l'utilisateur est redirigé vers son tableau de bord selon son rôle (étudiant, enseignant, admin).
- Chaque utilisateur peut accéder à son emploi du temps et gérer ses informations de compte.

---

## **Intégration des Pages Comptes**

1. **Inscription et Connexion**
   - L'utilisateur remplit le formulaire d'inscription ou de connexion.
   - Les informations sont envoyées au backend pour validation.
   - En cas de succès, l'utilisateur est redirigé vers son tableau de bord selon son rôle.

2. **Redirection selon le Rôle**
   - Étudiant : redirection vers `/DashboardEtudiant`.
   - Enseignant : redirection vers `/DashboardEnseignant`.
   - Administrateur : redirection vers une interface d'administration.

---

## **Conclusion**
Ce projet offre une solution complète pour la gestion des emplois du temps, tout en intégrant un système de comptes utilisateurs simple et efficace. Il permet aux étudiants et enseignants de s'inscrire, se connecter et accéder à leurs emplois du temps respectifs, avec une gestion sécurisée des informations grâce à l'utilisation de rôles. L'application fournit une base solide pour une gestion flexible et centralisée des emplois du temps tout en respectant les besoins des utilisateurs.