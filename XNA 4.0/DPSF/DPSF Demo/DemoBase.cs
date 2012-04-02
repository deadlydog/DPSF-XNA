#region File Description
//===================================================================
// GameMain.cs
//
// This is the application file used to test the Dynamic Particle System Framework
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using DPSF_Demo.Input;
using DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo;
using DPSF_Demo.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DPSF;
using DPSF.ParticleSystems;
#endregion

namespace DPSF_Demo
{
	/// <summary>
	/// Application class showing how to use the Dynamic Particle System Framework
	/// </summary>
	public class DemoBase : Microsoft.Xna.Framework.Game
	{
		#region Fields

		//===========================================================
		// Global Constants - User Settings
		//===========================================================
		
		// If this is set to true, a MessageBox will display any unhandled exceptions that occur. This
		// is useful when not debugging (i.e. when running directly from the executable) as it will still 
		// tell you what type of exception occurred and what line of code produced it.
		// Leave this set to false while debugging to have Visual Studio automatically take you to the 
		// line that threw the exception.
		public const bool RELEASE_MODE = false;

		// To allow the game to run as fast as possible, set this to false
		const bool LIMIT_FPS = false;

		// How often the Particle Systems should be updated (zero = update as often as possible)
		const int PARTICLE_SYSTEM_UPDATES_PER_SECOND = 0;

		// The Width and Height of the application's window (default is 800x600)
		int WINDOW_WIDTH = 800;
		int WINDOW_HEIGHT = 600;

		// The background color to use
	    private static readonly Color BACKGROUND_COLOR = Color.Black;

	    // Specify the text Colors to use
	    public static readonly Color PROPERTY_TEXT_COlOR = Color.WhiteSmoke;
        public static readonly Color VALUE_TEXT_COLOR = Color.Yellow;
        public static readonly Color CONTROL_TEXT_COLOR = Color.PowderBlue;

	    // Static Particle Settings
		float mfStaticParticleTimeStep = 1.0f / 30.0f;	// The Time Step between the drawing of each frame of the Static Particles (1 / # of fps, example, 1 / 30 = 30fps).
		float mfStaticParticleTotalTime = 3.0f;			// The number of seconds that the Static Particle should be drawn over.

		// "Draw To Files" Settings
		float mfDrawPSToFilesTimeStep = 1.0f / 10.0f;
		float mfDrawPSToFilesTotalTime = 2.0f;
		int miDrawPSToFilesImageWidth = 200;
		int miDrawPSToFilesImageHeight = 150;
		string msDrawPSToFilesDirectoryName = "AnimationFrames";
		bool mbCreateAnimatedGIF = true;
		bool mbCreateTileSetImage = true;

		string msSerializedPSFileName = "SerializedParticleSystem.dat"; // The name of the file to serialize the particle system to

		//===========================================================
		// Class Structures and Variables
		//===========================================================

		// Class to hold information about the Position, Size, and Visibility of an Object
		class SimpleObject
		{
			public Vector3 sPosition = Vector3.Zero;
			public Vector3 sVelocity = Vector3.Zero;
			public float fSize = 20.0f;
			public bool bVisible = false;
			public TimeSpan cTimeAliveInSeconds = new TimeSpan();

			public void Update(float fElapsedTimeInSeconds)
			{
				sPosition += sVelocity * fElapsedTimeInSeconds;
				cTimeAliveInSeconds += TimeSpan.FromSeconds(fElapsedTimeInSeconds);
			}
		}

		// Enumeration of all the Particle System Effects
		enum PSEffects
		{
			Random = 0,
			Fire,
			FireSprite,
			Smoke,
			Snow,
			SquarePattern,
			Fountain,
			Random2D,
			GasFall,
			Dot,
			Fireworks,
			Figure8,
			Star,
			Ball,
			RotatingQuad,
			Box,
			Image,
			AnimatedTexturedQuad,
			Sprite,
			AnimatedSprite,
			QuadSpray,
			Magnets,
			Sparkler,
			GridQuad,
			Sphere,
			MultipleParticleImages,
			MultipleParticleImagesSprite,
			ExplosionFireSmoke,
			ExplosionFlash,
			ExplosionFlyingSparks,
			ExplosionSmokeTrails,
			ExplosionRoundSparks,
			ExplosionDebris,
			ExplosionDebrisSprite,
			ExplosionShockwave,
			Explosion,
			Trail,
			SpriteParticleSystemTemplate,
			Sprite3DBillboardParticleSystemTemplate,
			QuadParticleSystemTemplate,
			TexturedQuadParticleSystemTemplate,
			DefaultSpriteParticleSystemTemplate,
			DefaultSprite3DBillboardParticleSystemTemplate,
			DefaultQuadParticleSystemTemplate,
			DefaultTexturedQuadParticleSystemTemplate,
			LastInList = 43     // This value should be the number of Effects in the enumeration minus one (since we start at index 0) (excluding the Splash Screen)
		}

		// List of all the textures
		enum Textures
		{
			AnimatedButterfly = 0,
			AnimatedExplosion,
			Arrow,
			Arrow2,
			Arrow3,
			Arrow4,
			Arrow5,
			Ball,
			Bubble,
			Cloud,
			CloudLight,
			CrossArrow,
			Dog,
			Donut,
			DPSFLogo,
			DPSFText,
			Fire,
			Flower,
			Flower2,
			Flower3,
			Flower4,
			Flower5,
			Gear,
			Gear2,
			Gear3,
			Gear4,
			Gear5,
			MoveArrow,
			Paper,
			Particle,
			RedCircle,
			Shape1,
			Shape2,
			Smoke,
			Spark,
			Splat,
			Star,
			Star2,
			Star3,
			Star4,
			Star5,
			Star6,
			Star7,
			Star8,
			Star9,
			StarFish,
			Sun,
			Sun2,
			Sun3,
			ThrowingStar,
			Wheel,
			WhiteSquare,
			WhiteSquare1,
			WhiteSquare2,
			WhiteSquare10,
			WordBubble,
			X,
			LastInList = 56     // This value should be the same as the Texture with the largest value
		}

		// Initialize which Particle System to use
		PSEffects meCurrentPS = PSEffects.Random;

		// Initialize the Texture to use
		Textures meCurrentTexture = Textures.Bubble;

		GraphicsDeviceManager mcGraphics;       // Handle to the Graphics object

		SpriteBatch mcSpriteBatch;              // Batch used to draw Sprites
		SpriteFont mcFont;                      // Font used to draw text
		Model mcFloorModel;                     // Model of the Floor
		Model mcSphereModel;                    // Model of a sphere

		// Initialize the Sphere Object
		SimpleObject mcSphere = new SimpleObject();

		bool mbShowText = true;                 // Tells if Text should be shown or not
		bool mbShowCommonControls = false;      // Tells if the Common Controls should be shown or not
		bool mbShowPSControls = false;          // Tells if the Particle System specific Controls should be shown or not
		bool mbShowCameraControls = false;      // Tells if the Camera Controls should be shown or not
		bool mbShowPerformanceText = false;     // Tells if we should draw Performance info or not, such as how much memory is currently set to be collected by the Garbage Collector.

		bool DrawPerformanceText
		{
			get { return mbShowPerformanceText; }
			set
			{
				bool previousValue = mbShowPerformanceText;
				mbShowPerformanceText = value;

				// Only enable the Performance Profiling if we are going to be displaying it.

				// Set it on the Defaults so it is enabled/disabled whenever we initialize a new particle system.
				DPSFDefaultSettings.PerformanceProfilingIsEnabled = mbShowPerformanceText;

				// Set it on the Manager to enable/disable it for the particle system that is currently running.
				_particleSystemManager.SetPerformanceProfilingIsEnabledForAllParticleSystems(mbShowPerformanceText);

				// If this value was changed from off to on, hook up the event handler to calculate garbage collection
				if (mbShowPerformanceText && !previousValue)
				{
					FPS.FPSUpdated += new EventHandler<FPS.FPSEventArgs>(FPS_FPSUpdated);
				}
				// Else if this value was turned off, unhook the event handler
				else if (!mbShowPerformanceText)
				{
					FPS.FPSUpdated -= new EventHandler<FPS.FPSEventArgs>(FPS_FPSUpdated);
				}
			}
		}

		/// <summary>
		/// Handles the FPSUpdated event of the FPS control to calculate the average amount of garbage created each frame in the last second.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DPSF_Demo.FPS.FPSEventArgs"/> instance containing the event data.</param>
		void FPS_FPSUpdated(object sender, FPS.FPSEventArgs e)
		{
			// Get how much Garbage is waiting to be collected
			long currentGarbageAmount = GC.GetTotalMemory(false);

			// If the Garbage Collector did not run in the past second, calculate the average amount of garbage created per frame in the past second.
			if (currentGarbageAmount > _garbageAmountAtLastFPSUpdate)
			{
				float garbageCreatedInLastSecondInKB = (currentGarbageAmount - _garbageAmountAtLastFPSUpdate) / 1024f;
				_garbageAverageCreatedPerFrameInKB = garbageCreatedInLastSecondInKB / e.FPS;

				int updatesPerSecond = _currentDPSFDemoParticleSystemWrapper.UpdatesPerSecond;
				updatesPerSecond = updatesPerSecond > 0 ? updatesPerSecond : _updatesPerSecond;
				_garbageAverageCreatedPerUpdateInKB = garbageCreatedInLastSecondInKB / updatesPerSecond;
			}

			// Record the current amount of garbage to use to calculate the Garbage Created Per Second on the next update
			_garbageAmountAtLastFPSUpdate = currentGarbageAmount;

			// Reset how many updates have been done in the past second
			_updatesPerSecond = 0;
		}

		long _garbageAmountAtLastFPSUpdate = 0;         // The amount of garbage waiting to be collected by the Garbage Collector the last time the FPS were updated (one second interval).
		float _garbageCurrentAmountInKB = 0;            // The amount of garbage currently waiting to be collected by the Garbage Collector (in Kilobytes).
		float _garbageAverageCreatedPerFrameInKB = 0;   // How much garbage was created in the past second (in Kilobytes).
		float _garbageAverageCreatedPerUpdateInKB = 0;  // How much garbage was created during the last Update() (in Kilobytes).
		int _updatesPerSecond = 0;                      // The number of times the Updater() function was called in the past second.

		bool mbShowFloor = true;                // Tells if the Floor should be Shown or not.
		bool mbPaused = false;                  // Tells if the game should be Paused or not.
		bool mbClearScreenEveryFrame = true;    // Tells if the Screen should be Cleared every Frame or not.
		bool _clearScreenEveryFrameJustToggled = false;	// Tells if the ClearScreenEveryFrame variable was just toggled or not.
		RenderTarget2D _renderTarget = null;	// Used to draw to when we want draws to persist across multiple frames.

		// Draw Static Particle variables (2 of the variables are up in the User Settings)
		bool mbDrawStaticPS = false;            // Tells if Static Particles should be drawn or not.
		bool mbStaticParticlesDrawn = false;    // Tells if the Static Particles have already been drawn or not.
		

		// The World, View, and Projection matrices
		Matrix msWorldMatrix = Matrix.Identity;
		Matrix msViewMatrix = Matrix.Identity;
		Matrix msProjectionMatrix = Matrix.Identity;

		// Variables used to draw the lines indicating positive axis directions
		bool mbShowAxis = false;
		VertexPositionColor[] msaAxisDirectionVertices; // Vertices to draw lines on the floor indicating positive axis directions
		VertexDeclaration mcAxisVertexDeclaration;
		BasicEffect mcAxisEffect;

		// Initialize the Camera
		Camera msCamera = new Camera(true);

