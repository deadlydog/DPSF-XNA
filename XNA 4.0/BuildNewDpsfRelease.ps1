<#
	.SYNOPSIS
	This script builds a new Release version for DPSF.
	
	.DESCRIPTION
	This script adjusts the DPSF version to the given version, then builds all of the DPSF DLLs in Release mode.
	
	.PARAMETER VersionNumber
	The new Version Number to give the DPSF DLLs.
	If null or empty, the user will be prompted for the assembly version number.
#>
param
(
	[parameter(Position=0,Mandatory=$false,HelpMessage="The 4 hex-value version number to build the DPSF assemblies with (x.x.x.x).")]
	[ValidatePattern("^\d{1,5}\.\d{1,5}\.\d{1,5}\.\d{1,5}$")]
	[String] $VersionNumber
)

# Turn on Strict Mode to help catch syntax-related errors.
# 	This must come after a script's/function's param section.
# 	Forces a function to be the first non-comment code to appear in a PowerShell Script/Module.
Set-StrictMode -Version Latest

# Get the direcotry that this script is in.
$thisScriptsDirectory = Split-Path $script:MyInvocation.MyCommand.Path

# Import the module used to build the .sln files.
$InvokeMsBuildModulePath = Join-Path $thisScriptsDirectory "BuildScriptUtilities\Invoke-MsBuild\Invoke-MsBuild.psm1"
Import-Module -Name $InvokeMsBuildModulePath


#==========================================================
# Define any necessary global variables, such as file paths.
#==========================================================
$DPSF_ROOT_DIRECTORY = $thisScriptsDirectory
$DPSF_COMMON_ASSEMBLY_INFO_FILE_PATH = Join-Path $DPSF_ROOT_DIRECTORY "\DPSF\DPSF\CommonAssemblyInfo.cs"
$DPSF_SOLUTION_FILE_PATH = Join-Path $DPSF_ROOT_DIRECTORY "\DPSF\DPSF.sln"
$DPSF_WINRT_SOLUTION_FILE_PATH = Join-Path $DPSF_ROOT_DIRECTORY "\DPSF\DPSF WinRT.sln"
$LATEST_DLL_FILES_DIRECTORY_PATH = Join-Path $DPSF_ROOT_DIRECTORY "\DPSF\LatestDLLBuild"
$DPSF_CSPROJ_FILE_PATHS = @( 
	(Join-Path $DPSF_ROOT_DIRECTORY "DPSF\DPSF\DPSF.csproj"),
	(Join-Path $DPSF_ROOT_DIRECTORY "DPSF\DPSF\Xbox 360 Copy of DPSF.csproj"), 
	(Join-Path $DPSF_ROOT_DIRECTORY "DPSF\DPSF\Windows Phone Copy of DPSF.csproj"),
	(Join-Path $DPSF_ROOT_DIRECTORY "DPSF\Mono for Android Copy of DPSF\Mono for Android Copy of DPSF.csproj"),
	(Join-Path $DPSF_ROOT_DIRECTORY "DPSF\DPSF WinRT\DPSF WinRT.csproj")
)
$MSBUILD_LOG_DIRECTORY_PATH = $DPSF_ROOT_DIRECTORY

$INSTALLER_FILES_DIRECTORY_PATH = Join-Path $DPSF_ROOT_DIRECTORY "\Installer\Installer Files"
$TEST_DPSF_DLL_SLN_PATH = Join-Path $DPSF_ROOT_DIRECTORY "Installer\Tests\TestDPSFDLL\TestDPSFDLL.sln"
$TEST_DPSF_INHERITS_DLL_SLN_PATH = Join-Path $DPSF_ROOT_DIRECTORY "Installer\Tests\TestDPSFInheritsDLL\TestDPSFInheritsDLL.sln"
$DPSF_SPLASH_SCREEN_EXAMPLE_SLN_PATH = Join-Path $INSTALLER_FILES_DIRECTORY_PATH "Logos\DPSFSplashScreenExample\DPSFSplashScreenExample.sln"

