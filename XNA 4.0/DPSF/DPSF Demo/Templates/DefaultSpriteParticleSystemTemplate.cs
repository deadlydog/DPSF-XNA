#region File Description
//===================================================================
// DefaultSpriteParticleSystemTemplate.cs
// 
// This file provides the template for creating a new Sprite Particle
// System that inherits from the Default Sprite Particle System.
//
// The spots that should be modified are marked with TODO statements.
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF_Demo.ParticleSystems
{
	//-----------------------------------------------------------
	// TODO: Rename/Refactor the Particle System class
	//-----------------------------------------------------------
	/// <summary>
	/// Create a new Particle System class that inherits from a Default DPSF Particle System.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	class DefaultSpriteParticleSystemTemplate : DefaultSpriteParticleSystem
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
		/// parameter if not using a Game object.</param>
		public DefaultSpriteParticleSystemTemplate(Game cGame) : base(cGame) { }

		//===========================================================
		// Structures and Variables
		//===========================================================

		//-----------------------------------------------------------
		// TODO: Place any Particle System properties here
		//-----------------------------------------------------------
		float mfSizeMin = 32;
		float mfSizeMax = 128;

		//===========================================================
		// Overridden Particle System Functions
		//===========================================================

		//-----------------------------------------------------------
		// TODO: Place any overridden Particle System functions here
		//-----------------------------------------------------------

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
			InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, "Textures/Star9", cSpriteBatch);
			
			// Set the Name of the Particle System
			Name = "Default Sprite Particle System Template";

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
			//-----------------------------------------------------------
			// TODO: Setup the Particle System to achieve the desired result.
			// You may change all of the code in this function. It is just
			// provided to show you how to setup a simple particle system.
			//-----------------------------------------------------------

			// Set the Function to use to Initialize new Particles.
			// The Default Templates include a Particle Initialization Function called
			// InitializeParticleUsingInitialProperties, which initializes new Particles
			// according to the settings in the InitialProperties object (see further below).
			// You can also create your own Particle Initialization Functions as well, as shown with
			// the InitializeParticleProperties function below.
			ParticleInitializationFunction = InitializeParticleUsingInitialProperties;
			//ParticleInitializationFunction = InitializeParticleProperties;

			// Setup the Initial Properties of the Particles.
			// These are only applied if using InitializeParticleUsingInitialProperties 
			// as the Particle Initialization Function.
			InitialProperties.LifetimeMin = 2.0f;
			InitialProperties.LifetimeMax = 2.0f;
			InitialProperties.PositionMin = Vector3.Zero;
			InitialProperties.PositionMax = Vector3.Zero;
			InitialProperties.VelocityMin = new Vector3(-200, -200, 0);
			InitialProperties.VelocityMax = new Vector3(200, -400, 0);
			InitialProperties.RotationMin = 0.0f;
			InitialProperties.RotationMax = MathHelper.Pi;
			InitialProperties.RotationalVelocityMin = -MathHelper.Pi;
			InitialProperties.RotationalVelocityMax = MathHelper.Pi;
			InitialProperties.StartWidthMin = mfSizeMin;
			InitialProperties.StartWidthMax = mfSizeMax;
			InitialProperties.StartHeightMin = mfSizeMin;
			InitialProperties.StartHeightMax = mfSizeMax;
			InitialProperties.EndWidthMin = 30;
			InitialProperties.EndWidthMax = 30;
			InitialProperties.EndHeightMin = 30;
			InitialProperties.EndHeightMax = 30;
			InitialProperties.StartColorMin = Color.Black;
			InitialProperties.StartColorMax = Color.White;
			InitialProperties.EndColorMin = Color.Black;
			InitialProperties.EndColorMax = Color.White;

			// Remove all Events first so that none are added twice if this function is called again
			ParticleEvents.RemoveAllEvents();
			ParticleSystemEvents.RemoveAllEvents();

			// Allow the Particle's Position, Rotation, Width and Height, Color, and Transparency to be updated each frame
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);

			// This function must be executed after the Color Lerp function as the Color Lerp will overwrite the Color's
			// Transparency value, so we give this function an Execution Order of 100 to make sure it is executed last.
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);

			// Set the Particle System's Emitter to toggle on and off every 0.5 seconds
			ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
			ParticleSystemEvents.LifetimeData.Lifetime = 1.0f;
			ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemEmitParticlesAutomaticallyOn);
			ParticleSystemEvents.AddTimedEvent(0.5f, UpdateParticleSystemEmitParticlesAutomaticallyOff);

			// Setup the Emitter
			Emitter.ParticlesPerSecond = 25;
			Emitter.PositionData.Position = new Vector3(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height, 0);
		}

		/// <summary>
		/// Example of how to create a Particle Initialization Function
		/// </summary>
		/// <param name="cParticle">The Particle to be Initialized</param>
		public void InitializeParticleProperties(DefaultSpriteParticle cParticle)
		{
			//-----------------------------------------------------------
			// TODO: Initialize all of the Particle's properties here.
			// If you plan on simply using the default InitializeParticleUsingInitialProperties
			// Particle Initialization Function (see the LoadParticleSystem() function above), 
			// then you may delete this function all together.
			//-----------------------------------------------------------

			// Set the Particle's Lifetime (how long it should exist for)
			cParticle.Lifetime = 2.0f;

			// Set the Particle's initial Position to be wherever the Emitter is
			cParticle.Position = Emitter.PositionData.Position;

			// Set the Particle's Velocity
			Vector3 sVelocityMin = new Vector3(-200, -200, 0);
			Vector3 sVelocityMax = new Vector3(200, -400, 0);
			cParticle.Velocity = DPSFHelper.RandomVectorBetweenTwoVectors(sVelocityMin, sVelocityMax);

			// Adjust the Particle's Velocity direction according to the Emitter's Orientation
			cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);

			// Give the Particle a random Size
			// Since we have Size Lerp enabled we must also set the Start and End Size
			cParticle.Width = cParticle.StartWidth = cParticle.EndWidth =
				cParticle.Height = cParticle.StartHeight = cParticle.EndHeight = RandomNumber.Next((int)mfSizeMin, (int)mfSizeMax);

			// Give the Particle a random Color
			// Since we have Color Lerp enabled we must also set the Start and End Color
			cParticle.Color = cParticle.StartColor = cParticle.EndColor = DPSFHelper.RandomColor();
		}

		//===========================================================
		// Particle Update Functions
		//===========================================================

		//-----------------------------------------------------------
		// TODO: Place your Particle Update functions here, using the 
		// same function prototype as below (i.e. public void FunctionName(DPSFParticle, float))
		//-----------------------------------------------------------

		/// <summary>
		/// Example of how to create a Particle Event Function
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleFunctionExample(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Place code to update the Particle here
			// Example: cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
		}

		//===========================================================
		// Particle System Update Functions
		//===========================================================

		//-----------------------------------------------------------
		// TODO: Place your Particle System Update functions here, using 
		// the same function prototype as below (i.e. public void FunctionName(float))
		//-----------------------------------------------------------

		/// <summary>
		/// Example of how to create a Particle System Event Function
		/// </summary>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleSystemFunctionExample(float fElapsedTimeInSeconds)
		{
			// Place code to update the Particle System here
			// Example: Emitter.EmitParticles = true;
			// Example: SetTexture("TextureAssetName");
		}

		//===========================================================
		// Other Particle System Functions
		//===========================================================

		//-----------------------------------------------------------
		// TODO: Place any other functions here
		//-----------------------------------------------------------
	}
}
