#region Using Statements
using System;
using System.Collections.Generic;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace DPSFViewer.ParticleSystems
{
    /// <summary>
    /// Create a new Particle System class that inherits from a Default DPSF Particle System.
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    class RandomParticleSystem : DefaultSprite3DBillboardParticleSystem
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
            InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, "Textures/Spark");
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

        public void InitializeParticleSpiral(DefaultSprite3DBillboardParticle cParticle)
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
            ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
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










//    /// <summary>
//    /// Create a new Particle System class that inherits from a Default DPSF Particle System.
//    /// </summary>
//#if (WINDOWS)
//    [Serializable]
//#endif
//    class RandomParticleSystem : DefaultTexturedQuadParticleSystem
//    {
//        /// <summary>
//        /// Constructor
//        /// </summary>
//        public RandomParticleSystem(Game cGame) : base(cGame) { }

//        //===========================================================
//        // Structures and Variables
//        //===========================================================
//        private float mfAngle = 0.0f;

//        //===========================================================
//        // Overridden Particle System Functions
//        //===========================================================

//        //===========================================================
//        // Initialization Functions
//        //===========================================================

//        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
//        {
//            InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, UpdateVertexProperties, "Textures/Spark");
//            Name = "Random";
//            Emitter.ParticlesPerSecond = 400;
//            LoadRandomEvents();
//        }

//        public void LoadRandomEvents()
//        {
//            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;

//            ParticleEvents.RemoveAllEvents();
//            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
//            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
//            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);
//            ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera);

//            Emitter.PositionData.Position = new Vector3(0, 50, 0);

//            InitialProperties.LifetimeMin = 2.0f;
//            InitialProperties.LifetimeMax = 2.0f;
//            InitialProperties.PositionMin = Vector3.Zero;
//            InitialProperties.PositionMax = Vector3.Zero;
//            InitialProperties.StartSizeMin = 20.0f;
//            InitialProperties.StartSizeMax = 20.0f;
//            InitialProperties.EndSizeMin = 20.0f;
//            InitialProperties.EndSizeMax = 20.0f;
//            InitialProperties.StartColorMin = Color.Red;
//            InitialProperties.StartColorMax = Color.DarkRed;
//            InitialProperties.EndColorMin = Color.Black;
//            InitialProperties.EndColorMax = Color.White;
//            InitialProperties.InterpolateBetweenMinAndMaxColors = false;
//            InitialProperties.VelocityMin = new Vector3(-50, -50, -50);
//            InitialProperties.VelocityMax = new Vector3(50, 50, 50);
//            InitialProperties.AccelerationMin = Vector3.Zero;
//            InitialProperties.AccelerationMax = Vector3.Zero;
//        }

//        public void InitializeParticleSpiral(DefaultTexturedQuadParticle cParticle)
//        {
//            InitializeParticleUsingInitialProperties(cParticle);

//            // Have the new particles' velocities form a circle pattern around position (0, 50, 0)
//            mfAngle += 0.1f;
//            Vector3 sDirection = DPSFHelper.PointOnSphere(-MathHelper.PiOver2, mfAngle, 70, new Vector3(0, 50, 0));
//            cParticle.Velocity = sDirection - cParticle.Position;
//        }

//        public void LoadRandomSpiralEvents()
//        {
//            ParticleInitializationFunction = InitializeParticleSpiral;

//            ParticleEvents.RemoveAllEvents();
//            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
//            ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
//            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
//            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);

//            Emitter.PositionData.Position = new Vector3(0, 50, 0);

//            InitialProperties.LifetimeMin = 2.0f;
//            InitialProperties.LifetimeMax = 2.0f;
//            InitialProperties.PositionMin = Vector3.Zero;
//            InitialProperties.PositionMax = Vector3.Zero;
//            InitialProperties.StartSizeMin = 20.0f;
//            InitialProperties.StartSizeMax = 20.0f;
//            InitialProperties.EndSizeMin = 20.0f;
//            InitialProperties.EndSizeMax = 20.0f;
//            InitialProperties.StartColorMin = Color.Red;
//            InitialProperties.StartColorMax = Color.DarkRed;
//            InitialProperties.EndColorMin = Color.Black;
//            InitialProperties.EndColorMax = Color.White;
//            InitialProperties.VelocityMin = new Vector3(50, 0, 0);
//            InitialProperties.VelocityMax = new Vector3(50, 0, 0);
//            InitialProperties.AccelerationMin = Vector3.Zero;
//            InitialProperties.AccelerationMax = Vector3.Zero;
//        }

//        public void LoadTubeEvents()
//        {
//            LoadRandomSpiralEvents();
//            Emitter.PositionData.Position = new Vector3(50, 100, -200);
//            Emitter.ParticlesPerSecond = 1000;
//        }

//        //===========================================================
//        // Particle Update Functions
//        //===========================================================

//        //===========================================================
//        // Particle System Update Functions
//        //===========================================================

//        //===========================================================
//        // Other Particle System Functions
//        //===========================================================
//    }
}
