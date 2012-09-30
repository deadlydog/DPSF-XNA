using System;
using BasicVirtualEnvironment.Input;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
#if (WINDOWS)
	[Serializable]
#endif
	class SphereDPSFDemoParticleSystemWrapper : SphereParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public SphereDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    { }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Decrease Radius:", new Vector2(5, 250), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "X", new Vector2(165, 250), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Increase Radius:", new Vector2(5, 275), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "C", new Vector2(155, 275), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Same Direction:", new Vector2(5, 300), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "V", new Vector2(150, 300), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Random Direction:", new Vector2(5, 325), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "B", new Vector2(170, 325), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Less Particles:", new Vector2(5, 350), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "N", new Vector2(140, 350), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "More Particles:", new Vector2(5, 375), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "M", new Vector2(140, 375), draw.ControlTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyIsDown(Keys.X, 0.02f))
            {
                this.ChangeSphereRadius(-5);
            }

            if (KeyboardManager.KeyIsDown(Keys.C, 0.02f))
            {
                this.ChangeSphereRadius(5);
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.V))
            {
                this.MakeParticlesTravelInTheSameDirection();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.B))
            {
                this.MakeParticlesTravelInRandomDirections();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.N))
            {
                this.ChangeNumberOfParticles(-50);
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.M))
            {
                this.ChangeNumberOfParticles(50);
            }
	    }
	}
}