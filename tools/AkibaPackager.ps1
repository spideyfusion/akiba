Param(
    [Parameter(Mandatory=$true)]
    [string]$configurationName,

    [Parameter(Mandatory=$true)]
    [string]$targetPath,
    
    [Parameter(Mandatory=$true)]
    [string]$targetDir,
    
    [Parameter(Mandatory=$true)]
    [string]$targetName,
    
    [Parameter(Mandatory=$true)]
    [string]$targetFileName
)

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

If ($configurationName -cne "Release")
{
    Write-Host "[AkibaPackager] Nothing for me to do yet..."
    Exit 0
}

$deploymentVersion = [System.Reflection.Assembly]::LoadFile($targetPath).GetName().Version

# This will be our final product!
$deploymentItem = "{0}{1}-v{2}.{3}.{4}.zip" -f $targetDir, $targetName, $deploymentVersion.Major, $deploymentVersion.Minor, $deploymentVersion.Build

If (Test-Path $deploymentItem)
{
    # Remove an obsolete release if it exists.
    Remove-Item $deploymentItem
}
  
$zip = [System.IO.Compression.ZipFile]::Open($deploymentItem, [System.IO.Compression.ZipArchiveMode]::Create)

[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $targetPath, $targetFileName, [System.IO.Compression.CompressionLevel]::Optimal) | Out-Null

$zip.Dispose()

Write-Host ("[AkibaPackager] The release is now available at: {0}" -f $deploymentItem) -ForegroundColor Green
