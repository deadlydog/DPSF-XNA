<#
.SYNOPSIS
   This script builds a new Release version for DPSF.
.DESCRIPTION
   This script adjusts the DPSF version to the given version, then builds all of the DPSF DLLs in Release mode.
.PARAMETER VersionNumber
   The new Version Number to give the DPSF DLLs.
   If null or empty, the assembly version number will not be updated.
.EXAMPLE
   <An example of using the script>
#>

param
(
	[parameter(Position=0,Mandatory=$false,HelpMessage="The new 4 hex-value version number to build the DPSF assemblies with.")]
	[ValidateScript({$_ -match "^\d{1,5}\.\d{1,5}\.\d{1,5}\.\d{1,5}$"})]
	[String] $VersionNumber
)

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
$CSPROJ_FILE_PATHS_TO_MODIFY_AND_REBUILD = @( (Join-Path $DPSF_ROOT_DIRECTORY "DPSF/DPSF/DPSF.csproj"),
	(		Join-Path $DPSF_ROOT_DIRECTORY "DPSF/DPSF/Xbox 360 Copy of DPSF.csproj"), 
	(		Join-Path $DPSF_ROOT_DIRECTORY "DPSF/DPSF/Windows Phone Copy of DPSF.csproj"))
$INSTALLER_FILES_DIRECTORY_PATH = Join-Path $DPSF_ROOT_DIRECTORY "/Installer/Installer Files/"
$DPSF_COMMON_ASSEMBLY_INFO_FILE_PATH = Join-Path $DPSF_ROOT_DIRECTORY "/DPSF/DPSF/CommonAssemblyInfo.cs"


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
	# Throw an error.
	Throw "ERROR: Invalid .csproj file name given. Cannot find the file '$CsprojFilePath'."
} 

# Write the new file contents to the file.
[System.IO.File]::WriteAllText($CsprojFilePath, $fileContents)
}

function DpsfVersionNumber
{
 <#
	.SYNOPSIS
	Gets/Sets the version number in the DPSF CommonAssemblyInfo.cs file.
	
	.DESCRIPTION
	Gets/Sets the version number in the DPSF CommonAssemblyInfo.cs file.
#>

[CmdletBinding(DefaultParameterSetName="Get")]
param
(
	[parameter(Position=0,Mandatory=$true,ParameterSetName="Get")]
	[Switch] $GetVersionNumber,

	[parameter(Position=0,Mandatory=$true,ParameterSetName="Set")]
	[Switch] $SetVersionNumber,

	[parameter(Position=1,Mandatory=$true,ParameterSetName="Set")]
	[ValidateScript({$_ -match "^\d{1,5}\.\d{1,5}\.\d{1,5}\.\d{1,5}$"})]
	[String] $NewVersionNumber
)

	# If we can't find the DPSF Common Assembly Info file, display an error and exit.
	if (!(Test-Path -Path $DPSF_COMMON_ASSEMBLY_INFO_FILE_PATH))
	{
		Throw "Cannot find the DPSF Common Assembly Info file at '$DPSF_COMMON_ASSEMBLY_INFO_FILE_PATH'."
	}
	
	$versionNumberLineRegex = [regex]'\[assembly: AssemblyVersion\("(?<VersionNumber>.*?)"\)\]'
	$fileContents = [System.IO.File]::ReadAllText($DPSF_COMMON_ASSEMBLY_INFO_FILE_PATH)
	
	# If we were able to find the version number in the DPSF Comoon Assembly Info file.
	$match = $versionNumberLineRegex.Match($fileContents)
	if ($match.Success)
	{
		# Get the current Version Number.
		$currentVersionNumber = $match.Groups["VersionNumber"].Value

		# If we just want to Get the version number, return it.
		if ($GetVersionNumber)
		{
			return $currentVersionNumber
		}
		# Otherwise we want to set it.
		else
		{
			# If the Version Number is already what we want to set it to, don't do anything.
			if ($currentVersionNumber -eq $NewVersionNumber)
			{
				return $currentVersionNumber
			}
	
			# Update the version number to the desired version.
			$newAssemblyVersionLine = "[assembly: AssemblyVersion(""$NewVersionNumber"")]"
			$fileContents = $versionNumberLineRegex.Replace($fileContents, $newAssemblyVersionLine)
			[System.IO.File]::WriteAllText($DPSF_COMMON_ASSEMBLY_INFO_FILE_PATH, $fileContents)
			return $NewVersionNumber
		}
	}
	else
	{
		Throw "Could not find version number in the DPSF CommonAssemblyInfo.cs file"
	}
}


