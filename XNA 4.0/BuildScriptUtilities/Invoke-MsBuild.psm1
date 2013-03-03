function Invoke-MsBuild
{
	<#
    .SYNOPSIS
    Executes the MSBuild.exe tool against the specified Visual Studio solution or project file.

    .DESCRIPTION
	Builds the given Visual Studio solution or project file using MSBuild.

    .PARAMETER Path
    The path of the Visual Studio solution (.sln) or project to build.
	
	.PARAMETER Target
    The targets for MsBuild (i.e. the actions it should perform), such as "Build", "Clean", "Deploy", etc. Default is "Clean;Build".

    .PARAMETER Configuration
    The project configuration to build within the solution file (e.g. "Debug", "Release", etc.). Default is "Debug".

	.PARAMETER BuildVerbosity
	The verbosity that the build should use. Valid values include: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]. Default is minimal.

	.PARAMETER AdditionalParameters
	Additional parameters to pass to the MsBuild command-line tool. This can be any valid MsBuild command-line parameters.
	http://msdn.microsoft.com/en-ca/library/vstudio/ms164311.aspx

	.PARAMETER $BuildLogDirectoryPath
    The directory path to write the build log file to. Defaults to putting the log file beside the .sln or project file being built if not provided.

    .PARAMETER AutoLaunchBuildLog
    If set, this switch will cause the build log to automatically be launched into the default viewer if the build fails.
	
	.PARAMETER KeepBuildLogOnSuccessfulBuilds
	If set, this switch will cause the msbuild log file to not be deleted on successful builds; normally it is only kept around on failed builds.
	
	.PARAMETER ShowBuildWindow
	If set, this switch will cause a command prompt window to be shown in order to view the progress of the build.
	
	.PARAMETER PromptForInputBeforeClosingBuildWindow
	If set, the command prompt window used to show the build progress will remain open until the user presses a key on it.
	NOTE: This switch only has an effect if the ShowBuildWindow switch is also used.
	
	.PARAMETER PassThru
	If set, this switch will cause the script not to wait until the build (launched in another process) completes before continuing execution.
	Instead the build will be started in a new process and that process will immediately be returned, allowing the calling script to continue 
	execution while the build is performed.

    .EXAMPLE
	Invoke-MsBuild -Path "C:\Some Folder\MySolution.sln"
	
	.EXAMPLE
	Invoke-MsBuild -Path "C:\Some Folder\MyProject.csproj" -Target Build -ShowBuildWindow
	
	.EXAMPLE
	Invoke-MsBuild -Path "C:\MySolution.sln" -Configuration "Release" -BuildLogDirectoryPath "C:\BuildLogs" -KeepBuildLogOnSuccessfulBuilds -BuildVerbosity Detailed -AdditionalParameters "/maxcpucount /p:BuildInParallel=true /nologo"
	
	.EXAMPLE
	Invoke-MsBuild -Path "C:\Database\Database.dbproj" -Target Deploy -AdditionalParameters "/property:TargetDatabase=MyDatabase /property:TargetConnectionString=`"Data Source=DatabaseServerName`;Integrated Security=True`;Pooling=False`" /property:DeployToDatabase=True"

    .NOTES
    Name:   Invoke-MsBuild
    Author: Daniel Schroeder (originally based on the module at http://geekswithblogs.net/dwdii/archive/2011/05/27/part-2-automating-a-visual-studio-build-with-powershell.aspx)
#>
# Maybe make a separate Invoke-MsBuildAsync module since a lot of the switches won't apply if -Wait is specified; want a couple
# 	different switches for async, like CreateLogFiles.

# Have a switch to just return the full Log File Path. So it doesn't actually do a build, it just returns what the Log file path will be when it does do the build.

# Google Parameter Sets for specifying the different sets of parameters that should be used together.

# I think with ValueFromPipeline=$true I need to use the Begin Process End blocks.

	[CmdletBinding(DefaultParameterSetName="Wait")]
	param
	(
		[parameter(Position=0,Mandatory=$true,ValueFromPipeline=$true)]
		[ValidateScript({Test-Path $_})]
		[String] $Path,

		[parameter(Mandatory=$false)]
		[ValidateNotNullOrEmpty()]
		[String] $Target = 'Clean;Build',

		[parameter(Mandatory=$false)]
		[ValidateNotNullOrEmpty()]
		[String] $Configuration = 'Debug',

		[parameter(Mandatory=$false)]
		[ValidateSet('q','quiet','m','minimal','n','normal','d','detailed','diag','diagnostic')]
		[Alias("V")]
		[String] $BuildVerbosity = 'minimal',

		[parameter(Mandatory=$false)]
		[Alias("Params")]
		[String] $AdditionalParameters,

		[parameter(Mandatory=$false)]
		[Alias("L")]
		[string] $BuildLogDirectoryPath, # = $env:Temp

		[parameter(Mandatory=$false,ParameterSetName="Wait")]
		[ValidateNotNullOrEmpty()]
		[Switch] $AutoLaunchBuildLogOnFailure,

		[parameter(Mandatory=$false,ParameterSetName="Wait")]
		[ValidateNotNullOrEmpty()]
		[Switch] $KeepBuildLogOnSuccessfulBuilds,

		[parameter(Mandatory=$false)]
		[Switch] $ShowBuildWindow,
		
		[parameter(Mandatory=$false)]
		[Switch] $PromptForInputBeforeClosingBuildWindow,
		
		[parameter(Mandatory=$false,ParameterSetName="PassThru")]
		[Switch] $PassThru
	)

	# Turn on Strict Mode to help catch syntax-related errors.
	# 	This must come after a script's/function's param section.
	# 	Forces a function to be the first non-comment code to appear in a PowerShell Script/Module.
	Set-StrictMode -Version Latest

	# If no Build Log Directory was supplied, default to placing the log in the same folder as the solution being built.
	if ([string]::IsNullOrWhiteSpace($BuildLogDirectoryPath))
	{
		$BuildLogDirectoryPath = [System.IO.Path]::GetDirectoryName($Path)
	}

	# Get some environmental paths.
	$vs2010CommandPrompt = $env:VS100COMNTOOLS + "vcvarsall.bat"
	$vs2012CommandPrompt = $env:VS110COMNTOOLS + "VsDevCmd.bat"

	# Store the VS Command Prompt to do the build in, if one exists.
	$vsCommandPrompt = Get-VisualStudioCommandPromptPath

	# Local Variables.
	$solutionFileName = (Get-ItemProperty -Path $Path).Name
	$buildLogFilePath = (Join-Path $BuildLogDirectoryPath $solutionFileName) + ".msbuild.$Configuration.log"
	$windowStyle = if ($ShowBuildWindow) { "Normal" } else { "Hidden" }
	$buildCrashed = $false;

	# Try and build the solution.
	try
	{
		# Build the arguments to pass to MsBuild.
		$buildArguments = """$Path"" /target:$Target /property:Configuration=$Configuration /verbosity:$BuildVerbosity $AdditionalParameters /fileLoggerParameters:LogFile=""$buildLogFilePath"""

		# If a VS Command Prompt was found, build in that since it sets environmental variables that may be needed to build some projects.
		if ($vsCommandPrompt -ne $null)
		{
			# Create the arguments to pass into cmd.exe in order to call MsBuild from the VS Command Prompt.
			$pauseForInput = if ($PromptForInputBeforeClosingBuildWindow) { "Pause & " } else { "" }
			$cmdArgumentsToRunMsBuildInVsCommandPrompt = "/k "" ""$vsCommandPrompt"" & msbuild $buildArguments & $pauseForInput Exit"" "

			Write-Debug "Starting new cmd.exe process with arguments ""$cmdArgumentsToRunMsBuildInVsCommandPrompt""."

			# Perform the build.
			if ($PassThru)
			{
				return Start-Process cmd.exe -ArgumentList $cmdArgumentsToRunMsBuildInVsCommandPrompt -WindowStyle $windowStyle -PassThru
			}
			else
			{
				Start-Process cmd.exe -ArgumentList $cmdArgumentsToRunMsBuildInVsCommandPrompt -WindowStyle $windowStyle -Wait
			}
		}
		# Else let's just build using MSBuild directly.
		else
		{
			# Get the path to the MsBuild executable.
			$msBuildPath = Get-MsBuildPath
			
			Write-Debug "Starting new MsBuild.exe process with arguments ""$buildArguments""."
			
			# Perform the build.
			if ($PassThru)
			{
				return Start-Process -FilePath "$msBuildPath" -ArgumentList "$buildArguments" $waitForProcessToExit -WindowStyle $windowStyle -PassThru
			}
			else
			{
				Start-Process -FilePath "$msBuildPath" -ArgumentList "$buildArguments" $waitForProcessToExit -WindowStyle $windowStyle -Wait
			}
		}
	}
	catch
	{
		$buildCrashed = $true;
		$errorMessage = $_
		Write-Error ("Unexpect error occured while building ""$Path"": $errorMessage" );
	}

	# If the build didn't crash.
	if (!$buildCrashed)
	{
		# Get if the build failed or not by looking at the log file.
		$buildFailed = Select-String -Path $buildLogFilePath -Pattern "Build FAILED." -SimpleMatch

		# If the build succeeded.
		if($buildFailed -eq $null)
		{
			# If we shouldn't keep the log around, delete it.
			if (!$KeepBuildLogOnSuccessfulBuilds)
			{
				Remove-Item -Path $buildLogFilePath
			}
		}
		# Else at least one of the projects failed to build.
		else
		{
			# Write the error message as a warning.
			Write-Warning "FAILED to build ""$Path"". Please check the build log ""$buildLogFilePath"" for details." 

			# If we should show the build log automatically, open it with the default viewer.
			if($AutoLaunchBuildLogOnFailure)
			{
				Start-Process -verb "Open" $buildLogFilePath;
			}
		}
	}
}

function Get-VisualStudioCommandPromptPath
{
 <#
	.SYNOPSIS
		Gets the file path to the latest Visual Studio Command Prompt. Returns $null if a path is not found.
	
	.DESCRIPTION
		Gets the file path to the latest Visual Studio Command Prompt. Returns $null if a path is not found.
	#>

# Get some environmental paths.
$vs2010CommandPrompt = $env:VS100COMNTOOLS + "vcvarsall.bat"
$vs2012CommandPrompt = $env:VS110COMNTOOLS + "VsDevCmd.bat"

# Store the VS Command Prompt to do the build in, if one exists.
$vsCommandPrompt = $null
if (Test-Path $vs2012CommandPrompt)
{
	$vsCommandPrompt = $vs2012CommandPrompt
}
elseif (Test-Path $vs2010CommandPrompt)
{
	$vsCommandPrompt = $vs2010CommandPrompt
}

# Return the path to the VS Command Prompt if it was found.
return $vsCommandPrompt
}

function Get-MsBuildPath
{
 <#
	.SYNOPSIS
		Gets the path to the latest version of MsBuild.exe. Returns $null if a path is not found.
	
	.DESCRIPTION
		Gets the path to the latest version of MsBuild.exe. Returns $null if a path is not found.
	#>

# Array of valid MsBuild versions
$Versions = @("4.0", "3.5", "2.0")

# Loop through each version from largest to smallest
foreach ($Version in $Versions) 
{
	# Try to find an instance of that particular version in the registry
	$RegKey = "HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\${Version}"
	$ItemProperty = Get-ItemProperty $RegKey -ErrorAction SilentlyContinue

	# If registry entry exsists, then get the msbuild path and retrun 
	if ($ItemProperty -ne $null)
	{
		return Join-Path $ItemProperty.MSBuildToolsPath -ChildPath MsBuild.exe
	}
} 

# Return that we were not able to find MsBuild.exe.
return $null
}
Export-ModuleMember -Function Invoke-MsBuild