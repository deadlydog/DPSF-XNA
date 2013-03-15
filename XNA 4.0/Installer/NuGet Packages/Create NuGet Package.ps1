# Get the absolute path of the DPSF.dll so we can pull the version number from it.
$AbsoluteDLLPath = [System.IO.Path]::GetFullPath("../Installer Files/DPSF.dll")
if (![System.IO.File]::Exists($AbsoluteDLLPath))
{
	Write-Error "File does not exist: '$AbsoluteDLLPath'"
}

# Grab the version number of the DPSF DLL file.
[string]$VersionNumber = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$AbsoluteDLLPath").FileVersion.ToString()

# Get the directory that this script is in.
$thisScriptsDirectory = Split-Path $script:MyInvocation.MyCommand.Path

# If the file to create already exists, prompt that we want to overwrite it.
$NugetFilePath = "$thisScriptsDirectory/Packages/DPSF.$VersionNumber.nupkg"
if (Test-Path $NugetFilePath)
{
	[string]$answer = Read-Host "File `"$NugetFilePath`" already exists. Overwrite it? (Y|N): "
	if (!($answer.StartsWith("Y") -or $Answer.StartsWith("y")))
	{
		Write-Host "ABORTED: Did not create new NuGet package."
		EXIT
	}
}

# Create the nuget package with the proper version number.
NuGet pack "$thisScriptsDirectory/Package.nuspec" -OutputDirectory "$thisScriptsDirectory/Packages" -Version "$VersionNumber"

$AbsolutePackagePath = [System.IO.Path]::GetFullPath($NugetFilePath)
Write-Host "Created package: '$AbsolutePackagePath'"
