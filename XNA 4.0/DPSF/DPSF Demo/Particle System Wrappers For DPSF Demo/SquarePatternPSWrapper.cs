using BasicVirtualEnvironment.Input;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class SquarePatternDPSFDemoParticleSystemWrapper : SquarePatternParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public SquarePatternDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    { }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Square Pattern:", new Vector2(5, 250), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "X", new Vector2(150, 250), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Multiple Color Changes:", new Vector2(5, 275), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "C", new Vector2(220, 275), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Change Color:", new Vector2(5, 300), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "V", new Vector2(140, 300), draw.PropertyTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.LoadSquarePatternEvents();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.LoadChangeColorEvents();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.V))
            {
                this.ChangeParticlesToRandomColors();
            }
	    }
	}
}