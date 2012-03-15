#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace DPSF.ParticleSystems
{
    /// <summary>
    /// Create a new Particle System class that inherits from DPSF using 
    /// our created Particle class and Particle Vertex structure.
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    class MagnetsParticleSystem : DefaultSprite3DBillboardParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MagnetsParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        MagnetPoint mcEmitterPointMagnet = null;
        public bool mbMagnetsAffectPosition = true;

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
            InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, "Textures/Star2");

            Name = "Magnet";

            // Finish loading the Particle System in a separate function call, so if
            // we want to reset the Particle System later we don't need to completely 
            // re-initialize it, we can just call this function to reset it.
            LoadEmitterMagnetParticleSystem();
        }

        /// <summary>
        /// Load the Particle System Events and any other settings
        /// </summary>
        public void LoadEmitterMagnetParticleSystem()
        {
            // Set the Particle Initialization Function, as it can be changed on the fly
            // and we want to make sure we are using the right one to start with to start with.
            ParticleInitializationFunction = InitializeParticleRandomDirection;

            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();

            // Setup the Emitter
            Emitter.ParticlesPerSecond = 100;
            Emitter.PositionData.Position = new Vector3(0, 50, 0);

            // Clear all of the Magnets so they can be re-added
            MagnetList.Clear();

            // Setup the Maget attached to the Emitter and add it to the Magnet List
            mcEmitterPointMagnet = new MagnetPoint(Emitter.PositionData.Position,
                                                    DefaultParticleSystemMagnet.MagnetModes.Attract,
                                                    DefaultParticleSystemMagnet.DistanceFunctions.Cubed,
                                                    0, 100, 15, 0);
            MagnetList.AddFirst(mcEmitterPointMagnet);

            // Allow the Particle's Velocity, Rotational Velocity, Color, and Transparency to be updated each frame
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);

            // This function must be executed after the Color Lerp function as the Color Lerp will overwrite the Color's
            // Transparency value, so we give this function an Execution Order of 100 to make sure it is executed last.
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);

            // Add the Magnet Event to the Particle Events
            AddMagnetParticleEvent();

            // Add Particle System Events
            ParticleSystemEvents.AddEveryTimeEvent(UpdateEmitterMagnetToTheEmittersPosition);
        }

        public void LoadSeparateEmitterMagnetsParticleSystem()
        {
            LoadEmitterMagnetParticleSystem();

            // Remove the unnecessary event that was added in the LoadEmitterMagnetParticleSytem() function
            ParticleSystemEvents.RemoveEveryTimeEvent(UpdateEmitterMagnetToTheEmittersPosition, 0, 0);

            // Clear the Magnets List
            MagnetList.Clear();

            // Add two Point Magnets
            MagnetList.AddFirst(new MagnetPoint(new Vector3(100, 50, 0),
                                    DefaultParticleSystemMagnet.MagnetModes.Attract,
                                    DefaultParticleSystemMagnet.DistanceFunctions.SquaredInverse,
                                    0, 100, 20, 0));
            MagnetList.AddFirst(new MagnetPoint(new Vector3(-100, 50, 0),
                                    DefaultParticleSystemMagnet.MagnetModes.Repel,
                                    DefaultParticleSystemMagnet.DistanceFunctions.SquaredInverse,
                                    0, 100, 20, 0));
        }

        public void InitializeParticleRandomDirection(DefaultSprite3DBillboardParticle cParticle)
        {
            cParticle.Lifetime = 5;
            cParticle.Position = Emitter.PositionData.Position;

            // Give the Particle a random velocity direction to start with
            cParticle.Velocity = DPSFHelper.RandomNormalizedVector() * 100;

            cParticle.RotationalVelocity = RandomNumber.Between(-MathHelper.Pi, MathHelper.Pi);
            cParticle.Size = 10;
            cParticle.StartColor = cParticle.EndColor = cParticle.Color = DPSFHelper.RandomColor();
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        /// <summary>
        /// Update the Emitter Magnet's Position to the Emitter's Position
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        protected void UpdateEmitterMagnetToTheEmittersPosition(float fElapsedTimeInSeconds)
        {
            if (mcEmitterPointMagnet != null)
            {
                mcEmitterPointMagnet.PositionData.Position = Emitter.PositionData.Position;
            }
        }

        //===========================================================
        // Other Particle System Functions
        //===========================================================
        public void ToggleMagnetsAffectingPositionVsVelocity()
        {
            // Toggle what should be affected by the Magnets
            mbMagnetsAffectPosition = !mbMagnetsAffectPosition;

            // Use the proper Particle Events
            AddMagnetParticleEvent();
        }

        public void AddMagnetParticleEvent()
        {
            // Remove the Magnet Particle Events
            ParticleEvents.RemoveAllEventsInGroup(1);

            // If the Magnets should affect Position
            if (mbMagnetsAffectPosition)
            {
                ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAccordingToMagnets, 500, 1);
            }
            // Else the Magnets should affect Velocity
            else
            {
                ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityAccordingToMagnets, 500, 1);
            }
        }
    }
}
