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

		// Variable to tell if the Particles should be initialized to travel in the same direction or not
		private bool mbInitializeParticlesWithRandomDirection = true;

        // The Min and Max Distance a Particle must be from the Magnet to be affected by the Magnet
        private float mfMinDistance = 0;
        private float mfMaxDistance = 100;

        /// <summary>
        /// Get / Set the Max Force that the Magnets should exert on the Particles
        /// </summary>
        public float MagnetsForce
        {
            get { return mfMagnetsForce; }
            set
            {
                mfMagnetsForce = value;

                // Set the Max Force of each Magnet
                foreach (DefaultParticleSystemMagnet cMagnet in MagnetList)
                {
                    cMagnet.MaxForce = mfMagnetsForce;
                }
            }
        }
        private float mfMagnetsForce = 20;

        /// <summary>
        /// Get / Set the Distance Function that the Magnets use
        /// </summary>
        public DefaultParticleSystemMagnet.DistanceFunctions MagnetsDistanceFunction
        {
            get { return meMagnetsDistanceFunction; }
            set
            {
                meMagnetsDistanceFunction = value;

                // Set the Distance Function of each Magnet
                foreach (DefaultParticleSystemMagnet cMagnet in MagnetList)
                {
                    cMagnet.DistanceFunction = meMagnetsDistanceFunction;
                }
            }
        }
        private DefaultParticleSystemMagnet.DistanceFunctions meMagnetsDistanceFunction = DefaultParticleSystemMagnet.DistanceFunctions.Linear;

        /// <summary>
        /// Get / Set the Mode that the Magnets should use
        /// </summary>
        public DefaultParticleSystemMagnet.MagnetModes MagnetsMode
        {
            get { return meMagnetsMode; }
            set
            {
                meMagnetsMode = value;

                // Set the Magnet Mode of each Magnet
                foreach (DefaultParticleSystemMagnet cMagnet in MagnetList)
                {
                    cMagnet.Mode = meMagnetsMode;
                }
            }
        }
        private DefaultParticleSystemMagnet.MagnetModes meMagnetsMode = DefaultParticleSystemMagnet.MagnetModes.Attract;

        /// <summary>
        /// Get whether the Magnets affect a Particle's Position or Velocity
        /// </summary>
        public bool MagnetsAffectPosition { get; private set; }

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
			// Set the Particle Initialization Function, as it can be changed on the fly
			// and we want to make sure we are using the right one to start with to start with.
			ParticleInitializationFunction = InitializeParticleProperties;

			// Remove all Events first so that none are added twice if this function is called again
			ParticleEvents.RemoveAllEvents();
			ParticleSystemEvents.RemoveAllEvents();

			// Setup the Emitter
			Emitter.ParticlesPerSecond = 100;
			Emitter.PositionData.Position = new Vector3(-100, 50, 0);

			// Allow the Particle's Velocity, Rotational Velocity, Color, and Transparency to be updated each frame
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);

			// This function must be executed after the Color Lerp function as the Color Lerp will overwrite the Color's
			// Transparency value, so we give this function an Execution Order of 100 to make sure it is executed last.
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);


			//=================================================================
			// Tutorial 7 Specific Code
			//=================================================================

            // Call function to add Magnet Particle Event
            ToogleMagnetsAffectPositionOrVelocity();

			// Specify to use the Point Magnet by defualt
			UseMagnetType(DefaultParticleSystemMagnet.MagnetTypes.PointMagnet);
        }

        /// <summary>
        /// Example of how to create a Particle Initialization Function
        /// </summary>
        /// <param name="cParticle">The Particle to be Initialized</param>
        public void InitializeParticleProperties(DefaultPointSpriteParticle cParticle)
        {
            // Set the Particle's Lifetime (how long it should exist for) and initial Position
			cParticle.Lifetime = 5;
			cParticle.Position = Emitter.PositionData.Position;

			// If the Particles should travel in random directions
			if (mbInitializeParticlesWithRandomDirection)
			{
				// Give the Particle a random velocity direction to start with
				cParticle.Velocity = DPSFHelper.RandomNormalizedVector() * 50;
			}
			else
			{
				// Emit all of the Particles in the same direction
				cParticle.Velocity = Vector3.Right * 50;

				// Adjust the Particle's starting velocity direction according to the Emitter's Orientation
				cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);
			}

			cParticle.RotationalVelocity = RandomNumber.Between(-MathHelper.Pi, MathHelper.Pi);
			cParticle.Size = 10;
			cParticle.StartColor = cParticle.EndColor = cParticle.Color = DPSFHelper.RandomColor();
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

        //===========================================================
        // Other Particle System Functions
        //===========================================================

		/// <summary>
		/// Toggles the variable indicating whether the Particles should be initialized 
		/// with a random direction or not
		/// </summary>
		public void ToggleInitialRandomDirection()
		{
			mbInitializeParticlesWithRandomDirection = !mbInitializeParticlesWithRandomDirection;
		}

        /// <summary>
        /// Toggles if Magnets should affect the Particles' Position or Velocity, and adds 
        /// the appropriate Particle Event to do so
        /// </summary>
        public void ToogleMagnetsAffectPositionOrVelocity()
        {
            // Toggle if Magnets should affect the Particles' Position or Velocity
            MagnetsAffectPosition = !MagnetsAffectPosition;

            // Remove the previous Magnet Particle Events
            ParticleEvents.RemoveAllEventsInGroup(1);

            // If the Magnets should affect the Particles' Velocity
            if (MagnetsAffectPosition)
            {
                // Specify that Magnets should affect the Particles' Position
                ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAccordingToMagnets, 0, 1);
            }
            // Else they should affect the Particles' Position
            else
            {
                // Specify that Magnets should affect the Particles' Velocity
                ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityAccordingToMagnets, 0, 1);
            }
        }

		/// <summary>
		/// Specify which Type of Magnet we should use to affect the Particles
		/// </summary>
		/// <param name="iMagnetTypeToEnabled">The Type of Magnet to use (1 - 4)</param>
		public void UseMagnetType(DefaultParticleSystemMagnet.MagnetTypes eMagnetTypeToUse)
		{
			//=================================================================
			// Tutorial 7 Specific Code
			//=================================================================

			// Clear all of the Magnets so they can be re-added
			MagnetList.Clear();

			// Use the specified Type of Magnet
			switch (eMagnetTypeToUse)
			{
				default:
				case DefaultParticleSystemMagnet.MagnetTypes.PointMagnet:
					MagnetList.AddFirst(new MagnetPoint(new Vector3(0, 50, 0),
											MagnetsMode, MagnetsDistanceFunction,
                                            mfMinDistance, mfMaxDistance, MagnetsForce, 0));
				break;
				case DefaultParticleSystemMagnet.MagnetTypes.LineMagnet:
					MagnetList.AddFirst(new MagnetLine(new Vector3(0, 50, 0), Vector3.Up,
                                            MagnetsMode, MagnetsDistanceFunction,
                                            mfMinDistance, mfMaxDistance, MagnetsForce, 0));
				break;
				case DefaultParticleSystemMagnet.MagnetTypes.LineSegmentMagnet:
					MagnetList.AddFirst(new MagnetLineSegment(new Vector3(0, 25, 0), new Vector3(0, 75, 0),
                                            MagnetsMode, MagnetsDistanceFunction,
                                            mfMinDistance, mfMaxDistance, MagnetsForce, 0));
				break;
				case DefaultParticleSystemMagnet.MagnetTypes.PlaneMagnet:
					MagnetList.AddFirst(new MagnetPlane(new Vector3(0, 50, 0), Vector3.Right,
                                            MagnetsMode, MagnetsDistanceFunction,
                                            mfMinDistance, mfMaxDistance, MagnetsForce, 0));
				break;
			}
		}
    }
}
