using System;
using BasicVirtualEnvironment.Input;
using DPSF;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
#if (WINDOWS)
	[Serializable]
#endif
	class RandomDPSFDemoParticleSystemWrapper : RandomParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public RandomDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Particle Size:", new Vector2(draw.TextSafeArea.Left + 300, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, this.InitialProperties.StartSizeMin.ToString(), new Vector2(draw.TextSafeArea.Left + 425, draw.TextSafeArea.Top + 2), draw.ValueTextColor);
	    }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Random Pattern:", new Vector2(5, 250), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "X", new Vector2(160, 250), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Spiral Pattern:", new Vector2(5, 275), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "C", new Vector2(140, 275), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Increase Size:", new Vector2(5, 300), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "V", new Vector2(135, 300), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Decrease Size:", new Vector2(5, 325), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "B", new Vector2(145, 325), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Change Start Color:", new Vector2(5, 350), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "N", new Vector2(190, 350), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Change End Color:", new Vector2(5, 375), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "M", new Vector2(180, 375), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Tube Mode:", new Vector2(5, 400), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "P", new Vector2(115, 400), draw.ControlTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.LoadRandomEvents();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.LoadRandomSpiralEvents();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.V))
            {
                this.InitialProperties.StartSizeMin += 2.0f;

                if (this.InitialProperties.StartSizeMin > 100.0f)
                {
                    this.InitialProperties.StartSizeMin = 100.0f;
                }

                this.InitialProperties.StartSizeMax =
                    this.InitialProperties.EndSizeMin =
                    this.InitialProperties.EndSizeMax =
                    this.InitialProperties.StartSizeMin;
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.B))
            {
                this.InitialProperties.StartSizeMin -= 2.0f;

                if (this.InitialProperties.StartSizeMin < 2.0f)
                {
                    this.InitialProperties.StartSizeMin = 2.0f;
                }

                this.InitialProperties.StartSizeMax =
                    this.InitialProperties.EndSizeMin =
                    this.InitialProperties.EndSizeMax =
                    this.InitialProperties.StartSizeMin;
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.N))
            {
                this.InitialProperties.StartColorMin =
                    this.InitialProperties.StartColorMax =
                    DPSFHelper.LerpColor(Color.Black, Color.White, (float)RandomNumber.NextDouble(), (float)RandomNumber.NextDouble(), (float)RandomNumber.NextDouble(), (float)RandomNumber.NextDouble());
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.M))
            {
                this.InitialProperties.EndColorMin =
                    this.InitialProperties.EndColorMax =
                    DPSFHelper.LerpColor(Color.Black, Color.White, (float)RandomNumber.NextDouble(), (float)RandomNumber.NextDouble(), (float)RandomNumber.NextDouble(), (float)RandomNumber.NextDouble());
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.P))
            {
                this.LoadTubeEvents();
            }
	    }
	}
}
