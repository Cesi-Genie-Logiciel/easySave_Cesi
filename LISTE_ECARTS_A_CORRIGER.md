# Liste des écarts à corriger (conformité schéma)

Branche : `feat/conformite-schema-v3`

---

## 1. GUI – ViewModels / Commandes

- [ ] **BackupJobViewModel** : ajouter **PauseCommand** et **StopCommand** (le XAML les lie déjà ; appeler `_model.Pause()` / `_model.Stop()`).
- [ ] **MainViewModel** : ajouter **OpenSettingsCommand** (ouvre la fenêtre Paramètres).
- [ ] **MainWindow** : ajouter un bouton « Paramètres » lié à `OpenSettingsCommand`.

---

## 2. GUI – Écran Paramètres

- [ ] Créer **SettingsView** (fenêtre ou UserControl) avec champs : LogFormat, LogDestination, LogServerUrl, PriorityExtensions, ExtensionsToEncrypt, LargeFileThresholdKo, BusinessSoftwareName.
- [ ] Créer **SettingsViewModel** avec propriétés liées à AppSettings + **SaveCommand** et **CancelCommand**, utilisant **ISettingsService** (Load / Save).
- [ ] Enregistrer SettingsViewModel dans le DI (App.xaml.cs) et injecter **ISettingsService** dans MainViewModel pour OpenSettingsCommand.

---

## 3. Modèle AppSettings

- [ ] Ajouter **PriorityExtensions** : `List<string>` (défaut vide).
- [ ] Ajouter **LargeFileThresholdKo** : `long` (défaut ex. 1024).
- [ ] Vérifier que **SettingsService** sérialise/désérialise ces propriétés (automatique avec System.Text.Json).

---

## 4. BackupJobState et observers

- [ ] Créer l’enum **BackupJobState** (Pending, Running, Paused, Stopped, Completed, Error) dans `EasySave.Models`.
- [ ] Étendre **IBackupObserver** avec **OnBackupStateChanged(string backupName, BackupJobState state)**.
- [ ] Implémenter **OnBackupStateChanged** (corps vide ou log) dans **ConsoleObserver**, **LoggerObserver**, **StateObserver**.
- [ ] (Optionnel) Dans **BackupJob**, appeler les observers avec OnBackupStateChanged lors des changements d’état.

---

## 5. IBackupService / BackupService

- [ ] Ajouter à **IBackupService** : **ResumeBackupJob(BackupJob job)**.
- [ ] Ajouter à **IBackupService** : **PauseAllBackupJobs()**, **ResumeAllBackupJobs()**, **StopAllBackupJobs()**.
- [ ] Implémenter dans **BackupService** (déléguer au coordinateur ou parcourir les jobs et appeler Pause/Resume/Stop).

---

## 6. BackupExecutionContext

- [ ] Ajouter **PauseEvent** : `ManualResetEventSlim?` (optionnel pour ne pas casser l’existant).
- [ ] Ajouter **PriorityManager** : `PriorityFileManager?`, **BandwidthGuard** : `LargeFileTransferGuard?`, **ExtensionsToEncrypt** : `List<string>?` (optionnels, null par défaut).
- [ ] Adapter **ParallelBackupCoordinator.BuildExecutionContext** pour renseigner ces champs si besoin (sinon laisser null).

---

## 7. LogCentralizer (Docker)

- [ ] Ajouter à **ILogRepository** : **SaveState(string clientId, BackupState state)**.
- [ ] Implémenter **SaveState** dans **FileLogRepository** (écrire un fichier état par client/date par ex.).
- [ ] Créer ou réutiliser un modèle **BackupState** côté LogCentralizer (nom, timestamp, state, totalFiles, etc.).
- [ ] Ajouter dans **LogController** l’endpoint **PostState** (POST) acceptant clientId + state, appelant `_logRepository.SaveState`.

---

## 8. Optionnel (schéma)

- [ ] **CreateJobViewModel** : extraire la logique du dialogue CreateJobDialog dans un ViewModel dédié (MVVM strict).
- [ ] **IBackupStrategy.ExecuteBackup** : ajouter un paramètre optionnel **BackupExecutionContext** (et l’utiliser dans les stratégies si le coordinateur le fournit).
- [ ] **BackupJob.Execute** : accepter un paramètre optionnel **BackupExecutionContext** pour utilisation future (pause, priorités).

---

## Ordre suggéré

1. AppSettings (3) + BackupJobState + IBackupObserver (4)  
2. BackupJobViewModel Pause/Stop (1)  
3. IBackupService + BackupService (5)  
4. SettingsView + SettingsViewModel + OpenSettingsCommand (2)  
5. BackupExecutionContext (6)  
6. LogCentralizer PostState / SaveState (7)  
7. Optionnel (8)
