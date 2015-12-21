Param
(
    [String] $Configuration,
    [String] $DestinationDir = "",
    [Bool] $DeleteOriginalAfterZip = $true
)

$runningDirectory = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition

if ($DestinationDir -eq "") 
{
    $DestinationDir = $runningDirectory + "\release"
}
elseif ($DestinationDir.StartsWith(".")) 
{
     $DestinationDir = $runningDirectory + "\" + $DestinationDir.Substring(1)
}

$DestinationDir = [System.IO.Path]::GetFullPath($DestinationDir)
$DestinationDirExe = $DestinationDir + "\MDbGuiNet"

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

$appDir = $DestinationDirExe.ToString() + "\app"
New-Item -Force -ItemType directory -Path $appDir

Write-Host "Destination dir is  $DestinationDirExe"

Write-Host "Cleaning up $DestinationDirExe"
Get-ChildItem $DestinationDirExe -Include *.xml | foreach ($_) {remove-item $_.fullname}


