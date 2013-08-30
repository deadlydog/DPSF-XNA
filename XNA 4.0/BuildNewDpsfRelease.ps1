<#
	.SYNOPSIS
	This script builds a new Release version for DPSF.
	
	.DESCRIPTION
	This script performs the complete process required for making a new DPSF Release, prompting the user when it requires they do work in 
	other applications (e.g. create help documentation, verify test solutions run properly, etc.)
	
	Depends on having the Powershell Community Extensions installed for zipping files (http://pscx.codeplex.com/).
	Depends on having Git Extensions installed for checking files into Git (http://code.google.com/p/gitextensions/).
	
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
$THIS_SCRIPTS_DIRECTORY = Split-Path $script:MyInvocation.MyCommand.Path

# Import the module used to build the .sln files.
$InvokeMsBuildModulePath = Join-Path $THIS_SCRIPTS_DIRECTORY "BuildScriptUtilities\Invoke-MsBuild\Invoke-MsBuild.psm1"
Import-Module -Name $InvokeMsBuildModulePath

# Import the System.Windows.Forms namespace which is used to show Message Boxes and the Open File Dialog.
Add-Type -AssemblyName System.Windows.Forms


#==========================================================
# Define any necessary global variables, such as file paths.
#==========================================================
$GIT_EXTENSIONS_COMMAND_LINE_TOOL_PATH = "C:\Program Files (x86)\GitExtensions\gitex.cmd"
$DPSF_DEFAULT_INSTALL_DIRECTORY = "C:\DPSF"

$DPSF_ROOT_DIRECTORY = $THIS_SCRIPTS_DIRECTORY
$DPSF_REPOSITORY_ROOT_DIRECTORY = Resolve-Path -Path "$DPSF_ROOT_DIRECTORY\.."
$DPSF_COMMON_ASSEMBLY_INFO_FILE_PATH = Join-Path $DPSF_ROOT_DIRECTORY "\DPSF\DPSF\CommonAssemblyInfo.cs"
$DPSF_SOLUTION_FILE_PATH = Join-Path $DPSF_ROOT_DIRECTORY "\DPSF\DPSF.sln"
$DPSF_WINRT_SOLUTION_FILE_PATH = Join-Path $DPSF_ROOT_DIRECTORY "\DPSF\DPSF WinRT.sln"
$LATEST_DLL_FILES_DIRECTORY_PATH = Join-Path $DPSF_ROOT_DIRECTORY "\DPSF\LatestDLLBuild"
$DPSF_CSPROJ_FILE_PATHS = @( 
	(Join-Path $DPSF_ROOT_DIRECTORY "DPSF\DPSF\DPSF.csproj"),
	(Join-Path $DPSF_ROOT_DIRECTORY "DPSF\DPSF\Xbox 360 Copy of DPSF.csproj"), 
	(Join-Path $DPSF_ROOT_DIRECTORY "DPSF\DPSF\Windows Phone Copy of DPSF.csproj"),
	(Join-Path $DPSF_ROOT_DIRECTORY "DPSF\DPSF WinRT\DPSF WinRT.csproj"),
    (Join-Path $DPSF_ROOT_DIRECTORY "DPSF\Mono for Android Copy of DPSF\Mono for Android Copy of DPSF.csproj"),
    (Join-Path $DPSF_ROOT_DIRECTORY "DPSF\Xamarin.iOS Copy of DPSF\Xamarin.iOS Copy of DPSF.csproj")
)
$MSBUILD_LOG_DIRECTORY_PATH = $DPSF_ROOT_DIRECTORY
$MSBUILD_PARAMETERS = "/target:Clean;Build /property:Configuration=Release;Platform=""Mixed Platforms"" /verbosity:Quiet"
$WINRT_MSBUILD_PARAMETERS = "/target:Clean;Build /property:Configuration=Release;Platform=""Any CPU"" /verbosity:Quiet"
$MSBUILD_PARAMETERS_X86 = "/target:Clean;Build /property:Configuration=Release;Platform=""x86"" /verbosity:Quiet"

$INSTALLER_FILES_DIRECTORY_PATH = Join-Path $DPSF_ROOT_DIRECTORY "\Installer\Installer Files"
$TEST_DPSF_DLL_SLN_PATH = Join-Path $DPSF_ROOT_DIRECTORY "Installer\Tests\TestDPSFDLL\TestDPSFDLL.sln"
$TEST_DPSF_INHERITS_DLL_SLN_PATH = Join-Path $DPSF_ROOT_DIRECTORY "Installer\Tests\TestDPSFInheritsDLL\TestDPSFInheritsDLL.sln"
$DPSF_SPLASH_SCREEN_EXAMPLE_SLN_PATH = Join-Path $INSTALLER_FILES_DIRECTORY_PATH "Logos\DPSFSplashScreenExample\DPSFSplashScreenExample.sln"

$DPSF_DEMO_TEMPLATES_DIRECTORY_PATH = Join-Path $INSTALLER_FILES_DIRECTORY_PATH "DPSF Demo\DPSF Demo\DPSF Demo\Templates"
$INSTALLER_FILES_TEMPLATES_DIRECTORY_PATH = Join-Path $INSTALLER_FILES_DIRECTORY_PATH "Templates"
$DPSF_DEFAULTS_DIRECTORY = Join-Path $DPSF_ROOT_DIRECTORY "DPSF\DPSF\DPSF Defaults"
$INSTALLER_FILES_TEMPLATES_DPSF_DEFAULTS_DIRECTORY = Join-Path $INSTALLER_FILES_TEMPLATES_DIRECTORY_PATH "DPSF Defaults"
$DPSF_DEFAULT_EFFECT_FILE_PATH = Join-Path $DPSF_ROOT_DIRECTORY "DPSF\DPSF\DPSF Effects\HLSL\DPSFDefaultEffect.fx"

$DPSF_DEMO_CSPROJ_FILE_PATHS = @( 
	(Join-Path $INSTALLER_FILES_DIRECTORY_PATH "DPSF Demo\DPSF Demo\DPSF Demo\DPSF Demo.csproj"),
	(Join-Path $INSTALLER_FILES_DIRECTORY_PATH "DPSF Demo\DPSF Demo\DPSF Demo\Xbox 360 Copy of DPSF Demo.csproj"), 
	(Join-Path $INSTALLER_FILES_DIRECTORY_PATH "DPSF Demo\DPSF Demo for Windows Phone\DPSF Demo for Windows Phone\DPSF Demo for Windows Phone.csproj"),
	(Join-Path $INSTALLER_FILES_DIRECTORY_PATH "DPSF Demo\DPSF Demo for WinRT\DPSF Demo for WinRT.csproj"),
    (Join-Path $INSTALLER_FILES_DIRECTORY_PATH "DPSF Demo\DPSF Demo for Mono for Android\DPSF Demo for Mono for Android.csproj")
)
$CSPROJ_FILE_PATHS_BACKUP_DIRECTORY = Join-Path $DPSF_ROOT_DIRECTORY "CsprojBackups"

$HELP_FILES_DIRECTORY_PATH = Join-Path $DPSF_ROOT_DIRECTORY "Help Files"
$API_DOCUMENTATION_SANDCASTLE_PROJECT_FILE_PATH = Join-Path $HELP_FILES_DIRECTORY_PATH "DPSF API Documentation\DPSF API Documentation Sandcastle Project.shfbproj"
$HELP_DOCUMENTATION_HELP_AND_MANUAL_PROJECT_FILE_PATH = Join-Path $HELP_FILES_DIRECTORY_PATH "DPSF Help Without API Documentation.hmxz"
$HELP_DOCUMENTATION_UPDATE_PROCESS_FILE_PATH = Join-Path $HELP_FILES_DIRECTORY_PATH "DPSF Help Update Process.txt"
$HELP_DOCUMENTATION_HTML_DIRECTORY = Join-Path $HELP_FILES_DIRECTORY_PATH "HTML"
$HELP_DOCUMENTATION_CHM_FILE_NAME = "DPSF Help.chm"

$INSTALLER_CREATOR_PROJECT_FILE_PATH = Join-Path $DPSF_ROOT_DIRECTORY "Installer\DPSF Installer Settings.iit"
$DPSF_INSTALLER_FILE_PATH = Join-Path $DPSF_ROOT_DIRECTORY "Installer\DPSF Installer.exe"
$DPSF_UNINSTALLER_FILE_PATH = Join-Path $DPSF_DEFAULT_INSTALL_DIRECTORY "Uninstal.exe"
$ARCHIVED_INSTALLERS_DIRECTORY = Join-Path $DPSF_ROOT_DIRECTORY "Installer\Archived Installers"
$DPSF_CHANGE_LOG_FILE_PATH = Join-Path $DPSF_ROOT_DIRECTORY "DPSF\DPSF\ChangeLog.txt"

$DPSF_DEV_WEBSITE_DIRECTORY_PATH = Resolve-Path -Path (Join-Path "$DPSF_REPOSITORY_ROOT_DIRECTORY\.." "Websites\DPSF Website\Dev")
$DPSF_DEV_WEBSITE_DPSF_INSTALLER_ZIP_FILE_PATH = Join-Path $DPSF_DEV_WEBSITE_DIRECTORY_PATH "DPSF Installer.zip"
$DPSF_DEV_WEBSITE_ARCHIVED_INSTALLERS_DIRECTORY = Join-Path $DPSF_DEV_WEBSITE_DIRECTORY_PATH "ArchivedDPSFVersions"
$DPSF_DEV_WEBSITE_HELP_FILES_DIRECTORY = Join-Path $DPSF_DEV_WEBSITE_DIRECTORY_PATH "DPSFHelp"
$DPSF_DEV_WEBSITE_RELEASE_PROCESS_FILE_PATH = Join-Path $DPSF_DEV_WEBSITE_DIRECTORY_PATH "Release Process.txt"

$NUGET_DIRECTORY_PATH = Join-Path $DPSF_ROOT_DIRECTORY "\Installer\NuGet"
$CREATE_AND_PUSH_NEW_NUGET_PACKAGE_SCRIPT_PATH = Join-Path $NUGET_DIRECTORY_PATH "CreateNewDPSFNuGetPackage.ps1"


#==========================================================
# Define functions used by the script.
#==========================================================
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
	
	$versionNumberLineRegex = [regex] '\[assembly: AssemblyVersion\("(?<VersionNumber>.*?)"\)\]'
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
	elseif ((Split-Path -Path $CsprojFilePath -Leaf) -eq 'DPSF WinRT.csproj')
	{
		$fileContents = $fileContents.Replace('<AssemblyName>DPSFWinRT</AssemblyName>', '<AssemblyName>DPSFWinRTAsDrawableGameComponent</AssemblyName>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\Debug\DPSFWinRT.xml</DocumentationFile>', '<DocumentationFile>bin\Debug\DPSFWinRTAsDrawableGameComponent.xml</DocumentationFile>')
		$fileContents = $fileContents.Replace('<DefineConstants>TRACE;NETFX_CORE WIN_RT</DefineConstants>', '<DefineConstants>TRACE;NETFX_CORE WIN_RT DPSFAsDrawableGameComponent</DefineConstants>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\Release\DPSFWinRT.xml</DocumentationFile>', '<DocumentationFile>bin\Release\DPSFWinRTAsDrawableGameComponent.xml</DocumentationFile>')
	}
    elseif ((Split-Path -Path $CsprojFilePath -Leaf) -eq 'Mono for Android Copy of DPSF.csproj')
	{
		$fileContents = $fileContents.Replace('<AssemblyName>DPSFMonoForAndoid</AssemblyName>', '<AssemblyName>DPSFMonoForAndoidAsDrawableGameComponent</AssemblyName>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\Debug\DPSFMonoForAndoid.xml</DocumentationFile>', '<DocumentationFile>bin\Debug\DPSFMonoForAndoidAsDrawableGameComponent.xml</DocumentationFile>')
		$fileContents = $fileContents.Replace('<DefineConstants>TRACE;ANDROID</DefineConstants>', '<DefineConstants>TRACE;ANDROID DPSFAsDrawableGameComponent</DefineConstants>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\Release\DPSFMonoForAndoid.xml</DocumentationFile>', '<DocumentationFile>bin\Release\DPSFMonoForAndoidAsDrawableGameComponent.xml</DocumentationFile>')
	}
    elseif ((Split-Path -Path $CsprojFilePath -Leaf) -eq 'Mono for Android Copy of DPSF.csproj')
	{
		$fileContents = $fileContents.Replace('<AssemblyName>DPSFXamarin.iOS</AssemblyName>', '<AssemblyName>DPSFXamarin.iOSAsDrawableGameComponent</AssemblyName>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\Debug\DPSFXamarin.iOS.xml</DocumentationFile>', '<DocumentationFile>bin\Debug\DPSFXamarin.iOSAsDrawableGameComponent.xml</DocumentationFile>')
		$fileContents = $fileContents.Replace('<DefineConstants>TRACE;iOS</DefineConstants>', '<DefineConstants>TRACE;iOS DPSFAsDrawableGameComponent</DefineConstants>')
		$fileContents = $fileContents.Replace('<DocumentationFile>bin\Release\DPSFXamarin.iOS.xml</DocumentationFile>', '<DocumentationFile>bin\Release\DPSFXamarin.iOSAsDrawableGameComponent.xml</DocumentationFile>')
	}
	else
	{
		# Throw an error.
		throw "ERROR: Invalid .csproj file name given. Cannot find the file '$CsprojFilePath'."
	} 
	
	# Write the new file contents to the file.
	[System.IO.File]::WriteAllText($CsprojFilePath, $fileContents)
}

function UpdateCsprojFileToReferenceDllInDpsfInstallDirectory
{
 <#
	.SYNOPSIS
	Edits the given .csproj file to refernce the DPSF dlls from the DPSF Install directory.

	.DESCRIPTION
	Edits the given .csproj file to refernce the DPSF dlls from the DPSF Install directory.
#>
	param 
	(
		[ValidateNotNullOrEmpty()]
		[String] $CsprojFilePath
	)
	
	# Read in the entire file contents
	$fileContents = [System.IO.File]::ReadAllText($CsprojFilePath)
	
	# Replace all of the necessary settings, depending on which csproj file this is.
	if ((Split-Path -Path $CsprojFilePath -Leaf) -eq 'DPSF Demo.csproj')
	{
		$projectAssemblyName = "DPSF"
		$projectFileName = "DPSF.csproj"
	} 
	elseif ((Split-Path -Path $CsprojFilePath -Leaf) -eq 'Xbox 360 Copy of DPSF Demo.csproj')
	{
		$projectAssemblyName = "DPSFXbox360"
		$projectFileName = "Xbox 360 Copy of DPSF.csproj"
	}
	elseif ((Split-Path -Path $CsprojFilePath -Leaf) -eq 'DPSF Demo for Windows Phone.csproj')
	{
		$projectAssemblyName = "DPSFPhone"
		$projectFileName = "Windows Phone Copy of DPSF.csproj"
	}
	elseif ((Split-Path -Path $CsprojFilePath -Leaf) -eq 'DPSF Demo for Mono for Android.csproj')
	{
		$projectAssemblyName = "DPSFMonoForAndoid"
		$projectFileName = "Mono for Android Copy of DPSF.csproj"
	}
	elseif ((Split-Path -Path $CsprojFilePath -Leaf) -eq 'DPSF Demo for WinRT.csproj')
	{
		$projectAssemblyName = "DPSFWinRT"
		$projectFileName = "DPSF WinRT.csproj"
	}
	else
	{
		# Throw an error.
		throw "ERROR: Invalid .csproj file name given. Cannot find the file '$CsprojFilePath'."
	}
	
	# Replace the regex match with the replacement text.
	$dllFilePath = Join-Path $DPSF_DEFAULT_INSTALL_DIRECTORY "$projectAssemblyName.dll"
	$regex = [regex] "(?i)(<ProjectReference Include=`".*?\\$projectFileName`">(.|\n|\r)*?</ProjectReference>)"
	$replacementText = "<Reference Include=`"$projectAssemblyName`"><HintPath>$dllFilePath</HintPath></Reference>"
	$fileContents = $regex.Replace($fileContents, $replacementText)
	
	# Write the new file contents to the file.
	[System.IO.File]::WriteAllText($CsprojFilePath, $fileContents)
}

function UninstallDPSF
{
	# If DPSF is installed, launch the uninstaller.
	if (Test-Path $DPSF_UNINSTALLER_FILE_PATH)
	{
		Write-Host "Launching DPSF Uninstaller for you to uninstall DPSF before installing the new version..."
		Invoke-Item $DPSF_UNINSTALLER_FILE_PATH
		
		Write-Host "Prompt for when the DPSF Uninstaller has completed..."
		if ([System.Windows.Forms.MessageBox]::Show("Hit OK once DPSF has been uninstalled.", "Uninstall DPSF", [System.Windows.Forms.MessageBoxButtons]::OKCancel, [System.Windows.Forms.MessageBoxIcon]::Stop) -eq [System.Windows.Forms.DialogResult]::Cancel)
		{
			Write-Host "Exiting script since Cancel was pressed when asked to uninstall DPSF."
			Exit
		}
	}
}

# Show an Open Folder Dialog and return the directory selected by the user.
function Read-FolderBrowserDialog([string]$Message, [string]$InitialDirectory)
{
	$app = New-Object -ComObject Shell.Application
	$folder = $app.BrowseForFolder(0, $Message, 0, $InitialDirectory)
	if ($folder) { return $folder.Self.Path } else { return '' }
}

# Catch any exceptions thrown and stop the script.
trap [Exception] { Write-Host "Error: $_"; break; }


#==========================================================
# Perform the script tasks.
#==========================================================

Write-Host "Beginning script to create DPSF Release..."

$creatingRealRelease = $false
Write-Host "Prompt to see if we are making an actual official release or not..."
$answer = [System.Windows.Forms.MessageBox]::Show("Are you making an actual official release?`nIf not, many prompts will be skipped and the script will exit after the new DLL files have been created.", "Is This A Real Release?", [System.Windows.Forms.MessageBoxButtons]::YesNoCancel, [System.Windows.Forms.MessageBoxIcon]::Question)
if ($answer -eq [System.Windows.Forms.DialogResult]::Yes)
{
	$creatingRealRelease = $true
}
elseif ($answer -eq [System.Windows.Forms.DialogResult]::Cancel)
{
	Write-Host "Exiting script because you clicked the Cancel button."
	Exit
}

<#
0a - If the DPSPDefaultEffect.fx file was modified, you will need to re-add the .bin files as resources so that the changes take effect. 
Make sure to do the build in release mode so the generated .bin files are nice and small, then go into DPSFResources.resx in the DPSF Project, remove the effect resources, 
and then re-add them from "DPSF/DPSF Effects/Raw Effect Code". Be sure to do a thorough test on both the PC and Xbox for all particle types to make sure everything is good.

0b - If any files were added, removed, renamed, or moved in the DPSF project, you must reflect these changes in the 'DPSF WinRT' and 'Mono for Android Copy of DPSF' project as well.
#>

Write-Host "Prompt to see if the DPSFDefaultEffect.fx file was modified..."
if ($creatingRealRelease -and [System.Windows.Forms.MessageBox]::Show("Was the DPSFDefaultEffect.fx file modified?", "Was The Effect File Modified?", [System.Windows.Forms.MessageBoxButtons]::YesNo, [System.Windows.Forms.MessageBoxIcon]::Question) -eq [System.Windows.Forms.DialogResult]::Yes)
{
	$prompt = "Did you re-add the .bin files as resources so that the changes take effect?"
	if ($creatingRealRelease -and [System.Windows.Forms.MessageBox]::Show($prompt, "Did You Update The Bin Files In DPSFReources.resx?", [System.Windows.Forms.MessageBoxButtons]::YesNo, [System.Windows.Forms.MessageBoxIcon]::Question) -eq [System.Windows.Forms.DialogResult]::No)
	{
		Write-Host "To re-add the .bin files do a build in Release mode, then go into DPSFResources.resx in the DPSF Project, remove the effect resources, and then re-add them from 'DPSF\DPSF Effects\Raw Effect Code'."
		Write-Host "Exiting the script so that you can update the Effect in the Resource files."
		Exit
	}
}

Write-Host "Prompt to see if any project files were added, removed, renamed, or moved..."
if ($creatingRealRelease -and [System.Windows.Forms.MessageBox]::Show("Were any files added, removed, renamed, or moved in the projects?", "Were Project File Names Modified?", [System.Windows.Forms.MessageBoxButtons]::YesNo, [System.Windows.Forms.MessageBoxIcon]::Question) -eq [System.Windows.Forms.DialogResult]::Yes)
{
	$prompt = "Did you reflect the changes in the 'DPSF WinRT' and 'Mono for Android Copy of DPSF' projects?"
	if ($creatingRealRelease -and [System.Windows.Forms.MessageBox]::Show($prompt, "Did You Update The Bin Files In DPSFReources.resx?", [System.Windows.Forms.MessageBoxButtons]::YesNo, [System.Windows.Forms.MessageBoxIcon]::Question) -eq [System.Windows.Forms.DialogResult]::No)
	{
		Write-Host "Exiting the script so that you can update the 'DPSF WinRT' and 'Mono for Android Copy of DPSF' projects."
		Exit
	}
}

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
	$VersionNumber = [Microsoft.VisualBasic.Interaction]::InputBox("Enter the 4 hex-value version number to build the DPSF assemblies with (x.x.x.x).`nThe last hex value is typically zero unless making an unoffical build", "DPSF Version Number To Use", $currentVersionNumber)
}

