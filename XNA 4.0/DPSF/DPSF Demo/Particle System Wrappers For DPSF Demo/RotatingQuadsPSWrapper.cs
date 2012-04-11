using BasicVirtualEnvironment.Input;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class RotatingQuadsDPSFDemoParticleSystemWrapper : RotatingQuadsParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public RotatingQuadsDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

		public void DrawStatusText(DrawTextRequirements draw)
		{ }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Normal:", new Vector2(5, 250), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "X", new Vector2(75, 250), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Billboards:", new Vector2(5, 275), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "C", new Vector2(100, 275), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Ball:", new Vector2(5, 300), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "V", new Vector2(45, 300), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Number of Particles:", new Vector2(5, 325), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "B", new Vector2(195, 325), draw.ControlTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.MakeParticlesFaceWhateverDirection();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.MakeParticlesFaceTheCamera();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.V))
            {
                this.MakeParticlesFaceTheCenter();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.B))
            {
                this.RemoveAllParticles();
                int iNumberOfParticles = this.MaxNumberOfParticlesAllowed;

                switch (iNumberOfParticles)
                {
                    default:
                    case 1:
                        this.MaxNumberOfParticlesAllowed = 10;
                        break;

                    case 10:
                        this.MaxNumberOfParticlesAllowed = 100;
                        break;

                    case 100:
                        this.MaxNumberOfParticlesAllowed = 500;
                        break;

                    case 500:
                        this.MaxNumberOfParticlesAllowed = 1000;
                        break;

                    case 1000:
                        this.MaxNumberOfParticlesAllowed = 1;
                        break;
                }
            }
	    }
	}
}