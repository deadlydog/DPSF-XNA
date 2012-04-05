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
	/// Create a new type of Particle for this Particle System.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	class BoxParticle : DefaultTexturedQuadParticle
	{
		// We need another variable to hold the Particle's untransformed Position (it's Emitter Independent Position)
		public Vector3 sEmitterIndependentPosition;
		public Quaternion cEmitterIndependentOrientation;

		public override void Reset()
		{
			base.Reset();
			sEmitterIndependentPosition = Vector3.Zero;
			cEmitterIndependentOrientation = Quaternion.Identity;
		}

		public override void CopyFrom(DPSFParticle ParticleToCopy)
		{
			// Cast the Particle to the type it really is
			BoxParticle cParticleToCopy = (BoxParticle)ParticleToCopy;

			base.CopyFrom(cParticleToCopy);
			sEmitterIndependentPosition = cParticleToCopy.sEmitterIndependentPosition;
			cEmitterIndependentOrientation = cParticleToCopy.cEmitterIndependentOrientation;
		}
	}

	/// <summary>
	/// Create a new Particle System class that inherits from a
	/// Default DPSF Particle System
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	class BoxParticleSystem : DPSFDefaultTexturedQuadParticleSystem<BoxParticle, DefaultTexturedQuadParticleVertex>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public BoxParticleSystem(Game cGame) : base(cGame) { }

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
			// Show the texture on both the front and back of the quads, since the box is partially transparent
			RenderProperties.RasterizerState.CullMode = CullMode.None;
		}

		//===========================================================
		// Initialization Functions
		//===========================================================
		public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
		{
			InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, 
												UpdateVertexProperties, "Textures/WhiteSquare1");
			Name = "Box";
			LoadPartiallyTranparentBox();
		}

		public void LoadPartiallyTranparentBox()
		{
			ParticleInitializationFunction = InitializeParticlePartiallyTransparentBox;

			ParticleEvents.RemoveAllEvents();
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAccordingToEmitter);
			MaxNumberOfParticlesAllowed = 6;
			Emitter.ParticlesPerSecond = 1000;
			Emitter.PositionData.Position = new Vector3(0, 51, 0);

			RemoveAllParticles();
		}

		public void InitializeParticlePartiallyTransparentBox(BoxParticle cParticle)
		{
			cParticle.Lifetime = 0;
			cParticle.Width = cParticle.Height = 100;

			float fHalfSize = cParticle.Width / 2.0f;

			switch (NumberOfActiveParticles)
			{
				default:
				case 0:
					// Front side
					cParticle.sEmitterIndependentPosition = new Vector3(0, 0, -fHalfSize);
					cParticle.Normal = new Vector3(0, 0, -1);
					cParticle.Up = new Vector3(0, 1, 0);
					cParticle.cEmitterIndependentOrientation = cParticle.Orientation;
					cParticle.Color = Color.Red;
				break;

				case 1:
					// Back side
					cParticle.sEmitterIndependentPosition = new Vector3(0, 0, fHalfSize);
					cParticle.Normal = new Vector3(0, 0, 1);
					cParticle.Up = new Vector3(0, 1, 0);
					cParticle.cEmitterIndependentOrientation = cParticle.Orientation;
					cParticle.Color = Color.Green;
				break;

				case 2:
					// Left side
					cParticle.sEmitterIndependentPosition = new Vector3(-fHalfSize, 0, 0);
					cParticle.Normal = new Vector3(-1, 0, 0);
					cParticle.Up = new Vector3(0, 1, 0);
					cParticle.cEmitterIndependentOrientation = cParticle.Orientation;
					cParticle.Color = Color.Blue;
				break;

				case 3:
					// Right side
					cParticle.sEmitterIndependentPosition = new Vector3(fHalfSize, 0, 0);
					cParticle.Normal = new Vector3(1, 0, 0);
					cParticle.Up = new Vector3(0, 1, 0);
					cParticle.cEmitterIndependentOrientation = cParticle.Orientation;
					cParticle.Color = Color.Yellow;
				break;

				case 4:
					// Top side
					cParticle.sEmitterIndependentPosition = new Vector3(0, fHalfSize, 0);
					cParticle.Normal = new Vector3(0, 1, 0);
					cParticle.Up = new Vector3(0, 0, 1);
					cParticle.cEmitterIndependentOrientation = cParticle.Orientation;
					cParticle.Color = Color.Chocolate;
				break;

				case 5:
					// Bottom side
					cParticle.sEmitterIndependentPosition = new Vector3(0, -fHalfSize, 0);
					cParticle.Normal = new Vector3(0, -1, 0);
					cParticle.Up = new Vector3(0, 0, 1);
					cParticle.cEmitterIndependentOrientation = cParticle.Orientation;
					cParticle.Color = Color.White;
				break;
			}

			// Set the particle to be partially transparent
			cParticle.Color.A = 128;

			// Turn off Depth (z-buffer) sorting so that we see all sides of the box, since it's partially transparent
			RenderProperties.DepthStencilState = DPSFHelper.CloneDepthStencilState(DepthStencilState.Default);
			RenderProperties.DepthStencilState.DepthBufferWriteEnable = false;
		}

		public void LoadOpaqueBoxBars()
		{
			ParticleInitializationFunction = InitializeParticleOpaqueBoxBars;

			ParticleEvents.RemoveAllEvents();
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAccordingToEmitter);

			MaxNumberOfParticlesAllowed = 40;
			Emitter.ParticlesPerSecond = 1000;
			Emitter.PositionData.Position = new Vector3(0, 51, 0);

			RemoveAllParticles();
		}

		public void InitializeParticleOpaqueBoxBars(BoxParticle cParticle)
		{
			int iCubeSize = 100;
			int iHalfCubeSize = iCubeSize / 2;

			cParticle.Lifetime = 0;
			cParticle.Width = 10;
			cParticle.Height = iCubeSize;

			int iNumberOfBarsPerSide = 4;
			int iSide = NumberOfActiveParticles / iNumberOfBarsPerSide;
			int iBar = NumberOfActiveParticles % iNumberOfBarsPerSide;

			// Assume total cube width should be the same as the height
			float fSpacing = (cParticle.Height / iNumberOfBarsPerSide) / 2;
			float fBarPosition = (-(cParticle.Height / 2.0f) + fSpacing) + (iBar * 2 * fSpacing);

			switch (iSide)
			{
				default:
				case 0:
					// Front side
					cParticle.sEmitterIndependentPosition = new Vector3(fBarPosition, 0, -iHalfCubeSize);
					cParticle.Normal = new Vector3(0, 0, -1);
					cParticle.Up = new Vector3(0, 1, 0);
					cParticle.cEmitterIndependentOrientation = cParticle.Orientation;
					cParticle.Color = Color.Red;
				break;

				case 1:
					// Back side
					cParticle.sEmitterIndependentPosition = new Vector3(fBarPosition, 0, iHalfCubeSize);
					cParticle.Normal = new Vector3(0, 0, 1);
					cParticle.Up = new Vector3(0, 1, 0);
					cParticle.cEmitterIndependentOrientation = cParticle.Orientation;
					cParticle.Color = Color.Green;
				break;

				case 2:
					// Left side
					cParticle.sEmitterIndependentPosition = new Vector3(-iHalfCubeSize, 0, fBarPosition);
					cParticle.Normal = new Vector3(-1, 0, 0);
					cParticle.Up = new Vector3(0, 1, 0);
					cParticle.cEmitterIndependentOrientation = cParticle.Orientation;
					cParticle.Color = Color.Blue;
				break;

				case 3:
					// Right side
					cParticle.sEmitterIndependentPosition = new Vector3(iHalfCubeSize, 0, fBarPosition);
					cParticle.Normal = new Vector3(1, 0, 0);
					cParticle.Up = new Vector3(0, 1, 0);
					cParticle.cEmitterIndependentOrientation = cParticle.Orientation;
					cParticle.Color = Color.Yellow;
				break;

				case 4:
					// Top side
					cParticle.sEmitterIndependentPosition = new Vector3(fBarPosition, iHalfCubeSize, 0);
					cParticle.Normal = new Vector3(0, 1, 0);
					cParticle.Up = new Vector3(0, 0, 1);
					cParticle.cEmitterIndependentOrientation = cParticle.Orientation;
					cParticle.Color = Color.Chocolate;
				break;

				case 5:
					// Bottom side
					cParticle.sEmitterIndependentPosition = new Vector3(fBarPosition, -iHalfCubeSize, 0);
					cParticle.Normal = new Vector3(0, -1, 0);
					cParticle.Up = new Vector3(0, 0, 1);
					cParticle.cEmitterIndependentOrientation = cParticle.Orientation;
					cParticle.Color = Color.White;
				break;

				case 6:
					// Top side (other direction)
					cParticle.sEmitterIndependentPosition = new Vector3(0, iHalfCubeSize + 1, fBarPosition);
					cParticle.Normal = new Vector3(0, 1, 0);
					cParticle.Up = new Vector3(1, 0, 0);
					cParticle.cEmitterIndependentOrientation = cParticle.Orientation;
					cParticle.Color = Color.Chocolate;
				break;

				case 7:
					// Bottom side (other direction)
					cParticle.sEmitterIndependentPosition = new Vector3(0, -iHalfCubeSize + 1, fBarPosition);
					cParticle.Normal = new Vector3(0, -1, 0);
					cParticle.Up = new Vector3(1, 0, 0);
					cParticle.cEmitterIndependentOrientation = cParticle.Orientation;
					cParticle.Color = Color.White;
				break;
			}

			// Particles will now be fully opaque
			// Turn on Depth (z-buffer) sorting, since the particles are opaque and we won't see anything behind them.
			RenderProperties.DepthStencilState = DPSFHelper.CloneDepthStencilState(DepthStencilState.Default);
			RenderProperties.DepthStencilState.DepthBufferWriteEnable = true;
		}

		//===========================================================
		// Particle Update Functions
		//===========================================================
		protected void UpdateParticlePositionAccordingToEmitter(BoxParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Rotate Particle Position around the Emitter
			cParticle.Position = Vector3.Transform(cParticle.sEmitterIndependentPosition, Emitter.OrientationData.Orientation);
			cParticle.Position += Emitter.PositionData.Position;

			// Rotate Particle Orientation around the Emitter
			cParticle.Orientation = Emitter.OrientationData.Orientation * cParticle.cEmitterIndependentOrientation;
		}

		protected void ChangeColorRandomlyAndResetElapedTime(BoxParticle cParticle, float fElapsedTimeInSeconds)
		{
			byte alpha = cParticle.Color.A;
			cParticle.Color = DPSFHelper.RandomColor();
			cParticle.Color.A = alpha;	// Restore the particles previous alpha value
			cParticle.ElapsedTime = 0;
		}

		protected void ResetElapsedTime(BoxParticle cParticle, float fElapsedTimeInSeconds)
		{
			cParticle.ElapsedTime = 0;
		}

		//===========================================================
		// Particle System Update Functions
		//===========================================================
		
		//===========================================================
		// Other Particle System Functions
		//===========================================================
		public void ToggleColorChanges()
		{
			if (ParticleEvents.RemoveTimedEvents(ChangeColorRandomlyAndResetElapedTime) == 0)
			{
				// Reset all of the Particles Elapsed Times
				ParticleEvents.AddOneTimeEvent(ResetElapsedTime);

				// Add the Event to change the Color
				ParticleEvents.AddTimedEvent(1.0f, ChangeColorRandomlyAndResetElapedTime);
			}
		}
	}
}