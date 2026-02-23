# 📋 Release Notes - EasySave

## Version 2.0 - 14 février 2026

**Type de release :** Release majeure  
**Statut :** Production stable  
**Build :** 2.0.0  

---

## 🎉 Nouveautés majeures

### Interface Graphique WPF

Une toute nouvelle interface graphique moderne basée sur WPF et le pattern MVVM.

- ✨ **Interface utilisateur moderne et intuitive**
  - Design épuré et professionnel
  - Navigation fluide entre les jobs
  - Thème moderne avec contraste optimisé

- 📊 **Panneau de détails interactif**
  - Informations complètes sur chaque job
  - Statut en temps réel
  - Progression détaillée avec compteur de fichiers

- 🖱️ **Interactions avancées**
  - Double-clic sur les chemins pour ouvrir l'Explorateur Windows
  - Sélection intuitive des dossiers avec dialogue natif
  - Actualisation sans perte d'état des jobs en cours

### Gestion illimitée des jobs

- ♾️ **Suppression de la limite de 5 jobs**
  - Créez autant de jobs que nécessaire
  - Organisez vos sauvegardes selon vos besoins
  - Stockage persistant dans `%APPDATA%\EasySave\jobs.json`

### Cryptage sélectif par extension

- 🔐 **Nouveau système de cryptage intelligent**
  - Configuration via `appsettings.json`
  - Cryptage automatique basé sur les extensions de fichiers
  - Liste personnalisable d'extensions à protéger
  
- 🎯 **Cryptage ciblé**
  - Seuls les fichiers configurés sont cryptés
  - Les autres fichiers restent en clair
  - Performance optimisée

**Exemple de configuration :**
```json
{
  "ExtensionsToEncrypt": [".doc", ".docx", ".xls", ".xlsx", ".pdf", ".txt"]
}
```

### Système de logging amélioré

- 📝 **EasyLog v1.1 intégré**
  - Support des formats JSON et XML
  - Logs détaillés avec horodatage
  - Enregistrement du temps de cryptage
  - Événements de job (démarrage, pause, interruption, complétion)

### Détection de logiciel métier

- 🚫 **Pause automatique des sauvegardes**
  - Configuration du nom du processus à surveiller
  - Mise en pause automatique si détecté
  - Reprise automatique quand le logiciel est fermé

---

## 🔧 Améliorations

### Performance

- ⚡ **Exécution asynchrone**
  - Interface non bloquante pendant les sauvegardes
  - Utilisation de `Task.Run()` pour les opérations longues
  - Meilleure réactivité de l'interface

- 📈 **Optimisations diverses**
  - Chargement plus rapide des jobs
  - Actualisation intelligente de la liste
  - Gestion mémoire optimisée

### Expérience utilisateur

- ✅ **Validation des entrées**
  - Vérification de l'existence des dossiers
  - Messages d'erreur clairs et explicites
  - Prévention des configurations invalides

- 🔄 **Actualisation intelligente**
  - Préservation de l'état des jobs en cours
  - Pas de perte de progression lors du refresh
  - Mise à jour sans interruption

- 📱 **Fenêtres redimensionnables**
  - Taille adaptative des composants
  - GridSplitter pour ajuster les panneaux
  - Expérience utilisateur améliorée

### Stabilité

- 🛡️ **Gestion d'erreurs robuste**
  - Try-catch généralisés
  - Messages d'erreur informatifs
  - Récupération gracieuse des erreurs

- 💾 **Persistance fiable**
  - Sauvegarde automatique des jobs
  - Protection contre la corruption de données
  - Création automatique des dossiers nécessaires

---

## 🐛 Corrections de bugs

### Critiques

- ✅ **Fix : NullReferenceException lors de la suppression**
  - Résolu : Stockage du nom du job avant suppression
  - Impact : Empêchait la suppression de jobs

