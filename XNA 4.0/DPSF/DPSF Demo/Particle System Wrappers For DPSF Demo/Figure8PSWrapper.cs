using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DPSF.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class Figure8DPSFDemoParticleSystemWrapper : Figure8ParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public Figure8DPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    { }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    { }

	    public void ProcessInput()
	    { }
	}
}