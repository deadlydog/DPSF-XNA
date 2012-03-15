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
    class RandomParticleSystem : DefaultPointSpriteParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RandomParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        private float mfAngle = 0.0f;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        //===========================================================
        // Initialization Functions
        //===========================================================

        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializePointSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, 
                                                UpdateVertexProperties, "Textures/Spark");
            Name = "Random";
			Emitter.ParticlesPerSecond = 400;
            LoadRandomEvents();
        }

        public void LoadRandomEvents()
        {
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);

            Emitter.PositionData.Position = new Vector3(0, 50, 0);

            InitialProperties.LifetimeMin = 2.0f;
            InitialProperties.LifetimeMax = 2.0f;
            InitialProperties.PositionMin = Vector3.Zero;
            InitialProperties.PositionMax = Vector3.Zero;
            InitialProperties.StartSizeMin = 20.0f;
            InitialProperties.StartSizeMax = 20.0f;
            InitialProperties.EndSizeMin = 20.0f;
            InitialProperties.EndSizeMax = 20.0f;
            InitialProperties.StartColorMin = Color.Red;
            InitialProperties.StartColorMax = Color.DarkRed;
            InitialProperties.EndColorMin = Color.Black;
            InitialProperties.EndColorMax = Color.White;
            InitialProperties.InterpolateBetweenMinAndMaxColors = false;
            InitialProperties.VelocityMin = new Vector3(-50, -50, -50);
            InitialProperties.VelocityMax = new Vector3(50, 50, 50);
            InitialProperties.AccelerationMin = Vector3.Zero;
            InitialProperties.AccelerationMax = Vector3.Zero;
        }

		public void InitializeParticleSpiral(DefaultPointSpriteParticle cParticle)
		{
			InitializeParticleUsingInitialProperties(cParticle);

			// Have the new particles' velocities form a circle pattern around position (0, 50, 0)
			mfAngle += 0.1f;
			Vector3 sDirection = DPSFHelper.PointOnSphere(-MathHelper.PiOver2, mfAngle, 70, new Vector3(0, 50, 0));
			cParticle.Velocity = sDirection - cParticle.Position;
		}

        public void LoadRandomSpiralEvents()
        {
            ParticleInitializationFunction = InitializeParticleSpiral;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleSizeUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);

            Emitter.PositionData.Position = new Vector3(0, 50, 0);

            InitialProperties.LifetimeMin = 2.0f;
            InitialProperties.LifetimeMax = 2.0f;
            InitialProperties.PositionMin = Vector3.Zero;
            InitialProperties.PositionMax = Vector3.Zero;
            InitialProperties.StartSizeMin = 20.0f;
            InitialProperties.StartSizeMax = 20.0f;
            InitialProperties.EndSizeMin = 20.0f;
            InitialProperties.EndSizeMax = 20.0f;
            InitialProperties.StartColorMin = Color.Red;
            InitialProperties.StartColorMax = Color.DarkRed;
            InitialProperties.EndColorMin = Color.Black;
            InitialProperties.EndColorMax = Color.White;
            InitialProperties.VelocityMin = new Vector3(50, 0, 0);
            InitialProperties.VelocityMax = new Vector3(50, 0, 0);
            InitialProperties.AccelerationMin = Vector3.Zero;
            InitialProperties.AccelerationMax = Vector3.Zero;
        }

        public void LoadTubeEvents()
        {
            LoadRandomSpiralEvents();
            Emitter.PositionData.Position = new Vector3(50, 100, -200);
            Emitter.ParticlesPerSecond = 1000;
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