- ✅ **Fix : Statut non mis à jour après complétion**
  - Résolu : Actualisation intelligente sans recréation des ViewModels
  - Impact : Améliore le suivi de progression

- ✅ **Fix : UI bloquée pendant l'exécution**
  - Résolu : Exécution asynchrone avec `Task.Run()`
  - Impact : Interface responsive pendant les sauvegardes

### Interface utilisateur

- ✅ **Fix : Bouton "Créer" invisible dans CreateJobDialog**
  - Résolu : Augmentation de la hauteur de la fenêtre à 480px
  - Ajout de `SizeToContent="Height"`
  - Impact : Tous les boutons sont maintenant visibles

- ✅ **Fix : Texte illisible sur sélection (bleu sur bleu)**
  - Résolu : DataTrigger changeant la couleur en blanc sur sélection
  - Impact : Meilleure lisibilité des chemins sélectionnés

### Backend

- ✅ **Fix : Logger instantiation incorrecte**
  - Résolu : Utilisation de `LoggerFactory.CreateLogger`
  - Impact : Logging fonctionnel avec EasyLog v1.1

- ✅ **Fix : StateObserver avec méthode inexistante**
  - Résolu : Remplacement par `UpdateState()`
  - Impact : Persistence d'état fonctionnelle

---

## 🔄 Changements techniques

### Architecture

- 🏗️ **Pattern MVVM complet**
  - `BaseViewModel` avec `INotifyPropertyChanged`
  - `RelayCommand` et `AsyncRelayCommand`
  - Séparation claire Model-View-ViewModel

- 🔌 **Dependency Injection**
  - Configuration dans `App.xaml.cs`
  - Services enregistrés comme Singleton
  - ViewModels résolus automatiquement

### Nouvelles dépendances

- ➕ **Microsoft.Extensions.DependencyInjection** (8.0.0)
  - Gestion du conteneur DI
  - Injection de services

- ➕ **EasyLog v1.1**
  - Architecture Logger/Formatter
  - Support JobEventType
  - Logging amélioré

### Structure du projet

```
EasySave.sln
├── EasySave/              # Backend (Models, Services, Strategies)
├── EasySave.GUI/          # Frontend WPF (Views, ViewModels, Commands)
└── CryptoSoft/            # Outil de cryptage externe
```

---

## 📦 Fichiers de la release

### Contenu du package

```
EasySave_v2.0_Release.zip (0.27 MB)
├── EasySave.GUI.exe       # Application principale
├── EasySave.dll           # Bibliothèque backend
├── EasyLog.dll            # Bibliothèque de logging
├── Microsoft.*.dll        # Dépendances .NET
└── appsettings.json       # Configuration (créé au premier lancement)
```

### Documentation incluse

- 📘 **MANUEL_UTILISATEUR.md** - Guide complet pour utilisateurs
- 📄 **README.md** - Documentation technique et installation
- 📋 **RELEASE_NOTES.md** - Ce fichier
- 🚀 **PUBLISH_GUIDE.md** - Guide de publication

---

## ⚙️ Configuration requise

### Système

- **OS :** Windows 10/11 (64-bit)
- **RAM :** 512 MB minimum, 1 GB recommandé
- **Espace disque :** 50 MB pour l'application + espace pour les sauvegardes
- **Résolution :** 1280x720 minimum, 1920x1080 recommandé

### Runtime

- **.NET Desktop Runtime 8.0** ou supérieur
- Télécharger : https://dotnet.microsoft.com/download/dotnet/8.0

---

## 🚀 Migration depuis v1.0

### Changements incompatibles

- ❌ **Interface console supprimée**
  - Remplacée par l'interface graphique WPF
  - L'exécutable CLI `EasySave.exe` existe toujours mais nécessite une configuration manuelle

### Migration des données

- ✅ **Jobs existants compatibles**
  - Le fichier `jobs.json` est compatible
  - Emplacement : `%APPDATA%\EasySave\jobs.json`
  - Pas de migration nécessaire