$DPSF_DEMO_TEMPLATES_DIRECTORY_PATH = Join-Path $INSTALLER_FILES_DIRECTORY_PATH "DPSF Demo\DPSF Demo\DPSF Demo\Templates"
$INSTALLER_FILES_TEMPLATES_DIRECTORY_PATH = Join-Path $INSTALLER_FILES_DIRECTORY_PATH "Templates"
$DPSF_DEFAULTS_DIRECTORY = Join-Path $DPSF_ROOT_DIRECTORY "DPSF\DPSF\DPSF Defaults"
$INSTALLER_FILES_TEMPLATES_DPSF_DEFAULTS_DIRECTORY = Join-Path $INSTALLER_FILES_TEMPLATES_DIRECTORY_PATH "DPSF Defaults"
$DPSF_DEFAULT_EFFECT_FILE_PATH = Join-Path $DPSF_ROOT_DIRECTORY "DPSF\DPSF\DPSF Effects\HLSL\DPSFDefaultEffect.fx"

$MSBUILD_PARAMETERS = "/target:Clean;Build /property:Configuration=Release;Platform=""Mixed Platforms"" /verbosity:Quiet"
$WINRT_MSBUILD_PARAMETERS = "/target:Clean;Build /property:Configuration=Release;Platform=""Any CPU"" /verbosity:Quiet"


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
	param 
	(
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
	elseif ((Split-Path -Path $CsprojFilePath -Leaf) -eq 'Mono for Android Copy of DPSF.csproj')
	{
		$fileContents = $fileContents.Replace('<AssemblyName>DPSFMonoForAndoid</AssemblyName>', '<AssemblyName>DPSFMonoForAndoidAsDrawableGameComponent</AssemblyName>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\Debug\DPSFMonoForAndoid.xml</DocumentationFile>', '<DocumentationFile>bin\Debug\DPSFMonoForAndoidAsDrawableGameComponent.xml</DocumentationFile>')
		$fileContents = $fileContents.Replace('<DefineConstants>TRACE;ANDROID</DefineConstants>', '<DefineConstants>TRACE;ANDROID DPSFAsDrawableGameComponent</DefineConstants>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\Release\DPSFMonoForAndoid.xml</DocumentationFile>', '<DocumentationFile>bin\Release\DPSFMonoForAndoidAsDrawableGameComponent.xml</DocumentationFile>')
	}
	elseif ((Split-Path -Path $CsprojFilePath -Leaf) -eq 'DPSF WinRT.csproj')
	{
		$fileContents = $fileContents.Replace('<AssemblyName>DPSFWinRT</AssemblyName>', '<AssemblyName>DPSFWinRTAsDrawableGameComponent</AssemblyName>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\Debug\DPSFWinRT.xml</DocumentationFile>', '<DocumentationFile>bin\Debug\DPSFWinRTAsDrawableGameComponent.xml</DocumentationFile>')
		$fileContents = $fileContents.Replace('<DefineConstants>TRACE;NETFX_CORE WIN_RT</DefineConstants>', '<DefineConstants>TRACE;NETFX_CORE WIN_RT DPSFAsDrawableGameComponent</DefineConstants>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\Release\DPSFWinRT.xml</DocumentationFile>', '<DocumentationFile>bin\Release\DPSFWinRTAsDrawableGameComponent.xml</DocumentationFile>')
	}
	else
	{
		# Throw an error.
		throw "ERROR: Invalid .csproj file name given. Cannot find the file '$CsprojFilePath'."
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
		throw "Cannot find the DPSF Common Assembly Info file at '$DPSF_COMMON_ASSEMBLY_INFO_FILE_PATH'."
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
		throw "Could not find version number in the DPSF CommonAssemblyInfo.cs file"
	}
}

# Catch any exceptions thrown and stop the script.
trap [Exception] { Write-Host "Error: $_"; break; }

#==========================================================
# Perform the script tasks.
#==========================================================

<#
1 - Go into the DPSF Project Properties and update the Assembly Information to use the new Assembly Version (Major.Minor.Build.Revision) 
(Major features, minor features, bug fixes, 0).
#>

# If a Version Number was not supplied, get the current version number and prompt for a new one.
if (!$VersionNumber)
{
	$currentVersionNumber = DpsfVersionNumber -GetVersionNumber

	# Prompt for the Version Number to use.
	Add-Type -AssemblyName Microsoft.VisualBasic
	$VersionNumber = [Microsoft.VisualBasic.Interaction]::InputBox("Enter the 4 hex-value version number to build the DPSF assemblies with (x.x.x.x):", "DPSF Version Number To Use", $currentVersionNumber)
}

