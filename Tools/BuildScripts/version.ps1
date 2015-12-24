Write-Output "Starting versioning script"
if(Get-Module -name BuildFunctions) 
{
    Remove-Module BuildFunctions
}
if(-not(Get-Module -name BuildFunctions)) 
{
    Import-Module -Name ".\BuildFunctions"
}
Write-Output "Invoking GitVersion.exe"
$Output = & ..\GitVersion\Gitversion.exe /nofetch | Out-String
Write-Output ("GitVersion returned:  " + $output)


$version = $output | ConvertFrom-Json
$assemblyVersion = $version.AssemblySemver
$assemblyFileVersion = $version.AssemblySemver
$assemblyInformationalVersion = ($version.SemVer + "/" + $version.Sha)
$preReleaseTag = $version.PreReleaseTag

$splitted = $preReleaseTag.Split('.')
$preReleaseNum = $splitted[$splitted.Length - 1]
if ([string]::IsNullOrEmpty($preReleaseNum))
{
    $preReleaseNum = "0"
}
$clickOnceVersion = $assemblyVersion + "." + $preReleaseNum

Write-Output "Pre Release Number is: $preReleaseNum"
Write-Output "SemVer - Assembly and File: $assemblyVersion Informational: $assemblyInformationalVersion"
Write-Output "Click Once Version: $clickOnceVersion"

Write-Output "Nuget Version: " + $version.NuGetVersionV2

Write-Output ("##vso[task.setvariable variable=NugetVersion;]" + $version.NugetVersionV2)
Write-Output ("##vso[task.setvariable variable=AssemblyVersion;]" + $assemblyVersion)
Write-Output ("##vso[task.setvariable variable=FileInfoVersion;]" + $assemblyFileVersion)
Write-Output ("##vso[task.setvariable variable=AssemblyInformationalVersion;]" + $assemblyInformationalVersion)
Write-Output ("##vso[task.setvariable variable=ClickOnceVersion;]" + $clickOnceVersion)

#change build number.
Write-Output ("##vso[task.setvariable variable=build.buildnumber;]" + $version.FullSemVer)
Write-Output ("##vso[build.updatebuildnumber]" + $version.FullSemVer)

Update-SourceVersion ..\..\src $assemblyVersion $assemblyFileVersion $assemblyInformationalVersion
