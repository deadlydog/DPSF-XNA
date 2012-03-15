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
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
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
            ParticleEvents.AddNormalizedTimedEvent(0.25f, UpdateParticleChangeDirection1);
            ParticleEvents.AddNormalizedTimedEvent(0.5f, UpdateParticleChangeDirection2);
            ParticleEvents.AddNormalizedTimedEvent(0.75f, UpdateParticleChangeDirection3);
        }

        public void LoadChangeColorEvents()
        {
            ParticleInitializationFunction = InitializeParticleChangeColor;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddNormalizedTimedEvent(0.25f, UpdateParticleChangeColor1);
            ParticleEvents.AddNormalizedTimedEvent(0.5f, UpdateParticleChangeColor2);
            ParticleEvents.AddNormalizedTimedEvent(0.75f, UpdateParticleChangeColor3);
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

        protected void UpdateParticleChangeParticleColor(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Assign this Particle a new Random Color
            cParticle.Color = DPSFHelper.RandomColor();
        }

        protected void UpdateParticleChangeDirection1(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Move Up
            cParticle.Velocity = new Vector3(0, 50, 0);
        }

        protected void UpdateParticleChangeDirection2(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Move Left
            cParticle.Velocity = new Vector3(-50, 0, 0);
        }

        protected void UpdateParticleChangeDirection3(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Move Down
            cParticle.Velocity = new Vector3(0, -50, 0);
        }

        protected void UpdateParticleChangeColor1(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Color = Color.Green;
        }

        protected void UpdateParticleChangeColor2(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Color = Color.Yellow;
        }

        protected void UpdateParticleChangeColor3(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Color = Color.Blue;
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================
        
        //===========================================================
        // Other Particle System Functions
        //===========================================================

        public void ChangeParticlesToRandomColors()
        {
            this.ParticleEvents.AddOneTimeEvent(UpdateParticleChangeParticleColor);
        }
    }
}