#region File Description
//===================================================================
// SpriteParticleSystemTemplate.cs
// 
// This file provides the template for creating a new Sprite Particle
// System from scratch, by creating a very basic Sprite Particle System.
// NOTE: Sprite particles are specified in 2D screen coordinates, not 
// 3D world coordinates like the other types of Particle Systems.
//
// First, it shows how to create a new Particle class so the user can 
// create Particles with whatever properties they need their Particles
// to contain. It then shows how to create the Particle System class itself.
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
	// TODO: Rename/Refactor the Particle class
	//-----------------------------------------------------------
	/// <summary>
	/// Create a new Particle class that inherits from DPSFParticle.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	class SpriteParticleSystemTemplateParticle : DPSFParticle
	{
		//-----------------------------------------------------------
		// TODO: Add in any properties that you want your Particles to have here.
		// NOTE: A Sprite Particle System requires the Particles to at least
		// have a Position. Also, if you want your Particles to be different
		// sizes, you must specify a Width and Height, or at least a Size that
		// can be used for both the Width and the Height. If you want all of
		// the Particles to be the same size, you can hard-code the width
		// and height into the DrawSprite() function of the Particle System.
		//-----------------------------------------------------------
		public Vector3 Position;        // The Position of the Particle in 3D space
		public Vector3 Velocity;        // The 3D Velocity of the Particle
		public float Width;             // The Width of the Particle
		public float Height;            // The Height of the Particle

		/// <summary>
		/// Resets the Particles variables to default values
		/// </summary>
		public override void Reset()
		{
			base.Reset();

			//-----------------------------------------------------------
			// TODO: Reset your Particle properties to their default values here
			//-----------------------------------------------------------
			Position = Vector3.Zero;
			Velocity = Vector3.Zero;
			Width = 64;
			Height = 64;
		}

		/// <summary>
		/// Deep copy the ParticleToCopy's values into this Particle
		/// </summary>
		/// <param name="ParticleToCopy">The Particle whose values should be Copied</param>
		public override void CopyFrom(DPSFParticle ParticleToCopy)
		{
			// Cast the Particle from its base type to its actual type
			SpriteParticleSystemTemplateParticle cParticleToCopy = (SpriteParticleSystemTemplateParticle)ParticleToCopy;
			base.CopyFrom(cParticleToCopy);

			//-----------------------------------------------------------
			// TODO: Copy your Particle properties from the given Particle here
			//-----------------------------------------------------------
			Position = cParticleToCopy.Position;
			Velocity = cParticleToCopy.Velocity;
			Width = cParticleToCopy.Width;
			Height = cParticleToCopy.Height;
		}
	}

	//-----------------------------------------------------------
	// TODO: Rename/Refactor the Particle System class
	//-----------------------------------------------------------
	/// <summary>
	/// Create a new Particle System class that inherits from DPSF using 
	/// our created Particle class and Particle Vertex structure.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	class SpriteParticleSystemTemplate : DPSF<SpriteParticleSystemTemplateParticle, DefaultSpriteParticleVertex>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
		/// parameter if not using a Game object.</param>
		public SpriteParticleSystemTemplate(Game cGame) : base(cGame) { }

		//===========================================================
		// Structures and Variables
		//===========================================================

		//-----------------------------------------------------------
		// TODO: Place any Particle System properties here
		//-----------------------------------------------------------
		float mfSizeMin = 32;
		float mfSizeMax = 128;

		//===========================================================
		// Vertex Update and Overridden Particle System Functions
		//===========================================================

		/// <summary>
		/// Function to draw a Sprite Particle. This function should be used to draw the given
		/// Particle with the provided SpriteBatch.
		/// </summary>
		/// <param name="cParticle">The Particle Sprite to Draw</param>
		/// <param name="cSpriteBatch">The SpriteBatch to use to doing the Drawing</param>
		protected override void DrawSprite(DPSFParticle Particle, SpriteBatch cSpriteBatch)
		{
			// Cast the Particle to the type it really is
			SpriteParticleSystemTemplateParticle cParticle = (SpriteParticleSystemTemplateParticle)Particle;

			//-----------------------------------------------------------
			// TODO: Put the Particle's properties into the required format for the 
			// SpriteBatch's Draw() function, and add any more parameters to the Draw()
			// function that you like.
			//-----------------------------------------------------------

			// Get the Position and Dimensions of the Particle in 2D space
			Rectangle sDestination = new Rectangle((int)cParticle.Position.X, (int)cParticle.Position.Y,
												   (int)cParticle.Width, (int)cParticle.Height);

			// Draw the Sprite
			cSpriteBatch.Draw(Texture, sDestination, Color.White);
		}

		/// <summary>
		/// Function to set the Shaders global variables before drawing
		/// </summary>
		protected override void SetEffectParameters()
		{
			//-----------------------------------------------------------
			// TODO: Set any global Shader variables required before drawing
			//-----------------------------------------------------------
		}

		/// <summary>
		/// Function to setup the Render Properties (i.e. BlendState, DepthStencilState, RasterizerState, and SamplerState)
		/// which will be applied to the Graphics Device before drawing the Particle System's Particles.
		/// <para>This function is called when initializing the particle system.</para>
		/// </summary>
		protected override void InitializeRenderProperties()
		{
			//-----------------------------------------------------------
			// TODO: Set any render properties required before drawing
			//-----------------------------------------------------------
		}

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
			// TODO: Change any Initialization parameters desired
			//-----------------------------------------------------------
			// Initialize the Particle System before doing anything else
			InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, "Textures/Bubble", cSpriteBatch);

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

			// Set the Function to use to Initialize new Particles
			ParticleInitializationFunction = InitializeParticleProperties;

			// Remove all Events first so that none are added twice if this function is called again
			ParticleEvents.RemoveAllEvents();
			ParticleSystemEvents.RemoveAllEvents();

			// Make the Particles move according to their Velocity
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);

			// Set the Particle System's Emitter to toggle on and off every 0.5 seconds
			ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
			ParticleSystemEvents.LifetimeData.Lifetime = 1.0f;
			ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemEmitParticlesAutomaticallyOn);
			ParticleSystemEvents.AddTimedEvent(0.5f, UpdateParticleSystemEmitParticlesAutomaticallyOff);

			// Setup the Emitter so Particles come from the bottom center of the screen
			Emitter.ParticlesPerSecond = 25;
			Emitter.PositionData.Position = new Vector3((GraphicsDevice.Viewport.Width / 2) - (mfSizeMax / 2), GraphicsDevice.Viewport.Height - mfSizeMax, 0);
		}

		/// <summary>
		/// Function to Initialize a new Particle's properties
		/// </summary>
		/// <param name="cParticle">The Particle to be Initialized</param>
		public void InitializeParticleProperties(SpriteParticleSystemTemplateParticle cParticle)
		{
			//-----------------------------------------------------------
			// TODO: Initialize all of the Particle's properties here.
			// In addition to initializing the Particle properties you added, you
			// must also initialize the Lifetime property that is inherited from DPSFParticle
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

			// Set the Particle's Width and Height
			cParticle.Width = RandomNumber.Next((int)mfSizeMin, (int)mfSizeMax);
			cParticle.Height = RandomNumber.Next((int)mfSizeMin, (int)mfSizeMax);
		}

		//===========================================================
		// Particle Update Functions
		//===========================================================

		//-----------------------------------------------------------
		// TODO: Place your Particle Update functions here, using the 
		// same function prototype as below (i.e. public void FunctionName(DPSFParticle, float))
		//-----------------------------------------------------------

		/// <summary>
		/// Update a Particle's Position according to its Velocity
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticlePositionUsingVelocity(SpriteParticleSystemTemplateParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Update the Particle's Position according to its Velocity
			cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
		}

		//===========================================================
		// Particle System Update Functions
		//===========================================================

		//-----------------------------------------------------------
		// TODO: Place your Particle System Update functions here, using 
		// the same function prototype as below (i.e. public void FunctionName(float))
		//-----------------------------------------------------------

		/// <summary>
		/// Sets the Emitter to Emit Particles Automatically
		/// </summary>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleSystemEmitParticlesAutomaticallyOn(float fElapsedTimeInSeconds)
		{
			Emitter.EmitParticlesAutomatically = true;
		}

		/// <summary>
		/// Sets the Emitter to not Emit Particles Automatically
		/// </summary>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleSystemEmitParticlesAutomaticallyOff(float fElapsedTimeInSeconds)
		{
			Emitter.EmitParticlesAutomatically = false;
		}

		//===========================================================
		// Other Particle System Functions
		//===========================================================

		//-----------------------------------------------------------
		// TODO: Place any other functions here
		//-----------------------------------------------------------
	}
}
