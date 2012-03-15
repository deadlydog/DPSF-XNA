#region File Description
//===================================================================
// DefaultQuadParticleSystemTemplate.cs
// 
// This file provides the template for creating a new Quad Particle
// System that inherits from the Default Quad Particle System.
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
#if (WINDOWS)
	[Serializable]
#endif
	class SphereParticle : DefaultTexturedQuadParticle
	{
        /// <summary>
        /// The position of the particle on the sphere, independent of the emitter's position.
        /// </summary>
		public Vector3 sEmitterIndependentPosition;

        /// <summary>
        /// How fast the particle is rotating around the sphere's origin.
        /// </summary>
		public Vector3 sPivotRotationVelocity;

		public override void Reset()
		{
			base.Reset();
			sEmitterIndependentPosition = Vector3.Zero;
			sPivotRotationVelocity = Vector3.Zero;
		}

		public override void CopyFrom(DPSFParticle ParticleToCopy)
		{
			base.CopyFrom(ParticleToCopy);
			SphereParticle cParticle = (SphereParticle)ParticleToCopy;
			sEmitterIndependentPosition = cParticle.sEmitterIndependentPosition;
			sPivotRotationVelocity = cParticle.sPivotRotationVelocity;
		}
	}

	/// <summary>
	/// Create a new Particle System class that inherits from a Default DPSF Particle System.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	class SphereParticleSystem : DPSFDefaultTexturedQuadParticleSystem<SphereParticle, DefaultTexturedQuadParticleVertex>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
		/// parameter if not using a Game object.</param>
		public SphereParticleSystem(Game cGame) : base(cGame) { }

		//===========================================================
		// Structures and Variables
		//===========================================================

		int miNumberOfParticles = 100;
		float mfSphereRadius = 50;
		float mfParticlePivotRotationMaxSpeed = MathHelper.PiOver2;
		Vector3 mfParticlePivotRotationDirection = DPSFHelper.RandomNormalizedVector();

		//===========================================================
		// Overridden Particle System Functions
		//===========================================================
		/// <summary>
		/// Function to setup the Render Properties (i.e. BlendState, DepthStencilState, RasterizerState, and SamplerState)
		/// which will be applied to the Graphics Device before drawing the Particle System's Particles.
		/// <para>This function is called when initializing the particle system.</para>
		/// </summary>
		protected override void InitializeRenderProperties()
		{
			base.InitializeRenderProperties();
			// Use additive blending
			RenderProperties.BlendState = BlendState.Additive;
		}

		//===========================================================
		// Initialization Functions
		//===========================================================

		/// <summary>
		/// Function to Initialize the Particle System with default values
		/// </summary>
		/// <param name="cGraphicsDevice">The Graphics Device to draw to</param>
		/// <param name="cContentManager">The Content Manager to use to load Textures and Effect files</param>
		public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
		{
			// Initialize the Particle System before doing anything else
			InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, miNumberOfParticles, miNumberOfParticles, 
												UpdateVertexProperties, "Textures/Particle");

			// Set the Name of the Particle System
			Name = "Sphere";

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

			// Allow the Particle's Velocity, Rotational Velocity, Width and Height, Color, Transparency, and Orientation to be updated each frame
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionToRotateAroundEmitter);

			// This function must be executed after the Color Lerp function as the Color Lerp will overwrite the Color's
			// Transparency value, so we give this function an Execution Order of 100 to make sure it is executed last.
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);

			ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 200);

			// Setup the Emitter
			Emitter.ParticlesPerSecond = 100;
			Emitter.PositionData.Position = new Vector3(0, 60, 0);

			MaxNumberOfParticlesAllowed = miNumberOfParticles;
		}

		/// <summary>
		/// Example of how to create a Particle Initialization Function
		/// </summary>
		/// <param name="cParticle">The Particle to be Initialized</param>
		public void InitializeParticleProperties(SphereParticle cParticle)
		{
			//-----------------------------------------------------------
			// TODO: Initialize all of the Particle's properties here.
			// If you plan on simply using the default InitializeParticleUsingInitialProperties
			// Particle Initialization Function (see the LoadParticleSystem() function above), 
			// then you may delete this function all together.
			//-----------------------------------------------------------
			cParticle.Lifetime = 0.0f;

			// Set the Particle's initial Position to be wherever the Emitter is
			cParticle.Position = Emitter.PositionData.Position;

			// Set the Particle to be Radius amount away from the Emitter
			cParticle.sEmitterIndependentPosition.X = mfSphereRadius;
			
			// Rotate the Particle to start somewhere on the surface of the sphere
			cParticle.sEmitterIndependentPosition = DPSFHelper.PointOnSphere(DPSFHelper.RandomNumberBetween(0, MathHelper.TwoPi), DPSFHelper.RandomNumberBetween(0, MathHelper.TwoPi), mfSphereRadius);

			cParticle.Size = 20;

			// Give the Particle a random Color
			// Since we have Color Lerp enabled we must also set the Start and End Color
			cParticle.Color = DPSFHelper.RandomColor();

			cParticle.sPivotRotationVelocity = DPSFHelper.RandomNormalizedVector() * mfParticlePivotRotationMaxSpeed * RandomNumber.NextFloat();
		}

		//===========================================================
		// Particle Update Functions
		//===========================================================

		/// <summary>
		/// Rotate a Particle around the Emitter
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticlePositionToRotateAroundEmitter(SphereParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Calculate how much to rotate this frame and Rotate the Particle's Position
			Vector3 sRotationAmount = cParticle.sPivotRotationVelocity * fElapsedTimeInSeconds;
			Matrix sRotation = Matrix.CreateFromYawPitchRoll(sRotationAmount.Y, sRotationAmount.X, sRotationAmount.Z);

			// Rotate the particle around the Emitter
			cParticle.sEmitterIndependentPosition = PivotPoint3D.RotatePosition(sRotation, Vector3.Zero, cParticle.sEmitterIndependentPosition);
			cParticle.Position = cParticle.sEmitterIndependentPosition + Emitter.PositionData.Position;
		}

		protected void UpdateParticleDistanceFromEmitter(SphereParticle cParticle, float fElapsedTimeInSeconds)
		{
			Vector3 sDirectionToParticle = cParticle.sEmitterIndependentPosition;
			sDirectionToParticle.Normalize();
			cParticle.sEmitterIndependentPosition = sDirectionToParticle * mfSphereRadius;
		}

		protected void UpdateParticlePivotRotationVelocityRandomly(SphereParticle cParticle, float fElapsedTimeInSeconds)
		{
			cParticle.sPivotRotationVelocity = DPSFHelper.RandomNormalizedVector() * mfParticlePivotRotationMaxSpeed * RandomNumber.NextFloat();
		}

		protected void UpdateParticlePivotRotationVelocityToBeTheSame(SphereParticle cParticle, float fElapsedTimeInSeconds)
		{
			cParticle.sPivotRotationVelocity = mfParticlePivotRotationDirection * mfParticlePivotRotationMaxSpeed * RandomNumber.NextFloat();
		}

		//===========================================================
		// Particle System Update Functions
		//===========================================================

		//===========================================================
		// Other Particle System Functions
		//===========================================================

		public void ChangeSphereRadius(float fAmountToChange)
		{
			mfSphereRadius += fAmountToChange;
			if (mfSphereRadius < 20)
			{
				mfSphereRadius = 20;
			}
			ParticleEvents.AddOneTimeEvent(UpdateParticleDistanceFromEmitter);
		}

		public void MakeParticlesTravelInTheSameDirection()
		{
			mfParticlePivotRotationDirection = DPSFHelper.RandomNormalizedVector();
			ParticleEvents.AddOneTimeEvent(UpdateParticlePivotRotationVelocityToBeTheSame);
		}

		public void MakeParticlesTravelInRandomDirections()
		{
			ParticleEvents.AddOneTimeEvent(UpdateParticlePivotRotationVelocityRandomly);
		}

		public void ChangeNumberOfParticles(int iAmountToChange)
		{
			MaxNumberOfParticlesAllowed += iAmountToChange;

			if (MaxNumberOfParticlesAllowed < 50)
			{
				MaxNumberOfParticlesAllowed = 50;
			}
			NumberOfParticlesAllocatedInMemory = MaxNumberOfParticlesAllowed;
		}
	}
}