# Set the Version Number
$VersionNumber = DpsfVersionNumber -SetVersionNumber -NewVersionNumber $VersionNumber

Write-Host "New DPSF version number set to '$VersionNumber'."

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
$buildSucceeded = Invoke-MsBuild -Path "$DPSF_SOLUTION_FILE_PATH" -MsBuildParameters "$MSBUILD_PARAMETERS" -BuildLogDirectoryPath "$MSBUILD_LOG_DIRECTORY_PATH" -AutoLaunchBuildLogOnFailure
if (!$buildSucceeded) { throw "Build failed." }
Write-Host "Building the DPSF WinRT solution..."
$buildSucceeded = Invoke-MsBuild -Path "$DPSF_WINRT_SOLUTION_FILE_PATH" -MsBuildParameters "$WINRT_MSBUILD_PARAMETERS" -BuildLogDirectoryPath "$MSBUILD_LOG_DIRECTORY_PATH" -AutoLaunchBuildLogOnFailure
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
$buildSucceeded = Invoke-MsBuild -Path "$DPSF_SOLUTION_FILE_PATH" -MsBuildParameters "$MSBUILD_PARAMETERS" -BuildLogDirectoryPath "$MSBUILD_LOG_DIRECTORY_PATH" -AutoLaunchBuildLogOnFailure
if (!$buildSucceeded) { throw "Build failed." }
Write-Host "Building the DPSF WinRT solution for AsDrawableGameComponent DLLs..."
$buildSucceeded = Invoke-MsBuild -Path "$DPSF_WINRT_SOLUTION_FILE_PATH" -MsBuildParameters "$WINRT_MSBUILD_PARAMETERS" -BuildLogDirectoryPath "$MSBUILD_LOG_DIRECTORY_PATH" -AutoLaunchBuildLogOnFailure
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
Copy-Item -Path "$LATEST_DLL_FILES_DIRECTORY_PATH/*" -Destination $DPSF_DEFAULT_INSTALL_DIRECTORY -Include "*.dll","*.xml"

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
Write-Host "Prompt to have user manually verify that the Test solutions work as expected..."
if ($creatingRealRelease -and [System.Windows.Forms.MessageBox]::Show("Do you want to launch the Test solutions to verify they work correctly?", "Launch Test Solutions", [System.Windows.Forms.MessageBoxButtons]::YesNo, [System.Windows.Forms.MessageBoxIcon]::Question) -eq [System.Windows.Forms.DialogResult]::Yes)
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
	Write-Host "Prompt to confirm that the test solutions ran correctly..."
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
RoboCopy "$DPSF_DEMO_TEMPLATES_DIRECTORY_PATH" "$INSTALLER_FILES_TEMPLATES_DIRECTORY_PATH" /s > $null	# Don't output the files copied to the powershell window.

