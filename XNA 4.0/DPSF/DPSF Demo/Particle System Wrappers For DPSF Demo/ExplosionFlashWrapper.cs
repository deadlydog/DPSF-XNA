﻿using System;
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
	class ExplosionFlashDPSFDemoParticleSystemWrapper : ExplosionFlashParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public ExplosionFlashDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

	    public void AfterAutoInitialize()
	    {
            SetupToAutoExplodeEveryInterval(1);
	    }

	    public void DrawStatusText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Intensity:", new Vector2(draw.TextSafeArea.Left + 330, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, this.ExplosionIntensity.ToString(), new Vector2(draw.TextSafeArea.Left + 410, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Size:", new Vector2(draw.TextSafeArea.Left + 450, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, this.ExplosionParticleSize.ToString(), new Vector2(draw.TextSafeArea.Left + 495, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
	    }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Decrease Intensity:", new Vector2(5, 250), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "X", new Vector2(180, 250), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Increase Intensity:", new Vector2(5, 275), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "C", new Vector2(170, 275), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Change Color:", new Vector2(5, 300), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "V", new Vector2(135, 300), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Decrease Particle Size:", new Vector2(5, 325), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "B", new Vector2(220, 325), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Increase Particle Size:", new Vector2(5, 350), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "N", new Vector2(210, 350), draw.PropertyTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.ExplosionIntensity -= 5;
                this.ExplosionIntensity = (this.ExplosionIntensity < 1 ? 1 : this.ExplosionIntensity);
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.ExplosionIntensity += 5;
                this.ExplosionIntensity = (this.ExplosionIntensity > 100 ? 100 : this.ExplosionIntensity);
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.V))
            {
                this.ChangeExplosionColor();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.B))
            {
                this.ExplosionParticleSize -= 5;
                this.ExplosionParticleSize = (this.ExplosionParticleSize < 1 ? 1 : this.ExplosionParticleSize);
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.N))
            {
                this.ExplosionParticleSize += 5;
                this.ExplosionParticleSize = (this.ExplosionParticleSize > 100 ? 100 : this.ExplosionParticleSize);
            }
	    }
	}
}