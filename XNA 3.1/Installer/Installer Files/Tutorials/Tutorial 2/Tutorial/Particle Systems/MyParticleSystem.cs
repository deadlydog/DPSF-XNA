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

        // Variables used by the InitializeParticleProperties2() function
        int miCurrentSize = 10;
        bool mbSizeIncreasing = true;

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
            // The Default Templates include a Particle Initialization Function called
            // InitializeParticleUsingInitialProperties, which initializes new Particles
            // according to the settings in the InitialProperties object (see further below).
            // You can also create your own Particle Initialization Functions as well, as shown with
            // the InitializeParticleProperties function below.
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;
            //ParticleInitializationFunction = InitializeParticleProperties;

            // Setup the Initial properties of the Particles.
            // These are only applied if using InitializeParticleUsingInitialProperties 
            // as the Particle Initialization Function.
            InitialProperties.LifetimeMin = 1.5f;
            InitialProperties.LifetimeMax = 3.0f;
            InitialProperties.PositionMin = Vector3.Zero;
            InitialProperties.PositionMax = Vector3.Zero;
            InitialProperties.VelocityMin = new Vector3(-50, 50, -50);
            InitialProperties.VelocityMax = new Vector3(50, 100, 50);
            InitialProperties.RotationMin = 0.0f;
            InitialProperties.RotationMax = MathHelper.Pi;
            InitialProperties.RotationalVelocityMin = -MathHelper.Pi;
            InitialProperties.RotationalVelocityMax = MathHelper.Pi;
            InitialProperties.StartSizeMin = 20;
            InitialProperties.StartSizeMax = 40;
            InitialProperties.EndSizeMin = 30;
            InitialProperties.EndSizeMax = 30;
            InitialProperties.StartColorMin = Color.Black;
            InitialProperties.StartColorMax = Color.White;
            InitialProperties.EndColorMin = Color.Black;
            InitialProperties.EndColorMax = Color.White;



            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();

            // Allow the Particle's Position, Rotation, Size, Color, and Transparency to be updated each frame
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleSizeUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);

            // This function must be executed after the Color Lerp function as the Color Lerp will overwrite the Color's
            // Transparency value, so we give this function an Execution Order of 100 to make sure it is executed last.
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);

            // Setup the Emitter
            Emitter.ParticlesPerSecond = 50;
            Emitter.PositionData.Position = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Example of how to create a Particle Initialization Function
        /// </summary>
        /// <param name="cParticle">The Particle to be Initialized</param>
        public void InitializeParticleProperties(DefaultPointSpriteParticle cParticle)
        {
            // Set the Particle's Lifetime (how long it should exist for)
            cParticle.Lifetime = 2.0f;

            // Set the Particle's initial Position to be wherever the Emitter is
            cParticle.Position = Emitter.PositionData.Position;

            // Set the Particle's Velocity
            Vector3 sVelocityMin = new Vector3(-100, 25, -100);
            Vector3 sVelocityMax = new Vector3(100, 50, 100);
            cParticle.Velocity = DPSFHelper.RandomVectorBetweenTwoVectors(sVelocityMin, sVelocityMax);

            // Adjust the Particle's Velocity direction according to the Emitter's Orientation
            cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);

            // Give the Particle a random Size
            // Since we have Size Lerp enabled we must also set the Start and End Size
            cParticle.Size = cParticle.StartSize = cParticle.EndSize = RandomNumber.Next(10, 20);

            // Give the Particle a random Color
            // Since we have Color Lerp enabled we must also set the Start and End Color
            cParticle.Color = cParticle.StartColor = cParticle.EndColor = DPSFHelper.RandomColor();
        }

        /// <summary>
        /// Another example of how to create a Particle Initialization Function
        /// </summary>
        /// <param name="cParticle">The Particle to be Initialized</param>
        public void InitializeParticleProperties2(DefaultPointSpriteParticle cParticle)
        {
            // Set the Particle's Lifetime (how long it should exist for)
            cParticle.Lifetime = 1.0f;

            // Set the Particle's initial Position to be wherever the Emitter is
            cParticle.Position = Emitter.PositionData.Position;

            // Set the Particle's Velocity
            cParticle.Velocity = new Vector3(0, 200, 0);

            // Adjust the Particle's Velocity direction according to the Emitter's Orientation
            cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);

            // If the Particles should be Increasing in Size
            if (mbSizeIncreasing)
            {
                miCurrentSize++;
            }
            // Else they should be Decreasing in Size
            else
            {
                miCurrentSize--;
            }

            // If the Particle Size is too small or too large, negate the size scaling
            if (miCurrentSize <= 10 || miCurrentSize >= 60)
            {
                mbSizeIncreasing = !mbSizeIncreasing;
            }

            // Set the Particle's Size to our CurrentSize value
            // Since we have Size Lerp enabled we must also set the Start and End Size
            cParticle.Size = cParticle.StartSize = cParticle.EndSize = miCurrentSize;

            // Give the Particle a random Color
            // Since we have Color Lerp enabled we must also set the Start and End Color
            cParticle.Color = cParticle.StartColor = cParticle.EndColor = DPSFHelper.RandomColor();
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

        /// <summary>
        /// Sets the InitializeParticleUsingInitialProperties() function as the Particle Initialization Function to use.
        /// This function is called when the 'X' key is pressed.
        /// </summary>
        public void UseInitializeParticleUsingInitialPropertiesFunction()
        {
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;
        }

        /// <summary>
        /// Sets the InitializeParticleProperties() function as the Particle Initialization Function to use.
        /// This function is called when the 'C' key is pressed.
        /// </summary>
        public void UseInitializeParticlePropertiesFunction()
        {
            ParticleInitializationFunction = InitializeParticleProperties;
        }

        /// <summary>
        /// Sets the InitializeParticleUsingInitialProperties2() function as the Particle Initialization Function to use.
        /// This function is called when the 'V' key is pressed.
        /// </summary>
        public void UseInitializeParticleProperties2Function()
        {
            ParticleInitializationFunction = InitializeParticleProperties2;
        }
    }
}
