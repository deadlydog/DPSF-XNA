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
    class ParticleSystem : DefaultPointSpriteParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public ParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        public bool mbUseRandomLifetimes = false;

        Vector3 msVelocityMin = new Vector3(-50, 50, -50);
        Vector3 msVelocityMax = new Vector3(50, 100, 50);

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
            //-----------------------------------------------------------
            // TODO: Change any Initialization parameters desired and the Name
            //-----------------------------------------------------------
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
            ParticleInitializationFunction = InitializeParticleProperties;

            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();

            // Allow the Particle's Velocity, Rotational Velocity, Size, Color, and Transparency to be updated each frame
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleSizeUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);

            // This function must be executed after the Color Lerp function as the Color Lerp will overwrite the Color's
            // Transparency value, so we give this function an Execution Order of 100 to make sure it is executed last.
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);

            ParticleEvents.AddNormalizedTimedEvent(0.5f, UpdateParticleVelocity, -100);

            ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
            ParticleSystemEvents.LifetimeData.Lifetime = 0.2f;
            ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemTurnEmitterOn);
            ParticleSystemEvents.AddTimedEvent(0.1f, UpdateParticleSystemTurnEmitterOff);

            // Setup the Emitter
            Emitter.ParticlesPerSecond = 1000;
            Emitter.PositionData.Position = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Example of how to create a Particle Initialization Function
        /// </summary>
        /// <param name="cParticle">The Particle to be Initialized</param>
        public void InitializeParticleProperties(DefaultPointSpriteParticle cParticle)
        {
            // If Random Lifetimes should be used
            if (mbUseRandomLifetimes)
            {
                // Set the Particle's Lifetime
                cParticle.Lifetime = 1.5f + (float)RandomNumber.NextDouble();
            }
            // Else Static Lifetimes should be used
            else
            {
                // Set the Particle's Lifetime
                cParticle.Lifetime = 2.0f;
            }

            // Set the Particle's initial Position to be wherever the Emitter is
            cParticle.Position = Emitter.PositionData.Position;

            // Set the Particle's Velocity
            cParticle.Velocity = DPSFHelper.RandomVectorBetweenTwoVectors(msVelocityMin, msVelocityMax);

            // Adjust the Particle's Velocity direction according to the Emitter's Orientation
            cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);

            // Give the Particle a random Size
            // Since we have Size Lerp enabled we must also set the Start and End Size
            cParticle.Size = cParticle.StartSize = cParticle.EndSize = RandomNumber.Next(10, 50);

            // Give the Particle a random Color
            // Since we have Color Lerp enabled we must also set the Start and End Color
            cParticle.Color = cParticle.StartColor = cParticle.EndColor = DPSFHelper.RandomColor();

            // Set the Particle's Rotation
            cParticle.Rotation = (float)RandomNumber.NextDouble() * MathHelper.TwoPi;
            cParticle.RotationalVelocity = -MathHelper.TwoPi + (float)(RandomNumber.NextDouble() * (2 * MathHelper.TwoPi));
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        public void UpdateParticleVelocity(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Velocity = DPSFHelper.RandomVectorBetweenTwoVectors(msVelocityMin, msVelocityMax);
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        public void UpdateParticleSystemTurnEmitterOn(float fElapsedTimeInSeconds)
        {
            //Emitter.EmitParticles = true;
            Emitter.EmitParticlesAutomatically = true;
        }

        public void UpdateParticleSystemTurnEmitterOff(float fElapsedTimeInSeconds)
        {
            //Emitter.EmitParticles = false;
            Emitter.EmitParticlesAutomatically = false;
        }

        //===========================================================
        // Other Particle System Functions
        //===========================================================
    }
}
