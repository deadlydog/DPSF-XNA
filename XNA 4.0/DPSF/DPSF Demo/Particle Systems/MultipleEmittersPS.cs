#region Using Statements
using System;
using System.Linq;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF_Demo.ParticleSystems
{
    /// <summary>
    /// Create a new Particle System class that inherits from a Default DPSF Particle System
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    class MultipleEmittersParticleSystem : DefaultSprite3DBillboardParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
		public MultipleEmittersParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

		int topLeftEmitterID = 0;
		int topRightEmitterID = 0;
		int bottomLeftEmitterID = 0;
		int bottomRightEmitterID = 0;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        //===========================================================
        // Initialization Functions
        //===========================================================

        /// <summary>
        /// Function to Initialize the Particle System with default values
        /// </summary>
        /// <param name="cGraphicsDevice">The Graphics Device to draw to</param>
        /// <param name="cContentManager">The Content Manager to use to load Textures and Effect files</param>
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            // Initialize the Particle System before doing anything else
            InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, "Textures/Bubble", cSpriteBatch);

            // Set the Name of the Particle System
            Name = "Multiple Emitters";

            // Finish loading the Particle System in a separate function call, so if
            // we want to reset the Particle System later we don't need to completely 
            // re-initialize it, we can just call this function to reset it.
            LoadParticleSystem();
        }

        /// <summary>
        /// Load the Particle System Events and any other settings
        /// </summary>
        public void LoadParticleSystem()
        {
			ParticleInitializationFunction = InitializeParticle;

            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();

            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);


            // Setup the static Emitters

			// Setup the top-left emitter. 
			// We have one emitter by default, so we don't need to create the first one.
            Emitter.ParticlesPerSecond = 5;
            Emitter.PositionData.Position = new Vector3(-100, 150, 0);
			Emitter.OrientationData.Rotate(Matrix.CreateRotationZ(MathHelper.ToRadians(-135)));
			topLeftEmitterID = Emitter.ID;

			// Setup the top-right emitter.
			// When we set the Emitter to a new ParticleEmitter it automatically gets added to the Emitters collection.
			Emitter = new ParticleEmitter();
			Emitter.ParticlesPerSecond = 5;
			Emitter.PositionData.Position = new Vector3(100, 150, 0);
			Emitter.OrientationData.Rotate(Matrix.CreateRotationZ(MathHelper.ToRadians(135)));
			topRightEmitterID = Emitter.ID;

			// Setup the bottom-left emitter.
			// Here we create a Particle Emitter and add it to the Emitters collection explicitly (just to show another way of doing it).
			var emitter = new ParticleEmitter();
			emitter.ParticlesPerSecond = 5;
			emitter.PositionData.Position = new Vector3(-100, 10, 0);
			emitter.OrientationData.Rotate(Matrix.CreateRotationZ(MathHelper.ToRadians(-45)));
			Emitters.Add(emitter);
			bottomLeftEmitterID = emitter.ID;

			// Setup the bottom-right emitter.
			Emitter = new ParticleEmitter();	// This will automatically get added to the Emitters collection.
			Emitter.ParticlesPerSecond = 5;
			Emitter.PositionData.Position = new Vector3(100, 10, 0);
			Emitter.OrientationData.Rotate(Matrix.CreateRotationZ(MathHelper.ToRadians(45)));
			bottomRightEmitterID = Emitter.ID;


			// Have the particle system events repeat every 2 seconds.
			ParticleSystemEvents.LifetimeData.Lifetime = 2f;
			ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;

			// Add an event to create a new emitter every time the particle system lifetime repeats.
			ParticleSystemEvents.AddTimedEvent(1.0f, CreateRandomSelfDestructiveEmitter);
        }

        /// <summary>
        /// Example of how to create a Particle Initialization Function
        /// </summary>
        /// <param name="cParticle">The Particle to be Initialized</param>
        public void InitializeParticle(DefaultSprite3DBillboardParticle cParticle)
        {
            cParticle.Lifetime = 1.0f;

            // Set the Particle's initial Position to be wherever the Emitter is
            cParticle.Position = Emitter.PositionData.Position;

            // Set the Particle's Velocity
            cParticle.Velocity = new Vector3(0, 100, 0);

            // Adjust the Particle's Velocity direction according to the Emitter's Orientation
            cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);

            // Give the Particle a random Size
            cParticle.Size = 10;

            // Give the Particle a color based on which emitter is creating it.
        	if (Emitter.ID == topLeftEmitterID)
        	{
        		cParticle.Color = Color.LightBlue;
        	}
        	else if (Emitter.ID == topRightEmitterID)
        	{
        		cParticle.Color = Color.Red;
        	}
        	else if (Emitter.ID == bottomLeftEmitterID)
        	{
        		cParticle.Color = Color.LightGreen;
        	}
        	else if (Emitter.ID == bottomRightEmitterID)
        	{
        		cParticle.Color = Color.HotPink;
        	}
        	else
        	{
				// Else this is one of the random emitters, so use a random color for the particle color.
        		cParticle.Color = DPSFHelper.RandomColor();
        	}
        }


        //===========================================================
        // Particle Update Functions
        //===========================================================

        //===========================================================
        // Particle System Update Functions
        //===========================================================

		private void CreateRandomSelfDestructiveEmitter(float elapsedTimeInSeconds)
		{
			// Create a new random particle system.
			Emitter = new ParticleEmitter();
			Emitter.ParticlesPerSecond = RandomNumber.Next(5, 50);
			Emitter.PositionData.Position = RandomPosition;
			Emitter.OrientationData.Orientation = new Quaternion(DPSFHelper.RandomNormalizedVector(), RandomNumber.NextFloat());
			Emitter.OrientationData.RotationalVelocity = DPSFHelper.RandomNormalizedVector() * RandomNumber.Next(0, 7);
			
			// Let's have the Emitter kill itself after emitting 200 particles.
			Emitter.EmitParticlesAutomatically = false;
			Emitter.BurstParticles = 200;
			Emitter.BurstComplete += new EventHandler(Emitter_BurstComplete);
		}

		void Emitter_BurstComplete(object sender, EventArgs e)
		{
			// Have the emitter that called this event remove itself from the list of emitters.
			ParticleEmitter emitter = sender as ParticleEmitter;
			Emitters.Remove(emitter.ID);
		}

        //===========================================================
        // Other Particle System Functions
        //===========================================================

    	private Vector3 RandomPosition
    	{
			get { return new Vector3(RandomNumber.Between(-100, 100), RandomNumber.Between(0, 160), RandomNumber.Between(-100, 100)); }
    	}

		public void ToggleStaticEmittersOnAndOff()
		{
			// Toggle each of the static emitters on/off.
			Emitters[topLeftEmitterID].Enabled = !Emitters[topLeftEmitterID].Enabled;
			Emitters[topRightEmitterID].Enabled = !Emitters[topRightEmitterID].Enabled;
			Emitters[bottomLeftEmitterID].Enabled = !Emitters[bottomLeftEmitterID].Enabled;
			Emitters[bottomRightEmitterID].Enabled = !Emitters[bottomRightEmitterID].Enabled;
		}

		public void ToggleRandomEmittersOnAndOff()
		{
			bool randomEmittersExist = false;

			// Loop through each random emitter.
			foreach (ParticleEmitter randomEmitter in Emitters.Emitters.Where(p => p.ID != topLeftEmitterID &&
				p.ID != topRightEmitterID && p.ID != bottomLeftEmitterID && p.ID != bottomRightEmitterID))
			{
				// Kill the random emitter.
				randomEmitter.BurstParticles = 0;

				// Mark that a random emitter did exist.
				randomEmittersExist = true;
			}

			// If random emitters were turned on.
			if (randomEmittersExist)
			{
				// Make sure the particle system stops creating new random emitters.
				ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Nothing;
				ParticleSystemEvents.LifetimeData.NormalizedElapsedTime = 1.0f;
			}
			else
			{
				// Allow the particle system to start creating random emitters again.
				ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;

				// Create an emitter right now.
				CreateRandomSelfDestructiveEmitter(0);
			}
		}
    }
}
