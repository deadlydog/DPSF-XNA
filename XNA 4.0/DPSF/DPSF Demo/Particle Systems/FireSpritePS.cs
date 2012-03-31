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
    class FireSpriteParticleSystem : DefaultSprite3DBillboardParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FireSpriteParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        private bool mbUseAdditiveBlending = false;
        private float mfAmountOfSmokeToRelease = 0f;
        public SmokeRingSpriteParticleSystem mcSmokeParticleSystem = null;

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

        protected override void InitializeRenderProperties()
        {
            base.InitializeRenderProperties();
            mbUseAdditiveBlending = false;
            ToggleAdditiveBlending();

            AlphaTestEffect effect = this.Effect as AlphaTestEffect;
            if (effect != null)
            {
                effect.VertexColorEnabled = false;
            }
        }

        protected override void AfterInitialize()
        {
            mcSmokeParticleSystem = new SmokeRingSpriteParticleSystem(this.Game);

            // Determine which Sprite Batch object to pass into the Smoke Particle System.
            // If a custom one was provided to the Fire Particle System, we want to use it, otherwise we want
            // the Smoke Particle System to use its own Sprite Batch, so we just pass in null.
            SpriteBatch spriteBatch = this.UsingExternalSpriteBatchToDrawParticles ? this.SpriteBatch : null;

            // Initialize the Smoke Particle System
            mcSmokeParticleSystem.AutoInitialize(this.GraphicsDevice, this.ContentManager, spriteBatch);
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
            if (mcSmokeParticleSystem.IsInitialized)
            {
                // Update the Smoke Particle System manually
                mcSmokeParticleSystem.CameraPosition = this.CameraPosition;
                mcSmokeParticleSystem.Update(fElapsedTimeInSeconds);
            }
        }

        protected override void AfterDraw()
        {
            // Set the World, View, and Projection matrices so the Smoke Particle System knows how to draw the particles on screen properly
            mcSmokeParticleSystem.SetWorldViewProjectionMatrices(World, View, Projection);

            // If the Smoke Particle System is Initialized
            if (mcSmokeParticleSystem.IsInitialized)
            {
                // Draw the Smoke Particles manually
                mcSmokeParticleSystem.Draw();
            }
        }

        public override int TotalNumberOfActiveParticles { get { return base.TotalNumberOfActiveParticles + mcSmokeParticleSystem.TotalNumberOfActiveParticles; } }
        public override int TotalNumberOfParticlesAllocatedInMemory { get { return base.TotalNumberOfParticlesAllocatedInMemory + mcSmokeParticleSystem.TotalNumberOfParticlesAllocatedInMemory; } }
        public override int TotalNumberOfParticlesBeingDrawn { get { return base.TotalNumberOfParticlesBeingDrawn + mcSmokeParticleSystem.TotalNumberOfParticlesBeingDrawn; } }

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, "Textures/Fire", cSpriteBatch);
            Name = "Fire and Smoke (Sprites)";
            LoadFireRingEvents();
            Emitter.ParticlesPerSecond = 500;
            SetAmountOfSmokeToRelease(0.25f);
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
            InitialProperties.StartSizeMin = 25.0f;
            InitialProperties.StartSizeMax = 50.0f;
            InitialProperties.EndSizeMin = 4.0f;
            InitialProperties.EndSizeMax = 20.0f;
            InitialProperties.StartColorMin = Color.White;
            InitialProperties.StartColorMax = Color.White;
            InitialProperties.EndColorMin = Color.White;
            InitialProperties.EndColorMax = Color.White;
            InitialProperties.InterpolateBetweenMinAndMaxColors = false;
            InitialProperties.RotationMin = 0;
            InitialProperties.RotationMax = MathHelper.TwoPi;
            InitialProperties.VelocityMin = new Vector3(-10, 15, -10);
            InitialProperties.VelocityMax = new Vector3(10, 30, 10);
            InitialProperties.AccelerationMin = Vector3.Zero;
            InitialProperties.AccelerationMax = Vector3.Zero;
            InitialProperties.RotationalVelocityMin = -MathHelper.TwoPi;
            InitialProperties.RotationalVelocityMax = MathHelper.TwoPi;

            mcSmokeParticleSystem.LoadEvents();
        }

        public void InitializeParticleFireOnVerticalRing(DefaultSprite3DBillboardParticle cParticle)
        {
            Quaternion cBackup = Emitter.OrientationData.Orientation;
            Emitter.OrientationData.Orientation = Quaternion.Identity;
            InitializeParticleUsingInitialProperties(cParticle);
            Emitter.OrientationData.Orientation = cBackup;

            cParticle.Position = DPSFHelper.PointOnSphere(MathHelper.PiOver2, RandomNumber.Between(0, MathHelper.TwoPi), 40);
            cParticle.Position = Vector3.Transform(cParticle.Position, Emitter.OrientationData.Orientation);
            cParticle.Position += Emitter.PositionData.Position;
        }

        public void InitializeParticleFireOnHorizontalRing(DefaultSprite3DBillboardParticle cParticle)
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
        protected void ReduceSizeBasedOnLifetime(DefaultSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Size = ((1.0f - cParticle.NormalizedElapsedTime) / 1.0f) * cParticle.StartSize;
        }

        protected void GenerateSmokeParticle(DefaultSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
        {
            // If the Smoke Particle System is initialized
            if (mcSmokeParticleSystem != null && mcSmokeParticleSystem.IsInitialized)
            {
                // Only create a Smoke Particles some of the time
                if (RandomNumber.NextFloat() < mfAmountOfSmokeToRelease)
                {
                    // Create a new Smoke Particle at the same Position as this Fire Particle
                    DefaultSprite3DBillboardParticle cSmokeParticle = new DefaultSprite3DBillboardParticle();
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

        public void ToggleAdditiveBlending()
        {
            // Toggle Additive Blending on/off
            mbUseAdditiveBlending = !mbUseAdditiveBlending;

            // If Additive Blending should be used
            if (mbUseAdditiveBlending)
            {
                // Turn it on
                RenderProperties.BlendState = BlendState.Additive;
            }
            else
            {
                // Turn off Additive Blending
                RenderProperties.BlendState = BlendState.AlphaBlend;
            }
        }

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


#if (WINDOWS)
        [Serializable]
#endif
        public class SmokeRingSpriteParticleSystem : DefaultSprite3DBillboardParticleSystem
        {
            public SmokeRingSpriteParticleSystem(Game cGame) : base(cGame) { }

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

            public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
            {
                InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, "Textures/Smoke", cSpriteBatch);
                LoadEvents();
            }

            protected override void InitializeRenderProperties()
            {
                base.InitializeRenderProperties();

				// Turn coloring tinting off, so that we just use the original texture as is
                AlphaTestEffect effect = this.Effect as AlphaTestEffect;
                if (effect != null)
                {
                    effect.VertexColorEnabled = false;
                }
            }

            public void LoadEvents()
            {
                ParticleInitializationFunction = InitializeSmokeRingParticle;

                ParticleEvents.RemoveAllEvents();
                ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
                ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
                ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
                ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
            }

            // Used to generate smoke coming off the ring of fire
            public void InitializeSmokeRingParticle(DefaultSprite3DBillboardParticle cParticle)
            {
                cParticle.Lifetime = RandomNumber.Between(1.0f, 5.0f);

                cParticle.Position = new Vector3(0, 10, 0);
                cParticle.StartSize = RandomNumber.Next(10, 40);
                cParticle.EndSize = RandomNumber.Next(20, 60);
                cParticle.Size = cParticle.StartSize;
                cParticle.Color = Color.White; //Color.Gray;
                cParticle.Rotation = RandomNumber.Between(0, MathHelper.TwoPi);

                cParticle.Velocity = new Vector3(RandomNumber.Next(0, 30), RandomNumber.Next(10, 30), RandomNumber.Next(-20, 10));
                cParticle.Acceleration = Vector3.Zero;
                cParticle.RotationalVelocity = RandomNumber.Between(-MathHelper.Pi, MathHelper.Pi);
            }
        }
    }
}