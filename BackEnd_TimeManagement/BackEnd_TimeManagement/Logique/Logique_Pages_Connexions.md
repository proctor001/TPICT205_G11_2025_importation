---

## **1. Page de Connexion (Login)**

### **Logique Frontend (Blazor Server)**

#### **Rôle du Frontend** :
- Afficher un formulaire de connexion.
- Envoyer les données de connexion (email et mot de passe) au backend.
- Gérer la réponse du backend (succès ou échec).
- Rediriger l'utilisateur vers le tableau de bord en cas de succès.

#### **Étapes** :
1. **Formulaire de connexion** :
    - Deux champs : email et mot de passe.
    - Un bouton "Se connecter" pour soumettre le formulaire.
    - Un lien vers la page d'inscription ("Je n'ai pas de compte").

2. **Soumission du formulaire** :
    - Lorsque l'utilisateur clique sur "Se connecter", le frontend envoie une requête HTTP POST au backend avec les données du formulaire (email et mot de passe).

3. **Gestion de la réponse** :
    - Si la connexion réussit, le backend renvoie un **token JWT**.
    - Le frontend stocke ce token (dans le localStorage ou un service d'état Blazor).
    - Redirige l'utilisateur vers le tableau de bord.

    - Si la connexion échoue, le frontend affiche un message d'erreur (ex: "Email ou mot de passe incorrect").

#### **Exemple de code Frontend (Blazor)** :
```razor
@page "/login"
@inject HttpClient Http
@inject NavigationManager Navigation

<h3>Connexion</h3>

<EditForm Model="@loginRequest" OnValidSubmit="HandleLogin">
    <InputText @bind-Value="loginRequest.Email" placeholder="Email" />
    <InputText @bind-Value="loginRequest.MotDePasse" type="password" placeholder="Mot de passe" />
    <button type="submit">Se connecter</button>
</EditForm>

<p><a href="/register">Je n'ai pas de compte</a></p>

@code {
    private LoginRequest loginRequest = new LoginRequest();

    private async Task HandleLogin()
    {
        var response = await Http.PostAsJsonAsync("api/etudiants/login", loginRequest);

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadAsStringAsync();
            // Stocker le token (ex: dans le localStorage ou un service Blazor)
            Navigation.NavigateTo("/dashboard"); // Redirection vers le tableau de bord
        }
        else
        {
            // Afficher un message d'erreur
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string MotDePasse { get; set; }
    }
}
```

---

### **Logique Backend (ASP.NET Core)**

#### **Rôle du Backend** :
- Recevoir les données de connexion (email et mot de passe).
- Vérifier si l'email existe dans la base de données.
- Comparer le mot de passe fourni avec le mot de passe hashé stocké.
- Générer un token JWT en cas de succès.
- Renvoyer une réponse au frontend (token ou message d'erreur).

#### **Étapes** :
1. **Endpoint** :
    - Créer un endpoint `POST /api/etudiants/login` pour gérer la connexion.

2. **Validation** :
    - Vérifier si l'email existe dans la base de données.
    - Comparer le mot de passe fourni avec le mot de passe hashé stocké.

3. **Génération du token** :
    - Si les identifiants sont valides, générer un token JWT contenant des informations comme l'ID de l'étudiant et son rôle.

4. **Réponse** :
    - Renvoyer le token JWT au frontend en cas de succès.
    - Renvoyer un message d'erreur en cas d'échec.

#### **Exemple de code Backend (ASP.NET Core)** :
```csharp
[HttpPost("login")]
public async Task<ActionResult<string>> Login(LoginRequest request)
{
    // Trouver l'étudiant par email
    var etudiant = await _context.Etudiants.FirstOrDefaultAsync(e => e.Email == request.Email);

    // Vérifier si l'étudiant existe et si le mot de passe est correct
    if (etudiant == null || !VerifyPassword(request.MotDePasse, etudiant.MotDePasse))
    {
        return Unauthorized("Email ou mot de passe incorrect.");
    }

    // Générer un token JWT
    var token = GenerateJwtToken(etudiant);

    return Ok(new { Token = token });
}

private bool VerifyPassword(string password, string hashedPassword)
{
    // Implémenter la vérification du mot de passe (ex: avec BCrypt)
    return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
}

private string GenerateJwtToken(Etudiant etudiant)
{
    // Implémenter la génération du token JWT
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, etudiant.Id.ToString()),
            new Claim(ClaimTypes.Email, etudiant.Email)
        }),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}
```

---

## **2. Page d'Inscription (Register)**

### **Logique Frontend (Blazor Server)**

#### **Rôle du Frontend** :
- Afficher un formulaire d'inscription.
- Envoyer les données d'inscription au backend.
- Gérer la réponse du backend (succès ou échec).
- Rediriger l'utilisateur vers la page de connexion en cas de succès.

#### **Étapes** :
1. **Formulaire d'inscription** :
    - Champs : nom, prénom, email, mot de passe, département, filière, niveau.
    - Un bouton "S'inscrire" pour soumettre le formulaire.
    - Un lien vers la page de connexion ("J'ai déjà un compte").

2. **Soumission du formulaire** :
    - Lorsque l'utilisateur clique sur "S'inscrire", le frontend envoie une requête HTTP POST au backend avec les données du formulaire.

3. **Gestion de la réponse** :
    - Si l'inscription réussit, afficher un message de succès et rediriger vers la page de connexion.
    - Si l'inscription échoue, afficher un message d'erreur (ex: "Cet email est déjà utilisé").

#### **Exemple de code Frontend (Blazor)** :
```razor
@page "/register"
@inject HttpClient Http
@inject NavigationManager Navigation

<h3>Inscription</h3>

<EditForm Model="@etudiant" OnValidSubmit="HandleRegister">
    <InputText @bind-Value="etudiant.Nom" placeholder="Nom" />
    <InputText @bind-Value="etudiant.Prenom" placeholder="Prénom" />
    <InputText @bind-Value="etudiant.Email" placeholder="Email" />
    <InputText @bind-Value="etudiant.MotDePasse" type="password" placeholder="Mot de passe" />
    <InputText @bind-Value="etudiant.Departement" placeholder="Département" />
    <InputText @bind-Value="etudiant.Filiere" placeholder="Filière" />
    <InputText @bind-Value="etudiant.Niveau" placeholder="Niveau" />
    <button type="submit">S'inscrire</button>
</EditForm>

<p><a href="/login">J'ai déjà un compte</a></p>

@code {
    private Etudiant etudiant = new Etudiant();

    private async Task HandleRegister()
    {
        var response = await Http.PostAsJsonAsync("api/etudiants/register", etudiant);

        if (response.IsSuccessStatusCode)
        {
            Navigation.NavigateTo("/login"); // Redirection vers la page de connexion
        }
        else
        {
            // Afficher un message d'erreur
        }
    }

    public class Etudiant
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public string Departement { get; set; }
        public string Filiere { get; set; }
        public string Niveau { get; set; }
    }
}
```

---

### **Logique Backend (ASP.NET Core)**

#### **Rôle du Backend** :
- Recevoir les données d'inscription.
- Vérifier si l'email est déjà utilisé.
- Hasher le mot de passe.
- Enregistrer l'étudiant dans la base de données.
- Renvoyer une réponse au frontend (succès ou échec).

#### **Étapes** :
1. **Endpoint** :
    - Créer un endpoint `POST /api/etudiants/register` pour gérer l'inscription.

2. **Validation** :
    - Vérifier si l'email existe déjà dans la base de données.

3. **Hash du mot de passe** :
    - Hasher le mot de passe avant de l'enregistrer en base de données.

4. **Enregistrement** :
    - Ajouter l'étudiant à la base de données.

5. **Réponse** :
    - Renvoyer un message de succès ou d'erreur.

#### **Exemple de code Backend (ASP.NET Core)** :
```csharp
[HttpPost("register")]
public async Task<ActionResult> Register(Etudiant etudiant)
{
    // Vérifier si l'email existe déjà
    if (_context.Etudiants.Any(e => e.Email == etudiant.Email))
    {
        return BadRequest("Cet email est déjà utilisé.");
    }

    // Hasher le mot de passe
    etudiant.MotDePasse = BCrypt.Net.BCrypt.HashPassword(etudiant.MotDePasse);

    // Ajouter l'étudiant à la base de données
    _context.Etudiants.Add(etudiant);
    await _context.SaveChangesAsync();

    return Ok("Inscription réussie.");
}
```

---

## **Résumé des responsabilités**

### **Frontend** :
- **Page de connexion** : Afficher le formulaire, envoyer les données, gérer la réponse, rediriger.
- **Page d'inscription** : Afficher le formulaire, envoyer les données, gérer la réponse, rediriger.

### **Backend** :
- **Connexion** : Valider les identifiants, générer un token JWT.
- **Inscription** : Valider les données, hasher le mot de passe, enregistrer l'étudiant.

---
