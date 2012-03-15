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
	class MyParticleSystem : DefaultTexturedQuadParticleSystem
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

        Color msNewParticleColor = Color.Red;

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
			InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, 
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
            // Set the Particle Initialization Function
            ParticleInitializationFunction = InitializeParticleProperties;


            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();


            // Allow the Particle's Position and Transparency to be updated each frame
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp);

			// Update the particle to face the camera. Do this after updating it's rotation/orientation.
			ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 200);


            // Change the Color of new Particles at random intervals
            ParticleSystemEvents.AddEveryTimeEvent(UpdateParticleSystemChangeParticleColorRandomly);


            // Set the Particle System's Lifetime and what should happen when it reaches its Lifetime
            ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
            ParticleSystemEvents.LifetimeData.Lifetime = 1.0f;

            // Set the Particle System's Emitter to toggle on and off every 0.5 seconds
            ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemEmitParticlesAutomaticallyOn);
            ParticleSystemEvents.AddTimedEvent(0.5f, UpdateParticleSystemEmitParticlesAutomaticallyOff);

            // Change Textures every time the Particle System's Lifetime is reached
            ParticleSystemEvents.AddNormalizedTimedEvent(1.0f, UpdateParticleSystemSwapTexture);


            // Setup the Emitter
            Emitter.ParticlesPerSecond = 50;
            Emitter.PositionData.Position = new Vector3(0, 0, 0);
        }

        public void LoadParticleSystem2()
        {
            // Set the Particle Initialization Function
            ParticleInitializationFunction = InitializeParticleProperties;


            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();


            // Allow the Particle's Position, Rotation, and Transparency to be updated each frame
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp);

			// Update the particle to face the camera. Do this after updating it's rotation/orientation.
			ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 200);


            // Set the Particle System's Lifetime and what should happen when it reaches its Lifetime
            ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
            ParticleSystemEvents.LifetimeData.Lifetime = 2.0f;

            // Set the Particle System's Emitter to toggle on for 0.5 seconds and off for 1.5 seconds
            ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemEmitParticlesAutomaticallyOn);
            ParticleSystemEvents.AddTimedEvent(0.5f, UpdateParticleSystemEmitParticlesAutomaticallyOff);

            // Change Textures every time the Particle System's Lifetime is reached
            ParticleSystemEvents.AddNormalizedTimedEvent(1.0f, UpdateParticleSystemSwapTexture);


            // Setup the Emitter
            Emitter.ParticlesPerSecond = 50;
            Emitter.PositionData.Position = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Example of how to create a Particle Initialization Function
        /// </summary>
        /// <param name="cParticle">The Particle to be Initialized</param>
		public void InitializeParticleProperties(DefaultTexturedQuadParticle cParticle)
        {
            // Set the Particle's Lifetime (how long it should exist for)
            cParticle.Lifetime = 2.0f;

            // Set the Particle's initial Position to be wherever the Emitter is
            cParticle.Position = Emitter.PositionData.Position;

            // Set the Particle's Velocity
            Vector3 sVelocityMin = new Vector3(-50, 50, -50);
            Vector3 sVelocityMax = new Vector3(50, 100, 50);
            cParticle.Velocity = DPSFHelper.RandomVectorBetweenTwoVectors(sVelocityMin, sVelocityMax);

            // Set the Particle's Rotational Velocity
            cParticle.RotationalVelocity.Z = RandomNumber.Between(-MathHelper.TwoPi, MathHelper.TwoPi);

            // Adjust the Particle's Velocity direction according to the Emitter's Orientation
            cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);

            // Give the Particle a random Size
            // Since we have Size Lerp enabled we must also set the Start and End Size
            cParticle.Size = 30;

            // Give the Particle a random Color
            // Since we have Color Lerp enabled we must also set the Start and End Color
            cParticle.Color = msNewParticleColor;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        /// <summary>
        /// Example of how to create a Particle System Event Function
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleSystemFunctionExample(float fElapsedTimeInSeconds)
        {
            // Place code to update the Particle System here
            // Example: Emitter.EmitParticles = true;
            // Example: SetTexture("TextureAssetName");
        }

        // Change the Color or New Particles at random intervals
        public void UpdateParticleSystemChangeParticleColorRandomly(float fElapsedTimeInSeconds)
        {
            // If we randomly should change the color now
            if (RandomNumber.Next(0, 60) == 30)
            {
                msNewParticleColor = DPSFHelper.RandomColor();
            }
        }

        // Particle System Update function to Double the Simulation Speed
        public void UpdateParticleSystemDoubleSimulationSpeed(float fElapsedTimeInSeconds)
        {
            SimulationSpeed = 2.0f;
        }

        // Changes the current Texture being used
        public void UpdateParticleSystemSwapTexture(float fElapsedTimeInSeconds)
        {
            // If the Star Texture is being used
            if (Texture.Name.Equals("Textures/Star9"))
            {
                SetTexture("Textures/Sun3");
            }
            // Else the Sun Texture is being used
            else
            {
                SetTexture("Textures/Star9");
            }
        }

        //===========================================================
        // Other Particle System Functions
        //===========================================================

        /// <summary>
        /// Doubles the Simulation Speed using a One Time Event.
        /// This function is called when the X key is pressed.
        /// </summary>
        public void DoubleSimulationSpeed()
        {
            ParticleSystemEvents.AddOneTimeEvent(UpdateParticleSystemDoubleSimulationSpeed);
        }

        /// <summary>
        /// Sets the Simulation Speed to Normal, without using a One Time Event.
        /// This function is called when the C key is pressed.
        /// </summary>
        public void NormalSimulationSpeed()
        {
            SimulationSpeed = 1.0f;
        }
    }
}
