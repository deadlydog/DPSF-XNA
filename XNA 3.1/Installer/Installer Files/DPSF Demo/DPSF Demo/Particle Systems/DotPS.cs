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
    class DotParticleSystem : DefaultPointSpriteParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DotParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializePointSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000,
                                                UpdateVertexProperties, "Textures/Smoke");
            LoadParticleSystem();
            Emitter.ParticlesPerSecond = 200;
            Name = "Dot";
        }

        public void InitializeParticleDot(DefaultPointSpriteParticle cParticle)
        {
            cParticle.Lifetime = 1.0f;

            cParticle.Position = Emitter.PositionData.Position;
            cParticle.Size = 10.0f;
            cParticle.Color = Color.White;

            cParticle.Velocity = Vector3.Zero;
            cParticle.Acceleration = Vector3.Zero;
        }

        public void LoadParticleSystem()
        {
            ParticleInitializationFunction = InitializeParticleDot;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp);

            Emitter.PositionData.Position = new Vector3(0, 50, 0);
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