		// Declare the Particle System Manager to manage the Particle Systems
		ParticleSystemManager _particleSystemManager = new ParticleSystemManager();

		// Declare the Particle System Variables
		DPSFSplashScreenDPSFDemoParticleSystemWrapper _mcDPSFSplashScreenDPSFDemoParticleSystemWrapper = null;
		RandomDPSFDemoParticleSystemWrapper _mcRandomDPSFDemoParticleSystemWrapper = null;
		FireDPSFDemoParticleSystemWrapper _mcFireDPSFDemoParticleSystemWrapper = null;
		FireSpriteDPSFDemoParticleSystemWrapper _mcFireSpriteDPSFDemoParticleSystemWrapper = null;
		SmokeDPSFDemoParticleSystemWrapper _mcSmokeDPSFDemoParticleSystemWrapper = null;
		SnowDPSFDemoParticleSystemWrapper _mcSnowDPSFDemoParticleSystemWrapper = null;
		SquarePatternDPSFDemoParticleSystemWrapper _mcSquarePatternDPSFDemoParticleSystemWrapper = null;
		FountainDPSFDemoParticleSystemWrapper _mcFountainDPSFDemoParticleSystemWrapper = null;
		Random2DDPSFDemoParticleSystemWrapper _mcRandom2DdpsfDemoParticleSystemWrapper = null;
		GasFallDPSFDemoParticleSystemWrapper _mcGasFallDPSFDemoParticleSystemWrapper = null;
		DotDPSFDemoParticleSystemWrapper _mcDotDPSFDemoParticleSystemWrapper = null;
		FireworksDPSFDemoParticleSystemWrapper _mcFireworksDPSFDemoParticleSystemWrapper = null;
		Figure8DPSFDemoParticleSystemWrapper _mcFigure8DPSFDemoParticleSystemWrapper = null;
		StarDPSFDemoParticleSystemWrapper _mcStarDPSFDemoParticleSystemWrapper = null;
		BallDPSFDemoParticleSystemWrapper _mcBallDPSFDemoParticleSystemWrapper = null;
		RotatingQuadsDPSFDemoParticleSystemWrapper _mcRotatingQuadDPSFDemoParticleSystemWrapper = null;
		BoxDPSFDemoParticleSystemWrapper _mcBoxDPSFDemoParticleSystemWrapper = null;
		ImageDPSFDemoParticleSystemWrapper _mcImageDPSFDemoParticleSystemWrapper = null;
		AnimatedQuadDPSFDemoParticleSystemWrapper _mcAnimatedQuadDPSFDemoParticleSystemWrapper = null;
		SpriteDPSFDemoParticleSystemWrapper _mcSpriteDPSFDemoParticleSystemWrapper = null;
		AnimatedSpriteDPSFDemoParticleSystemWrapper _mcAnimatedSpriteDPSFDemoParticleSystemWrapper = null;
		QuadSprayDPSFDemoParticleSystemWrapper _mcQuadSprayDPSFDemoParticleSystemWrapper = null;
		MagnetsDPSFDemoParticleSystemWrapper _mcMagnetDPSFDemoParticleSystemWrapper = null;
		SparklerDPSFDemoParticleSystemWrapper _mcSparklerDPSFDemoParticleSystemWrapper = null;
		GridQuadDPSFDemoParticleSystemWrapper _mcGridQuadDPSFDemoParticleSystemWrapper = null;
		SphereDPSFDemoParticleSystemWrapper _mcSphereDPSFDemoParticleSystemWrapper = null;
		MultipleDPSFDemoParticleImagesDPSFDemoParticleSystemWrapper _mcMultipleDPSFDemoImagesDPSFDemoParticleSystemWrapper = null;
		MultipleDPSFDemoParticleImagesSpriteDPSFDemoParticleSystemWrapper _mcMultipleDPSFDemoImagesSpriteDPSFDemoParticleSystemWrapper = null;
		ExplosionFireSmokeDPSFDemoParticleSystemWrapper _mcExplosionFireSmokeDPSFDemoParticleSystemWrapper = null;
		ExplosionFlashDPSFDemoParticleSystemWrapper _mcExplosionFlashDPSFDemoParticleSystemWrapper = null;
		ExplosionFlyingSparksDPSFDemoParticleSystemWrapper _mcExplosionFlyingSparksDPSFDemoParticleSystemWrapper = null;
		ExplosionSmokeTrailsDPSFDemoParticleSystemWrapper _mcExplosionSmokeTrailsDPSFDemoParticleSystemWrapper = null;
		ExplosionRoundSparksDPSFDemoParticleSystemWrapper _mcExplosionRoundSparksDPSFDemoParticleSystemWrapper = null;
		ExplosionDebrisDPSFDemoParticleSystemWrapper _mcExplosionDebrisDPSFDemoParticleSystemWrapper = null;
		ExplosionDebrisSpriteDPSFDemoParticleSystemWrapper _mcExplosionDebrisSpriteDPSFDemoParticleSystemWrapper = null;
		ExplosionShockwaveDPSFDemoParticleSystemWrapper _mcExplosionShockwaveDPSFDemoParticleSystemWrapper = null;
		ExplosionDPSFDemoParticleSystemWrapper _mcExplosionDPSFDemoParticleSystemWrapper = null;
		TrailDPSFDemoParticleSystemWrapper _mcTrailDPSFDemoParticleSystemWrapper = null;
		SpriteDPSFDemoParticleSystemTemplateWrapper _mcSpriteDPSFDemoParticleSystemTemplateWrapper = null;
		Sprite3DBillboardDPSFDemoParticleSystemTemplateWrapper _mcSprite3DBillboardDPSFDemoParticleSystemTemplateWrapper = null;
		QuadDPSFDemoParticleSystemTemplateWrapper _mcQuadDPSFDemoParticleSystemTemplateWrapper = null;
		TexturedQuadDPSFDemoParticleSystemTemplateWrapper _mcTexturedQuadDPSFDemoParticleSystemTemplateWrapper = null;
		DefaultSpriteDPSFDemoParticleSystemTemplateWrapper _mcDefaultSpriteDPSFDemoParticleSystemTemplateWrapper = null;
		DefaultSprite3DBillboardDPSFDemoParticleSystemTemplateWrapper _mcDefaultSprite3DBillboardDPSFDemoParticleSystemTemplateWrapper = null;
		DefaultQuadDPSFDemoParticleSystemTemplateWrapper _mcDefaultQuadDPSFDemoParticleSystemTemplateWrapper = null;
		DefaultTexturedQuadDPSFDemoParticleSystemTemplateWrapper _mcDefaultTexturedQuadDPSFDemoParticleSystemTemplateWrapper = null;

		// Declare a Particle System pointer to point to the Current Particle System being used.
		IWrapDPSFDemoParticleSystems _currentDPSFDemoParticleSystemWrapper;

		#endregion

		#region Initialization

		/// <summary>
		/// Constructor
		/// </summary>
		public DemoBase()
		{
			mcGraphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			// If we should not Limit the FPS
			if (!LIMIT_FPS)
			{
				// Make the game run as fast as possible (i.e. don't limit the FPS)
				this.IsFixedTimeStep = false;
				mcGraphics.SynchronizeWithVerticalRetrace = false;
			}
			
			// Set the resolution
			mcGraphics.PreferredBackBufferWidth = WINDOW_WIDTH;
			mcGraphics.PreferredBackBufferHeight = WINDOW_HEIGHT;

			// Set the Title of the Window
			Window.Title = "Dynamic Particle System Framework Demo";

			// Do we want to show the mouse
			this.IsMouseVisible = false;
		}

		/// <summary>
		/// Load your graphics content
		/// </summary>
		protected override void LoadContent()
		{
			mcSpriteBatch = new SpriteBatch(GraphicsDevice);

			// Load fonts and models for test application
			mcFont = Content.Load<SpriteFont>("Fonts/font");
			mcFloorModel = Content.Load<Model>("grid");
			mcSphereModel = Content.Load<Model>("SphereHighPoly");

			// Setup our render target to draw to when we want draws to persist across multiple frames
			_renderTarget = new RenderTarget2D(mcGraphics.GraphicsDevice, mcGraphics.PreferredBackBufferWidth, mcGraphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

			// Specify vertices indicating positive axis directions
			int iLineLength = 50;
			msaAxisDirectionVertices = new VertexPositionColor[6];
			msaAxisDirectionVertices[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);
			msaAxisDirectionVertices[1] = new VertexPositionColor(new Vector3(iLineLength, 1, 0), Color.Red);
			msaAxisDirectionVertices[2] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Green);
			msaAxisDirectionVertices[3] = new VertexPositionColor(new Vector3(0, iLineLength, 0), Color.Green);
			msaAxisDirectionVertices[4] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Blue);
			msaAxisDirectionVertices[5] = new VertexPositionColor(new Vector3(0, 1, iLineLength), Color.Blue);
			
			mcAxisEffect = new BasicEffect(GraphicsDevice);
			mcAxisEffect.VertexColorEnabled = true;
			mcAxisEffect.LightingEnabled = false;
			mcAxisEffect.TextureEnabled = false;
			mcAxisEffect.FogEnabled = false;
			mcAxisVertexDeclaration = VertexPositionColor.VertexDeclaration;

