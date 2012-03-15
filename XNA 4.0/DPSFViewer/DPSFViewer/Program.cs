using System;

namespace DPSFViewer
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			using (Viewer game = new Viewer())
			{
				game.Run();
			}
		}
	}
}

