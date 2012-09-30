
namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	/// <summary>
	/// This interface defines the extra properties and functions that should be present for a particle system in the DPSF Demo.
	/// </summary>
	public interface IWrapDPSFDemoParticleSystems : DPSF.IDPSFParticleSystem
	{
		/// <summary>
		/// Gets the DPSF Demo Friendly name of the particle system.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Perform any DPSF Demo specific setup after being Auto Initialized.
		/// </summary>
		void AfterAutoInitialize();

	    /// <summary>
	    /// Draw any text describing the status of the particles system.
	    /// </summary>
	    /// <param name="draw"> </param>
	    void DrawStatusText(DrawTextRequirements draw);

	    /// <summary>
	    /// Draw any text describing input that may be used to control the particle system.
	    /// </summary>
	    /// <param name="draw"> </param>
	    void DrawInputControlsText(DrawTextRequirements draw);

		/// <summary>
		/// Process any user input used to control the particle system.
		/// </summary>
		void ProcessInput();
	}
}
