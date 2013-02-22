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


#==========================================================
# Perform the script tasks.
#==========================================================

# Delete the existing Dpsf files before building the new ones.
Write-Host "Deleting existing DLLs..."
Remove-Item -Recurse -Path "$LATEST_DLL_FILES_DIRECTORY_PATH"					# Delete the entire folder.
New-Item -ItemType Directory -Path "$LATEST_DLL_FILES_DIRECTORY_PATH" > $null	# Recreate the empty folder (and trash the output it creates).

# Build the DPSF solution in Release mode to create the new Dlls.
Write-Host "Building the DPSF solution..."
Invoke-MsBuild -Path "$DPSF_SOLUTION_FILE_PATH" -Configuration "Release" -BuildLogDirectoryPath "$MSBUILD_LOG_DIRECTORY_PATH" -BuildVerbosity Quiet -ShowBuildWindow
Write-Host "Building the DPSF WinRT solution..."
Invoke-MsBuild -Path "$DPSF_WINRT_SOLUTION_FILE_PATH" -Configuration "Release" -BuildLogDirectoryPath "$MSBUILD_LOG_DIRECTORY_PATH" -BuildVerbosity Quiet -ShowBuildWindow






<#

#==========================================================
desc "Update the .csproj files to build the AsDrawableGameComponent DLLs."
task :UpdateCsprojFilesToBuildAsDrawableGameComponentDLLs do
	# Update the .csproj files to build with AsDrawableGameComponent.
	puts 'Updating the .csproj files to build AsDrawableGameComponent DLLs...'
	CSPROJ_FILE_PATHS.each do |csprojFilePath|
		UpdateCsprojFileToAsDrawableGameComponent(csprojFilePath)
	end
end

# Define the function to actually do the text read, replace, and write.
def UpdateCsprojFileToAsDrawableGameComponent(csprojFilePath)
	# Backup the file before modifying it.
	FileUtils.copy_file(csprojFilePath, csprojFilePath + '.backup')

	# Read in the entire file contents
	fileContents = File.read(csprojFilePath)

	# Replace all of the necessary settings, depending on which csproj file this is.
	if (File.basename(csprojFilePath) == 'DPSF.csproj')
		fileContents.gsub!('<AssemblyName>DPSF</AssemblyName>', '<AssemblyName>DPSFAsDrawableGameComponent</AssemblyName>')
		fileContents.gsub!('<DocumentationFile>bin\x86\Debug\DPSF.xml</DocumentationFile>', '<DocumentationFile>bin\x86\Debug\DPSFAsDrawableGameComponent.xml</DocumentationFile>')
		fileContents.gsub!('<DefineConstants>TRACE;WINDOWS</DefineConstants>', '<DefineConstants>TRACE;WINDOWS DPSFAsDrawableGameComponent</DefineConstants>')
		fileContents.gsub!('<DocumentationFile>bin\x86\Release\DPSF.xml</DocumentationFile>', '<DocumentationFile>bin\x86\Release\DPSFAsDrawableGameComponent.xml</DocumentationFile>')
	elsif (File.basename(csprojFilePath) == 'Xbox 360 Copy of DPSF.csproj')
		fileContents.gsub!('<AssemblyName>DPSFXbox360</AssemblyName>', '<AssemblyName>DPSFXbox360AsDrawableGameComponent</AssemblyName>')
		fileContents.gsub!('<DocumentationFile>bin\Xbox 360\Debug\DPSFXbox360.xml</DocumentationFile>', '<DocumentationFile>bin\Xbox 360\Debug\DPSFXbox360AsDrawableGameComponent.xml</DocumentationFile>')
		fileContents.gsub!('<DefineConstants>TRACE;XBOX;XBOX360</DefineConstants>', '<DefineConstants>TRACE;XBOX;XBOX360 DPSFAsDrawableGameComponent</DefineConstants>')
		fileContents.gsub!('<DocumentationFile>bin\Xbox 360\Release\DPSFXbox360.xml</DocumentationFile>', '<DocumentationFile>bin\Xbox 360\Release\DPSFXbox360AsDrawableGameComponent.xml</DocumentationFile>')
	elsif (File.basename(csprojFilePath) == 'Windows Phone Copy of DPSF.csproj')
		fileContents.gsub!('<AssemblyName>DPSFPhone</AssemblyName>', '<AssemblyName>DPSFPhoneAsDrawableGameComponent</AssemblyName>')
		fileContents.gsub!('<DocumentationFile>bin\Windows Phone\Debug\DPSFPhone.xml</DocumentationFile>', '<DocumentationFile>bin\Windows Phone\Debug\DPSFPhoneAsDrawableGameComponent.xml</DocumentationFile>')
		fileContents.gsub!('<DefineConstants>TRACE;WINDOWS_PHONE</DefineConstants>', '<DefineConstants>TRACE;WINDOWS_PHONE DPSFAsDrawableGameComponent</DefineConstants>')
		fileContents.gsub!('<DocumentationFile>bin\Windows Phone\Release\DPSFPhone.xml</DocumentationFile>', '<DocumentationFile>bin\Windows Phone\Release\DPSFPhoneAsDrawableGameComponent.xml</DocumentationFile>')
	else
		# Display an error and cancel the rake build.
		puts "ERROR: Invalid .csproj file name given. Cannot find the file #{csprojFilePath}"
		exit
	end	

	# Write the new file contents to the file.
	File.open(csprojFilePath, 'w') do |f|
		f.puts fileContents
	end
