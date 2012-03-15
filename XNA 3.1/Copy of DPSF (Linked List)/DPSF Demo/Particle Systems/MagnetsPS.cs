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
    /// our created Particle class and Particle Vertex structure
    /// </summary>
    class MagnetsParticleSystem : DefaultPointSpriteParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MagnetsParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        DefaultParticleSystemMagnet mcEmitterMagnet = null;
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
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager)
        {
            // Initialize the Particle System before doing anything else
            InitializePointSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, 
                                                UpdateVertexProperties, "Textures/Star2");

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
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;

            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();

            // Setup the Emitter
            Emitter.ParticlesPerSecond = 100;
            Emitter.PositionData.Position = new Vector3(0, 50, 0);

            // Clear all of the Magnets so they can be re-added
            MagnetList.Clear();

            // Setup the Maget attached to the Emitter and add it to the Magnet List
            mcEmitterMagnet = new DefaultParticleSystemMagnet(DefaultParticleSystemMagnet.MagnetModes.Attract,
                                        DefaultParticleSystemMagnet.DistanceFunctions.Cubed,
                                        Emitter.PositionData.Position, 0, 100, 15, 0);
            MagnetList.AddFirst(mcEmitterMagnet);

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


            // Setup the Initial properties of the Particles.
            // You could also write your own Particle Initialization Function as
            // shown below instead to initialize the Particles in a specific way.
            InitialProperties.LifetimeMin = 4.0f;
            InitialProperties.LifetimeMax = 4.0f;
            InitialProperties.PositionMin = Vector3.Zero;
            InitialProperties.PositionMax = Vector3.Zero;
            InitialProperties.VelocityMin = new Vector3(-100, -100, -100);
            InitialProperties.VelocityMax = new Vector3(100, 100, 100);
            InitialProperties.RotationMin = 0.0f;
            InitialProperties.RotationMax = MathHelper.Pi;
            InitialProperties.RotationalVelocityMin = -MathHelper.Pi;
            InitialProperties.RotationalVelocityMax = MathHelper.Pi;
            InitialProperties.StartSizeMin = 10;
            InitialProperties.StartSizeMax = 10;
            InitialProperties.EndSizeMin = 10;
            InitialProperties.EndSizeMax = 10;
            InitialProperties.StartColorMin = Color.Black;
            InitialProperties.StartColorMax = Color.White;
            InitialProperties.EndColorMin = Color.Black;
            InitialProperties.EndColorMax = Color.White;
        }

        public void LoadSeparateEmitterMagnetsParticleSystem()
        {
            LoadEmitterMagnetParticleSystem();

            // Remove the unneccessary event that was added in the LoadEmitterMagnetParticleSytem() function
            ParticleSystemEvents.RemoveEveryTimeEvent(UpdateEmitterMagnetToTheEmittersPosition, 0, 0);

            MagnetList.Clear();
            MagnetList.AddFirst(new DefaultParticleSystemMagnet(DefaultParticleSystemMagnet.MagnetModes.Attract,
                                        DefaultParticleSystemMagnet.DistanceFunctions.SquaredInverse, new Vector3(100, 50, 0),
                                        0, 100, 20, 0));
            MagnetList.AddFirst(new DefaultParticleSystemMagnet(DefaultParticleSystemMagnet.MagnetModes.Repel,
                                        DefaultParticleSystemMagnet.DistanceFunctions.SquaredInverse, new Vector3(-100, 50, 0),
                                        0, 100, 20, 0));
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
        public void UpdateEmitterMagnetToTheEmittersPosition(float fElapsedTimeInSeconds)
        {
            if (mcEmitterMagnet != null)
            {
                mcEmitterMagnet.PositionData.Position = Emitter.PositionData.Position;
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
