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
    class FountainParticleSystem : DefaultPointSpriteParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FountainParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        // How much the Particle should bounce back off of the floor
        public float mfBounciness = 0.5f;
        public bool mbUseAdditiveBlending = false;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================
        protected override void SetRenderState(RenderState cRenderState)
        {
            base.SetRenderState(cRenderState);

            // If Additive Blending should be used
            if (mbUseAdditiveBlending)
            {
                // Turn it on
                cRenderState.DestinationBlend = Blend.One;
            }
        }

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager)
        {
            InitializePointSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000,
                                                UpdateVertexProperties, "Textures/Bubble");
            LoadFountainEvents();
            Emitter.ParticlesPerSecond = 10;
            Name = "Fountain";
        }

        public void LoadFountainEvents()
        {
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingExternalForce, 400);
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration, 500);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);
            ParticleEvents.AddEveryTimeEvent(BounceOffFloor, 450);

            Emitter.PositionData.Position = new Vector3(0, 50, 0);

            InitialProperties.LifetimeMin = 8.0f;
            InitialProperties.LifetimeMax = 13.0f;
            InitialProperties.PositionMin = Vector3.Zero;
            InitialProperties.PositionMax = Vector3.Zero;
            InitialProperties.StartSizeMin = 10.0f;
            InitialProperties.StartSizeMax = 10.0f;
            InitialProperties.EndSizeMin = 0.0f;
            InitialProperties.EndSizeMax = 0.0f;
            InitialProperties.StartColorMin = Color.Black;
            InitialProperties.StartColorMax = Color.White;
            InitialProperties.EndColorMin = Color.Red;
            InitialProperties.EndColorMax = Color.DarkBlue;
            InitialProperties.InterpolateBetweenMinAndMaxColors = false;
            InitialProperties.RotationMin = 0.0f;
            InitialProperties.RotationMax = MathHelper.TwoPi;
            InitialProperties.VelocityMin = new Vector3(-25, 20, -25);
            InitialProperties.VelocityMax = new Vector3(25, 75, 25);
            InitialProperties.AccelerationMin = Vector3.Zero;
            InitialProperties.AccelerationMax = Vector3.Zero;
            InitialProperties.RotationalVelocityMin = -MathHelper.TwoPi;
            InitialProperties.RotationalVelocityMax = MathHelper.TwoPi;
            InitialProperties.ExternalForceMin = new Vector3(0, -40, 0);
            InitialProperties.ExternalForceMax = new Vector3(0, -40, 0);
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================
        public void BounceOffFloor(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
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