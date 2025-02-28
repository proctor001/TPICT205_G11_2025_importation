### **Instructions pour la Liaison Backend + Frontend (Connexions)**

Chacun va se concentrer sur la **liaison entre le Backend et le Frontend** en ce qui concerne les fonctionnalités de **connexion** et **d'inscription**. Voici les points à respecter :

---

#### **1. Respect du Modèle Backend**

- Le modèle d'étudiant dans le backend doit être strictement respecté. Chaque étudiant est caractérisé par les propriétés suivantes :
    - **Nom**
    - **Prénom**
    - **Email**
    - **Mot de passe** (doit être hashé avant d'être stocké)
    - **Département**
    - **Filière**
    - **Niveau**
    - **Rôle** (utilisé pour la redirection après la connexion)

- Le rôle est essentiel pour déterminer où rediriger l'utilisateur après la connexion (ex: étudiant, enseignant, administrateur, etc.).

---

#### **2. Flux à Respecter**

- **Inscription** :
    1. L'étudiant remplit le formulaire d'inscription.
    2. Les données sont envoyées au backend.
    3. Si l'inscription réussit, l'étudiant est redirigé vers la **page de login**.

- **Connexion** :
    1. L'étudiant remplit le formulaire de connexion (email et mot de passe).
    2. Les données sont envoyées au backend.
    3. Si la connexion réussit, l'étudiant est redirigé vers son **dashboard** en fonction de son rôle.

---

#### **3. Liberté sur l'Implémentation**

- Vous êtes libres d'implémenter le **frontend** et le **backend** comme vous le souhaitez, **mais** :
    - Le **fond** doit rester le même :
        - **Frontend** : Le style CSS et le thème doivent être respectés (couleurs, polices, disposition, etc.).
        - **Backend** : Les modèles (classes) et la logique de base (hashing, validation, génération de token JWT) doivent être respectés.

---

#### **4. Liens des Pages**

- Les liens des pages à implémenter sont les suivants :
    - **Page d'inscription** : `/inscriptionEtudiant`
    - **Page de connexion** : `/login`
    - **Tableau de bord (dashboard)** : `/dashboard` (ou autre en fonction du rôle).

  Vous allez pouvoir accéder à 5 pages principales

    - <http://localhost:5202/>
    - <http://localhost:5202/login>
    - <http://localhost:5202/inscriptionEtudiant>
    - <http://localhost:5202/DashboardEtudiant>
    - <http://localhost:5202/DashboardEnseignant>.

---

#### **5. Rôle de l'Étudiant**

- Le **rôle** de l'étudiant est crucial pour la redirection après la connexion. Par exemple :
    - Si le rôle est **étudiant**, rediriger vers `/DashboardEtudiant`.
    - Si le rôle est **admin**, rediriger vers `/DashboardEnseignant`.

---

#### **6. Exemple de Modèle Backend**

Je vous enverrai le modèle d'étudiant à respecter !

#### **7. Exemple de Flux Frontend**

- **Inscription** :
    - Formulaire → Envoi des données → Redirection vers `/login`.
- **Connexion** :
    - Formulaire → Envoi des données → Redirection vers `/dashboard` (ou autre en fonction du rôle).

---

#### **8. Points Importants**

- **Backend** :
    - Valider les données (email unique, mot de passe hashé).
    - Générer un token JWT après une connexion réussie.
    - Utiliser le rôle pour déterminer la redirection.

- **Frontend** :
    - Respecter le style CSS et le thème.
    - Gérer les redirections après l'inscription et la connexion.
    - Stocker le token JWT (dans le localStorage ou un service Blazor).

---

### **Résumé des Responsabilités**

- **Backend** : Valider, hasher, générer des tokens, gérer les rôles.
- **Frontend** : Afficher les formulaires, envoyer les données, gérer les redirections, respecter le style.

---
