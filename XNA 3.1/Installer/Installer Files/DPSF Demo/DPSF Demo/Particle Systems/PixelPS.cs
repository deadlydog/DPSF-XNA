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
    class PixelParticleSystem : DefaultPixelParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PixelParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        public bool mbGravityEnabled = true;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializePixelParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, UpdateVertexProperties);
            Name = "Pixels";
            LoadSprayEvents();
        }

        public void LoadSprayEvents()
        {
            ParticleInitializationFunction = InitializeParticleSpray;

            Emitter.ParticlesPerSecond = 2000;
            Emitter.PositionData.Position = new Vector3(-100, 50, 0);

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration, 100);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingExternalForce);
        }

        public void InitializeParticleSpray(DefaultPixelParticle cParticle)
        {
            cParticle.Lifetime = 3.0f;
            cParticle.Position = Emitter.PositionData.Position;

            // Update Velocity direction according to Emitter's Orientation
            cParticle.Velocity = new Vector3(RandomNumber.Next(20, 100), RandomNumber.Next(5, 50), RandomNumber.Next(-25, 25));
            cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);

            if (mbGravityEnabled)
            {
                cParticle.ExternalForce = new Vector3(0, -30, 0);
            }
            cParticle.Color = DPSFHelper.RandomColor();
        }

        public void LoadWallEvents()
        {
            ParticleInitializationFunction = InitializeParticleWall;  

            Emitter.ParticlesPerSecond = 5000;
            Emitter.PositionData.Position = Vector3.Zero;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration, 100);
        }

        public void InitializeParticleWall(DefaultPixelParticle cParticle)
        {
            cParticle.Lifetime = 1.0f;
            cParticle.Position = new Vector3(RandomNumber.Next(-200, 200), RandomNumber.Next(-40, 160), RandomNumber.Next(0, 100));
            
            // Update the Particle's Position according to the Emitters Orientation and Position
            cParticle.Position = Vector3.Transform(cParticle.Position, Emitter.OrientationData.Orientation);
            cParticle.Position += Emitter.PositionData.Position;

            cParticle.Color = DPSFHelper.RandomColor();

            if (mbGravityEnabled)
            {
                cParticle.Velocity = new Vector3(0, RandomNumber.Next(-50, -5), 0);
            }
            cParticle.Acceleration = Vector3.Zero;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        //===========================================================
        // Particle System Update Functions
        //===========================================================
        
        //===========================================================
        // Other Particle System Functions
        //===========================================================
    }
}