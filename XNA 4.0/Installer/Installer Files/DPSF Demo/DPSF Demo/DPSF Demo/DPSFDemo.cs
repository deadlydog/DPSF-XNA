#region File Description
//===================================================================
// DPSFDemo.cs
//
// This class is used to test and demo the Dynamic Particle System Framework.
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using BasicVirtualEnvironment;
using BasicVirtualEnvironment.Input;
using DPSF;
using DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace DPSF_Demo
{
	/// <summary>
	/// Class to show off many particle systems built using the Dynamic Particle System Framework.
	/// </summary>
	public class DPSFDemo : BasicVirtualEnvironmentBase
	{
		#region Fields

		//===========================================================
		// Global Constants - User Settings
		//===========================================================

		// How often the Particle Systems should be updated (zero = update as often as possible)
		const int PARTICLE_SYSTEM_UPDATES_PER_SECOND = 60;

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
			MultipleEmitters,
			SpriteParticleSystemTemplate,
			Sprite3DBillboardParticleSystemTemplate,
			QuadParticleSystemTemplate,
			TexturedQuadParticleSystemTemplate,
			DefaultSpriteParticleSystemTemplate,
			DefaultSprite3DBillboardParticleSystemTemplate,
			DefaultQuadParticleSystemTemplate,
			DefaultTexturedQuadParticleSystemTemplate,
			LastInList = 45     // This value should be the number of Effects in the enumeration minus one (since we start at index 0) (excluding the Splash Screen)
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

		protected override string LastToggleTextFunctionKey { get { return "F4"; } }

		bool _showParticleSystemControls = true;          // Tells if the Particle System specific Controls should be shown or not (on by default).

		/// <summary>
		/// Called whenever the value of ShowPerformanceText is changed.
		/// </summary>
		/// <param name="enabled">The new value of ShowPerformanceText.</param>
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
		MultipleEmittersParticleSystemWrapper _mcMultipleEmittersParticleSystemWrapper = null;
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
		{ }

		protected override void LoadContent()
		{
			base.LoadContent();

			_sphereModel = Content.Load<Model>("Models/SphereHighPoly");

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
			_mcMultipleEmittersParticleSystemWrapper = new MultipleEmittersParticleSystemWrapper(this);
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
			_particleSystemManager.AddParticleSystem(_mcMultipleEmittersParticleSystemWrapper);
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
		private void mcDPSFSplashScreenParticleSystem_SplashScreenComplete(object sender, EventArgs e)
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
				case ParticleSystemEffects.MultipleEmitters: _currentDPSFDemoParticleSystemWrapper = _mcMultipleEmittersParticleSystemWrapper; break;
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

		/// <summary>
		/// Perform any game logic.
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		protected override void UpdateGame(GameTime gameTime)
		{
			base.UpdateGame(gameTime);

			// Update the Quad Particle Systems to know where the Camera is so that they can display
			// the particles as billboards if needed (i.e. have particle always face the camera).
			_particleSystemManager.SetCameraPositionForAllParticleSystems(Camera.Position);

			// Set the World, View, and Projection Matrices for the Particle Systems.
			_particleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(WorldMatrix, ViewMatrix, ProjectionMatrix);

			// If the Game is Paused.
			if (Paused)
			{
				// Update the particle systems with 0 elapsed time, just to allow the particles to rotate to face the camera.
				_particleSystemManager.UpdateAllParticleSystems(0);

				// Exit without updating anything.
				return;
			}

			// If the Current Particle System is Initialized.
			if (_currentDPSFDemoParticleSystemWrapper != null && _currentDPSFDemoParticleSystemWrapper.IsInitialized)
			{
				// If Static Particles should be drawn (i.e. draw an interval of time in a single frame).
				if (_drawStaticParticles)
				{
					// If the Static Particles haven't been drawn yet.
					if (!_staticParticlesDrawn)
					{
						// Draw this frame to a Render Target so we can have it persist across frames.
						SetupToDrawToRenderTarget(true);

						// Update the Particle System iteratively by the Time Step amount until the 
						// Particle System behavior over the Total Time has been drawn.
						float fElapsedTime = 0;
						while (fElapsedTime < _staticParticleTotalTime)
						{
							// Update and draw this frame of the Particle System.
							_particleSystemManager.UpdateAllParticleSystems(_staticParticleTimeStep);
							_particleSystemManager.DrawAllParticleSystems();
							fElapsedTime += _staticParticleTimeStep;
						}
						_staticParticlesDrawn = true;

						SpriteBatch.Begin();
						SpriteBatch.DrawString(Font, "F6 to continue", new Vector2(310, 25), Color.LawnGreen);
						SpriteBatch.End();
					}
				}
				// Else the Particle Systems should be drawn normally.
				else
				{
					// Update all Particle Systems manually.
					_particleSystemManager.UpdateAllParticleSystems((float)gameTime.ElapsedGameTime.TotalSeconds);
				}

				// If the Sphere is Visible and we are on the Smoke Particle System.
				if (_sphereObject.bVisible && _currentParticleSystem == ParticleSystemEffects.Smoke)
				{
					// Update it
					_sphereObject.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

					// Update the PS's External Object Position to the Sphere's Position.
					_mcSmokeDPSFDemoParticleSystemWrapper.mcExternalObjectPosition = _sphereObject.sPosition;

					// If the Sphere has been alive long enough.
					if (_sphereObject.cTimeAliveInSeconds > TimeSpan.FromSeconds(6.0f))
					{
						_sphereObject.bVisible = false;
						_mcSmokeDPSFDemoParticleSystemWrapper.StopParticleAttractionAndRepulsionToExternalObject();
					}
				}
			}
		}

		/// <summary>
		/// Called before anything is drawn to the screen.
		/// <para>NOTE: If this returns false the Draw() function will exit immediately without drawing anything.</para>
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		protected override bool BeforeDrawGame(GameTime gameTime)
		{
			base.BeforeDrawGame(gameTime);

			// If Static Particles were drawn to the Render Target, draw the Render Target to the screen and signal to exit without drawing anything else.
			if (_drawStaticParticles)
			{
				DrawRenderTargetToScreen();
				return false;
			}
			return true;
		}

		/// <summary>
		/// Draw any game components to the screen.
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		protected override void DrawGame(GameTime gameTime)
		{
			base.DrawGame(gameTime);

			// Draw the Particle Systems manually.
			_particleSystemManager.DrawAllParticleSystems();
		}

		/// <summary>
		/// Draws any game text to the screen.
		/// <para>NOTE: SpriteBatch.Begin() has already been called, so all you need to do is call SpriteBatch.DrawString().</para>
		/// </summary>
		protected override void DrawGameText()
		{
			base.DrawGameText();

			// If we don't have a handle to a particle system, it is because we serialized it.
			if (_currentDPSFDemoParticleSystemWrapper == null)
			{
				SpriteBatch.DrawString(Font, "Particle system has been serialized to the file: " + _serializedParticleSystemFileName + ".", new Vector2(25, 200), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "To deserialize the particle system from the file, restoring the instance of", new Vector2(25, 225), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "the particle system,", new Vector2(25, 250), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "press F9", new Vector2(210, 250), CONTROL_TEXT_COLOR);
				return;
			}

			// If the Particle System has been destroyed, just write that to the screen and exit.
			if (!_currentDPSFDemoParticleSystemWrapper.IsInitialized)
			{
				SpriteBatch.DrawString(Font, "The current particle system has been destroyed.", new Vector2(140, 200), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "Press G / H to switch to a different particle system.", new Vector2(125, 225), PROPERTY_TEXT_COlOR);
				return;
			}

			// Get area on screen that it is safe to draw text to (so that we are sure it will be displayed on the screen).
			Rectangle textSafeArea = GetTextSafeArea();

			// Setup the information that a particle system wrapper would need to draw text with.
			var toolsToDrawText = new DrawTextRequirements()
			{
				TextWriter = SpriteBatch,
				Font = Font,
				TextSafeArea = textSafeArea,
				ControlTextColor = CONTROL_TEXT_COLOR,
				PropertyTextColor = PROPERTY_TEXT_COlOR,
				ValueTextColor = VALUE_TEXT_COLOR
			};

			// Get the Name of the current Particle System and how many Particles are currently Active.
			string sEffectName = _currentDPSFDemoParticleSystemWrapper.Name;
			int iNumberOfActiveParticles = _currentDPSFDemoParticleSystemWrapper.TotalNumberOfActiveParticles;
			int iNumberOfParticlesAllocatedInMemory = _currentDPSFDemoParticleSystemWrapper.TotalNumberOfParticlesAllocatedInMemory;

			// Convert values to strings.
			string sTotalParticleCountValue = iNumberOfActiveParticles.ToString();
			string sEmitterOnValue = (_currentDPSFDemoParticleSystemWrapper.Emitter.EmitParticlesAutomatically ? "On" : "Off");
			string sParticleSystemEffectValue = sEffectName;
			string sParticlesPerSecondValue = _currentDPSFDemoParticleSystemWrapper.Emitter.ParticlesPerSecond.ToString("0.00");
			string sParticleSystemSpeedScale = _particleSystemManager.SimulationSpeed.ToString("0.0");
			string sAllocatedParticles = iNumberOfParticlesAllocatedInMemory.ToString();
			string sTexture = "N/A";
			if (_currentDPSFDemoParticleSystemWrapper.Texture != null)
			{
				sTexture = _currentDPSFDemoParticleSystemWrapper.Texture.Name.TrimStart("Textures/".ToCharArray());
			}


			SpriteBatch.DrawString(Font, "Allocated:", new Vector2(textSafeArea.Left + 120, textSafeArea.Bottom - 50), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sAllocatedParticles, new Vector2(textSafeArea.Left + 210, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Texture:", new Vector2(textSafeArea.Left + 440, textSafeArea.Bottom - 50), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sTexture, new Vector2(textSafeArea.Left + 520, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Speed:", new Vector2(textSafeArea.Right - 100, textSafeArea.Bottom - 50), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sParticleSystemSpeedScale, new Vector2(textSafeArea.Right - 35, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Particles:", new Vector2(textSafeArea.Left + 120, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sTotalParticleCountValue, new Vector2(textSafeArea.Left + 205, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Emitter:", new Vector2(textSafeArea.Left + 275, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sEmitterOnValue, new Vector2(textSafeArea.Left + 345, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Particles Per Second:", new Vector2(textSafeArea.Left + 390, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sParticlesPerSecondValue, new Vector2(textSafeArea.Left + 585, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Effect:", new Vector2(textSafeArea.Left + 5, textSafeArea.Top + 2), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sParticleSystemEffectValue, new Vector2(textSafeArea.Left + 70, textSafeArea.Top + 2), VALUE_TEXT_COLOR);

			// Display particle system specific values.
			_currentDPSFDemoParticleSystemWrapper.DrawStatusText(toolsToDrawText);

			// If the Particle System specific Controls should be shown, display them.
			if (_showParticleSystemControls)
			{
				_currentDPSFDemoParticleSystemWrapper.DrawInputControlsText(toolsToDrawText);
			}

			// If the Common Controls should be shown, display them.
			if (ShowCommonControls)
			{
				SpriteBatch.DrawString(Font, "Change Particle System:", new Vector2(5, 25), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "G / H", new Vector2(235, 25), CONTROL_TEXT_COLOR);

				SpriteBatch.DrawString(Font, "Toggle Emitter On/Off:", new Vector2(5, 50), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "Delete", new Vector2(220, 50), CONTROL_TEXT_COLOR);

				SpriteBatch.DrawString(Font, "Increase/Decrease Emitter Speed:", new Vector2(5, 75), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "+ / -", new Vector2(320, 75), CONTROL_TEXT_COLOR);

				SpriteBatch.DrawString(Font, "Add Particle:", new Vector2(5, 100), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "Insert(one), Home(many), PgUp(max)", new Vector2(130, 100), CONTROL_TEXT_COLOR);

				SpriteBatch.DrawString(Font, "Move Emitter:", new Vector2(5, 125), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "A/D, W/S, Q/E", new Vector2(135, 125), CONTROL_TEXT_COLOR);

				SpriteBatch.DrawString(Font, "Rotate Emitter:", new Vector2(5, 150), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "J/L(yaw), I/Vertex(pitch), U/O(roll)", new Vector2(150, 150), CONTROL_TEXT_COLOR);

				SpriteBatch.DrawString(Font, "Rotate Emitter Around Pivot:", new Vector2(5, 175), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "Y + Rotate Emitter", new Vector2(275, 175), CONTROL_TEXT_COLOR);

				SpriteBatch.DrawString(Font, "Reset Emitter's Position and Orientation:", new Vector2(5, 200), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "Z", new Vector2(375, 200), CONTROL_TEXT_COLOR);

				SpriteBatch.DrawString(Font, "Change Texture:", new Vector2(485, 200), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "T / Shift + T", new Vector2(640, 200), CONTROL_TEXT_COLOR);
// Rearrange these to put them into an order that makes better sense
				SpriteBatch.DrawString(Font, "Speed Up/Down PS:", new Vector2(485, 225), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "* / /", new Vector2(680, 225), CONTROL_TEXT_COLOR);

				SpriteBatch.DrawString(Font, "Draw Static Particles:", new Vector2(485, 250), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "F7", new Vector2(690, 250), CONTROL_TEXT_COLOR);

				SpriteBatch.DrawString(Font, "Create Animation Images:", new Vector2(485, 275), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "F8", new Vector2(725, 275), CONTROL_TEXT_COLOR);

				SpriteBatch.DrawString(Font, "Serialize Particle System:", new Vector2(485, 300), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "F9", new Vector2(725, 300), CONTROL_TEXT_COLOR);
			}

			// If performance related text should be drawn, display how long updates and draws of the current particle system take.
			if (ShowPerformanceText)
			{
				SpriteBatch.DrawString(Font, "Update Time (ms): " + _particleSystemManager.TotalPerformanceTimeToDoUpdatesInMilliseconds.ToString("0.000"), new Vector2(529, GraphicsDeviceManager.PreferredBackBufferHeight - 250), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "Draw Time (ms): " + _particleSystemManager.TotalPerformanceTimeToDoDrawsInMilliseconds.ToString("0.000"), new Vector2(545, GraphicsDeviceManager.PreferredBackBufferHeight - 225), PROPERTY_TEXT_COlOR);
			}
		}

		/// <summary>
		/// Draw any models to the screen.
		/// </summary>
		/// <param name="worldMatrix">The world matrix.</param>
		/// <param name="viewMatrix">The view matrix.</param>
		/// <param name="projectionMatrix">The projection matrix.</param>
		protected override void DrawModels(Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
		{
			base.DrawModels(worldMatrix, viewMatrix, projectionMatrix);

			// If the Sphere should be visible
			if (_sphereObject.bVisible)
			{
				_sphereModel.Draw(Matrix.CreateScale(_sphereObject.fSize) * Matrix.CreateTranslation(_sphereObject.sPosition), viewMatrix, projectionMatrix);
			}
		}

		#endregion

		#region Handle Input

		/// <summary>
		/// Process any input for game events.
		/// <para>This is called after the new input device states have been obtained, but before any other input is processed.</para>
		/// <para>NOTE: If this returns false any other input will not be processed.</para>
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		protected override bool ProcessInputForGame(GameTime gameTime)
		{
			base.ProcessInputForGame(gameTime);

			// If we should toggle showing the Particle System specific Controls.
			if (KeyboardManager.KeyWasJustPressed(Keys.F4))
			{
				_showParticleSystemControls = !_showParticleSystemControls;
			}

			// If the particle lifetimes should be drawn in one frame.
			if (KeyboardManager.KeyWasJustPressed(Keys.F7))
			{
				_drawStaticParticles = !_drawStaticParticles;
				_staticParticlesDrawn = false;
			}

#if (WINDOWS)
			// If the particle system should be drawn to files.
			if (KeyboardManager.KeyWasJustPressed(Keys.F8))
			{
				// Draw the Particle System Animation to a series of Image Files.
				_particleSystemManager.DrawAllParticleSystemsAnimationToFiles(GraphicsDevice, _drawPSToFilesImageWidth, _drawPSToFilesImageHeight,
				_drawPSToFilesDirectoryName, _drawPSToFilesTotalTime, _drawPSToFilesTimeStep, _createAnimatedGIF, _createTileSetImage);
			}

			// If the particle system should be serialized to a file.
			if (KeyboardManager.KeyWasJustPressed(Keys.F9))
			{
				// Only particle systems that do not inherit the DrawableGameComponent can be serialized.
				if (!DPSFHelper.DPSFInheritsDrawableGameComponent)
				{
					// If we have the particle system right now.
					if (_currentDPSFDemoParticleSystemWrapper != null)
					{
						// Serialize the particle system into a file.
						System.IO.Stream stream = System.IO.File.Open("SerializedParticleSystem.dat", System.IO.FileMode.Create);
						System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
						formatter.Serialize(stream, _currentDPSFDemoParticleSystemWrapper);
						stream.Close();

						// Remove the particle system from the manager and destroy the particle system in memory.
						_particleSystemManager.RemoveParticleSystem(_currentDPSFDemoParticleSystemWrapper);
						_currentDPSFDemoParticleSystemWrapper.Destroy();
						_currentDPSFDemoParticleSystemWrapper = null;
					}
					// Else we don't have the particle system right now.
					else
					{
						// Deserialize the particle system from a file.
						System.IO.Stream stream = System.IO.File.Open("SerializedParticleSystem.dat", System.IO.FileMode.Open);
						System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
						_currentDPSFDemoParticleSystemWrapper = (IWrapDPSFDemoParticleSystems)formatter.Deserialize(stream);
						stream.Close();

						try
						{
							// Setup the particle system properties that couldn't be serialized.
							_currentDPSFDemoParticleSystemWrapper.InitializeNonSerializableProperties(this, this.GraphicsDevice, this.Content);
						}
						// Catch the case where the Particle System requires a texture, but one wasn't loaded.
						catch (DPSF.Exceptions.DPSFArgumentNullException)
						{
							// Assign the particle system a texture to use.
							_currentDPSFDemoParticleSystemWrapper.SetTexture("Textures/Bubble");
						}

						// Re-add the particle system to the particle system manager.
						_particleSystemManager.AddParticleSystem(_currentDPSFDemoParticleSystemWrapper);
					}
				}
			}
#endif
			// Handle input for controlling the Particle Systems.
			return ProcessInputForParticleSystem(gameTime);
		}

		/// <summary>
		/// Function to control the Particle Systems based on user input.
		/// Returns false if no other input should be processed this frame.
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		private bool ProcessInputForParticleSystem(GameTime gameTime)
		{
			// Save how long it's been since the last time Input was Handled.
			float elapsedTimeInSecondsSinceLastUpdate = (float)gameTime.ElapsedGameTime.TotalSeconds;

			// If we are currently showing the Splash Screen and a key was pressed, skip the Splash Screen.
			if (_mcDPSFSplashScreenDPSFDemoParticleSystemWrapper != null &&
				((KeyboardManager.CurrentKeyboardState.GetPressedKeys().Length > 0 && KeyboardManager.CurrentKeyboardState.GetPressedKeys()[0] != Keys.None) ||
				(MouseManager.CurrentMouseState.LeftButton == ButtonState.Pressed || MouseManager.CurrentMouseState.RightButton == ButtonState.Pressed) ||
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.A | Buttons.B | Buttons.X | Buttons.Y | Buttons.Start))))
			{
				_mcDPSFSplashScreenDPSFDemoParticleSystemWrapper.IsSplashScreenComplete = true;
				return false;
			}

			// If the Current Particle System should be changed to the next Particle System
			if (KeyboardManager.KeyWasJustPressed(Keys.H) || GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.RightTrigger))
			{
				_currentParticleSystem++;
				if (_currentParticleSystem > ParticleSystemEffects.LastInList)
				{
					_currentParticleSystem = 0;
				}

				// Initialize the new Particle System
				InitializeCurrentParticleSystem();
			}
			// Else if the Current Particle System should be changed back to the previous Particle System
			else if (KeyboardManager.KeyWasJustPressed(Keys.G) || GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.LeftTrigger))
			{
				_currentParticleSystem--;
				if (_currentParticleSystem < 0)
				{
					_currentParticleSystem = ParticleSystemEffects.LastInList;
				}

				// Initialize the new Particle System
				InitializeCurrentParticleSystem();
			}

			// If the Current Particle System is not Initialized
			if (_currentDPSFDemoParticleSystemWrapper == null || !_currentDPSFDemoParticleSystemWrapper.IsInitialized)
			{
				return true;
			}

			// Define how fast the user can move and rotate the Emitter
			float fEmitterMoveDelta = 75 * elapsedTimeInSecondsSinceLastUpdate;
			float fEmitterRotateDelta = MathHelper.Pi * elapsedTimeInSecondsSinceLastUpdate;

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
			if ((_currentParticleSystem != ParticleSystemEffects.Star && _currentParticleSystem != ParticleSystemEffects.Ball) ||
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
							_currentTexture = (Textures)i;
						}
					}

					// If we should go to the previous Texture
					if (KeyboardManager.KeyIsDown(Keys.LeftShift) || KeyboardManager.KeyIsDown(Keys.RightShift))
					{
						_currentTexture--;
						if (_currentTexture < 0)
						{
							_currentTexture = Textures.LastInList;
						}
					}
					// Else we should go to the next Texture
					else
					{
						_currentTexture++;
						if (_currentTexture > Textures.LastInList)
						{
							_currentTexture = 0;
						}
					}

					// Change the Texture being used to draw the Particles
					_currentDPSFDemoParticleSystemWrapper.SetTexture("Textures/" + _currentTexture.ToString());
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
			switch (_currentParticleSystem)
			{
				case ParticleSystemEffects.Smoke:
					if (KeyboardManager.KeyWasJustPressed(Keys.V))
					{
						SmokeParticleSystem smokeParticleSystem = _currentDPSFDemoParticleSystemWrapper as SmokeParticleSystem;

						// Setup the sphere to pass by the particle system.
						_sphereObject.bVisible = true;
						_sphereObject.sPosition = smokeParticleSystem.mcExternalObjectPosition = new Vector3(-125, 50, 0);
						_sphereObject.sVelocity = new Vector3(50, 0, 0);
						_sphereObject.cTimeAliveInSeconds = TimeSpan.Zero;

						// Setup the particle system to be affected by the sphere.
						smokeParticleSystem.mfAttractRepelRange = _sphereObject.fSize * 2;
						smokeParticleSystem.mfAttractRepelForce = 3.0f;
						smokeParticleSystem.MakeParticlesAttractToExternalObject();
					}

					if (KeyboardManager.KeyWasJustPressed(Keys.B))
					{
						SmokeParticleSystem smokeParticleSystem = _currentDPSFDemoParticleSystemWrapper as SmokeParticleSystem;

						// Setup the sphere to pass by the particle system.
						_sphereObject.bVisible = true;
						_sphereObject.sPosition = smokeParticleSystem.mcExternalObjectPosition = new Vector3(-125, 50, 0);
						_sphereObject.sVelocity = new Vector3(50, 0, 0);
						_sphereObject.cTimeAliveInSeconds = TimeSpan.Zero;

						// Setup the particle system to be affected by the sphere.
						smokeParticleSystem.mfAttractRepelRange = _sphereObject.fSize * 2f;
						smokeParticleSystem.mfAttractRepelForce = 0.5f;
						smokeParticleSystem.MakeParticlesRepelFromExternalObject();
					}
					break;
			}

			return true;
		}

		#endregion
	}
}
