using BasicVirtualEnvironment.Input;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class MagnetsDPSFDemoParticleSystemWrapper : MagnetsParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public MagnetsDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Magnets Affect:", new Vector2(draw.TextSafeArea.Left + 260, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, this.mbMagnetsAffectPosition ? "Position" : "Velocity", new Vector2(draw.TextSafeArea.Left + 410, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
	    }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Emitter Magnet:", new Vector2(5, 250), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "X", new Vector2(150, 250), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Multiple Magnets:", new Vector2(5, 275), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "C", new Vector2(160, 275), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Toggle Affecting Position vs Velocity:", new Vector2(5, 300), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "V", new Vector2(355, 300), draw.PropertyTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.LoadEmitterMagnetParticleSystem();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.LoadSeparateEmitterMagnetsParticleSystem();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.V))
            {
                this.ToggleMagnetsAffectingPositionVsVelocity();
            }
	    }
	}
}