# Set the Version Number
$VersionNumber = DpsfVersionNumber -SetVersionNumber -NewVersionNumber $VersionNumber

Write-Host "Beginning script to create DPSF Release '$VersionNumber'..."

<#
2 - First make sure the configuration manager is set to Mixed Platforms, so that x86, Xbox 360, Windows Phone, and Mono for Android files are built.

3 - Do a "Build Solution" in RELEASE mode to generate an updated DPSF.dll/.xml, DPSFXbox360.dll/.xml, and DPSFPhone.dll/.xml files.

4 - In the "DPSF" Project Properties, change the Assembly Name to DPSFAsDrawableGameComponent and add DPSFAsDrawableGameComponent to the 
Conditional Compilation Symbols (in the Build tab). In the DPSF Xbox 360 Project Properties change the Assembly Name to DPSFXbox360AsDrawableGameComponent 
and add DPSFAsDrawableGameComponent to the Conditional Compilation Symbols. In the Windows Phone Copy of DPSF Project Properties change the Assembly Name to 
DPSFPhoneAsDrawableGameComponent and add DPSFAsDrawableGameComponent to the Conditional Compilation Symbols. In the Mono for Android Copy of DPSF Project 
Properties change the Assembly Name to DPSFMonoForAndoidAsDrawableGameComponent and add DPSFAsDrawableGameComponent to the Conditional Compilation Symbols. 

Then do a Build Solution to generate new DPSFAsDrawableGameComponent.dll/.xml, DPSFXbox360AsDrawableGameComponent.dll/.xml, 
DPSFPhoneAsDrawableGameComponent.dll/.xml, and DPSFMonoForAndoidAsDrawableGameComponent.dll/.xml files that inherit from DrawableGameComponent.

6 - Remove the Conditional Compilation Symbol from all 4 Project Properties, and rename the Assembly Names back to DPSF, DPSFXbox360, DPSFPhone, 
and DPSFMonoForAndroid.
#>

# Delete the existing DPSF files before building the new ones.
Write-Host "Deleting existing DLLs..."
Remove-Item -Recurse -Path "$LATEST_DLL_FILES_DIRECTORY_PATH" # Delete the entire folder.
New-Item -ItemType Directory -Path "$LATEST_DLL_FILES_DIRECTORY_PATH" > $null # Recreate the empty folder (and trash the output it creates).

# Build the DPSF solution in Release mode to create the new DLLs.
Write-Host "Building the DPSF solution..."
$buildSucceeded = Invoke-MsBuild -Path "$DPSF_SOLUTION_FILE_PATH" -MsBuildParameters "$MSBUILD_PARAMETERS" -BuildLogDirectoryPath "$MSBUILD_LOG_DIRECTORY_PATH" -ShowBuildWindow -AutoLaunchBuildLogOnFailure
if (!$buildSucceeded) { throw "Build failed." }
Write-Host "Building the DPSF WinRT solution..."
$buildSucceeded = Invoke-MsBuild -Path "$DPSF_WINRT_SOLUTION_FILE_PATH" -MsBuildParameters "$WINRT_MSBUILD_PARAMETERS" -BuildLogDirectoryPath "$MSBUILD_LOG_DIRECTORY_PATH" -ShowBuildWindow -AutoLaunchBuildLogOnFailure
if (!$buildSucceeded) { throw "Build failed." }

# Update the .csproj files' to build the AsDrawableGameComponent DLLs.
Write-Host "Updating the .csproj files to build AsDrawableGameComponent DLLs..."
foreach ($csprojFilePath in $DPSF_CSPROJ_FILE_PATHS)
{
	# Backup the file before modifying it.
	Copy-Item -Path $csprojFilePath -Destination "$CsprojFilePath.backup"

	UpdateCsprojFileToAsDrawableGameComponent $csprojFilePath
}

# Rebuild the solutions to create the AsDrawableGameComponent DLLs.
Write-Host "Building the DPSF solution for AsDrawableGameComponent DLLs..."
$buildSucceeded = Invoke-MsBuild -Path "$DPSF_SOLUTION_FILE_PATH" -MsBuildParameters "$MSBUILD_PARAMETERS" -BuildLogDirectoryPath "$MSBUILD_LOG_DIRECTORY_PATH" -ShowBuildWindow -AutoLaunchBuildLogOnFailure
if (!$buildSucceeded) { throw "Build failed." }
Write-Host "Building the DPSF WinRT for AsDrawableGameComponent DLLs..."
$buildSucceeded = Invoke-MsBuild -Path "$DPSF_WINRT_SOLUTION_FILE_PATH" -MsBuildParameters "$WINRT_MSBUILD_PARAMETERS" -BuildLogDirectoryPath "$MSBUILD_LOG_DIRECTORY_PATH" -ShowBuildWindow -AutoLaunchBuildLogOnFailure
if (!$buildSucceeded) { throw "Build failed." }

