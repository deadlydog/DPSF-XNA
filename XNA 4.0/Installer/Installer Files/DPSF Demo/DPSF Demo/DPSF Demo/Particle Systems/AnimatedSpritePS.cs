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
	class AnimatedSpriteParticleSystem : DefaultAnimatedSpriteParticleSystem
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public AnimatedSpriteParticleSystem(Game cGame) : base(cGame) { }

		//===========================================================
		// Structures and Variables
		//===========================================================
		Animations mcExplosionAnimation = null;
		Animations mcButterflyAnimation = null;
		int miNumberOfPicturesInAnimation = 16;
		float mfTimeBetweenAnimationImages = 0.08f; // Default Animation speed
		float mfMinTimeBetweenAnimationImages = 0.04f;  // Fastest speed
		float mfMaxTimeBetweenAnimationImages = 0.1f;   // Slowest speed

		int miButterflyMaxSpeed = 200;
		Vector3 mcMousePosition = new Vector3();

		int miScreenWidth = 0;
		int miScreenHeight = 0;

	#if (WINDOWS)
		[Serializable]
	#endif
		enum EColorAmounts
		{
			None = 0,
			Some = 1,
			All = 2,
			LastInList = 2
		}
		EColorAmounts meColorAmount = EColorAmounts.None;

		//===========================================================
		// Overridden Particle System Functions
		//===========================================================
		protected override void  AfterInitialize()
		{
			base.AfterInitialize();

			// Setup the Animation
			mcExplosionAnimation = new Animations();
			mcButterflyAnimation = new Animations();

			// The Order of the Picture IDs to make up the Animation
			int[] iaAnimationOrder = new int[miNumberOfPicturesInAnimation];
			for (int iIndex = 0; iIndex < miNumberOfPicturesInAnimation; iIndex++)
			{
				iaAnimationOrder[iIndex] = iIndex;
			}

			// Create the Pictures and Animation and Set the Animation to use for Explosions
			mcExplosionAnimation.CreatePicturesFromTileSet(miNumberOfPicturesInAnimation, 16, new Rectangle(0, 0, 64, 64));
			int iAnimationID = mcExplosionAnimation.CreateAnimation(iaAnimationOrder, mfTimeBetweenAnimationImages, 1);
			mcExplosionAnimation.CurrentAnimationID = iAnimationID;

			// Create the Pictures and Animation and Set the Animation to use for Butterflies
			mcButterflyAnimation.CreatePicturesFromTileSet(16, 4, new Rectangle(0, 0, 128, 128));
			iAnimationID = mcButterflyAnimation.CreateAnimation(iaAnimationOrder, mfTimeBetweenAnimationImages, 0);
			mcButterflyAnimation.CurrentAnimationID = iAnimationID;
		}

		protected override void AfterDestroy()
		{
			base.AfterDestroy();
			mcExplosionAnimation = null;
			mcButterflyAnimation = null;
		}

		protected override void SetEffectParameters()
		{
			base.SetEffectParameters();

			float fColorAmount = 0.0f;

			switch (meColorAmount)
			{
				default:
				case EColorAmounts.None:
					fColorAmount = 0.0f;
				break;

				case EColorAmounts.Some:
					fColorAmount = 0.5f;
				break;

				case EColorAmounts.All:
					fColorAmount = 1.0f;
				break;
			}

			// Show only the Textures Color (do not blend with Particle Color)
			Effect.Parameters["xColorBlendAmount"].SetValue(fColorAmount);
		}

		/// <summary>
		/// Function to setup the Render Properties (i.e. BlendState, DepthStencilState, RasterizerState, and SamplerState)
		/// which will be applied to the Graphics Device before drawing the Particle System's Particles.
		/// <para>This function is called when initializing the particle system.</para>
		/// </summary>
		protected override void InitializeRenderProperties()
		{
			base.InitializeRenderProperties();

			// Use the old custom DPSF effect (instead of the Windows-Phone-friendly default SpriteBatch effect) to support the meColorAmount property.
			SetEffectAndTechnique(DPSFDefaultEffect, DPSFDefaultEffectTechniques.Sprites.ToString());

			// Use DepthRead to eliminate nasty black box artefacts
			RenderProperties.DepthStencilState = DepthStencilState.DepthRead;
		}

		//===========================================================
		// Initialization Functions
		//===========================================================

		public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
		{
			InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000,
											"Textures/AnimatedExplosion", cSpriteBatch);

			miScreenWidth = GraphicsDevice.Viewport.Width;
			miScreenHeight = GraphicsDevice.Viewport.Height;

			LoadExplosionEvents();
		}

		public void LoadExplosionEvents()
		{
			Name = "Animated Sprite Explosion";
			SetTexture("Textures/AnimatedExplosion");

			RemoveAllParticles();
			ParticleInitializationFunction = InitializeParticleAnimatedExplosion;

			ParticleEvents.RemoveAllEvents();
			ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleDepthFromFrontToBackUsingLerp);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleAnimationAndTextureCoordinates, 500);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleToDieOnceAnimationFinishesPlaying, 1000);

			ParticleSystemEvents.RemoveAllEvents();
			ParticleSystemEvents.AddEveryTimeEvent(UpdateParticleSystemToSortParticlesByDepth, 50);

			Emitter.ParticlesPerSecond = 50;
			Emitter.PositionData.Position = new Vector3(miScreenWidth / 2, miScreenHeight / 2, 0);

			InitialProperties.LifetimeMin = mcExplosionAnimation.TimeRequiredToPlayCurrentAnimation;
			InitialProperties.LifetimeMax = mcExplosionAnimation.TimeRequiredToPlayCurrentAnimation;
			InitialProperties.PositionMin = Vector3.Zero;
			InitialProperties.PositionMax = Vector3.Zero;
			InitialProperties.VelocityMin = Vector3.Zero;
			InitialProperties.VelocityMax = Vector3.Zero;
			InitialProperties.AccelerationMin = Vector3.Zero;
			InitialProperties.AccelerationMax = Vector3.Zero;
			InitialProperties.FrictionMin = 0.0f;
			InitialProperties.FrictionMax = 0.0f;
			InitialProperties.StartColorMin = Color.Red;
			InitialProperties.StartColorMax = Color.White;
			InitialProperties.EndColorMin = Color.Blue;
			InitialProperties.EndColorMax = Color.White;
			InitialProperties.InterpolateBetweenMinAndMaxColors = false;
			InitialProperties.StartWidthMin = 100.0f;
			InitialProperties.StartWidthMax = 200.0f;
			InitialProperties.StartHeightMin = InitialProperties.StartWidthMin;
			InitialProperties.StartHeightMax = InitialProperties.StartWidthMax;
			InitialProperties.EndWidthMin = 200.0f;
			InitialProperties.EndWidthMax = 300.0f;
			InitialProperties.EndHeightMin = InitialProperties.EndWidthMin;
			InitialProperties.EndHeightMax = InitialProperties.EndWidthMax;
			InitialProperties.RotationMin = 0.0f;
			InitialProperties.RotationMax = MathHelper.Pi;
			InitialProperties.RotationalVelocityMin = 0.0f;
			InitialProperties.RotationalVelocityMax = 0.0f;
			InitialProperties.RotationalAccelerationMin = 0.0f;
			InitialProperties.RotationalAccelerationMax = 0.0f;
		}

		public void InitializeParticleAnimatedExplosion(DefaultAnimatedSpriteParticle cParticle)
		{
			InitializeParticleUsingInitialProperties(cParticle);
			cParticle.Animation.CopyFrom(mcExplosionAnimation);
		}

		public void LoadButterflyEvents()
		{
			Name = "Animated Sprite Butterflies";
			SetTexture("Textures/AnimatedButterfly");

			RemoveAllParticles();
			ParticleInitializationFunction = InitializeParticleAnimatedButterfly;

			ParticleEvents.RemoveAllEvents();
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
			ParticleEvents.AddEveryTimeEvent(ChangeDirectionRandomly);
			ParticleEvents.AddEveryTimeEvent(BounceOffScreenEdges, 1000);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleDepthFromFrontToBackUsingLerp);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleAnimationAndTextureCoordinates, 500);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleToDieOnceAnimationFinishesPlaying, 1000);

			ParticleSystemEvents.AddEveryTimeEvent(UpdateParticleSystemToSortParticlesByDepth, 50);

			Emitter.ParticlesPerSecond = 0;
			Emitter.PositionData.Position = new Vector3(miScreenWidth / 2, miScreenHeight / 2, 0);

			InitialProperties.LifetimeMin = 7.0f;
			InitialProperties.LifetimeMax = 10.0f;
			InitialProperties.PositionMin = Vector3.Zero;
			InitialProperties.PositionMax = Vector3.Zero;
			InitialProperties.VelocityMin = Vector3.Zero;
			InitialProperties.VelocityMax = Vector3.Zero;
			InitialProperties.AccelerationMin = Vector3.Zero;
			InitialProperties.AccelerationMax = Vector3.Zero;
			InitialProperties.FrictionMin = 0.0f;
			InitialProperties.FrictionMax = 0.0f;
			InitialProperties.StartColorMin = Color.Red;
			InitialProperties.StartColorMax = Color.White;
			InitialProperties.EndColorMin = Color.Blue;
			InitialProperties.EndColorMax = Color.White;
			InitialProperties.InterpolateBetweenMinAndMaxColors = false;
			InitialProperties.StartWidthMin = 150.0f;
			InitialProperties.StartWidthMax = 150.0f;
			InitialProperties.StartHeightMin = InitialProperties.StartWidthMin;
			InitialProperties.StartHeightMax = InitialProperties.StartWidthMax;
			InitialProperties.EndWidthMin = 150.0f;
			InitialProperties.EndWidthMax = 150.0f;
			InitialProperties.EndHeightMin = InitialProperties.EndWidthMin;
			InitialProperties.EndHeightMax = InitialProperties.EndWidthMax;
			InitialProperties.RotationMin = MathHelper.PiOver2;
			InitialProperties.RotationMax = MathHelper.PiOver2;
			InitialProperties.RotationalVelocityMin = 0.0f;
			InitialProperties.RotationalVelocityMax = 0.0f;
			InitialProperties.RotationalAccelerationMin = 0.0f;
			InitialProperties.RotationalAccelerationMax = 0.0f;

			// Add the Mouse Butterfly Particle
			AddParticle();
		}

		public void InitializeParticleAnimatedButterfly(DefaultAnimatedSpriteParticle cParticle)
		{
			InitializeParticleUsingInitialProperties(cParticle);
			cParticle.Animation.CopyFrom(mcButterflyAnimation);

			// If this is the first Particle to be added
			if (NumberOfActiveParticles == 0)
			{
				// Set the Particle to live forever
				cParticle.Lifetime = -1.0f;

				// Place the Particle in the middle of the screen to begin with
				cParticle.Position = new Vector3(miScreenWidth / 2, miScreenHeight / 2, 0);

				// Make sure it does not move on it's own
				cParticle.Velocity = Vector3.Zero;
				cParticle.Acceleration = Vector3.Zero;
				cParticle.ExternalForce = Vector3.Zero;

				// Set the Animation to play forever (since the particles die when the animation ends)
				cParticle.Animation.CurrentAnimationsNumberOfTimesToPlay = 0;
			}
			else
			{
				// Set this new Particle to start from the same position in the Animation as the Mouse Butterfly current is
				cParticle.Animation.CurrentAnimationsPictureRotationOrderIndex = ActiveParticles.Last.Value.Animation.CurrentAnimationsPictureRotationOrderIndex;

				// Have this butterfly go in a random direction
				ChangeDirectionRandomly(cParticle, -1);
			}
		}

		//===========================================================
		// Particle Update Functions
		//===========================================================

		protected void BounceOffScreenEdges(DefaultAnimatedSpriteParticle cParticle, float fElapsedTimeInSeconds)
		{
			int iLeft = (int)(cParticle.Position.X - (cParticle.Width / 4));
			int iRight = (int)(cParticle.Position.X + (cParticle.Width / 4));
			int iTop = (int)(cParticle.Position.Y - (cParticle.Height / 4));
			int iBottom = (int)(cParticle.Position.Y + (cParticle.Height / 4));

			// If the Particle is heading off the left or right side of the screen
			if ((iLeft < 0 && cParticle.Velocity.X < 0) || 
				(iRight > miScreenWidth && cParticle.Velocity.X > 0))
			{
				// Reverse it's horizontal direction
				cParticle.Velocity.X *= -1;
			}

			// If the Particle is heading off the top or bottom of the screen
			if ((iTop < 0 && cParticle.Velocity.Y < 0) || 
				(iBottom > miScreenHeight && cParticle.Velocity.Y > 0))
			{
				// Reverse it's vertical direction
				cParticle.Velocity.Y *= -1;
			}

			// Make sure the Butterfly is still orientated properly
			MakeButterflyFaceProperDirectionAndAdjustAnimationSpeed(cParticle);
		}

		protected void ChangeDirectionRandomly(DefaultAnimatedSpriteParticle cParticle, float fElapsedTimeInSeconds)
		{
			// If this is the Mouse butterfly
			if (ActiveParticles.Last != null && cParticle == ActiveParticles.Last.Value)
			{
				// Exit without doing anything
				return;
			}

			// If we should pick a new Direction (happens randomly) (-1 is specified when initializing new particles)
			float fClamped = MathHelper.Clamp(fElapsedTimeInSeconds, -1f, 0.01f);
			if (RandomNumber.NextFloat() < fClamped || fElapsedTimeInSeconds < 0)
			{
				// Calculate a new Velocity direction
				cParticle.Velocity = new Vector3(RandomNumber.Next(-miButterflyMaxSpeed, miButterflyMaxSpeed),
												  RandomNumber.Next(-miButterflyMaxSpeed, miButterflyMaxSpeed),
												  0);

				// Calculate the new Direction the Butterfly should face
				MakeButterflyFaceProperDirectionAndAdjustAnimationSpeed(cParticle);
			}
		}

		//===========================================================
		// Particle System Update Functions
		//===========================================================

		//===========================================================
		// Other Particle System Functions
		//===========================================================
		public void ToggleColorAmount()
		{
			meColorAmount++;

			// If we've gone past the End of the List
			if (meColorAmount > EColorAmounts.LastInList)
			{
				meColorAmount = 0;
			}
		}

		public Vector3 MousePosition
		{
			get { return mcMousePosition; }
			set
			{
				Emitter.PositionData.Position = value;

				if (ActiveParticles.Last != null)
				{
					ActiveParticles.Last.Value.Position = value;
				}
			}
		}

		private void MakeButterflyFaceProperDirectionAndAdjustAnimationSpeed(DefaultAnimatedSpriteParticle cParticle)
		{
			// If the butterfly should face left
			if (cParticle.Velocity.X < 0)
			{
				cParticle.FlipMode = SpriteEffects.FlipVertically;
			}
			// Else it should face right (the image default)
			else
			{
				cParticle.FlipMode = SpriteEffects.None;
			}

			// Make the Butterfly animation go faster based on how fast it is moving upwards
			float fNormalizedYVelocity = (cParticle.Velocity.Y + miButterflyMaxSpeed) / (miButterflyMaxSpeed * 2);
			float fNewAnimationTime = MathHelper.Lerp(mfMinTimeBetweenAnimationImages, mfMaxTimeBetweenAnimationImages, fNormalizedYVelocity);
			cParticle.Animation.CurrentAnimationsPictureRotationTime = fNewAnimationTime;
		}
	}
}
