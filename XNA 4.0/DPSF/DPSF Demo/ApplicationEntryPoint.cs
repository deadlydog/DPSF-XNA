using System;

namespace DPSF_Demo
{
	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	static class ApplicationEntryPoint
	{
		static void Main()
		{
			using (DemoBase game = new DPSFDemo())
			{
#if (WINDOWS)
				// String to hold any prerequisites error messages.
				string prerequisitesErrorMessage = string.Empty;

				// If XNA 4.0 is not installed.
				using (Microsoft.Win32.RegistryKey xnaKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\XNA\\Framework\\v4.0"))
				{
					if (xnaKey == null || (int)xnaKey.GetValue("Installed") != 1)
					{
						// Store the error message.
						prerequisitesErrorMessage += "XNA 4.0 must be installed to run this program. You can download the XNA 4.0 Redistributable from http://www.microsoft.com/downloads/ \n\n";
					}
				}

				// If .NET 4 is not installed.
				using (Microsoft.Win32.RegistryKey netKey4 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Client"))
				{
					bool net4NotInstalled = (netKey4 == null || (int)netKey4.GetValue("Install") != 1);
					if (net4NotInstalled)
					{
						// Store the error message.
						prerequisitesErrorMessage += "The .NET Framework 4.0 or greater must be installed to run this program. You can download the .NET Framework from http://www.microsoft.com/downloads/ \n\n";
					}
				}

				// If not all of the prerequisites are installed.
				if (!string.IsNullOrEmpty(prerequisitesErrorMessage))
				{
					// Add to the error message the option of trying to run the program anyways.
					prerequisitesErrorMessage += "Do you want to try and run the program anyways, even though not all of the prerequisites are installed?";

					// Display the error message and exit.
					if (System.Windows.Forms.MessageBox.Show(prerequisitesErrorMessage, "Prerequisites Not Installed", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
					{
						return;
					}
				}

				// If we are in Release Mode.
				if (DemoBase.RELEASE_MODE)
				{
					try
					{
						game.Run();	// Start the game.
					}
					catch (Exception e)
					{
						// Display any error messages that occur in a nice message box.
						System.Windows.Forms.MessageBox.Show(e.ToString(), "Unhandled Exception");
					}
				}
				// Else we are in debug mode, so allow Visual Studio to show us the error message.
				else
				{
					game.Run();
				}
#else
				Game.Run();
#endif
			}
		}
	}
}