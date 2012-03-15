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
    class GasFallParticleSystem : DefaultPointSpriteParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GasFallParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================
        protected override void SetEffectParameters()
        {
            base.SetEffectParameters();

            // Specify to not use the Color component when drawing (Texture color is not tinted)
            Effect.Parameters["xColorBlendAmount"].SetValue(1.0f);
        }

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializePointSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000,
                                                UpdateVertexProperties, "Textures/Smoke");
            LoadEvents();
            Emitter.ParticlesPerSecond = 100;
            Name = "Gas Fall";
        }

        public void LoadEvents()
        {
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingExternalForce);
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration, 500);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleSizeUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);

            InitialProperties.LifetimeMin = 5.0f;
            InitialProperties.LifetimeMax = 5.0f;
            InitialProperties.PositionMin = new Vector3(0, 100, 0);
            InitialProperties.PositionMax = new Vector3(0, 100, 0);
            InitialProperties.StartSizeMin = 1.0f;
            InitialProperties.StartSizeMax = 10.0f;
            InitialProperties.EndSizeMin = 40.0f;
            InitialProperties.EndSizeMax = 50.0f;
            InitialProperties.StartColorMin = Color.Green;
            InitialProperties.StartColorMax = Color.Green;
            InitialProperties.EndColorMin = Color.Yellow;
            InitialProperties.EndColorMax = Color.Yellow;
            InitialProperties.InterpolateBetweenMinAndMaxColors = true;
            InitialProperties.RotationMin = -MathHelper.TwoPi;
            InitialProperties.RotationMax = MathHelper.TwoPi;
            InitialProperties.VelocityMin = new Vector3(-20, 0, 0);
            InitialProperties.VelocityMax = new Vector3(20, 0, 0);
            InitialProperties.AccelerationMin = Vector3.Zero;
            InitialProperties.AccelerationMax = Vector3.Zero;
            InitialProperties.RotationalVelocityMin = -MathHelper.TwoPi;
            InitialProperties.RotationalVelocityMax = MathHelper.TwoPi;
            InitialProperties.ExternalForceMin = new Vector3(0, -10, 0);
            InitialProperties.ExternalForceMax = new Vector3(0, -10, 0);
        }

        public void LoadExtraEvents()
        {
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingExternalForce);
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration, 500);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleSizeUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);

            ParticleEvents.AddNormalizedTimedEvent(0.6f, IncreaseHorizontalMovement);

            InitialProperties.LifetimeMin = 4.0f;
            InitialProperties.LifetimeMax = 5.0f;
            InitialProperties.PositionMin = new Vector3(0, 100, 0);
            InitialProperties.PositionMax = new Vector3(0, 100, 0);
            InitialProperties.StartSizeMin = 10.0f;
            InitialProperties.StartSizeMax = 10.0f;
            InitialProperties.EndSizeMin = 50.0f;
            InitialProperties.EndSizeMax = 50.0f;
            InitialProperties.StartColorMin = Color.Green;
            InitialProperties.StartColorMax = Color.Yellow;
            InitialProperties.EndColorMin = Color.Yellow;
            InitialProperties.EndColorMax = Color.Yellow;
            InitialProperties.RotationMin = -MathHelper.TwoPi;
            InitialProperties.RotationMax = MathHelper.TwoPi;
            InitialProperties.VelocityMin = new Vector3(-20, 0, -15);
            InitialProperties.VelocityMax = new Vector3(20, 0, -10);
            InitialProperties.AccelerationMin = Vector3.Zero;
            InitialProperties.AccelerationMax = Vector3.Zero;
            InitialProperties.RotationalVelocityMin = -MathHelper.TwoPi;
            InitialProperties.RotationalVelocityMax = MathHelper.TwoPi;
            InitialProperties.ExternalForceMin = new Vector3(0, -10, 0);
            InitialProperties.ExternalForceMax = new Vector3(0, -10, 0);
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================
        protected void IncreaseHorizontalMovement(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            if (cParticle.Position.X > -50 && cParticle.Position.X < 50)
            {
                cParticle.Velocity.X *= Math.Abs((50.0f / cParticle.Position.X));
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