#==========================================================
# Perform the script tasks.
#==========================================================

# If a Version Number was supplied, update the DPSF Version Number before creating the DLLs.
if ($VersionNumber)
{
	$VersionNumber = DpsfVersionNumber -SetVersionNumber -NewVersionNumber $VersionNumber
}
# Else a Version Number was not supplied, so just get the current DPSF Version Number.
else
{
	$VersionNumber = DpsfVersionNumber -GetVersionNumber
}

Write-Host "Beginning script to create DPSF Release '$VersionNumber'..."

# Delete the existing DPSF files before building the new ones.
Write-Host "Deleting existing DLLs..."
Remove-Item -Recurse -Path "$LATEST_DLL_FILES_DIRECTORY_PATH" # Delete the entire folder.
New-Item -ItemType Directory -Path "$LATEST_DLL_FILES_DIRECTORY_PATH" > $null # Recreate the empty folder (and trash the output it creates).

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







<#

When releasing a new version of DPSF, be sure to follow these steps:

0a - If the DPSPDefaultEffect.fx file was modified, you will need to re-add the .bin files as resources so that the changes take effect. 
Make sure to do the build in release mode so the generated .bin files are nice and small, then go into DPSFResources.resx in the DPSF Project, 
remove the effect resources, and then re-add them from "DPSF/DPSF Effects/Raw Effect Code". Be sure to do a thorough test on both the PC and Xbox 
for all particle types to make sure everything is good.

0b - If any files were added, removed, renamed, or moved in the DPSF project, you must reflect these changes in the Mono for Android Copy of DPSF project as well.

1 - Go into the DPSF Project Properties and update the Assembly Information to use the new Assembly Version (Major.Minor.Build.Revision) 
(Major features, minor features, bug fixes, 0).

You will need to manually update the AssemblyVersion property in the Mono for Android Copy of DPSF project's Properties/AssemblyInfo.cs file.

2 - First make sure the configuration manager is set to Mixed Platforms, so that x86, Xbox 360, Windows Phone, and Mono for Android files are built.

3 - Do a "Build Solution" in RELEASE mode to generate an updated DPSF.dll/.xml, DPSFXbox360.dll/.xml, and DPSFPhone.dll/.xml files.

4 - In the "DPSF" Project Properties, change the Assembly Name to DPSFAsDrawableGameComponent and add DPSFAsDrawableGameComponent to the 
Conditional Compilation Symbols (in the Build tab). In the DPSF Xbox 360 Project Properties change the Assembly Name to DPSFXbox360AsDrawableGameComponent 
and add DPSFAsDrawableGameComponent to the Conditional Compilation Symbols. In the Windows Phone Copy of DPSF Project Properties change the Assembly Name to 
DPSFPhoneAsDrawableGameComponent and add DPSFAsDrawableGameComponent to the Conditional Compilation Symbols. In the Mono for Android Copy of DPSF Project 
Properties change the Assembly Name to DPSFMonoForAndoidAsDrawableGameComponent and add DPSFAsDrawableGameComponent to the Conditional Compilation Symbols. 

Then do a Build Solution to generate new DPSFAsDrawableGameComponent.dll/.xml, DPSFXbox360AsDrawableGameComponent.dll/.xml, 
DPSFPhoneAsDrawableGameComponent.dll/.xml, and DPSFMonoForAndoidAsDrawableGameComponent.dll/.xml files that inherit from DrawableGameComponent.

5 - Copy the DPSF.dll/.xml, DPSFAsDrawableGameComponent.dll/.xml, DPSFXbox360.dll/.xml, DPSFXbox360AsDrawableGameComponenet.dll/.xml, 
DPSFPhone.dll/.xml, DPSFPhoneAsDrawableGameComponent.dll/.xml, DPSFMonoForAndoid.dll/.xml, and DPSFMonoForAndoidAsDrawableGameComponent.dll/.xml 
files in the "DPSF\LatestDLLBuild" folder into the "Installer Files" folder, and copy them into the "C:\DPSF" folder as well so that the 
"Installer Files\DPSF Demo" project can find the new files.