			// Instantiate all of the Particle Systems
			_mcDPSFSplashScreenDPSFDemoParticleSystemWrapper = new DPSFSplashScreenDPSFDemoParticleSystemWrapper(this);
			_mcRandomDPSFDemoParticleSystemWrapper = new RandomDPSFDemoParticleSystemWrapper(this);
            _mcFireDPSFDemoParticleSystemWrapper = new FireDPSFDemoParticleSystemWrapper(this);
            _mcFireSpriteDPSFDemoParticleSystemWrapper = new FireSpriteDPSFDemoParticleSystemWrapper(this);
            _mcSmokeDPSFDemoParticleSystemWrapper = new SmokeDPSFDemoParticleSystemWrapper(this);
            _mcSnowDPSFDemoParticleSystemWrapper = new SnowDPSFDemoParticleSystemWrapper(this);
            _mcSquarePatternDPSFDemoParticleSystemWrapper = new SquarePatternDPSFDemoParticleSystemWrapper(this);
            _mcFountainDPSFDemoParticleSystemWrapper = new FountainDPSFDemoParticleSystemWrapper(this);
            _mcRandom2DdpsfDemoParticleSystemWrapper = new Random2DDPSFDemoParticleSystemWrapper(this);
            _mcGasFallDPSFDemoParticleSystemWrapper = new GasFallDPSFDemoParticleSystemWrapper(this);
            _mcDotDPSFDemoParticleSystemWrapper = new DotDPSFDemoParticleSystemWrapper(this);
            _mcFireworksDPSFDemoParticleSystemWrapper = new FireworksDPSFDemoParticleSystemWrapper(this);
            _mcFigure8DPSFDemoParticleSystemWrapper = new Figure8DPSFDemoParticleSystemWrapper(this);
            _mcStarDPSFDemoParticleSystemWrapper = new StarDPSFDemoParticleSystemWrapper(this);
            _mcBallDPSFDemoParticleSystemWrapper = new BallDPSFDemoParticleSystemWrapper(this);
            _mcRotatingQuadDPSFDemoParticleSystemWrapper = new RotatingQuadsDPSFDemoParticleSystemWrapper(this);
            _mcBoxDPSFDemoParticleSystemWrapper = new BoxDPSFDemoParticleSystemWrapper(this);
            _mcImageDPSFDemoParticleSystemWrapper = new ImageDPSFDemoParticleSystemWrapper(this);
            _mcAnimatedQuadDPSFDemoParticleSystemWrapper = new AnimatedQuadDPSFDemoParticleSystemWrapper(this);
            _mcSpriteDPSFDemoParticleSystemWrapper = new SpriteDPSFDemoParticleSystemWrapper(this);
            _mcAnimatedSpriteDPSFDemoParticleSystemWrapper = new AnimatedSpriteDPSFDemoParticleSystemWrapper(this);
            _mcQuadSprayDPSFDemoParticleSystemWrapper = new QuadSprayDPSFDemoParticleSystemWrapper(this);
            _mcMagnetDPSFDemoParticleSystemWrapper = new MagnetsDPSFDemoParticleSystemWrapper(this);
            _mcSparklerDPSFDemoParticleSystemWrapper = new SparklerDPSFDemoParticleSystemWrapper(this);
            _mcGridQuadDPSFDemoParticleSystemWrapper = new GridQuadDPSFDemoParticleSystemWrapper(this);
            _mcSphereDPSFDemoParticleSystemWrapper = new SphereDPSFDemoParticleSystemWrapper(this);
            _mcMultipleDPSFDemoImagesDPSFDemoParticleSystemWrapper = new MultipleDPSFDemoParticleImagesDPSFDemoParticleSystemWrapper(this);
            _mcMultipleDPSFDemoImagesSpriteDPSFDemoParticleSystemWrapper = new MultipleDPSFDemoParticleImagesSpriteDPSFDemoParticleSystemWrapper(this);
            _mcExplosionFireSmokeDPSFDemoParticleSystemWrapper = new ExplosionFireSmokeDPSFDemoParticleSystemWrapper(this);
            _mcExplosionFlashDPSFDemoParticleSystemWrapper = new ExplosionFlashDPSFDemoParticleSystemWrapper(this);
            _mcExplosionFlyingSparksDPSFDemoParticleSystemWrapper = new ExplosionFlyingSparksDPSFDemoParticleSystemWrapper(this);
            _mcExplosionSmokeTrailsDPSFDemoParticleSystemWrapper = new ExplosionSmokeTrailsDPSFDemoParticleSystemWrapper(this);
            _mcExplosionRoundSparksDPSFDemoParticleSystemWrapper = new ExplosionRoundSparksDPSFDemoParticleSystemWrapper(this);
            _mcExplosionDebrisDPSFDemoParticleSystemWrapper = new ExplosionDebrisDPSFDemoParticleSystemWrapper(this);
            _mcExplosionDebrisSpriteDPSFDemoParticleSystemWrapper = new ExplosionDebrisSpriteDPSFDemoParticleSystemWrapper(this);
            _mcExplosionShockwaveDPSFDemoParticleSystemWrapper = new ExplosionShockwaveDPSFDemoParticleSystemWrapper(this);
            _mcExplosionDPSFDemoParticleSystemWrapper = new ExplosionDPSFDemoParticleSystemWrapper(this);
            _mcTrailDPSFDemoParticleSystemWrapper = new TrailDPSFDemoParticleSystemWrapper(this);
            _mcSpriteDPSFDemoParticleSystemTemplateWrapper = new SpriteDPSFDemoParticleSystemTemplateWrapper(this);
            _mcSprite3DBillboardDPSFDemoParticleSystemTemplateWrapper = new Sprite3DBillboardDPSFDemoParticleSystemTemplateWrapper(this);
            _mcQuadDPSFDemoParticleSystemTemplateWrapper = new QuadDPSFDemoParticleSystemTemplateWrapper(this);
            _mcTexturedQuadDPSFDemoParticleSystemTemplateWrapper = new TexturedQuadDPSFDemoParticleSystemTemplateWrapper(this);
            _mcDefaultSpriteDPSFDemoParticleSystemTemplateWrapper = new DefaultSpriteDPSFDemoParticleSystemTemplateWrapper(this);
            _mcDefaultSprite3DBillboardDPSFDemoParticleSystemTemplateWrapper = new DefaultSprite3DBillboardDPSFDemoParticleSystemTemplateWrapper(this);
            _mcDefaultQuadDPSFDemoParticleSystemTemplateWrapper = new DefaultQuadDPSFDemoParticleSystemTemplateWrapper(this);
            _mcDefaultTexturedQuadDPSFDemoParticleSystemTemplateWrapper = new DefaultTexturedQuadDPSFDemoParticleSystemTemplateWrapper(this);

			// Add all Particle Systems to the Particle System Manager
			_particleSystemManager.AddParticleSystem(_mcDPSFSplashScreenDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcRandomDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcFireDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcFireSpriteDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcSmokeDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcSnowDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcSquarePatternDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcFountainDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcRandom2DdpsfDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcGasFallDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcDotDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcFireworksDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcFigure8DPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcStarDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcBallDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcRotatingQuadDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcBoxDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcImageDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcAnimatedQuadDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcSpriteDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcAnimatedSpriteDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcQuadSprayDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcMagnetDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcSparklerDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcGridQuadDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcSphereDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcMultipleDPSFDemoImagesDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcMultipleDPSFDemoImagesSpriteDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcExplosionFireSmokeDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcExplosionFlashDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcExplosionFlyingSparksDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcExplosionSmokeTrailsDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcExplosionRoundSparksDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcExplosionDebrisDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcExplosionDebrisSpriteDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcExplosionShockwaveDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcExplosionDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcTrailDPSFDemoParticleSystemWrapper);
			_particleSystemManager.AddParticleSystem(_mcSpriteDPSFDemoParticleSystemTemplateWrapper);
			_particleSystemManager.AddParticleSystem(_mcSprite3DBillboardDPSFDemoParticleSystemTemplateWrapper);
			_particleSystemManager.AddParticleSystem(_mcQuadDPSFDemoParticleSystemTemplateWrapper);
			_particleSystemManager.AddParticleSystem(_mcTexturedQuadDPSFDemoParticleSystemTemplateWrapper);
			_particleSystemManager.AddParticleSystem(_mcDefaultSpriteDPSFDemoParticleSystemTemplateWrapper);
			_particleSystemManager.AddParticleSystem(_mcDefaultSprite3DBillboardDPSFDemoParticleSystemTemplateWrapper);
			_particleSystemManager.AddParticleSystem(_mcDefaultQuadDPSFDemoParticleSystemTemplateWrapper);
			_particleSystemManager.AddParticleSystem(_mcDefaultTexturedQuadDPSFDemoParticleSystemTemplateWrapper);

			// Set how often the Particle Systems should be Updated
			_particleSystemManager.UpdatesPerSecond = PARTICLE_SYSTEM_UPDATES_PER_SECOND;

			// Hide text and other things while displaying the Splash Screen
			mbShowText = false; 
            mbShowFloor = false;

			// Setup the Splash Screen to display before anything else
			_mcDPSFSplashScreenDPSFDemoParticleSystemWrapper.AutoInitialize(this.GraphicsDevice, this.Content, null);
			_mcDPSFSplashScreenDPSFDemoParticleSystemWrapper.SplashScreenComplete += new EventHandler(mcDPSFSplashScreenParticleSystem_SplashScreenComplete);
			_currentDPSFDemoParticleSystemWrapper = _mcDPSFSplashScreenDPSFDemoParticleSystemWrapper;
		}

		/// <summary>
		/// Handles the SplashScreenComplete event of the mcDPSFSplashScreenParticleSystem control.
		/// This gets called when the Splash Screen is done playing, so we can then load the regular code to start the demo.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void mcDPSFSplashScreenParticleSystem_SplashScreenComplete(object sender, EventArgs e)
		{
			// Now that the Splash Screen is done displaying, clean it up.
			_mcDPSFSplashScreenDPSFDemoParticleSystemWrapper.SplashScreenComplete -= new EventHandler(mcDPSFSplashScreenParticleSystem_SplashScreenComplete);
			_particleSystemManager.RemoveParticleSystem(_mcDPSFSplashScreenDPSFDemoParticleSystemWrapper);
			_mcDPSFSplashScreenDPSFDemoParticleSystemWrapper.Destroy();
			_mcDPSFSplashScreenDPSFDemoParticleSystemWrapper = null;

			// Reset some of the settings now that the Splash Screen is done.
			mbShowText = true;
			mbShowFloor = true;

			// Start displaying the demo's particle systems
			_currentDPSFDemoParticleSystemWrapper = null;
			InitializeCurrentParticleSystem();
		}