end


#>



















<#
#==========================================================
# Do any necessary setup.
# - If it says that any of these gems are missing when you try and run this script, you can use the following to install them:
#	gem install --remote [rake|albaore|etc.]
#==========================================================
# Import any necessary modules (i.e. libraries).
require 'FileUtils'
require 'rake/clean'
require 'albacore'
require 'rubygems'
#include Rake::DSL	# This is needed to be able to invoke rake tasks from other tasks.

# Change our working directory to the directory this file is in.
THIS_FILES_ABSOLUTE_DIRECTORY_PATH = File.expand_path(File.dirname(__FILE__))
Dir.chdir(THIS_FILES_ABSOLUTE_DIRECTORY_PATH)

#==========================================================
# Define any necessary global variables, such as file paths.
#==========================================================
# Create function to return the absolute path of a relative file path.
def AbsoluteFilePath(relativeFilePath)
	return File.join(File.dirname(__FILE__), relativeFilePath)
end

# Define all of the absolute paths for all of the relative paths that we will work with.
DPSF_SOLUTION_FILE_PATH = AbsoluteFilePath('DPSF/DPSF.sln')
MSBUILD_LOG_FILE_PATH = AbsoluteFilePath('msbuild.log')
CSPROJ_FILE_PATHS = [AbsoluteFilePath('DPSF/DPSF/DPSF.csproj'), 
					AbsoluteFilePath('DPSF/DPSF/Xbox 360 Copy of DPSF.csproj'), 
					AbsoluteFilePath('DPSF/DPSF/Windows Phone Copy of DPSF.csproj')]
NEW_DLL_FILE_PATHS = [AbsoluteFilePath('DPSF/LatestDLLBuild/DPSF.dll'),
				AbsoluteFilePath('DPSF/LatestDLLBuild/DPSF.xml'),
				AbsoluteFilePath('DPSF/LatestDLLBuild/DPSFPhone.dll'),
				AbsoluteFilePath('DPSF/LatestDLLBuild/DPSFPhone.xml'),
				AbsoluteFilePath('DPSF/LatestDLLBuild/DPSFXbox360.dll'),
				AbsoluteFilePath('DPSF/LatestDLLBuild/DPSFXbox360.xml'),
				AbsoluteFilePath('DPSF/LatestDLLBuild/DPSFAsDrawableGameComponent.dll'),
				AbsoluteFilePath('DPSF/LatestDLLBuild/DPSFAsDrawableGameComponent.xml'),
				AbsoluteFilePath('DPSF/LatestDLLBuild/DPSFPhoneAsDrawableGameComponent.dll'),
				AbsoluteFilePath('DPSF/LatestDLLBuild/DPSFPhoneAsDrawableGameComponent.xml'),
				AbsoluteFilePath('DPSF/LatestDLLBuild/DPSFXbox360AsDrawableGameComponent.dll'),
				AbsoluteFilePath('DPSF/LatestDLLBuild/DPSFXbox360AsDrawableGameComponent.xml')]
				
				#AbsoluteFilePath('DPSF/LatestDLLBuild/DPSFXbox360AsDrawableGameComponent.xml'),
				#AbsoluteFilePath('DPSF/DPSF/DPSF Effects/Raw Effect Code/DPSFDefaultEffectWindowsHiDef.bin'),
				#AbsoluteFilePath('DPSF/DPSF/DPSF Effects/Raw Effect Code/DPSFDefaultEffectWindowsReach.bin'),
				#AbsoluteFilePath('DPSF/DPSF/DPSF Effects/Raw Effect Code/DPSFDefaultEffectXbox360HiDef.bin')]
