# 📦 Guide de Publication EasySave v2.0

## 🎯 Processus de Publication

### Prérequis
- .NET 8.0 SDK installé
- Accès au dépôt Git
- EasyLog v1.1 à jour dans `../easyLog_Cesi`

---

## 📋 Étapes de Publication

### 1️⃣ Vérifier que tout est à jour

```powershell
# Se positionner dans le repo EasySave
cd "c:\Users\hp\source\repos\Cesi-Genie-Logiciel\easySave_Cesi"

# Vérifier la branche actuelle
git status

# Pull les dernières modifications
git pull origin dev
```

### 2️⃣ Mettre à jour EasyLog

```powershell
# Aller dans le repo EasyLog
cd "c:\Users\hp\source\repos\Cesi-Genie-Logiciel\easyLog_Cesi"

# Pull les dernières modifications
git pull origin dev

# Retourner dans EasySave
cd "c:\Users\hp\source\repos\Cesi-Genie-Logiciel\easySave_Cesi"
```

### 3️⃣ Compiler en mode Release

```powershell
# Build du projet GUI en Release
dotnet build "c:\Users\hp\source\repos\Cesi-Genie-Logiciel\easySave_Cesi\EasySave.GUI\EasySave.GUI.csproj" --configuration Release
```

### 4️⃣ Tester l'application

```powershell
# Lancer l'application pour test
Start-Process "c:\Users\hp\source\repos\Cesi-Genie-Logiciel\easySave_Cesi\EasySave.GUI\bin\Release\net8.0-windows\EasySave.GUI.exe"
```

**Tests à effectuer :**
- ✅ Créer un nouveau job
- ✅ Exécuter un job et vérifier la progression
- ✅ Vérifier que le statut se met à jour à la fin
- ✅ Supprimer un job
- ✅ Actualiser la liste
- ✅ Double-cliquer sur les chemins
- ✅ Sélectionner un job pour voir les détails

### 5️⃣ Publier l'application

```powershell
# Publier l'application
dotnet publish "c:\Users\hp\source\repos\Cesi-Genie-Logiciel\easySave_Cesi\EasySave.GUI\EasySave.GUI.csproj" --configuration Release --output "c:\Users\hp\source\repos\Cesi-Genie-Logiciel\easySave_Cesi\publish" --self-contained false
```

### 6️⃣ Créer le ZIP de distribution

```powershell
# Créer le ZIP
Compress-Archive -Path "c:\Users\hp\source\repos\Cesi-Genie-Logiciel\easySave_Cesi\publish\*" -DestinationPath "c:\Users\hp\source\repos\Cesi-Genie-Logiciel\easySave_Cesi\EasySave_v2.0_Release.zip" -Force
```

**Le ZIP sera créé à :**
```
c:\Users\hp\source\repos\Cesi-Genie-Logiciel\easySave_Cesi\EasySave_v2.0_Release.zip
```

---

## 📁 Contenu du Package

Le ZIP contient :
- `EasySave.GUI.exe` - Application graphique principale
- `EasySave.dll` - Bibliothèque backend
- `EasyLog.dll` - Bibliothèque de logging
- Fichiers de configuration et dépendances .NET

---

## 🚀 Installation pour l'utilisateur final

### Prérequis utilisateur
- Windows 10/11
- .NET 8.0 Runtime (ou SDK)

### Installation
1. Extraire le contenu du ZIP dans un dossier
2. Double-cliquer sur `EasySave.GUI.exe`
3. L'application créera automatiquement les fichiers de configuration au premier lancement

---

## ⚙️ Configuration CryptoSoft (optionnel)

Pour activer le cryptage automatique :

1. Créer un fichier `appsettings.json` à côté de `EasySave.GUI.exe` :

```json
{
  "LogFormat": "JSON",
  "ExtensionsToEncrypt": [".doc", ".docx", ".pdf", ".txt", ".xls", ".xlsx"],
  "BusinessSoftwareName": ""
}
```

2. S'assurer que `CryptoSoft.exe` est présent dans le dossier d'installation

---

## 📝 Notes de Version

### v2.0 - Release Finale (14/02/2026)

**✅ Fonctionnalités implémentées :**
- Interface graphique WPF moderne (MVVM)
- Gestion illimitée de jobs de sauvegarde
- Création/modification/suppression de jobs via GUI
- Exécution de jobs avec progression en temps réel
- Panneau de détails pour chaque job
- Double-clic sur chemins pour ouvrir l'Explorateur
- Support des stratégies Complete et Differential
- Logging JSON/XML (EasyLog v1.1)
- **CryptoSoft fonctionnel avec filtrage par extension**

**✅ Cryptage sélectif :**
- Cryptage automatique basé sur les extensions de fichiers
- Configuration via `appsettings.json`
- Seuls les fichiers avec extensions configurées sont cryptés
- Support de CryptoSoft détection automatique cross-platform

**⚠️ Limitations connues :**
- Pas d'interface GUI pour les paramètres généraux (feature P2 future)
- Configuration CryptoSoft nécessite édition manuelle de `appsettings.json`
- Détection de logiciel métier partiellement implémentée

**🔮 Prochaines versions :**
- Interface de configuration des paramètres (P2)
- Tests de validation complets

---

## 🐛 Dépannage

### L'application ne démarre pas
- Vérifier que .NET 8.0 Runtime est installé
- Vérifier les logs dans `%APPDATA%\EasySave\logs\`

### Les jobs ne se sauvegardent pas
- Vérifier les permissions d'écriture dans `%APPDATA%\EasySave\`
- Vérifier que `jobs.json` n'est pas corrompu

### CryptoSoft ne fonctionne pas
- Vérifier que `CryptoSoft.exe` existe
- Vérifier que `appsettings.json` est correctement configuré
- Vérifier les extensions dans `ExtensionsToEncrypt`

---

## 📞 Support

Pour toute question ou problème :
- Consulter la documentation technique : `TECHNICAL_DOCUMENTATION_v2.0.md`
- Consulter le guide de passation : `HANDOVER_GUIDE.md`
- Contacter l'équipe de développement

---

**Généré le :** 13/02/2026  
**Version :** 2.0 (Release Intermédiaire)  
**Équipe :** P1 (GUI/MVVM), P2 (Backend), P3 (EasyLog), P4 (CryptoSoft)
