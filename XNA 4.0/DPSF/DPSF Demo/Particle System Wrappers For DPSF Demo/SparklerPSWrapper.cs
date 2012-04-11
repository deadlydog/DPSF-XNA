using BasicVirtualEnvironment.Input;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class SparklerDPSFDemoParticleSystemWrapper : SparklerParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public SparklerDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    { }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Simple:", new Vector2(5, 250), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "X", new Vector2(70, 250), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Complex:", new Vector2(5, 275), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "C", new Vector2(90, 275), draw.ControlTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.LoadSimpleParticleSystem();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.LoadComplexParticleSystem();
            }
	    }
	}
}