6 - Remove the Conditional Compilation Symbol from all 4 Project Properties, and rename the Assembly Names back to DPSF, DPSFXbox360, DPSFPhone, 
and DPSFMonoForAndroid.

7 - Open the TestDPSFDLL solution and run it to ensure that DPSF.dll is working correctly. Look at the DPSF Reference properties to make sure it is 
using the correct version of the dll file. Test it both with "this" and "null" supplied in the particle system's constructor; both should run fine. Test it 
on the Xbox 360 as well if possible by right-clicking the Xbox 360 copy of the project and selecting "Set as StartUp Project.

8 - Open the TestDPSFInheritsDLL solution and run it to ensure that DPSFAsDrawableGameComponent.dll is working correctly. Look at the DPSF Reference 
properties to make sure it is using the correct version of the dll file. Test it both with "this" and "null" supplied in the particle system's constructor; 
using "null" should throw an exception when you try and run it. Test it on the Xbox 360 as well if possible by right-clicking the Xbox 360 copy of the project 
and selecting "Set as StartUp Project.

9 - Open the "Installer Files\Logos\DPSFSplashScreenExample" solution and make sure it still compiles and runs fine.

10 - Copy the files in "Installer Files\DPSF Demo\DPSF Demo\DPSF Demo\Templates" to the "Installer Files\Templates" folder.

11 - Copy the files in "DPSF\DPSF\DPSF Defaults" to the "Installer Files\Templates\DPSF Defaults" folder.

12 - Copy "DPSF\DPSF\DPSF Effects\HLSL\DPSFDefaultEffect.fx" to the "Installer Files\Templates" folder.

13 - Update the "DPSF API Documentation" to use the .dll's new .xml files generated (using Sandcastle Help File Builder program). You will need to update the 
HelpFileVersion to match the new DPSF version number.

14 - Update the Help document (including the change log), generating a new "DPSF Help.chm" and copy it into the "Installer Files" folder. Generating the 
Help document has it's own process document that should be followed (DPSF Help Update Process.txt).

15 - Do a search on the "Installer Files" folder and delete all "Debug" and "Release" folders, ".suo", and ".cachefile" files, and any files or folders 
with "Resharper" or "ncrunch" in their name.  This will help keep the size of the installer small, but will require users to build the applications 
(tutorials, etc.) in visual studio before running them. 

16 - Open the "DPSF\DPSF.sln" and change the DPSF Demo projects to reference the "C:\DPSF\DPSF.dll" files rather than the DPSF project. You will need to 
do this for the Windows, Xbox, and Windows Phone DPSF Demo projects. We need to do this so that when the user opens the DPSF Demo.sln the DPSF references 
will already be pointing to the "C:\DPSF" directory.

Then re-run the "DPSF\DPSF.sln" in x86 Release mode to generate the executable and required .xnb files so that the DPSF Demo can be ran without needing 
Visual Studio. The DPSF Demo (Phone) does not generate an executable that can be run by Windows, so we don't need to do this with it. Then change the 
configuration manager back to Mixed Debug mode when done.

17 - Open the "DPSF Installer Settings.iit" and Build a new "DPSF Installer.exe", making sure to include any new links that should appear in the 
Start Menu DPSF folder, such as links to new tutorials, demos, etc. and update the DPSF EULA if it was updated in the help documentation.

18 - Install DPSF from the new installer and make sure the DPSF Demo works properly. Then uninstall it and make sure everything is removed properly.

19 - Create a copy of the installer and move it into the "Archived Installers" section, renaming it with it's version number, and then zip it up to be 
uploaded to the web.

20 - In the DPSF.sln, change the DPSF Demo projects back to referencing the DPSF project, rather than the DLL files in the C:\DPSF folder, and change back 
to using the Debug Mixed configuration.

21 - Check files into Git, adding the current dll version (e.g. v1.0.1.1) and Change Log to the SVN commit comments.

22 - Upload the new version to the web, along with the new HTML help files, and update the RSS feed to show a new version. 
The web has it's own "Release Process.txt" file to follow.

#>


# Wait for input before closing.
Write-Host "Press any key to continue ..."
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyUp")