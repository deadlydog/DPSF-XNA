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
	public class ExplosionFlyingSparksParticleSystem : DefaultTexturedQuadTextureCoordinatesParticleSystem
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ExplosionFlyingSparksParticleSystem(Game game) : base(game) { }

		//===========================================================
		// Structures and Variables
		//===========================================================

		/// <summary>
		/// The Color of the explosion.
		/// </summary>
		public Color ExplosionColor { get; set; }

		/// <summary>
		/// The Size of the individual Particles.
		/// </summary>
		public int ExplosionParticleSize { get; set; }

		/// <summary>
		/// The Intensity of the explosion.
		/// </summary>
		public int ExplosionIntensity { get; set; }

		Rectangle _sparkTextureCoordinates = new Rectangle(384, 445, 128, 13);
		float _textureAspectRatio = 13f / 128f;

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

			Name = "Explosion - Flying Sparks";
			LoadEvents();
		}

		public void LoadEvents()
		{
			// Specify the particle initialization function
			ParticleInitializationFunction = InitializeParticleExplosion;

			// Setup the behaviors that the particles should have
			ParticleEvents.RemoveAllEvents();
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);

			// Setup the emitter
			Emitter.PositionData.Position = new Vector3(0, 50, 0);
			Emitter.ParticlesPerSecond = 10000;
			Emitter.EmitParticlesAutomatically = false; // We will call the Explode() function to release a burst of particles instead of always emitting them

			// Set the default explosion settings
			ExplosionColor = new Color(255, 120, 0);
			ExplosionParticleSize = 10;
			ExplosionIntensity = 25;
		}

		public void SetupToAutoExplodeEveryInterval(float intervalInSeconds)
		{
			// Set the Particle System's Emitter to release a burst of particles after a set interval
			ParticleSystemEvents.RemoveAllEventsInGroup(1);
			ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
			ParticleSystemEvents.LifetimeData.Lifetime = intervalInSeconds;
			ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemToExplode, 0, 1);
		}

		public void InitializeParticleExplosion(DefaultTextureQuadTextureCoordinatesParticle particle)
		{
			particle.Lifetime = RandomNumber.Between(0.5f, 1.0f);
			particle.Color = ExplosionColor;
			particle.Position = Emitter.PositionData.Position;
			particle.Velocity = DPSFHelper.RandomNormalizedVector() * RandomNumber.Next(175, 225);
			particle.Right = -particle.Velocity;
			particle.Width = ExplosionParticleSize;
			particle.Height = ExplosionParticleSize * _textureAspectRatio;

			// Set the spark particle's texture coordinates
			particle.SetTextureCoordinates(_sparkTextureCoordinates, Texture.Width, Texture.Height);
		}

		//===========================================================
		// Particle Update Functions
		//===========================================================

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
			this.Emitter.BurstParticles = this.ExplosionIntensity;
		}

		/// <summary>
		/// Change the color of the explosion to a random color.
		/// </summary>
		public void ChangeExplosionColor()
		{
			ExplosionColor = DPSFHelper.RandomColor();
		}

		/// <summary>
		/// Change the color of the explosion to the given color.
		/// </summary>
		/// <param name="color">The color the explosion should be.</param>
		public void ChangeExplosionColor(Color color)
		{
			ExplosionColor = color;
		}
	}
}