#Revert the .csproj files back to their original states now that we have the DLLs.
Write-Host "Reverting .csproj files back to their original states..."
foreach ($csprojFilePath in $DPSF_CSPROJ_FILE_PATHS)
{
	# Copy the backup back overtop of the original to revert it, and then delete the backup file.
	if (Test-Path "$csprojFilePath.backup")
	{
		Copy-Item -Path "$csprojFilePath.backup" -Destination "$csprojFilePath"
		Remove-Item -Path "$csprojFilePath.backup"
	}
}

<#
5 - Copy the DPSF.dll/.xml, DPSFAsDrawableGameComponent.dll/.xml, DPSFXbox360.dll/.xml, DPSFXbox360AsDrawableGameComponenet.dll/.xml, 
DPSFPhone.dll/.xml, DPSFPhoneAsDrawableGameComponent.dll/.xml, DPSFMonoForAndoid.dll/.xml, and DPSFMonoForAndoidAsDrawableGameComponent.dll/.xml 
files in the "DPSF\LatestDLLBuild" folder into the "Installer Files" folder, and copy them into the "C:\DPSF" folder as well so that the 
"Installer Files\DPSF Demo" project can find the new files.
#>

# Copy the DLL files to the 'Installer Files' directory.
Write-Host "Copying new DLL and XML files to the Installer Files directory..."
Copy-Item -Path "$LATEST_DLL_FILES_DIRECTORY_PATH/*" -Destination $INSTALLER_FILES_DIRECTORY_PATH -Include "*.dll","*.xml"

# Copy the DLL files to the 'C:\DPSF' directory.
Write-Host "Copying new DLL and XML files to the 'C:\DPSF' directory..."
Copy-Item -Path "$LATEST_DLL_FILES_DIRECTORY_PATH/*" -Destination "C:\DPSF" -Include "*.dll","*.xml"

# Make sure all of the DPSF DLLs were built and copied properly.
Write-Host "Verifying that the DPSF DLL files were built and copied properly..."
Get-ChildItem -Path "$LATEST_DLL_FILES_DIRECTORY_PATH/*" -Include "DPSF*.dll" | foreach {
	# Make sure all of the DLLs are on the right version.
	$fileVersion = $_.VersionInfo.FileVersion
	if ($fileVersion -ne $VersionNumber)
	{
		throw "File '$_' is version '$fileVersion' instead of the expected '$VersionNumber'."
	}
	
	# Make sure all of the DLLs were just updated.
	$lastModifiedDateTime = [System.DateTime]::Parse($_.LastWriteTime)
	$now = [System.DateTime]::Now
	$timePassedSinceFileWasUpdated = ($now - $lastModifiedDateTime)
	if ($timePassedSinceFileWasUpdated -gt [System.TimeSpan]::FromMinutes(5))
	{
		throw "File '$_' does not seem to have been updated. It was last updated at '$lastModifiedDateTime', which was '$($timePassedSinceFileWasUpdated.TotalMinutes)' minutes ago."
	}
}

<#
7 - Open the TestDPSFDLL solution and run it to ensure that DPSF.dll is working correctly. Look at the DPSF Reference properties to make sure it is 
using the correct version of the dll file. Test it both with "this" and "null" supplied in the particle system's constructor; both should run fine. Test it 
on the Xbox 360 as well if possible by right-clicking the Xbox 360 copy of the project and selecting "Set as StartUp Project.

8 - Open the TestDPSFInheritsDLL solution and run it to ensure that DPSFAsDrawableGameComponent.dll is working correctly. Look at the DPSF Reference 
properties to make sure it is using the correct version of the dll file. Test it both with "this" and "null" supplied in the particle system's constructor; 
using "null" should throw an exception when you try and run it. Test it on the Xbox 360 as well if possible by right-clicking the Xbox 360 copy of the project 
and selecting "Set as StartUp Project.

