using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DPSF.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class TrailDPSFDemoParticleSystemWrapper : TrailParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public TrailDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Move the emitter with (Shift) W/A/S/D", new Vector2(draw.TextSafeArea.Left + 5, draw.TextSafeArea.Top + 250), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "Change textures with (Shift) T", new Vector2(draw.TextSafeArea.Left + 5, draw.TextSafeArea.Top + 275), draw.PropertyTextColor);
	    }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    { }

	    public void ProcessInput()
	    { }
	}
}