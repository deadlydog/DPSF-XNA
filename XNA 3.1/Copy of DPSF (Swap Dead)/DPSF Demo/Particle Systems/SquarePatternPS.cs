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
    class SquarePatternParticleSystem : DefaultPointSpriteParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SquarePatternParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager)
        {
            InitializePointSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000,
                                                UpdateVertexProperties, "Textures/Bubble");
            LoadSquarePatternEvents();
            Emitter.ParticlesPerSecond = 100;
            Name = "Square Pattern";
        }

        public void LoadSquarePatternEvents()
        {
            ParticleInitializationFunction = InitializeParticleSquarePattern;

            Emitter.PositionData.Position = new Vector3(0, 50, 0);

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddNormalizedTimedEvent(0.25f, ChangeDirection1);
            ParticleEvents.AddNormalizedTimedEvent(0.5f, ChangeDirection2);
            ParticleEvents.AddNormalizedTimedEvent(0.75f, ChangeDirection3);
        }

        public void LoadChangeColorEvents()
        {
            ParticleInitializationFunction = InitializeParticleChangeColor;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddNormalizedTimedEvent(0.25f, ChangeColor1);
            ParticleEvents.AddNormalizedTimedEvent(0.5f, ChangeColor2);
            ParticleEvents.AddNormalizedTimedEvent(0.75f, ChangeColor3);
        }

        public void InitializeParticleSquarePattern(DefaultPointSpriteParticle cParticle)
        {
            cParticle.Lifetime = (float)(5.0f);

            cParticle.Position = Emitter.PositionData.Position;
            cParticle.Position += new Vector3(RandomNumber.Next(-50, 50), RandomNumber.Next(-50, 50), RandomNumber.Next(-50, 50));
            cParticle.Size = RandomNumber.Next(5, 20);
            cParticle.Color = new Color(0, RandomNumber.NextFloat(), 0);

            // Move Right
            cParticle.Velocity = new Vector3(50, 0, 0);
            cParticle.Acceleration = Vector3.Zero;
        }

        public void InitializeParticleChangeColor(DefaultPointSpriteParticle cParticle)
        {
            cParticle.Lifetime = (float)(4.0f);

            cParticle.Position = Emitter.PositionData.Position;
            cParticle.Position += new Vector3(-100, RandomNumber.Next(0, 100), RandomNumber.Next(-50, 50));
            cParticle.Size = RandomNumber.Next(5, 20);
            cParticle.Color = Color.Red;

            cParticle.Velocity = new Vector3(50, 0, 0);
            cParticle.Acceleration = Vector3.Zero;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        public void ChangeParticleColor(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Assign this Particle a new Random Color
            cParticle.Color = DPSFHelper.RandomColor();
        }
        
        public void ChangeDirection1(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Move Up
            cParticle.Velocity = new Vector3(0, 50, 0);
        }

        public void ChangeDirection2(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Move Left
            cParticle.Velocity = new Vector3(-50, 0, 0);
        }

        public void ChangeDirection3(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Move Down
            cParticle.Velocity = new Vector3(0, -50, 0);
        }

        public void ChangeColor1(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Color = Color.Green;
        }

        public void ChangeColor2(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Color = Color.Yellow;
        }

        public void ChangeColor3(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Color = Color.Blue;
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================
        
        //===========================================================
        // Other Particle System Functions
        //===========================================================
    }
}