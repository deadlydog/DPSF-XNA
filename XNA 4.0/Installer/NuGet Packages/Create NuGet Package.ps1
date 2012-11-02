# Get the absolute path of the DPSF.dll so we can pull the version number from it.
$AbsoluteDLLPath = [System.IO.Path]::GetFullPath("../Installer Files/DPSF.dll")
if (![System.IO.File]::Exists($AbsoluteDLLPath))
{
	Write-Error "File does not exist: '$AbsoluteDLLPath'"
}

# Grab the version number of the DPSF DLL file.
[string]$VersionNumber = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$AbsoluteDLLPath").FileVersion.ToString()

# If the file to create already exists, prompt that we want to overwrite it.
$NugetFilePath = "./Packages/DPSF.$VersionNumber.nupkg"
if ([System.IO.File]::Exists($NugetFilePath))
{
	[string]$Answer = Read-Host "File `"$NugetFilePath`" already exists. Overwrite it? (Y|N): "
	if (!($Answer.StartsWith("Y") -or $Answer.StartsWith("y")))
	{
		Write-Host "ABORTED: Did not create new NuGet package."
		EXIT
	}
}

# Create the nuget package with the proper version number.
NuGet pack "./Package.nuspec" -OutputDirectory Packages -Version "$VersionNumber"

$AbsolutePackagePath = [System.IO.Path]::GetFullPath($NugetFilePath)
Write-Host "Created package: '$AbsolutePackagePath'"
