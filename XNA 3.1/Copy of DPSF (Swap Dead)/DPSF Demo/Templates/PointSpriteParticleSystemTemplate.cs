#region File Description
//===================================================================
// PointSpriteParticleSystemTemplate.cs
// 
// This file provides the template for creating a new Point Sprite Particle
// System from scratch, by creating a very basic Point Sprite Particle System.
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
using System;
using System.Collections.Generic;
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
    class PointSpriteParticleSystemTemplateParticle : DPSFParticle
    {
        //-----------------------------------------------------------
        // TODO: Add in any properties that you want your Particles to have here.
        // NOTE: A Point Sprite Particle System requires the Particles to at least
        // have a Position and Color (unless using a different Shader than DPSFEffects.fx). 
        // If you want the Particles to have different sizes they must also 
        // have a Size property. If you want them all to be the same size 
        // you can set the RenderState.PointSize in the SetRenderState()
        // function of the Particle System.
        //-----------------------------------------------------------
        public Vector3 Position;        // The Position of the Particle in 3D space
        public Vector3 Velocity;        // The 3D Velocity of the Particle
        public float Size;              // The Width and Height of the Particle
        public Color Color;             // The Color of the Particle

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
            Size = 25;
            Color = Color.White;
        }

        /// <summary>
        /// Deep copy the ParticleToCopy's values into this Particle
        /// </summary>
        /// <param name="ParticleToCopy">The Particle whose values should be Copied</param>
        public override void CopyFrom(DPSFParticle ParticleToCopy)
        {
            // Cast the Particle from its base type to its actual type
            PointSpriteParticleSystemTemplateParticle cParticleToCopy = (PointSpriteParticleSystemTemplateParticle)ParticleToCopy;
            base.CopyFrom(cParticleToCopy);

            //-----------------------------------------------------------
            // TODO: Copy your Particle properties from the given Particle here
            //-----------------------------------------------------------
            Position = cParticleToCopy.Position;
            Velocity = cParticleToCopy.Velocity;
            Size = cParticleToCopy.Size;
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
    struct PointSpriteParticleSystemTemplateParticleVertex : IDPSFParticleVertex
    {
        //===========================================================
        // TODO: Add any more Vertex variables needed to draw your Particles here.
        // Notice how Velocity is not included here, since the Velocity of the
        // Particle cannot be drawn; only its position can. Other drawable properties
        // a Particle might have are Color, Size, Rotation or Normal direction, 
        // Texture coordinates, etc.
        // NOTE: If you are using your own Shaders (i.e. Effect file) you may
        // specify whatever properties here that you wish, but if using the
        // default DPSFEffect.fx file, Point Sprite Vertices must have a
        // Position, Size, Color, and optionally a Rotation 
        // (specified as a float with VertexElementFormat.Single and 
        // VertexElementUsage.TextureCoordinate).
        //===========================================================
        public Vector3 Position;        // The Position of the Particle in 3D space
        public float Size;              // The Width and Height of the Particle
        public Color Color;             // The Color of the Particle

        // Describe the vertex structure used to display a Particle
        private static readonly VertexElement[] msVertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.Position, 0),

            new VertexElement(0, 12, VertexElementFormat.Single,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.PointSize, 0),

            new VertexElement(0, 16, VertexElementFormat.Color,
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
        private const int miSizeInBytes = 12 + 4 + 4;

        /// <summary>
        /// An array describing the attributes of each Vertex
        /// </summary>
        public VertexElement[] VertexElements
        {
            get { return PointSpriteParticleSystemTemplateParticleVertex.msVertexElements; }
        }

        /// <summary>
        /// The Size of one Vertex Element in Bytes
        /// </summary>
        public int SizeInBytes
        {
            get { return PointSpriteParticleSystemTemplateParticleVertex.miSizeInBytes; }
        }
    }

    //-----------------------------------------------------------
    // TODO: Rename/Refactor the Particle System class
    //-----------------------------------------------------------
    /// <summary>
    /// Create a new Particle System class that inherits from DPSF using 
    /// our created Particle class and Particle Vertex structure
    /// </summary>
    class PointSpriteParticleSystemTemplate : DPSF<PointSpriteParticleSystemTemplateParticle, PointSpriteParticleSystemTemplateParticleVertex>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public PointSpriteParticleSystemTemplate(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place any Particle System properties here
        //-----------------------------------------------------------


        //===========================================================
        // Vertex Update and Overridden Particle System Functions
        //===========================================================

        /// <summary>
        /// Function to update the Vertex properties according to the Particle properties
        /// </summary>
        /// <param name="sVertexBuffer">The array containing the Vertices to be drawn</param>
        /// <param name="iIndex">The Index in the array where the Particle's Vertex info should be placed</param>
        /// <param name="Particle">The Particle to copy the information from</param>
        public virtual void UpdateVertexProperties(ref PointSpriteParticleSystemTemplateParticleVertex[] sVertexBuffer, int iIndex, DPSFParticle Particle)
        {
            // Cast the Particle to the type it really is
            PointSpriteParticleSystemTemplateParticle cParticle = (PointSpriteParticleSystemTemplateParticle)Particle;

            //-----------------------------------------------------------
            // TODO: Copy the Particle's renderable properties to the Vertex Buffer
            //-----------------------------------------------------------
            sVertexBuffer[iIndex].Position = cParticle.Position;
            sVertexBuffer[iIndex].Size = cParticle.Size;
            sVertexBuffer[iIndex].Color = cParticle.Color;
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

            // Also specify the Viewport Height as it's used in determining a 
            // Point Sprite's Size relative to how far it is from the Camera
            Effect.Parameters["xViewportHeight"].SetValue(GraphicsDevice.Viewport.Height);

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
            // Enable point sprites
            cRenderState.PointSpriteEnable = true;
            cRenderState.PointSizeMax = 256;

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
            cRenderState.PointSpriteEnable = false;
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
            InitializePointSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, 
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

            // Make the Particles move according to their Velocity
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);

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
        public void InitializeParticleProperties(PointSpriteParticleSystemTemplateParticle cParticle)
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
            cParticle.Size = RandomNumber.Next(10, 50);

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
        public void UpdateParticlePositionUsingVelocity(PointSpriteParticleSystemTemplateParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Position according to its Velocity
            cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
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
