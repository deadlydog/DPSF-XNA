using BasicVirtualEnvironment.Input;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
#if (WINDOWS)
    [Serializable]
#endif
	class FireSpriteDPSFDemoParticleSystemWrapper : FireSpriteParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public FireSpriteDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Smokiness:", new Vector2(draw.TextSafeArea.Left + 300, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, this.GetAmountOfSmokeBeingReleased().ToString("0.00"), new Vector2(draw.TextSafeArea.Left + 410, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
	    }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Vertical Ring:", new Vector2(5, 250), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "X", new Vector2(130, 250), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Horizontal Ring:", new Vector2(5, 275), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "C", new Vector2(150, 275), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Decrease Smoke:", new Vector2(5, 300), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "V", new Vector2(170, 300), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Increase Smoke:", new Vector2(5, 325), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "B", new Vector2(160, 325), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Toggle Additive Blending:", new Vector2(5, 350), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "N", new Vector2(240, 350), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Toggle Ring Movement:", new Vector2(5, 375), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "M", new Vector2(225, 375), draw.ControlTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.ParticleInitializationFunction = this.InitializeParticleFireOnVerticalRing;
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.ParticleInitializationFunction = this.InitializeParticleFireOnHorizontalRing;
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.V))
            {
                float fAmount = this.GetAmountOfSmokeBeingReleased();
                if (fAmount > 0.0f)
                {
                    this.SetAmountOfSmokeToRelease(fAmount - 0.05f);
                }
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.B))
            {
                float fAmount = this.GetAmountOfSmokeBeingReleased();
                if (fAmount < 1.0f)
                {
                    this.SetAmountOfSmokeToRelease(fAmount + 0.05f);
                }
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.N))
            {
                this.ToggleAdditiveBlending();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.M))
            {
                if (this.Emitter.PositionData.Velocity == Vector3.Zero)
                {
                    this.Emitter.PositionData.Velocity = new Vector3(30, 0, 0);
                }
                else
                {
                    this.Emitter.PositionData.Velocity = Vector3.Zero;
                }
            }
	    }
	}
}