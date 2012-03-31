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
	class SnowDPSFDemoParticleSystemWrapper : SnowParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public SnowDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    { }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Apply Wind Force:", new Vector2(5, 250), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "X", new Vector2(180, 250), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Remove Wind Force:", new Vector2(5, 275), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "C", new Vector2(200, 275), draw.PropertyTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.AddWindForce();
            }

            if (KeyboardManager.KeyWasJustReleased(Keys.C))
            {
                this.RemoveWindForce();
            }
	    }
	}
}