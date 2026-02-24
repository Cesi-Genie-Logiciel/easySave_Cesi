# 📘 EasySave v2.0 - Manuel Utilisateur

<div align="center">
  <h2>Guide complet d'utilisation</h2>
  <p>Version 2.0 - Février 2026</p>
</div>

---

## 📋 Table des matières

1. [Introduction](#introduction)
2. [Installation](#installation)
3. [Premier démarrage](#premier-démarrage)
4. [Interface principale](#interface-principale)
5. [Créer un job de sauvegarde](#créer-un-job-de-sauvegarde)
6. [Exécuter une sauvegarde](#exécuter-une-sauvegarde)
7. [Gérer vos jobs](#gérer-vos-jobs)
8. [Configuration avancée](#configuration-avancée)
9. [Cryptage des fichiers](#cryptage-des-fichiers)
10. [Questions fréquentes](#questions-fréquentes)
11. [Dépannage](#dépannage)

---

## 🎯 Introduction

### Qu'est-ce qu'EasySave ?

EasySave est une application de sauvegarde de fichiers simple et efficace. Elle vous permet de :

- ✅ **Sauvegarder** vos documents importants
- ✅ **Protéger** vos données avec le cryptage
- ✅ **Automatiser** vos sauvegardes
- ✅ **Suivre** la progression en temps réel

### À qui s'adresse ce logiciel ?

- 👤 **Particuliers** souhaitant protéger leurs documents personnels
- 🏢 **Petites entreprises** nécessitant une solution de sauvegarde simple
- 💼 **Professionnels** ayant besoin de sauvegardes régulières

---

## 💾 Installation

### Prérequis

Avant d'installer EasySave, assurez-vous d'avoir :

- **Windows 10 ou 11** (version 64-bit)
- **Au moins 50 MB** d'espace disque libre
- **Droits d'administrateur** pour l'installation

### Installation de .NET 8.0

EasySave nécessite .NET 8.0 pour fonctionner.

**Vérifier si .NET est déjà installé :**

1. Ouvrir le menu Démarrer
2. Taper `cmd` et appuyer sur Entrée
3. Taper : `dotnet --version`
4. Si vous voyez `8.0.x`, .NET est installé ✅

**Installer .NET 8.0 si nécessaire :**

1. Visiter : https://dotnet.microsoft.com/download/dotnet/8.0
2. Télécharger ".NET Desktop Runtime 8.0"
3. Exécuter le fichier téléchargé
4. Suivre les instructions d'installation

### Installation d'EasySave

1. **Télécharger** le fichier `EasySave_v2.0_Release.zip`

2. **Extraire** le contenu
   - Clic droit sur le fichier ZIP
   - Choisir "Extraire tout..."
   - Sélectionner un emplacement (ex: `C:\Program Files\EasySave`)

3. **Vérifier** le contenu extrait
   ```
   EasySave/
   ├── EasySave.GUI.exe    ← Programme principal
   ├── EasySave.dll
   ├── EasyLog.dll
   └── ...autres fichiers
   ```

4. **Créer un raccourci** (optionnel)
   - Clic droit sur `EasySave.GUI.exe`
   - Choisir "Créer un raccourci"
   - Déplacer le raccourci sur le Bureau

---

## 🚀 Premier démarrage

### Lancer l'application

1. **Double-cliquer** sur `EasySave.GUI.exe`
2. **Autoriser** l'application si Windows demande la permission
3. L'interface principale s'affiche

### Configuration initiale

Au premier lancement, EasySave va :

1. ✅ Créer le dossier `%APPDATA%\EasySave\`
2. ✅ Créer le fichier de configuration par défaut
3. ✅ Afficher la liste vide de jobs

**C'est prêt !** Vous pouvez maintenant créer votre premier job de sauvegarde.

---

## 🖥️ Interface principale

### Vue d'ensemble

```
┌─────────────────────────────────────────────────────────────┐
│  EasySave v2.0                                    ─  □  ✕   │
├─────────────────────────────────────────────────────────────┤
│  [➕ Créer un job]  [▶️ Tout exécuter]  [🗑️ Supprimer]      │
│  [🔄 Actualiser]                                             │
├──────────────────────────────┬──────────────────────────────┤
│  Liste des jobs              │  Détails du job              │
│                              │                              │
│  📁 Backup Documents         │  Nom : Backup Documents      │
│  ▶️ Sauvegarde Photos        │  Type : Complete             │
│  ⏸️ Projet Important          │  Source : C:\Users\...       │
│                              │  Cible : D:\Backup\...       │
│                              │  Statut : Prêt               │
│                              │                              │
│                              │  [▶️ Exécuter]               │
└──────────────────────────────┴──────────────────────────────┘
```

### Zone supérieure - Barre d'outils

| Bouton | Description |
|--------|-------------|
| **➕ Créer un job** | Ouvre la fenêtre de création d'un nouveau job |
| **▶️ Tout exécuter** | Lance tous les jobs les uns après les autres |
| **🗑️ Supprimer** | Supprime le job sélectionné |
| **🔄 Actualiser** | Recharge la liste des jobs |

### Zone gauche - Liste des jobs

- Affiche tous vos jobs de sauvegarde
- **Cliquer** sur un job pour voir ses détails
- **Icônes de statut** :
  - 📁 Prêt
  - ▶️ En cours
  - ✅ Terminé
  - ⚠️ En pause
  - ❌ Erreur

### Zone droite - Détails du job

Affiche les informations du job sélectionné :
- Nom du job
- Type de sauvegarde (Complete / Differential)
- Chemin source
- Chemin de destination
- Statut actuel
- Progression (nombre de fichiers transférés)
- Boutons d'action

### Double-clic sur les chemins

💡 **Astuce :** Double-cliquez sur le chemin source ou cible pour ouvrir le dossier dans l'Explorateur Windows.

---

## ➕ Créer un job de sauvegarde

### Étape 1 : Ouvrir la fenêtre de création

1. Cliquer sur le bouton **"➕ Créer un job"**
2. Une nouvelle fenêtre s'ouvre

### Étape 2 : Remplir les informations

```
┌──────────────────────────────────────────┐
│  Créer un nouveau job de sauvegarde      │
├──────────────────────────────────────────┤
│  Nom du job :                            │
│  [Mes Documents              ]           │
│                                          │
│  Chemin source :                         │
│  [C:\Users\Moi\Documents     ] [📁]      │
│                                          │
│  Chemin de destination :                 │
│  [D:\Backups\Documents       ] [📁]      │
│                                          │
│  Type de sauvegarde :                    │
│  ( ) Complete                            │
│  (•) Differential                        │
│                                          │
│  [Créer]  [Annuler]                      │
└──────────────────────────────────────────┘
```

#### Champ "Nom du job"

- Donnez un **nom descriptif** à votre sauvegarde
- Exemples :
  - ✅ "Backup Documents Importants"
  - ✅ "Photos de famille"
  - ✅ "Projet Client XYZ"
  - ❌ "Backup1" (trop vague)

#### Champ "Chemin source"

C'est le dossier que vous voulez sauvegarder.

1. Cliquer sur le bouton **📁** à droite
2. Naviguer jusqu'au dossier souhaité
3. Cliquer sur **"Sélectionner un dossier"**

**Exemples de sources courantes :**
- `C:\Users\VotreNom\Documents`
- `C:\Users\VotreNom\Pictures`
- `C:\Projets\MonProjet`

#### Champ "Chemin de destination"

C'est où la sauvegarde sera stockée.

1. Cliquer sur le bouton **📁** à droite
2. Naviguer jusqu'à l'emplacement de sauvegarde
3. Cliquer sur **"Sélectionner un dossier"**

**Recommandations :**
- ✅ Utiliser un **disque externe** (clé USB, disque dur externe)
- ✅ Utiliser un **autre disque** que celui de la source
- ✅ Utiliser un **emplacement réseau** (NAS, serveur)
- ❌ Éviter de sauvegarder sur le même disque

#### Type de sauvegarde

**Complete (Complète)**
- ✅ Copie **TOUS** les fichiers du dossier source
- ✅ Recommandé pour la **première sauvegarde**
- ⚠️ Prend plus de temps et d'espace

**Differential (Différentielle)**
- ✅ Copie **seulement** les fichiers modifiés ou nouveaux
- ✅ Plus **rapide** et économise de l'espace
- ⚠️ Nécessite une sauvegarde complète initiale

### Étape 3 : Valider

1. Vérifier que toutes les informations sont correctes
2. Cliquer sur **"Créer"**
3. Le nouveau job apparaît dans la liste ✅

---

## ▶️ Exécuter une sauvegarde

### Exécuter un job unique

1. **Sélectionner** le job dans la liste (clic gauche)
2. **Cliquer** sur le bouton ▶️ dans les détails du job
3. La sauvegarde démarre !

### Pendant l'exécution

Vous pouvez suivre la progression :

```
Statut : En cours d'exécution ▶️
Progression : 145 / 520 fichiers transférés
```

**Indicateurs :**
- Le statut change en "Running"
- Le compteur de fichiers s'incrémente
- Une icône de chargement peut apparaître

### Exécuter tous les jobs

1. Cliquer sur **"▶️ Tout exécuter"**
2. Tous les jobs s'exécutent **séquentiellement**
3. Chaque job attend que le précédent soit terminé

### Fin de la sauvegarde

Quand la sauvegarde est terminée :
- ✅ Statut : "Completed"
- ✅ Compteur de fichiers finalisé
- ✅ Vous pouvez vérifier le dossier de destination

---

## 🗂️ Gérer vos jobs

### Modifier un job

⚠️ **Note :** La modification de jobs n'est pas encore disponible dans l'interface graphique.

**Solution temporaire :**
1. Supprimer le job existant
2. Recréer un nouveau job avec les bons paramètres

### Supprimer un job

1. **Sélectionner** le job dans la liste
2. **Cliquer** sur 🗑️ "Supprimer"
3. **Confirmer** la suppression
4. Le job est supprimé ✅

⚠️ **Attention :** Cette action supprime uniquement le job de la liste, **PAS** les fichiers sauvegardés.

### Actualiser la liste

Si vous avez modifié les jobs manuellement :

1. **Cliquer** sur 🔄 "Actualiser"
2. La liste se recharge depuis le fichier de configuration

---

## ⚙️ Configuration avancée

### Fichier de configuration

Le fichier `appsettings.json` permet de configurer des options avancées.

**Emplacement :**
```
C:\Program Files\EasySave\appsettings.json
```

### Créer le fichier de configuration

Si le fichier n'existe pas :

1. Ouvrir le **Bloc-notes**
2. Copier le contenu suivant :

```json
{
  "LogFormat": "JSON",
  "ExtensionsToEncrypt": [],
  "BusinessSoftwareName": ""
}
```

3. **Enregistrer sous** : `appsettings.json`
4. Emplacement : à côté de `EasySave.GUI.exe`

### Options disponibles

#### LogFormat

Format des fichiers de log.

```json
"LogFormat": "JSON"
```

Valeurs possibles :
- `"JSON"` - Format JSON (recommandé)
- `"XML"` - Format XML

#### ExtensionsToEncrypt

Liste des extensions de fichiers à crypter.

```json
"ExtensionsToEncrypt": [".doc", ".docx", ".pdf", ".txt"]
```

**Exemples :**
- Crypter tous les documents Office :
  ```json
  "ExtensionsToEncrypt": [".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx"]
  ```

- Crypter tous les documents :
  ```json
  "ExtensionsToEncrypt": [".doc", ".docx", ".pdf", ".txt", ".odt"]
  ```

- Ne rien crypter :
  ```json
  "ExtensionsToEncrypt": []
  ```

#### BusinessSoftwareName

Nom d'un logiciel métier à surveiller.

```json
"BusinessSoftwareName": "calculatrice"
```

Si ce logiciel est détecté en cours d'exécution, les sauvegardes se mettent automatiquement en pause.

**Exemple :** Si vous utilisez un logiciel de comptabilité et ne voulez pas que les fichiers soient sauvegardés pendant son utilisation.

---

## 🔐 Cryptage des fichiers

### Qu'est-ce que le cryptage ?

Le cryptage transforme vos fichiers pour les rendre **illisibles** sans la clé de décryptage. C'est utile pour protéger vos données sensibles.

### Activer le cryptage

1. **Créer/Modifier** le fichier `appsettings.json`
2. **Ajouter** les extensions à crypter :

```json
{
  "LogFormat": "JSON",
  "ExtensionsToEncrypt": [".doc", ".docx", ".xls", ".xlsx", ".pdf", ".txt"],
  "BusinessSoftwareName": ""
}
```

3. **Enregistrer** le fichier
4. **Relancer** EasySave

### Comment ça fonctionne ?

Lors d'une sauvegarde :

1. EasySave copie le fichier vers la destination
2. Si l'extension est dans `ExtensionsToEncrypt` :
   - ✅ Le fichier est **automatiquement crypté**
   - 🔒 Le contenu devient illisible
3. Si l'extension n'est PAS dans la liste :
   - ⏩ Le fichier reste **en clair**

### Exemple

**Configuration :**
```json
"ExtensionsToEncrypt": [".txt", ".doc"]
```

**Résultat :**
- `document.txt` → 🔒 **CRYPTÉ**
- `rapport.doc` → 🔒 **CRYPTÉ**
- `photo.jpg` → ⏩ **EN CLAIR**
- `video.mp4` → ⏩ **EN CLAIR**

### Décrypter les fichiers

⚠️ **Important :** Le décryptage n'est pas encore implémenté dans cette version.

Pour récupérer vos fichiers :
1. Conservez une copie non cryptée de vos fichiers importants
2. Ou désactivez le cryptage pour les futures sauvegardes

---

## ❓ Questions fréquentes

### Combien de jobs puis-je créer ?

**Illimité !** La v2.0 supprime la limite de 5 jobs de la v1.0.

### Où sont stockés mes jobs ?

Les jobs sont sauvegardés dans :
```
%APPDATA%\EasySave\jobs.json
```

Vous pouvez les sauvegarder en copiant ce fichier.

### Puis-je sauvegarder vers un disque réseau ?

Oui ! Utilisez un chemin réseau comme destination :
```
\\SERVEUR\Partage\Backup
```

ou un lecteur mappé :
```
Z:\Backup
```

### Que se passe-t-il si j'arrête une sauvegarde ?

- Les fichiers déjà copiés restent dans la destination
- La sauvegarde reprendra depuis le début au prochain lancement
- Aucun fichier n'est perdu

### Puis-je sauvegarder vers une clé USB ?

Oui ! Sélectionnez simplement le lecteur USB comme destination :
```
E:\Backup
```

⚠️ **Attention :** Assurez-vous que la clé USB a suffisamment d'espace.

### Les sauvegardes sont-elles automatiques ?

Non, dans cette version vous devez lancer manuellement chaque sauvegarde.

**Solution :** Vous pouvez créer une tâche planifiée Windows pour lancer EasySave automatiquement.

### Puis-je voir les fichiers de log ?

Oui ! Les logs sont dans :
```
Destination\logs\
```

Exemple : Si votre destination est `D:\Backup\`, les logs sont dans `D:\Backup\logs\`

---

## 🔧 Dépannage

### L'application ne démarre pas

**Symptôme :** Double-cliquer sur EasySave.GUI.exe ne fait rien

**Solutions :**

1. **Vérifier .NET 8.0**
   ```
   Ouvrir cmd
   Taper : dotnet --version
   Doit afficher : 8.0.x
   ```

2. **Vérifier les permissions**
   - Clic droit sur EasySave.GUI.exe
   - Propriétés → Général
   - Décocher "Débloquer" si présent

3. **Exécuter en tant qu'administrateur**
   - Clic droit sur EasySave.GUI.exe
   - "Exécuter en tant qu'administrateur"

### Erreur "Le dossier source n'existe pas"

**Solutions :**

1. Vérifier que le chemin source est correct
2. Vérifier que vous avez les permissions de lecture
3. Le dossier doit exister avant de créer le job

### La sauvegarde est très lente

**Causes possibles :**

- 📦 **Beaucoup de fichiers** : Normal, soyez patient
- 💾 **Disque externe lent** : Les USB 2.0 sont lents
- 🔒 **Cryptage activé** : Le cryptage ralentit la copie
- 🌐 **Sauvegarde réseau** : Le réseau peut être lent

**Solutions :**

- Utiliser un disque USB 3.0 ou plus rapide
- Désactiver le cryptage pour les tests
- Utiliser le type "Differential" après une première sauvegarde complète

### "CryptoSoft not available"

**Symptôme :** Message dans les logs ou la console

**Solutions :**

1. Vérifier que `CryptoSoft.dll` est présent
2. Le fichier doit être dans le même dossier que EasySave.GUI.exe
3. Réinstaller l'application

### Le cryptage ne fonctionne pas

**Vérifications :**

1. ✅ `appsettings.json` existe ?
2. ✅ `ExtensionsToEncrypt` contient des extensions ?
3. ✅ Les extensions commencent par un point `.txt` ?
4. ✅ EasySave a été relancé après la modification ?

**Tester :**

1. Créer un fichier `test.txt` dans le dossier source
2. Lancer une sauvegarde
3. Ouvrir `test.txt` dans la destination
4. Le contenu doit être illisible

### Les jobs ont disparu

**Cause :** Le fichier `jobs.json` a été supprimé ou corrompu

**Solution :**

1. Vérifier si le fichier existe :
   ```
   %APPDATA%\EasySave\jobs.json
   ```

2. Si le fichier est corrompu, le supprimer
3. Relancer EasySave
4. Recréer vos jobs

**Prévention :**

- Faire des copies régulières de `jobs.json`
- Utiliser un logiciel de sauvegarde pour ce fichier

---

## 📞 Support

### Obtenir de l'aide

Si vous rencontrez un problème non résolu par ce manuel :

1. **Consulter les logs**
   ```
   %APPDATA%\EasySave\logs\
   Destination\logs\
   ```

2. **Vérifier la documentation technique**
   - `TECHNICAL_DOCUMENTATION_v2.0.md`
   - `README.md`

3. **Contacter le support**
   - Email : support@prosoft.example.com
   - Issues GitHub : https://github.com/Cesi-Genie-Logiciel/easySave_Cesi/issues

### Informations utiles à fournir

Quand vous contactez le support, préparez :

- Version d'EasySave (2.0)
- Version de Windows
- Description du problème
- Messages d'erreur (capture d'écran)
- Fichiers de log

---

## 📋 Glossaire

| Terme | Définition |
|-------|------------|
| **Job** | Une tâche de sauvegarde configurée |
| **Source** | Dossier à sauvegarder |
| **Destination / Cible** | Dossier où la sauvegarde est stockée |
| **Complete** | Sauvegarde qui copie tous les fichiers |
| **Differential** | Sauvegarde qui copie seulement les fichiers modifiés |
| **Cryptage** | Transformation d'un fichier pour le rendre illisible |
| **Extension** | Suffixe du nom de fichier (.txt, .doc, .pdf) |

---

## 📚 Annexes

### Raccourcis clavier

| Raccourci | Action |
|-----------|--------|
| `F5` | Actualiser la liste |
| `Ctrl + N` | Nouveau job |
| `Delete` | Supprimer le job sélectionné |

### Chemins importants

| Description | Chemin |
|-------------|--------|
| Dossier de données | `%APPDATA%\EasySave\` |
| Fichier des jobs | `%APPDATA%\EasySave\jobs.json` |
| Logs | `%APPDATA%\EasySave\logs\` |
| Configuration | `appsettings.json` (à côté de l'exe) |

### Formats de fichiers

- **jobs.json** : Liste des jobs au format JSON
- **appsettings.json** : Configuration de l'application
- **Logs JSON** : Fichiers de log au format JSON
- **Logs XML** : Fichiers de log au format XML

---

<div align="center">
  <p><strong>Merci d'utiliser EasySave v2.0 !</strong></p>
  <p>© 2026 ProSoft - Tous droits réservés</p>
  <p>Version du manuel : 2.0 - Février 2026</p>
</div>
