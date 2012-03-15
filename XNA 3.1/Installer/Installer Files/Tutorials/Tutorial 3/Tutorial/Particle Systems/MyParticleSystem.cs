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
    /// <summary>
    /// Create a new Particle System class that inherits from a
    /// Default DPSF Particle System
    /// </summary>
    class MyParticleSystem : DefaultPointSpriteParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public MyParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        //===========================================================
        // Initialization Functions
        //===========================================================

		/// <summary>
		/// Function to Initialize the Particle System with default values.
		/// Particle system properties should not be set until after this is called, as
		/// they are likely to be reset to their default values.
		/// </summary>
		/// <param name="cGraphicsDevice">The Graphics Device the Particle System should use</param>
		/// <param name="cContentManager">The Content Manager the Particle System should use to load resources</param>
		/// <param name="cSpriteBatch">The Sprite Batch that the Sprite Particle System should use to draw its particles.
		/// If this is not initializing a Sprite particle system, or you want the particle system to use its own Sprite Batch,
		/// pass in null.</param>
		public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            // Initialize the Particle System before doing anything else
            InitializePointSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, 
                                                UpdateVertexProperties, "Textures/Star9");

            // Finish loading the Particle System in a separate function call, so if
            // we want to reset the Particle System later we don't need to completely 
            // re-initialize it, we can just call this function to reset it.
            LoadParticleSystem();
        }

        /// <summary>
        /// Load the Particle System Events and any other settings
        /// </summary>
        public void LoadParticleSystem()
        {
            // Set the Function to use to Initialize new Particles.
            ParticleInitializationFunction = InitializeParticleProperties;

            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();


            // Allow the Particle's Position, Rotation, Color, and Transparency to be updated each frame
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);

            // This function must be executed after the Color Lerp function as the Color Lerp will overwrite the Color's
            // Transparency value, so we give this function an Execution Order of 100 to make sure it is executed last.
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);


            // Make the Particles start to Rotate after existing for 0.5 seconds
            ParticleEvents.AddTimedEvent(0.5f, UpdateParticleToUseRotationalVelocity);


            // Change the Particle Size's half way through their Lifetime
            ParticleEvents.AddNormalizedTimedEvent(0.5f, UpdateParticleSizeTo20);


            // Setup the Emitter
            Emitter.ParticlesPerSecond = 10;
            Emitter.PositionData.Position = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Function to Initialize a Particle's Properties
        /// </summary>
        /// <param name="cParticle">The Particle to be Initialized</param>
        public void InitializeParticleProperties(DefaultPointSpriteParticle cParticle)
        {
            // Set the Particle's Lifetime (how long it should exist for)
            cParticle.Lifetime = 2.0f;

            // Set the Particle's initial Position to be wherever the Emitter is
            cParticle.Position = Emitter.PositionData.Position;

            // Set the Particle's Velocity
            Vector3 sVelocityMin = new Vector3(-50, 50, -50);
            Vector3 sVelocityMax = new Vector3(50, 100, 50);
            cParticle.Velocity = DPSFHelper.RandomVectorBetweenTwoVectors(sVelocityMin, sVelocityMax);

            // Adjust the Particle's Velocity direction according to the Emitter's Orientation
            cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);

            // Specify the Particle's Rotation and Rotational Velocity
            cParticle.Rotation = RandomNumber.Between(0, MathHelper.TwoPi);
            cParticle.RotationalVelocity = 0;

            // Give the Particle a random Size
            // Since we have Size Lerp enabled we must also set the Start and End Size
            cParticle.Size = RandomNumber.Next(40, 50);

            // Give the Particle a random Color
            // Since we have Color Lerp enabled we must also set the Start and End Color
            cParticle.Color = cParticle.StartColor = Color.Red;
            cParticle.EndColor = Color.Blue;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        /// <summary>
        /// Example of how to create a Particle Event Function
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleFunctionExample(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Place code to update the Particle here
            // Example: cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
        }

        // Change the Particle's Velocity
        public void UpdateParticleVelocityToTravelRight(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Velocity = new Vector3(RandomNumber.Next(50, 100), 0, 0);
        }

        // Change the Particle's Rotational Velocity
        public void UpdateParticleToUseRotationalVelocity(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.RotationalVelocity = RandomNumber.Between(-MathHelper.TwoPi, MathHelper.TwoPi);
        }

        // Change the Particle Size
        public void UpdateParticleSizeTo20(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Size = 20;
        }

        // Make the Particle Visible
        public void UpdateParticleVisibleOn(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Visible = true;
        }

        // Make the Particle InVisible
        public void UpdateParticleVisibleOff(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Visible = false;
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        //===========================================================
        // Other Particle System Functions
        //===========================================================

        /// <summary>
        /// Changes all Active Particle's Velocity to travel straight right.
        /// This function is called when the X key is pressed.
        /// </summary>
        public void MakeParticlesTravelRight()
        {
            ParticleEvents.AddOneTimeEvent(UpdateParticleVelocityToTravelRight);
        }

        /// <summary>
        /// Adds several Particle Events to change the Particles' behaviour.
        /// This function is called when the C key is pressed
        /// </summary>
        public void AddMultipleEvents()
        {
            // Make the Particles flash
            ParticleEvents.AddNormalizedTimedEvent(0.05f, UpdateParticleVisibleOff, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.1f, UpdateParticleVisibleOn, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.15f, UpdateParticleVisibleOff, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.2f, UpdateParticleVisibleOn, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.25f, UpdateParticleVisibleOff, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.3f, UpdateParticleVisibleOn, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.35f, UpdateParticleVisibleOff, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.4f, UpdateParticleVisibleOn, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.45f, UpdateParticleVisibleOff, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.5f, UpdateParticleVisibleOn, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.55f, UpdateParticleVisibleOff, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.6f, UpdateParticleVisibleOn, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.65f, UpdateParticleVisibleOff, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.7f, UpdateParticleVisibleOn, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.75f, UpdateParticleVisibleOff, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.8f, UpdateParticleVisibleOn, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.85f, UpdateParticleVisibleOff, 0, 1);
            ParticleEvents.AddNormalizedTimedEvent(0.9f, UpdateParticleVisibleOn, 0, 1);
        }

        /// <summary>
        /// Removes the Particle Events added with the AddMultipleEvents() function.
        /// This function is called when the V key is pressed
        /// </summary>
        public void RemoveMultipleEvents()
        {
            ParticleEvents.RemoveAllEventsInGroup(1);
        }
    }
}
