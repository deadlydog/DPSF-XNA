using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DPSF.ParticleSystems;
using DPSF_Demo.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class FountainDPSFDemoParticleSystemWrapper : FountainParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public FountainDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Bounciness:", new Vector2(draw.TextSafeArea.Left + 300, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, this.mfBounciness.ToString("0.00"), new Vector2(draw.TextSafeArea.Left + 410, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
	    }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Floor Collision On:", new Vector2(5, 250), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "X", new Vector2(170, 250), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Floor Collision Off:", new Vector2(5, 275), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "C", new Vector2(180, 275), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Decrease Bounciness:", new Vector2(5, 300), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "V", new Vector2(205, 300), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Increase Bounciness:", new Vector2(5, 325), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "B", new Vector2(195, 325), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Shrinking On:", new Vector2(5, 350), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "N", new Vector2(130, 350), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Shrinking Off:", new Vector2(5, 375), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "M", new Vector2(135, 375), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Toggle Additive Blending:", new Vector2(5, 400), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "P", new Vector2(240, 400), draw.PropertyTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.MakeParticlesBounceOffFloor();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.MakeParticlesNotBounceOffFloor();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.V))
            {
                this.mfBounciness -= 0.05f;

                if (this.mfBounciness < 0.0f)
                {
                    this.mfBounciness = 0.0f;
                }
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.B))
            {
                this.mfBounciness += 0.05f;

                if (this.mfBounciness > 2.0f)
                {
                    this.mfBounciness = 2.0f;
                }
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.N))
            {
                this.MakeParticlesShrink();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.M))
            {
                this.MakeParticlesNotShrink();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.P))
            {
                this.ToggleAdditiveBlending();
            }
	    }
	}
}