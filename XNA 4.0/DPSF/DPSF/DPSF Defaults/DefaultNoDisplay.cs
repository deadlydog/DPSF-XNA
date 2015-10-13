#region Using Statements
using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF
{
	/// <summary>
	/// The Default No Display Particle System to inherit from, which uses Default Pixel Particles
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DefaultNoDisplayParticleSystem : DPSFDefaultNoDisplayParticleSystem<DefaultNoDisplayParticle, DefaultNoDisplayParticleVertex>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
		/// parameter if not using a Game object.</param>
		public DefaultNoDisplayParticleSystem(Game cGame) : base(cGame) { }
	}

	/// <summary>
	/// Particle used by the No Display Particle System
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class DefaultNoDisplayParticle : DPSFDefaultBaseParticle
	{ }

	/// <summary>
	/// The Default No Display Particle System class
	/// </summary>
	/// <typeparam name="Particle">The Particle class to use</typeparam>
	/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DPSFDefaultNoDisplayParticleSystem<Particle, Vertex> : DPSFDefaultBaseParticleSystem<Particle, Vertex>
		where Particle : DPSFParticle, new()
		where Vertex : struct, IDPSFParticleVertex
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
		/// parameter if not using a Game object.</param>
		public DPSFDefaultNoDisplayParticleSystem(Game cGame) : base(cGame) { }
	}

	/// <summary>
	/// Dummy structure used for the vertices of a No Display particle system.
	/// Since the particles are not drawn, they do not have vertices, so this structure is empty.
	/// </summary>
#if (WINDOWS)
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
#endif
	public struct DefaultNoDisplayParticleVertex : IDPSFParticleVertex
	{
		/// <summary>
		/// An array describing the attributes of each Vertex
		/// </summary>
		public VertexDeclaration VertexDeclaration
		{
			get { return null; }
		}
	}
}
