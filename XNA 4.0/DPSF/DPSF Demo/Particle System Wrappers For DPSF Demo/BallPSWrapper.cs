#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace DPSF.ParticleSystems
{
    // Create a new type of Particle for this Particle System
#if (WINDOWS)
    [Serializable]
#endif
    class BallParticle : DefaultTexturedQuadParticle
    {
        // We need another variable to hold the Particle's untransformed Position (it's Emitter Independent Position)
        public Vector3 sEmitterIndependentPosition;

        public BallParticle()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            sEmitterIndependentPosition = Vector3.Zero;
        }

        public override void CopyFrom(DPSFParticle ParticleToCopy)
        {
            // Cast the Particle to the type it really is
            BallParticle cParticleToCopy = (BallParticle)ParticleToCopy;

            base.CopyFrom(cParticleToCopy);
            sEmitterIndependentPosition = cParticleToCopy.sEmitterIndependentPosition;
        }
    }

    /// <summary>
    /// Create a new Particle System class that inherits from a
    /// Default DPSF Particle System
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    class BallParticleSystem : DPSFDefaultTexturedQuadParticleSystem<BallParticle, DefaultTexturedQuadParticleVertex>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BallParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        private int miBallRadius = 50;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000,
                                                    UpdateVertexProperties, "Textures/Particle");
            Name = "Ball";
            LoadParticleSystem();
        }

        private void LoadParticleSystem()
        {
            ParticleInitializationFunction = InitializeParticleBall;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAccordingToEmitter);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 100);

            miBallRadius = 50;

            Emitter.PositionData.Position = new Vector3(0, miBallRadius, 0);
            Emitter.ParticlesPerSecond = 100;

            MaxNumberOfParticlesAllowed = 300;
        }

        public void InitializeParticleBall(BallParticle cParticle)
        {
            cParticle.Lifetime = 0.0f;  // Particle never dies
            cParticle.Size = 10.0f;

            // Calculate the distance between each Particle on the Ball
            int iOneThirdOfParticlesAllowed = MaxNumberOfParticlesAllowed / 3;
            float fStepSize = MathHelper.TwoPi / (float)iOneThirdOfParticlesAllowed;

            // If we are initializing the first third of Particles (Z-axis)
            if (NumberOfActiveParticles < iOneThirdOfParticlesAllowed)
            {
                // Find Particle's position on the Vertical Circle
                float fAngle = fStepSize * NumberOfActiveParticles;
                cParticle.sEmitterIndependentPosition = DPSFHelper.PointOnSphere(-MathHelper.PiOver2, fAngle, miBallRadius);
                cParticle.Color = Color.Blue;
            }
            // Else we are initializing the second third of Particles (Y-axis)
            else if (NumberOfActiveParticles < (2 * iOneThirdOfParticlesAllowed))
            {
                // Find Particle's position on the Vertical Circle
                float fAngle = fStepSize * NumberOfActiveParticles;
                cParticle.sEmitterIndependentPosition = DPSFHelper.PointOnSphere(0, fAngle, miBallRadius);
                cParticle.Color = Color.Green;
            }
            // Else we are initializing the last third of Particles (X-axis)
            else
            {
                // Find Particle's position on the Horizontal Circle
                float fAngle = fStepSize * (NumberOfActiveParticles - iOneThirdOfParticlesAllowed);
                fAngle += MathHelper.Pi;    // Have this circle start at the other side of the ring
                cParticle.sEmitterIndependentPosition = DPSFHelper.PointOnSphere(fAngle, 0, miBallRadius);
                cParticle.Color = Color.Red;
            }
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================
        protected void UpdateParticlePositionAccordingToEmitter(BallParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Rotate the Particle around the Emitter according to the Emitters Orientation
            cParticle.Position = Vector3.Transform(cParticle.sEmitterIndependentPosition, Emitter.OrientationData.Orientation);
            cParticle.Position += Emitter.PositionData.Position;
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        //===========================================================
        // Other Particle System Functions
        //===========================================================
        public void IncreaseRadius()
        {
            miBallRadius++;
            RecreateBall();
        }

        public void DecreaseRadius()
        {
            miBallRadius--;

            if (miBallRadius < 10)
            {
                miBallRadius = 10;
            }
            RecreateBall();
        }

        public void MoreParticles()
        {
            MaxNumberOfParticlesAllowed += 3;
            RecreateBall();
        }

        public void LessParticles()
        {
            MaxNumberOfParticlesAllowed -= 3;
            if (MaxNumberOfParticlesAllowed < 3)
            {
                MaxNumberOfParticlesAllowed = 3;
            }
            RecreateBall();
        }

        private void RecreateBall()
        {
            RemoveAllParticles();
            while (AddParticle())
            { }
        }
    }
}