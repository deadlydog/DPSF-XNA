using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	/// <summary>
	/// This interface defines the functions that need to be provided in order to display a
	/// particle system in the DPSF Demo.
	/// </summary>
	public interface IWrapParticleSystemsForDPSFDemo
	{
		/// <summary>
		/// Perform any DPSF Demo specific setup after being Auto Initialized.
		/// </summary>
		void AfterAutoInitialize();

		/// <summary>
		/// Perform any DPSF Demo specific work before drawing text.
		/// </summary>
		void BeforeDrawText();

		/// <summary>
		/// Draws any DPSF Demo specific text to the screen.
		/// </summary>
		void DrawText();

		/// <summary>
		/// Processes any input specific to this particle system.
		/// </summary>
		void ProcessInput();
	}
}
