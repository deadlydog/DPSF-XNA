using System;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using BasicVirtualEnvironment.Input;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
#if (WINDOWS)
	[Serializable]
#endif
	class TrailDPSFDemoParticleSystemWrapper : TrailParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public TrailDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    {
			draw.TextWriter.DrawString(draw.Font, "Frequency:", new Vector2(draw.TextSafeArea.Left + 300, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, this.NumberOfParticlesToEmitScale.ToString("0.00"), new Vector2(draw.TextSafeArea.Left + 410, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);

			// Normally we would put this in the "DrawInputControlsText" function, but we want this text to always display for this particle system so we put it here.
            draw.TextWriter.DrawString(draw.Font, "Move the emitter with (Shift):", new Vector2(draw.TextSafeArea.Left + 5, draw.TextSafeArea.Top + 250), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "W/A/S/D", new Vector2(draw.TextSafeArea.Left + 285, draw.TextSafeArea.Top + 250), draw.ControlTextColor);
            draw.TextWriter.DrawString(draw.Font, "Change textures with (Shift):", new Vector2(draw.TextSafeArea.Left + 5, draw.TextSafeArea.Top + 275), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "T", new Vector2(draw.TextSafeArea.Left + 275, draw.TextSafeArea.Top + 275), draw.ControlTextColor);
			
			draw.TextWriter.DrawString(draw.Font, "Grow, No Spin:", new Vector2(draw.TextSafeArea.Left + 5, draw.TextSafeArea.Top + 300), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "X", new Vector2(draw.TextSafeArea.Left + 155, draw.TextSafeArea.Top + 300), draw.ControlTextColor);
			draw.TextWriter.DrawString(draw.Font, "Shrink, Spin:", new Vector2(draw.TextSafeArea.Left + 5, draw.TextSafeArea.Top + 325), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "C", new Vector2(draw.TextSafeArea.Left + 125, draw.TextSafeArea.Top + 325), draw.ControlTextColor);

			draw.TextWriter.DrawString(draw.Font, "Less Frequent Particles:", new Vector2(5, 350), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "V", new Vector2(draw.TextSafeArea.Left + 245, draw.TextSafeArea.Top + 350), draw.ControlTextColor);
			draw.TextWriter.DrawString(draw.Font, "More Frequent Particles:", new Vector2(5, 375), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "B", new Vector2(draw.TextSafeArea.Left + 245, draw.TextSafeArea.Top + 375), draw.ControlTextColor);
	    }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    { }

	    public void ProcessInput()
	    {
			if (KeyboardManager.KeyWasJustPressed(Keys.X))
			{
				this.LoadParticleSystem();
			}

			if (KeyboardManager.KeyWasJustPressed(Keys.C))
			{
				this.LoadSpinningTrailParticleSystem();
			}

			if (KeyboardManager.KeyWasJustPressed(Keys.V))
				this.NumberOfParticlesToEmitScale += 0.1f;

			if (KeyboardManager.KeyWasJustPressed(Keys.B))
				this.NumberOfParticlesToEmitScale -= 0.1f;
	    }
	}
}