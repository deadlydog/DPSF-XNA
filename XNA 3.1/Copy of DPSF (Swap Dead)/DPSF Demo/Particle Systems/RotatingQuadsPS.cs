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
    class RotatingQuadsParticleSystem: DefaultTexturedQuadParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RotatingQuadsParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================
        protected override void SetRenderState(RenderState cRenderState)
        {
            base.SetRenderState(cRenderState);
            cRenderState.CullMode = CullMode.None;
        }

        protected override void ResetRenderState(RenderState cRenderState)
        {
            base.ResetRenderState(cRenderState);
            cRenderState.CullMode = CullMode.CullCounterClockwiseFace;
        }

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager)
        {
            InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, 
                                                UpdateVertexProperties, "Textures/Bubble");

            Name = "Rotating Quads";
            LoadEvents();
        }

        public void LoadEvents()
        {
            ParticleInitializationFunction = InitializeParticleRotatingQuad;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);

            Emitter.ParticlesPerSecond = 1000;

            MaxNumberOfParticlesAllowed = 100;

            // Particle Initial Settings
            InitialProperties.LifetimeMin = 0.0f;
            InitialProperties.LifetimeMax = 0.0f;
            InitialProperties.PositionMin = new Vector3(-50, 0, -50);
            InitialProperties.PositionMax = new Vector3(50, 100, 50);
            InitialProperties.VelocityMin = Vector3.Zero;
            InitialProperties.VelocityMax = Vector3.Zero;
            InitialProperties.AccelerationMin = Vector3.Zero;
            InitialProperties.AccelerationMax = Vector3.Zero;
            InitialProperties.FrictionMin = 0.0f;
            InitialProperties.FrictionMax = 0.0f;
            InitialProperties.ExternalForceMin = Vector3.Zero;
            InitialProperties.ExternalForceMax = Vector3.Zero;
            InitialProperties.StartColorMin = Color.Black;
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
            InitialProperties.RotationalVelocityMin = new Vector3(MathHelper.PiOver4, MathHelper.PiOver4, MathHelper.PiOver4);
            InitialProperties.RotationalVelocityMax = new Vector3(MathHelper.TwoPi, MathHelper.TwoPi, MathHelper.TwoPi);
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

        public void InitializeParticleRotatingQuad(DefaultTexturedQuadParticle cParticle)
        {            
            // Set the type of Rotation this Particle should do
            switch (RandomNumber.Next(0, 5))
            {
                case 0:
                    InitialProperties.RotationalVelocityMin = new Vector3(MathHelper.PiOver4, 0, 0);
                    InitialProperties.RotationalVelocityMax = new Vector3(MathHelper.TwoPi, 0, 0);
                break;

                case 1:
                    InitialProperties.RotationalVelocityMin = new Vector3(0, MathHelper.PiOver4, 0);
                    InitialProperties.RotationalVelocityMax = new Vector3(0, MathHelper.TwoPi, 0);
                break;

                case 2:
                    InitialProperties.RotationalVelocityMin = new Vector3(0, 0, MathHelper.PiOver4);
                    InitialProperties.RotationalVelocityMax = new Vector3(0, 0, MathHelper.TwoPi);
                break;

                default:
                case 3:
                    InitialProperties.RotationalVelocityMin = Vector3.Zero;
                    InitialProperties.RotationalVelocityMax = new Vector3(MathHelper.TwoPi, MathHelper.TwoPi, MathHelper.TwoPi);
                    InitialProperties.InterpolateBetweenMinAndMaxRotationalVelocity = false;
                break;
            }

            InitializeParticleUsingInitialProperties(cParticle);
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================
        public void UpdateParticleToFaceCenter(DefaultTexturedQuadParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Normal = Vector3.Lerp(InitialProperties.PositionMin, InitialProperties.PositionMax, 0.5f) - cParticle.Position;
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================
        
        //===========================================================
        // Other Particle System Functions
        //===========================================================
    }
}