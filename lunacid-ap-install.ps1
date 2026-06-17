# LunacidAPClient Installer/Uninstaller
$AppId = "1745510"
$DefaultGameDir = "$env:ProgramFiles(x86)\Steam\steamapps\common\Lunacid"
$ApiUrl = "https://api.github.com/repos/Witchybun/LunacidAPClient/releases"

$GameDir = $DefaultGameDir
if (-Not (Test-Path $GameDir)) {
    Add-Type -AssemblyName System.Windows.Forms
    $FolderBrowser = New-Object System.Windows.Forms.FolderBrowserDialog
    $FolderBrowser.Description = "Select the Lunacid game directory"
    if ($FolderBrowser.ShowDialog() -eq "OK") { $GameDir = $FolderBrowser.SelectedPath }
    else { Write-Error "No game directory selected. Exiting."; exit }
}

if (-Not (Test-Path (Join-Path $GameDir "Lunacid.exe"))) {
    Write-Error "$GameDir does not contain Lunacid.exe. Exiting."
    exit
}

Write-Host "Using game directory: $GameDir"
$PluginDir = Join-Path $GameDir "BepInEx\plugins\LunacidAP"

if (Test-Path $PluginDir) {
    Write-Host "1) Install/Update LunacidAPClient"
    Write-Host "2) Uninstall LunacidAPClient"
    $Action = Read-Host "Select an option [1/2]"
    if ($Action -eq "2") {
        Write-Host "Removing LunacidAPClient from $PluginDir ..."
        Remove-Item -Recurse -Force $PluginDir
        Write-Host "Uninstall completed."
        exit
    }
}

Write-Host "Installing/Updating LunacidAPClient..."

$ReleasesJson = Invoke-RestMethod -Uri $ApiUrl -UseBasicParsing

$Releases = $ReleasesJson | Sort-Object {[datetime]$_.published_at} -Descending
$Tags = $Releases.tag_name

$Cols = 3
$Width = 25
$Rows = [math]::Ceiling($Tags.Count / $Cols)
for ($r=0; $r -lt $Rows; $r++) {
    $line = ""
    for ($c=0; $c -lt $Cols; $c++) {
        $i = $c*$Rows + $r
        if ($i -lt $Tags.Count) {
            $label = $Tags[$i]
            if ($i -eq 0) { $label += " (latest)" }
            $line += ("[{0}] {1,-25}" -f $i, $label)
        }
    }
    Write-Host $line
}

$IndexInput = Read-Host "Select a version number [ENTER for latest]"
if ([string]::IsNullOrWhiteSpace($IndexInput)) { $Index = 0 }
else {
    $IndexInput = $IndexInput.Trim()
    if (-Not ($IndexInput -as [int]) -or [int]$IndexInput -ge $Tags.Count) {
        Write-Error "Invalid selection."
        exit
    }
    $Index = [int]$IndexInput
}

$SelectedTag = $Tags[$Index]
Write-Host "Selected version: $SelectedTag"

$Assets = $Releases | Where-Object { $_.tag_name -eq $SelectedTag } | Select-Object -First 1
$DownloadUrl = $Assets.assets | Where-Object { $_.name -match "^Lunacid.*\.zip$" } | Select-Object -First 1 -ExpandProperty browser_download_url
if (-Not $DownloadUrl) { Write-Error "No valid Lunacid*.zip asset found."; exit }

$TmpZip = Join-Path $env:TEMP ("lunacid_$SelectedTag.zip")
Invoke-WebRequest -Uri $DownloadUrl -OutFile $TmpZip

if (Test-Path $TmpZip) {
    if (Test-Path $PluginDir) { Remove-Item -Recurse -Force $PluginDir }
    Expand-Archive -LiteralPath $TmpZip -DestinationPath $GameDir -Force
    Remove-Item $TmpZip
}

Write-Host "LunacidAPClient ($SelectedTag) installation completed successfully."
