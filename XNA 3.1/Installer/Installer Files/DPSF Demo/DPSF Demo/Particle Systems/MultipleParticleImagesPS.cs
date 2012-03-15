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
	class MultipleParticleImagesParticleSystem : DefaultPointSpriteTextureCoordinatesParticleSystem
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MultipleParticleImagesParticleSystem(Game cGame) : base(cGame) { }

		//===========================================================
		// Structures and Variables
		//===========================================================

		// Specify the texture coordinates of the images we want to use for the particles
		Rectangle msCloudTextureCoordinates = new Rectangle(0, 0, 128, 128);
		Rectangle msSparkTextureCoordinates = new Rectangle(384, 0, 128, 128);
		Rectangle msRockTextureCoordinates = new Rectangle(254, 256, 128, 128);
		Rectangle msRingTextureCoordinates = new Rectangle(382, 255, 130, 130);

		// How much the Particle should bounce back off of the floor
		public float mfBounciness = 0.35f;

		//===========================================================
		// Overridden Particle System Functions
		//===========================================================

		protected override void SetRenderState(RenderState cRenderState)
		{
			base.SetRenderState(cRenderState);
			cRenderState.DepthBufferWriteEnable = true;	// Turn on Depth Sorting (i.e. Z-buffer)
		}

		//===========================================================
		// Initialization Functions
		//===========================================================

		public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
		{
			InitializePointSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000,
												UpdateVertexProperties, "Textures/ExplosionParticles");
			Name = "Multiple Particle Images";
			Emitter.ParticlesPerSecond = 5;
			LoadEvents();
		}

		public void LoadEvents()
		{
			ParticleInitializationFunction = InitializeParticleWithTextureCoordinates;

			ParticleEvents.RemoveAllEvents();
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
			ParticleEvents.AddEveryTimeEvent(BounceOffFloor, 200);

			Emitter.PositionData.Position = new Vector3(0, 50, 0);

			InitialProperties.LifetimeMin = 8.0f;
			InitialProperties.LifetimeMax = 8.0f;
			InitialProperties.PositionMin = new Vector3(-100, 100, -100);
			InitialProperties.PositionMax = new Vector3(100, 100, 100);
			InitialProperties.StartSizeMin = 30.0f;
			InitialProperties.StartSizeMax = 30.0f;
			InitialProperties.EndSizeMin = 30.0f;
			InitialProperties.EndSizeMax = 30.0f;
			InitialProperties.StartColorMin = Color.Black;
			InitialProperties.StartColorMax = Color.White;
			InitialProperties.EndColorMin = Color.Black;
			InitialProperties.EndColorMax = Color.White;
			InitialProperties.InterpolateBetweenMinAndMaxColors = false;
			InitialProperties.VelocityMin = new Vector3(-50, -50, -50);
			InitialProperties.VelocityMax = new Vector3(50, -100, 50);
			InitialProperties.AccelerationMin = new Vector3(0, -50, 0);
			InitialProperties.AccelerationMax = new Vector3(0, -50, 0);
			InitialProperties.RotationalVelocityMin = -MathHelper.PiOver2;
			InitialProperties.RotationalVelocityMax = MathHelper.PiOver2;
		}

		public void InitializeParticleWithTextureCoordinates(DefaultPointSpriteTextureCoordinatesParticle cParticle)
		{
			// Initialize the particle using the InitialProperties specified above
			InitializeParticleUsingInitialProperties(cParticle);
			
			// Randomly pick which texture coordinates to use for this particle
			Rectangle sTextureCoordinates;
			switch (RandomNumber.Next(0, 4))
			{
				default:
				case 0: sTextureCoordinates = msCloudTextureCoordinates; break;
				case 1: sTextureCoordinates = msSparkTextureCoordinates; break;
				case 2: sTextureCoordinates = msRockTextureCoordinates; break;
				case 3: sTextureCoordinates = msRingTextureCoordinates; break;
			}

			// Set the Particle's Texture Coordinates
			cParticle.SetTextureCoordinates(sTextureCoordinates, Texture.Width, Texture.Height);
		}
		
		//===========================================================
		// Particle Update Functions
		//===========================================================
        protected void BounceOffFloor(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
		{
			// If the Particle has hit the floor and is still travelling downwards
			if (cParticle.Position.Y <= 0 && cParticle.Velocity.Y < 0)
			{
				// Make the Particle Bounce upwards
				cParticle.Velocity.Y *= -mfBounciness;

				// Reduce the Particles X and Z speed
				cParticle.Velocity.X *= 0.8f;
				cParticle.Velocity.Z *= 0.8f;

				// Reduce the Particles Rotation speed
				cParticle.RotationalVelocity *= 0.8f;
			}
		}

		//===========================================================
		// Particle System Update Functions
		//===========================================================

		//===========================================================
		// Other Particle System Functions
		//===========================================================

	}
}
