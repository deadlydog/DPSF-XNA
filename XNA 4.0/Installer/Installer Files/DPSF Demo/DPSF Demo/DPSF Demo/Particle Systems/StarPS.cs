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
    /// Create a new Particle System class that inherits from a Default DPSF Particle System.
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    class StarParticleSystem : DefaultSprite3DBillboardParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public StarParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        public bool mbHighlightAxis = true;
        public int miIntermittanceTimeMode = 1;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        protected override void InitializeRenderProperties()
        {
            base.InitializeRenderProperties();
            //RenderProperties.DepthStencilState = DepthStencilState.Default;
        }

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, "Textures/Particle");
            Name = "Star";
            LoadEvents();
        }

        public void InitializeParticleStar2D(DefaultSprite3DBillboardParticle cParticle)
        {
            InitializeParticleUsingInitialProperties(cParticle);

            // Add 7 more Particles, going in different 2D directions
            DefaultSprite3DBillboardParticle cParticle2 = new DefaultSprite3DBillboardParticle();
            DefaultSprite3DBillboardParticle cParticle3 = new DefaultSprite3DBillboardParticle();
            DefaultSprite3DBillboardParticle cParticle4 = new DefaultSprite3DBillboardParticle();
            DefaultSprite3DBillboardParticle cParticle5 = new DefaultSprite3DBillboardParticle();
            DefaultSprite3DBillboardParticle cParticle6 = new DefaultSprite3DBillboardParticle();
            DefaultSprite3DBillboardParticle cParticle7 = new DefaultSprite3DBillboardParticle();
            DefaultSprite3DBillboardParticle cParticle8 = new DefaultSprite3DBillboardParticle();

            cParticle2.CopyFrom(cParticle);
            cParticle3.CopyFrom(cParticle);
            cParticle4.CopyFrom(cParticle);
            cParticle5.CopyFrom(cParticle);
            cParticle6.CopyFrom(cParticle);
            cParticle7.CopyFrom(cParticle);
            cParticle8.CopyFrom(cParticle);

            cParticle2.Velocity = new Vector3(50, 50, 0);
            cParticle3.Velocity = new Vector3(100, 0, 0);
            cParticle4.Velocity = new Vector3(50, -50, 0);
            cParticle5.Velocity = new Vector3(0, -100, 0);
            cParticle6.Velocity = new Vector3(-50, -50, 0);
            cParticle7.Velocity = new Vector3(-100, 0, 0);
            cParticle8.Velocity = new Vector3(-50, 50, 0);

            cParticle2.Velocity = Vector3.Transform(cParticle2.Velocity, Emitter.OrientationData.Orientation);
            cParticle3.Velocity = Vector3.Transform(cParticle3.Velocity, Emitter.OrientationData.Orientation);
            cParticle4.Velocity = Vector3.Transform(cParticle4.Velocity, Emitter.OrientationData.Orientation);
            cParticle5.Velocity = Vector3.Transform(cParticle5.Velocity, Emitter.OrientationData.Orientation);
            cParticle6.Velocity = Vector3.Transform(cParticle6.Velocity, Emitter.OrientationData.Orientation);
            cParticle7.Velocity = Vector3.Transform(cParticle7.Velocity, Emitter.OrientationData.Orientation);
            cParticle8.Velocity = Vector3.Transform(cParticle8.Velocity, Emitter.OrientationData.Orientation);

            // If the axis should be distinguishable
            if (mbHighlightAxis)
            {
                cParticle.Color = Color.Green;
                cParticle3.Color = Color.Red;
            }

            AddParticle(cParticle2);
            AddParticle(cParticle3);
            AddParticle(cParticle4);
            AddParticle(cParticle5);
            AddParticle(cParticle6);
            AddParticle(cParticle7);
            AddParticle(cParticle8);
        }

        public void InitializeParticleStar3D(DefaultSprite3DBillboardParticle cParticle)
        {
            InitializeParticleStar2D(cParticle);

            // Add 6 more Particles, going in different 3D directions
            DefaultSprite3DBillboardParticle cParticle2 = new DefaultSprite3DBillboardParticle();
            DefaultSprite3DBillboardParticle cParticle3 = new DefaultSprite3DBillboardParticle();
            DefaultSprite3DBillboardParticle cParticle4 = new DefaultSprite3DBillboardParticle();
            DefaultSprite3DBillboardParticle cParticle5 = new DefaultSprite3DBillboardParticle();
            DefaultSprite3DBillboardParticle cParticle6 = new DefaultSprite3DBillboardParticle();
            DefaultSprite3DBillboardParticle cParticle7 = new DefaultSprite3DBillboardParticle();

            cParticle.Color = Color.White;

            cParticle2.CopyFrom(cParticle);
            cParticle3.CopyFrom(cParticle);
            cParticle4.CopyFrom(cParticle);
            cParticle5.CopyFrom(cParticle);
            cParticle6.CopyFrom(cParticle);
            cParticle7.CopyFrom(cParticle);

            cParticle2.Velocity = new Vector3(0, 50, 50);
            cParticle3.Velocity = new Vector3(0, 0, 100);
            cParticle4.Velocity = new Vector3(0, -50, 50);
            cParticle5.Velocity = new Vector3(0, -50, -50);
            cParticle6.Velocity = new Vector3(-0, 0, -100);
            cParticle7.Velocity = new Vector3(0, 50, -50);

            cParticle2.Velocity = Vector3.Transform(cParticle2.Velocity, Emitter.OrientationData.Orientation);
            cParticle3.Velocity = Vector3.Transform(cParticle3.Velocity, Emitter.OrientationData.Orientation);
            cParticle4.Velocity = Vector3.Transform(cParticle4.Velocity, Emitter.OrientationData.Orientation);
            cParticle5.Velocity = Vector3.Transform(cParticle5.Velocity, Emitter.OrientationData.Orientation);
            cParticle6.Velocity = Vector3.Transform(cParticle6.Velocity, Emitter.OrientationData.Orientation);
            cParticle7.Velocity = Vector3.Transform(cParticle7.Velocity, Emitter.OrientationData.Orientation);

            // If the axis should be distinguishable
            if (mbHighlightAxis)
            {
                cParticle.Color = Color.Green;
                cParticle3.Color = Color.Blue;
            }

            AddParticle(cParticle2);
            AddParticle(cParticle3);
            AddParticle(cParticle4);
            AddParticle(cParticle5);
            AddParticle(cParticle6);
            AddParticle(cParticle7);
        }

        public void LoadEvents()
        {
            ParticleInitializationFunction = InitializeParticleStar2D;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration, 500);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);

            // These textures contain both opaque and semi-transparent portions, so in order to get them drawn correctly (so that
            // particles further away aren't drawn overtop of closer particles), we must sort the particles so that they are drawn 
            // in the order of farthest from the camera to closest to the camera.
            // These operations are expensive, so they should only be used if it is important that particles in front hide the particles behind them.
            ParticleEvents.AddEveryTimeEvent(UpdateParticleDistanceFromCameraSquared);
            ParticleSystemEvents.AddEveryTimeEvent(UpdateParticleSystemToSortParticlesByDistanceFromCamera);

            ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;

            Emitter.PositionData.Position = new Vector3(0, 50, 0);
            Emitter.ParticlesPerSecond = 60;

            InitialProperties.LifetimeMin = 1.0f;
            InitialProperties.LifetimeMax = 1.0f;
            InitialProperties.PositionMin = new Vector3(0, 0, 0);
            InitialProperties.PositionMax = new Vector3(0, 0, 0);
            InitialProperties.StartSizeMin = 20.0f;
            InitialProperties.StartSizeMax = 20.0f;
            InitialProperties.EndSizeMin = 20.0f;
            InitialProperties.EndSizeMax = 20.0f;
            InitialProperties.StartColorMin = Color.White;
            InitialProperties.StartColorMax = Color.White;
            InitialProperties.EndColorMin = Color.White;
            InitialProperties.EndColorMax = Color.White;
            InitialProperties.InterpolateBetweenMinAndMaxColors = false;
            InitialProperties.VelocityMin = new Vector3(0, 100, 0);
            InitialProperties.VelocityMax = new Vector3(0, 100, 0);
            InitialProperties.AccelerationMin = Vector3.Zero;
            InitialProperties.AccelerationMax = Vector3.Zero;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        //===========================================================
        // Particle System Update Functions
        //===========================================================
        protected void UpdateParticleSystemReverseEmitterVelocities(float fElapsedTimeInSeconds)
        {
            Emitter.PositionData.Velocity *= -1;
            Emitter.OrientationData.RotationalVelocity *= -1;
        }

        //===========================================================
        // Other Particle System Functions
        //===========================================================
        public void LoadSlowWiggle()
        {
            Emitter.PositionData.Velocity = new Vector3(10, 0, 0);
            Emitter.OrientationData.RotationalVelocity = Vector3.Zero;
            ParticleSystemEvents.RemoveAllTimedAndNormalizedTimedEvents();
            ParticleSystemEvents.AddTimedEvent(0.1f, UpdateParticleSystemReverseEmitterVelocities);
            ParticleSystemEvents.LifetimeData.Lifetime = 0.1f;
        }

        public void LoadMediumWiggle()
        {
            Emitter.PositionData.Velocity = new Vector3(100, 0, 0);
            Emitter.OrientationData.RotationalVelocity = Vector3.Zero;
            ParticleSystemEvents.RemoveAllTimedAndNormalizedTimedEvents();
            ParticleSystemEvents.AddTimedEvent(0.1f, UpdateParticleSystemReverseEmitterVelocities);
            ParticleSystemEvents.LifetimeData.Lifetime = 0.1f;
        }

        public void LoadFastWiggle()
        {
            Emitter.PositionData.Velocity = new Vector3(1000, 0, 0);
            Emitter.OrientationData.RotationalVelocity = Vector3.Zero;
            ParticleSystemEvents.RemoveAllTimedAndNormalizedTimedEvents();
            ParticleSystemEvents.AddTimedEvent(0.1f, UpdateParticleSystemReverseEmitterVelocities);
            ParticleSystemEvents.LifetimeData.Lifetime = 0.1f;
        }

        public void LoadSlowRotationalWiggle()
        {
            Emitter.PositionData.Velocity = Vector3.Zero;
            Emitter.OrientationData.RotationalVelocity = new Vector3(0, 0, 0.2f);
            ParticleSystemEvents.RemoveAllTimedAndNormalizedTimedEvents();
            ParticleSystemEvents.AddTimedEvent(0.1f, UpdateParticleSystemReverseEmitterVelocities);
            ParticleSystemEvents.LifetimeData.Lifetime = 0.1f;
        }

        public void LoadMediumRotationalWiggle()
        {
            Emitter.PositionData.Velocity = Vector3.Zero;
            Emitter.OrientationData.RotationalVelocity = new Vector3(0, 0, 1);
            ParticleSystemEvents.RemoveAllTimedAndNormalizedTimedEvents();
            ParticleSystemEvents.AddTimedEvent(0.1f, UpdateParticleSystemReverseEmitterVelocities);
            ParticleSystemEvents.LifetimeData.Lifetime = 0.1f;
        }

        public void LoadFastRotationalWiggle()
        {
            Emitter.PositionData.Velocity = Vector3.Zero;
            Emitter.OrientationData.RotationalVelocity = new Vector3(0, 0, 5);
            ParticleSystemEvents.RemoveAllTimedAndNormalizedTimedEvents();
            ParticleSystemEvents.AddTimedEvent(0.1f, UpdateParticleSystemReverseEmitterVelocities);
            ParticleSystemEvents.LifetimeData.Lifetime = 0.1f;
        }

        public void ToggleEmitterIntermittance()
        {
            ParticleSystemEvents.RemoveAllTimedAndNormalizedTimedEvents();
            ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemEmitParticlesAutomaticallyOn);

            miIntermittanceTimeMode++;
            if (miIntermittanceTimeMode > 11)
            {
                miIntermittanceTimeMode = 1;
            }

            switch (miIntermittanceTimeMode)
            {
                default:
                case 1:
                    Emitter.EmitParticlesAutomatically = true;
                    ParticleSystemEvents.RemoveAllTimedAndNormalizedTimedEvents();
                    break;

                case 2:
                    ParticleSystemEvents.AddTimedEvent(0.05f, UpdateParticleSystemEmitParticlesAutomaticallyOff);
                    ParticleSystemEvents.LifetimeData.Lifetime = 0.1f;
                    break;

                case 3:
                    ParticleSystemEvents.AddTimedEvent(0.1f, UpdateParticleSystemEmitParticlesAutomaticallyOff);
                    ParticleSystemEvents.LifetimeData.Lifetime = 0.2f;
                    break;

                case 4:
                    ParticleSystemEvents.AddTimedEvent(0.25f, UpdateParticleSystemEmitParticlesAutomaticallyOff);
                    ParticleSystemEvents.LifetimeData.Lifetime = 0.5f;
                    break;

                case 5:
                    ParticleSystemEvents.AddTimedEvent(0.5f, UpdateParticleSystemEmitParticlesAutomaticallyOff);
                    ParticleSystemEvents.LifetimeData.Lifetime = 1.0f;
                    break;

                case 6:
                    ParticleSystemEvents.AddTimedEvent(1.0f, UpdateParticleSystemEmitParticlesAutomaticallyOff);
                    ParticleSystemEvents.LifetimeData.Lifetime = 2.0f;
                    break;

                case 7:
                    ParticleSystemEvents.AddTimedEvent(0.05f, UpdateParticleSystemEmitParticlesAutomaticallyOff);
                    ParticleSystemEvents.LifetimeData.Lifetime = 0.5f;
                    break;

                case 8:
                    ParticleSystemEvents.AddTimedEvent(0.4f, UpdateParticleSystemEmitParticlesAutomaticallyOff);
                    ParticleSystemEvents.LifetimeData.Lifetime = 0.6f;
                    break;

                case 9:
                    ParticleSystemEvents.AddTimedEvent(0.25f, UpdateParticleSystemEmitParticlesAutomaticallyOff);
                    ParticleSystemEvents.LifetimeData.Lifetime = 0.3f;
                    break;

                case 10:
                    ParticleSystemEvents.AddTimedEvent(0.1f, UpdateParticleSystemEmitParticlesAutomaticallyOff);
                    ParticleSystemEvents.AddTimedEvent(0.2f, UpdateParticleSystemEmitParticlesAutomaticallyOn);
                    ParticleSystemEvents.AddTimedEvent(0.5f, UpdateParticleSystemEmitParticlesAutomaticallyOff);
                    ParticleSystemEvents.LifetimeData.Lifetime = 0.6f;
                    break;

                case 11:
                    ParticleSystemEvents.AddTimedEvent(0.2f, UpdateParticleSystemEmitParticlesAutomaticallyOff);
                    ParticleSystemEvents.AddTimedEvent(0.25f, UpdateParticleSystemEmitParticlesAutomaticallyOn);
                    ParticleSystemEvents.AddTimedEvent(0.3f, UpdateParticleSystemEmitParticlesAutomaticallyOff);
                    ParticleSystemEvents.LifetimeData.Lifetime = 0.35f;
                    break;
            }
        }
    }
}