9 - Open the "Installer Files\Logos\DPSFSplashScreenExample" solution and make sure it still compiles and runs fine.
#>

# Prompt if user wants to launch the test solutions and verify they work correctly and save answer for later.
Add-Type -AssemblyName System.Windows.Forms
if ([System.Windows.Forms.MessageBox]::Show("Do you want to launch the Test solutions to verify they work correctly?", "Launch Test Solutions", [System.Windows.Forms.MessageBoxButtons]::YesNo, [System.Windows.Forms.MessageBoxIcon]::Question) -eq [System.Windows.Forms.DialogResult]::Yes)
{
	# Open the TestDPSFDLL solution to verify that it works correctly.
	& $TEST_DPSF_DLL_SLN_PATH

	# Open the TestDPSFInheritsDLL solution to verify that it works correctly.
	& $TEST_DPSF_INHERITS_DLL_SLN_PATH

	# Open the DPSFSplashScreenExample solution to verify that it works correctly.
	& $DPSF_SPLASH_SCREEN_EXAMPLE_SLN_PATH
	
	# Ask user if the Test solutions all work correctly or not, and exit if they don't work correctly.
	$confirmTestsWorkProperlyMessage = @"
Do all of the Test solutions work correctly?

1. Does the Test DPSF Dll solution run properly with both 'this' and 'null' supplied in the particle system's constructor?

2. Does the Test DPSF Inherits Dll solution run properly with both 'this' and 'null' supplied in the particle system's constructor? Using 'null' should throw an exception.

3. Does the DPSF Splash Screen Example solution run properly?
"@
	if ([System.Windows.Forms.MessageBox]::Show($confirmTestsWorkProperlyMessage, "Do Test Solutions Run Correctly?", [System.Windows.Forms.MessageBoxButtons]::YesNo, [System.Windows.Forms.MessageBoxIcon]::Question) -eq [System.Windows.Forms.DialogResult]::No)
	{
		Exit
	}
}

<#
10 - Copy the files in "Installer Files\DPSF Demo\DPSF Demo\DPSF Demo\Templates" to the "Installer Files\Templates" folder.

11 - Copy the files in "DPSF\DPSF\DPSF Defaults" to the "Installer Files\Templates\DPSF Defaults" folder.

12 - Copy "DPSF\DPSF\DPSF Effects\HLSL\DPSFDefaultEffect.fx" to the "Installer Files\Templates" folder.
#>

Write-Host "Deleting the folder '$INSTALLER_FILES_TEMPLATES_DIRECTORY_PATH'..."
Remove-Item -Recurse -Path "$INSTALLER_FILES_TEMPLATES_DIRECTORY_PATH" # Delete the entire folder.
New-Item -ItemType Directory -Path "$INSTALLER_FILES_TEMPLATES_DIRECTORY_PATH" > $null # Recreate the empty folder (and trash the output it creates).

Write-Host "Copying the files in '$DPSF_DEMO_TEMPLATES_DIRECTORY_PATH' to '$INSTALLER_FILES_TEMPLATES_DIRECTORY_PATH'..."
RoboCopy "$DPSF_DEMO_TEMPLATES_DIRECTORY_PATH" "$INSTALLER_FILES_TEMPLATES_DIRECTORY_PATH" /s

Write-Host "Copying the files in '$DPSF_DEFAULTS_DIRECTORY' to '$INSTALLER_FILES_TEMPLATES_DPSF_DEFAULTS_DIRECTORY'..."
RoboCopy "$DPSF_DEFAULTS_DIRECTORY" "$INSTALLER_FILES_TEMPLATES_DPSF_DEFAULTS_DIRECTORY" /s

Write-Host "Copying the file '$DPSF_DEFAULT_EFFECT_FILE_PATH' to the folder '$INSTALLER_FILES_TEMPLATES_DIRECTORY_PATH'..."
Copy-Item -Path "$DPSF_DEFAULT_EFFECT_FILE_PATH" -Destination "$INSTALLER_FILES_TEMPLATES_DIRECTORY_PATH"

<#
13 - Do a search on the "Installer Files" folder and delete all "Debug" and "Release" folders, ".suo", and ".cachefile" files, and any files or folders 
with "Resharper" or "ncrunch" in their name.  This will help keep the size of the installer small, but will require users to build the applications 
(tutorials, etc.) in visual studio before running them. 
#>

