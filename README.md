# EasySave (CESI - Génie Logiciel)

EasySave est un logiciel de sauvegarde développé dans le cadre du projet fil rouge ProSoft (Programmation Système).
L'objectif est de concevoir et faire évoluer une solution de sauvegarde au fil de plusieurs versions, avec une attention particulière portée à la qualité, la maintenabilité, la traçabilité (logs/états) et la gestion de versions.

## Table des matières
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

**Limite :** Maximum 5 travaux de sauvegarde simultanés

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
│   │   ├── IBackupService.cs          # Interface service
│   │   └── IBackupStrategy.cs         # Pattern Strategy
│   ├── Models/
│   │   ├── BackupConfig.cs            # Configuration de sauvegarde
│   │   ├── BackupEventArgs.cs         # Événements de progression
│   │   ├── BackupJob.cs               # Travail de sauvegarde
│   │   └── BackupState.cs             # État d'une sauvegarde
│   ├── Observers/
│   │   ├── ConsoleObserver.cs         # Affichage console
│   │   ├── LoggerObserver.cs          # Logs JSON
│   │   └── StateObserver.cs           # Fichier d'état
│   ├── Services/
│   │   └── BackupService.cs           # Service de gestion des sauvegardes
│   ├── Strategies/
│   │   ├── CompleteBackupStrategy.cs  # Sauvegarde complète
│   │   └── DifferentialBackupStrategy.cs # Sauvegarde différentielle
│   └── Program.cs                     # Point d'entrée
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
└── README.md                          # Ce fichier
```

### Design Patterns utilisés

1. **Strategy Pattern** : Pour les différents types de sauvegarde (Complete, Differential)
2. **Observer Pattern** : Pour notifier les différents composants (Console, Logger, State)
3. **Factory Pattern** : Pour créer des BackupJob configurés
4. **Singleton Pattern** : Pour le Logger (instance unique)

## Fichiers générés

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
