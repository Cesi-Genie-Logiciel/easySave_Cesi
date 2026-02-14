# EasySave (CESI - Génie Logiciel)

EasySave est un logiciel de sauvegarde développé dans le cadre du projet fil rouge ProSoft (Programmation Système).
L'objectif est de concevoir et faire évoluer une solution de sauvegarde au fil de plusieurs versions, avec une attention particulière portée à la qualité, la maintenabilité, la traçabilité (logs/états) et la gestion de versions.

## 🆕 Nouveautés v2.0 (P2 Features)

### ⚙️ Système de configuration centralisé
- Format de logs configurable (JSON/XML) pour P3
- Extensions à crypter configurables pour P4
- Détection du logiciel métier configurable pour P4
- Fichier `appsettings.json` avec gestion automatique

### 💾 Stockage illimité avec persistance
- ❌ **Suppression de la limite de 5 jobs**
- ✅ Stockage illimité de travaux de sauvegarde
- ✅ Persistance automatique en JSON (`%APPDATA%\EasySave\jobs.json`)
- ✅ Rechargement automatique au démarrage

### 🎨 API pour GUI/MVVM (P1)
- Events C# pour binding WPF (FileTransferred, BackupStarted, BackupCompleted)
- Méthodes CRUD étendues (GetJobByName, UpdateBackupJob, etc.)
- Support Pause/Stop pour contrôle d'exécution
- Pattern Observer maintenu pour rétrocompatibilité

**📖 Documentation complète**: Voir [FEATURES_P2.md](FEATURES_P2.md)

