using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class TrailDPSFDemoParticleSystemWrapper : TrailParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public TrailDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    {
			// Normally we would put this in the "DrawInputControlsText" function, but we want this text to always display for this particle system so we put it here.
            draw.TextWriter.DrawString(draw.Font, "Move the emitter with (Shift):", new Vector2(draw.TextSafeArea.Left + 5, draw.TextSafeArea.Top + 250), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "W/A/S/D", new Vector2(draw.TextSafeArea.Left + 285, draw.TextSafeArea.Top + 250), draw.ControlTextColor);
            draw.TextWriter.DrawString(draw.Font, "Change textures with (Shift):", new Vector2(draw.TextSafeArea.Left + 5, draw.TextSafeArea.Top + 275), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "T", new Vector2(draw.TextSafeArea.Left + 275, draw.TextSafeArea.Top + 275), draw.ControlTextColor);
	    }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    { }

	    public void ProcessInput()
	    { }
	}
}