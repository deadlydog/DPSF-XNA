using BasicVirtualEnvironment.Input;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class ExplosionShockwaveDPSFDemoParticleSystemWrapper : ExplosionShockwaveParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public ExplosionShockwaveDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

	    public void AfterAutoInitialize()
	    {
            SetupToAutoExplodeEveryInterval(2);
	    }

	    public void DrawStatusText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Size:", new Vector2(draw.TextSafeArea.Left + 450, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, this.ShockwaveSize.ToString(), new Vector2(draw.TextSafeArea.Left + 495, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
	    }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Change Color:", new Vector2(5, 300), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "V", new Vector2(135, 300), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Decrease Particle Size:", new Vector2(5, 325), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "B", new Vector2(220, 325), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Increase Particle Size:", new Vector2(5, 350), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "N", new Vector2(210, 350), draw.PropertyTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.V))
            {
                this.ChangeExplosionColor();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.B))
            {
                this.ShockwaveSize -= 5;
                this.ShockwaveSize = (this.ShockwaveSize < 100 ? 100 : this.ShockwaveSize);
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.N))
            {
                this.ShockwaveSize += 5;
                this.ShockwaveSize = (this.ShockwaveSize > 400 ? 400 : this.ShockwaveSize);
            }
	    }
	}
}