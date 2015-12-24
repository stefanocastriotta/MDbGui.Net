Param
(
    [String] $Configuration,
    [String] $DestinationDir = "",
    [Bool] $DeleteOriginalAfterZip = $true
)

$runningDirectory = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition

if ($DestinationDir -eq "") 
{
    $DestinationDir = $runningDirectory + "\$Configuration"
}
elseif ($DestinationDir.StartsWith(".")) 
{
     $DestinationDir = $runningDirectory + "\" + $DestinationDir.Substring(1)
}

$DestinationDir = [System.IO.Path]::GetFullPath($DestinationDir)
$DestinationDirExe = $DestinationDir + "\MDbGuiNet"
$DestinationDirClickOnce = $DestinationDir + "\MDbGuiNetClickOnce"

Write-Host "Destination dir is $DestinationDir"

if(Get-Module -name commonUtils) 
{
    Remove-Module -Name "commonUtils"
}

if(-not(Get-Module -name commonUtils)) 
{
    Import-Module -Name ".\commonUtils"
}

if(Test-Path -Path $DestinationDir )
{
    Remove-Item $DestinationDir -Recurse
}

$DestinationDir = $DestinationDir.TrimEnd('/', '\')

New-Item -ItemType Directory -Path $DestinationDir
New-Item -ItemType Directory -Path $DestinationDirExe

Copy-Item ".\..\..\src\MDbGui.Net\bin\$configuration\*.*" `
    $DestinationDirExe `
    -Force -Recurse

Copy-Item ".\..\..\src\MDbGui.Net\Resources\" "$DestinationDirExe\Resources\" -Force -Recurse

Write-Host "Destination dir is  $DestinationDirExe"

Move-Item "$DestinationDirExe\app.publish" "$DestinationDirClickOnce\" -Force

Write-Host "Cleaning up $DestinationDirExe"
Get-ChildItem $DestinationDirExe -Include *.xml | foreach ($_) {remove-item $_.fullname}


