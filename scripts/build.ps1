$ErrorActionPreference = "Stop"

$rootDir = "."
$thunderstoreDir = "$rootDir/thunderstore"

$buildDir = "$rootDir/Cyberhead/bin/Release/net462"
$patcherBuildDir = "$rootDir/Cyberhead.Patcher/bin/Release/net462"
$outDir = "$rootDir/out"
$bepinexDir = "$thunderstoreDir/BepInEx"
$thunderstorePluginDir = "$bepinexDir/plugins/Cyberhead"
$thunderstorePatcherDir = "$bepinexDir/patchers/Cyberhead"

# Clean up everything
if (Test-Path $buildDir) {
    Remove-Item $buildDir -Recurse -Force
}

if (Test-Path $outDir) {
    Remove-Item $outDir -Recurse -Force
}

if (Test-Path $thunderstorePluginDir) {
    Remove-Item $thunderstorePluginDir -Recurse -Force
}

if (Test-Path $thunderstorePatcherDir) {
    Remove-Item $thunderstorePatcherDir -Recurse -Force
}

New-Item $buildDir -ItemType Directory -Force
New-Item $outDir -ItemType Directory -Force
New-Item $thunderstorePluginDir -ItemType Directory -Force
New-Item $thunderstorePatcherDir -ItemType Directory -Force

dotnet clean --configuration Release
dotnet build --configuration Release

# GitHub release
$version = (Get-Item "$buildDir/Cyberhead.dll").VersionInfo.ProductVersion
$version = $version -replace '\+.*$'

# Thunderstore release
Copy-Item "$buildDir/*" "$thunderstorePluginDir/" -Recurse -Force
Copy-Item "$patcherBuildDir/*" "$thunderstorePatcherDir/" -Recurse -Force
Copy-Item "$rootDir/README.md" "$thunderstoreDir/README.md"
# Copy-Item "$rootDir/CHANGELOG.md" "$thunderstoreDir/CHANGELOG.md"

# Edit the Thunderstore JSON's `version_number` property to match
$manifest = "$thunderstoreDir/manifest.json"
$edited = (jq --arg version $version '.version_number = $version' $manifest)

# Write the edited JSON
# PowerShell struggles with LF with no BOM, but I will will it anyways
$lines = $edited -split "`r`n"
$stream = [System.IO.StreamWriter] $manifest

foreach ($line in $lines) {
    $stream.Write($line)
    $stream.Write("`n")
}

$stream.Close()

Compress-Archive -Path "$bepinexDir/*" -DestinationPath "$outDir/plugin.zip" -Force
Compress-Archive -Path "$thunderstoreDir/*" -DestinationPath "$outDir/thunderstore.zip" -Force
