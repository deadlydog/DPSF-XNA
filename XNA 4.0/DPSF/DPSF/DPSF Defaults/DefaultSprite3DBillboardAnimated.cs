#region Using Statements
using System;
using Microsoft.Xna.Framework;

#endregion

namespace DPSF
{
    /// <summary>
    /// The Default Animated 3D Billboard Sprite Particle System to inherit from, which uses Default Animated Sprite Particles
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
	public abstract class DefaultAnimatedSprite3DBillboardParticleSystem : DPSFDefaultAnimatedSprite3DBillboardParticleSystem<DefaultAnimatedSprite3DBillboardParticle, DefaultSpriteParticleVertex>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DefaultAnimatedSprite3DBillboardParticleSystem(Game cGame) : base(cGame) { }
    }

    /// <summary>
    /// Particle used by the Default Animated Sprite 3D Billboard Particle System
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    public class DefaultAnimatedSprite3DBillboardParticle : DefaultSprite3DBillboardTextureCoordinatesParticle
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
            DefaultAnimatedSprite3DBillboardParticle cParticleToCopy = (DefaultAnimatedSprite3DBillboardParticle)ParticleToCopy;

            base.CopyFrom(cParticleToCopy);
            Animation.CopyFrom(cParticleToCopy.Animation);
        }
    }

    /// <summary>
    /// The Default Animated Sprite Particle System class
    /// </summary>
    /// <typeparam name="Particle">The Particle class to use</typeparam>
    /// <typeparam name="Vertex">The Vertex Format to use</typeparam>
#if (WINDOWS)
    [Serializable]
#endif
	public abstract class DPSFDefaultAnimatedSprite3DBillboardParticleSystem<Particle, Vertex> : DPSFDefaultSprite3DBillboardTextureCoordinates<Particle, Vertex>
        where Particle : DPSFParticle, new()
        where Vertex : struct, IDPSFParticleVertex
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DPSFDefaultAnimatedSprite3DBillboardParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        /// <summary>
        /// Updates the Animation, as well as the Particle's Texture Coordinates to match the Animation
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        protected void UpdateParticleAnimationAndTextureCoordinates(DefaultAnimatedSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Animation
            cParticle.Animation.Update(fElapsedTimeInSeconds);

            // Get the Particle's Texture Coordinates to use
            cParticle.TextureCoordinates = cParticle.Animation.CurrentPicturesTextureCoordinates;
        }

        /// <summary>
        /// Updates the Particle to be removed from the Particle System once the Animation finishes Playing
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        protected void UpdateParticleToDieOnceAnimationFinishesPlaying(DefaultAnimatedSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
        {
            // If the Animation has finished Playing
            if (cParticle.Animation.CurrentAnimationIsDonePlaying)
            {
                // Make sure the Lifetime is greater than zero
                // We make it a small value to try and keep from triggering any Timed Events by accident
                cParticle.Lifetime = 0.000001f;

                // Set the Particle to die
                cParticle.NormalizedElapsedTime = 1.0f;
            }
        }
    }
}
