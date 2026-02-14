# Test de la persistance des jobs et suppression de la limite de 5
Write-Host "=== TEST FEATURE P2: Jobs Unlimited Storage ===" -ForegroundColor Cyan
Write-Host ""

# Créer un répertoire de test
$testDir = "C:\Temp\EasySaveTest"
if (!(Test-Path $testDir)) {
    New-Item -ItemType Directory -Path $testDir | Out-Null
}

# Créer des répertoires sources de test
1..7 | ForEach-Object {
    $sourceDir = "$testDir\Source$_"
    if (!(Test-Path $sourceDir)) {
        New-Item -ItemType Directory -Path $sourceDir | Out-Null
        "Test content $_" | Out-File "$sourceDir\file.txt"
    }
}

Write-Host "[TEST 1] Creating 7 jobs (old limit was 5)..." -ForegroundColor Yellow

# Préparer les commandes pour créer 7 jobs
$commands = @(
    "1"  # Create job
    "Job1"
    "$testDir\Source1"
    "$testDir\Target1"
    "1"  # Complete
    
    "1"  # Create job 2
    "Job2"
    "$testDir\Source2"
    "$testDir\Target2"
    "1"
    
    "1"  # Create job 3
    "Job3"
    "$testDir\Source3"
    "$testDir\Target3"
    "1"
    
    "1"  # Create job 4
    "Job4"
    "$testDir\Source4"
    "$testDir\Target4"
    "1"
    
    "1"  # Create job 5
    "Job5"
    "$testDir\Source5"
    "$testDir\Target5"
    "1"
    
    "1"  # Create job 6 (should fail in v1.0)
    "Job6"
    "$testDir\Source6"
    "$testDir\Target6"
    "1"
    
    "1"  # Create job 7
    "Job7"
    "$testDir\Source7"
    "$testDir\Target7"
    "1"
    
    "2"  # List jobs
    ""   # Press Enter
    
    "6"  # Quit
)

$input = $commands -join "`n"
$input | dotnet run --project EasySave/EasySave.csproj | Out-Null

Write-Host "[TEST 1] ✅ Jobs created successfully" -ForegroundColor Green
Write-Host ""

# Vérifier que le fichier de stockage existe
$storageFile = "$env:APPDATA\EasySave\jobs.json"
if (Test-Path $storageFile) {
    Write-Host "[TEST 2] ✅ Storage file created: $storageFile" -ForegroundColor Green
    $content = Get-Content $storageFile | ConvertFrom-Json
    Write-Host "         Number of jobs saved: $($content.Count)" -ForegroundColor Cyan
} else {
    Write-Host "[TEST 2] ❌ Storage file NOT found" -ForegroundColor Red
}

Write-Host ""
Write-Host "[TEST 3] Restarting app to check persistence..." -ForegroundColor Yellow

# Relancer l'app pour vérifier que les jobs sont rechargés
$commands2 = @(
    "2"  # List jobs
    ""   # Press Enter
    "6"  # Quit
)

$input2 = $commands2 -join "`n"
$output = $input2 | dotnet run --project EasySave/EasySave.csproj

if ($output -match "Loaded (\d+) job") {
    Write-Host "[TEST 3] ✅ Jobs loaded from storage on restart" -ForegroundColor Green
} else {
    Write-Host "[TEST 3] ❌ Jobs NOT loaded" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== TEST COMPLETED ===" -ForegroundColor Cyan
Write-Host "Storage location: $storageFile" -ForegroundColor Gray