Write-Host "Copying the files in '$DPSF_DEFAULTS_DIRECTORY' to '$INSTALLER_FILES_TEMPLATES_DPSF_DEFAULTS_DIRECTORY'..."
RoboCopy "$DPSF_DEFAULTS_DIRECTORY" "$INSTALLER_FILES_TEMPLATES_DPSF_DEFAULTS_DIRECTORY" /s > $null	# Don't output the files copied to the powershell window.

Write-Host "Copying the file '$DPSF_DEFAULT_EFFECT_FILE_PATH' to the folder '$INSTALLER_FILES_TEMPLATES_DIRECTORY_PATH'..."
Copy-Item -Path "$DPSF_DEFAULT_EFFECT_FILE_PATH" -Destination "$INSTALLER_FILES_TEMPLATES_DIRECTORY_PATH"

# If we are not creating a full release, exit the script now.
if (-not $creatingRealRelease)
{
	Write-Host "Zipping temporary dll files up..."
	Write-Zip -Path $LATEST_DLL_FILES_DIRECTORY_PATH\* -OutputPath "$LATEST_DLL_FILES_DIRECTORY_PATH\DPSF $VersionNumber DLLs.zip" -Level 9 -Quiet > $null # Don't output the zip file info to the powershell window.

	Write-Host "Opening folder containing new DLL files..."
	Invoke-Item $LATEST_DLL_FILES_DIRECTORY_PATH

	Write-Host "Exiting script since this was not a full release."
	Exit
}

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

