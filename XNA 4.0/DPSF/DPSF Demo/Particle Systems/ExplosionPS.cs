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
    public class ExplosionParticleSystem : DefaultNoDisplayParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExplosionParticleSystem(Game game) : base(game) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        /// <summary>
        /// The Color of the explosion.
        /// </summary>
        public Color ExplosionColor 
        {
            get { return _explosionColor; }
            set
            {
                _explosionColor = value;
                _debrisParticleSystem.ChangeExplosionColor(_explosionColor);
                _fireSmokeParticleSystem.ChangeExplosionColor(_explosionColor);
                _flashParticleSystem.ChangeExplosionColor(_explosionColor);
                _flyingSparksParticleSystem.ChangeExplosionColor(_explosionColor);
                _roundSparksParticleSystem.ChangeExplosionColor(_explosionColor);
                _shockwaveParticleSystem.ChangeExplosionColor(_explosionColor);
                _smokeTrailsParticleSystem.ChangeExplosionColor(_explosionColor);
            }
        }
        private Color _explosionColor = new Color(255, 120, 0);

        /// <summary>
        /// The Size of the individual Particles.
        /// </summary>
        public int ExplosionParticleSize { get; set; }

        /// <summary>
        /// The Intensity of the explosion.
        /// </summary>
        public int ExplosionIntensity { get; set; }

        ParticleSystemManager _particleSystemManager = null;
        ExplosionDebrisParticleSystem _debrisParticleSystem = null;
        ExplosionFireSmokeParticleSystem _fireSmokeParticleSystem = null;
        ExplosionFlashParticleSystem _flashParticleSystem = null;
        ExplosionFlyingSparksParticleSystem _flyingSparksParticleSystem = null;
        ExplosionRoundSparksParticleSystem _roundSparksParticleSystem = null;
        ExplosionShockwaveParticleSystem _shockwaveParticleSystem = null;
        ExplosionSmokeTrailsParticleSystem _smokeTrailsParticleSystem = null;

        /// <summary>
        /// Get / Set the Camera Position used by the particle system
        /// </summary>
        public Vector3 CameraPosition { get; set; }

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        /// <summary>
        /// Sets the camera position.
        /// </summary>
        /// <param name="cameraPosition">The camera position.</param>
        public override void SetCameraPosition(Vector3 cameraPosition)
        {
            this.CameraPosition = cameraPosition;
        }

        protected override void AfterAddParticle()
        {
            base.AfterAddParticle();
            Explode();
        }
        
        protected override void AfterDestroy()
        {
            base.AfterDestroy();

            // Remove all particle systems from the manager and destroy it
            if (_particleSystemManager != null)
            {
                _particleSystemManager.RemoveAllParticleSystems();
                _particleSystemManager = null;
            }

            // Destroy all of the particle systems
            if (_debrisParticleSystem != null)
            {
                _debrisParticleSystem.Destroy();
                _debrisParticleSystem = null;
            }

            if (_fireSmokeParticleSystem != null)
            {
                _fireSmokeParticleSystem.Destroy();
                _fireSmokeParticleSystem = null;
            }

            if (_flashParticleSystem != null)
            {
                _flashParticleSystem.Destroy();
                _flashParticleSystem = null;
            }

            if (_flyingSparksParticleSystem != null)
            {
                _flyingSparksParticleSystem.Destroy();
                _flyingSparksParticleSystem = null;
            }

            if (_roundSparksParticleSystem != null)
            {
                _roundSparksParticleSystem.Destroy();
                _roundSparksParticleSystem = null;
            }

            if (_shockwaveParticleSystem != null)
            {
                _shockwaveParticleSystem.Destroy();
                _shockwaveParticleSystem = null;
            }

            if (_smokeTrailsParticleSystem != null)
            {
                _smokeTrailsParticleSystem.Destroy();
                _smokeTrailsParticleSystem = null;
            }
        }

        protected override void AfterUpdate(float fElapsedTimeInSeconds)
        {
            base.AfterUpdate(fElapsedTimeInSeconds);

            // Update the Camera Position for the particle systems first
            _debrisParticleSystem.CameraPosition = this.CameraPosition;
            _fireSmokeParticleSystem.CameraPosition = this.CameraPosition;
            _flashParticleSystem.CameraPosition = this.CameraPosition;
            _flyingSparksParticleSystem.CameraPosition = this.CameraPosition;
            _roundSparksParticleSystem.CameraPosition = this.CameraPosition;
            _shockwaveParticleSystem.CameraPosition = this.CameraPosition;
            _smokeTrailsParticleSystem.CameraPosition = this.CameraPosition;

            // Update the Position of the Emitters
            _debrisParticleSystem.Emitter.PositionData.Position = this.Emitter.PositionData.Position;
            _fireSmokeParticleSystem.Emitter.PositionData.Position = this.Emitter.PositionData.Position;
            _flashParticleSystem.Emitter.PositionData.Position = this.Emitter.PositionData.Position;
            _flyingSparksParticleSystem.Emitter.PositionData.Position = this.Emitter.PositionData.Position;
            _roundSparksParticleSystem.Emitter.PositionData.Position = this.Emitter.PositionData.Position;
            _shockwaveParticleSystem.Emitter.PositionData.Position = this.Emitter.PositionData.Position;
            _smokeTrailsParticleSystem.Emitter.PositionData.Position = this.Emitter.PositionData.Position;

            // Update all of the particle systems
            _particleSystemManager.UpdateAllParticleSystems(fElapsedTimeInSeconds);
        }

        protected override void AfterDraw()
        {
            base.AfterDraw();

            // Draw all of the particle systems
            _particleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(this.World, this.View, this.Projection);
            _particleSystemManager.DrawAllParticleSystems();
        }

        public override int TotalNumberOfActiveParticles { get { return base.TotalNumberOfActiveParticles + _particleSystemManager.TotalNumberOfActiveParticles; } }
        public override int TotalNumberOfParticlesAllocatedInMemory { get { return base.TotalNumberOfParticlesAllocatedInMemory + _particleSystemManager.TotalNumberOfParticlesAllocatedInMemory; } }
        public override int TotalNumberOfParticlesBeingDrawn { get { return base.TotalNumberOfParticlesBeingDrawn + _particleSystemManager.TotalNumberOfParticlesBeingDrawn; } }

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice graphicsDevice, ContentManager contentManager, SpriteBatch spriteBatch)
        {
            InitializeNoDisplayParticleSystem(10, 100);

            // A No Display particle system doesn't take a graphics device or content manager, so we can't
            // override the AfterInitialize() function to initialize our other particle systems. So we just
            // create another function to do it after setting the graphics device and content manager.
            SetGraphicsDevice(graphicsDevice);
            this.ContentManager = contentManager;
            AutoInitializeOtherParticleSystems();

            Name = "Explosion";
            LoadLargeExplosion();
        }

        /// <summary>
        /// Initialize all of the particle systems used by this particle system class.
        /// </summary>
        private void AutoInitializeOtherParticleSystems()
        {
            _particleSystemManager = new ParticleSystemManager();

            // Create all the particle systems
            _debrisParticleSystem = new ExplosionDebrisParticleSystem(this.Game);
            _fireSmokeParticleSystem = new ExplosionFireSmokeParticleSystem(this.Game);
            _flashParticleSystem = new ExplosionFlashParticleSystem(this.Game);
            _flyingSparksParticleSystem = new ExplosionFlyingSparksParticleSystem(this.Game);
            _roundSparksParticleSystem = new ExplosionRoundSparksParticleSystem(this.Game);
            _shockwaveParticleSystem = new ExplosionShockwaveParticleSystem(this.Game);
            _smokeTrailsParticleSystem = new ExplosionSmokeTrailsParticleSystem(this.Game);

            // Specify the order the particle systems should be drawn in
            _debrisParticleSystem.DrawOrder = 100;
            _fireSmokeParticleSystem.DrawOrder = 200;
            _flashParticleSystem.DrawOrder = 200;
            _flyingSparksParticleSystem.DrawOrder = 200;
            _roundSparksParticleSystem.DrawOrder = 200;
            _shockwaveParticleSystem.DrawOrder = 200;
            _smokeTrailsParticleSystem.DrawOrder = 200;

            // Add all of the particle systems to the manager
            _particleSystemManager.AddParticleSystem(_debrisParticleSystem);
            _particleSystemManager.AddParticleSystem(_fireSmokeParticleSystem);
            _particleSystemManager.AddParticleSystem(_flashParticleSystem);
            _particleSystemManager.AddParticleSystem(_flyingSparksParticleSystem);
            _particleSystemManager.AddParticleSystem(_roundSparksParticleSystem);
            _particleSystemManager.AddParticleSystem(_shockwaveParticleSystem);
            _particleSystemManager.AddParticleSystem(_smokeTrailsParticleSystem);

            // Initialize all of the particle systems
            _particleSystemManager.AutoInitializeAllParticleSystems(this.GraphicsDevice, this.ContentManager, null);
        }

        public void LoadLargeExplosion()
        {
            // Specify the particle initialization function
            ParticleInitializationFunction = InitializeParticleExplosion;

            // Setup the behaviors that the particles should have
            ParticleEvents.RemoveAllEvents();

            // Setup the emitter
            Emitter.PositionData.Position = new Vector3(0, 25, 0);
            Emitter.ParticlesPerSecond = 10000;
            Emitter.EmitParticlesAutomatically = false; // We will call the Explode() function to release a burst of particles instead of always emitting them

            // Set the default explosion settings
            ExplosionColor = new Color(255, 120, 0);
            ExplosionParticleSize = 10;
            ExplosionIntensity = 20;
        }

        public void SetupToAutoExplodeEveryInterval(float intervalInSeconds)
        {
            // Set the Particle System's Emitter to release a burst of particles after a set interval
            ParticleSystemEvents.RemoveAllEventsInGroup(1);
            ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
            ParticleSystemEvents.LifetimeData.Lifetime = intervalInSeconds;
            ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemToExplode, 0, 1);
        }

        public void InitializeParticleExplosion(DefaultNoDisplayParticle particle)
        {
            // Just kill the particle instantly, as we just want adding a particle to trigger the rest of the particle systems to add their particles.
            particle.Lifetime = 1;
            particle.NormalizedElapsedTime = 1;
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
            _debrisParticleSystem.Explode();
            _fireSmokeParticleSystem.Explode();
            _flashParticleSystem.Explode();
            _flyingSparksParticleSystem.Explode();
            _roundSparksParticleSystem.Explode();
            _shockwaveParticleSystem.Explode();
            _smokeTrailsParticleSystem.Explode();
        }

        /// <summary>
        /// Change the color of the explosion.
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
