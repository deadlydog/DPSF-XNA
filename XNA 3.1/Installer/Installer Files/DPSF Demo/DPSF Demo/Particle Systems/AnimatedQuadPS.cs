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
    class AnimatedQuadParticleSystem : DefaultAnimatedTexturedQuadParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AnimatedQuadParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        Animations mcAnimation = null;
        float mfMinTimeBetweenAnimationImages = 0.02f;  // Fastest speed
        float mfMaxTimeBetweenAnimationImages = 0.1f;   // Slowest speed
        int miButterflyMaxSpeed = 35;

        // The box that the Butterflies must stay contained within
        Vector3 msBoxMin = new Vector3(-100, 0, -100);
        Vector3 msBoxMax = new Vector3(100, 100, 100);

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        protected override void AfterInitialize()
        {
			base.AfterInitialize();

            // Setup the Animation
            mcAnimation = new Animations();

            // The Order of the Picture IDs to make up the Animation
            int[] iaAnimationOrder = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            // Create the Pictures and Animation
            mcAnimation.CreatePicturesFromTileSet(16, 4, new Rectangle(0, 0, 128, 128));
            int iAnimationID = mcAnimation.CreateAnimation(iaAnimationOrder, mfMaxTimeBetweenAnimationImages, 0);

            // Set the Animation to use
            mcAnimation.CurrentAnimationID = iAnimationID;
        }

        protected override void AfterDestroy()
        {
			base.AfterDestroy();
            mcAnimation = null;
        }

        protected override void SetRenderState(RenderState cRenderState)
        {
            base.SetRenderState(cRenderState);
            cRenderState.CullMode = CullMode.None;
            cRenderState.DepthBufferWriteEnable = true;
        }

        protected override void ResetRenderState(RenderState cRenderState)
        {
            base.ResetRenderState(cRenderState);
            cRenderState.CullMode = CullMode.CullCounterClockwiseFace;
        }

        protected override void SetEffectParameters()
        {
            base.SetEffectParameters();

            // Show only the Textures Color (do not blend with Particle Color)
            Effect.Parameters["xColorBlendAmount"].SetValue(0.0f);
        }

        //===========================================================
        // Initialization Functions
        //===========================================================

        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, 
                                            UpdateVertexProperties, "Textures/AnimatedButterfly");
            Name = "Animated Textured Quads";
            LoadEvents();
        }
        
        public void LoadEvents()
        {
            ParticleInitializationFunction = InitializeParticleAnimated;
            
            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleAnimationAndTextureCoordinates);
            ParticleEvents.AddEveryTimeEvent(KeepParticleInBox, 100);
            ParticleEvents.AddEveryTimeEvent(ChangeDirectionRandomly);

            MaxNumberOfParticlesAllowed = 20;
            Emitter.ParticlesPerSecond = 5;

            // Particle Initial Settings
            InitialProperties.LifetimeMin = 0.0f;
            InitialProperties.LifetimeMax = 0.0f;
            InitialProperties.PositionMin = msBoxMin;
            InitialProperties.PositionMax = msBoxMax;
            InitialProperties.VelocityMin = Vector3.Zero;
            InitialProperties.VelocityMax = Vector3.Zero;
            InitialProperties.AccelerationMin = Vector3.Zero;
            InitialProperties.AccelerationMax = Vector3.Zero;
            InitialProperties.FrictionMin = 0.0f;
            InitialProperties.FrictionMax = 0.0f;
            InitialProperties.ExternalForceMin = Vector3.Zero;
            InitialProperties.ExternalForceMax = Vector3.Zero;
            InitialProperties.StartColorMin = Color.White;
            InitialProperties.StartColorMax = Color.White;
            InitialProperties.EndColorMin = Color.White;
            InitialProperties.EndColorMax = Color.White;
            InitialProperties.InterpolateBetweenMinAndMaxPosition = false;
            InitialProperties.InterpolateBetweenMinAndMaxVelocity = false;
            InitialProperties.InterpolateBetweenMinAndMaxAcceleration = false;
            InitialProperties.InterpolateBetweenMinAndMaxExternalForce = false;
            InitialProperties.InterpolateBetweenMinAndMaxColors = false;

            // Quad Particle Initial Settings
            InitialProperties.RotationMin = Vector3.Zero;
            InitialProperties.RotationMax = Vector3.Zero;
            InitialProperties.RotationalVelocityMin = Vector3.Zero;
            InitialProperties.RotationalVelocityMax = Vector3.Zero;
            InitialProperties.RotationalAccelerationMin = Vector3.Zero;
            InitialProperties.RotationalAccelerationMax = Vector3.Zero;
            InitialProperties.StartWidthMin = 20.0f;
            InitialProperties.StartWidthMax = 20.0f;
            InitialProperties.StartHeightMin = 20.0f;
            InitialProperties.StartHeightMax = 20.0f;
            InitialProperties.EndWidthMin = 20.0f;
            InitialProperties.EndWidthMax = 20.0f;
            InitialProperties.EndHeightMin = 20.0f;
            InitialProperties.EndHeightMax = 20.0f;
            InitialProperties.InterpolateBetweenMinAndMaxRotation = false;
            InitialProperties.InterpolateBetweenMinAndMaxRotationalVelocity = false;
            InitialProperties.InterpolateBetweenMinAndMaxRotationalAcceleration = false;
        }

        public void InitializeParticleAnimated(DefaultAnimatedTexturedQuadParticle cParticle)
        {
            InitializeParticleUsingInitialProperties(cParticle);
            cParticle.Animation.CopyFrom(mcAnimation);

            ChangeDirectionRandomly(cParticle, -1);
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        protected void KeepParticleInBox(DefaultAnimatedTexturedQuadParticle cParticle, float fElapsedTimeInSeconds)
        {
            bool bDirectionChanged = false;

            // If the Particle is heading outside of the box
            if ((cParticle.Position.X > msBoxMax.X && cParticle.Velocity.X > 0) ||
                (cParticle.Position.X < msBoxMin.X && cParticle.Velocity.X < 0))
            {
                // Reverse it's velocity to keep it inside the box
                cParticle.Velocity.X *= -1;
                bDirectionChanged = true;
            }

            // If the Particle is heading outside of the box
            if ((cParticle.Position.Y > msBoxMax.Y && cParticle.Velocity.Y > 0) ||
                (cParticle.Position.Y < msBoxMin.Y && cParticle.Velocity.Y < 0))
            {
                // Reverse it's velocity to keep it inside the box
                cParticle.Velocity.Y *= -1;
                bDirectionChanged = true;
            }

            // If the Particle is heading outside of the box
            if ((cParticle.Position.Z > msBoxMax.Z && cParticle.Velocity.Z > 0) ||
                (cParticle.Position.Z < msBoxMin.Z && cParticle.Velocity.Z < 0))
            {
                // Reverse it's velocity to keep it inside the box
                cParticle.Velocity.Z *= -1;
                bDirectionChanged = true;
            }

            // If the Direction was changed
            if (bDirectionChanged)
            {
                // Calculate the new Direction the Butterfly should face
                MakeButterflyFaceProperDirectionAndAdjustAnimationSpeed(cParticle);
            }
        }

        protected void ChangeDirectionRandomly(DefaultAnimatedTexturedQuadParticle cParticle, float fElapsedTimeInSeconds)
        {
            // If we should pick a new Direction (happens randomly) (-1 is specified when initializing new particles)
            float fClamped = MathHelper.Clamp(fElapsedTimeInSeconds, -1f, 0.01f);
            if (RandomNumber.NextFloat() < fClamped || fElapsedTimeInSeconds < 0)
            {
                // Calculate a new Velocity direction
                cParticle.Velocity = new Vector3(RandomNumber.Next(-miButterflyMaxSpeed, miButterflyMaxSpeed),
                                                  RandomNumber.Next(-miButterflyMaxSpeed, miButterflyMaxSpeed),
                                                  RandomNumber.Next(-miButterflyMaxSpeed, miButterflyMaxSpeed));

                // Calculate the new Direction the Butterfly should face
                MakeButterflyFaceProperDirectionAndAdjustAnimationSpeed(cParticle);
            }
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        //===========================================================
        // Other Particle System Functions
        //===========================================================
        private void MakeButterflyFaceProperDirectionAndAdjustAnimationSpeed(DefaultAnimatedTexturedQuadParticle cParticle)
        {
            // Calculate the new Direction the Butterfly should face
            Vector3 sFacingDirection = Vector3.Cross(cParticle.Velocity, Vector3.Up);
            cParticle.Orientation = Orientation3D.GetQuaternionWithOrientation(sFacingDirection, Vector3.Up);

            // Make the Butterfly animation go faster based on how fast it is moving upwards
            float fNormalizedYVelocity = (cParticle.Velocity.Y + miButterflyMaxSpeed) / (miButterflyMaxSpeed * 2);
            float fNewAnimationTime = MathHelper.Lerp(mfMaxTimeBetweenAnimationImages, mfMinTimeBetweenAnimationImages, fNormalizedYVelocity);
            cParticle.Animation.CurrentAnimationsPictureRotationTime = fNewAnimationTime;
        }
    }
}