INSTALLER_FILES_DIRECTORY_PATH = AbsoluteFilePath('Installer/Installer Files/')

# Albacore still defaults to MSBuild 3.5, so specify the exe location manually
#msBuildPath = 'C:/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe'
Albacore.configure do |config|
	config.msbuild.use :net4
end


#==========================================================
# Define the flow of execution for this rake script.
#==========================================================

task :default => :DoEverythingAndWaitForInput

desc "Builds new regular and AsDrawableGameComponent DLLs, then copies them to Installer Files directory, and waits for input."
task :DoEverythingAndWaitForInput => [:DoEverything, :WaitForInput]

desc "Builds new regular and AsDrawableGameComponent DLLs, then copies them to Installer Files directory."
task :DoEverything => [:BuildNewDLLs, :CopyDLLsToInstallerFilesDirectory]

desc "Builds new regular and AsDrawableGameComponent DLLs."
task :BuildNewDLLs => [:DeleteExistingDLLs, :Build, :UpdateCsprojFilesToBuildAsDrawableGameComponentDLLs, :ReBuild, :RevertCsprojFilesToBuildRegularDLLs, :DeleteBuildLog]


#==========================================================
# Define the script functionality.
#==========================================================

#==========================================================
desc "Deletes existing DLLs from the LatestDLLBuild folder."
task :DeleteExistingDLLs do
	
end

#==========================================================
desc "Builds the DPSF.sln in Release mode."
msbuild :Build do |msb|
	puts 'Building the DPSF solution...'
	msb.properties :configuration => :Release
	msb.targets [:Clean, :Rebuild]
	msb.solution = DPSF_SOLUTION_FILE_PATH
	msb.parameters "/nologo", "/maxcpucount", "/fileLogger", "/noconsolelogger"
	msb.verbosity = "quiet"	# Use "diagnostic" instead of "quiet" for troubleshooting build problems.
end

#==========================================================
desc "Calls the :Build target. :Build can only be called once by default, so call this to run the :Build task subsequent times."
task :ReBuild do
	::Rake.application['Build'].reenable
	::Rake.application['Build'].invoke
end

#==========================================================
desc "Deletes the build log file if it exists."
task :DeleteBuildLog do
	puts 'Deleting build log file...'
	if (File.exist?(MSBUILD_LOG_FILE_PATH))
		File.delete(MSBUILD_LOG_FILE_PATH)
	end
end

#==========================================================
desc "Update the .csproj files to build the AsDrawableGameComponent DLLs."
task :UpdateCsprojFilesToBuildAsDrawableGameComponentDLLs do
	# Update the .csproj files to build with AsDrawableGameComponent.
	puts 'Updating the .csproj files to build AsDrawableGameComponent DLLs...'
	CSPROJ_FILE_PATHS.each do |csprojFilePath|
		UpdateCsprojFileToAsDrawableGameComponent(csprojFilePath)
	end
end

