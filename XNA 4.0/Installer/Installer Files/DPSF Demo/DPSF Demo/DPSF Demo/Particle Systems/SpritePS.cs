#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace DPSF.ParticleSystems
{
	// Create a new type of Particle for this Particle System
#if (WINDOWS)
	[Serializable]
#endif
	class SpriteParticle : DefaultSpriteParticle
	{
		public Vector2 sPivotPoint;

		public override void Reset()
		{
			base.Reset();
			sPivotPoint = new Vector2();
		}

		public override void CopyFrom(DPSFParticle ParticleToCopy)
		{
			// Cast the Particle to the type it really is
			SpriteParticle cParticleToCopy = (SpriteParticle)ParticleToCopy;

			base.CopyFrom(cParticleToCopy);
			sPivotPoint = cParticleToCopy.sPivotPoint;
		}
	}

	/// <summary>
	/// Create a new Particle System class that inherits from a
	/// Default DPSF Particle System
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	class SpriteParticleSystem : DPSFDefaultSpriteParticleSystem<SpriteParticle, DefaultSpriteParticleVertex>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public SpriteParticleSystem(Game cGame) : base(cGame) { }

		//===========================================================
		// Structures and Variables
		//===========================================================
		int miScreenWidth = 0;
		int miScreenHeight = 0;

		int miRows = 0;
		int miColumns = 0;

		// The position to attract the Particles to. This could be implemented much easier
		// by using a Default Particle System Magnet instead, but this was implemented before
		// the Magnets so I'm leaving it as an example. To see how to do a similar effect
		// using magnets, see the MagnetsPS.cs file
		Vector3 msAttractorPosition = Vector3.Zero;
		float mfAttractorStrength = 3;
		float mfAttractorAffectDistance = 0;
		float mfGravity = 150;

	#if (WINDOWS)
		[Serializable]
	#endif
		public enum EAttractorModes
		{
			None = 0,
			Attract = 1,
			Repel = 2,
			AttractFriction = 3,
			RepelFriction = 4,
			LastInList = 4
		}
		EAttractorModes meAttractorMode = EAttractorModes.Attract;

		//===========================================================
		// Overridden Particle System Functions
		//===========================================================
		protected override void AfterAddParticle()
		{
			if (Name.Equals("Sprite Force"))
			{
				// We reset the Attractor Mode so that this Particle's Gravity/Friction gets set properly.
				// NOTE: The current implementation of this is poor since doing this will loop through and
				// set the Gravity/Friction for every Active Particle, not just this newly added one, but it
				// is good enough for this demo for now.
				AttractorMode = meAttractorMode;
			}
		}

		protected override void InitializeRenderProperties()
		{
			base.InitializeRenderProperties();
			RenderProperties.BlendState = BlendState.Additive;	// Use additive blending
		}

		//===========================================================
		// Initialization Functions
		//===========================================================
		public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
		{
			InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, "Textures/Smoke");

			Emitter.ParticlesPerSecond = 200;

			miScreenWidth = GraphicsDevice.Viewport.Width;
			miScreenHeight = GraphicsDevice.Viewport.Height;

			LoadAttractionEvents();
		}

		public void LoadCloudEvents()
		{
			Name = "Sprite Cloud";
			SetTexture("Textures/Smoke");

			RemoveAllParticles();
			MaxNumberOfParticlesAllowed = 1000;

			ParticleInitializationFunction = InitializeParticleUsingInitialProperties;

			ParticleEvents.RemoveAllEvents();
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration, 500);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);

			Emitter.PositionData.Position = new Vector3(miScreenWidth / 2, miScreenHeight / 2, 0);
			AttractorMode = EAttractorModes.None;
			msAttractorPosition = new Vector3(miScreenWidth / 2, miScreenHeight / 2, 0);
			mfAttractorAffectDistance = 300;

			InitialProperties.LifetimeMin = 3.0f;
			InitialProperties.LifetimeMax = 3.0f;
			InitialProperties.PositionMin = Vector3.Zero;
			InitialProperties.PositionMax = Vector3.Zero;
			InitialProperties.VelocityMin = new Vector3(-50, -50, -50);
			InitialProperties.VelocityMax = new Vector3(50, 50, 50);
			InitialProperties.AccelerationMin = Vector3.Zero;
			InitialProperties.AccelerationMax = Vector3.Zero;
			InitialProperties.StartColorMin = Color.Red;
			InitialProperties.StartColorMax = Color.White;
			InitialProperties.EndColorMin = Color.Blue;
			InitialProperties.EndColorMax = Color.White;
			InitialProperties.InterpolateBetweenMinAndMaxColors = false;
			InitialProperties.StartWidthMin = 40.0f;
			InitialProperties.StartWidthMax = 60.0f;
			InitialProperties.StartHeightMin = InitialProperties.StartWidthMin;
			InitialProperties.StartHeightMax = InitialProperties.StartWidthMax;
			InitialProperties.EndWidthMin = 60.0f;
			InitialProperties.EndWidthMax = 80.0f;
			InitialProperties.EndHeightMin = InitialProperties.EndWidthMin;
			InitialProperties.EndHeightMax = InitialProperties.EndWidthMax;
			InitialProperties.RotationMin = 0.0f;
			InitialProperties.RotationMax = MathHelper.Pi;
			InitialProperties.RotationalVelocityMin = -MathHelper.Pi;
			InitialProperties.RotationalVelocityMax = MathHelper.Pi;
			InitialProperties.RotationalAccelerationMin = -MathHelper.PiOver2;
			InitialProperties.RotationalAccelerationMax = MathHelper.PiOver2;
			InitialProperties.FrictionMin = mfGravity;
			InitialProperties.FrictionMax = mfGravity;
		}

		public void LoadAttractionEvents()
		{
			Name = "Sprite Force";
			SetTexture("Textures/Particle");

			RemoveAllParticles();
			MaxNumberOfParticlesAllowed = 1500;

			ParticleInitializationFunction = InitializeParticleAttraction;

			ParticleEvents.RemoveAllEvents();
			ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingExternalForce, 100);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingFriction, 200);
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration, 500);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleKeepParticleOnScreen, 1000);

			AttractorMode = EAttractorModes.Attract;
			mfAttractorAffectDistance = 200;
		}

		public void InitializeParticleAttraction(DPSFParticle Particle)
		{
			SpriteParticle cParticle = (SpriteParticle)Particle;

			cParticle.Lifetime = 0;
			cParticle.Width = cParticle.Height = 40;
			cParticle.Position = new Vector3(RandomNumber.Next(0, miScreenWidth), RandomNumber.Next(0, miScreenHeight), 0);
			cParticle.Color = DPSFHelper.RandomColor();
			cParticle.ExternalForce = new Vector3(0, mfGravity, 0);
			cParticle.Friction = mfGravity;

			// If this is the Attractor Particle (i.e. the first Particle added)
			if (NumberOfActiveParticles == 0)
			{
				// Make the Attractor Particle larger than the rest
				cParticle.Width = cParticle.Height = 100;

				// Place the Attractor in the middle of the screen
				cParticle.Position = new Vector3(miScreenWidth / 2, miScreenHeight / 2, 0);

				// Make the Attractor White
				cParticle.Color = Color.White;
			}
		}

		public void LoadGridEvents()
		{
			Name = "Sprite Grid";
			SetTexture("Textures/WhiteSquare");

			RemoveAllParticles();
			miRows = 20;
			miColumns = 20;
			MaxNumberOfParticlesAllowed = miRows + miColumns;

			ParticleInitializationFunction = InitializeParticleGrid;

			ParticleEvents.RemoveAllEvents();
		}

		public void InitializeParticleGrid(DPSFParticle Particle)
		{
			SpriteParticle cParticle = (SpriteParticle)Particle;

			int iLineWidth = 1;

			cParticle.Lifetime = 0;
			cParticle.Color = Color.Green;
			cParticle.RotationalVelocity = MathHelper.PiOver4;

			// If this should be a Horizontal Line
			if (NumberOfActiveParticles < miRows)
			{
				cParticle.Width = miScreenWidth;
				cParticle.Height = iLineWidth;

				float fSpaceBetweenLines = miScreenHeight / miRows;

				float fX = miScreenWidth / 2;
				float fY = (fSpaceBetweenLines / 2) + (NumberOfActiveParticles * fSpaceBetweenLines) - (iLineWidth / 2);
				cParticle.Position = new Vector3(fX, fY, 0);
			}
			// Else it should be a Vertical Line
			else
			{
				cParticle.Width = iLineWidth;
				cParticle.Height = miScreenHeight;

				float fSpaceBetweenLines = miScreenWidth / miColumns;

				float fX = (fSpaceBetweenLines / 2) + ((NumberOfActiveParticles - miRows) * fSpaceBetweenLines) - (iLineWidth / 2);
				float fY = miScreenHeight / 2;
				cParticle.Position = new Vector3(fX, fY, 0);
			}
		}

		public void LoadRotatorsEvents()
		{
			Name = "Sprite Rotators";
			SetTexture("Textures/Bubble");

			RemoveAllParticles();
			miRows = 3;
			miColumns = 3;
			MaxNumberOfParticlesAllowed = miRows * miColumns;

			ParticleInitializationFunction = InitializeParticleRotators;

			ParticleEvents.RemoveAllEvents();
			ParticleEvents.AddEveryTimeEvent(UpdateParticleRotateParticleAroundPivotPoint);
		}

		public void InitializeParticleRotators(DPSFParticle Particle)
		{
			SpriteParticle cParticle = (SpriteParticle)Particle;

			cParticle.Lifetime = 0;
			cParticle.Color = DPSFHelper.RandomColor();
			cParticle.Width = cParticle.Height = 40;

			int iWidthBetweenParticles = miScreenWidth / miColumns;
			int iHeightBetweenParticles = miScreenHeight / miRows;
			int iRow = NumberOfActiveParticles / miColumns;
			int iColumn = NumberOfActiveParticles % miColumns;

			float fX = (iWidthBetweenParticles / 4) + (iColumn * iWidthBetweenParticles) - (cParticle.Width / 2);
			float fY = (iHeightBetweenParticles / 4) + (iRow * iHeightBetweenParticles) - (cParticle.Height / 2);
			cParticle.Position = new Vector3(fX, fY, 0);

			cParticle.sPivotPoint = new Vector2(fX + (iWidthBetweenParticles / 4), fY + (iHeightBetweenParticles / 4));
			cParticle.RotationalVelocity = RandomNumber.Between(-MathHelper.TwoPi, MathHelper.TwoPi);
		}

		//===========================================================
		// Particle Update Functions
		//===========================================================

		protected void UpdateParticleKeepParticleOnScreen(SpriteParticle cParticle, float fElapsedTimeInSeconds)
		{
			// If this is the Attractor
			if (ActiveParticles.Last != null && cParticle == ActiveParticles.Last.Value)
			{
				// Place the Attractor at the Mouse Position and exit
				ActiveParticles.Last.Value.Position = msAttractorPosition;
				return;
			}

			float fLeft = cParticle.Position.X -(cParticle.Width / 2);
			float fRight = cParticle.Position.X +(cParticle.Width / 2);
			float fTop = cParticle.Position.Y -(cParticle.Height / 2);
			float fBottom = cParticle.Position.Y +(cParticle.Height / 2);

			// Bounce off of sides, losing a bit of speed
			if ((fLeft < 0 && cParticle.Velocity.X < 0) || 
				(fRight > miScreenWidth && cParticle.Velocity.X > 0))
			{
				cParticle.Velocity.X *= -0.75f;

				if (fLeft < 0)
				{
					cParticle.Position.X = (cParticle.Width / 2);
				}
				else
				{
					cParticle.Position.X = miScreenWidth - (cParticle.Width / 2);
				}
			}

			// Bounce off of ceiling, losing some speed
			if (fTop < 0 && cParticle.Velocity.Y < 0)
			{
				cParticle.Velocity.Y *= -0.75f;

				cParticle.Position.Y = (cParticle.Height / 2);
			}
			// Bounce off of floor, losing some speed
			else if(fBottom > miScreenHeight && cParticle.Velocity.Y > 0)
			{
				cParticle.Velocity.Y *= -0.5f;

				// Reposition the Particle to make sure it's not off the screen
				cParticle.Position.Y = miScreenHeight - (cParticle.Height / 2);
			}
		}

		protected void UpdateParticleAttractParticleToAttractor(SpriteParticle cParticle, float fElapsedTimeInSeconds)
		{
			// If this is the Attractor
			if (ActiveParticles.Last == null || cParticle == ActiveParticles.Last.Value)
			{
				// Exit without doing anything
				return;
			}

			Vector3 sVectorToAttractor = msAttractorPosition - cParticle.Position;
			float fDistanceFromAttractor = sVectorToAttractor.Length();

			// If the Particle is close enough to be affected by the Attractor
			if (fDistanceFromAttractor < mfAttractorAffectDistance)
			{
				sVectorToAttractor.Normalize();

				// Attract the Particle towards the Attractor based on its distance from the Attractor
				cParticle.Acceleration = sVectorToAttractor * ((mfAttractorAffectDistance - fDistanceFromAttractor) * mfAttractorStrength);
			}
			// Else the Particle is too far away from the Attractor to be affected by it
			else
			{
				cParticle.Acceleration = Vector3.Zero;
			}
		}

		protected void UpdateParticleRepelParticleFromAttractor(SpriteParticle cParticle, float fElapsedTimeInSeconds)
		{
			// If this is the Attractor
			if (ActiveParticles.Last == null || cParticle == ActiveParticles.Last.Value)
			{
				// Exit without doing anything
				return;
			}

			Vector3 sVectorAwayFromAttractor = cParticle.Position - msAttractorPosition;
			float fDistanceFromAttractor = sVectorAwayFromAttractor.Length();

			// If the Particle is close enough to be affected by the Attractor
			if (fDistanceFromAttractor < mfAttractorAffectDistance)
			{
				sVectorAwayFromAttractor.Normalize();

				// Repel the Particle away from the Attractor based on its distance from the Attractor
				cParticle.Acceleration = sVectorAwayFromAttractor * ((mfAttractorAffectDistance - fDistanceFromAttractor) * mfAttractorStrength);
			}
			// Else the Particle is too far away from the Attractor to be affected by it
			else
			{
				cParticle.Acceleration = Vector3.Zero;
			}
		}

		protected void UpdateParticleRotateParticleAroundPivotPoint(SpriteParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Calculate how much to rotate this frame and Rotate the Particle's Position
			float fRotation = cParticle.RotationalVelocity * fElapsedTimeInSeconds;
			PivotPoint2D.RotatePositionAndOrientationVector3(fRotation, new Vector3(cParticle.sPivotPoint.X, cParticle.sPivotPoint.Y, 0),
															 ref cParticle.Position, ref cParticle.Rotation);
		}

		//===========================================================
		// Particle System Update Functions
		//===========================================================
		
		//===========================================================
		// Other Particle System Functions
		//===========================================================
		public Vector3 AttractorPosition
		{
			get { return msAttractorPosition; }
			set
			{
				Emitter.PositionData.Position = value;

				if (Name.Equals("Sprite Force"))
				{
					if (ActiveParticles.Last != null)
					{
						msAttractorPosition = value;
						ActiveParticles.Last.Value.Position = value;
					}
				}
			}
		}

		public EAttractorModes AttractorMode
		{
			get { return meAttractorMode; }
			set
			{
				meAttractorMode = value;
				ParticleEvents.RemoveAllEventsInGroup(1);

				// If we are in Sprite Cloud mode
				if (Name.Equals("Sprite Cloud"))
				{
					switch (meAttractorMode)
					{
						default:
						case EAttractorModes.None:
						break;

						case EAttractorModes.Attract:
							ParticleEvents.AddEveryTimeEvent(UpdateParticleAttractParticleToAttractor, 400, 1);
						break;

						case EAttractorModes.Repel:
							ParticleEvents.AddEveryTimeEvent(UpdateParticleRepelParticleFromAttractor, 400, 1);
						break;

						case EAttractorModes.AttractFriction:
							ParticleEvents.AddEveryTimeEvent(UpdateParticleAttractParticleToAttractor, 400, 1);
							ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingFriction, 410, 1);
						break;

						case EAttractorModes.RepelFriction:
							ParticleEvents.AddEveryTimeEvent(UpdateParticleRepelParticleFromAttractor, 400, 1);
							ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingFriction, 410, 1);
						break;
					}
				}
				// Else we are in Sprite Force mode
				else
				{
					// Get a handle to the first Active Particle in the List
					LinkedListNode<SpriteParticle> cNode = ActiveParticles.First;

					switch (meAttractorMode)
					{
						default:
						case EAttractorModes.None:
							// Apply Gravity
							while (cNode != null)
							{
								cNode.Value.ExternalForce = Vector3.Zero;
								cNode.Value.Friction = 0;
								cNode.Value.Acceleration = Vector3.Zero;
								cNode = cNode.Next;
							}
						break;

						case EAttractorModes.Attract:
							ParticleEvents.AddEveryTimeEvent(UpdateParticleAttractParticleToAttractor, 400, 1);

							// Apply Gravity
							while (cNode != null)
							{
								cNode.Value.ExternalForce = new Vector3(0, mfGravity, 0);
								cNode.Value.Friction = 0;
								cNode = cNode.Next;
							}
						break;

						case EAttractorModes.Repel:
							ParticleEvents.AddEveryTimeEvent(UpdateParticleRepelParticleFromAttractor, 400, 1);

							// Apply Gravity
							while (cNode != null)
							{
								cNode.Value.ExternalForce = new Vector3(0, mfGravity, 0);
								cNode.Value.Friction = 0;
								cNode = cNode.Next;
							}
						break;

						case EAttractorModes.AttractFriction:
							ParticleEvents.AddEveryTimeEvent(UpdateParticleAttractParticleToAttractor, 400, 1);

							// Apply Friction
							while (cNode != null)
							{
								cNode.Value.Friction = mfGravity;
								cNode.Value.ExternalForce = Vector3.Zero;
								cNode = cNode.Next;
							}
						break;

						case EAttractorModes.RepelFriction:
							ParticleEvents.AddEveryTimeEvent(UpdateParticleRepelParticleFromAttractor, 400, 1);

							// Apply Friction
							while (cNode != null)
							{
								cNode.Value.Friction = mfGravity;
								cNode.Value.ExternalForce = Vector3.Zero;
								cNode = cNode.Next;
							}
						break;
					}
				}
			}
		}

		public void ToggleAttractorMode()
		{
			meAttractorMode++;

			if (meAttractorMode > EAttractorModes.LastInList)
			{
				meAttractorMode = 0;
			}

			AttractorMode = meAttractorMode;
		}

		public float AttractorStrength
		{
			get { return mfAttractorStrength; }
			set { mfAttractorStrength = value; }
		}

		public void ToggleAttractorStrength()
		{
			if (AttractorStrength < 2.0f)
			{
				AttractorStrength += 0.2f;
			}
			else
			{
				AttractorStrength++;
				if (AttractorStrength > 10)
				{
					AttractorStrength = 1;
				}
			}
		}
	}
}