# Update the .csproj files' to build the AsDrawableGameComponent DLLs.
Write-Host "Updating the DPSF Demo .csproj files to reference the DPFS DLLs from the default DPFS install directory..."
if (!(Test-Path $CSPROJ_FILE_PATHS_BACKUP_DIRECTORY)) { New-Item -ItemType Directory -Path $CSPROJ_FILE_PATHS_BACKUP_DIRECTORY > $null }	# Create the temp folder (and trash the output it creates).
foreach ($csprojFilePath in $DPSF_DEMO_CSPROJ_FILE_PATHS)
{
	# Backup the file before modifying it.
	# Have to back these up to a different directory outside of the Installer Files folder so they aren't included in the DPSF installer.
	$fileName = [System.IO.Path]::GetFileName($csprojFilePath)
	$backupFilePath = Join-Path $CSPROJ_FILE_PATHS_BACKUP_DIRECTORY "$fileName.backup"
	Copy-Item -Path $csprojFilePath -Destination $backupFilePath
	
	UpdateCsprojFileToReferenceDllInDpsfInstallDirectory $csprojFilePath
}

<#
15 - Re-run the "DPSF\DPSF.sln" in x86 Release mode to generate the executable and required .xnb files so that the DPSF Demo can be ran without 
needing Visual Studio. No other projects generate an executable that can be run by Windows, so we don't need to do this with them. 
Then change the configuration manager back to Mixed Debug mode when done.
#>

