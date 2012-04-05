using BasicVirtualEnvironment.Input;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class QuadSprayDPSFDemoParticleSystemWrapper : QuadSprayParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public QuadSprayDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

		public void DrawStatusText(DrawTextRequirements draw)
		{ }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Spray:", new Vector2(5, 250), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "X", new Vector2(65, 250), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Wall:", new Vector2(5, 275), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "C", new Vector2(55, 275), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Toggle Gravity:", new Vector2(5, 300), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "V", new Vector2(150, 300), draw.PropertyTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.LoadSprayEvents();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.LoadWallEvents();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.V))
            {
                this.ToggleGravity();
            }
	    }
	}
}