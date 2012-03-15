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
    class FireParticleSystem : DefaultPointSpriteParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FireParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        public bool mbUseAdditiveBlending = true;
        private float mfAmountOfSmokeToRelease = 0.5f;
        public SmokeRingParticleSystem mcSmokeParticleSystem = null;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        protected override void SetEffectParameters()
        {
            base.SetEffectParameters();

            // Specify to not use the Color component when drawing (Texture color is not tinted)
            Effect.Parameters["xColorBlendAmount"].SetValue(0.0f);
        }

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

        protected override void AfterInitialize()
        {
            mcSmokeParticleSystem = new SmokeRingParticleSystem(Game);

            // Initialize the Smoke Particle System
            mcSmokeParticleSystem.InitializePointSpriteParticleSystem(GraphicsDevice, ContentManager, 1000, 50000,
                                             mcSmokeParticleSystem.UpdateVertexProperties, "Textures/Smoke");
            mcSmokeParticleSystem.LoadEvents();

            mcSmokeParticleSystem.DrawOrder = 100;
        }

        protected override void AfterDestroy()
        {
            if (mcSmokeParticleSystem != null)
            {
                mcSmokeParticleSystem.Destroy();
                mcSmokeParticleSystem = null;
            }
        }

        protected override void AfterUpdate(float fElapsedTimeInSeconds)
        {
            // If the Smoke Particle System is Initialized
            if (mcSmokeParticleSystem.IsInitialized())
            {
                // Update the Smoke Particle System manually
                mcSmokeParticleSystem.Update(fElapsedTimeInSeconds);
            }
        }

        protected override void AfterDraw()
        {
            // Set the World, View, and Projection matrices so the Smoke Particle System knows how to draw the particles on screen properly
            mcSmokeParticleSystem.SetWorldViewProjectionMatrices(World, View, Projection);

            // If the Smoke Particle System is Initialized
            if (mcSmokeParticleSystem.IsInitialized())
            {
                // Draw the Smoke Particles manually
                mcSmokeParticleSystem.Draw();
            }
        }

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager)
        {

            InitializePointSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000,
                                                UpdateVertexProperties, "Textures/Fire");
            Name = "Fire and Smoke";
            LoadFireRingEvents();
            Emitter.ParticlesPerSecond = 500;
            SetAmountOfSmokeToRelease(0.5f);
        }

        public void LoadFireRingEvents()
        {
            ParticleInitializationFunction = InitializeParticleFireOnVerticalRing;

            // Set the Events to use
            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
            ParticleEvents.AddEveryTimeEvent(ReduceSizeBasedOnLifetime);
            ParticleEvents.AddNormalizedTimedEvent(0.5f, GenerateSmokeParticle);

            Emitter.PositionData.Position = new Vector3(0, 50, 0);

            // Set the Fire Ring Settings
            InitialProperties.LifetimeMin = 0.1f;
            InitialProperties.LifetimeMax = 3.0f;
            InitialProperties.PositionMin = Vector3.Zero;
            InitialProperties.PositionMax = Vector3.Zero;
            InitialProperties.StartSizeMin = 15.0f;
            InitialProperties.StartSizeMax = 40.0f;
            InitialProperties.EndSizeMin = 2.0f;
            InitialProperties.EndSizeMax = 15.0f;
            InitialProperties.StartColorMin = Color.White;
            InitialProperties.StartColorMax = Color.White;
            InitialProperties.EndColorMin = Color.White;
            InitialProperties.EndColorMax = Color.White;
            InitialProperties.InterpolateBetweenMinAndMaxColors = false;
            InitialProperties.RotationMin = 0.0f;
            InitialProperties.RotationMax = MathHelper.TwoPi;
            InitialProperties.VelocityMin = new Vector3(-10, 15, -10);
            InitialProperties.VelocityMax = new Vector3(10, 30, 10);
            InitialProperties.AccelerationMin = Vector3.Zero;
            InitialProperties.AccelerationMax = Vector3.Zero;
            InitialProperties.RotationalVelocityMin = -MathHelper.TwoPi;
            InitialProperties.RotationalVelocityMax = MathHelper.TwoPi;

            mcSmokeParticleSystem.LoadEvents();
        }

        public void InitializeParticleFireOnVerticalRing(DefaultPointSpriteParticle cParticle)
        {
            Quaternion cBackup = Emitter.OrientationData.Orientation;
            Emitter.OrientationData.Orientation = Quaternion.Identity;
            InitializeParticleUsingInitialProperties(cParticle);
            Emitter.OrientationData.Orientation = cBackup;

            cParticle.Position = DPSFHelper.PointOnSphere(MathHelper.PiOver2, RandomNumber.Between(0, MathHelper.TwoPi), 40);
            cParticle.Position = Vector3.Transform(cParticle.Position, Emitter.OrientationData.Orientation);
            cParticle.Position += Emitter.PositionData.Position;
        }

        public void InitializeParticleFireOnHorizontalRing(DefaultPointSpriteParticle cParticle)
        {
            Quaternion cBackup = Emitter.OrientationData.Orientation;
            Emitter.OrientationData.Orientation = Quaternion.Identity;
            InitializeParticleUsingInitialProperties(cParticle);
            Emitter.OrientationData.Orientation = cBackup;

            cParticle.Position = DPSFHelper.PointOnSphere(RandomNumber.Between(0, MathHelper.TwoPi), 0, 100);
            cParticle.Position = Vector3.Transform(cParticle.Position, Emitter.OrientationData.Orientation);
            cParticle.Position += Emitter.PositionData.Position;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================
        public void ReduceSizeBasedOnLifetime(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Size = ((1.0f - cParticle.NormalizedElapsedTime) / 1.0f) * cParticle.StartSize;
        }

        public void GenerateSmokeParticle(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // If the Smoke Particle System is initialized
            if (mcSmokeParticleSystem != null && mcSmokeParticleSystem.IsInitialized())
            {
                // Only create a Smoke Particles some of the time
                if (RandomNumber.NextFloat() < mfAmountOfSmokeToRelease)
                {
                    // Create a new Smoke Particle at the same Position as this Fire Particle
                    DefaultPointSpriteParticle cSmokeParticle = new DefaultPointSpriteParticle();
                    mcSmokeParticleSystem.InitializeParticle(cSmokeParticle);
                    cSmokeParticle.Position = cParticle.Position;

                    // Add the Particle to the Smoke Particle System
                    mcSmokeParticleSystem.AddParticle(cSmokeParticle);
                }
            }
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================
        
        //===========================================================
        // Other Particle System Functions
        //===========================================================
        /// <summary>
        /// Sets how much Smoke the Fire should produce
        /// </summary>
        /// <param name="fNormalizedAmount">0.0 = No smoke, 1.0 = Max smoke</param>
        public void SetAmountOfSmokeToRelease(float fNormalizedAmount)
        {
            if (fNormalizedAmount < 0.0f)
            {
                fNormalizedAmount = 0.0f;
            }
            else if (fNormalizedAmount > 1.0f)
            {
                fNormalizedAmount = 1.0f;
            }

            mfAmountOfSmokeToRelease = fNormalizedAmount / 2.0f;
        }

        public float GetAmountOfSmokeBeingReleased()
        {
            return mfAmountOfSmokeToRelease * 2.0f;
        }



        public class SmokeRingParticleSystem : DefaultPointSpriteParticleSystem
        {
            public SmokeRingParticleSystem(Game cGame) : base(cGame) { }

            // Used to generate smoke coming off the ring of fire
            public void InitializeSmokeRingParticle(DefaultPointSpriteParticle cParticle)
            {
                cParticle.Lifetime = RandomNumber.Between(1.0f, 5.0f);

                cParticle.Position = new Vector3(0, 10, 0);
                cParticle.StartSize = RandomNumber.Next(10, 40);
                cParticle.EndSize = RandomNumber.Next(20, 60);
                cParticle.Size = cParticle.StartSize;
                cParticle.Color = Color.Black;
                cParticle.Rotation = RandomNumber.Between(0, MathHelper.TwoPi);

                cParticle.Velocity = new Vector3(RandomNumber.Next(0, 30), RandomNumber.Next(10, 30), RandomNumber.Next(-20, 10));
                cParticle.Acceleration = Vector3.Zero;
                cParticle.RotationalVelocity = RandomNumber.Between(-MathHelper.Pi, MathHelper.Pi);
            }

            protected override void SetEffectParameters()
            {
                base.SetEffectParameters();

                // Specify to not use the Color component when drawing (Texture color is not tinted)
                Effect.Parameters["xColorBlendAmount"].SetValue(0);
            }

            public void LoadEvents()
            {
                ParticleInitializationFunction = InitializeSmokeRingParticle;

                ParticleEvents.RemoveAllEvents();
                ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
                ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
                ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
                ParticleEvents.AddEveryTimeEvent(UpdateParticleSizeUsingLerp);
            }
        }
    }
}