# Build the DPSF solution in x86 Release mode to create the new .xnb files.
Write-Host "Building the DPSF solution in x86 Release mode to create .xnb files for the installer..."
$buildSucceeded = Invoke-MsBuild -Path "$DPSF_SOLUTION_FILE_PATH" -MsBuildParameters "$MSBUILD_PARAMETERS_X86" -BuildLogDirectoryPath "$MSBUILD_LOG_DIRECTORY_PATH" -AutoLaunchBuildLogOnFailure
if (!$buildSucceeded) { throw "Build failed." }

<#
16 - Update the "DPSF API Documentation" to use the .dll's new .xml files generated (using Sandcastle Help File Builder program). 
You will need to update the HelpFileVersion to match the new DPSF version number.
#>

# Update the version number in the API Documentation builder project file.
Write-Host "Updating version number in the DPSF API Documentation builder project file..."
$fileContents = [System.IO.File]::ReadAllText($API_DOCUMENTATION_SANDCASTLE_PROJECT_FILE_PATH)
$rxHelpFileVersion = [regex] "(?i)(<HelpFileVersion>.*?</HelpFileVersion>)"
$fileContents = $rxHelpFileVersion.Replace($fileContents, "<HelpFileVersion>$VersionNumber</HelpFileVersion>")
[System.IO.File]::WriteAllText($API_DOCUMENTATION_SANDCASTLE_PROJECT_FILE_PATH, $fileContents)

