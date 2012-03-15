#region File Description
//===================================================================
// TexturedQuadParticleSystemTemplate.cs
// 
// This file provides the template for creating a new Textured Quad Particle
// System from scratch, by creating a very basic Textured Quad Particle System.
//
// First, it shows how to create a new Particle class so the user can 
// create Particles with whatever properties they need their Particles
// to contain. Next, it shows how to create the Particle Vertex structure, 
// which is used to draw the particles to the screen. Last, it shows 
// how to create the Particle System class itself.
//
// The spots that should be modified are marked with TODO statements.
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace DPSF.ParticleSystems
{
    //-----------------------------------------------------------
    // TODO: Rename/Refactor the Particle class
    //-----------------------------------------------------------
    /// <summary>
    /// Create a new Particle class that inherits from DPSFParticle
    /// </summary>
    class TexturedQuadParticleSystemTemplateParticle : DPSFParticle
    {
        //-----------------------------------------------------------
        // TODO: Add in any properties that you want your Particles to have here.
        // NOTE: A Quad Particle System requires the Particles to at least
        // have a Position and Color (unless using a different Shader than DPSFEffects.fx). 
        // If you want the Particles to have different sizes they must also 
        // have some property to indicate it, such as Size, Width and Height,
        // or Vertex 1 - 4's position relative to the Particle's Position. 
        // If you want them all to be the same size you can specify the size
        // in the UpdateVertexProperties() function of the Particle System class.
        //-----------------------------------------------------------
        public Vector3 Position;        // The Position of the Particle in 3D space
        public Vector3 Velocity;        // The 3D Velocity of the Particle
        public Quaternion Orientation;  // The 3D Orientation of the Particle
        public float Width;             // The Width of the Particle
        public float Height;            // The Height of the Particle
        public Color Color;             // The Color of the Particle

        /// <summary>
        /// Get / Set the Normal (forward) direction of the Particle (i.e. which direction it is facing)
        /// </summary>
        public Vector3 Normal
        {
            get { return Orientation3D.GetNormalDirection(Orientation); }
            set { Orientation3D.SetNormalDirection(ref Orientation, value); }
        }

        /// <summary>
        /// Get / Set the Up direction of the Particle
        /// </summary>
        public Vector3 Up
        {
            get { return Orientation3D.GetUpDirection(Orientation); }
            set { Orientation3D.SetUpDirection(ref Orientation, value); }
        }

        /// <summary>
        /// Get / Set the Right direction of the Particle
        /// </summary>
        public Vector3 Right
        {
            get { return Orientation3D.GetRightDirection(Orientation); }
            set { Orientation3D.SetRightDirection(ref Orientation, value); }
        }

        /// <summary>
        /// Resets the Particles variables to default values
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            //-----------------------------------------------------------
            // TODO: Reset your Particle properties to their default values here
            //-----------------------------------------------------------
            Position = Vector3.Zero;
            Velocity = Vector3.Zero;
            Orientation = Quaternion.Identity;
            Width = 25;
            Height = 25;
            Color = Color.White;
        }

        /// <summary>
        /// Deep copy the ParticleToCopy's values into this Particle
        /// </summary>
        /// <param name="ParticleToCopy">The Particle whose values should be Copied</param>
        public override void CopyFrom(DPSFParticle ParticleToCopy)
        {
            // Cast the Particle from its base type to its actual type
            TexturedQuadParticleSystemTemplateParticle cParticleToCopy = (TexturedQuadParticleSystemTemplateParticle)ParticleToCopy;
            base.CopyFrom(cParticleToCopy);

            //-----------------------------------------------------------
            // TODO: Copy your Particle properties from the given Particle here
            //-----------------------------------------------------------
            Position = cParticleToCopy.Position;
            Velocity = cParticleToCopy.Velocity;
            Orientation = cParticleToCopy.Orientation;
            Width = cParticleToCopy.Width;
            Height = cParticleToCopy.Height;
            Color = cParticleToCopy.Color;
        }
    }

    //-----------------------------------------------------------
    // TODO: Rename/Refactor the Particle Vertex struct
    //-----------------------------------------------------------
    /// <summary>
    /// Create a new structure that inherits from IDSPFParticleVertex to hold
    /// the Particle Vertex properties used to draw the Particle
    /// </summary>
    struct TexturedQuadParticleSystemTemplateParticleVertex : IDPSFParticleVertex
    {
        //===========================================================
        // TODO: Add any more Vertex variables needed to draw your Particles here.
        // Notice how Velocity is not included here, since the Velocity of the
        // Particle cannot be drawn; only its position can. Other drawable properties
        // a Particle might have are Color, Size, Rotation or Normal direction, 
        // Texture coordinates, etc.
        // NOTE: If you are using your own Shaders (i.e. Effect file) you may
        // specify whatever properties here that you wish, but if using the
        // default DPSFEffect.fx file, Quad Vertices must have a
        // Position, Color, and Texture Coordinates.
        //===========================================================
        public Vector3 Position;            // The Position of the Particle in 3D space
        public Vector2 TextureCoordinate;   // The Texture Coordinates of the Vertex
        public Color Color;                 // The Color of the Particle

        // Describe the vertex structure used to display a Particle
        private static readonly VertexElement[] msVertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.Position, 0),

            new VertexElement(0, 12, VertexElementFormat.Vector2,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.TextureCoordinate, 0),

            new VertexElement(0, 20, VertexElementFormat.Color,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.Color, 0),

            //-----------------------------------------------------------
            // TODO: Add the VertexElements describing the Vertex variables you added here
            //-----------------------------------------------------------
        };

        //-----------------------------------------------------------
        // TODO: Change miSizeInBytes to reflect the total size of the msVertexElements
        // array if any new Vertex Elements were added to it
        //-----------------------------------------------------------
        // The size of the vertex structure in bytes
        private const int miSizeInBytes = 12 + 8 + 4;

        /// <summary>
        /// An array describing the attributes of each Vertex
        /// </summary>
        public VertexElement[] VertexElements
        {
            get { return TexturedQuadParticleSystemTemplateParticleVertex.msVertexElements; }
        }

        /// <summary>
        /// The Size of one Vertex Element in Bytes
        /// </summary>
        public int SizeInBytes
        {
            get { return TexturedQuadParticleSystemTemplateParticleVertex.miSizeInBytes; }
        }
    }

    //-----------------------------------------------------------
    // TODO: Rename/Refactor the Particle System class
    //-----------------------------------------------------------
    /// <summary>
    /// Create a new Particle System class that inherits from DPSF using 
    /// our created Particle class and Particle Vertex structure
    /// </summary>
    class TexturedQuadParticleSystemTemplate : DPSF<TexturedQuadParticleSystemTemplateParticle, TexturedQuadParticleSystemTemplateParticleVertex>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public TexturedQuadParticleSystemTemplate(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place any Particle System properties here
        //-----------------------------------------------------------
        Vector3 msCameraPosition = Vector3.Zero;

        /// <summary>
        /// Get / Set the Position of the Camera.
        /// NOTE: This should be Set (updated) every frame if Billboarding will be used 
        /// (i.e. Always have the Particles face the Camera)
        /// </summary>
        public Vector3 CameraPosition
        {
            get { return msCameraPosition; }
            set { msCameraPosition = value; }
        }

        //===========================================================
        // Vertex Update and Overridden Particle System Functions
        //===========================================================

        /// <summary>
        /// Function to update the Vertex properties according to the Particle properties
        /// </summary>
        /// <param name="sVertexBuffer">The array containing the Vertices to be drawn</param>
        /// <param name="iIndex">The Index in the array where the Particle's Vertex info should be placed</param>
        /// <param name="Particle">The Particle to copy the information from</param>
        public virtual void UpdateVertexProperties(ref TexturedQuadParticleSystemTemplateParticleVertex[] sVertexBuffer, int iIndex, DPSFParticle Particle)
        {
            // Cast the Particle to the type it really is
            TexturedQuadParticleSystemTemplateParticle cParticle = (TexturedQuadParticleSystemTemplateParticle)Particle;

            //-----------------------------------------------------------
            // TODO: Copy the Particle's renderable properties to the Vertex Buffer.
            // NOTE: Because a Quad is made up of 4 Vertices, we must populate
            // each of the 4 Vertices data in the Vertex Buffer, as well as
            // specify the Vertex order in the Index Buffer.
            //-----------------------------------------------------------
            // Calculate what half of the Quads Width and Height are
            int iHalfWidth = (int)(cParticle.Width / 2.0f);
            int iHalfHeight = (int)(cParticle.Height / 2.0f);

            // Calculate the Positions of the Quads corners around the origin
            Vector3 sTopLeft = new Vector3(-iHalfWidth, iHalfHeight, 0);
            Vector3 sTopRight = new Vector3(iHalfWidth, iHalfHeight, 0);
            Vector3 sBottomLeft = new Vector3(-iHalfWidth, -iHalfHeight, 0);
            Vector3 sBottomRight = new Vector3(iHalfWidth, -iHalfHeight, 0);

            // Rotate the Quad corners around the origin to make it face the Camera, 
            // then calculate their final Positions
            sTopLeft = Vector3.Transform(sTopLeft, cParticle.Orientation) + cParticle.Position;
            sTopRight = Vector3.Transform(sTopRight, cParticle.Orientation) + cParticle.Position;
            sBottomLeft = Vector3.Transform(sBottomLeft, cParticle.Orientation) + cParticle.Position;
            sBottomRight = Vector3.Transform(sBottomRight, cParticle.Orientation) + cParticle.Position;

            // Copy this Particle's renderable Properties to the Vertex Buffer
            // This is a Quad so we must copy all 4 Vertices over
            sVertexBuffer[iIndex].Position = sBottomLeft;
            sVertexBuffer[iIndex].TextureCoordinate = new Vector2(0, 1);
            sVertexBuffer[iIndex].Color = cParticle.Color;

            sVertexBuffer[iIndex + 1].Position = sTopLeft;
            sVertexBuffer[iIndex + 1].TextureCoordinate = new Vector2(0, 0);
            sVertexBuffer[iIndex + 1].Color = cParticle.Color;

            sVertexBuffer[iIndex + 2].Position = sBottomRight;
            sVertexBuffer[iIndex + 2].TextureCoordinate = new Vector2(1, 1);
            sVertexBuffer[iIndex + 2].Color = cParticle.Color;

            sVertexBuffer[iIndex + 3].Position = sTopRight;
            sVertexBuffer[iIndex + 3].TextureCoordinate = new Vector2(1, 0);
            sVertexBuffer[iIndex + 3].Color = cParticle.Color;

            // Fill in the Index Buffer for the newly added Vertices.
            // Specify the Vertices in Counter-Clockwise order.
            // It takes 6 Indexes to represent a quad (2 triangles = 6 corners).
            IndexBuffer[IndexBufferIndex++] = iIndex;
            IndexBuffer[IndexBufferIndex++] = iIndex + 2;
            IndexBuffer[IndexBufferIndex++] = iIndex + 1;
            IndexBuffer[IndexBufferIndex++] = iIndex + 2;
            IndexBuffer[IndexBufferIndex++] = iIndex + 3;
            IndexBuffer[IndexBufferIndex++] = iIndex + 1;   
        }

        /// <summary>
        /// Function to set the Shaders global variables before drawing
        /// </summary>
        protected override void SetEffectParameters()
        {
            //-----------------------------------------------------------
            // TODO: Set any global Shader variables required before drawing
            //-----------------------------------------------------------
            // Specify the World, View, and Projection Matrices
            Effect.Parameters["xWorld"].SetValue(World);
            Effect.Parameters["xView"].SetValue(View);
            Effect.Parameters["xProjection"].SetValue(Projection);

            // Set the Texture to use for the Particles
            Effect.Parameters["xTexture"].SetValue(Texture);

            // Blend the Particle's Color evenly with the Texture's Color
            Effect.Parameters["xColorBlendAmount"].SetValue(0.5f);
        }

        /// <summary>
        /// Function to set the RenderState properties before drawing
        /// </summary>
        /// <param name="cRenderState">The RenderState used to draw</param>
        protected override void SetRenderState(RenderState cRenderState)
        {
            //-----------------------------------------------------------
            // TODO: Set any RenderState properties required before drawing
            //-----------------------------------------------------------
            // Set the alpha blend mode
            cRenderState.AlphaBlendEnable = true;
            cRenderState.AlphaBlendOperation = BlendFunction.Add;
            cRenderState.SourceBlend = Blend.SourceAlpha;
            cRenderState.DestinationBlend = Blend.InverseSourceAlpha;

            // Set the alpha test mode
            cRenderState.AlphaTestEnable = true;
            cRenderState.AlphaFunction = CompareFunction.Greater;
            cRenderState.ReferenceAlpha = 0;

            // Enable the depth buffer (so particles will not be visible through
            // solid objects like the ground plane), but disable depth writes
            // (so particles will not obscure other particles).
            cRenderState.DepthBufferEnable = true;
            cRenderState.DepthBufferWriteEnable = false;
        }

        /// <summary>
        /// Function to reset any RenderState properties that were changed after drawing
        /// </summary>
        /// <param name="cRenderState">The RenderState used to draw</param>
        protected override void ResetRenderState(RenderState cRenderState)
        {
            //-----------------------------------------------------------
            // TODO: Reset any unusual RenderState properties that were changed
            //-----------------------------------------------------------
            // Reset the more unusual Render States that we changed,
            // so as not to mess up any other subsequent drawing
            cRenderState.AlphaBlendEnable = false;
            cRenderState.AlphaTestEnable = false;
            cRenderState.DepthBufferWriteEnable = true;
        }

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
            //-----------------------------------------------------------
            // TODO: Change any Initialization parameters desired
            //-----------------------------------------------------------
            // Initialize the Particle System before doing anything else
            InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, 
                                                    UpdateVertexProperties, "Textures/Bubble");

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
            //-----------------------------------------------------------
            // TODO: Setup the Particle System to achieve the desired result.
            // You may change all of the code in this function. It is just
            // provided to show you how to setup a simple particle system.
            //-----------------------------------------------------------

            // Set the Function to use to Initialize new Particles
            ParticleInitializationFunction = InitializeParticleProperties;

            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();

            // Make the Particles move according to their Velocity, and orient them to face the Camera
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 1000);

            // Set the Particle System's Emitter to toggle on and off every 0.5 seconds
            ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
            ParticleSystemEvents.LifetimeData.Lifetime = 1.0f;
            ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemEmitParticlesAutomaticallyOn);
            ParticleSystemEvents.AddTimedEvent(0.5f, UpdateParticleSystemEmitParticlesAutomaticallyOff);

            // Setup the Emitter
            Emitter.ParticlesPerSecond = 50;
            Emitter.PositionData.Position = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Function to Initialize a new Particle's properties
        /// </summary>
        /// <param name="cParticle">The Particle to be Initialized</param>
        public void InitializeParticleProperties(TexturedQuadParticleSystemTemplateParticle cParticle)
        {
            //-----------------------------------------------------------
            // TODO: Initialize all of the Particle's properties here.
            // In addition to initializing the Particle properties you added, you
            // must also initialize the Lifetime property that is inherited from DPSFParticle
            //-----------------------------------------------------------

            // Set the Particle's Lifetime (how long it should exist for)
            cParticle.Lifetime = 2.0f;

            // Set the Particle's initial Position to be wherever the Emitter is
            cParticle.Position = Emitter.PositionData.Position;

            // Set the Particle's Velocity
            Vector3 sVelocityMin = new Vector3(-50, 50, -50);
            Vector3 sVelocityMax = new Vector3(50, 100, 50);
            cParticle.Velocity = DPSFHelper.RandomVectorBetweenTwoVectors(sVelocityMin, sVelocityMax);

            // Adjust the Particle's Velocity direction according to the Emitter's Orientation
            cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);

            // Give the Particle a random Size
            cParticle.Width = RandomNumber.Next(20, 40);
            cParticle.Height = RandomNumber.Next(20, 40);

            // Give the Particle a random Color
            cParticle.Color = DPSFHelper.RandomColor();
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place your Particle Update functions here, using the 
        // same function prototype as below (i.e. public void FunctionName(DPSFParticle, float))
        //-----------------------------------------------------------

        /// <summary>
        /// Update a Particle's Position according to its Velocity
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticlePositionUsingVelocity(TexturedQuadParticleSystemTemplateParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Position according to its Velocity
            cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
        }

        /// <summary>
        /// Turns the Particle into a Billboard Particle (i.e. The Particle always faces the Camera).
        /// NOTE: This Update function should be called after all other Update functions to ensure that 
        /// the Particle is orientated correctly.
        /// NOTE: Update the Particle System's Camera Position every frame to ensure that this works correctly.
        /// NOTE: Only Roll Rotations (i.e. around the Z axis) will be visible when this is used.
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleToFaceTheCamera(TexturedQuadParticleSystemTemplateParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Make the Particle face the Camera
            cParticle.Normal = CameraPosition - cParticle.Position;
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place your Particle System Update functions here, using 
        // the same function prototype as below (i.e. public void FunctionName(float))
        //-----------------------------------------------------------

        /// <summary>
        /// Sets the Emitter to Emit Particles Automatically
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleSystemEmitParticlesAutomaticallyOn(float fElapsedTimeInSeconds)
        {
            Emitter.EmitParticlesAutomatically = true;
        }

        /// <summary>
        /// Sets the Emitter to not Emit Particles Automatically
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleSystemEmitParticlesAutomaticallyOff(float fElapsedTimeInSeconds)
        {
            Emitter.EmitParticlesAutomatically = false;
        }

        //===========================================================
        // Other Particle System Functions
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place any other functions here
        //-----------------------------------------------------------
    }
}
