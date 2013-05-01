#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF
{
	/// <summary>
	/// The Default Animated Textured Quad Particle System to inherit from, which uses Default Animated Textured Quad Particles
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DefaultAnimatedTexturedQuadParticleSystem : DPSFDefaultAnimatedTexturedQuadParticleSystem<DefaultAnimatedTexturedQuadParticle, DefaultTexturedQuadParticleVertex>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
		/// parameter if not using a Game object.</param>
		public DefaultAnimatedTexturedQuadParticleSystem(Game cGame) : base(cGame) { }
	}

	/// <summary>
	/// Particle used by the Default Animated Quad Particle System
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class DefaultAnimatedTexturedQuadParticle : DefaultTextureQuadTextureCoordinatesParticle
	{
		/// <summary>
		/// Class to hold this Particle's Animation information
		/// </summary>
		public Animations Animation;

		/// <summary>
		/// Resets the Particle variables to their default values
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			Animation = new Animations();
		}

		/// <summary>
		/// Deep copy all of the Particle properties
		/// </summary>
		/// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
		public override void CopyFrom(DPSFParticle ParticleToCopy)
		{
			// Cast the Particle to the type it really is
			DefaultAnimatedTexturedQuadParticle cParticleToCopy = (DefaultAnimatedTexturedQuadParticle)ParticleToCopy;

			base.CopyFrom(ParticleToCopy);
			Animation.CopyFrom(cParticleToCopy.Animation);
		}
	}

	/// <summary>
	/// The Default Animated Textured Quad Particle System class
	/// </summary>
	/// <typeparam name="Particle">The Particle class to use</typeparam>
	/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DPSFDefaultAnimatedTexturedQuadParticleSystem<Particle, Vertex> : DPSFDefaultTexturedQuadTextureCoordinatesParticleSystem<Particle, Vertex>
		where Particle : DPSFParticle, new()
		where Vertex : struct, IDPSFParticleVertex
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
		/// parameter if not using a Game object.</param>
		public DPSFDefaultAnimatedTexturedQuadParticleSystem(Game cGame) : base(cGame) { }

		//===========================================================
		// Particle Update Functions
		//===========================================================

		/// <summary>
		/// Updates the Animation, as well as the Particle's Texture Coordinates to match the Animation
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleAnimationAndTextureCoordinates(DefaultAnimatedTexturedQuadParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Update the Animation
			cParticle.Animation.Update(fElapsedTimeInSeconds);

			// Get the Particle's Texture Coordinates
			Rectangle sSourceRect = cParticle.Animation.CurrentPicturesTextureCoordinates;

			// Set the Particle's Texture Coordinates to use
			cParticle.SetTextureCoordinates(sSourceRect, Texture.Width, Texture.Height);
		}

		/// <summary>
		/// Updates the Particle to be removed from the Particle System once the Animation finishes Playing
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleToDieOnceAnimationFinishesPlaying(DefaultAnimatedTexturedQuadParticle cParticle, float fElapsedTimeInSeconds)
		{
			// If the Animation has finished Playing
			if (cParticle.Animation.CurrentAnimationIsDonePlaying)
			{
				// Make sure the Lifetime is greater than zero
				// We make it a small value to try and keep from triggering any Timed Events by accident
				cParticle.Lifetime = 0.00001f;

				// Set the Particle to die
				cParticle.NormalizedElapsedTime = 1.0f;
			}
		}
	}
}