# Open the API Documentation builder project so that we can manually create the new .chm file.
Write-Host "Launching Sandcastle so you can generate the DPSF API Documentation..."
Invoke-Item $API_DOCUMENTATION_SANDCASTLE_PROJECT_FILE_PATH

Write-Host "Prompt for when the DPSF API Documentation has been generated..."
if ([System.Windows.Forms.MessageBox]::Show("Hit OK once the new DPSF API Documentation file has been generated by Sandcastle.", "Create Sandcastle Documentation", [System.Windows.Forms.MessageBoxButtons]::OKCancel, [System.Windows.Forms.MessageBoxIcon]::Stop) -eq [System.Windows.Forms.DialogResult]::Cancel)
{
	Write-Host "Exiting script since Cancel was pressed when asked to create the new DPSF API Documentation using Sandcastle."
	Exit
}

<#
17 - Update the Help document (including the change log), generating a new "DPSF Help.chm" and copy it into the "Installer Files" folder. 
Generating the Help document has it's own process document that should be followed (DPSF Help Update Process.txt).
#>

# Delete the existing HTML documentation if it exists.
if (Test-Path $HELP_DOCUMENTATION_HTML_DIRECTORY)
{
	Write-Host "Deleting Help Documentation HTML directory..."
	Remove-Item -Path $HELP_DOCUMENTATION_HTML_DIRECTORY -Recurse -Force
}

Write-Host "Launching Help And Manual project and Help Update Process file so you can update and generate the new Help Documentation..."
Invoke-Item $HELP_DOCUMENTATION_HELP_AND_MANUAL_PROJECT_FILE_PATH
Invoke-Item $HELP_DOCUMENTATION_UPDATE_PROCESS_FILE_PATH

Write-Host "Prompt for when the DPSF Help has finished being generated..."
if ([System.Windows.Forms.MessageBox]::Show("Hit OK once the new DPSF Help documentation has been generated by Help And Manual.", "Create New DPSF Help Documentation", [System.Windows.Forms.MessageBoxButtons]::OKCancel, [System.Windows.Forms.MessageBoxIcon]::Stop) -eq [System.Windows.Forms.DialogResult]::Cancel)
{
	Write-Host "Exiting script since Cancel was pressed when asked to create the new 'DPSF Help.chm' using Help And Manual."
	Exit
}

# Copy the 'DPSF Help.chm' to the 'Installer Files' directory.
Write-Host "Copying updated '$HELP_DOCUMENTATION_CHM_FILE_NAME' to '$INSTALLER_FILES_DIRECTORY_PATH'..."
Copy-Item -Path (Join-Path $HELP_FILES_DIRECTORY_PATH $HELP_DOCUMENTATION_CHM_FILE_NAME) -Destination (Join-Path $INSTALLER_FILES_DIRECTORY_PATH $HELP_DOCUMENTATION_CHM_FILE_NAME)

<#
18 - Open the "DPSF Installer Settings.iit" and Build a new "DPSF Installer.exe", making sure to include any new links that should appear in the Start Menu DPSF folder, such as links to new tutorials, demos, etc. and update the DPSF EULA if it was updated in the help documentation.
#>

Write-Host "Opening the DPSF Install Creator project for you to create a new 'DPSF Installer.exe'..."
Invoke-Item $INSTALLER_CREATOR_PROJECT_FILE_PATH

Write-Host "Prompt for when the new DPSF Installer has finished being created..."
if ([System.Windows.Forms.MessageBox]::Show("Make sure to include any new links that should appear in the Start Menu DPSF folder, such as links to new tutorials, demos, etc. and update the DPSF EULA if it was updated in the help documentation.`n`nHit OK once the new 'DPSF Installer.exe' has been created.", "Create New DPSF Installer", [System.Windows.Forms.MessageBoxButtons]::OKCancel, [System.Windows.Forms.MessageBoxIcon]::Stop) -eq [System.Windows.Forms.DialogResult]::Cancel)
{
	Write-Host "Exiting script since Cancel was pressed when asked to create the new 'DPSF Installer.exe'."
	Exit
}

<#
19 - Install DPSF from the new installer and make sure the DPSF Demo works properly. Then uninstall it and make sure everything is removed properly.
#>

# If DPSF is already installed, uninstall it before installing the new version.
UninstallDPSF

# Remove any files that may have been left over in the DPSF directory.
Write-Host "Removing '$DPSF_DEFAULT_INSTALL_DIRECTORY' in case files were left over from the uninstall..."
Remove-Item -Path $DPSF_DEFAULT_INSTALL_DIRECTORY -Recurse -Force

Write-Host "Starting the DPSF Installer for you to install DPSF and verify it works correctly..."
Invoke-Item $DPSF_INSTALLER_FILE_PATH

Write-Host "Prompt for when the DPSF Installer has finished installing DPSF..."
if ([System.Windows.Forms.MessageBox]::Show("Hit OK once the new version of DPSF has been installed.", "Install DPSF", [System.Windows.Forms.MessageBoxButtons]::OKCancel, [System.Windows.Forms.MessageBoxIcon]::Stop) -eq [System.Windows.Forms.DialogResult]::Cancel)
{
	Write-Host "Exiting script since Cancel was pressed when asked to install the new version of DPSF."
	Exit
}

Write-Host "Prompt that DPSF was installed correctly..."
if ([System.Windows.Forms.MessageBox]::Show("Was DPSF installed correctly?`n`nCan you run the DPSF Demo from the Start Menu?", "Was DPSF Installed Correctly?", [System.Windows.Forms.MessageBoxButtons]::YesNo, [System.Windows.Forms.MessageBoxIcon]::Question) -eq [System.Windows.Forms.DialogResult]::No)
{
	Write-Host "Exiting script because you said that DPSF was not installed correctly."
	Exit
}

# Uninstall DPSF to make sure the uninstall went properly.
UninstallDPSF

