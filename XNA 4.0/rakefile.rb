#==========================================================
# Do any necessary setup.
#==========================================================
# Import any necessary modules (i.e. libraries).
require 'FileUtils'
require 'rake'
require 'albacore'

# Change our working directory to the directory this file is in.
thisFilesAbsoluteDirectoryPath = File.expand_path(File.dirname(__FILE__))
Dir.chdir(thisFilesAbsoluteDirectoryPath)

#==========================================================
# Define any necessary variables, such as file paths.
#==========================================================
# Create function to return the absolute path of a relative file path.
def AbsoluteFilePath(relativeFilePath)
	return File.join(File.dirname(__FILE__), relativeFilePath)
end

# Define all of the absolute paths for all of the relative paths that we will work with.
dpsfSolutionFilePath = AbsoluteFilePath('DPSF/DPSF.sln')
csprojFilePaths = [AbsoluteFilePath('DPSF/DPSF/DPSF.csproj'), 
					AbsoluteFilePath('DPSF/DPSF/Xbox 360 Copy of DPSF.csproj'), 
					AbsoluteFilePath('DPSF/DPSF/Windows Phone Copy of DPSF.csproj')]
newDLLFiles = [AbsoluteFilePath('DPSF/LatestDLLBuild/DPSF.dll'),
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
installerFilesDirectory = AbsoluteFilePath('Installer/Installer Files/')

# Albacore still defaults to MSBuild 3.5, so specify the exe location manually
msBuildPath = 'C:/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe'


#==========================================================
# Define the flow of execution for this rake script.
#==========================================================
# task :default => [:full]
 
# task :full => [:clean,:build]
 


#==========================================================
# Build the solution in Release mode to create the regular DLLs.
#==========================================================
# Define the function to build the DPSF Solution in Release mode.
def BuildDPSFSolution()
	# msbuild "#{dpsfSolutionFilePath}" /p:Configuration=Release /p:Platform="Mixed Platforms"
	puts 'Build Solution not implemented.'
	
	msbuild :build do |msb|
		msb.path_to_command = msBuildPath
		msb.properties :configuration => :Release
		msb.targets :Build
		msb.solution = dpsfSolutionFilePath
	end
end

# Delete the existing DLL files before creating the new ones.
puts 'Deleting existing DLLs.'
FileUtils.rm(newDLLFiles, :force => true)	# Use Force to ignore errors if the files don't exist.

# Build the solution to get the regular DLL files.
puts 'Build the DPSF solution with the regular assemblies.'
BuildDPSFSolution()


#==========================================================
# Then update the necessary files so we can build the AsDrawableGameComponent DLLs.
#==========================================================
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
		puts "ERROR: Invalid .csproj file name given. Cannot find the file #{csprojFilePath}"
	end	

	# Write the new file contents to the file.
	File.open(csprojFilePath, 'w') do |f|
		f.puts fileContents
	end
end

# Update the .csproj files to build with AsDrawableGameComponent
puts 'Updating the .csproj files to build AsDrawableGameComponent DLLs.'
csprojFilePaths.each do |csprojFilePath|
	UpdateCsprojFileToAsDrawableGameComponent(csprojFilePath)
end


#==========================================================
# Build the solution in Release mode to create the AsDrawableGameComponent DLLs.
#==========================================================
puts 'Building the AsDrawableGameComponent DLLs.'
BuildDPSFSolution()

#==========================================================
# Revert the .csproj files back to their original states now that we have the DLLs.
#==========================================================
# Define the function to revert a .csproj file back from its .backup file.
def RevertCsprojFile(csprojFilePath)
	# Copy the backup back overtop of the original to revert it, and then delete the backup file.
	FileUtils.copy_file(csprojFilePath + '.backup', csprojFilePath)
	File.delete(csprojFilePath + '.backup')
end

# Update the .csproj files to build with AsDrawableGameComponent
puts 'Reverting .csproj files back to their original states.'
csprojFilePaths.each do |csprojFilePath|
	RevertCsprojFile(csprojFilePath)
end


#==========================================================
# Copy the DLL files to the Installer directory
#==========================================================
puts 'Copying new DLL files to the Installer Files directory.'
FileUtils.cp(newDLLFiles, installerFilesDirectory)

puts 'Done!'