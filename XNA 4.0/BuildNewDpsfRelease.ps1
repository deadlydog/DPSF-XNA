<#
.SYNOPSIS
   This script builds a new Release version for DPSF.
.DESCRIPTION
   This script adjusts the DPSF version to the given version, then builds all of the DPSF DLLs in Release mode.
.PARAMETER <paramName>
   <Description of script parameter>
.EXAMPLE
   <An example of using the script>
#>

# Turn on Strict Mode to help catch syntax-related errors.
# 	This must come after a script's/function's param section.
# 	Forces a function to be the first non-comment code to appear in a PowerShell Script/Module.
Set-StrictMode -Version Latest

# Get the direcotry that this script is in.
$thisScriptsDirectory = Split-Path $script:MyInvocation.MyCommand.Path

# Import the module used to build the .sln files.
$InvokeMsBuildModulePath = Join-Path $thisScriptsDirectory "BuildScriptUtilities/Invoke-MsBuild.psm1"
Import-Module -Name $InvokeMsBuildModulePath


#==========================================================
# Define any necessary global variables, such as file paths.
#==========================================================
$DPSF_ROOT_DIRECTORY = $thisScriptsDirectory
$DPSF_SOLUTION_FILE_PATH = Join-Path $DPSF_ROOT_DIRECTORY "/DPSF/DPSF.sln"
$DPSF_WINRT_SOLUTION_FILE_PATH = Join-Path $DPSF_ROOT_DIRECTORY "/DPSF/DPSF WinRT.sln"
$LATEST_DLL_FILES_DIRECTORY_PATH = Join-Path $DPSF_ROOT_DIRECTORY "/DPSF/LatestDLLBuild/"
$MSBUILD_LOG_DIRECTORY_PATH = $DPSF_ROOT_DIRECTORY
$CSPROJ_FILE_PATHS_TO_MODIFY_AND_REBUILD = @(	(Join-Path $DPSF_ROOT_DIRECTORY "DPSF/DPSF/DPSF.csproj"),
												(Join-Path $DPSF_ROOT_DIRECTORY "DPSF/DPSF/Xbox 360 Copy of DPSF.csproj"), 
												(Join-Path $DPSF_ROOT_DIRECTORY "DPSF/DPSF/Windows Phone Copy of DPSF.csproj"))
$INSTALLER_FILES_DIRECTORY_PATH = Join-Path $DPSF_ROOT_DIRECTORY "/Installer/Installer Files/"


#==========================================================
# Define functions used by the script.
#==========================================================
function UpdateCsprojFileToAsDrawableGameComponent
{
	<#
	.SYNOPSIS
	Edits the given .csproj file to build the AsDrawableGameComponent equivalent dll.

	.DESCRIPTION
	Edits the given .csproj file to build the AsDrawableGameComponent equivalent dll.
	#>
	param (
		[ValidateNotNullOrEmpty()]
		[String] $CsprojFilePath
	)
	
	# Read in the entire file contents
	$fileContents = [System.IO.File]::ReadAllText($CsprojFilePath)
	
	# Replace all of the necessary settings, depending on which csproj file this is.
	if ((Split-Path -Path $CsprojFilePath -Leaf) -eq 'DPSF.csproj')
	{
		$fileContents = $fileContents.Replace('<AssemblyName>DPSF</AssemblyName>', '<AssemblyName>DPSFAsDrawableGameComponent</AssemblyName>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\x86\Debug\DPSF.xml</DocumentationFile>', '<DocumentationFile>bin\x86\Debug\DPSFAsDrawableGameComponent.xml</DocumentationFile>')
		$fileContents = $fileContents.Replace('<DefineConstants>TRACE;WINDOWS</DefineConstants>', '<DefineConstants>TRACE;WINDOWS DPSFAsDrawableGameComponent</DefineConstants>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\x86\Release\DPSF.xml</DocumentationFile>', '<DocumentationFile>bin\x86\Release\DPSFAsDrawableGameComponent.xml</DocumentationFile>')
	} 
	elseif ((Split-Path -Path $CsprojFilePath -Leaf) -eq 'Xbox 360 Copy of DPSF.csproj')
	{
		$fileContents = $fileContents.Replace('<AssemblyName>DPSFXbox360</AssemblyName>', '<AssemblyName>DPSFXbox360AsDrawableGameComponent</AssemblyName>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\Xbox 360\Debug\DPSFXbox360.xml</DocumentationFile>', '<DocumentationFile>bin\Xbox 360\Debug\DPSFXbox360AsDrawableGameComponent.xml</DocumentationFile>')
		$fileContents = $fileContents.Replace('<DefineConstants>TRACE;XBOX;XBOX360</DefineConstants>', '<DefineConstants>TRACE;XBOX;XBOX360 DPSFAsDrawableGameComponent</DefineConstants>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\Xbox 360\Release\DPSFXbox360.xml</DocumentationFile>', '<DocumentationFile>bin\Xbox 360\Release\DPSFXbox360AsDrawableGameComponent.xml</DocumentationFile>')
	}
	elseif ((Split-Path -Path $CsprojFilePath -Leaf) -eq 'Windows Phone Copy of DPSF.csproj')
	{
		$fileContents = $fileContents.Replace('<AssemblyName>DPSFPhone</AssemblyName>', '<AssemblyName>DPSFPhoneAsDrawableGameComponent</AssemblyName>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\Windows Phone\Debug\DPSFPhone.xml</DocumentationFile>', '<DocumentationFile>bin\Windows Phone\Debug\DPSFPhoneAsDrawableGameComponent.xml</DocumentationFile>')
		$fileContents = $fileContents.Replace('<DefineConstants>TRACE;WINDOWS_PHONE</DefineConstants>', '<DefineConstants>TRACE;WINDOWS_PHONE DPSFAsDrawableGameComponent</DefineConstants>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\Windows Phone\Release\DPSFPhone.xml</DocumentationFile>', '<DocumentationFile>bin\Windows Phone\Release\DPSFPhoneAsDrawableGameComponent.xml</DocumentationFile>')
	}
	else
	{
		# Display an error.
		Write-Error "ERROR: Invalid .csproj file name given. Cannot find the file '$CsprojFilePath'."
	}	
	
	# Write the new file contents to the file.
	[System.IO.File]::WriteAllText($CsprojFilePath, $fileContents)
}


