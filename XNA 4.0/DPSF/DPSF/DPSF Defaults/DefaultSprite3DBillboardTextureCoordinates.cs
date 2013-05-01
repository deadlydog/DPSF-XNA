#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF
{
    /// <summary>
    /// The Default Sprite Particle System to inherit from, which uses Default Sprite Particles.
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
	public abstract class DefaultSprite3DBillboardTextureCoordinatesParticleSystem : DPSFDefaultSprite3DBillboardTextureCoordinates<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this.
        /// parameter if not using a Game object.</param>
        public DefaultSprite3DBillboardTextureCoordinatesParticleSystem(Game cGame) : base(cGame) { }
    }

    /// <summary>
    /// Particle used by the Default Sprite 3D Billboard Particle System
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    public class DefaultSprite3DBillboardTextureCoordinatesParticle : DefaultSprite3DBillboardParticle
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
        /// Sets the Texture Coordinates to use for the Picture that represents this Particle
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
            DefaultSprite3DBillboardTextureCoordinatesParticle cParticleToCopy = (DefaultSprite3DBillboardTextureCoordinatesParticle)ParticleToCopy;

            base.CopyFrom(cParticleToCopy);
            this.TextureCoordinates = cParticleToCopy.TextureCoordinates;
        }
    }

    /// <summary>
    /// The Default Sprite 3D Billboard Particle System class.
    /// This class just inherits from the Default Sprite Particle System class and overrides the DrawSprite()
    /// function to draw the sprites as Billboards in 3D space.
    /// </summary>
    /// <typeparam name="Particle">The Particle class to use.</typeparam>
    /// <typeparam name="Vertex">The Vertex Format to use.</typeparam>
#if (WINDOWS)
    [Serializable]
#endif
	public abstract class DPSFDefaultSprite3DBillboardTextureCoordinates<Particle, Vertex> : DPSFDefaultSprite3DBillboardParticleSystem<Particle, Vertex>
        where Particle : DPSFParticle, new()
        where Vertex : struct, IDPSFParticleVertex
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DPSFDefaultSprite3DBillboardTextureCoordinates(Game cGame) : base(cGame) { }

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
            DefaultSprite3DBillboardTextureCoordinatesParticle particle = (DefaultSprite3DBillboardTextureCoordinatesParticle)Particle;

            // Calculate the sprites position in 3D space, and flip it vertically so that it is not upside-down.
            Vector3 viewSpacePosition = Vector3.Transform(particle.Position, this.View);

            // Get the Position of the Particle relative to the camera
            Vector2 destination = new Vector2(viewSpacePosition.X, viewSpacePosition.Y);

            // Get the Position and Dimensions from the Texture to use for this Sprite
            Rectangle sourceFromTexture = particle.TextureCoordinates;

            // Calculate how much to scale the sprite to get it to the desired Width and Height.
            // Use negative height in order to flip the texture to be right-side up.
            Vector2 scale = new Vector2(particle.Width / sourceFromTexture.Width, -particle.Height / sourceFromTexture.Height);

            // Make the Sprite rotate about its center
            Vector2 origin = new Vector2(sourceFromTexture.Width / 2, sourceFromTexture.Height / 2);

            // Draw the Sprite
            cSpriteBatch.Draw(Texture, destination, sourceFromTexture, particle.ColorAsPremultiplied, particle.Rotation, origin, scale, particle.FlipMode, viewSpacePosition.Z);
        }
    }
}
