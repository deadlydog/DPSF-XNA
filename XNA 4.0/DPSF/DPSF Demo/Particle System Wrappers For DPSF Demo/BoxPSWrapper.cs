using BasicVirtualEnvironment.Input;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class BoxDPSFDemoParticleSystemWrapper : BoxParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public BoxDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    { }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Box:", new Vector2(5, 250), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "X", new Vector2(50, 250), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Bars:", new Vector2(5, 275), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "C", new Vector2(50, 275), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Toggle Change Colors:", new Vector2(5, 300), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "V", new Vector2(210, 300), draw.PropertyTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.LoadPartiallyTranparentBox();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.LoadOpaqueBoxBars();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.V))
            {
                this.ToggleColorChanges();
            }
	    }
	}
}