		public void InitializeCurrentParticleSystem()
		{
			// If the Current Particle System has been set
			if (_currentDPSFDemoParticleSystemWrapper != null)
			{
				// Destroy the Current Particle System.
				// This frees up any resources/memory held by the Particle System, so it's
				// good to destroy them if we know they won't be used for a while.
				_currentDPSFDemoParticleSystemWrapper.Destroy();
			}

			// Initialize the Current Particle System
			switch (meCurrentPS)
			{
				default:
				case PSEffects.Random: _currentDPSFDemoParticleSystemWrapper = _mcRandomDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Fire: _currentDPSFDemoParticleSystemWrapper = _mcFireDPSFDemoParticleSystemWrapper; break;
				case PSEffects.FireSprite: _currentDPSFDemoParticleSystemWrapper = _mcFireSpriteDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Smoke: _currentDPSFDemoParticleSystemWrapper = _mcSmokeDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Snow: _currentDPSFDemoParticleSystemWrapper = _mcSnowDPSFDemoParticleSystemWrapper; break;
				case PSEffects.SquarePattern: _currentDPSFDemoParticleSystemWrapper = _mcSquarePatternDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Fountain: _currentDPSFDemoParticleSystemWrapper = _mcFountainDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Random2D: _currentDPSFDemoParticleSystemWrapper = _mcRandom2DdpsfDemoParticleSystemWrapper; break;
				case PSEffects.GasFall: _currentDPSFDemoParticleSystemWrapper = _mcGasFallDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Dot: _currentDPSFDemoParticleSystemWrapper = _mcDotDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Fireworks: _currentDPSFDemoParticleSystemWrapper = _mcFireworksDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Figure8: _currentDPSFDemoParticleSystemWrapper = _mcFigure8DPSFDemoParticleSystemWrapper; break;
				case PSEffects.Star: _currentDPSFDemoParticleSystemWrapper = _mcStarDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Ball: _currentDPSFDemoParticleSystemWrapper = _mcBallDPSFDemoParticleSystemWrapper; break;
				case PSEffects.RotatingQuad: _currentDPSFDemoParticleSystemWrapper = _mcRotatingQuadDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Box: _currentDPSFDemoParticleSystemWrapper = _mcBoxDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Image: _currentDPSFDemoParticleSystemWrapper = _mcImageDPSFDemoParticleSystemWrapper; break;
				case PSEffects.AnimatedTexturedQuad: _currentDPSFDemoParticleSystemWrapper = _mcAnimatedQuadDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Sprite: _currentDPSFDemoParticleSystemWrapper = _mcSpriteDPSFDemoParticleSystemWrapper; break;
				case PSEffects.AnimatedSprite: _currentDPSFDemoParticleSystemWrapper = _mcAnimatedSpriteDPSFDemoParticleSystemWrapper; break;
				case PSEffects.QuadSpray: _currentDPSFDemoParticleSystemWrapper = _mcQuadSprayDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Magnets: _currentDPSFDemoParticleSystemWrapper = _mcMagnetDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Sparkler: _currentDPSFDemoParticleSystemWrapper = _mcSparklerDPSFDemoParticleSystemWrapper; break;
				case PSEffects.GridQuad: _currentDPSFDemoParticleSystemWrapper = _mcGridQuadDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Sphere: _currentDPSFDemoParticleSystemWrapper = _mcSphereDPSFDemoParticleSystemWrapper; break;
				case PSEffects.MultipleParticleImages: _currentDPSFDemoParticleSystemWrapper = _mcMultipleDPSFDemoImagesDPSFDemoParticleSystemWrapper; break;
				case PSEffects.MultipleParticleImagesSprite: _currentDPSFDemoParticleSystemWrapper = _mcMultipleDPSFDemoImagesSpriteDPSFDemoParticleSystemWrapper; break;
				case PSEffects.ExplosionFireSmoke: _currentDPSFDemoParticleSystemWrapper = _mcExplosionFireSmokeDPSFDemoParticleSystemWrapper; break;
				case PSEffects.ExplosionFlash: _currentDPSFDemoParticleSystemWrapper = _mcExplosionFlashDPSFDemoParticleSystemWrapper; break;
				case PSEffects.ExplosionFlyingSparks: _currentDPSFDemoParticleSystemWrapper = _mcExplosionFlyingSparksDPSFDemoParticleSystemWrapper; break;
				case PSEffects.ExplosionSmokeTrails: _currentDPSFDemoParticleSystemWrapper = _mcExplosionSmokeTrailsDPSFDemoParticleSystemWrapper; break;
				case PSEffects.ExplosionRoundSparks: _currentDPSFDemoParticleSystemWrapper = _mcExplosionRoundSparksDPSFDemoParticleSystemWrapper; break;
				case PSEffects.ExplosionDebris: _currentDPSFDemoParticleSystemWrapper = _mcExplosionDebrisDPSFDemoParticleSystemWrapper; break;
				case PSEffects.ExplosionDebrisSprite: _currentDPSFDemoParticleSystemWrapper = _mcExplosionDebrisSpriteDPSFDemoParticleSystemWrapper; break;
				case PSEffects.ExplosionShockwave: _currentDPSFDemoParticleSystemWrapper = _mcExplosionShockwaveDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Explosion: _currentDPSFDemoParticleSystemWrapper = _mcExplosionDPSFDemoParticleSystemWrapper; break;
				case PSEffects.Trail: _currentDPSFDemoParticleSystemWrapper = _mcTrailDPSFDemoParticleSystemWrapper; break;
				case PSEffects.SpriteParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcSpriteDPSFDemoParticleSystemTemplateWrapper; break;
				case PSEffects.Sprite3DBillboardParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcSprite3DBillboardDPSFDemoParticleSystemTemplateWrapper; break;
				case PSEffects.QuadParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcQuadDPSFDemoParticleSystemTemplateWrapper; break;
				case PSEffects.TexturedQuadParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcTexturedQuadDPSFDemoParticleSystemTemplateWrapper; break;
				case PSEffects.DefaultSpriteParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcDefaultSpriteDPSFDemoParticleSystemTemplateWrapper; break;
				case PSEffects.DefaultSprite3DBillboardParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcDefaultSprite3DBillboardDPSFDemoParticleSystemTemplateWrapper; break;
				case PSEffects.DefaultQuadParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcDefaultQuadDPSFDemoParticleSystemTemplateWrapper; break;
				case PSEffects.DefaultTexturedQuadParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcDefaultTexturedQuadDPSFDemoParticleSystemTemplateWrapper; break;
			}

			// Initialize the Particle System
			_currentDPSFDemoParticleSystemWrapper.AutoInitialize(this.GraphicsDevice, this.Content, null);

			// Do any necessary after initialization work 
			_currentDPSFDemoParticleSystemWrapper.AfterAutoInitialize();
		}

		#endregion

		#region Update and Draw

		/// <summary>
		/// Allows the game to run logic
		/// </summary>
		protected override void Update(GameTime cGameTime)
		{
			// Get and process user Input
			ProcessInput(cGameTime);

			// Allow the camera to be moved around, even if the particle systems are paused

			// Update the World, View, and Projection matrices
			UpdateWorldViewProjectionMatrices();


			// Update the Quad Particle Systems to know where the Camera is so that they can display
			// the particles as billboards if needed (i.e. have particle always face the camera).
			_particleSystemManager.SetCameraPositionForAllParticleSystems(msCamera.Position);

			// Set the World, View, and Projection Matrices for the Particle Systems
			_particleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(msWorldMatrix, msViewMatrix, msProjectionMatrix);

			// If the Game is Paused
			if (mbPaused)
			{
				// Update the particle systems with 0 elapsed time, just to allow the particles to rotate to face the camera.
				_particleSystemManager.UpdateAllParticleSystems(0);

				// Exit without updating anything
				return;
			}

			// If the Current Particle System is Initialized
			if (_currentDPSFDemoParticleSystemWrapper != null && _currentDPSFDemoParticleSystemWrapper.IsInitialized)
			{
				// If Static Particles should be drawn
				if (mbDrawStaticPS)
				{
					// If the Static Particles haven't been drawn yet
					if (!mbStaticParticlesDrawn)
					{
						// Draw this frame to a Render Target so we can have it persist across frames.
						SetupToDrawToRenderTarget(true);

						// Update the Particle System iteratively by the Time Step amount until the 
						// Particle System behavior over the Total Time has been drawn
						float fElapsedTime = 0;
						while (fElapsedTime < mfStaticParticleTotalTime)
						{
							// Update and draw this frame of the Particle System
							_particleSystemManager.UpdateAllParticleSystems(mfStaticParticleTimeStep);
							_particleSystemManager.DrawAllParticleSystems();
							fElapsedTime += mfStaticParticleTimeStep;
						}
						mbStaticParticlesDrawn = true;

						mcSpriteBatch.Begin();
						mcSpriteBatch.DrawString(mcFont, "F6 to continue", new Vector2(310, 25), Color.LawnGreen);
						mcSpriteBatch.End();
					}
				}
				// Else the Particle Systems should be drawn normally
				else
				{
					// Update all Particle Systems manually
					_particleSystemManager.UpdateAllParticleSystems((float)cGameTime.ElapsedGameTime.TotalSeconds);
				}


				// If the Sphere is Visible and we are on the Smoke Particle System
				if (mcSphere.bVisible && meCurrentPS == PSEffects.Smoke)
				{
					// Update it
					mcSphere.Update((float)cGameTime.ElapsedGameTime.TotalSeconds);

					// Update the PS's External Object Position to the Sphere's Position
					_mcSmokeDPSFDemoParticleSystemWrapper.mcExternalObjectPosition = mcSphere.sPosition;

					// If the Sphere has been alive long enough
					if (mcSphere.cTimeAliveInSeconds > TimeSpan.FromSeconds(6.0f))
					{
						mcSphere.bVisible = false;
						_mcSmokeDPSFDemoParticleSystemWrapper.StopParticleAttractionAndRepulsionToExternalObject();
					}
				}
			}
		
			// Update any other Drawable Game Components.
			base.Update(cGameTime);

			// If we are drawing garbage collection info.
			if (DrawPerformanceText)
			{
				// Record how much Garbage is waiting to be collected in Kilobytes.
				_garbageCurrentAmountInKB = GC.GetTotalMemory(false) / 1024f;

				// Increment the number of updates that have been performed in the past second.
				_updatesPerSecond++;
			}
		}

