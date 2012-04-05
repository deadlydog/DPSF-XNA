#region Using Statements
using System;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF_Demo.ParticleSystems
{
	/// <summary>
	/// Create a new Particle System class that inherits from a Default DPSF Particle System.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	class ExplosionShockwaveParticleSystem : DefaultTexturedQuadTextureCoordinatesParticleSystem
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ExplosionShockwaveParticleSystem(Game game) : base(game) { }

		//===========================================================
		// Structures and Variables
		//===========================================================

		/// <summary>
		/// The Color of the explosion.
		/// </summary>
		public Color ShockwaveColor { get; set; }

		/// <summary>
		/// The Size that the shockwave particle should grow to be before fading out.
		/// </summary>
		public int ShockwaveSize { get; set; }

		/// <summary>
		/// How long the shockwave particle should live for.
		/// </summary>
		public float ShockwaveDuration { get; set; }

		/// <summary>
		/// The Transparency of the shockwave. This should be between 0.0 - 1.0.
		/// </summary>
		public float ShockwaveTransparency { get; set; }

		/// <summary>
		/// Get / Set if a shockwave whose normal direction is parallel to the X axis should be created or not.
		/// </summary>
		public bool ShockwaveXAxisEnabled { get; set; }

		/// <summary>
		/// Get / Set if a shockwave whose normal direction is parallel to the Y axis should be created or not.
		/// </summary>
		public bool ShockwaveYAxisEnabled { get; set; }

		/// <summary>
		/// Get / Set if a shockwave whose normal direction is parallel to the Z axis should be created or not.
		/// </summary>
		public bool ShockwaveZAxisEnabled { get; set; }

		/// <summary>
		/// Get / Set if a shockwave whose normal direction is parallel to the X-Y axis should be created or not.
		/// </summary>
		public bool ShockwaveXYAxisEnabled { get; set; }

		/// <summary>
		/// Get / Set if a shockwave whose normal direction is parallel to the X-Z axis should be created or not.
		/// </summary>
		public bool ShockwaveXZAxisEnabled { get; set; }

		/// <summary>
		/// Get / Set if a shockwave whose normal direction is parallel to the Y-Z axis should be created or not.
		/// </summary>
		public bool ShockwaveYZAxisEnabled { get; set; }

		/// <summary>
		/// Get / Set if a shockwave whose normal direction is parallel to the X-Y axis should be created or not.
		/// </summary>
		public bool ShockwaveXYNegativeAxisEnabled { get; set; }

		/// <summary>
		/// Get / Set if a shockwave whose normal direction is parallel to the X-Z axis should be created or not.
		/// </summary>
		public bool ShockwaveXZNegativeAxisEnabled { get; set; }

		/// <summary>
		/// Get / Set if a shockwave whose normal direction is parallel to the Y-Z axis should be created or not.
		/// </summary>
		public bool ShockwaveYZNegativeAxisEnabled { get; set; }

		Rectangle _shockwaveTextureCoordinates = new Rectangle(384, 256, 128, 128);

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
			// Show the texture on both the front and back of the quad
			RenderProperties.RasterizerState.CullMode = CullMode.None;
		}

		//===========================================================
		// Initialization Functions
		//===========================================================
		public override void AutoInitialize(GraphicsDevice graphicsDevice, ContentManager contentManager, SpriteBatch spriteBatch)
		{
			InitializeTexturedQuadParticleSystem(graphicsDevice, contentManager, 1000, 50000,
												UpdateVertexProperties, "Textures/ExplosionParticles");

			Name = "Explosion - Shockwave";
			LoadEvents();
		}

		public void LoadEvents()
		{
			// Specify the particle initialization function
			ParticleInitializationFunction = InitializeParticleShockwave;

			// Setup the behaviors that the particles should have
			ParticleEvents.RemoveAllEvents();
			ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToBeMoreTransparent, 101);

			// Setup the emitter
			Emitter.PositionData.Position = new Vector3(0, 25, 0);
			Emitter.ParticlesPerSecond = 10000;
			Emitter.EmitParticlesAutomatically = false; // We will call the Explode() function to release a burst of particles instead of always emitting them

			// Set the default explosion settings
			ShockwaveColor = new Color(255, 120, 0);
			ShockwaveSize = 300;
			ShockwaveDuration = 0.5f;
			ShockwaveTransparency = 0.25f;
			ShockwaveXAxisEnabled = true;
			ShockwaveYAxisEnabled = true;
			ShockwaveZAxisEnabled = true;
			ShockwaveXYAxisEnabled = true;
			ShockwaveXZAxisEnabled = true;
			ShockwaveYZAxisEnabled = true;
			ShockwaveXYNegativeAxisEnabled = true;
			ShockwaveXZNegativeAxisEnabled = true;
			ShockwaveYZNegativeAxisEnabled = true;
		}

		public void SetupToAutoExplodeEveryInterval(float intervalInSeconds)
		{
			// Set the Particle System's Emitter to release a burst of particles after a set interval
			ParticleSystemEvents.RemoveAllEventsInGroup(1);
			ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
			ParticleSystemEvents.LifetimeData.Lifetime = intervalInSeconds;
			ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemToExplode, 0, 1);
		}

		public void InitializeParticleShockwave(DefaultTextureQuadTextureCoordinatesParticle particle)
		{
			particle.Lifetime = ShockwaveDuration;
			particle.Color = ShockwaveColor;
			particle.Position = Emitter.PositionData.Position;
			particle.Normal = new Vector3(0, 1, 0);
			particle.Size = particle.StartSize = 1;
			particle.EndSize = ShockwaveSize;

			particle.SetTextureCoordinates(_shockwaveTextureCoordinates, Texture.Width, Texture.Height);
		}

		//===========================================================
		// Particle Update Functions
		//===========================================================
		protected void UpdateParticleTransparencyToBeMoreTransparent(DefaultTextureQuadTextureCoordinatesParticle particle, float elapsedTimeInSeconds)
		{
			particle.Color.A = (byte)(particle.Color.A * this.ShockwaveTransparency);
		}

		//===========================================================
		// Particle System Update Functions
		//===========================================================
		protected void UpdateParticleSystemToExplode(float elapsedTimeInSeconds)
		{
			Explode();
		}

		//===========================================================
		// Other Particle System Functions
		//===========================================================

		/// <summary>
		/// Start the explosion.
		/// </summary>
		public void Explode()
		{
			// Create the particle that we will model other particles added to the particle system after (i.e. a Shockwave particle)
			DefaultTextureQuadTextureCoordinatesParticle particle = new DefaultTextureQuadTextureCoordinatesParticle();
			this.InitializeParticleShockwave(particle);

			// If a shockwave should be created, set the model-particle to the proper orientation and add a copy of the model-particle to the particle system.
			if (ShockwaveXAxisEnabled)
			{
				particle.Normal = new Vector3(1, 0, 0);
				this.AddParticle(particle);
			}

			if (ShockwaveYAxisEnabled)
			{
				particle.Normal = new Vector3(0, 1, 0);
				this.AddParticle(particle);
			}

			if (ShockwaveZAxisEnabled)
			{
				particle.Normal = new Vector3(0, 0, 1);
				this.AddParticle(particle);
			}

			if (ShockwaveXYAxisEnabled)
			{
				particle.Normal = new Vector3(1, 1, 0);
				this.AddParticle(particle);
			}

			if (ShockwaveXZAxisEnabled)
			{
				particle.Normal = new Vector3(1, 0, 1);
				this.AddParticle(particle);
			}

			if (ShockwaveYZAxisEnabled)
			{
				particle.Normal = new Vector3(0, 1, 1);
				this.AddParticle(particle);
			}

			if (ShockwaveXYNegativeAxisEnabled)
			{
				particle.Normal = new Vector3(-1, 1, 0);
				this.AddParticle(particle);
			}

			if (ShockwaveXZNegativeAxisEnabled)
			{
				particle.Normal = new Vector3(-1, 0, 1);
				this.AddParticle(particle);
			}

			if (ShockwaveYZNegativeAxisEnabled)
			{
				particle.Normal = new Vector3(0, 1, -1);
				this.AddParticle(particle);
			}
		}

		/// <summary>
		/// Change the color of the explosion to a random color.
		/// </summary>
		public void ChangeExplosionColor()
		{
			ShockwaveColor = DPSFHelper.RandomColor();
		}

		/// <summary>
		/// Change the color of the explosion to the given color.
		/// </summary>
		/// <param name="color">The color the explosion should be.</param>
		public void ChangeExplosionColor(Color color)
		{
			ShockwaveColor = color;
		}
	}
}