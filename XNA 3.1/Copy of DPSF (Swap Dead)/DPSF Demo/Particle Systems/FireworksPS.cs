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
    /// Create a new Particle System class that inherits from a
    /// Default DPSF Particle System
    /// </summary>
    class FireworksParticleSystem : DefaultPointSpriteParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FireworksParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        // Different Explosion particle systems to use particles with different textures
        public FireworksExplosionParticleParticleSystem mcFireworksExplosionParticleSystem1 = null;
        public FireworksExplosionParticleParticleSystem mcFireworksExplosionParticleSystem2 = null;
        public FireworksExplosionParticleParticleSystem mcFireworksExplosionParticleSystem3 = null;
        public FireworksExplosionParticleParticleSystem mcFireworksExplosionParticleSystem4 = null;
        
        // Smoke Particle System for Explosions
        public FireworksExplosionParticleParticleSystem mcFireworksExplosionSmokeParticleSystem = null;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        // Initialize internal particle systems after this one has been Initialized
        protected override void AfterInitialize()
        {
            mcFireworksExplosionParticleSystem1 = new FireworksExplosionParticleParticleSystem(Game);
            mcFireworksExplosionParticleSystem2 = new FireworksExplosionParticleParticleSystem(Game);
            mcFireworksExplosionParticleSystem3 = new FireworksExplosionParticleParticleSystem(Game);
            mcFireworksExplosionParticleSystem4 = new FireworksExplosionParticleParticleSystem(Game);
            mcFireworksExplosionSmokeParticleSystem = new FireworksExplosionParticleParticleSystem(Game);

            // Initialize the Fireworks Explosion Particle Systems
            mcFireworksExplosionParticleSystem1.AutoInitialize(GraphicsDevice, ContentManager);
            mcFireworksExplosionParticleSystem1.SetTexture("Textures/Star");

            mcFireworksExplosionParticleSystem2.AutoInitialize(GraphicsDevice, ContentManager);
            mcFireworksExplosionParticleSystem2.SetTexture("Textures/Cloud");

            mcFireworksExplosionParticleSystem3.AutoInitialize(GraphicsDevice, ContentManager);
            mcFireworksExplosionParticleSystem3.SetTexture("Textures/Spark");

            mcFireworksExplosionParticleSystem4.AutoInitialize(GraphicsDevice, ContentManager);
            mcFireworksExplosionParticleSystem4.SetTexture("Textures/Particle");
            mcFireworksExplosionParticleSystem4.LoadShimmeringExplosionParticleEvents();

            mcFireworksExplosionSmokeParticleSystem.AutoInitialize(GraphicsDevice, ContentManager);
            mcFireworksExplosionSmokeParticleSystem.SetTexture("Textures/Smoke");
            mcFireworksExplosionSmokeParticleSystem.LoadExplosionSmokeParticleEvents();

            mcFireworksExplosionParticleSystem1.AutoMemoryManagerSettings.SecondsMaxNumberOfParticlesMustExistForBeforeReducingSize = 10;
            mcFireworksExplosionParticleSystem2.AutoMemoryManagerSettings.SecondsMaxNumberOfParticlesMustExistForBeforeReducingSize = 10;
            mcFireworksExplosionParticleSystem3.AutoMemoryManagerSettings.SecondsMaxNumberOfParticlesMustExistForBeforeReducingSize = 10;
            mcFireworksExplosionParticleSystem4.AutoMemoryManagerSettings.SecondsMaxNumberOfParticlesMustExistForBeforeReducingSize = 10;
            mcFireworksExplosionSmokeParticleSystem.AutoMemoryManagerSettings.SecondsMaxNumberOfParticlesMustExistForBeforeReducingSize = 10;
        }

        protected override void AfterDestroy()
        {
            if (mcFireworksExplosionParticleSystem1 != null)
            {
                mcFireworksExplosionParticleSystem1.Destroy();
                mcFireworksExplosionParticleSystem1 = null;
                mcFireworksExplosionParticleSystem2.Destroy();
                mcFireworksExplosionParticleSystem2 = null;
                mcFireworksExplosionParticleSystem3.Destroy();
                mcFireworksExplosionParticleSystem3 = null;
                mcFireworksExplosionParticleSystem4.Destroy();
                mcFireworksExplosionParticleSystem4 = null;
                mcFireworksExplosionSmokeParticleSystem.Destroy();
                mcFireworksExplosionSmokeParticleSystem = null;
            }
        }

        protected override void AfterUpdate(float fElapsedTimeInSeconds)
        {
            // If the Fireworks Explosion Particle System is Initialized
            if (mcFireworksExplosionParticleSystem1.IsInitialized())
            {
                // Update the Fireworks Explosion Particle System
                mcFireworksExplosionParticleSystem1.Update(fElapsedTimeInSeconds);
            }

            // If the Fireworks Explosion Particle System is Initialized
            if (mcFireworksExplosionParticleSystem2.IsInitialized())
            {
                // Update the Fireworks Explosion Particle System
                mcFireworksExplosionParticleSystem2.Update(fElapsedTimeInSeconds);
            }

            // If the Fireworks Explosion Particle System is Initialized
            if (mcFireworksExplosionParticleSystem3.IsInitialized())
            {
                // Update the Fireworks Explosion Particle System
                mcFireworksExplosionParticleSystem3.Update(fElapsedTimeInSeconds);
            }

            // If the Fireworks Explosion Particle System is Initialized
            if (mcFireworksExplosionParticleSystem4.IsInitialized())
            {
                // Update the Fireworks Explosion Particle System
                mcFireworksExplosionParticleSystem4.Update(fElapsedTimeInSeconds);
            }

            // If the Fireworks Explosion Smoke Particle System is Initialized
            if (mcFireworksExplosionSmokeParticleSystem.IsInitialized())
            {
                // Update the Fireworks Explosion Smoke Particle System
                mcFireworksExplosionSmokeParticleSystem.Update(fElapsedTimeInSeconds);
            }
        }

        protected override void AfterDraw()
        {
            // Set the World, View, and Projection matrices so the Smoke Particle System knows how to draw the particles on screen properly
            mcFireworksExplosionSmokeParticleSystem.SetWorldViewProjectionMatrices(World, View, Projection);
            mcFireworksExplosionParticleSystem1.SetWorldViewProjectionMatrices(World, View, Projection);
            mcFireworksExplosionParticleSystem2.SetWorldViewProjectionMatrices(World, View, Projection);
            mcFireworksExplosionParticleSystem3.SetWorldViewProjectionMatrices(World, View, Projection);
            mcFireworksExplosionParticleSystem4.SetWorldViewProjectionMatrices(World, View, Projection);

            // Draw Smoke first
            // If the Fireworks Explosion Smoke Particle System is Initialized
            if (mcFireworksExplosionSmokeParticleSystem.IsInitialized())
            {
                // Draw the Fireworks Explosion Smoke Particles
                mcFireworksExplosionSmokeParticleSystem.Draw();
            }

            // If the Fireworks Explosion Particle System is Initialized
            if (mcFireworksExplosionParticleSystem1.IsInitialized())
            {
                // Draw the Fireworks Explosion Particles
                mcFireworksExplosionParticleSystem1.Draw();
            }

            // If the Fireworks Explosion Particle System is Initialized
            if (mcFireworksExplosionParticleSystem2.IsInitialized())
            {
                // Draw the Fireworks Explosion Particles
                mcFireworksExplosionParticleSystem2.Draw();
            }

            // If the Fireworks Explosion Particle System is Initialized
            if (mcFireworksExplosionParticleSystem3.IsInitialized())
            {
                // Draw the Fireworks Explosion Particles
                mcFireworksExplosionParticleSystem3.Draw();
            }

            // If the Fireworks Explosion Particle System is Initialized
            if (mcFireworksExplosionParticleSystem4.IsInitialized())
            {
                // Draw the Fireworks Explosion Particles
                mcFireworksExplosionParticleSystem4.Draw();
            }
        }

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager)
        {
            InitializePointSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000,
                                                UpdateVertexProperties, "Textures/Particle");
            LoadEvents();
            Emitter.ParticlesPerSecond = 1;
            Name = "Fireworks";
        }

        public void LoadEvents()
        {
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingExternalForce);
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration, 100);

            ParticleEvents.AddNormalizedTimedEvent(1.0f, CreateExplosionParticles);

            InitialProperties.LifetimeMin = 2.0f;
            InitialProperties.LifetimeMax = 4.5f;
            InitialProperties.PositionMin = new Vector3(0, 0, 0);
            InitialProperties.PositionMax = new Vector3(0, 0, 0);
            InitialProperties.StartSizeMin = 15.0f;
            InitialProperties.StartSizeMax = 15.0f;
            InitialProperties.EndSizeMin = 2.0f;
            InitialProperties.EndSizeMax = 2.0f;
            InitialProperties.StartColorMin = Color.Black;
            InitialProperties.StartColorMax = Color.White;
            InitialProperties.EndColorMin = Color.Red;
            InitialProperties.EndColorMax = Color.DarkBlue;
            InitialProperties.RotationMin = 0.0f;
            InitialProperties.RotationMax = 0.0f;
            InitialProperties.VelocityMin = new Vector3(-25, 50, -25);
            InitialProperties.VelocityMax = new Vector3(25, 70, 25);
            InitialProperties.AccelerationMin = Vector3.Zero;
            InitialProperties.AccelerationMax = Vector3.Zero;
            InitialProperties.RotationalVelocityMin = 0.0f;
            InitialProperties.RotationalVelocityMax = 0.0f;
            InitialProperties.ExternalForceMin = new Vector3(0, -20, 0);
            InitialProperties.ExternalForceMax = new Vector3(0, -20, 0);
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================
        public void CreateExplosionParticles(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // If the Fireworks Explosion Particle Systems are Initialized
            if (mcFireworksExplosionParticleSystem1.IsInitialized() &&
                mcFireworksExplosionParticleSystem2.IsInitialized() &&
                mcFireworksExplosionParticleSystem3.IsInitialized() &&
                mcFireworksExplosionParticleSystem4.IsInitialized())
            {
                // Randomly choose which Particle System to use for the Explosion
                int iPSToUse = RandomNumber.Next(1, 5);
                DPSFDefaultPointSpriteParticleSystem<DefaultPointSpriteParticle, DefaultPointSpriteParticleVertex> cExplosionPS;
                switch (iPSToUse)
                {
                    default:
                    case 1:
                        cExplosionPS = mcFireworksExplosionParticleSystem1;
                    break;

                    case 2:
                        cExplosionPS = mcFireworksExplosionParticleSystem2;
                    break;

                    case 3:
                        cExplosionPS = mcFireworksExplosionParticleSystem3;
                    break;

                    case 4:
                        cExplosionPS = mcFireworksExplosionParticleSystem4;
                    break;
                }

                // Randomly select how many particles should be in this explosion
                int iNumberOfExplosionParticles = RandomNumber.Next(5, 100);

                // Randomly select the size of the Explosion Particles
                int iParticleSize = RandomNumber.Next(0, (int)cExplosionPS.InitialProperties.StartSizeMax);

                // Initialize the Explosion Particles Color variable
                Color sParticleColor = Color.White;

                // If all of the Explosion Particles should be the same Color
                if (RandomNumber.Next(0, 5) != 2)
                {
                    // Randomly select the color of the Explosion Particles
                    sParticleColor = DPSFHelper.LerpColor(Color.Black, Color.White, RandomNumber.NextFloat(), RandomNumber.NextFloat(), RandomNumber.NextFloat(), RandomNumber.NextFloat());
                }
                
                // Create the explosion particles
                for (int iIndex = 0; iIndex < iNumberOfExplosionParticles; iIndex++)
                {
                    DefaultPointSpriteParticle cExplosionParticle = new DefaultPointSpriteParticle();
                    cExplosionPS.InitializeParticle(cExplosionParticle);
                    
                    // Start the Explosion Particles where the fireworks Particle died
                    cExplosionParticle.Position = cParticle.Position;

                    // If the random Particle Size is big enough
                    if (iParticleSize > cExplosionPS.InitialProperties.StartSizeMin)
                    {
                        // Make all Explosion Particles this Size
                        cExplosionParticle.Size = iParticleSize;
                    }
                    // Else each Particle will be a random Size

                    // If the Explosion Particles should all be the same Color
                    if (sParticleColor != Color.White)
                    {
                        cExplosionParticle.Color = sParticleColor;
                    }

                    // Add the Explosion Particle to the Explosion Particle System
                    cExplosionPS.AddParticle(cExplosionParticle);
                }

                // If the Explosion Smoke Particle System is initialized
                if (mcFireworksExplosionSmokeParticleSystem.IsInitialized())
                {
                    // Create some Smoke at the position where the Particle died
                    mcFireworksExplosionSmokeParticleSystem.Emitter.PositionData.Position = cParticle.Position;

                    // Choose how much Smoke to create based on how big the Explosion was
                    int iNumberOfSmokeParticles = iNumberOfExplosionParticles / 2;

                    // Create the Smoke Particles
                    for (int iIndex = 0; iIndex < iNumberOfSmokeParticles; iIndex++)
                    {
                        mcFireworksExplosionSmokeParticleSystem.AddParticle();
                    }
                }
            }
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================
        
        //===========================================================
        // Other Particle System Functions
        //===========================================================
        public void TurnExplosionsOn()
        {
            ParticleEvents.RemoveNormalizedTimedEvents(CreateExplosionParticles);
            ParticleEvents.AddNormalizedTimedEvent(1.0f, CreateExplosionParticles);
        }

        public void TurnExplosionsOff()
        {
            ParticleEvents.RemoveNormalizedTimedEvents(CreateExplosionParticles);
        }


        public class FireworksExplosionParticleParticleSystem : DefaultPointSpriteParticleSystem
        {
            public FireworksExplosionParticleParticleSystem(Game cGame) : base(cGame) { }

            public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager)
            {
                InitializePointSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 10000,
                                                    UpdateVertexProperties, "Textures/Star");
                LoadExplosionParticleEvents();
            }

            public void InitializeParticleFireworksExplosion(DefaultPointSpriteParticle cParticle)
            {
                InitializeParticleUsingInitialProperties(cParticle);
            }

            public void InitializeParticleFireworksExplosionSmoke(DefaultPointSpriteParticle cParticle)
            {
                InitializeParticleUsingInitialProperties(cParticle);

                // Make it so all Smoke doesn't start from the same spot
                cParticle.Position += new Vector3(RandomNumber.Next(-20, 20), RandomNumber.Next(-20, 20), RandomNumber.Next(-20, 20));
            }

            public void LoadExplosionParticleEvents()
            {
                ParticleInitializationFunction = InitializeParticleFireworksExplosion;

                ParticleEvents.RemoveAllEvents();
                ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingFriction);
                ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration, 200);
                ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingExternalForce, 100);
                ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);

                InitialProperties.LifetimeMin = 0.5f;
                InitialProperties.LifetimeMax = 3.0f;
                InitialProperties.PositionMin = new Vector3(0, 0, 0);
                InitialProperties.PositionMax = new Vector3(0, 0, 0);
                InitialProperties.StartSizeMin = 5.0f;
                InitialProperties.StartSizeMax = 25.0f;
                InitialProperties.EndSizeMin = 5.0f;
                InitialProperties.EndSizeMax = 5.0f;
                InitialProperties.StartColorMin = Color.Black;
                InitialProperties.StartColorMax = Color.White;
                InitialProperties.EndColorMin = Color.Red;
                InitialProperties.EndColorMax = Color.DarkBlue;
                InitialProperties.InterpolateBetweenMinAndMaxColors = false;
                InitialProperties.RotationMin = 0.0f;
                InitialProperties.RotationMax = MathHelper.TwoPi;
                InitialProperties.VelocityMin = new Vector3(-50, -50, -50);
                InitialProperties.VelocityMax = new Vector3(50, 50, 50);
                InitialProperties.AccelerationMin = Vector3.Zero;
                InitialProperties.AccelerationMax = Vector3.Zero;
                InitialProperties.FrictionMin = 40.0f;
                InitialProperties.FrictionMax = 40.0f;
                InitialProperties.RotationalVelocityMin = 0.0f;
                InitialProperties.RotationalVelocityMax = 0.0f;
                InitialProperties.ExternalForceMin = new Vector3(0, -10, 0);
                InitialProperties.ExternalForceMax = new Vector3(0, -10, 0);
            }

            public void LoadShimmeringExplosionParticleEvents()
            {
                LoadExplosionParticleEvents();
                ParticleEvents.AddEveryTimeEvent(RandomlyToggleVisiblity);
            }

            public void LoadExplosionSmokeParticleEvents()
            {
                ParticleInitializationFunction = InitializeParticleFireworksExplosionSmoke;

                ParticleEvents.RemoveAllEvents();
                ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityUsingExternalForce);
                ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration, 500);
                ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
                ParticleEvents.AddEveryTimeEvent(UpdateParticleSizeUsingLerp);
                ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);

                InitialProperties.LifetimeMin = 2.0f;
                InitialProperties.LifetimeMax = 5.0f;
                InitialProperties.PositionMin = new Vector3(0, 0, 0);
                InitialProperties.PositionMax = new Vector3(0, 0, 0);
                InitialProperties.StartSizeMin = 5.0f;
                InitialProperties.StartSizeMax = 15.0f;
                InitialProperties.EndSizeMin = 10.0f;
                InitialProperties.EndSizeMax = 20.0f;
                InitialProperties.StartColorMin = Color.Gray;
                InitialProperties.StartColorMax = Color.DarkGray;
                InitialProperties.InterpolateBetweenMinAndMaxColors = true;
                InitialProperties.EndColorMin = Color.Red;
                InitialProperties.EndColorMax = Color.DarkBlue;
                InitialProperties.RotationMin = 0.0f;
                InitialProperties.RotationMax = MathHelper.TwoPi;
                InitialProperties.VelocityMin = new Vector3(-5, -5, -5);
                InitialProperties.VelocityMax = new Vector3(5, 5, 5);
                InitialProperties.AccelerationMin = Vector3.Zero;
                InitialProperties.AccelerationMax = Vector3.Zero;
                InitialProperties.RotationalVelocityMin = -MathHelper.TwoPi;
                InitialProperties.RotationalVelocityMax = MathHelper.TwoPi;
                InitialProperties.ExternalForceMin = new Vector3(-5, 0, 2);
                InitialProperties.ExternalForceMax = new Vector3(-10, 0, 7);
            }

            public void RandomlyToggleVisiblity(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
            {
                if (RandomNumber.NextFloat() < 0.1f)
                {
                    cParticle.Visible = !cParticle.Visible;
                }
            }
        }
    }
}