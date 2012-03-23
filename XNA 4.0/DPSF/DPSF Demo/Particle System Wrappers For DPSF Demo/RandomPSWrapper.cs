using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DPSF.ParticleSystems;
using Microsoft.Xna.Framework;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class RandomParticleSystemWrapper : RandomParticleSystem, IWrapParticleSystemsForDPSFDemo
	{
		public RandomParticleSystemWrapper(Game cGame) : base(cGame)
		{}

		public void AfterAutoInitialize()
		{
			throw new NotImplementedException();
		}

		public void BeforeDrawText()
		{
			throw new NotImplementedException();
		}

		public void DrawText()
		{
			throw new NotImplementedException();
		}

		public void ProcessInput()
		{
			throw new NotImplementedException();
		}
	}
}
