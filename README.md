<div align="center">

# 🛒 Olympia — API Backend

### API REST de e-commerce B2B & prise de commande (ASP.NET Core 6)

Back-end commun au **storefront client** et à l'espace **commerciaux (VRP)**. Gère le catalogue articles, la **tarification par client avec remises**, la prise de commande, le suivi des commandes/règlements et les **notifications SMS**, le tout connecté à la base **ERP DIVA / OLYMPIA** (SQL Server).

[![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/aspnet/core/)
[![EF Core](https://img.shields.io/badge/EF%20Core-6-512BD4)](https://learn.microsoft.com/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Swagger](https://img.shields.io/badge/Docs-Swagger-85EA2D?logo=swagger&logoColor=black)](https://swagger.io/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

</div>

---

## 📋 Table des matières

- [Présentation](#-présentation)
- [Architecture](#-architecture)
- [Fonctionnalités](#-fonctionnalités)
- [Technologies utilisées](#-technologies-utilisées)
- [Endpoints de l'API](#-endpoints-de-lapi)
- [Logique métier : tarification & commande](#-logique-métier--tarification--commande)
- [Structure du projet](#-structure-du-projet)
- [Installation](#-installation)
- [Configuration](#-configuration)
- [Sécurité](#-sécurité)
- [Écosystème du projet](#-écosystème-du-projet)
- [Améliorations futures](#-améliorations-futures)
- [Auteur](#-auteur)
- [Licence](#-licence)

---

## 🎯 Présentation

**Olympia API** est le cœur applicatif d'une solution de **vente B2B en ligne**. Elle fait le **pont entre le front commercial** (catalogue, panier, prise de commande) et le **système ERP DIVA** (articles, tarifs, clients, règlements) hébergé sur SQL Server.

Le principe métier central est la **tarification contextuelle** : un même article n'a pas le même prix selon le client, son barème et ses remises. L'API résout le bon tarif, applique les remises, puis transforme le panier en **commande** persistée et notifiée par **SMS**.

| | |
|---|---|
| **Objectif** | Exposer le catalogue ERP et permettre la prise de commande B2B avec tarification par client |
| **Consommateurs** | Storefront client (Angular) + espace commerciaux/VRP |
| **Type** | API REST documentée via Swagger / OpenAPI |
| **Base de données** | SQL Server — base `OLYMPIA` (schéma ERP `ERP210OLYMPIA_F`) |

> 🔒 **Note** : ce dépôt est une version *portfolio*. Les secrets réels (chaîne de connexion SQL, clé API SMS) ont été retirés du suivi Git et remplacés par un modèle — voir [Configuration](#-configuration).

---

## 🏗 Architecture

```
┌─────────────────┐     ┌─────────────────┐
│  Storefront     │     │  Espace VRP     │
│  client (Angular)│     │  (commerciaux)  │
└────────┬────────┘     └────────┬────────┘
         │       HTTP / CORS     │
         └───────────┬───────────┘
                     ▼
        ┌──────────────────────────┐
        │   API ASP.NET Core 6     │
        │ Controllers · DTO · CORS │
        └────────────┬─────────────┘
                     │  Repositories (Scoped)
                     ▼
        ┌──────────────────────────┐
        │   EF Core 6 (DbContext)  │   ──►  SMS (WinSmsPro)
        │   Database-First         │
        └────────────┬─────────────┘
                     ▼
        ┌──────────────────────────┐
        │      SQL Server          │
        │  Base ERP « OLYMPIA »    │
        └──────────────────────────┘
```

L'API suit une **architecture en couches** avec injection de dépendances :

```
Controllers  →  Interfaces  →  Repositories  →  DbContext  →  SQL Server
                                   ▲
                              Services (SmsService)
```

- **Approche Database-First** : le `DbContext` (`ERP210OLYMPIA_FContext`) et les entités ERP sont **scaffoldés** depuis la base existante via *EF Core Power Tools* (`efpt.config.json`).
- **Pattern Repository** : chaque domaine (articles, tarifs, remises, auth, invités) est isolé derrière une interface (`IArtRepository`, `ITarRepository`…) enregistrée en `Scoped`, gardant les contrôleurs fins et testables.
- **Front servi en fallback** : un `FallbackController` sert le SPA Angular depuis `wwwroot/` pour toute route non-API.

---

## ✨ Fonctionnalités

- 🛍️ **Catalogue articles** : listing, détail, recherche par référence, regroupement par **famille** et par **dossier** (`dos`).
- 💰 **Tarification par client** : résolution du tarif applicable selon la référence client et son barème (`Tar`, `TarController`).
- 🎯 **Remises** : application des remises par couple `référence / code remise`, et tarifs « nets » combinés (`TarWithRemise`).
- 🛒 **Prise de commande** : transformation d'un panier (`CommandeDto`) en commande persistée avec ses lignes (`Commande` + `CommandeArticle`).
- 📦 **Suivi des commandes** : consultation par commande, par client, par contact ou par commercial, détail des articles et **règlements**.
- 👥 **Acteurs multiples** : clients (`tiers`/contacts), **commerciaux (VRP)** et **invités** (`Guest`), chacun avec ses propres vues.
- 🔐 **Authentification multi-rôles** : connexion dédiée client, contact et commercial, gestion des rôles et changement de mot de passe.
- 📲 **Notifications SMS** : envoi de confirmations via la passerelle **WinSmsPro** (`SmsService` sur `HttpClientFactory`).
- 🏠 **Carnet d'adresses** : enregistrement des adresses de livraison/facturation.
- 🌐 **Fallback SPA** + **Swagger/OpenAPI** activé en développement.

---

## 🛠 Technologies utilisées

| Technologie | Usage |
|-------------|-------|
| **.NET 6 / ASP.NET Core** | Framework de l'API REST |
| **C# 10** | Langage (nullable + implicit usings activés) |
| **Entity Framework Core 6** | ORM (Database-First, SQL Server) |
| **SQL Server** | Base de données ERP (`OLYMPIA`) |
| **EF Core Power Tools** | Scaffolding du `DbContext` et des entités (`efpt.config.json`) |
| **Swashbuckle (Swagger)** | Documentation interactive de l'API |
| **HttpClientFactory** | Client HTTP typé pour la passerelle SMS |
| **WinSmsPro** | Passerelle d'envoi de SMS |

---

## 📡 Endpoints de l'API

Toutes les routes utilisent le préfixe `api/[controller]`.

### 🛍️ `api/Art` — Catalogue & commandes
| Méthode | Route | Description |
|---------|-------|-------------|
| `GET` | `/` · `/{id}` | Liste / détail des articles |
| `GET` | `/famille/{dos}` · `/fam` | Articles par dossier / familles |
| `GET` | `/sref1` · `/sref2/{reference}` | Sous-références |
| `GET` | `/tarif` | Tarif d'un article |
| `POST` | `/commande` | Passer une commande (`CommandeDto`) |
| `GET` | `/commande/{commandeId}` · `/commande/{commandeId}/articles` | Détail d'une commande / ses lignes |
| `GET` | `/client/{tiers}/commandes` | Commandes d'un client |
| `GET` | `/contact/{contactId}/commandes` | Commandes d'un contact |
| `GET` | `/commercial/{reference}/commandes` | Commandes d'un commercial |
| `GET` | `/list/commandes` · `/list/comdart` · `/list/reglement` | Listes globales : commandes, lignes, règlements |

### 🔐 `api/Auth` — Authentification
| Méthode | Route | Description |
|---------|-------|-------------|
| `POST` | `/login` · `/login/comm` · `/login/contact` | Connexion client / commercial / contact |
| `POST` | `/` | Création d'un compte client |
| `GET` | `/` · `/{username}` | Liste / détail des comptes |
| `GET` | `/RoleContact` | Rôles des contacts |
| `PUT` | `/change-password` | Changement de mot de passe |

### 💰 `api/Tar` — Tarifs
| Méthode | Route | Description |
|---------|-------|-------------|
| `GET` | `/` · `/{id}` | Liste / détail des tarifs |
| `GET` | `/list/{reference}` | Tarifs d'une référence |
| `GET` | `/famille` · `/tarifs/{reference}` | Tarifs par famille / par référence |
| `GET` | `/image` | Ressource image associée |

### 🎯 `api/Remise` · `api/TarWithRemise` — Remises & tarifs nets
| Méthode | Route | Description |
|---------|-------|-------------|
| `GET` | `Remise/` · `Remise/{reference}/{remcod}` | Remises (toutes / par client + code) |
| `GET` | `TarWithRemise/{reference}` · `/{reference}/{remcod}` | Tarif net remisé par client |

### 👥 `api/Cli` — Clients, contacts & commerciaux
| Méthode | Route | Description |
|---------|-------|-------------|
| `GET` | `/` · `/{id}` · `/tiers/{tiers}` | Clients (liste / id / tiers) |
| `GET` | `/client/comm/{reference}` · `/client/contact/{tiers}` | Clients par commercial / contact |
| `GET` | `/commercial` · `/commercial/{id}` | Commerciaux (VRP) |
| `GET` | `/contact` · `/contact/{id}` | Contacts |

### 🧑‍💼 `api/Guest` · 🏠 `api/Adresse`
Gestion des invités (`GET`, `POST`, `DELETE /{id}`) et enregistrement des adresses (`POST`).

---

## ⚖️ Logique métier : tarification & commande

Le prix d'un article dans Olympia n'est **pas un attribut fixe** : il dépend du client et de ses conditions commerciales.

| Élément | Rôle |
|---------|------|
| `Art` | Article du catalogue (données ERP) |
| `Tar` | Barème de prix applicable à une référence |
| `Remise` | Remise liée à un couple `référence / code remise` (`remcod`) |
| `TarWithRemise` | Tarif **net** = barème after application de la remise client |
| `Commande` / `CommandeArticle` | Commande validée et ses lignes |

**Flux de prise de commande** (`POST api/Art/commande`) :

1. Le front envoie un `CommandeDto` (client + lignes panier).
2. L'API résout, pour chaque ligne, le **tarif** puis la **remise** propres au client (`TarWithRemise`).
3. Une `Commande` est créée et ses `CommandeArticle` persistés en base ERP.
4. Une **confirmation SMS** est déclenchée via `SmsService` (WinSmsPro).
5. La commande devient consultable par le client, son contact et son commercial via les endpoints `…/commandes`.

---

## 📁 Structure du projet

```
Olympia-nouveau/
├── Olympia.sln
├── .gitignore
└── Olympia/
    ├── Controllers/          # Art, Auth, Tar, TarWithRemise, Remise, Cli, Guest, Adresse, Fallback
    ├── Interfaces/           # Contrats des repositories (IArtRepository, ITarRepository…)
    ├── Repository/           # Implémentations EF Core des repositories
    ├── Services/             # SmsService (passerelle WinSmsPro)
    ├── Models/               # Entités EF (ERP + app) + DTO + ERP210OLYMPIA_FContext (scaffoldé)
    ├── wwwroot/              # Front Angular compilé, servi en fallback
    ├── Program.cs            # Bootstrap : EF, CORS, Swagger, DI
    ├── efpt.config.json      # Config EF Core Power Tools (scaffolding)
    ├── appsettings.json      # ⚠️ Config & secrets (git-ignoré)
    └── appsettings.Example.json  # Modèle versionné
```

> ⚠️ Le fichier `Models/ERP210OLYMPIA_FContext.cs` et la plupart des entités (`Art`, `Tar`, `T0xx`, `VRP`…) sont **auto-générés** par EF Core Power Tools depuis la base. Les **régénérer** plutôt que les modifier à la main.

---

## 🚀 Installation

### Prérequis

- **.NET 6 SDK**
- **SQL Server** (accès à la base `OLYMPIA` ou équivalente)
- *(optionnel)* **EF Core Power Tools** pour re-scaffolder le modèle

### Étapes

```bash
# 1. Cloner le dépôt
git clone https://github.com/<votre-compte>/olympia-api.git
cd olympia-api

# 2. Configurer les secrets (voir section suivante)
cp Olympia/appsettings.Example.json Olympia/appsettings.json
# puis éditer la chaîne de connexion et la clé SMS

# 3. Restaurer & lancer
dotnet restore
dotnet run --project Olympia
```

L'API démarre sur `https://localhost:7166` (et `http://localhost:5040`). En développement, **Swagger UI** est disponible sur `/swagger`.

```bash
# Build de production
dotnet build -c Release

# Publication
dotnet publish Olympia -c Release
```

---

## ⚙️ Configuration

La configuration se trouve dans `Olympia/appsettings.json` :

```jsonc
{
  "WinSms": {
    "ApiKey": "<clé API WinSmsPro>",
    "SenderId": "OLYMPIA"
  },
  "ConnectionStrings": {
    "DataBase": "Data Source=SERVER\\INSTANCE;Initial Catalog=OLYMPIA;User ID=...;Password=...;TrustServerCertificate=True;Connect Timeout=120"
  },
  "AllowedHosts": "*"
}
```

> ⚠️ `appsettings.json` et `appsettings.*.json` sont **ignorés par Git** (`.gitignore`) car ils contiennent la chaîne de connexion SQL et la clé API SMS. Copiez `appsettings.Example.json` après chaque clone. Pour le développement local, privilégiez les **User Secrets** (`dotnet user-secrets`) ou des variables d'environnement.
>
> ℹ️ La clé de connexion s'appelle **`DataBase`** (et non `DefaultConnection`).

---

## 🔐 Sécurité

- **Secrets hors du dépôt** via `.gitignore` + modèle d'exemple versionné.
- **Authentification multi-rôles** dédiée (client / contact / commercial).
- **Connexion SQL** chiffrée côté transport (`TrustServerCertificate`).

**Pistes de durcissement** (voir [Améliorations futures](#-améliorations-futures)) : restreindre la politique **CORS** (actuellement `AllowAnyOrigin` + `"*"`), introduire des **tokens JWT** et `[Authorize]` sur les endpoints sensibles, **hacher** les mots de passe si ce n'est pas déjà le cas côté ERP, et externaliser totalement les secrets en production.

---

## 🔗 Écosystème du projet

| Composant | Rôle | Techno |
|-----------|------|--------|
| **Olympia API** *(ce dépôt)* | Back-end REST & logique métier | ASP.NET Core 6 |
| **Storefront Olympia** | Site client (catalogue, panier, commande) | Angular (servi depuis `wwwroot/`) |
| **ERP DIVA / OLYMPIA** | Source des données (articles, tarifs, clients) | SQL Server |
| **WinSmsPro** | Passerelle de notifications SMS | API externe |

---


[![GitHub](https://img.shields.io/badge/GitHub-181717?logo=github&logoColor=white)](https://github.com/votre-compte)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0A66C2?logo=linkedin&logoColor=white)](https://linkedin.com/in/votre-profil)
[![Email](https://img.shields.io/badge/Email-D14836?logo=gmail&logoColor=white)](mailto:vous@exemple.com)

---

## 📄 Licence

Distribué sous licence **MIT**. Voir le fichier [`LICENSE`](LICENSE) pour plus d'informations.

<div align="center">

⭐️ Si ce projet vous a plu, n'hésitez pas à laisser une étoile !

</div>
