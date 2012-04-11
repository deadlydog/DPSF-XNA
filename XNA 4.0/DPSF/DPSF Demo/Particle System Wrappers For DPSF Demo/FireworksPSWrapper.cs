using BasicVirtualEnvironment.Input;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class FireworksDPSFDemoParticleSystemWrapper : FireworksParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public FireworksDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    { }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Common Origin:", new Vector2(5, 250), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "X", new Vector2(150, 250), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Spread Out Origin:", new Vector2(5, 275), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "C", new Vector2(180, 275), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Explosions On:", new Vector2(5, 300), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "V", new Vector2(140, 300), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Explosions Off:", new Vector2(5, 325), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "B", new Vector2(145, 325), draw.ControlTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.InitialProperties.PositionMin = new Vector3();
                this.InitialProperties.PositionMax = new Vector3();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.InitialProperties.PositionMin = new Vector3(-100, 0, 0);
                this.InitialProperties.PositionMax = new Vector3(100, 0, 0);
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.V))
            {
                this.TurnExplosionsOn();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.B))
            {
                this.TurnExplosionsOff();
            }
	    }
	}
}