namespace DPSF_Demo
{
	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	static class ApplicationEntryPoint
	{
		/// <summary>
		/// The application entry point when this project is set as the Startup Project.
		/// </summary>
		private static void Main()
		{
			using (var game = new DPSFDemo())
			{
				BasicVirtualEnvironment.ApplicationEntryPoint.RunGame(game);
			}
		}
	}
}