# If the default DPSF install directory still exists after doing the uninstall.
if (Test-Path $DPSF_DEFAULT_INSTALL_DIRECTORY)
{
	Write-Host "Prompt to see if the user wants to continue since DPSF does not appear to have uninstalled correctly..."
	if ([System.Windows.Forms.MessageBox]::Show("'$DPSF_DEFAULT_INSTALL_DIRECTORY' still exists after DPSF was uninstalled. This folder should have been completely deleted.`n`nDo you want to continue making the new Release anyways?", "Continue Even Though Uninstall Went Wrong?", [System.Windows.Forms.MessageBoxButtons]::YesNo, [System.Windows.Forms.MessageBoxIcon]::Question) -eq [System.Windows.Forms.DialogResult]::No)
	{
		Write-Host "Exiting script because '$DPSF_DEFAULT_INSTALL_DIRECTORY' still exists after uninstalling DPSF and you chose to cancel the release process."
		Exit
	}
}

<#
20 - Create a copy of the installer and move it into the "Archived Installers" section, renaming it with it's version number.
#>

# Grab the first 3 hex values of the version number (i.e. the public version number) and create the path to copy the DPSF Installer to.
$rxPublicVersionNumber = [regex] "^\d{1,5}\.\d{1,5}\.\d{1,5}"
$publicVersionNumber = rxPublicVersionNumber.Match($VersionNumber).Value
$newDpsfInstallerInArchiveDirectoryFilePath = Join-Path $ARCHIVED_INSTALLERS_DIRECTORY "DPSF Installer v$publicVersionNumber.exe"

Write-Host "Copying new DPSF Installer to '$newDpsfInstallerInArchiveDirectoryFilePath'"
Copy-Item -Path $DPSF_INSTALLER_FILE_PATH -Destination $newDpsfInstallerInArchiveDirectoryFilePath

<#
21 - In the DPSF.sln and "DPSF WinRT.sln", change the DPSF Demo projects back to referencing the DPSF projects, rather than the DLL files in 
the C:\DPSF folder, and change back to using the Debug Mixed configuration.
#>

Write-Host "Prompt to revert the DPSF Demo project files back..."
if ([System.Windows.Forms.MessageBox]::Show("Revert the DPSF Demo project files now?", "Revert DPSF Demo Project Files?", [System.Windows.Forms.MessageBoxButtons]::YesNo, [System.Windows.Forms.MessageBoxIcon]::Question) -eq [System.Windows.Forms.DialogResult]::Yes)
{
	# Revert the .csproj files back to their original states now that we have the DLLs.
	Write-Host "Reverting .csproj files back to their original states..."
	foreach ($csprojFilePath in $DPSF_DEMO_CSPROJ_FILE_PATHS)
	{
		$fileName = [System.IO.Path]::GetFileName($csprojFilePath)
		$backupFilePath = Join-Path $CSPROJ_FILE_PATHS_BACKUP_DIRECTORY "$fileName.backup"
	
		# Copy the backup back overtop of the original to revert it, and then delete the backup file.
		if (Test-Path "$backupFilePath")
		{
			Copy-Item -Path "$backupFilePath" -Destination "$csprojFilePath"
			Remove-Item -Path "$backupFilePath"
		}
	}
	
	# Delete the backups directory now that we are done with it.
	Remove-Item -Path $CSPROJ_FILE_PATHS_BACKUP_DIRECTORY
}

<#
22 - Check files into Git, adding the current dll version (e.g. v1.0.1.1) and Change Log to the SVN commit comments.
#>

# Open the Changelog file so user can copy the comments into the Git commit.
Invoke-Item -Path $DPSF_CHANGE_LOG_FILE_PATH