Write-Host "Delete the 'Debug' and 'Release' folders, and any other temp files in the '$INSTALLER_FILES_DIRECTORY_PATH' directory..."
Get-ChildItem -Recurse -Force -Path "$INSTALLER_FILES_DIRECTORY_PATH" -Include "bin","obj","*.suo","*.cachefile","*ncrunch*","*ReSharper*" | Remove-Item -Recurse -Force

<#
14 - Open the "DPSF\DPSF.sln" and change the DPSF Demo projects to reference the "C:\DPSF\DPSF.dll" files rather than the DPSF project. 
You will need to do this for the Windows, Xbox, Windows Phone, and Mono for Android DPSF Demo projects, as well as the "DPSF\DPSF WinRT.sln" DPSF Demo project. 
We need to do this so that when the user opens the DPSF Demo.sln the DPSF references will already be pointing to the "C:\DPSF" directory.
#>



<#
15 - Re-run the "DPSF\DPSF.sln" in x86 Release mode to generate the executable and required .xnb files so that the DPSF Demo can be ran without 
needing Visual Studio. The DPSF Demo (Phone) does not generate an executable that can be run by Windows, so we don't need to do this with it. 
Then change the configuration manager back to Mixed Debug mode when done.
#>








<# Put these comments directly into the script so they explain what the script does manually.

When releasing a new version of DPSF, be sure to follow these steps:

0a - If the DPSPDefaultEffect.fx file was modified, you will need to re-add the .bin files as resources so that the changes take effect. Make sure to do the build in release mode so the generated .bin files are nice and small, then go into DPSFResources.resx in the DPSF Project, remove the effect resources, and then re-add them from "DPSF/DPSF Effects/Raw Effect Code". Be sure to do a thorough test on both the PC and Xbox for all particle types to make sure everything is good.

0b - If any files were added, removed, renamed, or moved in the DPSF project, you must reflect these changes in the Mono for Android Copy of DPSF project as well.

1 - Update the CommonAssemblyInfo.cs file to use the new Assembly Version (Major.Minor.Build.Revision) (Major features, minor features, bug fixes, 0).

2 - First make sure the configuration manager is set to Mixed Platforms, so that x86, Xbox 360, Windows Phone, and Mono for Android files are built. It should be Any CPU in the "DPSF WinRT.sln".

3 - Do a "Build Solution" in RELEASE mode to generate updated DPSF.dll/.xml, DPSFXbox360.dll/.xml, DPSFPhone.dll/.xml, and DPSFMonoForAndroid.dll/xml files. Also build the DPSF WinRT.sln in RELEASE mode to generate updated DPSFWinRT.dll/.xml files.

4 - In each of the DPSF Project Properties do the following:
a. Append "AsDrawableGameComponent" to the Assembly Name. I.e. DPSF => DPSFAsDrawableGameComponent, DPSFXbox360 => DPSFXbox360AsDrawableGameComponent, DPSFPhone => DPSFPhoneAsDrawableGameComponent, DPSFMonoForAndroid => DPSFMonoForAndoidAsDrawableGameComponent, DPSFWinRT => DPSFWinRTAsDrawableGameComponent.
b. Add "DPSFAsDrawableGameComponent" to the Conditional Compilation Symbols (in the Build tab).

Then do a Build Solution on both the DPSF.sln and the "DPSF WinRT.sln" to generate new DPSFAsDrawableGameComponent.dll/.xml, DPSFXbox360AsDrawableGameComponent.dll/.xml, DPSFPhoneAsDrawableGameComponent.dll/.xml, DPSFMonoForAndoidAsDrawableGameComponent.dll/.xml, and DPSFWinRTAsDrawableGameComponent.dll/.xml files that inherit from DrawableGameComponent.

5 - Copy the DPSF.dll/.xml, DPSFAsDrawableGameComponent.dll/.xml, DPSFXbox360.dll/.xml, DPSFXbox360AsDrawableGameComponenet.dll/.xml, DPSFPhone.dll/.xml, DPSFPhoneAsDrawableGameComponent.dll/.xml, DPSFMonoForAndoid.dll/.xml, DPSFMonoForAndoidAsDrawableGameComponent.dll/.xml, DPSFWinRT.dll/.xml, and DPSFWinRTAsDrawableGameComponent.dll/.xml files in the "DPSF\LatestDLLBuild" folder into the "Installer Files" folder, and copy them into the "C:\DPSF" folder as well so that the "Installer Files\DPSF Demo" project can find the new files.

