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
    /// Create a new Particle System class that inherits from a Default DPSF Particle System
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    class ExplosionDebrisSpriteParticleSystem : DefaultSpriteTextureCoordinatesParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
		public ExplosionDebrisSpriteParticleSystem(Game game) : base(game) { }

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

        Rectangle _debris1TextureCoordinates = new Rectangle(256, 256, 39, 44);
        Rectangle _debris2TextureCoordinates = new Rectangle(300, 261, 35, 33);
        Rectangle _debris3TextureCoordinates = new Rectangle(344, 263, 38, 30);
        Rectangle _debris4TextureCoordinates = new Rectangle(259, 302, 37, 35);
        Rectangle _debris5TextureCoordinates = new Rectangle(298, 299, 42, 41);
        Rectangle _debris6TextureCoordinates = new Rectangle(342, 306, 40, 32);
        Rectangle _debris7TextureCoordinates = new Rectangle(257, 345, 39, 36);
        Rectangle _debris8TextureCoordinates = new Rectangle(299, 349, 41, 25);
        Rectangle _debris9TextureCoordinates = new Rectangle(343, 342, 36, 40);

        /// <summary>
        /// Get / Set the Camera Position used by the particle system
        /// </summary>
        public Vector3 CameraPosition { get; set; }

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        /// <summary>
        /// Sets the camera position.
        /// </summary>
        /// <param name="cameraPosition">The camera position.</param>
        public override void SetCameraPosition(Vector3 cameraPosition)
        {
            this.CameraPosition = cameraPosition;
        }

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice graphicsDevice, ContentManager contentManager, SpriteBatch spriteBatch)
        {
            InitializeSpriteParticleSystem(graphicsDevice, contentManager, 1000, 50000, "Textures/ExplosionParticles", spriteBatch);

            Name = "Explosion - Debris (Sprites)";
            LoadEvents();
        }

        public void LoadEvents()
        {
            // Specify the particle initialization function
            ParticleInitializationFunction = InitializeParticleExplosion;

            // Setup the behaviours that the particles should have
            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingExternalForce);
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndQuickFadeOut, 100);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);

            // Setup the emitter
            Emitter.PositionData.Position = new Vector3(this.GraphicsDevice.Viewport.Width / 2, this.GraphicsDevice.Viewport.Height / 2, 0);
            Emitter.ParticlesPerSecond = 10000;
            Emitter.EmitParticlesAutomatically = false; // We will call the Explode() function to release a burst of particles instead of always emitting them

            // Set the default explosion settings
            ExplosionColor = new Color(255, 120, 0);
            ExplosionParticleSize = 20;
            ExplosionIntensity = 20;
        }

        public void SetupToAutoExplodeEveryInterval(float intervalInSeconds)
        {
            // Set the Particle System's Emitter to release a burst of particles after a set interval
            ParticleSystemEvents.RemoveAllEventsInGroup(1);
            ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
            ParticleSystemEvents.LifetimeData.Lifetime = intervalInSeconds;
            ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemToExplode, 0, 1);
        }

        public void InitializeParticleExplosion(DefaultSpriteTextureCoordinatesParticle particle)
        {
            particle.Lifetime = RandomNumber.Between(1.2f, 1.7f);
            particle.Color = particle.StartColor = ExplosionColor;
            particle.EndColor = Color.DarkGray;
            particle.Position = Emitter.PositionData.Position;
            particle.ExternalForce = new Vector3(0, 80, 0);
            particle.RotationalVelocity = RandomNumber.Between(-MathHelper.PiOver2, MathHelper.PiOver2);
            particle.Velocity = DPSFHelper.RandomNormalizedVector();    // Calculate the direction the particle will travel in

            // We want the debris to travel upwards more often than downward, so if it's travelling downward switch it to travel upward 50% of the time
            if (particle.Velocity.Y > 0 && RandomNumber.Next(0, 2) == 0)
                particle.Velocity.Y *= -1;

            // Fire some particles towards the camera (but not directly at it) for a more dramatic effect
            if (RandomNumber.Between(0, 5) == 0)
            {
                // Calculate a point somewhere around the camera
                int distance = 10;
                Vector3 somewhereAroundTheCamera = new Vector3(CameraPosition.X + RandomNumber.Next(-distance, distance),
                                                               CameraPosition.Y + RandomNumber.Next(-distance, distance),
                                                               CameraPosition.Z + RandomNumber.Next(-distance, distance));

                // Direct the Particle towards the spot around the camera
                particle.Velocity = somewhereAroundTheCamera - particle.Position;
                particle.Velocity.Normalize();
            }

            // Set the Particle's Speed
            particle.Velocity *= RandomNumber.Next(100, 150);

            // Randomly pick which texture coordinates to use for this particle
            Rectangle textureCoordinates;
            switch (RandomNumber.Next(0, 9))
            {
                default:
                case 0: textureCoordinates = _debris1TextureCoordinates; break;
                case 1: textureCoordinates = _debris2TextureCoordinates; break;
                case 2: textureCoordinates = _debris3TextureCoordinates; break;
                case 3: textureCoordinates = _debris4TextureCoordinates; break;
                case 4: textureCoordinates = _debris5TextureCoordinates; break;
                case 5: textureCoordinates = _debris6TextureCoordinates; break;
                case 6: textureCoordinates = _debris7TextureCoordinates; break;
                case 7: textureCoordinates = _debris8TextureCoordinates; break;
                case 8: textureCoordinates = _debris9TextureCoordinates; break;
            }

            particle.SetTextureCoordinates(textureCoordinates);

            // Set the Width to Height ratio so the image isn't skewed when we scale it
            particle.Width = textureCoordinates.Width;
            particle.Height = textureCoordinates.Height;

            // Set the particle to the specified size, give or take 25%
            particle.ScaleToWidth(ExplosionParticleSize * RandomNumber.Between(0.75f, 1.25f));
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