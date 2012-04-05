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
    /// Create a new Particle System class that inherits from a Default DPSF Particle System.
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    class FountainParticleSystem : DefaultSprite3DBillboardParticleSystem
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
        private bool mbUseAdditiveBlending = false;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        protected override void InitializeRenderProperties()
        {
            base.InitializeRenderProperties();

            // Turn on depth buffer writing so particles are drawn in the correct order.
            // This can cause clipping problems with some textures.
            RenderProperties.DepthStencilState = DepthStencilState.Default;
        }

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, "Textures/Bubble", cSpriteBatch);
            LoadFountainEvents();
            Emitter.ParticlesPerSecond = 100;
            Name = "Fountain";
        }

        public void LoadFountainEvents()
        {
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingExternalForce, 400);
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration, 500);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndQuickFadeOut, 100);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleBounceOffFloor, 450);

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
            InitialProperties.RotationMin = 0;
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
        protected void UpdateParticleBounceOffFloor(DefaultSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
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

        public void MakeParticlesShrink()
        {
            this.ParticleEvents.RemoveEveryTimeEvents(UpdateParticleWidthAndHeightUsingLerp);
            this.ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
        }

        public void MakeParticlesNotShrink()
        {
            this.ParticleEvents.RemoveEveryTimeEvents(UpdateParticleWidthAndHeightUsingLerp);
        }

        public void MakeParticlesBounceOffFloor()
        {
            this.ParticleEvents.RemoveEveryTimeEvents(UpdateParticleBounceOffFloor);
            this.ParticleEvents.AddEveryTimeEvent(UpdateParticleBounceOffFloor);
        }

        public void MakeParticlesNotBounceOffFloor()
        {
            this.ParticleEvents.RemoveEveryTimeEvents(UpdateParticleBounceOffFloor);
        }

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
    }
}