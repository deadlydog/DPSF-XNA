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
#if (WINDOWS)
	[Serializable]
#endif
	class Random2DParticleSystem : DefaultTexturedQuadParticleSystem
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Random2DParticleSystem(Game cGame) : base(cGame) { }

		//===========================================================
		// Structures and Variables
		//===========================================================

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
		public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
		{
			InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000,
												UpdateVertexProperties, "Textures/Fire");
			LoadEvents();
			Emitter.ParticlesPerSecond = 100;
			Name = "Random 2D";
		}

		public void LoadEvents()
		{
			ParticleInitializationFunction = InitializeParticleRandom2D;

			Emitter.PositionData.Position = new Vector3(0, 50, 0);

			ParticleEvents.RemoveAllEvents();
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 200);
		}

		public void LoadExtraEvents()
		{
			ParticleInitializationFunction = InitializeParticleRandom2D;

			Emitter.PositionData.Position = new Vector3(0, 50, 0);

			ParticleEvents.RemoveAllEvents();
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 200);

			ParticleEvents.AddNormalizedTimedEvent(0.2f, ChangeDirection);
			ParticleEvents.AddNormalizedTimedEvent(0.4f, ChangeDirection);
			ParticleEvents.AddNormalizedTimedEvent(0.6f, ChangeDirection);
			ParticleEvents.AddNormalizedTimedEvent(0.8f, ChangeDirection);
		}

		public void InitializeParticleRandom2D(DefaultTexturedQuadParticle cParticle)
		{
			cParticle.Lifetime = 1.5f;

			cParticle.Position = Vector3.Zero;
			cParticle.Position = PivotPoint3D.RotatePosition(Matrix.CreateFromQuaternion(Emitter.OrientationData.Orientation), Emitter.PivotPointData.PivotPoint, cParticle.Position);
			cParticle.Position += Emitter.PositionData.Position;
			cParticle.Size = 30.0f;
			cParticle.Color = Color.White;

			cParticle.Velocity = new Vector3(RandomNumber.Next(-50, 50), RandomNumber.Next(-50, 50), 0);
			cParticle.Velocity = PivotPoint3D.RotatePosition(Matrix.CreateFromQuaternion(Emitter.OrientationData.Orientation), Emitter.PivotPointData.PivotPoint, cParticle.Velocity);
			cParticle.Acceleration = Vector3.Zero;

			cParticle.StartSize = cParticle.Size;
		}

		//===========================================================
		// Particle Update Functions
		//===========================================================
		protected void ChangeDirection(DefaultTexturedQuadParticle cParticle, float fElapsedTimeInSeconds)
		{
			cParticle.Velocity = new Vector3(RandomNumber.Next(-50, 50), RandomNumber.Next(-50, 50), 0);
		}

		//===========================================================
		// Particle System Update Functions
		//===========================================================
		
		//===========================================================
		// Other Particle System Functions
		//===========================================================
	}
}