- ✅ **Configuration**
  - Nouveau fichier `appsettings.json` à créer manuellement
  - Voir documentation pour les détails

### Procédure de migration

1. **Sauvegarder** vos jobs existants
   ```
   Copier %APPDATA%\EasySave\jobs.json
   ```

2. **Installer** EasySave v2.0
   - Extraire le nouveau package
   - Installer .NET 8.0 si nécessaire

3. **Vérifier** les jobs
   - Lancer EasySave.GUI.exe
   - Vérifier que tous les jobs apparaissent

4. **Configurer** le cryptage (optionnel)
   - Créer `appsettings.json`
   - Ajouter les extensions à crypter

---

## 🔮 Fonctionnalités à venir

### Prévues pour v2.1

- 🎨 **Interface de configuration graphique**
  - Édition de `appsettings.json` via GUI
  - Configuration des extensions à crypter
  - Paramètres de logging

- ✏️ **Modification de jobs**
  - Éditer les jobs existants sans recréation
  - Modification du type de sauvegarde

- 📅 **Planification**
  - Sauvegardes automatiques planifiées
  - Intégration avec le planificateur Windows

### Futures versions

- 🌐 **Support multi-langue**
  - Interface en français et anglais
  - Localisation des messages

- 📊 **Statistiques détaillées**
  - Historique des sauvegardes
  - Graphiques de performance
  - Espace disque utilisé

- ☁️ **Cloud storage**
  - Support de destinations cloud
  - OneDrive, Google Drive, Dropbox

---

## ⚠️ Problèmes connus

### Interface

- 🐛 **Double-clic sur chemins ne fonctionne pas toujours**
  - **Workaround :** Copier-coller le chemin dans l'Explorateur
  - **Fix prévu :** v2.0.1

### Cryptage

- ⚠️ **Décryptage non implémenté**
  - Le décryptage des fichiers n'est pas encore disponible
  - **Recommandation :** Conserver une copie non cryptée des fichiers importants
  - **Fix prévu :** v2.1

### Divers

- 🐛 **Logs : Les accents peuvent être mal encodés**
  - **Impact :** Affichage incorrect dans certains éditeurs
  - **Workaround :** Utiliser un éditeur UTF-8 (VS Code, Notepad++)

---

## 📞 Support et contact

### Obtenir de l'aide

- 📧 **Email :** support@prosoft.example.com
- 🐛 **Issues GitHub :** https://github.com/Cesi-Genie-Logiciel/easySave_Cesi/issues
- 📚 **Documentation :** Voir `MANUEL_UTILISATEUR.md`

### Rapporter un bug

Merci d'inclure :
- Version d'EasySave (2.0)
- Version de Windows
- Description détaillée du problème
- Étapes pour reproduire
- Logs (`%APPDATA%\EasySave\logs\`)

---

## 👥 Crédits

### Équipe de développement

- **P1** - Interface GUI/MVVM
- **P2** - Backend, Settings, Storage
- **P3** - EasyLog v1.1, Logging
- **P4** - CryptoSoft, Cryptage

### Remerciements

Merci à tous les contributeurs et testeurs qui ont participé au développement de cette version.

---

## 📄 Licence

© 2026 ProSoft - Tous droits réservés

Ce logiciel est développé dans le cadre d'un projet académique CESI.

---

## 🔗 Liens utiles

- [Repository GitHub](https://github.com/Cesi-Genie-Logiciel/easySave_Cesi)
- [EasyLog Repository](https://github.com/Cesi-Genie-Logiciel/easyLog_Cesi)
- [Documentation .NET](https://docs.microsoft.com/dotnet/)
- [Guide WPF](https://docs.microsoft.com/wpf/)

---

<div align="center">
  <p><strong>Version 2.0 - Release Finale</strong></p>
  <p>Développé avec ❤️ par l'équipe EasySave</p>
  <p>ProSoft - Solutions professionnelles de sauvegarde</p>
</div>