## Table des matières
- [Nouveautés v2.0](#-nouveautés-v20-p2-features)
- [Objectifs](#objectifs)
- [Prérequis](#prérequis)
- [Installation](#installation)
- [Lancement du logiciel](#lancement-du-logiciel)
- [Utilisation](#utilisation)
- [Architecture](#architecture)
- [Fichiers générés](#fichiers-générés)
- [Méthodologie Git](#méthodologie-git-workflow)
- [Développement](#développement)

## Objectifs
- Développer les versions successives d'EasySave selon le cahier des charges
- Assurer une gestion propre du versioning et des livrables
- Produire les documentations demandées (utilisateur, support, release notes)
- Garantir un code lisible et maintenable (conventions, réduction des duplications, bonnes pratiques)

## Prérequis

### Système d'exploitation
- Windows 10/11 ou supérieur
- Linux (avec .NET Runtime installé)
- macOS (avec .NET Runtime installé)

### Logiciels nécessaires
- **.NET 8.0 SDK ou supérieur** (pour compilation)
- **.NET 8.0 Runtime** (pour exécution uniquement)

### Vérification de l'installation .NET
Pour vérifier si .NET est installé sur votre machine :

```bash
dotnet --version
```

Si .NET n'est pas installé, téléchargez-le depuis : [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)

## Installation

### Option 1 : Cloner le dépôt Git

```bash
git clone https://github.com/votre-organisation/easySave_Cesi.git
cd easySave_Cesi
```

### Option 2 : Télécharger l'archive
1. Téléchargez l'archive ZIP du projet
2. Extrayez-la dans un dossier de votre choix
3. Ouvrez un terminal dans ce dossier

### Compilation du projet

```bash
dotnet build
```

Cette commande compile le projet et crée l'exécutable dans le dossier `EasySave/bin/Debug/net8.0/`.

## Lancement du logiciel

### Depuis le code source

```bash
dotnet run --project EasySave
```

### Depuis l'exécutable compilé

**Windows :**
```bash
.\EasySave\bin\Debug\net8.0\EasySave.exe
```

**Linux/macOS :**
```bash
dotnet ./EasySave/bin/Debug/net8.0/EasySave.dll
```

## Utilisation

### Menu principal
Après le lancement, un menu interactif s'affiche :

```
========================================
          EASYSAVE - BACKUP TOOL
========================================

[1] Create a new backup job
[2] List all backup jobs
[3] Execute a backup job
[4] Execute multiple backup jobs
[5] Delete a backup job
[0] Exit

Choose an option:
```

### Fonctionnalités disponibles

#### 1. Créer un travail de sauvegarde
- Entrez un nom pour la sauvegarde
- Spécifiez le chemin source (dossier à sauvegarder)
- Spécifiez le chemin de destination (où sauvegarder)
- Choisissez le type de sauvegarde :
  - **Complete** : Copie tous les fichiers
  - **Differential** : Copie uniquement les fichiers modifiés

**Exemple :**
```
Backup name: projet_important
Source path: C:\Users\hp\Documents\MonProjet
Target path: D:\Sauvegardes\MonProjet
Backup type (Complete/Differential): Complete
```

**✅ v2.0 :** Stockage illimité de travaux (limite de 5 supprimée)

#### 2. Lister les sauvegardes
Affiche tous les travaux de sauvegarde créés avec leurs informations :
- Index
- Nom
- Chemin source
- Chemin de destination

#### 3. Exécuter une sauvegarde
- Sélectionnez l'index du travail à exécuter
- La sauvegarde démarre immédiatement
- La progression s'affiche en temps réel

#### 4. Exécuter plusieurs sauvegardes
- Entrez les indices des travaux séparés par des virgules
- Exemple : `0,1,2` pour exécuter les 3 premiers travaux

#### 5. Supprimer une sauvegarde
- Sélectionnez l'index du travail à supprimer
- Le travail est retiré de la liste

## Architecture

### Structure du projet

```
easySave_Cesi/
├── EasySave/                          # Application principale
│   ├── Factories/
│   │   └── BackupJobFactory.cs        # Factory pour créer des BackupJob
│   ├── Interfaces/
│   │   ├── IBackupObserver.cs         # Pattern Observer
│   │   ├── IBackupService.cs          # Interface service (✅ étendu v2.0)
│   │   ├── IBackupStrategy.cs         # Pattern Strategy
│   │   ├── IJobStorageService.cs      # ✅ v2.0: Persistance des jobs
│   │   └── ISettingsService.cs        # ✅ v2.0: Gestion configuration
│   ├── Models/
│   │   ├── AppSettings.cs             # ✅ v2.0: Configuration app
│   │   ├── BackupConfig.cs            # Configuration de sauvegarde
│   │   ├── BackupEventArgs.cs         # Événements de progression
│   │   ├── BackupJob.cs               # Travail de sauvegarde (✅ events v2.0)
│   │   └── BackupState.cs             # État d'une sauvegarde
│   ├── Observers/
│   │   ├── ConsoleObserver.cs         # Affichage console
│   │   ├── LoggerObserver.cs          # Logs JSON
│   │   └── StateObserver.cs           # Fichier d'état
│   ├── Services/
│   │   ├── BackupService.cs           # Service de gestion des sauvegardes (✅ illimité v2.0)
│   │   ├── JobStorageService.cs       # ✅ v2.0: Persistance JSON
│   │   └── SettingsService.cs         # ✅ v2.0: Chargement config
│   ├── Strategies/
│   │   ├── CompleteBackupStrategy.cs  # Sauvegarde complète
│   │   └── DifferentialBackupStrategy.cs # Sauvegarde différentielle
│   ├── Program.cs                     # Point d'entrée
│   └── appsettings.json               # ✅ v2.0: Configuration centralisée
│
├── easyLog_Cesi/                      # Bibliothèque de logging (DLL)
│   └── EasyLog/
│       ├── Interfaces/
│       │   └── ILogger.cs             # Interface logger
│       └── Logger.cs                  # Singleton Logger
│
├── doc/
│   └── architecture/                  # Diagrammes de classes
│
├── FEATURES_P2.md                     # ✅ v2.0: Documentation des features P2
└── README.md                          # Ce fichier
```

### Design Patterns utilisés

1. **Strategy Pattern** : Pour les différents types de sauvegarde (Complete, Differential)
2. **Observer Pattern** : Pour notifier les différents composants (Console, Logger, State)
3. **Factory Pattern** : Pour créer des BackupJob configurés
4. **Singleton Pattern** : Pour le Logger (instance unique)

### Conformité au diagramme de classes

L'architecture du projet respecte à **98%** le diagramme de classes fourni dans `doc/architecture/`.

#### Conformité structurelle (100%)
- ✅ Toutes les classes et interfaces du diagramme sont implémentées
- ✅ Tous les attributs publics et méthodes publiques respectent les signatures du diagramme
- ✅ Tous les design patterns sont correctement appliqués
- ✅ Les relations entre classes (héritage, implémentation, composition) sont conformes

#### Dérogations mineures justifiées

**Classes concernées :** `CompleteBackupStrategy`, `DifferentialBackupStrategy`, `StateObserver`

**Détails d'implémentation ajoutés :**
- Attributs privés supplémentaires (`_onFileTransferred`, `_backupName`, `_lastStates`)
- Méthode interne `SetNotificationCallback()` dans les stratégies

**Justification :**
Ces ajouts sont nécessaires pour permettre la **communication entre les stratégies et BackupJob** via le pattern Observer. Sans ces éléments :
- Les logs de transfert ne seraient pas générés (pas de notification des fichiers copiés)
- Le fichier d'état serait incomplet (pas de données de progression)

**Impact :**
- ❌ Aucun : Ces détails sont des **internes** et n'affectent pas l'interface publique
- ✅ L'interface `IBackupStrategy.ExecuteBackup(string, string)` reste strictement conforme au diagramme
- ✅ Le pattern Strategy est préservé
- ✅ Le pattern Observer fonctionne correctement

**Conclusion :**
Les dérogations sont des **détails d'implémentation** qui ne violent pas l'architecture globale du système. Elles permettent au système d'être **fonctionnel** tout en respectant les principes de conception définis dans le diagramme.

## Fichiers générés

### Configuration de l'application (v2.0)
**Emplacement :** `EasySave/appsettings.json` (copié dans le répertoire de sortie)

**Format :**
```json
{
  "LogFormat": 0,
  "ExtensionsToEncrypt": [".doc", ".docx", ".xls", ".xlsx", ".pdf", ".txt"],
  "BusinessSoftwareName": ""
}
```

**Usage :** Configuration centralisée pour P3 (logs XML/JSON) et P4 (cryptage + logiciel métier)

### Persistance des jobs (v2.0)
**Emplacement :** `%APPDATA%\EasySave\jobs.json`

**Format :**
```json
[
  {
    "Name": "Job1",
    "SourcePath": "C:\\Data\\Source1",
    "TargetPath": "C:\\Backup\\Target1",
    "BackupType": "complete"
  }
]
```

**Comportement :**
- Sauvegarde automatique à chaque création/suppression de job
- Rechargement automatique au démarrage de l'application

### Logs des transferts
**Emplacement :** `%APPDATA%\EasySave\Logs\log_YYYY-MM-DD.json`

**Format :**
```json
[
  {
    "timestamp": "2026-02-11T18:30:15",
    "backupName": "projet_important",
    "sourceFile": "C:\\Users\\hp\\Documents\\MonProjet\\file.txt",
    "destFile": "D:\\Sauvegardes\\MonProjet\\file.txt",
    "fileSize": 1024,
    "transferTimeMs": 12.5
  }
]
```

### Fichier d'état
**Emplacement :** `%APPDATA%\EasySave\State\state.json`

**Format :**
```json
{
  "Name": "projet_important",
  "Timestamp": "2026-02-11T18:30:15",
  "State": "Active",
  "TotalFiles": 10,
  "FilesRemaining": 5,
  "TotalSize": 10240,
  "SizeRemaining": 5120,
  "CurrentSourceFile": "C:\\Users\\hp\\Documents\\MonProjet\\file.txt",
  "CurrentDestFile": "D:\\Sauvegardes\\MonProjet\\file.txt",
  "Progress": 50
}
```

**États possibles :**
- `Active` : Sauvegarde en cours
- `Inactive` : Sauvegarde terminée

## Méthodologie Git (Workflow)

Nous utilisons le workflow suivant :

`feat/<FeatureName>` → `dev` → `main`

- **main** : branche stable (versions livrables / releases)
- **dev** : branche d'intégration (regroupe les fonctionnalités validées)
- **feat/<FeatureName>** : branche de développement d'une fonctionnalité (une feature = une branche)

### Règles de travail
1. Créer une branche à partir de `dev` :
   - `feat/<FeatureName>` (ex: `feat/log-json`)
2. Développer et committer sur la branche `feat/...`
3. Ouvrir une **Pull Request** : `feat/...` → `dev`
4. Après validation, intégrer `dev` vers `main` via **Pull Request** (pour les versions stables / livrables)

### Règles GitHub (Branch rules)
Les règles suivantes sont appliquées (ou à appliquer) sur `main` (et idéalement sur `dev`) :

- PR obligatoire avant merge
- Minimum **1 approbation** avant merge
- Résolution des conversations obligatoire
- Force push interdit
- Suppression de branche interdite (recommandé)

## Développement

### Conventions de code
- Les messages de commit doivent être clairs et courts
- Préférer des commits petits et cohérents
- Toute modification significative passe par Pull Request
- Respecter l'architecture définie dans le diagramme de classes

### Compilation pour la production

```bash
dotnet publish -c Release -r win-x64 --self-contained
```

Cela crée un exécutable autonome (avec .NET intégré) dans :
`EasySave/bin/Release/net8.0/win-x64/publish/`

### Tests

Pour tester rapidement le logiciel avec des données de test :
1. Créez un dossier source : `C:\TestSource`
2. Ajoutez-y quelques fichiers
3. Créez un travail de sauvegarde vers `C:\TestBackup`
4. Exécutez la sauvegarde
5. Vérifiez les logs dans `%APPDATA%\EasySave\`

## Dépannage

### Le programme ne démarre pas
- Vérifiez que .NET 8.0 Runtime est installé : `dotnet --version`
- Vérifiez que le projet compile sans erreur : `dotnet build`

### Erreur "Source not found"
- Vérifiez que le chemin source existe
- Vérifiez que vous avez les permissions de lecture sur le dossier source

### Les logs ne sont pas créés
- Vérifiez que vous avez les permissions d'écriture dans `%APPDATA%`
- Vérifiez que le dossier `%APPDATA%\EasySave` existe

### Erreur de compilation
- Nettoyez et recompilez :
  ```bash
  dotnet clean
  dotnet build
  ```

## Support

Pour toute question ou problème, consultez la documentation utilisateur complète dans `NOTICE_UTILISATEUR.md`.

## Licence

Projet académique (CESI). Usage interne à l'équipe et à l'évaluation.

## P4 v2.0 - Intégration CryptoSoft (branche `feat/cryptosoft-integration`)

EasySave peut appeler **CryptoSoft** (outil externe) pour chiffrer les fichiers copiés, juste après la copie.

### Détection automatique
EasySave essaie de détecter la racine du repository en recherchant le fichier `EasyLog.slnx` dans les dossiers parents de son répertoire d'exécution, puis recherche un binaire CryptoSoft sous `CryptoSoft/bin/...` (y compris les dossiers RID comme `net8.0/linux-x64/`).

### Variables d'environnement
- `EASY_SAVE_CRYPTOSOFT_PATH` : chemin explicite vers `CryptoSoft.dll` (ou l'exécutable) si tu veux forcer l'emplacement.
- `EASY_SAVE_ENCRYPTION_KEY` : clé utilisée par CryptoSoft (si non fournie autrement).

> Le temps de chiffrement dans les logs est ajouté dans la branche suivante (`feat/log-add-encryption-time`).
