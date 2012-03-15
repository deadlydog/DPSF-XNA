#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace DPSF.ParticleSystems
{
    /// <summary>
    /// Create a new Particle System class that inherits from a
    /// Default DPSF Particle System
    /// </summary>
    [Serializable]
    class ExplosionFireSmokeParticleSystem : DefaultTexturedQuadTextureCoordinatesParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExplosionFireSmokeParticleSystem(Game game) : base(game) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        /// <summary>
        /// The Color of the explosion.
        /// </summary>
        public Color ExplosionColor { get; set; }

        /// <summary>
        /// The Size of the individual Particles.
        /// </summary>
        public int ExplosionParticleSize { get; set; }

        /// <summary>
        /// The Intensity of the explosion.
        /// </summary>
        public int ExplosionIntensity { get; set; }

        Rectangle _flameSmoke1TextureCoordinates = new Rectangle(0, 0, 128, 128);
        Rectangle _flameSmoke2TextureCoordinates = new Rectangle(128, 0, 128, 128);
        Rectangle _flameSmoke3TextureCoordinates = new Rectangle(0, 128, 128, 128);
        Rectangle _flameSmoke4TextureCoordinates = new Rectangle(128, 128, 128, 128);

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================
        protected override void SetRenderState(RenderState renderState)
        {
            base.SetRenderState(renderState);

            // Show the texture on both the front and back of the quad
            renderState.CullMode = CullMode.None;

            // Use additive blending
            renderState.DestinationBlend = Blend.One;
        }

        protected override void ResetRenderState(RenderState renderState)
        {
            base.ResetRenderState(renderState);

            // Restore the Cull Mode
            renderState.CullMode = CullMode.CullCounterClockwiseFace;
        }

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice graphicsDevice, ContentManager contentManager, SpriteBatch spriteBatch)
        {
            InitializeTexturedQuadParticleSystem(graphicsDevice, contentManager, 1000, 50000,
                                                UpdateVertexProperties, "Textures/ExplosionParticles");

            Name = "Explosion - Fire Smoke";
            LoadEvents();
        }

        public void LoadEvents()
        {
            // Specify the particle initialization function
            ParticleInitializationFunction = InitializeParticleExplosion;

            // Setup the behaviors that the particles should have
            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingExternalForce);
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleFireSmokeColor);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleFireSmokeSize);

            // Setup the emitter
            Emitter.PositionData.Position = new Vector3(0, 50, 0);
            Emitter.ParticlesPerSecond = 10000;
            Emitter.EmitParticlesAutomatically = false; // We will call the Explode() function to release a burst of particles instead of always emitting them

            // Set the default explosion settings
            ExplosionColor = new Color(255, 120, 0);
            ExplosionParticleSize = 30;
            ExplosionIntensity = 25;
        }

        public void SetupToAutoExplodeEveryInterval(float intervalInSeconds)
        {
            // Set the Particle System's Emitter to release a burst of particles after a set interval
            ParticleSystemEvents.RemoveAllEventsInGroup(1);
            ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
            ParticleSystemEvents.LifetimeData.Lifetime = intervalInSeconds;
            ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemToExplode, 0, 1);
        }

        public void InitializeParticleExplosion(DefaultTextureQuadTextureCoordinatesParticle particle)
        {
            particle.Lifetime = RandomNumber.Between(0.3f, 0.7f);
            particle.Color = particle.StartColor = ExplosionColor;
            particle.EndColor = Color.Black;
            particle.Position = Emitter.PositionData.Position + new Vector3(RandomNumber.Next(-25, 25), RandomNumber.Next(-25, 25), RandomNumber.Next(-25, 25));
            particle.Velocity = DPSFHelper.RandomNormalizedVector() * RandomNumber.Next(1, 50);
            particle.ExternalForce = new Vector3(0, 80, 0); // We want the smoke to rise
            particle.Size = particle.StartSize = 1;         // Have the particles start small and grow
            particle.EndSize = ExplosionParticleSize;

            // Give the particle a random initial orientation and random rotation velocity (only need to Roll the particle since it will always face the camera)
            particle.Orientation = Orientation3D.Rotate(Matrix.CreateFromYawPitchRoll(0, 0, RandomNumber.Between(0, MathHelper.TwoPi)), particle.Orientation);
            particle.RotationalVelocity = new Vector3(0, 0, RandomNumber.Between(-MathHelper.PiOver2, MathHelper.PiOver2));

            // Randomly pick which texture coordinates to use for this particle
            Rectangle textureCoordinates;
            switch (RandomNumber.Next(0, 4))
            {
                default:
                case 0: textureCoordinates = _flameSmoke1TextureCoordinates; break;
                case 1: textureCoordinates = _flameSmoke2TextureCoordinates; break;
                case 2: textureCoordinates = _flameSmoke3TextureCoordinates; break;
                case 3: textureCoordinates = _flameSmoke4TextureCoordinates; break;
            }

            particle.SetTextureCoordinates(textureCoordinates, Texture.Width, Texture.Height);
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================
        protected void UpdateParticleFireSmokeColor(DefaultTextureQuadTextureCoordinatesParticle particle, float elapsedTimeInSeconds)
        {
            // Have particle be the specified color for the first part of its lifetime
            float firstPartOfLifetime = 0.2f;
            if (particle.NormalizedElapsedTime < firstPartOfLifetime)
            {
                particle.Color = particle.StartColor;
            }
            // Then start fading it to black to look like smoke
            else
            {
                float lerpAmount = (particle.NormalizedElapsedTime - firstPartOfLifetime) * (1.0f / (1.0f - firstPartOfLifetime));
                particle.Color = DPSFHelper.LerpColor(particle.StartColor, particle.EndColor, lerpAmount);
            }
        }

        protected void UpdateParticleFireSmokeSize(DefaultTextureQuadTextureCoordinatesParticle particle, float elapsedTimeInSeconds)
        {
            // Have particle grow to its full size within the first 20% of its lifetime
            if (particle.NormalizedElapsedTime < 0.2f)
            {
                particle.Size = MathHelper.Lerp(particle.StartWidth, particle.EndWidth, particle.NormalizedElapsedTime * 5);
            }
            else
            {
                particle.Size = particle.EndWidth;
            }
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================
        protected void UpdateParticleSystemToExplode(float elapsedTimeInSeconds)
        {
            Explode();
        }

        //===========================================================
        // Other Particle System Functions
        //===========================================================

        /// <summary>
        /// Start the explosion.
        /// </summary>
        public void Explode()
        {
            this.Emitter.BurstParticles = this.ExplosionIntensity;
        }

        /// <summary>
        /// Change the color of the explosion to a random color.
        /// </summary>
        public void ChangeExplosionColor()
        {
            ExplosionColor = DPSFHelper.RandomColor();
        }

        /// <summary>
        /// Change the color of the explosion to the given color.
        /// </summary>
        /// <param name="color">The color the explosion should be.</param>
        public void ChangeExplosionColor(Color color)
        {
            ExplosionColor = color;
        }
    }
}