#==========================================================
# Perform the script tasks.
#==========================================================

# Delete the existing DPSF files before building the new ones.
Write-Host "Deleting existing DLLs..."
Remove-Item -Recurse -Path "$LATEST_DLL_FILES_DIRECTORY_PATH"					# Delete the entire folder.
New-Item -ItemType Directory -Path "$LATEST_DLL_FILES_DIRECTORY_PATH" > $null	# Recreate the empty folder (and trash the output it creates).

# Build the DPSF solution in Release mode to create the new DLLs.
Write-Host "Building the DPSF solution..."
Invoke-MsBuild -Path "$DPSF_SOLUTION_FILE_PATH" -Configuration "Release" -BuildLogDirectoryPath "$MSBUILD_LOG_DIRECTORY_PATH" -BuildVerbosity Quiet -ShowBuildWindow
Write-Host "Building the DPSF WinRT solution..."
Invoke-MsBuild -Path "$DPSF_WINRT_SOLUTION_FILE_PATH" -Configuration "Release" -BuildLogDirectoryPath "$MSBUILD_LOG_DIRECTORY_PATH" -BuildVerbosity Quiet -ShowBuildWindow

# Update the .csproj files' to build the AsDrawableGameComponent DLLs.
Write-Host "Updating the .csproj files to build AsDrawableGameComponent DLLs..."
foreach ($csprojFilePath in $CSPROJ_FILE_PATHS_TO_MODIFY_AND_REBUILD)
{
	# Backup the file before modifying it.
	Copy-Item -Path $csprojFilePath -Destination "$CsprojFilePath.backup"
	
	UpdateCsprojFileToAsDrawableGameComponent $csprojFilePath
}

# Rebuild the solution to create the AsDrawableGameComponent DLLs.
Write-Host "Building the DPSF solution for AsDrawableGameComponent DLLs..."
Invoke-MsBuild -Path "$DPSF_SOLUTION_FILE_PATH" -Configuration "Release" -BuildLogDirectoryPath "$MSBUILD_LOG_DIRECTORY_PATH" -BuildVerbosity Quiet -ShowBuildWindow

#Revert the .csproj files back to their original states now that we have the DLLs.
Write-Host "Reverting .csproj files back to their original states..."
foreach ($csprojFilePath in $CSPROJ_FILE_PATHS_TO_MODIFY_AND_REBUILD)
{
	# Copy the backup back overtop of the original to revert it, and then delete the backup file.
	if (Test-Path "$csprojFilePath.backup")
	{
		Copy-Item -Path "$csprojFilePath.backup" -Destination "$csprojFilePath"
		Remove-Item -Path "$csprojFilePath.backup"
	}
}

# Copy the DLL files to the 'Installer Files' directory.
Write-Host "Copying new DLL and XML files to the Installer Files directory..."
Copy-Item -Path "$LATEST_DLL_FILES_DIRECTORY_PATH/*" -Destination $INSTALLER_FILES_DIRECTORY_PATH -Include "*.dll","*.xml"







# Wait for input before closing.
Write-Host "Press any key to continue ..."
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyUp")