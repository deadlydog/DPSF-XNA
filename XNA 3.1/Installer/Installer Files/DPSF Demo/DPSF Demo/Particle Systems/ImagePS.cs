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
    [Serializable]
    class ImageParticle : DefaultAnimatedTexturedQuadParticle
    {
        // We need another variable to hold the Particle's untransformed Position (it's Emitter Independent Position)
        public Vector3 sImagePosition;
        public Quaternion cImageOrientation;
        public Vector3 sRotation;
        public bool bGoingToImagePosition;
        public bool bReachedImagePosition;
        public bool bReachedImageOrientation;
        public Quaternion cOrientationBeforeAutomaticMovement;

        public ImageParticle()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            sImagePosition = Vector3.Zero;
            cImageOrientation = Quaternion.Identity;
            sRotation = Vector3.Zero;
            bGoingToImagePosition = bReachedImagePosition = bReachedImageOrientation = false;
            cOrientationBeforeAutomaticMovement = Quaternion.Identity;
        }

        public override void CopyFrom(DPSFParticle ParticleToCopy)
        {
            // Cast the Particle to the type it really is
            ImageParticle cParticleToCopy = (ImageParticle)ParticleToCopy;

            base.CopyFrom(cParticleToCopy);
            sImagePosition = cParticleToCopy.sImagePosition;
            cImageOrientation = cParticleToCopy.cImageOrientation;
            sRotation = cParticleToCopy.sRotation;
            bGoingToImagePosition = cParticleToCopy.bGoingToImagePosition;
            bReachedImagePosition = cParticleToCopy.bReachedImagePosition;
            bReachedImageOrientation = cParticleToCopy.bReachedImageOrientation;
            cOrientationBeforeAutomaticMovement = cParticleToCopy.cOrientationBeforeAutomaticMovement;
        }
    }

    /// <summary>
    /// Create a new Particle System class that inherits from a
    /// Default DPSF Particle System
    /// </summary>
    [Serializable]
    class ImageParticleSystem : DPSFDefaultAnimatedTexturedQuadParticleSystem<ImageParticle, DefaultTexturedQuadParticleVertex>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ImageParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        public int miNumberOfRows = 16;
        public int miNumberOfColumns = 16;
        private int miWidthOfCompositeImage = 128;
        private int miHeightOfCompositeImage = 128;
        private float mfTimeBeforeMovingToImagePosition = 5.0f;
        public string msSpinMode = "None";
        public bool mbUniformSpin = true;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================
        protected override void SetRenderState(RenderState cRenderState)
        {
            base.SetRenderState(cRenderState);
            cRenderState.CullMode = CullMode.None;
            cRenderState.DepthBufferWriteEnable = true;
        }

        protected override void ResetRenderState(RenderState cRenderState)
        {
            base.ResetRenderState(cRenderState);
            cRenderState.CullMode = CullMode.CullCounterClockwiseFace;
        }

        protected override void SetEffectParameters()
        {
            base.SetEffectParameters();

            // Show only the Textures Color (do not blend with Particle Color)
            Effect.Parameters["xColorBlendAmount"].SetValue(0.0f);
        }

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, 
                                                UpdateVertexProperties, "Textures/Dog");
            Emitter.ParticlesPerSecond = 200;
            LoadImage();
        }

        public void LoadImage()
        {
            Name = "Image";
            RemoveAllParticles();

            ParticleInitializationFunction = InitializeParticleImage;
            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);

            // Calculate how many Particles are required to create the Image
            MaxNumberOfParticlesAllowed = miNumberOfRows * miNumberOfColumns;

            msSpinMode = "None";
        }

        public void InitializeParticleImage(ImageParticle cParticle)
        {
            SetParticlePositionWidthHeightAndTextureCoordinatesToFormImage(cParticle);

            cParticle.Lifetime = 0;
            cParticle.Position = cParticle.sImagePosition;
            cParticle.bGoingToImagePosition = true;
            cParticle.bReachedImagePosition = true;
            cParticle.bReachedImageOrientation = true;
        }

        public void LoadVortexIntoFinalImage()
        {
            Name = "Vortex Into Image";
            RemoveAllParticles();

            ParticleInitializationFunction = InitializeParticleVortexIntoFinalImage;
            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);
            ParticleEvents.AddEveryTimeEvent(RotateAroundOrigin, 100);
            ParticleEvents.AddEveryTimeEvent(MoveToFinalPosition, 200);

            // Calculate how many Particles are required to create the Image
            MaxNumberOfParticlesAllowed = miNumberOfRows * miNumberOfColumns;

            msSpinMode = "Effect";
        }

        public void InitializeParticleVortexIntoFinalImage(ImageParticle cParticle)
        {
            SetParticlePositionWidthHeightAndTextureCoordinatesToFormImage(cParticle);

            cParticle.Lifetime = 0;
            cParticle.RotationalVelocity = new Vector3(RandomNumber.Between(0, MathHelper.Pi), RandomNumber.Between(0, MathHelper.Pi), RandomNumber.Between(0, MathHelper.Pi));
            cParticle.sRotation = new Vector3(0, MathHelper.Pi, 0);
            cParticle.Position = new Vector3(RandomNumber.Next(-100, 100), 0, RandomNumber.Next(-100, 100));
            cParticle.Velocity = new Vector3(0, RandomNumber.Next(1, 25), 0);
        }

        public void LoadSpiralIntoFinalImage()
        {
            Name = "Spiral Into Image";
            RemoveAllParticles();

            ParticleInitializationFunction = InitializeParticleSpiralIntoFinalImage;
            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);
            ParticleEvents.AddEveryTimeEvent(RotateAroundOrigin, 100);
            ParticleEvents.AddEveryTimeEvent(MoveToFinalPosition, 200);

            // Calculate how many Particles are required to create the Image
            MaxNumberOfParticlesAllowed = miNumberOfRows * miNumberOfColumns;

            msSpinMode = "Effect";
        }

        public void InitializeParticleSpiralIntoFinalImage(ImageParticle cParticle)
        {
            SetParticlePositionWidthHeightAndTextureCoordinatesToFormImage(cParticle);

            cParticle.Lifetime = 0;
            cParticle.RotationalVelocity = new Vector3(RandomNumber.Between(0, MathHelper.Pi), RandomNumber.Between(0, MathHelper.Pi), RandomNumber.Between(0, MathHelper.Pi));
            cParticle.sRotation = new Vector3(0, MathHelper.Pi, 0);
            cParticle.Position = new Vector3(-100, 0, 0);

            float fVerticalSpeed = cParticle.sImagePosition.Y / mfTimeBeforeMovingToImagePosition;
            cParticle.Velocity = new Vector3(0, fVerticalSpeed, 0);
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================
        protected void RotateAroundOrigin(ImageParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Calculate the Rotation Matrix to Rotate the Particle by
            Vector3 sAmountToRotate = cParticle.sRotation * fElapsedTimeInSeconds;
            Matrix sRotation = Matrix.CreateFromYawPitchRoll(sAmountToRotate.Y, sAmountToRotate.X, sAmountToRotate.Z);
            
            // Rotate the Particle around the origin
            cParticle.Position = PivotPoint3D.RotatePosition(sRotation, new Vector3(0, cParticle.Position.Y, 0), cParticle.Position);
        }

        protected void MoveToFinalPosition(ImageParticle cParticle, float fElapsedTimeInSeconds)
        {
            // If it is not time for the Particle to go to its final destination yet, or it has already reached its final destination and orientation
            if (cParticle.ElapsedTime < mfTimeBeforeMovingToImagePosition ||
                (cParticle.bReachedImagePosition && cParticle.bReachedImageOrientation))
            {
                // Exit without doing anything
                return;
            }

            if (!cParticle.bGoingToImagePosition)
            {
                // Make sure the Particle doesn't move on its own anymore (this function now controls it)
                cParticle.Acceleration = Vector3.Zero;
                cParticle.RotationalVelocity = Vector3.Zero;
                cParticle.RotationalAcceleration = Vector3.Zero;
                cParticle.sRotation = Vector3.Zero;

                // Make the Particle move towards its final destination
                cParticle.Velocity = cParticle.sImagePosition - cParticle.Position;

                Quaternion cRotationRequired = Orientation3D.GetRotationTo(Orientation3D.GetNormalDirection(cParticle.Orientation), Orientation3D.GetNormalDirection(cParticle.cImageOrientation));
                cRotationRequired *= Orientation3D.GetRotationTo(Orientation3D.GetUpDirection(cParticle.Orientation), Orientation3D.GetUpDirection(cParticle.cImageOrientation));
                cParticle.cOrientationBeforeAutomaticMovement = cRotationRequired;

                cParticle.bGoingToImagePosition = true;

                msSpinMode = "Effect";
            }

            // If the Particle hasn't made it to its Image Position yet
            if (!cParticle.bReachedImagePosition)
            {
                // Calculate the Vector from the current position to the Image Position
                Vector3 sVectorToFinalPosition = cParticle.sImagePosition - cParticle.Position;
                float fLength = sVectorToFinalPosition.LengthSquared();

                // If the Particle is pretty much in its final Position
                if (fLength < 1)
                {
                    cParticle.Velocity = Vector3.Zero;
                    cParticle.Position = cParticle.sImagePosition;
                    cParticle.bReachedImagePosition = true;
                }
                // Else if the Particle is still fairly far from its final Position
                else if (fLength > 500)
                {
                    cParticle.Velocity = sVectorToFinalPosition;
                }
            }

            // If the Particle hasn't made it to its Image Orientation yet
            if (!cParticle.bReachedImageOrientation)
            {
                float fLerpAmount = (cParticle.ElapsedTime - mfTimeBeforeMovingToImagePosition) / 3.0f;
                if (fLerpAmount > 1.0f)
                {
                    cParticle.Orientation = cParticle.cImageOrientation;
                    cParticle.bReachedImageOrientation = true;
                }
                else
                {
                    cParticle.Orientation = Quaternion.Slerp(cParticle.cOrientationBeforeAutomaticMovement, cParticle.cImageOrientation, fLerpAmount);
                }
            }
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================
        
        //===========================================================
        // Other Particle System Functions
        //===========================================================

        // This function sets the Particle Properties so that when all Particles are viewed together,
        // they form the complete image of the texture
        private void SetParticlePositionWidthHeightAndTextureCoordinatesToFormImage(ImageParticle cParticle)
        {
            // Calculate how big the Particles should be to achieve the desired size
            int iRequiredParticleWidth = miWidthOfCompositeImage / miNumberOfColumns;
            int iRequiredParticleHeight = miHeightOfCompositeImage / miNumberOfRows;
            
            // Make sure the Required Particle Width and Height are large enough to be seen
            if (iRequiredParticleWidth < 2) 
            { 
                iRequiredParticleWidth = 2; 
                miNumberOfColumns = miWidthOfCompositeImage / iRequiredParticleWidth;

                // ReCalculate how many Particles are required to create the Image
                MaxNumberOfParticlesAllowed = miNumberOfRows * miNumberOfColumns;
            }
            if (iRequiredParticleHeight < 2) 
            { 
                iRequiredParticleHeight = 2; 
                miNumberOfRows = miHeightOfCompositeImage / iRequiredParticleHeight;

                // ReCalculate how many Particles are required to create the Image
                MaxNumberOfParticlesAllowed = miNumberOfRows * miNumberOfColumns;
            }

            // Calculate how big one Row and Column from the texture should be
            int iTextureRowSize = Texture.Height / miNumberOfRows;
            int iTextureColumnSize = Texture.Width / miNumberOfColumns;

            // Calculate which Row and Column this Particle should be at
            int iRow = NumberOfActiveParticles / miNumberOfColumns;
            int iColumn = NumberOfActiveParticles % miNumberOfColumns;

            // Calculate this Particle's Position to create the full Image
            int iY = (miNumberOfRows * iRequiredParticleHeight) - ((iRow * iRequiredParticleHeight) + (iRequiredParticleHeight / 2));
            int iX = (iColumn * iRequiredParticleWidth) + (iRequiredParticleWidth / 2);
            iX -= (miNumberOfColumns * iRequiredParticleWidth) / 2;    // Center the image

            // Calculate this Particle's Texture Coordinates to use
            float fTextureTop = (iRow * iTextureRowSize) / (float)Texture.Height;
            float fTextureLeft = (iColumn * iTextureColumnSize) / (float)Texture.Width;
            float fTextureBottom = ((iRow * iTextureRowSize) + iTextureRowSize) / (float)Texture.Height;
            float fTextureRight = ((iColumn * iTextureColumnSize) + iTextureColumnSize) / (float)Texture.Width;

            // Set the Particle's Properties to Form the complete Image
            cParticle.Width = iRequiredParticleWidth;
            cParticle.Height = iRequiredParticleHeight;
            cParticle.sImagePosition = new Vector3(iX, iY, 0);
            cParticle.cImageOrientation = Orientation3D.GetQuaternionWithOrientation(Vector3.Forward, Vector3.Up);
            cParticle.NormalizedTextureCoordinateLeftTop = new Vector2(fTextureLeft, fTextureTop);
            cParticle.NormalizedTextureCoordinateRightBottom = new Vector2(fTextureRight, fTextureBottom);
        }

        public void SetNumberOfRowsAndColumns(int iRows, int iColumns)
        {
            miNumberOfRows = iRows;
            miNumberOfColumns = iColumns;

            // Calculate how many Particles are required to create the Image
            MaxNumberOfParticlesAllowed = miNumberOfRows * miNumberOfColumns;

            // Recreate the image with the new properties
            RemoveAllParticles();
        }

        public void SetSizeOfCompositeImage(int iWidth, int iHeight)
        {
            miWidthOfCompositeImage = iWidth;
            miHeightOfCompositeImage = iHeight;

            // Recreate the image with the new properties
            RemoveAllParticles();
        }

        public void ToggleSpin(string sSpinMode)
        {
            // Set the Spin Mode to use
            msSpinMode = sSpinMode;

            // Loop through all Active Particles
            LinkedListNode<ImageParticle> cNode = ActiveParticles.First;
            while (cNode != null)
            {
                // Get a handle to this Particle
                ImageParticle cParticle = (ImageParticle)(DPSFParticle)cNode.Value;

                // Reset the Particle to have no Rotation
                cParticle.Orientation = cParticle.cImageOrientation;

                float fSpinSpeed = 0, fSpinSpeed2 = 0, fSpinSpeed3 = 0;
                if (mbUniformSpin)
                {
                    fSpinSpeed = fSpinSpeed2 = fSpinSpeed3 = MathHelper.Pi;
                }
                else
                {
                    fSpinSpeed = RandomNumber.Between(0, MathHelper.TwoPi);
                    fSpinSpeed2 = RandomNumber.Between(0, MathHelper.TwoPi);
                    fSpinSpeed3 = RandomNumber.Between(0, MathHelper.TwoPi);
                }

                switch (sSpinMode)
                {
                    default:
                    case "Pitch":
                        cParticle.RotationalVelocity = new Vector3(fSpinSpeed, 0, 0);
                        msSpinMode = "Pitch";
                    break;

                    case "Yaw":
                    cParticle.RotationalVelocity = new Vector3(0, fSpinSpeed, 0);
                    break;

                    case "Roll":
                    cParticle.RotationalVelocity = new Vector3(0, 0, fSpinSpeed);
                    break;

                    case "All":
                    cParticle.RotationalVelocity = new Vector3(fSpinSpeed, fSpinSpeed2, fSpinSpeed3);
                    break;

                    case "None":
                        cParticle.RotationalVelocity = new Vector3(0, 0, 0);
                    break;
                }

                // Move to the Next Particle in the list
                cNode = cNode.Next;
            }
        }

        public void ToggleUniformSpin()
        {
            mbUniformSpin = !mbUniformSpin;

            ToggleSpin(msSpinMode);
        }

        public void Scatter()
        {
            // Loop through all Active Particles
            LinkedListNode<ImageParticle> cNode = ActiveParticles.First;
            while (cNode != null)
            {
                // Get a handle to this Particle
                ImageParticle cParticle = (ImageParticle)(DPSFParticle)cNode.Value;
 
                // Randomly select some Particles
                int iSize = MaxNumberOfParticlesAllowed;
                if (RandomNumber.Next(0, iSize) < (iSize / 10))
                {
                    // Swap the Position of this Particle with another
                    int iParticleToSwapWith = RandomNumber.Next(0, MaxNumberOfParticlesAllowed);
                    
                    // Find the Particle to swap with
                    LinkedListNode<ImageParticle> cSwapNode = ActiveParticles.First;
                    for (int iIndex = 0; iIndex < iParticleToSwapWith; iIndex++)
                    {
                        // If we are still in the List
                        if (cSwapNode != null)
                        {
                            cSwapNode = cSwapNode.Next;
                        }
                    }

                    // If we found a Particle to swap with
                    if (cSwapNode != null)
                    {
                        ImageParticle cSwapParticle = (ImageParticle)(DPSFParticle)cSwapNode.Value;
                        Vector3 sTempPosition = cParticle.Position;
                        cParticle.Position = cSwapParticle.Position;
                        cSwapParticle.Position = sTempPosition;
                    }
                }

                // Move to the Next Particle in the list
                cNode = cNode.Next;
            }
        }
    }
}