# Define the function to actually do the text read, replace, and write.
def UpdateCsprojFileToAsDrawableGameComponent(csprojFilePath)
	# Backup the file before modifying it.
	FileUtils.copy_file(csprojFilePath, csprojFilePath + '.backup')

	# Read in the entire file contents
	fileContents = File.read(csprojFilePath)

	# Replace all of the necessary settings, depending on which csproj file this is.
	if (File.basename(csprojFilePath) == 'DPSF.csproj')
		fileContents.gsub!('<AssemblyName>DPSF</AssemblyName>', '<AssemblyName>DPSFAsDrawableGameComponent</AssemblyName>')
		fileContents.gsub!('<DocumentationFile>bin\x86\Debug\DPSF.xml</DocumentationFile>', '<DocumentationFile>bin\x86\Debug\DPSFAsDrawableGameComponent.xml</DocumentationFile>')
		fileContents.gsub!('<DefineConstants>TRACE;WINDOWS</DefineConstants>', '<DefineConstants>TRACE;WINDOWS DPSFAsDrawableGameComponent</DefineConstants>')
		fileContents.gsub!('<DocumentationFile>bin\x86\Release\DPSF.xml</DocumentationFile>', '<DocumentationFile>bin\x86\Release\DPSFAsDrawableGameComponent.xml</DocumentationFile>')
	elsif (File.basename(csprojFilePath) == 'Xbox 360 Copy of DPSF.csproj')
		fileContents.gsub!('<AssemblyName>DPSFXbox360</AssemblyName>', '<AssemblyName>DPSFXbox360AsDrawableGameComponent</AssemblyName>')
		fileContents.gsub!('<DocumentationFile>bin\Xbox 360\Debug\DPSFXbox360.xml</DocumentationFile>', '<DocumentationFile>bin\Xbox 360\Debug\DPSFXbox360AsDrawableGameComponent.xml</DocumentationFile>')
		fileContents.gsub!('<DefineConstants>TRACE;XBOX;XBOX360</DefineConstants>', '<DefineConstants>TRACE;XBOX;XBOX360 DPSFAsDrawableGameComponent</DefineConstants>')
		fileContents.gsub!('<DocumentationFile>bin\Xbox 360\Release\DPSFXbox360.xml</DocumentationFile>', '<DocumentationFile>bin\Xbox 360\Release\DPSFXbox360AsDrawableGameComponent.xml</DocumentationFile>')
	elsif (File.basename(csprojFilePath) == 'Windows Phone Copy of DPSF.csproj')
		fileContents.gsub!('<AssemblyName>DPSFPhone</AssemblyName>', '<AssemblyName>DPSFPhoneAsDrawableGameComponent</AssemblyName>')
		fileContents.gsub!('<DocumentationFile>bin\Windows Phone\Debug\DPSFPhone.xml</DocumentationFile>', '<DocumentationFile>bin\Windows Phone\Debug\DPSFPhoneAsDrawableGameComponent.xml</DocumentationFile>')
		fileContents.gsub!('<DefineConstants>TRACE;WINDOWS_PHONE</DefineConstants>', '<DefineConstants>TRACE;WINDOWS_PHONE DPSFAsDrawableGameComponent</DefineConstants>')
		fileContents.gsub!('<DocumentationFile>bin\Windows Phone\Release\DPSFPhone.xml</DocumentationFile>', '<DocumentationFile>bin\Windows Phone\Release\DPSFPhoneAsDrawableGameComponent.xml</DocumentationFile>')
	else
		# Display an error and cancel the rake build.
		puts "ERROR: Invalid .csproj file name given. Cannot find the file #{csprojFilePath}"
		exit
	end	

	# Write the new file contents to the file.
	File.open(csprojFilePath, 'w') do |f|
		f.puts fileContents
	end
end

#==========================================================
desc "Revert the .csproj files back to their original states now that we have the DLLs."
task :RevertCsprojFilesToBuildRegularDLLs do
	puts 'Reverting .csproj files back to their original states...'
	CSPROJ_FILE_PATHS.each do |csprojFilePath|
		RevertCsprojFile(csprojFilePath)
	end
end

# Define the function to revert a .csproj file back from its .backup file.
def RevertCsprojFile(csprojFilePath)
	# Copy the backup back overtop of the original to revert it, and then delete the backup file.
	FileUtils.copy_file(csprojFilePath + '.backup', csprojFilePath)
	File.delete(csprojFilePath + '.backup')
end

#==========================================================
desc "Copy the DLL files to the 'Installer Files' directory."
task :CopyDLLsToInstallerFilesDirectory do
	puts 'Copying new DLL files to the Installer Files directory...'
	FileUtils.cp(NEW_DLL_FILE_PATHS, INSTALLER_FILES_DIRECTORY_PATH)
end

#==========================================================
desc "Has the script wait for the user to press a key before continuing."
task :WaitForInput do
	puts 'Press any key to continue.'
	begin
		system("stty raw -echo")
		str = STDIN.getc
	ensure
		system("stty -raw echo")
	end
end
#>