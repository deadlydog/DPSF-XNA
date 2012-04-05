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
    /// Create a new Particle System class that inherits from a
    /// Default DPSF Particle System
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    class MultipleParticleImagesParticleSystem : DefaultTexturedQuadTextureCoordinatesParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MultipleParticleImagesParticleSystem(Game cGame) : base(cGame) { }

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

        //===========================================================
        // Initialization Functions
        //===========================================================

        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000,
                                                UpdateVertexProperties, "Textures/ExplosionParticles");
            Name = "Multiple Particle Images";
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
            ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 300);

            // These textures contain both opaque and semi-transparent portions, so in order to get them drawn correctly (so that
            // particles further away aren't drawn overtop of closer particles), we must sort the particles so that they are drawn 
            // in the order of farthest from the camera to closest to the camera.
            // Simply using DepthStencilState.Default to turn on depth buffer writing can't be used because it produces clipping problems.
            // These operations are expensive, so they should only be used if it is important that particles in front hide the particles behind them.
            ParticleEvents.AddEveryTimeEvent(UpdateParticleDistanceFromCameraSquared);
            ParticleSystemEvents.AddEveryTimeEvent(UpdateParticleSystemToSortParticlesByDistanceFromCamera);

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
            InitialProperties.RotationalVelocityMin = new Vector3(0, 0, -MathHelper.PiOver2);
            InitialProperties.RotationalVelocityMax = new Vector3(0, 0, MathHelper.PiOver2);
        }

        public void InitializeParticleWithTextureCoordinates(DefaultTextureQuadTextureCoordinatesParticle cParticle)
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
            cParticle.SetTextureCoordinates(sTextureCoordinates, Texture.Width, Texture.Height);
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================
        protected void BounceOffFloor(DefaultTextureQuadTextureCoordinatesParticle cParticle, float fElapsedTimeInSeconds)
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

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        //===========================================================
        // Other Particle System Functions
        //===========================================================

    }
}