		/// <summary>
		/// Updates the World, View, and Projection matrices according to the Camera's current position.
		/// </summary>
		private void UpdateWorldViewProjectionMatrices()
		{
			// Compute the Aspect Ratio of the window
			float fAspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;

			// Setup the View matrix depending on which Camera mode is being used.
			// If we are using the Fixed Camera.
			if (msCamera.bUsingFixedCamera)
			{
				// Set up the View matrix according to the Camera's arc, rotation, and distance from the Offset position.
				msViewMatrix = Matrix.CreateTranslation(msCamera.sFixedCameraLookAtPosition) *
									 Matrix.CreateRotationY(MathHelper.ToRadians(msCamera.fCameraRotation)) *
									 Matrix.CreateRotationX(MathHelper.ToRadians(msCamera.fCameraArc)) *
									 Matrix.CreateLookAt(new Vector3(0, 0, -msCamera.fCameraDistance),
														 new Vector3(0, 0, 0), Vector3.Up);
			}
			// Else we are using the Free Camera
			else
			{
				// Set up our View matrix specifying the Camera position, a point to look-at, and a direction for which way is up.
				msViewMatrix = Matrix.CreateLookAt(msCamera.sVRP, msCamera.sVRP + msCamera.cVPN, msCamera.cVUP);
			}

			// Setup the Projection matrix by specifying the field of view (1/4 pi), aspect ratio, and the near and far clipping planes
			msProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, fAspectRatio, 1, 10000);
		}

		/// <summary>
		/// This is called when the game should draw itself
		/// </summary>
		protected override void Draw(GameTime cGameTime)
		{
			// If Static Particles were drawn to the Render Target, draw the Render Target to the screen and exit without drawing anything else.
			if (mbDrawStaticPS)
			{
				DrawRenderTargetToScreen();
				return;
			}

			// Clear the scene
			GraphicsDevice.Clear(BACKGROUND_COLOR);

			// If the screen should NOT be cleared each frame, draw to a render target instead of right to the screen.
			if (!mbClearScreenEveryFrame)
			{
				SetupToDrawToRenderTarget(_clearScreenEveryFrameJustToggled);
				_clearScreenEveryFrameJustToggled = false;
			}
			
			// Draw the Floor at the origin (0,0,0) and any other models
			DrawModels(msWorldMatrix, msViewMatrix, msProjectionMatrix);

			// If the Axis' should be drawn
			if (mbShowAxis)
			{
				// Draw lines at the origin (0,0,0) indicating positive axis directions
				DrawAxis(msWorldMatrix, msViewMatrix, msProjectionMatrix);
			}


			// Draw any other Drawable Game Components that may need to be drawn.
			// Call this before drawing our Particle Systems, so that our 2D Sprite particles
			// show up on top of the any other 2D Sprites drawn.
			base.Draw(cGameTime);

			// Draw the Particle Systems manually
			_particleSystemManager.DrawAllParticleSystems();


			// Update the Frames Per Second to be displayed
			FPS.Update((float)cGameTime.ElapsedGameTime.TotalSeconds);

			// Draw the Text to the screen last, so it is always on top
			DrawText();


			// If we were drawing this frame to the Render Target, draw the Render Target to the screen.
			if (!mbClearScreenEveryFrame)
			{
				DrawRenderTargetToScreen();
			}
		}

		/// <summary>
		/// Sets up the app to draw to the render target instead of directly to the screen.
		/// </summary>
		/// <param name="clearRenderTarget">If true the Render Target will be cleared before anything else is drawn to it.</param>
		private void SetupToDrawToRenderTarget(bool clearRenderTarget)
		{
			// Draw this frame to a Render Target so we can have it persist across frames.
			GraphicsDevice.SetRenderTarget(_renderTarget);

			// If we are about to start drawing to the Render Target, clear out any contents it may already have.
			if (clearRenderTarget)
				GraphicsDevice.Clear(BACKGROUND_COLOR);
		}

		/// <summary>
		/// Draws the Render Target contents to the screen.
		/// </summary>
		private void DrawRenderTargetToScreen()
		{
			GraphicsDevice.SetRenderTarget(null);		// Start drawing to the screen again instead of to the Render Target.
			GraphicsDevice.Clear(BACKGROUND_COLOR);
			mcSpriteBatch.Begin();
			// Draw the Render Target contents to the screen
			mcSpriteBatch.Draw(_renderTarget, new Rectangle(0, 0, _renderTarget.Width, _renderTarget.Height), Color.White);
			mcSpriteBatch.End();
		}

		/// <summary>
		/// Function to draw Text to the screen
		/// </summary>
		void DrawText()
		{
			// If no Text should be shown
			if (!mbShowText)
			{
				// Exit the function before drawing any Text
				return;
			}

            // Get area on screen that it is safe to draw text to (so that we are sure it will be displayed on the screen).
		    Rectangle textSafeArea = GetTextSafeArea();

            // Setup the 
		    var toolsToDrawText = new DrawTextRequirements()
		                              {
                                          TextWriter = mcSpriteBatch,
		                                  Font = mcFont,
		                                  TextSafeArea = textSafeArea,
		                                  ControlTextColor = CONTROL_TEXT_COLOR,
		                                  PropertyTextColor = PROPERTY_TEXT_COlOR,
		                                  ValueTextColor = VALUE_TEXT_COLOR
		                              };

			// If we don't have a handle to a particle system, it is because we serialized it
			if (_currentDPSFDemoParticleSystemWrapper == null)
			{
				mcSpriteBatch.Begin();
				mcSpriteBatch.DrawString(mcFont, "Particle system has been serialized to the file: " + msSerializedPSFileName + ".", new Vector2(25, 200), PROPERTY_TEXT_COlOR);
				mcSpriteBatch.DrawString(mcFont, "To deserialize the particle system from the file, restoring the instance of", new Vector2(25, 225), PROPERTY_TEXT_COlOR);
				mcSpriteBatch.DrawString(mcFont, "the particle system,", new Vector2(25, 250), PROPERTY_TEXT_COlOR);
				mcSpriteBatch.DrawString(mcFont, "press F9", new Vector2(210, 250), CONTROL_TEXT_COLOR);
				mcSpriteBatch.End();
				return;
			}
			
			// If the Particle System has been destroyed, just write that to the screen and exit
			if (!_currentDPSFDemoParticleSystemWrapper.IsInitialized)
			{
				mcSpriteBatch.Begin();
				mcSpriteBatch.DrawString(mcFont, "The current particle system has been destroyed.", new Vector2(140, 200), PROPERTY_TEXT_COlOR);
				mcSpriteBatch.DrawString(mcFont, "Press G / H to switch to a different particle system.", new Vector2(125, 225), PROPERTY_TEXT_COlOR);
				mcSpriteBatch.End();
				return;
			}

			// Get the Name of the Particle System and how many Particles are currently Active
			string sEffectName = _currentDPSFDemoParticleSystemWrapper.Name;
            int iNumberOfActiveParticles = _currentDPSFDemoParticleSystemWrapper.TotalNumberOfActiveParticles;
            int iNumberOfParticlesAllocatedInMemory = _currentDPSFDemoParticleSystemWrapper.TotalNumberOfParticlesAllocatedInMemory;

			// Convert values to strings
			string sFPSValue = FPS.CurrentFPS.ToString();
			string sAvgFPSValue = FPS.AverageFPS.ToString("0.0");
			string sTotalParticleCountValue = iNumberOfActiveParticles.ToString();
			string sEmitterOnValue = (_currentDPSFDemoParticleSystemWrapper.Emitter.EmitParticlesAutomatically ? "On" : "Off");
			string sParticleSystemEffectValue = sEffectName;
			string sParticlesPerSecondValue = _currentDPSFDemoParticleSystemWrapper.Emitter.ParticlesPerSecond.ToString("0.00");
			string sCameraModeValue = msCamera.bUsingFixedCamera ? "Fixed" : "Free";
			string sPSSpeedScale = _particleSystemManager.SimulationSpeed.ToString("0.0");
			string sCameraPosition = "(" + msCamera.Position.X.ToString("0") + "," + msCamera.Position.Y.ToString("0") + "," + msCamera.Position.Z.ToString("0") + ")";
			string sAllocatedParticles = iNumberOfParticlesAllocatedInMemory.ToString();
			string sTexture = "N/A";
			if (_currentDPSFDemoParticleSystemWrapper.Texture != null)
			{
				sTexture = _currentDPSFDemoParticleSystemWrapper.Texture.Name.TrimStart("Textures/".ToCharArray());
			}

			// Draw all of the text.
			mcSpriteBatch.Begin();

            // If the Particle System is Paused, draw a Paused message.
            if (mbPaused)
            {
                mcSpriteBatch.DrawString(mcFont, "Paused", new Vector2(textSafeArea.Left + 350, textSafeArea.Top + 25), VALUE_TEXT_COLOR);
            }

            // Draw text that is always displayed.
			mcSpriteBatch.DrawString(mcFont, "FPS:", new Vector2(textSafeArea.Left + 5, textSafeArea.Bottom - 50), PROPERTY_TEXT_COlOR);
			mcSpriteBatch.DrawString(mcFont, sFPSValue, new Vector2(textSafeArea.Left + 50, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			mcSpriteBatch.DrawString(mcFont, "Allocated:", new Vector2(textSafeArea.Left + 120, textSafeArea.Bottom - 50), PROPERTY_TEXT_COlOR);
			mcSpriteBatch.DrawString(mcFont, sAllocatedParticles, new Vector2(textSafeArea.Left + 210, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			//mcSpriteBatch.DrawString(mcFont, "Position:", new Vector2(textSafeArea.Left + 275, textSafeArea.Bottom - 75), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, sCameraPosition, new Vector2(textSafeArea.Left + 280, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			mcSpriteBatch.DrawString(mcFont, "Texture:", new Vector2(textSafeArea.Left + 440, textSafeArea.Bottom - 50), PROPERTY_TEXT_COlOR);
			mcSpriteBatch.DrawString(mcFont, sTexture, new Vector2(textSafeArea.Left + 520, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			mcSpriteBatch.DrawString(mcFont, "Speed:", new Vector2(textSafeArea.Right - 100, textSafeArea.Bottom - 50), PROPERTY_TEXT_COlOR);
			mcSpriteBatch.DrawString(mcFont, sPSSpeedScale, new Vector2(textSafeArea.Right - 35, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			mcSpriteBatch.DrawString(mcFont, "Avg:", new Vector2(textSafeArea.Left + 5, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			mcSpriteBatch.DrawString(mcFont, sAvgFPSValue, new Vector2(textSafeArea.Left + 50, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			mcSpriteBatch.DrawString(mcFont, "Particles:", new Vector2(textSafeArea.Left + 120, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			mcSpriteBatch.DrawString(mcFont, sTotalParticleCountValue, new Vector2(textSafeArea.Left + 205, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			mcSpriteBatch.DrawString(mcFont, "Emitter:", new Vector2(textSafeArea.Left + 275, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			mcSpriteBatch.DrawString(mcFont, sEmitterOnValue, new Vector2(textSafeArea.Left + 345, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			mcSpriteBatch.DrawString(mcFont, "Particles Per Second:", new Vector2(textSafeArea.Left + 390, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			mcSpriteBatch.DrawString(mcFont, sParticlesPerSecondValue, new Vector2(textSafeArea.Left + 585, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			mcSpriteBatch.DrawString(mcFont, "Camera:", new Vector2(textSafeArea.Left + 660, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			mcSpriteBatch.DrawString(mcFont, sCameraModeValue, new Vector2(textSafeArea.Left + 740, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			mcSpriteBatch.DrawString(mcFont, "Effect:", new Vector2(textSafeArea.Left + 5, textSafeArea.Top + 2), PROPERTY_TEXT_COlOR);
			mcSpriteBatch.DrawString(mcFont, sParticleSystemEffectValue, new Vector2(textSafeArea.Left + 70, textSafeArea.Top + 2), VALUE_TEXT_COLOR);

			mcSpriteBatch.DrawString(mcFont, "Show/Hide Controls:", new Vector2(textSafeArea.Right - 260, textSafeArea.Top + 2), PROPERTY_TEXT_COlOR);
			mcSpriteBatch.DrawString(mcFont, "F1 - F4", new Vector2(textSafeArea.Right - 70, textSafeArea.Top + 2), CONTROL_TEXT_COLOR);


            // Display particle system specific values.
            _currentDPSFDemoParticleSystemWrapper.DrawStatusText(toolsToDrawText);

            // If the Particle System specific Controls should be shown, display them.
            if (mbShowPSControls)
            {
                _currentDPSFDemoParticleSystemWrapper.DrawInputControlsText(toolsToDrawText);
            }

            // If the Common Controls should be shown, display them.
            if (mbShowCommonControls)
            {
                mcSpriteBatch.DrawString(mcFont, "Change Particle System:", new Vector2(5, 25), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "G / H", new Vector2(235, 25), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Toggle Emitter On/Off:", new Vector2(5, 50), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "Delete", new Vector2(220, 50), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Increase/Decrease Emitter Speed:", new Vector2(5, 75), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "+ / -", new Vector2(320, 75), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Add Particle:", new Vector2(5, 100), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "Insert(one), Home(many), PgUp(max)", new Vector2(130, 100), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Move Emitter:", new Vector2(5, 125), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "A/D, W/S, Q/E", new Vector2(135, 125), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Rotate Emitter:", new Vector2(5, 150), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "J/L(yaw), I/Vertex(pitch), U/O(roll)", new Vector2(150, 150), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Rotate Emitter Around Pivot:", new Vector2(5, 175), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "Y + Rotate Emitter", new Vector2(275, 175), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Reset Emitter's Position and Orientation:", new Vector2(5, 200), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "Z", new Vector2(375, 200), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Toggle Floor:", new Vector2(485, 25), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "F", new Vector2(610, 25), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Toggle Axis:", new Vector2(650, 25), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "F7", new Vector2(770, 25), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Toggle Full Screen:", new Vector2(485, 50), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "End", new Vector2(665, 50), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Toggle Camera Mode:", new Vector2(485, 75), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "PgDown", new Vector2(690, 75), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Reset Camera Position:", new Vector2(485, 100), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "R", new Vector2(705, 100), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Change Texture:", new Vector2(485, 125), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "T / Shift + T", new Vector2(640, 125), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Pause Particle System:", new Vector2(485, 150), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "Spacebar", new Vector2(700, 150), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Speed Up/Down PS:", new Vector2(485, 175), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "* / /", new Vector2(680, 175), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Draw Static Particles:", new Vector2(485, 200), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "F6", new Vector2(690, 200), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Clear Screen Each Frame:", new Vector2(485, 225), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "F5", new Vector2(730, 225), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Create Animation Images:", new Vector2(485, 250), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "F8", new Vector2(725, 250), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Serialize Particle System:", new Vector2(485, 275), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "F9", new Vector2(725, 275), CONTROL_TEXT_COLOR);

                mcSpriteBatch.DrawString(mcFont, "Draw Performance Info:", new Vector2(485, 300), PROPERTY_TEXT_COlOR);
                mcSpriteBatch.DrawString(mcFont, "F10", new Vector2(705, 300), CONTROL_TEXT_COLOR);
            }

			// If the Camera Controls should be shown
			if (mbShowCameraControls)
			{
				// If we are using a Fixed Camera
				if (msCamera.bUsingFixedCamera)
				{
					mcSpriteBatch.DrawString(mcFont, "Fixed Camera Controls:", new Vector2(5, mcGraphics.PreferredBackBufferHeight - 125), PROPERTY_TEXT_COlOR);
					mcSpriteBatch.DrawString(mcFont, "Keys: Left/Right Arrows, Up/Down Arrows, Num0/Num1", new Vector2(15, mcGraphics.PreferredBackBufferHeight - 100), CONTROL_TEXT_COLOR);
					mcSpriteBatch.DrawString(mcFont, "Mouse: Left Button + X/Y Movement, Right Button + Y Movement", new Vector2(15, mcGraphics.PreferredBackBufferHeight - 75), CONTROL_TEXT_COLOR);
				}
				// Else we are using a Free Camera
				else
				{
					mcSpriteBatch.DrawString(mcFont, "Free Camera Controls", new Vector2(5, mcGraphics.PreferredBackBufferHeight - 125), PROPERTY_TEXT_COlOR);
					mcSpriteBatch.DrawString(mcFont, "Keys: Left/Right Arrows, Up/Down Arrows, Num0/Num1, Num4/Num6, Num8/Num2", new Vector2(15, mcGraphics.PreferredBackBufferHeight - 100), CONTROL_TEXT_COLOR);
					mcSpriteBatch.DrawString(mcFont, "Mouse: Left Button + X/Y Movement, Right Button + X/Y Movement, Scroll Wheel", new Vector2(15, mcGraphics.PreferredBackBufferHeight - 75), CONTROL_TEXT_COLOR);
				}
			}

			// If we should draw the number of bytes allocated in memory
			if (DrawPerformanceText)
			{
				mcSpriteBatch.DrawString(mcFont, "Update Time (ms): " + _particleSystemManager.TotalPerformanceTimeToDoUpdatesInMilliseconds.ToString("0.000"), new Vector2(529, mcGraphics.PreferredBackBufferHeight - 250), PROPERTY_TEXT_COlOR);
				mcSpriteBatch.DrawString(mcFont, "Draw Time (ms): " + _particleSystemManager.TotalPerformanceTimeToDoDrawsInMilliseconds.ToString("0.000"), new Vector2(545, mcGraphics.PreferredBackBufferHeight - 225), PROPERTY_TEXT_COlOR);
				mcSpriteBatch.DrawString(mcFont, "Garbage Allocated (KB): " + _garbageCurrentAmountInKB.ToString("0.0"), new Vector2(480, mcGraphics.PreferredBackBufferHeight - 200), PROPERTY_TEXT_COlOR);
				mcSpriteBatch.DrawString(mcFont, "Avg Garbage Per Update (KB): " + _garbageAverageCreatedPerUpdateInKB.ToString("0.000"), new Vector2(440, mcGraphics.PreferredBackBufferHeight - 175), PROPERTY_TEXT_COlOR);
				mcSpriteBatch.DrawString(mcFont, "Avg Garbage Per Frame (KB): " + _garbageAverageCreatedPerFrameInKB.ToString("0.000"), new Vector2(445, mcGraphics.PreferredBackBufferHeight - 150), PROPERTY_TEXT_COlOR);
			}

			// Stop drawing text
			mcSpriteBatch.End();
		}

		/// <summary>
		/// Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).
		/// </summary>
		/// <returns>Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).</returns>
		Rectangle GetTextSafeArea()
		{
			return GetTextSafeArea(0.9f);
		}

		/// <summary>
		/// Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).
		/// </summary>
		/// <param name="fNormalizedPercent">The amount of screen space (normalized between 0.0 - 1.0) that should 
		/// safe to draw to (e.g. 0.8 to have a 10% border on all sides)</param>
		/// <returns>Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).</returns>
		Rectangle GetTextSafeArea(float fNormalizedPercent)
		{
			Rectangle rTextSafeArea = new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y,
											GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
#if (XBOX)
			// Find Title Safe area of Xbox 360.
			float fBorder = (1 - fNormalizedPercent) / 2;
			rTextSafeArea.X = (int)(fBorder * rTextSafeArea.Width);
			rTextSafeArea.Y = (int)(fBorder * rTextSafeArea.Height);
			rTextSafeArea.Width = (int)(fNormalizedPercent * rTextSafeArea.Width);
			rTextSafeArea.Height = (int)(fNormalizedPercent * rTextSafeArea.Height);
#endif
			return rTextSafeArea;
		}

		/// <summary>
		/// Helper for drawing the Models
		/// </summary>
		void DrawModels(Matrix cWorldMatrix, Matrix cViewMatrix, Matrix cProjectionMatrix)
		{
			// Set our sampler state to allow the ground to have a repeated texture
			GraphicsDevice.BlendState = BlendState.Opaque;
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

			// If the Floor should be drawn
			if (mbShowFloor)
			{
				mcFloorModel.Draw(cWorldMatrix, cViewMatrix, cProjectionMatrix);
			}

			// If the Sphere should be visible
			if (mcSphere.bVisible)
			{
				mcSphereModel.Draw(Matrix.CreateScale(mcSphere.fSize) * Matrix.CreateTranslation(mcSphere.sPosition), cViewMatrix, cProjectionMatrix);
			}
		}

		/// <summary>
		/// Helper for drawing lines showing the positive axis directions
		/// </summary>
		void DrawAxis(Matrix cWorldMatrix, Matrix cViewMatrix, Matrix cProjectionMatrix)
		{
			mcAxisEffect.World = cWorldMatrix;
			mcAxisEffect.View = cViewMatrix;
			mcAxisEffect.Projection = cProjectionMatrix;

			// Draw the lines
			foreach (EffectPass cPass in mcAxisEffect.CurrentTechnique.Passes)
			{
				cPass.Apply();
				GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, msaAxisDirectionVertices, 0, 3, mcAxisVertexDeclaration);
			}
		}


		#endregion

		#region Handle Input

		/// <summary>
		/// Gets and processes all user input
		/// </summary>
		void ProcessInput(GameTime cGameTime)
		{
			// Save how long it's been since the last time Input was Handled
			float fTimeInSeconds = (float)cGameTime.ElapsedGameTime.TotalSeconds;

			// Save the state of the input devices at this frame
			KeyboardManager.UpdateKeyboardStateForThisFrame(cGameTime.ElapsedGameTime);
			MouseManager.UpdateMouseStateForThisFrame(cGameTime.ElapsedGameTime);
			GamePadsManager.UpdateGamePadStatesForThisFrame(cGameTime.ElapsedGameTime);

			// If we should Exit
			if (KeyboardManager.KeyIsDown(Keys.Escape) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.Back))
			{
				Exit();
			}

			// If we are currently showing the Splash Screen and a key was pressed, skip the Splash Screen.
			if (_mcDPSFSplashScreenDPSFDemoParticleSystemWrapper != null && 
				((KeyboardManager.CurrentKeyboardState.GetPressedKeys().Length > 0 && KeyboardManager.CurrentKeyboardState.GetPressedKeys()[0] != Keys.None) || 
				(MouseManager.CurrentMouseState.LeftButton == ButtonState.Pressed || MouseManager.CurrentMouseState.RightButton == ButtonState.Pressed) ||
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.A | Buttons.B | Buttons.X | Buttons.Y | Buttons.Start))))
			{
				_mcDPSFSplashScreenDPSFDemoParticleSystemWrapper.IsSplashScreenComplete = true;
				return;
			}

			// If we should toggle showing the Floor
			if (KeyboardManager.KeyWasJustPressed(Keys.F))
			{
				mbShowFloor = !mbShowFloor;
			}

			// If we should toggle Pausing the game
			if (KeyboardManager.KeyWasJustPressed(Keys.Space) || GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.Start))
			{
				mbPaused = !mbPaused;
			}

			// If we should toggle between Full Screen and Windowed mode
			if (KeyboardManager.KeyWasJustPressed(Keys.End))
			{
				mcGraphics.ToggleFullScreen();
			}

			// If we should toggle showing the Common Controls
			if (KeyboardManager.KeyWasJustPressed(Keys.F1))
			{
				mbShowCommonControls = !mbShowCommonControls;
			}

			// If we should toggle showing the Particle System specific Controls
			if (KeyboardManager.KeyWasJustPressed(Keys.F2))
			{
				mbShowPSControls = !mbShowPSControls;
			}

			// If we should toggle showing the Camera Controls
			if (KeyboardManager.KeyWasJustPressed(Keys.F3))
			{
				mbShowCameraControls = !mbShowCameraControls;
			}

			// If we should toggle showing the Common Controls
			if (KeyboardManager.KeyWasJustPressed(Keys.F4))
			{
				mbShowText = !mbShowText;
			}

			// If we should toggle Clearing the Screen each Frame
			if (KeyboardManager.KeyWasJustPressed(Keys.F5))
			{
				mbClearScreenEveryFrame = !mbClearScreenEveryFrame;
				_clearScreenEveryFrameJustToggled = true;
			}
			
			// If the particle lifetimes should be drawn in one frame
			if (KeyboardManager.KeyWasJustPressed(Keys.F6))
			{
				mbDrawStaticPS = !mbDrawStaticPS;
				mbStaticParticlesDrawn = false;
			}

			// If the Axis should be toggled on/off
			if (KeyboardManager.KeyWasJustPressed(Keys.F7))
			{
				mbShowAxis = !mbShowAxis;
			}
#if (WINDOWS)
			// If the PS should be drawn to files
			if (KeyboardManager.KeyWasJustPressed(Keys.F8))
			{
				// Draw the Particle System Animation to a series of Image Files
				_particleSystemManager.DrawAllParticleSystemsAnimationToFiles(GraphicsDevice, miDrawPSToFilesImageWidth, miDrawPSToFilesImageHeight, 
							msDrawPSToFilesDirectoryName, mfDrawPSToFilesTotalTime, mfDrawPSToFilesTimeStep, mbCreateAnimatedGIF, mbCreateTileSetImage);
			}

			// If the PS should be serialized to a file
			if (KeyboardManager.KeyWasJustPressed(Keys.F9))
			{
				// Only particle systems that do not inherit the DrawableGameComponent can be serialized.
				if (!DPSFHelper.DPSFInheritsDrawableGameComponent)
				{
					// If we have the particle system right now
					if (_currentDPSFDemoParticleSystemWrapper != null)
					{
						// Serialize the particle system into a file
						System.IO.Stream stream = System.IO.File.Open("SerializedParticleSystem.dat", System.IO.FileMode.Create);
						System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
						formatter.Serialize(stream, _currentDPSFDemoParticleSystemWrapper);
						stream.Close();

						// Remove the particle system from the manager and destroy the particle system in memory
						_particleSystemManager.RemoveParticleSystem(_currentDPSFDemoParticleSystemWrapper);
						_currentDPSFDemoParticleSystemWrapper.Destroy();
						_currentDPSFDemoParticleSystemWrapper = null;
					}
					// Else we don't have the particle system right now
					else
					{
						// Deserialize the particle system from a file
						System.IO.Stream stream = System.IO.File.Open("SerializedParticleSystem.dat", System.IO.FileMode.Open);
						System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        _currentDPSFDemoParticleSystemWrapper = (IWrapDPSFDemoParticleSystems)formatter.Deserialize(stream);
						stream.Close();

						try
						{
							// Setup the particle system properties that couldn't be serialized
							_currentDPSFDemoParticleSystemWrapper.InitializeNonSerializableProperties(this, this.GraphicsDevice, this.Content);
						}
						// Catch the case where the Particle System requires a texture, but one wasn't loaded
						catch (ArgumentNullException)
						{
							// Assign the particle system a texture to use
							_currentDPSFDemoParticleSystemWrapper.SetTexture("Textures/Bubble");
						}

						// Readd the particle system to the particle system manager
						_particleSystemManager.AddParticleSystem(_currentDPSFDemoParticleSystemWrapper);
					}
				}
			}
#endif
			// If the Performance Profiling was toggled
			if (KeyboardManager.KeyWasJustPressed(Keys.F10))
			{
				// Toggle if the Performance Profiling text should be drawn
				DrawPerformanceText = !DrawPerformanceText;
			}

			// Handle input for moving the Camera
			ProcessInputForCamera(fTimeInSeconds);

			// Handle input for controlling the Particle Systems
			ProcessInputForParticleSystem(fTimeInSeconds);
		}

		/// <summary>
		/// Handle input for moving the Camera
		/// </summary>
		public void ProcessInputForCamera(float fTimeInSeconds)
		{
			// If we are using the Fixed Camera
			if (msCamera.bUsingFixedCamera)
			{
				// If the Camera should be rotated vertically
				if (KeyboardManager.KeyIsDown(Keys.NumPad1) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D1)) ||
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickUp) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftStick)))
				{
					msCamera.fCameraArc -= fTimeInSeconds * 25;
				}

				if (KeyboardManager.KeyIsDown(Keys.NumPad0) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D0)) ||
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickDown) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftStick)))
				{
					msCamera.fCameraArc += fTimeInSeconds * 25;
				}

				// If the Camera should rotate horizontally
				if (KeyboardManager.KeyIsDown(Keys.Right) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickRight))
				{
					msCamera.fCameraRotation -= fTimeInSeconds * 50;
				}

				if (KeyboardManager.KeyIsDown(Keys.Left) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickLeft))
				{
					msCamera.fCameraRotation += fTimeInSeconds * 50;
				}

				// If Camera should be zoomed out
				if (KeyboardManager.KeyIsDown(Keys.Down) || 
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickDown) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftStick)))
				{
					msCamera.fCameraDistance += fTimeInSeconds * 250;
				}

				// If Camera should be zoomed in
				if (KeyboardManager.KeyIsDown(Keys.Up) || 
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickUp) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftStick)))
				{
					msCamera.fCameraDistance -= fTimeInSeconds * 250;
				}


				// Calculate how much the Mouse was moved
				int iXMovement = MouseManager.CurrentMouseState.X - MouseManager.PreviousMouseState.X;
				int iYMovement = MouseManager.CurrentMouseState.Y - MouseManager.PreviousMouseState.Y;
				int iZMovement = MouseManager.CurrentMouseState.ScrollWheelValue - MouseManager.PreviousMouseState.ScrollWheelValue;

				const float fMOUSE_MOVEMENT_SPEED = 0.5f;
				const float fMOUSE_ROTATION_SPEED = 0.5f;

				// If the Left Mouse Button is pressed
				if (MouseManager.CurrentMouseState.LeftButton == ButtonState.Pressed)
				{
					// If the Camera should rotate horizontally
					if (iXMovement != 0)
					{
						msCamera.fCameraRotation -= (iXMovement * fMOUSE_ROTATION_SPEED);
					}

					// If the Camera should rotate vertically
					if (iYMovement != 0)
					{
						msCamera.fCameraArc -= (-iYMovement * fMOUSE_ROTATION_SPEED);
					}
				}

				// If the Right Mouse Button is pressed
				if (MouseManager.CurrentMouseState.RightButton == ButtonState.Pressed)
				{
					// If the Camera should zoom in/out
					if (iYMovement != 0)
					{
						msCamera.fCameraDistance += iYMovement * fMOUSE_MOVEMENT_SPEED;
					}
				}


				// Limit the Arc movement
				if (msCamera.fCameraArc > 90.0f)
				{
					msCamera.fCameraArc = 90.0f;
				}
				else if (msCamera.fCameraArc < -90.0f)
				{
					msCamera.fCameraArc = -90.0f;
				}

				// Limit the Camera zoom distance
				if (msCamera.fCameraDistance > 2000)
				{
					msCamera.fCameraDistance = 2000;
				}
				else if (msCamera.fCameraDistance < 1)
				{
					msCamera.fCameraDistance = 1;
				}
			}
			// Else we are using the Free Camera
			else
			{
				int iSPEED = 200;
				float fROTATE_SPEED = MathHelper.PiOver4;

				if (KeyboardManager.KeyIsDown(Keys.Decimal))
				{
					iSPEED = 100;
				}

				// If the Camera should move forward
				if (KeyboardManager.KeyIsDown(Keys.Up))
				{
					msCamera.MoveCameraForwardOrBackward(iSPEED * fTimeInSeconds);
				}

				// If the Camera should move backwards
				if (KeyboardManager.KeyIsDown(Keys.Down))
				{
					msCamera.MoveCameraForwardOrBackward(-iSPEED * fTimeInSeconds);
				}

				// If the Camera should strafe right
				if (KeyboardManager.KeyIsDown(Keys.Right))
				{
					msCamera.MoveCameraHorizontally(-iSPEED * fTimeInSeconds);
				}

				// If the Camera should strafe left
				if (KeyboardManager.KeyIsDown(Keys.Left))
				{
					msCamera.MoveCameraHorizontally(iSPEED * fTimeInSeconds);
				}

				// If the Camera should move upwards
				if (KeyboardManager.KeyIsDown(Keys.NumPad1) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D1)))
				{
					msCamera.MoveCameraVertically((iSPEED / 2) * fTimeInSeconds);
				}

				// If the Camera should move downwards
				if (KeyboardManager.KeyIsDown(Keys.NumPad0) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D0)))
				{
					msCamera.MoveCameraVertically((-iSPEED / 2) * fTimeInSeconds);
				}

				// If the Camera should yaw left
				if (KeyboardManager.KeyIsDown(Keys.NumPad4) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D4)))
				{
					msCamera.RotateCameraHorizontally(fROTATE_SPEED * fTimeInSeconds);
				}

				// If the Camera should yaw right
				if (KeyboardManager.KeyIsDown(Keys.NumPad6) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D6)))
				{
					msCamera.RotateCameraHorizontally(-fROTATE_SPEED * fTimeInSeconds);
				}

				// If the Camera should pitch up
				if (KeyboardManager.KeyIsDown(Keys.NumPad8) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D8)))
				{
					msCamera.RotateCameraVertically(-fROTATE_SPEED * fTimeInSeconds);
				}

				// If the Camera should pitch down
				if (KeyboardManager.KeyIsDown(Keys.NumPad2) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D2)))
				{
					msCamera.RotateCameraVertically(fROTATE_SPEED * fTimeInSeconds);
				}


				// Calculate how much the Mouse was moved
				int iXMovement = MouseManager.CurrentMouseState.X - MouseManager.PreviousMouseState.X;
				int iYMovement = MouseManager.CurrentMouseState.Y - MouseManager.PreviousMouseState.Y;
				int iZMovement = MouseManager.CurrentMouseState.ScrollWheelValue - MouseManager.PreviousMouseState.ScrollWheelValue;

				const float fMOUSE_MOVEMENT_SPEED = 0.5f;
				const float fMOUSE_ROTATION_SPEED = 0.005f;

				// If the Left Mouse Button is pressed
				if (MouseManager.CurrentMouseState.LeftButton == ButtonState.Pressed)
				{
					// If the Camera should yaw
					if (iXMovement != 0)
					{
						msCamera.RotateCameraHorizontally(-iXMovement * fMOUSE_ROTATION_SPEED);
					}

					// If the Camera should pitch
					if (iYMovement != 0)
					{
						msCamera.RotateCameraVertically(iYMovement * fMOUSE_ROTATION_SPEED);
					}
				}

				// If the Right Mouse Button is pressed
				if (MouseManager.CurrentMouseState.RightButton == ButtonState.Pressed)
				{
					// If the Camera should strafe
					if (iXMovement != 0)
					{
						msCamera.MoveCameraHorizontally(-iXMovement * fMOUSE_MOVEMENT_SPEED);
					}

					// If the Camera should move forward or backward
					if (iYMovement != 0)
					{
						msCamera.MoveCameraForwardOrBackward(-iYMovement * (fMOUSE_MOVEMENT_SPEED * 2.0f));
					}
				}

				// If the Middle Mouse Button is scrolled
				if (iZMovement != 0)
				{
					msCamera.MoveCameraVertically(iZMovement * (fMOUSE_MOVEMENT_SPEED / 10.0f));
				}
			}

			// If the Camera Mode should be switched
			if (KeyboardManager.KeyWasJustPressed(Keys.PageDown))
			{
				msCamera.bUsingFixedCamera = !msCamera.bUsingFixedCamera;
			}

			// If the Camera values should be Reset
			if (KeyboardManager.KeyIsDown(Keys.R) || 
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftShoulder) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightShoulder)))
			{
				msCamera.ResetFreeCameraVariables();
				msCamera.ResetFixedCameraVariables();
			}
		}

		/// <summary>
		/// Function to control the Particle Systems based on user input
		/// </summary>
		public void ProcessInputForParticleSystem(float fElapsedTimeInSeconds)
		{
			// If the Current Particle System should be changed to the next Particle System
			if (KeyboardManager.KeyWasJustPressed(Keys.H) || GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.RightTrigger))
			{
				meCurrentPS++;
				if (meCurrentPS > PSEffects.LastInList)
				{
					meCurrentPS = 0;
				}

				// Initialize the new Particle System
				InitializeCurrentParticleSystem();
			}
			// Else if the Current Particle System should be changed back to the previous Particle System
			else if (KeyboardManager.KeyWasJustPressed(Keys.G) || GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.LeftTrigger))
			{
				meCurrentPS--;
				if (meCurrentPS < 0)
				{
					meCurrentPS = PSEffects.LastInList;
				}

				// Initialize the new Particle System
				InitializeCurrentParticleSystem();
			}

			// If the Current Particle System is not Initialized
			if (_currentDPSFDemoParticleSystemWrapper == null || !_currentDPSFDemoParticleSystemWrapper.IsInitialized)
			{
				return;
			}

			// Define how fast the user can move and rotate the Emitter
			float fEmitterMoveDelta = 75 * fElapsedTimeInSeconds;
			float fEmitterRotateDelta = MathHelper.Pi * fElapsedTimeInSeconds;

			// If the Shift key is down, move faster
			if (KeyboardManager.KeyIsDown(Keys.LeftShift) || KeyboardManager.KeyIsDown(Keys.RightShift))
			{
				fEmitterMoveDelta *= 2;
			}

			// Check if the Emitter should be moved
			if (KeyboardManager.KeyIsDown(Keys.W) || 
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadUp) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				_currentDPSFDemoParticleSystemWrapper.Emitter.PositionData.Position += Vector3.Up * fEmitterMoveDelta;
			}

			if (KeyboardManager.KeyIsDown(Keys.S) || 
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadDown) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				_currentDPSFDemoParticleSystemWrapper.Emitter.PositionData.Position += Vector3.Down * fEmitterMoveDelta;
			}

			if (KeyboardManager.KeyIsDown(Keys.A) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadLeft))
			{
				_currentDPSFDemoParticleSystemWrapper.Emitter.PositionData.Position += Vector3.Left * fEmitterMoveDelta;
			}

			if (KeyboardManager.KeyIsDown(Keys.D) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadRight))
			{
				_currentDPSFDemoParticleSystemWrapper.Emitter.PositionData.Position += Vector3.Right * fEmitterMoveDelta;
			}

			if (KeyboardManager.KeyIsDown(Keys.E) || 
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadUp) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				_currentDPSFDemoParticleSystemWrapper.Emitter.PositionData.Position += Vector3.Forward * fEmitterMoveDelta;
			}

			if (KeyboardManager.KeyIsDown(Keys.Q) ||
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadDown) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				_currentDPSFDemoParticleSystemWrapper.Emitter.PositionData.Position += Vector3.Backward * fEmitterMoveDelta;
			}

			// Check if the Emitter should be rotated
			if ((meCurrentPS != PSEffects.Star && meCurrentPS != PSEffects.Ball) || 
				(!KeyboardManager.KeyIsDown(Keys.V) && !KeyboardManager.KeyIsDown(Keys.B) && !KeyboardManager.KeyIsDown(Keys.P)))
			{
				if (KeyboardManager.KeyIsDown(Keys.J) || 
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickLeft) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
				{
					// If we should Rotate the Emitter around the Pivot Point
					if (KeyboardManager.KeyIsDown(Keys.Y))
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(-fEmitterRotateDelta, 0.0f, 0.0f));
					}
					// Else we should just Rotate the Emitter about its center
					else
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(-fEmitterRotateDelta, 0.0f, 0.0f));
					}
				}

				if (KeyboardManager.KeyIsDown(Keys.L) || 
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickRight) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
				{
					// If we should Rotate the Emitter around the Pivot Point
					if (KeyboardManager.KeyIsDown(Keys.Y))
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(fEmitterRotateDelta, 0.0f, 0.0f));
					}
					// Else we should just Rotate the Emitter about its center
					else
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(fEmitterRotateDelta, 0.0f, 0.0f));
					}
				}

				if (KeyboardManager.KeyIsDown(Keys.I) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickUp))
				{
					// If we should Rotate the Emitter around the Pivot Point
					if (KeyboardManager.KeyIsDown(Keys.Y))
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, -fEmitterRotateDelta, 0.0f));
					}
					// Else we should just Rotate the Emitter about its center
					else
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, -fEmitterRotateDelta, 0.0f));
					}
				}

				if (KeyboardManager.KeyIsDown(Keys.K) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickDown))
				{
					// If we should Rotate the Emitter around the Pivot Point
					if (KeyboardManager.KeyIsDown(Keys.Y))
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, fEmitterRotateDelta, 0.0f));
					}
					// Else we should just Rotate the Emitter about its center
					else
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, fEmitterRotateDelta, 0.0f));
					}
				}

				if (KeyboardManager.KeyIsDown(Keys.U) ||
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickLeft) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
				{
					// If we should Rotate the Emitter around the Pivot Point
					if (KeyboardManager.KeyIsDown(Keys.Y))
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, fEmitterRotateDelta));
					}
					// Else we should just Rotate the Emitter about its center
					else
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, fEmitterRotateDelta));
					}
				}

				if (KeyboardManager.KeyIsDown(Keys.O) ||
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickRight) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
				{
					// If we should Rotate the Emitter around the Pivot Point
					if (KeyboardManager.KeyIsDown(Keys.Y))
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, -fEmitterRotateDelta));
					}
					// Else we should just Rotate the Emitter about its center
					else
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, -fEmitterRotateDelta));
					}
				}
			}

			// Check if the Emitter should be reset
			if (KeyboardManager.KeyWasJustPressed(Keys.Z))
			{
				_currentDPSFDemoParticleSystemWrapper.Emitter.PositionData.Position = Vector3.Zero;
				_currentDPSFDemoParticleSystemWrapper.Emitter.OrientationData.Orientation = Quaternion.Identity;
			}

			// If the Texture should be changed
			if (KeyboardManager.KeyWasJustPressed(Keys.T) || GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.Y))
			{
				if (_currentDPSFDemoParticleSystemWrapper.Texture != null)
				{
					// Get which Texture is currently being used for sure
					for (int i = 0; i < (int)Textures.LastInList + 1; i++)
					{
						Textures eTexture = (Textures)i;
						string sName = eTexture.ToString();

						if (_currentDPSFDemoParticleSystemWrapper.Texture.Name.Equals(sName))
						{
							meCurrentTexture = (Textures)i;
						}
					}

					// If we should go to the previous Texture
					if (KeyboardManager.KeyIsDown(Keys.LeftShift) || KeyboardManager.KeyIsDown(Keys.RightShift))
					{
						meCurrentTexture--;
						if (meCurrentTexture < 0)
						{
							meCurrentTexture = Textures.LastInList;
						}
					}
					// Else we should go to the next Texture
					else
					{
						meCurrentTexture++;
						if (meCurrentTexture > Textures.LastInList)
						{
							meCurrentTexture = 0;
						}
					}

					// Change the Texture being used to draw the Particles
					_currentDPSFDemoParticleSystemWrapper.SetTexture("Textures/" + meCurrentTexture.ToString());
				}
			}

			if (KeyboardManager.KeyWasJustPressed(Keys.Insert))
			{
				// Add a single Particle
				_currentDPSFDemoParticleSystemWrapper.AddParticle();
			}

			if (KeyboardManager.KeyIsDown(Keys.Home))
			{
				// Add Particles while the button is pressed
				_currentDPSFDemoParticleSystemWrapper.AddParticle();
			}

			if (KeyboardManager.KeyIsDown(Keys.PageUp))
			{
				// Add the max number of Particles
				while (_currentDPSFDemoParticleSystemWrapper.AddParticle()) { }
			}

			if (KeyboardManager.KeyWasJustPressed(Keys.Delete) || GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.X))
			{
				// Toggle emitting particles on/off
				_currentDPSFDemoParticleSystemWrapper.Emitter.EmitParticlesAutomatically = !_currentDPSFDemoParticleSystemWrapper.Emitter.EmitParticlesAutomatically;
			}

			if (KeyboardManager.KeyIsDown(Keys.Add, 0.02f) || KeyboardManager.KeyIsDown(Keys.OemPlus, 0.02f) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightShoulder, 0.02f))
			{
				// Increase the number of Particles being emitted
				_currentDPSFDemoParticleSystemWrapper.Emitter.ParticlesPerSecond++;
			}

			if (KeyboardManager.KeyIsDown(Keys.Subtract, 0.02f) || KeyboardManager.KeyIsDown(Keys.OemMinus, 0.02f) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftShoulder, 0.02f))
			{
				if (_currentDPSFDemoParticleSystemWrapper.Emitter.ParticlesPerSecond > 1)
				{
					// Decrease the number of Particles being emitted
					_currentDPSFDemoParticleSystemWrapper.Emitter.ParticlesPerSecond--;
				}
			}

			if (KeyboardManager.KeyWasJustPressed(Keys.Multiply) || 
				((KeyboardManager.KeyIsDown(Keys.LeftShift) || KeyboardManager.KeyIsDown(Keys.RightShift)) && KeyboardManager.KeyWasJustPressed(Keys.D8)) || 
				GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.B))
			{
				// Increase the Speed of the Particle System simulation
				_particleSystemManager.SimulationSpeed += 0.1f;

				if (_particleSystemManager.SimulationSpeed > 5.0f)
				{
					_particleSystemManager.SimulationSpeed = 5.0f;
				}

				// If DPSF is not inheriting from DrawableGameComponent then we need
				// to set the individual particle system's Simulation Speeds
				if (DPSFHelper.DPSFInheritsDrawableGameComponent)
				{
					_particleSystemManager.SetSimulationSpeedForAllParticleSystems(_particleSystemManager.SimulationSpeed);
				}
			}

			if (KeyboardManager.KeyWasJustPressed(Keys.Divide) ||
				(KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyWasJustPressed(Keys.OemQuestion)) || 
				GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.A))
			{
				// Decrease the Speed of the Particle System simulation
				_particleSystemManager.SimulationSpeed -= 0.1f;

				if (_particleSystemManager.SimulationSpeed < 0.1f)
				{
					_particleSystemManager.SimulationSpeed = 0.1f;
				}

				// If DPSF is not inheriting from DrawableGameComponent then we need
				// to set the individual particle system's Simulation Speeds
				if (DPSFHelper.DPSFInheritsDrawableGameComponent)
				{
					_particleSystemManager.SetSimulationSpeedForAllParticleSystems(_particleSystemManager.SimulationSpeed);
				}
			}

            // Perform particle system-specific input processing.
		    _currentDPSFDemoParticleSystemWrapper.ProcessInput();

			// Perform any particle system-specific input processing that affects objects external to the particle system.
			switch (meCurrentPS)
			{
				case PSEffects.Smoke:
					if (KeyboardManager.KeyWasJustPressed(Keys.V))
					{
						SmokeParticleSystem smokeParticleSystem = _currentDPSFDemoParticleSystemWrapper as SmokeParticleSystem;

						// Setup the sphere to pass by the particle system.
						mcSphere.bVisible = true;
						mcSphere.sPosition = smokeParticleSystem.mcExternalObjectPosition = new Vector3(-125, 50, 0);
						mcSphere.sVelocity = new Vector3(50, 0, 0);
						mcSphere.cTimeAliveInSeconds = TimeSpan.Zero;

						// Setup the particle system to be affected by the sphere.
						smokeParticleSystem.mfAttractRepelRange = mcSphere.fSize * 2;
						smokeParticleSystem.mfAttractRepelForce = 3.0f;
						smokeParticleSystem.MakeParticlesAttractToExternalObject();
					}

					if (KeyboardManager.KeyWasJustPressed(Keys.B))
					{
						SmokeParticleSystem smokeParticleSystem = _currentDPSFDemoParticleSystemWrapper as SmokeParticleSystem;

						// Setup the sphere to pass by the particle system.
						mcSphere.bVisible = true;
						mcSphere.sPosition = smokeParticleSystem.mcExternalObjectPosition = new Vector3(-125, 50, 0);
						mcSphere.sVelocity = new Vector3(50, 0, 0);
						mcSphere.cTimeAliveInSeconds = TimeSpan.Zero;

						// Setup the particle system to be affected by the sphere.
						smokeParticleSystem.mfAttractRepelRange = mcSphere.fSize * 2f;
						smokeParticleSystem.mfAttractRepelForce = 0.5f;
						smokeParticleSystem.MakeParticlesRepelFromExternalObject();
					}
					break;
			}
		}

		#endregion
	}

	#region Application Entry Point

	/// <summary>
	/// The main entry point for the application
	/// </summary>
	static class Program
	{
		static void Main()
		{
			using (DemoBase Game = new DemoBase())
			{
#if (WINDOWS)
				// String to hold any prerequisites error messages
				string prerequisitesErrorMessage = string.Empty;

				// If XNA 4.0 is not installed
				using (Microsoft.Win32.RegistryKey xnaKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\XNA\\Framework\\v4.0"))
				{
					if (xnaKey == null || (int)xnaKey.GetValue("Installed") != 1)
					{
						// Store the error message
						prerequisitesErrorMessage += "XNA 4.0 must be installed to run this program. You can download the XNA 4.0 Redistributable from http://www.microsoft.com/downloads/ \n\n";
					}
				}

				// If .NET 4 is not installed
				using (Microsoft.Win32.RegistryKey netKey4 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Client"))
				{
					bool net4NotInstalled = (netKey4 == null || (int)netKey4.GetValue("Install") != 1);
					if (net4NotInstalled)
					{
						// Store the error message
						prerequisitesErrorMessage += "The .NET Framework 4.0 or greater must be installed to run this program. You can download the .NET Framework from http://www.microsoft.com/downloads/ \n\n";
					}
				}

				// If not all of the prerequisites are installed
				if (!string.IsNullOrEmpty(prerequisitesErrorMessage))
				{
					// Add to the error message the option of trying to run the program anyways
					prerequisitesErrorMessage += "Do you want to try and run the program anyways, even though not all of the prerequisites are installed?";

					// Display the error message and exit
					if (System.Windows.Forms.MessageBox.Show(prerequisitesErrorMessage, "Prerequisites Not Installed", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
					{
						return;
					}
				}

				// If we are in Release Mode
				if (DemoBase.RELEASE_MODE)
				{
					try
					{
						Game.Run();     // Start the game
					}
					catch (Exception e)
					{
						// Display any error messages that occur
						System.Windows.Forms.MessageBox.Show(e.ToString(), "Unhandled Exception");
					}
				}
				// Else we are in debug mode, so allow Visual Studio to show us the error message
				else
				{
					Game.Run();
				}
#else
				Game.Run();
#endif
			}
		}
	}

	#endregion
}