# Launch the Git Extensions commit window so user can check in the changes.
Start-Process -FilePath cmd.exe -ArgumentList "/k `" cd `"$DPSF_REPOSITORY_ROOT_DIRECTORY`" & `"$GIT_EXTENSIONS_COMMAND_LINE_TOOL_PATH`" commit & EXIT"

Write-Host "Prompt to see if the user has finished checking files into Git..."
if ([System.Windows.Forms.MessageBox]::Show("Hit OK once the you have checked the changes into Git.`n`nInclude the ", "Check Changes Into Git", [System.Windows.Forms.MessageBoxButtons]::OKCancel, [System.Windows.Forms.MessageBoxIcon]::Stop) -eq [System.Windows.Forms.DialogResult]::Cancel)
{
	Write-Host "Exiting script since Cancel was pressed when asked to check the changes into Git."
	Exit
}

<#
23 - Rebuild the DPSF.sln in Release mode so that it doesn't error out next time we build it in Debug mode, since the Android project relies on the .xnb files being created in bin\Release.
#>

Write-Host "Building the DPSF solution to create required .xnb files before it can be built in Debug mode..."
Invoke-MsBuild -Path "$DPSF_SOLUTION_FILE_PATH" -MsBuildParameters "$MSBUILD_PARAMETERS" -BuildLogDirectoryPath "$MSBUILD_LOG_DIRECTORY_PATH" -ShowBuildWindow -PassThru

<#
24 - Upload the new DPSF Installer and HTML help files to the Dev website.
#>

# If the path to the DPSF Dev Website doesn't exist, prompt the user for it.
if (!(Test-Path $DPSF_DEV_WEBSITE_DIRECTORY_PATH))
{
	$DPSF_DEV_WEBSITE_DIRECTORY_PATH = Read-FolderBrowserDialog -Message "Please select the DPSF Dev Website folder" -InitialDirectory $DPSF_REPOSITORY_ROOT_DIRECTORY
	
	# If the user didn't choose a valid directory, exit.
	if (!(Test-Path $DPSF_DEV_WEBSITE_DIRECTORY_PATH))
	{
		Write-Host "Exiting script because a valid directory was not chosen for the DSPF Dev Website."
		Exit
	}
}

# Extract the previous DPSF Installer zip file into the Dev website's archived installers directory.
Write-Host "Unzipping previous DPSF Installer to the Dev website's archived installers directory..."
Expand-Archive -Path $DPSF_DEV_WEBSITE_DPSF_INSTALLER_ZIP_FILE_PATH -OutputPath $DPSF_DEV_WEBSITE_ARCHIVED_INSTALLERS_DIRECTORY

# Delete the old DPSF Installer zip file from the Dev website.
Write-Host "Deleting old '$DPSF_DEV_WEBSITE_DPSF_INSTALLER_ZIP_FILE_PATH'..."
Remove-Item $DPSF_DEV_WEBSITE_DPSF_INSTALLER_ZIP_FILE_PATH

# Zip up the new DPSF Installer and copy it to the Dev website.
Write-Host "Zipping up new DPSF installer and copying it to '$DPSF_DEV_WEBSITE_DPSF_INSTALLER_ZIP_FILE_PATH'..."
Write-Zip -Path $newDpsfInstallerInArchiveDirectoryFilePath -OutputPath $DPSF_DEV_WEBSITE_DPSF_INSTALLER_ZIP_FILE_PATH -Level 9 -Quiet > $null # Don't output the zip file info to the powershell window.

# Delete the previous HTML DPSF Help documenation on the Dev website.
Write-Host "Deleting the old Dev website HTML Help Documentation at '$DPSF_DEV_WEBSITE_HELP_FILES_DIRECTORY'."
Remove-Item -Recurse -Path $DPSF_DEV_WEBSITE_HELP_FILES_DIRECTORY

# Move to the new HTML DPSF Help Documentation to the Dev website.
Write-Host "Moving the new HTML DPSF Help documentation to the Dev website at '$DPSF_DEV_WEBSITE_HELP_FILES_DIRECTORY'."
RoboCopy $HELP_DOCUMENTATION_HTML_DIRECTORY $DPSF_DEV_WEBSITE_HELP_FILES_DIRECTORY /move > $null	# Don't output the files copied to the powershell window.

<#
25 - The web has it's own "Release Process.txt" file to follow to finish posting the new DPSF version online.
#>

# Open the Dev website Release Process file for user to finish up those steps.
Write-Host "Opening the Dev website release process file '$DPSF_DEV_WEBSITE_RELEASE_PROCESS_FILE_PATH'."
Invoke-Item -Path $DPSF_DEV_WEBSITE_RELEASE_PROCESS_FILE_PATH

# Tell user to do the Dev website Release Process steps to complete the release process.
Write-Host "Prompt user to complete the Dev website steps..."
[System.Windows.Forms.MessageBox]::Show("Follow the steps in the Dev website's Release Process file to complete the release process.", "Perform Dev Website Steps", [System.Windows.Forms.MessageBoxButtons]::OKCancel, [System.Windows.Forms.MessageBoxIcon]::Stop)


<#
26 - Build and push the new DPSF NuGet Packages
#>

Write-Host "Creating and pushing new DPSF NuGet packages..."
& $CREATE_AND_PUSH_NEW_NUGET_PACKAGE_SCRIPT_PATH -VersionNumber $VersionNumber



# Tell user that we are finished.
Write-Host "All done!!!"

# Wait for input before closing.
Write-Host "Press any key to continue ..."
$x = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyUp")



<#

When releasing a new version of DPSF, be sure to follow these steps:

0a - If the DPSPDefaultEffect.fx file was modified, you will need to re-add the .bin files as resources so that the changes take effect. Make sure to do the build in release mode so the generated .bin files are nice and small, then go into DPSFResources.resx in the DPSF Project, remove the effect resources, and then re-add them from "DPSF/DPSF Effects/Raw Effect Code". Be sure to do a thorough test on both the PC and Xbox for all particle types to make sure everything is good.

0b - If any files were added, removed, renamed, or moved in the DPSF project, you must reflect these changes in the 'DPSF WinRT' and 'Mono for Android Copy of DPSF' project as well.

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

15 - Re-run the "DPSF\DPSF.sln" in x86 Release mode to generate the executable and required .xnb files so that the DPSF Demo can be ran without needing Visual Studio. No other projects generate an executable that can be run by Windows, so we don't need to do this with them. Then change the configuration manager back to Mixed Debug mode when done.

16 - Update the "DPSF API Documentation" to use the .dll's new .xml files generated (using Sandcastle Help File Builder program). You will need to update the HelpFileVersion to match the new DPSF version number.

17 - Update the Help document (including the change log), generating a new "DPSF Help.chm" and copy it into the "Installer Files" folder. Generating the Help document has it's own process document that should be followed (DPSF Help Update Process.txt).

18 - Open the "DPSF Installer Settings.iit" and Build a new "DPSF Installer.exe", making sure to include any new links that should appear in the Start Menu DPSF folder, such as links to new tutorials, demos, etc. and update the DPSF EULA if it was updated in the help documentation.

19 - Install DPSF from the new installer and make sure the DPSF Demo works properly. Then uninstall it and make sure everything is removed properly.

20 - Create a copy of the installer and move it into the "Archived Installers" section, renaming it with it's version number.

21 - In the DPSF.sln and "DPSF WinRT.sln", change the DPSF Demo projects back to referencing the DPSF projects, rather than the DLL files in the C:\DPSF folder, and change back to using the Debug Mixed configuration.

22 - Check files into Git, adding the current dll version (e.g. v1.0.1.1) and Change Log to the SVN commit comments.

23 - Rebuild the DPSF.sln in Release mode so that it doesn't error out next time we build it in Debug mode, since the Android project relies on the .xnb files being created in bin\Release.

24 - Upload the new DPSF Installer and HTML help files to the Dev website.

25 - The web has it's own "Release Process.txt" file to follow to finish posting the new DPSF version online.

#>
