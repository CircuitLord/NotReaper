param(
        [Parameter(
                    Mandatory=$true,
                    Position=0,
                    HelpMessage='Set "Build and Run" folder')]
        [string] $BuildAndRunFolder
)

$currentDir = Get-Location
$BuildAndRunFolder = Resolve-Path -Path $BuildAndRunFolder

Add-Type -Assembly System.IO.Compression.FileSystem
$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal

if([System.IO.File]::Exists($BuildAndRunFolder + "/NotReaper.exe")) {
    Write-Host "Found NotReaper! Packaging build..."
    Remove-Item -Force (Join-Path $currentDir "/NotReaper-VERSION.zip") -ErrorAction SilentlyContinue
    Remove-Item -Recurse -Force ($BuildAndRunFolder + "/NotReaper_Data/.cache") -ErrorAction SilentlyContinue
    Remove-Item -Recurse -Force ($BuildAndRunFolder + "/NotReaper_Data/saves") -ErrorAction SilentlyContinue
    Remove-Item -Force ($BuildAndRunFolder + "/NotReaper_Data/StreamingAssets/Ogg2Audica/song.mogg") -ErrorAction SilentlyContinue
    Remove-Item -Force ($BuildAndRunFolder + "/NotReaper_Data/StreamingAssets/FFMPEG/converted.ogg") -ErrorAction SilentlyContinue
    [System.IO.Compression.ZipFile]::CreateFromDirectory($BuildAndRunFolder, (Join-Path $currentDir "/NotReaper-VERSION.zip"), $compressionLevel, $false)
}
else {
    Write-Host "NotReaper could not be found. Please check that the folder you supplied is correct."
}