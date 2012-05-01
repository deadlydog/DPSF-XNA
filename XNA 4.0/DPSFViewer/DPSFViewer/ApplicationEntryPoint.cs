using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DPSFViewer
{
	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	static class ApplicationEntryPoint
	{
		/// <summary>
		/// The application entry point when this project is set as the Startup Project.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			using (var game = new DPSFViewer())
			{
				BasicVirtualEnvironment.ApplicationEntryPoint.RunGame(game);
			}
		}
	}
}
