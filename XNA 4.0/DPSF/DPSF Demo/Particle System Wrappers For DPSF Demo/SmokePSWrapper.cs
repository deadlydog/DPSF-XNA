using BasicVirtualEnvironment.Input;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class SmokeDPSFDemoParticleSystemWrapper : SmokeParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public SmokeDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

		public void DrawStatusText(DrawTextRequirements draw)
		{ }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Rising Smoke Cloud:", new Vector2(5, 250), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "X", new Vector2(195, 250), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Dispersed Smoke:", new Vector2(5, 275), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "C", new Vector2(175, 275), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Suction Orb:", new Vector2(5, 300), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "V", new Vector2(120, 300), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Repel Orb:", new Vector2(5, 325), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "B", new Vector2(105, 325), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Change Color:", new Vector2(5, 350), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "N", new Vector2(135, 350), draw.PropertyTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.LoadSmokeEvents();
                this.ParticleInitializationFunction = this.InitializeParticleRisingSmoke;
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.LoadSmokeEvents();
                this.ParticleInitializationFunction = this.InitializeParticleFoggySmoke;
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.N))
            {
                this.ChangeColor();
            }

			// V and B keys are processed in GameMain.cs still since they make changes to objects external to the particle system.
	    }
	}
}