6 - Remove the Conditional Compilation Symbol from all of the Project Properties, and rename the Assembly Names back to DPSF, DPSFXbox360, DPSFPhone, DPSFMonoForAndroid, and DPSFWinRT.

7 - Open the TestDPSFDLL solution and run it to ensure that DPSF.dll is working correctly. Look at the DPSF Reference properties to make sure it is using the correct version of the dll file. Test it both with "this" and "null" supplied in the particle system's constructor; both should run fine. Test it on the Xbox 360 as well if possible by right-clicking the Xbox 360 copy of the project and selecting "Set as StartUp Project.

8 - Open the TestDPSFInheritsDLL solution and run it to ensure that DPSFAsDrawableGameComponent.dll is working correctly. Look at the DPSF Reference properties to make sure it is using the correct version of the dll file. Test it both with "this" and "null" supplied in the particle system's constructor; using "null" should throw an exception when you try and run it. Test it on the Xbox 360 as well if possible by right-clicking the Xbox 360 copy of the project and selecting "Set as StartUp Project.

9 - Open the "Installer Files\Logos\DPSFSplashScreenExample" solution and make sure it still compiles and runs fine.

10 - Copy the files in "Installer Files\DPSF Demo\DPSF Demo\DPSF Demo\Templates" to the "Installer Files\Templates" folder.

11 - Copy the files in "DPSF\DPSF\DPSF Defaults" to the "Installer Files\Templates\DPSF Defaults" folder.

12 - Copy "DPSF\DPSF\DPSF Effects\HLSL\DPSFDefaultEffect.fx" to the "Installer Files\Templates" folder.

13 - Do a search on the "Installer Files" folder and delete all "Debug" and "Release" folders, ".suo", and ".cachefile" files, and any files or folders with "Resharper" or "ncrunch" in their name.  This will help keep the size of the installer small, but will require users to build the applications (tutorials, etc.) in visual studio before running them. 

14 - Open the "DPSF\DPSF.sln" and change the DPSF Demo projects to reference the "C:\DPSF\DPSF.dll" files rather than the DPSF project. You will need to do this for the Windows, Xbox, Windows Phone, and Mono for Android DPSF Demo projects, as well as the "DPSF\DPSF WinRT.sln" DPSF Demo project. We need to do this so that when the user opens the DPSF Demo.sln the DPSF references will already be pointing to the "C:\DPSF" directory.

15 - Re-run the "DPSF\DPSF.sln" in x86 Release mode to generate the executable and required .xnb files so that the DPSF Demo can be ran without needing Visual Studio. The DPSF Demo (Phone) does not generate an executable that can be run by Windows, so we don't need to do this with it. Then change the configuration manager back to Mixed Debug mode when done.

16 - Update the "DPSF API Documentation" to use the .dll's new .xml files generated (using Sandcastle Help File Builder program). You will need to update the HelpFileVersion to match the new DPSF version number.

17 - Update the Help document (including the change log), generating a new "DPSF Help.chm" and copy it into the "Installer Files" folder. Generating the Help document has it's own process document that should be followed (DPSF Help Update Process.txt).

18 - Open the "DPSF Installer Settings.iit" and Build a new "DPSF Installer.exe", making sure to include any new links that should appear in the Start Menu DPSF folder, such as links to new tutorials, demos, etc. and update the DPSF EULA if it was updated in the help documentation.

19 - Install DPSF from the new installer and make sure the DPSF Demo works properly. Then uninstall it and make sure everything is removed properly.

20 - Create a copy of the installer and move it into the "Archived Installers" section, renaming it with it's version number, and then zip it up to be uploaded to the web.

21 - In the DPSF.sln and "DPSF WinRT.sln", change the DPSF Demo projects back to referencing the DPSF projects, rather than the DLL files in the C:\DPSF folder, and change back to using the Debug Mixed configuration.

22 - Check files into Git, adding the current dll version (e.g. v1.0.1.1) and Change Log to the SVN commit comments.

23 - Upload the new version to the web, along with the new HTML help files, and update the RSS feed to show a new version. The web has it's own "Release Process.txt" file to follow.

#>


# Wait for input before closing.
Write-Host "Press any key to continue ..."
$x = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyUp")
