#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF
{
	/// <summary>
	/// The Default Sprite with Texture Coordinates Particle System to inherit from, which uses Default Sprite with Texture Coordinates Particles
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DefaultSpriteTextureCoordinatesParticleSystem : DPSFDefaultSpriteTextureCoordinatesParticleSystem<DefaultSpriteTextureCoordinatesParticle, DefaultSpriteParticleVertex>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
		/// parameter if not using a Game object.</param>
		public DefaultSpriteTextureCoordinatesParticleSystem(Game cGame) : base(cGame) { }
	}

	/// <summary>
	/// Particle used by the Default Sprite with Texture Coordinates Particle System
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class DefaultSpriteTextureCoordinatesParticle : DefaultSpriteParticle
	{
		/// <summary>
		/// The top-left Position and the Dimensions of this Picture in the Texture
		/// </summary>
		public Rectangle TextureCoordinates;

        /// <summary>
        /// Sets the Texture Coordinates to use for the Picture that represents this Particle.
        /// </summary>
        /// <param name="textureCoordinates">The top-left Position and the Dimensions of the Picture in the Texture.</param>
        public void SetTextureCoordinates(Rectangle textureCoordinates)
        {
            TextureCoordinates.X = textureCoordinates.X;
            TextureCoordinates.Y = textureCoordinates.Y;
            TextureCoordinates.Width = textureCoordinates.Width;
            TextureCoordinates.Height = textureCoordinates.Height;
        }

		/// <summary>
		/// Sets the Texture Coordinates to use for the Picture that represents this Particle.
		/// </summary>
		/// <param name="iLeft">The X position of the top-left corner of the Picture in the Texture</param>
		/// <param name="iTop">The Y position of the top-left corner of the Picture in the Texture</param>
		/// <param name="iRight">The X position of the bottom-right corner of the Picture in the Texture</param>
		/// <param name="iBottom">The Y position of the bottom-right corner of the Picture in the Texture</param>
		public void SetTextureCoordinates(int iLeft, int iTop, int iRight, int iBottom)
		{
			TextureCoordinates.X = iLeft;
			TextureCoordinates.Y = iTop;
			TextureCoordinates.Width = (iRight - iLeft);
			TextureCoordinates.Height = (iBottom - iTop);
		}

		/// <summary>
		/// Resets the Particle variables to their default values
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			TextureCoordinates = new Rectangle();
		}

		/// <summary>
		/// Deep copy all of the Particle properties
		/// </summary>
		/// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
		public override void CopyFrom(DPSFParticle ParticleToCopy)
		{
			// Cast the Particle to the type it really is
            DefaultSpriteTextureCoordinatesParticle cParticleToCopy = (DefaultSpriteTextureCoordinatesParticle)ParticleToCopy;

			base.CopyFrom(cParticleToCopy);
			this.TextureCoordinates = cParticleToCopy.TextureCoordinates;
		}
	}

	/// <summary>
	/// The Default Sprite with Texture Coordinates Particle System class
	/// </summary>
	/// <typeparam name="Particle">The Particle class to use</typeparam>
	/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DPSFDefaultSpriteTextureCoordinatesParticleSystem<Particle, Vertex> : DPSFDefaultSpriteParticleSystem<Particle, Vertex>
		where Particle : DPSFParticle, new()
		where Vertex : struct, IDPSFParticleVertex
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
		/// parameter if not using a Game object.</param>
		public DPSFDefaultSpriteTextureCoordinatesParticleSystem(Game cGame) : base(cGame) { }

		//===========================================================
		// Draw Sprite and Overridden Particle System Functions
		//===========================================================

		/// <summary>
		/// Function to draw a Sprite Particle. This function should be used to draw the given
		/// Particle with the provided SpriteBatch.
		/// </summary>
		/// <param name="Particle">The Particle Sprite to Draw</param>
		/// <param name="cSpriteBatch">The SpriteBatch to use to doing the Drawing</param>
		protected override void DrawSprite(DPSFParticle Particle, SpriteBatch cSpriteBatch)
		{
			// Cast the Particle to the type it really is
			DefaultSpriteTextureCoordinatesParticle cParticle = (DefaultSpriteTextureCoordinatesParticle)Particle;

			// Get the Position and Dimensions of the Particle in 2D space
			Rectangle sDestination = new Rectangle((int)cParticle.Position.X, (int)cParticle.Position.Y,
												   (int)cParticle.Width, (int)cParticle.Height);

			// Get the Depth (Z-Buffer value) of the Particle clamped in the range 0.0 - 1.0 (0.0 = front, 1.0 = back)
			float fNormalizedDepth = MathHelper.Clamp(cParticle.Position.Z, 0.0f, 1.0f);

			// Get the Position and Dimensions from the Texture to use for this Sprite
			Rectangle sSourceFromTexture = cParticle.TextureCoordinates;

			// Make the Sprite rotate about its center
			Vector2 sOrigin = new Vector2(sSourceFromTexture.Width / 2, sSourceFromTexture.Height / 2);

			// Draw the Sprite
			cSpriteBatch.Draw(Texture, sDestination, sSourceFromTexture, cParticle.Color, cParticle.Rotation, sOrigin, cParticle.FlipMode, fNormalizedDepth);
		}
	}
}
