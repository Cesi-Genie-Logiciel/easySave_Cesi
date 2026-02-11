# Notice Utilisateur - EasySave

**Version 1.0**  
**Date : Février 2026**  
**Projet CESI - Génie Logiciel**

---

## Table des matières
1. [Introduction](#1-introduction)
2. [Installation](#2-installation)
3. [Premier démarrage](#3-premier-démarrage)
4. [Guide d'utilisation](#4-guide-dutilisation)
5. [Types de sauvegardes](#5-types-de-sauvegardes)
6. [Suivi des sauvegardes](#6-suivi-des-sauvegardes)
7. [Bonnes pratiques](#7-bonnes-pratiques)
8. [Résolution des problèmes](#8-résolution-des-problèmes)
9. [Questions fréquentes (FAQ)](#9-questions-fréquentes-faq)

---

## 1. Introduction

### 1.1 Qu'est-ce qu'EasySave ?

EasySave est un logiciel de sauvegarde simple et efficace qui permet de protéger vos données importantes en créant des copies de vos fichiers et dossiers.

### 1.2 Fonctionnalités principales

- **Sauvegarde complète** : Copie tous les fichiers d'un dossier source vers une destination
- **Sauvegarde différentielle** : Copie uniquement les fichiers modifiés depuis la dernière sauvegarde
- **Gestion de travaux de sauvegarde** : Créez jusqu'à 5 travaux de sauvegarde prédéfinis
- **Suivi en temps réel** : Visualisez la progression de vos sauvegardes
- **Historique** : Consultez les logs de tous les transferts de fichiers
- **Fichier d'état** : Suivez l'état actuel de chaque sauvegarde

### 1.3 Configuration minimale requise

- **Système d'exploitation** : Windows 10/11, Linux, macOS
- **Logiciel requis** : .NET 8.0 Runtime ou supérieur
- **Espace disque** : Variable selon la taille de vos données à sauvegarder
- **Permissions** : Droits de lecture sur les dossiers sources et d'écriture sur les destinations

---

## 2. Installation

### 2.1 Installation de .NET Runtime

**Windows :**
1. Téléchargez .NET 8.0 Runtime depuis : https://dotnet.microsoft.com/download
2. Exécutez l'installeur et suivez les instructions
3. Redémarrez votre ordinateur si nécessaire

**Vérification :**
Ouvrez un terminal (PowerShell ou CMD) et tapez :
```
dotnet --version
```
Vous devriez voir le numéro de version (ex: `8.0.1`)

### 2.2 Installation d'EasySave

**Option A : Depuis le code source**
1. Clonez ou téléchargez le projet
2. Ouvrez un terminal dans le dossier du projet
3. Compilez avec : `dotnet build`
4. L'exécutable est dans : `EasySave\bin\Debug\net8.0\`

**Option B : Version compilée**
1. Téléchargez l'archive EasySave
2. Extrayez-la dans un dossier de votre choix (ex: `C:\Program Files\EasySave\`)
3. Le logiciel est prêt à l'emploi

---

## 3. Premier démarrage

### 3.1 Lancer EasySave

**Windows :**
- Double-cliquez sur `EasySave.exe`
- OU lancez depuis un terminal : `.\EasySave.exe`

**Linux/macOS :**
```bash
dotnet EasySave.dll
```

### 3.2 Interface au démarrage

Vous verrez le menu principal :

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

---

## 4. Guide d'utilisation

### 4.1 Créer un travail de sauvegarde

**Étape 1 : Sélectionner l'option**
- Dans le menu principal, tapez `1` puis Entrée

**Étape 2 : Nommer votre sauvegarde**
- Entrez un nom descriptif (ex: `Documents_Personnels`)
- Appuyez sur Entrée

**Étape 3 : Définir la source**
- Entrez le chemin complet du dossier à sauvegarder
- Exemples :
  - Windows : `C:\Users\VotreNom\Documents`
  - Linux/macOS : `/home/votrenom/documents`
- Appuyez sur Entrée

**Étape 4 : Définir la destination**
- Entrez le chemin où sauvegarder vos fichiers
- Exemples :
  - Disque externe : `D:\Sauvegardes\Documents`
  - Serveur réseau : `\\serveur\sauvegardes\documents`
- Appuyez sur Entrée

**Étape 5 : Choisir le type de sauvegarde**
- Tapez `Complete` pour une sauvegarde complète
- OU tapez `Differential` pour une sauvegarde différentielle
- Appuyez sur Entrée

**Confirmation :**
```
Backup job 'Documents_Personnels' created successfully
```

**Limite :** Vous pouvez créer maximum 5 travaux de sauvegarde.

### 4.2 Lister les travaux de sauvegarde

**Action :**
- Dans le menu principal, tapez `2` puis Entrée

**Affichage :**
```
========================================
         ALL BACKUP JOBS
========================================

[0] Documents_Personnels
    Source: C:\Users\VotreNom\Documents
    Target: D:\Sauvegardes\Documents

[1] Photos_Vacances
    Source: C:\Users\VotreNom\Pictures\Vacances
    Target: D:\Sauvegardes\Photos

Total: 2 backup job(s)
```

### 4.3 Exécuter une sauvegarde

**Étape 1 : Sélectionner l'option**
- Dans le menu principal, tapez `3` puis Entrée

**Étape 2 : Choisir le travail**
- Entrez le numéro de l'index du travail (ex: `0` pour le premier)
- Appuyez sur Entrée

**Progression :**
```
[18:30:15] Starting backup: Documents_Personnels
  Strategy: Complete Backup (copy all files)
  [Console] Backup 'Documents_Personnels' started
    Copied: rapport.docx
    Copied: budget.xlsx
  [Console] Progress: 50% (5/10 files)
    Copied: photo.jpg
  [Console] Progress: 100% (10/10 files)
  [Console] Backup 'Documents_Personnels' completed successfully
[18:30:45] Backup completed: Documents_Personnels
```

### 4.4 Exécuter plusieurs sauvegardes

**Action :**
- Dans le menu principal, tapez `4` puis Entrée
- Entrez les indices séparés par des virgules (ex: `0,1,2`)
- Appuyez sur Entrée

**Résultat :**
Les sauvegardes s'exécutent l'une après l'autre dans l'ordre spécifié.

### 4.5 Supprimer un travail de sauvegarde

**Action :**
- Dans le menu principal, tapez `5` puis Entrée
- Entrez l'index du travail à supprimer
- Appuyez sur Entrée

**Confirmation :**
```
Backup job 'Documents_Personnels' deleted
```

**Attention :** Cette action ne supprime que la configuration, pas les fichiers sauvegardés.

### 4.6 Quitter le programme

**Action :**
- Dans le menu principal, tapez `0` puis Entrée

**Confirmation :**
```
Goodbye!
```

---

## 5. Types de sauvegardes

### 5.1 Sauvegarde complète (Complete)

**Description :**
Copie **tous les fichiers** du dossier source vers la destination, qu'ils aient été modifiés ou non.

**Quand l'utiliser :**
- Première sauvegarde d'un dossier
- Sauvegarde sur un nouveau support
- Vous voulez une copie intégrale et à jour

**Avantages :**
- Garantit une copie complète et identique
- Restauration simple et rapide

**Inconvénients :**
- Plus long (copie tout)
- Consomme plus d'espace disque

**Exemple d'utilisation :**
```
Source: C:\Projets\MonProjet (10 fichiers, 100 Mo)
Destination: D:\Backup\MonProjet

Résultat: 10 fichiers copiés, 100 Mo transférés
```

### 5.2 Sauvegarde différentielle (Differential)

**Description :**
Copie **uniquement les fichiers nouveaux ou modifiés** depuis la dernière sauvegarde.

**Quand l'utiliser :**
- Sauvegardes régulières d'un même dossier
- Vous avez déjà fait une sauvegarde complète
- Vous voulez économiser du temps et de l'espace

**Avantages :**
- Rapide (copie seulement ce qui a changé)
- Économise l'espace disque

**Inconvénients :**
- Nécessite une sauvegarde complète initiale
- Peut manquer des fichiers supprimés

**Exemple d'utilisation :**
```
Source: C:\Projets\MonProjet (10 fichiers)
Destination: D:\Backup\MonProjet (contient déjà 8 fichiers)

Résultat: 2 fichiers copiés (nouveaux ou modifiés), 8 fichiers ignorés
```

### 5.3 Comparaison

| Critère | Complète | Différentielle |
|---------|----------|----------------|
| Temps d'exécution | Long | Court |
| Espace disque utilisé | Élevé | Faible |
| Simplicité de restauration | Très simple | Simple |
| Usage recommandé | Première fois, périodique | Régulier |

---

## 6. Suivi des sauvegardes

### 6.1 Fichier de logs

**Emplacement :**
- Windows : `C:\Users\VotreNom\AppData\Roaming\EasySave\Logs\`
- Linux/macOS : `~/.config/EasySave/Logs/`

**Nom du fichier :**
`log_YYYY-MM-DD.json` (un fichier par jour)

**Contenu :**
Liste de tous les fichiers transférés avec leurs informations :
- Horodatage
- Nom de la sauvegarde
- Chemin source et destination
- Taille du fichier
- Temps de transfert

**Exemple :**
```json
[
  {
    "timestamp": "2026-02-11T18:30:15",
    "backupName": "Documents_Personnels",
    "sourceFile": "C:\\Users\\hp\\Documents\\rapport.docx",
    "destFile": "D:\\Sauvegardes\\Documents\\rapport.docx",
    "fileSize": 25600,
    "transferTimeMs": 15.3
  }
]
```

**Consultation :**
Ouvrez le fichier avec un éditeur de texte (Notepad, VS Code, etc.)

### 6.2 Fichier d'état

**Emplacement :**
`EasySave\State\state.json`

**Contenu :**
État en temps réel de la dernière sauvegarde exécutée :
- Nom de la sauvegarde
- État (Active / Inactive)
- Nombre de fichiers (total et restants)
- Taille (totale et restante)
- Fichier en cours de traitement
- Pourcentage de progression

**Exemple :**
```json
{
  "Name": "Documents_Personnels",
  "Timestamp": "2026-02-11T18:30:45",
  "State": "Inactive",
  "TotalFiles": 10,
  "FilesRemaining": 0,
  "TotalSize": 102400,
  "SizeRemaining": 0,
  "CurrentSourceFile": "C:\\Users\\hp\\Documents\\photo.jpg",
  "CurrentDestFile": "D:\\Sauvegardes\\Documents\\photo.jpg",
  "Progress": 100
}
```

**Usage :**
Ce fichier permet de suivre l'avancement d'une sauvegarde en cours, même depuis une autre application.

---

## 7. Bonnes pratiques

### 7.1 Organisation des sauvegardes

**Nommage :**
- Utilisez des noms clairs et descriptifs
- Exemples : `Documents_Travail`, `Photos_2026`, `Projets_Dev`

**Fréquence recommandée :**
- **Données critiques** : Quotidien (différentielle) + Hebdomadaire (complète)
- **Données importantes** : Hebdomadaire (différentielle) + Mensuelle (complète)
- **Données archivage** : Mensuelle (complète)

**Destinations multiples (règle 3-2-1) :**
1. **3 copies** : Original + 2 sauvegardes
2. **2 supports différents** : Disque dur + Disque externe
3. **1 copie hors-site** : Serveur distant, cloud, autre lieu physique

### 7.2 Gestion de l'espace disque

**Vérifications régulières :**
- Vérifiez l'espace disponible sur la destination
- Supprimez les anciennes sauvegardes si nécessaire
- Utilisez des sauvegardes différentielles pour économiser l'espace

**Recommandation :**
Prévoyez **1.5 à 2 fois** l'espace de vos données sources sur la destination.

### 7.3 Sécurité

**Chemins à éviter :**
- Ne sauvegardez pas dans le même dossier que la source
- Ne sauvegardez pas sur le même disque physique (risque de panne)

**Permissions :**
- Assurez-vous d'avoir les droits d'accès nécessaires
- Sur Windows, lancez en administrateur si nécessaire

**Test de restauration :**
- Testez régulièrement la restauration de vos fichiers
- Vérifiez que les fichiers sauvegardés sont lisibles

---

## 8. Résolution des problèmes

### 8.1 Le programme ne démarre pas

**Symptôme :** Double-clic sur l'exécutable sans effet

**Solution :**
1. Vérifiez que .NET 8.0 est installé :
   ```
   dotnet --version
   ```
2. Si absent, installez-le depuis https://dotnet.microsoft.com/download
3. Lancez depuis un terminal pour voir les erreurs :
   ```
   .\EasySave.exe
   ```

### 8.2 Erreur "Source not found"

**Symptôme :**
```
Error: Source not found: C:\MesDonnées
```

**Causes possibles :**
- Le chemin n'existe pas
- Faute de frappe dans le chemin
- Permissions insuffisantes

**Solutions :**
1. Vérifiez que le dossier existe (Explorateur Windows / File Manager)
2. Copiez-collez le chemin depuis l'explorateur de fichiers
3. Sur Windows, utilisez des guillemets si le chemin contient des espaces :
   ```
   "C:\Mes Documents\Photos"
   ```

### 8.3 Erreur de permissions

**Symptôme :**
```
Access denied / Accès refusé
```

**Solutions :**
- **Windows :** Lancez EasySave en tant qu'administrateur (clic droit → "Exécuter en tant qu'administrateur")
- **Linux/macOS :** Utilisez `sudo` si nécessaire
- Vérifiez les permissions du dossier destination

### 8.4 Espace disque insuffisant

**Symptôme :**
```
Disk full / Espace disque insuffisant
```

**Solutions :**
1. Libérez de l'espace sur le disque de destination
2. Supprimez d'anciennes sauvegardes
3. Choisissez une autre destination avec plus d'espace
4. Utilisez une sauvegarde différentielle au lieu de complète

### 8.5 La sauvegarde est très lente

**Causes possibles :**
- Nombreux fichiers ou fichiers très volumineux
- Disque lent (disque réseau, USB 2.0)
- Antivirus qui analyse chaque fichier

**Solutions :**
1. Utilisez un disque plus rapide (USB 3.0, SSD)
2. Ajoutez EasySave et les dossiers de sauvegarde aux exceptions de l'antivirus
3. Lancez les sauvegardes en heures creuses
4. Préférez la sauvegarde différentielle pour les mises à jour fréquentes

### 8.6 Les logs ne sont pas créés

**Symptôme :** Le dossier `Logs` est vide ou absent

**Solutions :**
1. Vérifiez les permissions d'écriture dans `%APPDATA%\EasySave\`
2. Exécutez au moins une sauvegarde (les logs se créent lors des transferts)
3. Vérifiez que l'antivirus ne bloque pas l'écriture

**Vérification manuelle :**
```
Windows : C:\Users\VotreNom\AppData\Roaming\EasySave\Logs\
```

---

## 9. Questions fréquentes (FAQ)

### Q1 : Puis-je créer plus de 5 travaux de sauvegarde ?

**Réponse :** Non, la limite est fixée à 5 travaux simultanés. Vous pouvez supprimer un ancien travail pour en créer un nouveau.

### Q2 : La sauvegarde supprime-t-elle les fichiers de la destination ?

**Réponse :** Non, EasySave ne supprime jamais de fichiers. Les fichiers existants sont écrasés si une version plus récente existe, mais aucune suppression automatique n'est effectuée.

### Q3 : Puis-je arrêter une sauvegarde en cours ?

**Réponse :** Oui, utilisez `Ctrl+C` dans le terminal. Attention : la sauvegarde sera incomplète.

### Q4 : Les sauvegardes fonctionnent-elles sur des dossiers réseau ?

**Réponse :** Oui, vous pouvez sauvegarder vers/depuis des dossiers réseau (UNC : `\\serveur\partage\`).

### Q5 : Dois-je faire une sauvegarde complète avant une différentielle ?

**Réponse :** Ce n'est pas obligatoire, mais recommandé. La première sauvegarde différentielle copiera tous les fichiers absents de la destination.

### Q6 : Puis-je planifier des sauvegardes automatiques ?

**Réponse :** Pas dans cette version. Vous devez lancer manuellement chaque sauvegarde. Vous pouvez utiliser le Planificateur de tâches Windows pour automatiser le lancement.

### Q7 : Les sous-dossiers sont-ils sauvegardés ?

**Réponse :** Oui, EasySave sauvegarde récursivement tous les fichiers et sous-dossiers du dossier source.

### Q8 : Que signifie "Progress: 50%" ?

**Réponse :** C'est le pourcentage de fichiers traités (ex: 5 fichiers sur 10 = 50%).

### Q9 : Puis-je sauvegarder vers un cloud (OneDrive, Google Drive) ?

**Réponse :** Oui, si le dossier cloud est synchronisé localement. Spécifiez le chemin local du dossier cloud.

### Q10 : Comment restaurer mes fichiers ?

**Réponse :** Copiez simplement les fichiers depuis le dossier de destination vers l'emplacement souhaité. EasySave ne gère pas la restauration automatique.

---

## Support et contact

Pour toute assistance supplémentaire, consultez :
- Le fichier `README.md` du projet
- Les diagrammes d'architecture dans `doc/architecture/`

**Projet académique CESI - Génie Logiciel**  
**Version 1.0 - Février 2026**

---

*Fin de la notice utilisateur*
