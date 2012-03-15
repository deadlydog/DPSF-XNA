#region File Description
//===================================================================
// DefaultPointSpriteParticleSystemTemplate.cs
// 
// This file provides the template for creating a new Point Sprite Particle
// System that inherits from the Default Point Sprite Particle System.
//
// The spots that should be modified are marked with TODO statements.
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace DPSF.ParticleSystems
{
    //-----------------------------------------------------------
    // TODO: Rename/Refactor the Particle System class
    //-----------------------------------------------------------
    /// <summary>
    /// Create a new Particle System class that inherits from a
    /// Default DPSF Particle System
    /// </summary>
    class SparklerParticleSystem : DefaultPointSpriteParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public SparklerParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        float mfParticleSpeed = 100;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        //===========================================================
        // Initialization Functions
        //===========================================================

        /// <summary>
        /// Function to Initialize the Particle System with default values
        /// </summary>
        /// <param name="cGraphicsDevice">The Graphics Device to draw to</param>
        /// <param name="cContentManager">The Content Manager to use to load Textures and Effect files</param>
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager)
        {
            // Initialize the Particle System before doing anything else
            InitializePointSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, 
                                                UpdateVertexProperties, "Textures/Star9");

            // Set the Name of the Particle System
            Name = "Sparkler";

            // Finish loading the Particle System in a separate function call, so if
            // we want to reset the Particle System later we don't need to completely 
            // re-initialize it, we can just call this function to reset it.
            LoadSimpleParticleSystem();
        }

        /// <summary>
        /// Load the Particle System Events and any other settings
        /// </summary>
        public void LoadSimpleParticleSystem()
        {
            ParticleInitializationFunction = InitializeParticleSimple;

            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();

            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
            ParticleEvents.AddNormalizedTimedEvent(0.5f, UpdateParticleVelocityRandomly);

            // Setup the Emitter
            Emitter.ParticlesPerSecond = 100;
            Emitter.PositionData.Position = Vector3.Zero;
        }

        /// <summary>
        /// Example of how to create a Particle Initialization Function
        /// </summary>
        /// <param name="cParticle">The Particle to be Initialized</param>
        public void InitializeParticleSimple(DefaultPointSpriteParticle cParticle)
        {
            cParticle.Lifetime = 1.0f;

            // Set the Particle's initial Position to be wherever the Emitter is
            cParticle.Position = Emitter.PositionData.Position;

            // Set the Particle's Velocity
            cParticle.Velocity = new Vector3(0, mfParticleSpeed, 0);

            // Adjust the Particle's Velocity direction according to the Emitter's Orientation
            cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);

            // Give the Particle a random Size
            // Since we have Size Lerp enabled we must also set the Start and End Size
            cParticle.Size = cParticle.StartSize = cParticle.EndSize = 25;

            // Give the Particle a random Color
            // Since we have Color Lerp enabled we must also set the Start and End Color
            cParticle.Color = cParticle.StartColor = cParticle.EndColor = Color.Red;
        }

        /// <summary>
        /// Load the Particle System Events and any other settings
        /// </summary>
        public void LoadComplexParticleSystem()
        {
            ParticleInitializationFunction = InitializeParticleComplex;

            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();

            // Allow the Particle's Velocity, Rotational Velocity, Size, Color, and Transparency to be updated each frame
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);

            ParticleEvents.AddNormalizedTimedEvent(0.5f, UpdateParticleToChange);

            // Setup the Emitter
            Emitter.ParticlesPerSecond = 100;
            Emitter.PositionData.Position = Vector3.Zero;
        }

        /// <summary>
        /// Example of how to create a Particle Initialization Function
        /// </summary>
        /// <param name="cParticle">The Particle to be Initialized</param>
        public void InitializeParticleComplex(DefaultPointSpriteParticle cParticle)
        {
            cParticle.Lifetime = 1.0f;

            // Set the Particle's initial Position to be wherever the Emitter is
            cParticle.Position = Emitter.PositionData.Position;

            // Set the Particle's Velocity
            cParticle.Velocity = new Vector3(0, mfParticleSpeed, 0);

            // Adjust the Particle's Velocity direction according to the Emitter's Orientation
            cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);

            // Give the Particle a random Size
            // Since we have Size Lerp enabled we must also set the Start and End Size
            cParticle.Size = cParticle.StartSize = cParticle.EndSize = 10;

            // Give the Particle a random Color
            // Since we have Color Lerp enabled we must also set the Start and End Color
            cParticle.Color = cParticle.StartColor = cParticle.EndColor = Color.White;
        }


        //===========================================================
        // Particle Update Functions
        //===========================================================

        /// <summary>
        /// Randomly changes the direction of the particle
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleVelocityRandomly(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Velocity = DPSFHelper.RandomNormalizedVector() * mfParticleSpeed;
        }

        public void UpdateParticleToChange(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Velocity = DPSFHelper.RandomNormalizedVector() * mfParticleSpeed;
            cParticle.Color = DPSFHelper.RandomColor();
            cParticle.Size = RandomNumber.Next(10, 40);
            cParticle.RotationalVelocity = RandomNumber.Between(-MathHelper.Pi, MathHelper.Pi);
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================


        //===========================================================
        // Other Particle System Functions
        //===========================================================
    }
}
