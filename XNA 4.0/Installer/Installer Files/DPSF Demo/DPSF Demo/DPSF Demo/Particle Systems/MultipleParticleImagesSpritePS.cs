#region Using Statements
using System;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF_Demo.ParticleSystems
{
    /// <summary>
    /// Create a new Particle System class that inherits from a Default DPSF Particle System
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    class MultipleParticleImagesSpriteParticleSystem : DefaultSprite3DBillboardTextureCoordinatesParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MultipleParticleImagesSpriteParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        // Specify the texture coordinates of the images we want to use for the particles
        Rectangle msCloudTextureCoordinates = new Rectangle(0, 0, 128, 128);
        Rectangle msSparkTextureCoordinates = new Rectangle(384, 0, 128, 128);
        Rectangle msRockTextureCoordinates = new Rectangle(254, 256, 128, 128);
        Rectangle msRingTextureCoordinates = new Rectangle(382, 255, 130, 130);

        // How much the Particle should bounce back off of the floor
        public float mfBounciness = 0.35f;

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

        /// <summary>
        /// Initializes the render properties.
        /// </summary>
        protected override void InitializeRenderProperties()
        {
            base.InitializeRenderProperties();

            // Enable the depth buffer
            RenderProperties.DepthStencilState = DepthStencilState.Default;
        }

        //===========================================================
        // Initialization Functions
        //===========================================================

        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, "Textures/ExplosionParticles", cSpriteBatch);
            Name = "Multiple Particle Images (Sprites)";
            Emitter.ParticlesPerSecond = 5;
            LoadEvents();
        }

        public void LoadEvents()
        {
            ParticleInitializationFunction = InitializeParticleWithTextureCoordinates;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(BounceOffFloor, 200);

            Emitter.PositionData.Position = new Vector3(0, 50, 0);

            InitialProperties.LifetimeMin = 8.0f;
            InitialProperties.LifetimeMax = 8.0f;
            InitialProperties.PositionMin = new Vector3(-100, 100, -100);
            InitialProperties.PositionMax = new Vector3(100, 100, 100);
            InitialProperties.StartSizeMin = 30.0f;
            InitialProperties.StartSizeMax = 30.0f;
            InitialProperties.EndSizeMin = 30.0f;
            InitialProperties.EndSizeMax = 30.0f;
            InitialProperties.StartColorMin = Color.Black;
            InitialProperties.StartColorMax = Color.White;
            InitialProperties.EndColorMin = Color.Black;
            InitialProperties.EndColorMax = Color.White;
            InitialProperties.InterpolateBetweenMinAndMaxColors = false;
            InitialProperties.VelocityMin = new Vector3(-50, -50, -50);
            InitialProperties.VelocityMax = new Vector3(50, -100, 50);
            InitialProperties.AccelerationMin = new Vector3(0, -50, 0);
            InitialProperties.AccelerationMax = new Vector3(0, -50, 0);
            InitialProperties.RotationalVelocityMin = -MathHelper.PiOver2;
            InitialProperties.RotationalVelocityMax = MathHelper.PiOver2;
        }

        public void InitializeParticleWithTextureCoordinates(DefaultSprite3DBillboardTextureCoordinatesParticle cParticle)
        {
            // Initialize the particle using the InitialProperties specified above
            InitializeParticleUsingInitialProperties(cParticle);

            // Randomly pick which texture coordinates to use for this particle
            Rectangle sTextureCoordinates;
            switch (RandomNumber.Next(0, 4))
            {
                default:
                case 0: sTextureCoordinates = msCloudTextureCoordinates; break;
                case 1: sTextureCoordinates = msSparkTextureCoordinates; break;
                case 2: sTextureCoordinates = msRockTextureCoordinates; break;
                case 3: sTextureCoordinates = msRingTextureCoordinates; break;
            }

            // Set the Particle's Texture Coordinates
            cParticle.SetTextureCoordinates(sTextureCoordinates);
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================
        protected void BounceOffFloor(DefaultSprite3DBillboardTextureCoordinatesParticle cParticle, float fElapsedTimeInSeconds)
        {
            // If the Particle has hit the floor and is still travelling downwards
            if (cParticle.Position.Y <= 0 && cParticle.Velocity.Y < 0)
            {
                // Make the Particle Bounce upwards
                cParticle.Velocity.Y *= -mfBounciness;

                // Reduce the Particles X and Z speed
                cParticle.Velocity.X *= 0.8f;
                cParticle.Velocity.Z *= 0.8f;

                // Reduce the Particles Rotation speed
                cParticle.RotationalVelocity *= 0.8f;
            }
        }

        /// <summary>
        /// Updates the Particle's DistanceFromCameraSquared property to reflect how far this Particle is from the Camera.
        /// </summary>
        /// <param name="cParticle">The Particle to update.</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update.</param>
        protected void UpdateParticleDistanceFromCameraSquared(DefaultSprite3DBillboardTextureCoordinatesParticle cParticle, float fElapsedTimeInSeconds)
        {
            //cParticle.DistanceFromCameraSquared = Vector3.DistanceSquared(this.CameraPosition, cParticle.Position);
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        //===========================================================
        // Other Particle System Functions
        //===========================================================

    }
}
