using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DPSF;
using DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo;
using DPSF_Demo.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF_Demo
{
	/// <summary>
	/// Class to show off many particle systems built using the Dynamic Particle System Framework.
	/// </summary>
	public class DPSFDemo : DemoBase
	{
		#region Fields

		//===========================================================
		// Global Constants - User Settings
		//===========================================================

		// How often the Particle Systems should be updated (zero = update as often as possible)
		const int PARTICLE_SYSTEM_UPDATES_PER_SECOND = 0;

		// Static Particle Settings
		float _staticParticleTimeStep = 1.0f / 30.0f;	// The Time Step between the drawing of each frame of the Static Particles (1 / # of fps, example, 1 / 30 = 30fps).
		float _staticParticleTotalTime = 3.0f;			// The number of seconds that the Static Particle should be drawn over.

		// "Draw To Files" Settings
		float _drawPSToFilesTimeStep = 1.0f / 10.0f;
		float _drawPSToFilesTotalTime = 2.0f;
		int _drawPSToFilesImageWidth = 200;
		int _drawPSToFilesImageHeight = 150;
		string _drawPSToFilesDirectoryName = "AnimationFrames";
		bool _createAnimatedGIF = true;
		bool _createTileSetImage = true;

		string _serializedParticleSystemFileName = "SerializedParticleSystem.dat"; // The name of the file to serialize the particle system to.


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
		enum ParticleSystemEffects
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

		// Initialize which Particle System to show first.
		ParticleSystemEffects _currentParticleSystem = ParticleSystemEffects.Random;

		// Initialize the Texture to use.
		Textures _currentTexture = Textures.Bubble;

		// Initialize the Sphere Object.
		Model _sphereModel;
		SimpleObject _sphereObject = new SimpleObject();

		bool _showCommonControls = false;      // Tells if the Common Controls should be shown or not
		bool _showParticleSystemControls = false;          // Tells if the Particle System specific Controls should be shown or not

		protected override void ShowPerformanceTextToggled(bool enabled)
		{
			// Only enable the Performance Profiling if we are going to be displaying it.

			// Set it on the Defaults so it is enabled/disabled whenever we initialize a new particle system.
			DPSFDefaultSettings.PerformanceProfilingIsEnabled = enabled;

			// Set it on the Manager to enable/disable it for the particle system that is currently running.
			_particleSystemManager.SetPerformanceProfilingIsEnabledForAllParticleSystems(enabled);
		}

		// Draw Static Particle variables (2 of the variables are up in the User Settings)
		bool _drawStaticParticles = false;	// Tells if Static Particles should be drawn or not.
		bool _staticParticlesDrawn = false;	// Tells if the Static Particles have already been drawn or not.

		// Declare the Particle System Manager to manage the Particle Systems.
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
		/// Initializes a new instance of the <see cref="DPSFDemo"/> class.
		/// </summary>
		public DPSFDemo()
		{
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			_sphereModel = Content.Load<Model>("SphereHighPoly");

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
			ShowText = false;
			ShowFloor = false;

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
			ShowText = true;
			ShowFloor = true;

			// Start displaying the demo's particle systems
			_currentDPSFDemoParticleSystemWrapper = null;
			InitializeCurrentParticleSystem();
		}

		/// <summary>
		/// Initializes whatever particle system should be displayed.
		/// </summary>
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
			switch (_currentParticleSystem)
			{
				default:
				case ParticleSystemEffects.Random: _currentDPSFDemoParticleSystemWrapper = _mcRandomDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Fire: _currentDPSFDemoParticleSystemWrapper = _mcFireDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.FireSprite: _currentDPSFDemoParticleSystemWrapper = _mcFireSpriteDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Smoke: _currentDPSFDemoParticleSystemWrapper = _mcSmokeDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Snow: _currentDPSFDemoParticleSystemWrapper = _mcSnowDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.SquarePattern: _currentDPSFDemoParticleSystemWrapper = _mcSquarePatternDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Fountain: _currentDPSFDemoParticleSystemWrapper = _mcFountainDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Random2D: _currentDPSFDemoParticleSystemWrapper = _mcRandom2DdpsfDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.GasFall: _currentDPSFDemoParticleSystemWrapper = _mcGasFallDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Dot: _currentDPSFDemoParticleSystemWrapper = _mcDotDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Fireworks: _currentDPSFDemoParticleSystemWrapper = _mcFireworksDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Figure8: _currentDPSFDemoParticleSystemWrapper = _mcFigure8DPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Star: _currentDPSFDemoParticleSystemWrapper = _mcStarDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Ball: _currentDPSFDemoParticleSystemWrapper = _mcBallDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.RotatingQuad: _currentDPSFDemoParticleSystemWrapper = _mcRotatingQuadDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Box: _currentDPSFDemoParticleSystemWrapper = _mcBoxDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Image: _currentDPSFDemoParticleSystemWrapper = _mcImageDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.AnimatedTexturedQuad: _currentDPSFDemoParticleSystemWrapper = _mcAnimatedQuadDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Sprite: _currentDPSFDemoParticleSystemWrapper = _mcSpriteDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.AnimatedSprite: _currentDPSFDemoParticleSystemWrapper = _mcAnimatedSpriteDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.QuadSpray: _currentDPSFDemoParticleSystemWrapper = _mcQuadSprayDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Magnets: _currentDPSFDemoParticleSystemWrapper = _mcMagnetDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Sparkler: _currentDPSFDemoParticleSystemWrapper = _mcSparklerDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.GridQuad: _currentDPSFDemoParticleSystemWrapper = _mcGridQuadDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Sphere: _currentDPSFDemoParticleSystemWrapper = _mcSphereDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.MultipleParticleImages: _currentDPSFDemoParticleSystemWrapper = _mcMultipleDPSFDemoImagesDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.MultipleParticleImagesSprite: _currentDPSFDemoParticleSystemWrapper = _mcMultipleDPSFDemoImagesSpriteDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.ExplosionFireSmoke: _currentDPSFDemoParticleSystemWrapper = _mcExplosionFireSmokeDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.ExplosionFlash: _currentDPSFDemoParticleSystemWrapper = _mcExplosionFlashDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.ExplosionFlyingSparks: _currentDPSFDemoParticleSystemWrapper = _mcExplosionFlyingSparksDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.ExplosionSmokeTrails: _currentDPSFDemoParticleSystemWrapper = _mcExplosionSmokeTrailsDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.ExplosionRoundSparks: _currentDPSFDemoParticleSystemWrapper = _mcExplosionRoundSparksDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.ExplosionDebris: _currentDPSFDemoParticleSystemWrapper = _mcExplosionDebrisDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.ExplosionDebrisSprite: _currentDPSFDemoParticleSystemWrapper = _mcExplosionDebrisSpriteDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.ExplosionShockwave: _currentDPSFDemoParticleSystemWrapper = _mcExplosionShockwaveDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Explosion: _currentDPSFDemoParticleSystemWrapper = _mcExplosionDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.Trail: _currentDPSFDemoParticleSystemWrapper = _mcTrailDPSFDemoParticleSystemWrapper; break;
				case ParticleSystemEffects.SpriteParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcSpriteDPSFDemoParticleSystemTemplateWrapper; break;
				case ParticleSystemEffects.Sprite3DBillboardParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcSprite3DBillboardDPSFDemoParticleSystemTemplateWrapper; break;
				case ParticleSystemEffects.QuadParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcQuadDPSFDemoParticleSystemTemplateWrapper; break;
				case ParticleSystemEffects.TexturedQuadParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcTexturedQuadDPSFDemoParticleSystemTemplateWrapper; break;
				case ParticleSystemEffects.DefaultSpriteParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcDefaultSpriteDPSFDemoParticleSystemTemplateWrapper; break;
				case ParticleSystemEffects.DefaultSprite3DBillboardParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcDefaultSprite3DBillboardDPSFDemoParticleSystemTemplateWrapper; break;
				case ParticleSystemEffects.DefaultQuadParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcDefaultQuadDPSFDemoParticleSystemTemplateWrapper; break;
				case ParticleSystemEffects.DefaultTexturedQuadParticleSystemTemplate: _currentDPSFDemoParticleSystemWrapper = _mcDefaultTexturedQuadDPSFDemoParticleSystemTemplateWrapper; break;
			}

			// Initialize the Particle System
			_currentDPSFDemoParticleSystemWrapper.AutoInitialize(this.GraphicsDevice, this.Content, null);

			// Do any necessary after initialization work 
			_currentDPSFDemoParticleSystemWrapper.AfterAutoInitialize();
		}

		#endregion

		#region Update and Draw



		#endregion
	}
}
