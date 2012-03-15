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
    public class ExplosionSmokeTrailsParticleSystem : DefaultTexturedQuadTextureCoordinatesParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExplosionSmokeTrailsParticleSystem(Game game) : base(game) { }

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

        Rectangle _smokeTrail1TextureCoordinates = new Rectangle(0, 256, 256, 60);
        Rectangle _smokeTrail2TextureCoordinates = new Rectangle(0, 320, 256, 60);
        Rectangle _smokeTrail3TextureCoordinates = new Rectangle(0, 385, 256, 60);

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================
        protected override void SetRenderState(RenderState renderState)
        {
            base.SetRenderState(renderState);

            // Show the texture on both the front and back of the quad
            renderState.CullMode = CullMode.None;
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

            Name = "Explosion - Smoke Trails";
            LoadEvents();
        }

        public void LoadEvents()
        {
            // Specify the particle initialization function
            ParticleInitializationFunction = InitializeParticleExplosion;

            // Setup the behaviors that the particles should have
            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);

            // Setup the emitter
            Emitter.PositionData.Position = new Vector3(0, 50, 0);
            Emitter.ParticlesPerSecond = 10000;
            Emitter.EmitParticlesAutomatically = false; // We will call the Explode() function to release a burst of particles instead of always emitting them

            // Set the default explosion settings
            ExplosionColor = new Color(250, 190, 90);
            ExplosionParticleSize = 20;
            ExplosionIntensity = 15;
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
            particle.Lifetime = RandomNumber.Between(0.3f, 0.5f);
            particle.Color = ExplosionColor;
            particle.Position = Emitter.PositionData.Position;
            particle.Velocity = DPSFHelper.RandomNormalizedVector() * RandomNumber.Next(100, 150);
            particle.Right = -particle.Velocity;

            // Specify the particle's width-to-height ratio as 8:1 so it looks better
            particle.Width = 8; particle.Height = 1;

            // Calculate and set what the particle's EndWidth and EndHeight should be (2 times the start size)
            particle.ScaleToWidth(ExplosionParticleSize * 2);
            particle.EndWidth = particle.Width;
            particle.EndHeight = particle.Height;

            // Calculate and set what the particle's StartWidth and StartHeight should be
            particle.ScaleToWidth(ExplosionParticleSize);
            particle.StartWidth = particle.Width;
            particle.StartHeight = particle.Height;

            // Randomly pick which texture coordinates to use for this particle
            Rectangle textureCoordinates;
            switch (RandomNumber.Next(0, 3))
            {
                default:
                case 0: textureCoordinates = _smokeTrail1TextureCoordinates; break;
                case 1: textureCoordinates = _smokeTrail2TextureCoordinates; break;
                case 2: textureCoordinates = _smokeTrail3TextureCoordinates; break;
            }
            
            particle.SetTextureCoordinates(textureCoordinates, Texture.Width, Texture.Height);
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

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
