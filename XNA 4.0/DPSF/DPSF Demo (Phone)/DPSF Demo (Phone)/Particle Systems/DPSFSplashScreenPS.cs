//===================================================================
// This file contains the Particle System used to generate the effect
// for the DPSF Splash Screen.
//
// How To Use:
//  - Call the AutoInitialize() function to load the particle system.
//  - Clear the screen using the color of the BackgroundColor property.
//  - Draw the particle system. You do not need to specify the World, 
//  View, and Projection matrices as they are specified automatically
//  when the particle system is loaded.
//  - Check the SplashScreenIsDonePlaying property to determine when
//  the Splash Screen in finished and you can move to the next screen
//  to continue loading your game.
//
// NOTE: The particle system looks for the DPSFLogo.png image file in
//  a folder called "Textures" in the Content directory.  If you do 
//  not have the image stored there, you will need to change the
//  msTextureAssetName variable to use the correct path.
//===================================================================

#region Using Statements
using System;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF_Demo.ParticleSystems
{
	// Create a new type of Particle for this Particle System
	class DPSFSplashScreenParticle : DefaultAnimatedTexturedQuadParticle
	{
		// We need another variable to hold the Particle's untransformed Position (it's Emitter Independent Position)
		public Vector3 ImagePosition;
		public Quaternion ImageOrientation;
		public Vector3 Rotation;
		public bool IsGoingToImagePosition;
		public bool IsImagePositionReached;
		public bool IsImageOrientationReached;
		public Quaternion OrientationBeforeAutomaticMovement;

		public DPSFSplashScreenParticle()
		{
			Reset();
		}

		public override void Reset()
		{
			base.Reset();
			ImagePosition = Vector3.Zero;
			ImageOrientation = Quaternion.Identity;
			Rotation = Vector3.Zero;
			IsGoingToImagePosition = IsImagePositionReached = IsImageOrientationReached = false;
			OrientationBeforeAutomaticMovement = Quaternion.Identity;
		}

		public override void CopyFrom(DPSFParticle ParticleToCopy)
		{
			// Cast the Particle to the type it really is
			DPSFSplashScreenParticle cParticleToCopy = (DPSFSplashScreenParticle)ParticleToCopy;

			base.CopyFrom(cParticleToCopy);
			ImagePosition = cParticleToCopy.ImagePosition;
			ImageOrientation = cParticleToCopy.ImageOrientation;
			Rotation = cParticleToCopy.Rotation;
			IsGoingToImagePosition = cParticleToCopy.IsGoingToImagePosition;
			IsImagePositionReached = cParticleToCopy.IsImagePositionReached;
			IsImageOrientationReached = cParticleToCopy.IsImageOrientationReached;
			OrientationBeforeAutomaticMovement = cParticleToCopy.OrientationBeforeAutomaticMovement;
		}
	}

	/// <summary>
	/// Create a new Particle System class that inherits from a
	/// Default DPSF Particle System
	/// </summary>
	class DPSFSplashScreenParticleSystem : DPSFDefaultAnimatedTexturedQuadParticleSystem<DPSFSplashScreenParticle, DefaultTexturedQuadParticleVertex>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public DPSFSplashScreenParticleSystem(Game cGame) : base(cGame) { }

		//===========================================================
		// Structures and Variables
		//===========================================================

		// The location of the DPSFLogo.png image file (change this to point to where you are storing the DPSF Logo)
		private const string _textureAssetName = "Textures/DPSFLogo";

		// Constant Effect Settings
		private int _maxNumberOfRows = 32;		// Don't use more than 1024 (i.e. 32*32) particles to ensure smooth animation on Xbox 360.
		private int _maxNumberOfColumns = 32;
		private const int _widthOfCompositeImage = 256;
		private const int _heightOfCompositeImage = 128;
		private const float _totalTimeInSecondsToDisplaySplashScreen = 5.0f;

		// Effect Setting Variables
		private float _timeInSecondsBeforeMovingToImagePosition = 1.25f;
		private float _timeInSecondsToReachImagePosition = 1.0f;
		private float _timeInSecondsToReachImageOrientation = 1.5f;

		// Class variables
		private int _maxNumberOfParticlesRequired = 0;
		private DateTime _splashScreenStartTime;

		// Have the particle die half a second before the splash screen ends so we see it fade out nicely
		private const float _maxParticleLifetime = _totalTimeInSecondsToDisplaySplashScreen - 0.5f;

		// Camera Settings to use while displaying the Splash Screen
		private Matrix _viewMatrix = Matrix.CreateLookAt(new Vector3(0, 50, 300), new Vector3(0, 50, 0), Vector3.Up);
		private Matrix _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1.33333f, 1, 10000);
		private Color _backgroundColor = Color.Black;

		//===========================================================
		// Overridden Particle System Functions
		//===========================================================
		protected override void InitializeRenderProperties()
		{
			base.InitializeRenderProperties();
			RenderProperties.RasterizerState.CullMode = CullMode.None;			// Using 3D quads, so make sure we can see the texture on the front and back.
			RenderProperties.DepthStencilState.DepthBufferWriteEnable = true;	// Enable the depth buffer so particles are sorted properly from front to back.
		}

		//===========================================================
		// Initialization Functions
		//===========================================================
		public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
		{
			// If we are using the Reach profile (instead of HiDef) use fewer particles so it's less expensive and animates smoothly.
			if (cGraphicsDevice.GraphicsProfile == GraphicsProfile.Reach)
			{
				_maxNumberOfRows = 24;
				_maxNumberOfColumns = 24;
			}
			_maxNumberOfParticlesRequired = _maxNumberOfRows * _maxNumberOfColumns;

			// Initialize the particle system with the texture to use
			InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, _maxNumberOfParticlesRequired, 
					_maxNumberOfParticlesRequired, UpdateVertexProperties, _textureAssetName);

			// Load one of the DPSF Splash Screens
			LoadDPSFSplashScreen();
		}

		public void LoadDPSFSplashScreen()
		{
			// Do the setup required by all Splash Screens
			DoCommonSplashScreenInitialization();

			// Randomly choose which Splash Screen to display
			int rand = RandomNumber.Next(2);
			switch (rand)
			{
				case 0: LoadVortexSplashScreen(); break;
				case 1: LoadFallingBlocksSplashScreen(); break;
			}
		}

		#region Common Splash Screen Initialization
		private void DoCommonSplashScreenInitialization()
		{
			// Setup the Camera and specify the default number of Particles to Emit Per Second
			SetWorldViewProjectionMatrices(Matrix.Identity, _viewMatrix, _projectionMatrix);
			Emitter.ParticlesPerSecond = 10000;

			// Make sure this is a fresh setup
			RemoveAllParticles();
			ParticleEvents.RemoveAllEvents();
			ParticleSystemEvents.RemoveAllEvents();

			// Add the event that triggers when the Splash Screen is complete
			ParticleSystemEvents.AddTimedEvent(_totalTimeInSecondsToDisplaySplashScreen, MarkSplashScreenAsDonePlaying);
		}

		private void DoCommonParticleInitialization(DPSFSplashScreenParticle particle)
		{
			// Fill in this Particles information about where it should be to form the composite image
			SetParticlePositionWidthHeightAndTextureCoordinatesToFormCompositeImage(particle);

			// If the Splash Screen just started, record what time it started at
			if (this.NumberOfActiveParticles == 0)
			{
				// Record what time the splash screen started
				_splashScreenStartTime = DateTime.Now;
			}
			// If this is the last particle to create, turn the emitter off so the animation doesn't restart when the particles die
			if (this.NumberOfActiveParticles == (_maxNumberOfParticlesRequired - 1))
			{
				this.Emitter.Enabled = false;
			}
		}

		/// <summary>
		/// This function sets the Particle Properties so that when all Particles are viewed together, 
		/// they form the complete composite image of the texture.
		/// </summary>
		/// <param name="particle"></param>
		private void SetParticlePositionWidthHeightAndTextureCoordinatesToFormCompositeImage(DPSFSplashScreenParticle particle)
		{
			// Calculate how big the Particles should be to achieve the desired size
			int iRequiredParticleWidth = _widthOfCompositeImage / _maxNumberOfColumns;
			int iRequiredParticleHeight = _heightOfCompositeImage / _maxNumberOfRows;

			// Calculate how big one Row and Column from the texture should be
			int iTextureRowSize = Texture.Height / _maxNumberOfRows;
			int iTextureColumnSize = Texture.Width / _maxNumberOfColumns;

			// Calculate which Row and Column this Particle should be at
			int iRow = NumberOfActiveParticles / _maxNumberOfColumns;
			int iColumn = NumberOfActiveParticles % _maxNumberOfColumns;

			// Calculate this Particle's Position to create the full Image
			int iY = (_maxNumberOfRows * iRequiredParticleHeight) - ((iRow * iRequiredParticleHeight) + (iRequiredParticleHeight / 2));
			int iX = (iColumn * iRequiredParticleWidth) + (iRequiredParticleWidth / 2);
			iX -= (_maxNumberOfColumns * iRequiredParticleWidth) / 2;    // Center the image

			// Calculate this Particle's Texture Coordinates to use
			float fTextureTop = (iRow * iTextureRowSize) / (float)Texture.Height;
			float fTextureLeft = (iColumn * iTextureColumnSize) / (float)Texture.Width;
			float fTextureBottom = ((iRow * iTextureRowSize) + iTextureRowSize) / (float)Texture.Height;
			float fTextureRight = ((iColumn * iTextureColumnSize) + iTextureColumnSize) / (float)Texture.Width;

			// Set the Particle's Properties to Form the complete Image
			particle.Width = iRequiredParticleWidth;
			particle.Height = iRequiredParticleHeight;
			particle.ImagePosition = new Vector3(iX, iY, 0);
			particle.ImageOrientation = Orientation3D.GetQuaternionWithOrientation(Vector3.Forward, Vector3.Up);
			particle.NormalizedTextureCoordinateLeftTop = new Vector2(fTextureLeft, fTextureTop);
			particle.NormalizedTextureCoordinateRightBottom = new Vector2(fTextureRight, fTextureBottom);
		}
		#endregion

		#region Vortex Splash Screen
		public void LoadVortexSplashScreen()
		{
			DoCommonSplashScreenInitialization();

			ParticleInitializationFunction = InitializeParticleVortexScreen;

			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndQuickFadeOut);
			ParticleEvents.AddEveryTimeEvent(RotateAroundOrigin, 100);
			ParticleEvents.AddEveryTimeEvent(MoveToCompositeImagePosition, 200);
		}

		public void InitializeParticleVortexScreen(DPSFSplashScreenParticle particle)
		{
			DoCommonParticleInitialization(particle);

			// Have the particle die before the splash screen ends so we see it fade out nicely
			particle.Lifetime = _maxParticleLifetime;

			// Set particle's velocity to straight up or down (the RotateAroundOrigin Particle Update Function will rotate it around the Y-axis)
			particle.Velocity = new Vector3(0, RandomNumber.Next(1, 100), 0);
			particle.Rotation = new Vector3(0, MathHelper.TwoPi, 0);
			particle.RotationalVelocity = new Vector3(RandomNumber.Between(0, MathHelper.TwoPi), RandomNumber.Between(0, MathHelper.TwoPi), RandomNumber.Between(0, MathHelper.TwoPi));

			// Have some particles start at the bottom of the screen, and others start at the top
			if (NumberOfActiveParticles % 2 == 0)
			{
				particle.Position = new Vector3(RandomNumber.Next(-100, 100), 0, RandomNumber.Next(-100, 100));
			}
			else
			{
				particle.Position = new Vector3(RandomNumber.Next(-100, 100), 100, RandomNumber.Next(-100, 100));
				particle.Velocity *= -1;
			}
		}
		#endregion

		#region Falling Blocks Splash Screen
		public void LoadFallingBlocksSplashScreen()
		{
			DoCommonSplashScreenInitialization();

			ParticleInitializationFunction = InitializeParticleFallingBlocksScreen;

			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndQuickFadeOut);
			ParticleEvents.AddEveryTimeEvent(MoveToCompositeImagePosition, 200);

			ParticleSystemEvents.AddTimedEvent(_totalTimeInSecondsToDisplaySplashScreen, MarkSplashScreenAsDonePlaying);

			this.Emitter.ParticlesPerSecond = 700;
			_timeInSecondsToReachImagePosition = 0.8f;
		}

		public void InitializeParticleFallingBlocksScreen(DPSFSplashScreenParticle particle)
		{
			DoCommonParticleInitialization(particle);

			// Have the particle die before the splash screen ends so we see it fade out nicely
			TimeSpan elapsedTime = DateTime.Now - _splashScreenStartTime;
			particle.Lifetime = _maxParticleLifetime - (float)elapsedTime.TotalSeconds;

			// Set particle's velocity to straight up or down (the RotateAroundOrigin Particle Update Function will rotate it around the Y-axis)
			particle.Velocity = new Vector3(0, -200, 0);

			particle.Position = particle.ImagePosition;
			particle.Position.Y = 200;

			// If this is the last particle to create, turn the emitter off so the animation doesn't restart when the particles die
			if (this.NumberOfActiveParticles == (_maxNumberOfParticlesRequired - 1))
			{
				this.Emitter.Enabled = false;
			}
		}
		#endregion

		//===========================================================
		// Particle Update Functions
		//===========================================================

		/// <summary>
		/// Rotates the particle around the world coordinates origin.
		/// </summary>
		/// <param name="particle">The c particle.</param>
		/// <param name="elapsedTimeInSeconds">The f elapsed time in seconds.</param>
		public void RotateAroundOrigin(DPSFSplashScreenParticle particle, float elapsedTimeInSeconds)
		{
			// Calculate the Rotation Matrix to Rotate the Particle by
			Vector3 sAmountToRotate = particle.Rotation * elapsedTimeInSeconds;
			Matrix sRotation = Matrix.CreateFromYawPitchRoll(sAmountToRotate.Y, sAmountToRotate.X, sAmountToRotate.Z);
			
			// Rotate the Particle around the origin
			particle.Position = PivotPoint3D.RotatePosition(sRotation, new Vector3(0, particle.Position.Y, 0), particle.Position);
		}

		/// <summary>
		/// Lerps the particle to its composite Image position and orientation.
		/// </summary>
		/// <param name="particle">The c particle.</param>
		/// <param name="elapsedTimeInSeconds">The f elapsed time in seconds.</param>
		public void MoveToCompositeImagePosition(DPSFSplashScreenParticle particle, float elapsedTimeInSeconds)
		{
			// If it is not time for the Particle to go to its final destination yet, or it has already reached it, then just exit.
			if (particle.ElapsedTime < _timeInSecondsBeforeMovingToImagePosition ||
				(particle.IsImagePositionReached && particle.IsImageOrientationReached))
			{
				// Exit without doing anything
				return;
			}

			// If the Particle isn't going to its Final Position yet, but it should be
			if (!particle.IsGoingToImagePosition)
			{
				// Make sure the Particle doesn't move on its own anymore (this function now controls it)
				particle.Acceleration = Vector3.Zero;
				particle.RotationalVelocity = Vector3.Zero;
				particle.RotationalAcceleration = Vector3.Zero;
				particle.Rotation = Vector3.Zero;

				// Make the Particle move towards its final destination
				particle.Velocity = (particle.ImagePosition - particle.Position) / _timeInSecondsToReachImagePosition;

				Quaternion cRotationRequired = Orientation3D.GetRotationTo(Orientation3D.GetNormalDirection(particle.Orientation), Orientation3D.GetNormalDirection(particle.ImageOrientation));
				cRotationRequired *= Orientation3D.GetRotationTo(Orientation3D.GetUpDirection(particle.Orientation), Orientation3D.GetUpDirection(particle.ImageOrientation));
				particle.OrientationBeforeAutomaticMovement = cRotationRequired;

				particle.IsGoingToImagePosition = true;
			}

			// If the Particle hasn't made it to its Image Position yet
			if (!particle.IsImagePositionReached)
			{
				// Calculate the Vector the particle must travel in to reach the Image Position
				Vector3 sVectorToImagePosition = particle.ImagePosition - particle.Position;

				// If the Particle is not travelling the same direction as it originally was to get to the
				// Image position, then it just past the Image position, so move it to the Image position.
				if (!DPSFHelper.VectorsAreTheSamePolarity(sVectorToImagePosition, particle.Velocity))
				{
					particle.Velocity = Vector3.Zero;
					particle.Position = particle.ImagePosition;
					particle.IsImagePositionReached = true;
				}
			}

			// If the Particle hasn't made it to its Image Orientation yet, then Lerp the Particle to its Image Orientation
			if (!particle.IsImageOrientationReached)
			{
				float fLerpAmount = (particle.ElapsedTime - _timeInSecondsBeforeMovingToImagePosition) / _timeInSecondsToReachImageOrientation;
				if (fLerpAmount > 1.0f)
				{
					particle.Orientation = particle.ImageOrientation;
					particle.IsImageOrientationReached = true;
				}
				else
				{
					particle.Orientation = Quaternion.Slerp(particle.OrientationBeforeAutomaticMovement, particle.ImageOrientation, fLerpAmount);
				}
			}
		}

		//===========================================================
		// Particle System Update Functions
		//===========================================================
		public void MarkSplashScreenAsDonePlaying(float elapsedTimeInSeconds)
		{
			IsSplashScreenComplete = true;
		}

		//===========================================================
		// Other Particle System Functions
		//===========================================================

		/// <summary>
		/// Get / Set if the Splash Screen is done playing
		/// </summary>
		public bool IsSplashScreenComplete
		{
			get { return _isSplashScreenComplete; }
			set 
			{ 
				_isSplashScreenComplete = value;

				// Let any listeners know that the Splash Screen is done playing
				if (_isSplashScreenComplete)
					SplashScreenComplete(this, EventArgs.Empty);
			}
		}
		private bool _isSplashScreenComplete = false;

		/// <summary>
		/// Occurs when the Splash Screen finishes playing
		/// </summary>
		public event EventHandler SplashScreenComplete = delegate(object sender, EventArgs e) { };

		/// <summary>
		/// Get the Background Color that should be used for the Splash Screen
		/// </summary>
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
		}
	}
}