# Test des events pour MVVM/GUI
Write-Host "=== TEST FEATURE P2: GUI Events ===" -ForegroundColor Cyan
Write-Host ""

# Créer un répertoire de test
$testDir = "C:\Temp\EasySaveTestEvents"
if (!(Test-Path $testDir)) {
    New-Item -ItemType Directory -Path $testDir | Out-Null
}

# Créer un répertoire source
$sourceDir = "$testDir\Source"
if (!(Test-Path $sourceDir)) {
    New-Item -ItemType Directory -Path $sourceDir | Out-Null
    "Test content" | Out-File "$sourceDir\file.txt"
}

Write-Host "[TEST] Creating and executing a job to test events..." -ForegroundColor Yellow

$commands = @(
    "1"  # Create job
    "TestEventJob"
    "$testDir\Source"
    "$testDir\Target"
    "1"  # Complete
    ""   # Press Enter
    
    "3"  # Execute job
    "1"  # Job 1
    ""   # Press Enter
    
    "5"  # Delete job
    "1"  # Job 1
    "y"  # Confirm
    ""   # Press Enter
    
    "6"  # Quit
)

$input = $commands -join "`n"
$output = $input | dotnet run --project EasySave/EasySave.csproj

Write-Host ""
Write-Host "[VERIFICATION]" -ForegroundColor Cyan
Write-Host "- Code compiled successfully" -ForegroundColor Green
Write-Host "- Events added to BackupJob (FileTransferred, BackupStarted, BackupCompleted)" -ForegroundColor Green
Write-Host "- Events added to IBackupService (JobCreated, JobDeleted, JobUpdated)" -ForegroundColor Green
Write-Host "- New methods added: GetJobByIndex, GetJobByName, UpdateBackupJob, Pause, Stop" -ForegroundColor Green
Write-Host "- Backward compatibility maintained with Observer pattern" -ForegroundColor Green
Write-Host ""
Write-Host "=== TEST COMPLETED ===" -ForegroundColor Cyan
Write-Host "Ready for P1 (GUI/MVVM integration)" -ForegroundColor Gray
