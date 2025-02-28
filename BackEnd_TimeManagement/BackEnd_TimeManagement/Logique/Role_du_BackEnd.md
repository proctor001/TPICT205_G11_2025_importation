---

### **Rôle du Backend**
Le backend a pour rôle de :
1. **Stocker les données** des étudiants dans une base de données MySQL gérée avec Entity Framework Core.
2. **Valider les données** reçues du frontend (emails, mots de passe, etc.).
3. **Authentifier les utilisateurs** (vérifier les identifiants de connexion).
4. **Gérer les sessions** via des tokens JWT pour maintenir l'utilisateur connecté.
5. **Renvoie des réponses structurées** (token JWT après une connexion réussie ou message d'erreur en cas d'échec).

---

### **Base de Données**
- Une base de données MySQL nommée **`TP205_TimeSchool_Etudiants`** a déjà été créée.
- Vous devez modifier les informations de connexion à la base de données dans le fichier **`appsettings.json`** pour qu'elles correspondent à votre environnement.

---

C'est court, précis et facile à comprendre !