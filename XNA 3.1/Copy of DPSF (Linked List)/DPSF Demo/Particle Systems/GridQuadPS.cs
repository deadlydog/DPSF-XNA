#region File Description
//===================================================================
// GridParticleSystem.cs
// 
// This file provides the template for creating a new Point Sprite Particle
// System that inherits from the Default Point Sprite Particle System.
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
    // TODO: Rename/Refactor the Particle System class
    //-----------------------------------------------------------
    /// <summary>
    /// Create a new Particle System class that inherits from a
    /// Default DPSF Particle System
    /// </summary>
    class GridQuadParticleSystem : DefaultTexturedQuadParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public GridQuadParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        const int miNumberOfRows = 20;
        const int miNumberOfColumns = 20;
        const int miNumberOfLayers = 20;
        int miNumberOfParticles = miNumberOfRows * miNumberOfColumns * miNumberOfLayers;
        int miSpaceBetweenParticles = 50;

        Color msStartColor = DPSFHelper.RandomColor();
        Color msEndColor = DPSFHelper.RandomColor();
        int miStartSize = 20;
        int miEndSize = 50;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        protected override void SetRenderState(RenderState cRenderState)
        {
            base.SetRenderState(cRenderState);
            cRenderState.DepthBufferWriteEnable = true; // Turn on depth (z-buffer) sorting
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
            // TODO: Change any Initialization parameters desired and the Name
            //-----------------------------------------------------------
            // Initialize the Particle System before doing anything else
            InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, miNumberOfParticles, miNumberOfParticles,
                                                UpdateVertexProperties, "Textures/Donut");

            // Set the Name of the Particle System
            Name = "Grid (Quads)";

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

            // Set the Function to use to Initialize new Particles.
            // The Default Templates include a Particle Initialization Function called
            // InitializeParticleUsingInitialProperties, which initializes new Particles
            // according to the settings in the InitialProperties object (see further below).
            // You can also create your own Particle Initialization Functions as well, as shown with
            // the InitializeParticleProperties function below.
            ParticleInitializationFunction = InitializeParticleProperties;

            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();

            ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
            ParticleEvents.AddNormalizedTimedEvent(1.0f, UpdateParticleResetProperties);

            ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera);

            // Setup the Emitter
            Emitter.ParticlesPerSecond = miNumberOfParticles;
            Emitter.PositionData.Position = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Example of how to create a Particle Initialization Function
        /// </summary>
        /// <param name="cParticle">The Particle to be Initialized</param>
        public void InitializeParticleProperties(DefaultTexturedQuadParticle cParticle)
        {
            //-----------------------------------------------------------
            // TODO: Initialize all of the Particle's properties here.
            // If you plan on simply using the default InitializeParticleUsingInitialProperties
            // Particle Initialization Function (see the LoadParticleSystem() function above), 
            // then you may delete this function all together.
            //-----------------------------------------------------------
            cParticle.Lifetime = 1.0f;

            int iTotalWidth = miNumberOfColumns * miSpaceBetweenParticles;
            int iTotalHeight = miNumberOfRows * miSpaceBetweenParticles;
            int iTotalDepth = miNumberOfLayers * miSpaceBetweenParticles;

            // Get which Particle this is being added
            int iParticleNumber = ActiveParticles.Count;

            // Calculate where in the Grid this Particle should be
            int iLayer = iParticleNumber / (miNumberOfRows * miNumberOfColumns);
            int iRow = (iParticleNumber / miNumberOfRows) % miNumberOfRows;
            int iColumn = iParticleNumber % miNumberOfRows;

            // Calculate the Particles absolute position
            Vector3 sPosition = new Vector3(iColumn * miSpaceBetweenParticles, iRow * miSpaceBetweenParticles, iLayer * miSpaceBetweenParticles);

            // Center the Grid around the origin
            sPosition.X -= (iTotalWidth / 2);
            sPosition.Y -= (iTotalHeight / 2);
            sPosition.Z -= (iTotalDepth / 2);

            // Set the Particle's initial Position
            cParticle.Position = sPosition;

            cParticle.Width = cParticle.Height = cParticle.StartWidth = cParticle.StartHeight = miStartSize;
            cParticle.EndWidth = cParticle.EndHeight = miEndSize;

            cParticle.Color = cParticle.StartColor = msStartColor;
            cParticle.EndColor = msEndColor;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place your Particle Update functions here, using the 
        // same function prototype as below (i.e. public void FunctionName(DPSFParticle, float))
        //-----------------------------------------------------------

        /// <summary>
        /// Updates the Particle's Start and End Colors
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleResetProperties(DefaultTexturedQuadParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.ElapsedTime = 0.0f;

            // If this is the first particle that was added to the list
            if (cParticle == ActiveParticles.Last.Value)
            {
                // Choose a new Random Color to use
                msStartColor = msEndColor;
                msEndColor = DPSFHelper.RandomColor();

                int iTempSize = miStartSize;
                miStartSize = miEndSize;
                miEndSize = iTempSize;
            }

            cParticle.StartColor = msStartColor;
            cParticle.EndColor = msEndColor;

            cParticle.StartWidth = cParticle.StartHeight = miStartSize;
            cParticle.EndWidth = cParticle.EndHeight = miEndSize;
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        //===========================================================
        // Other Particle System Functions
        //===========================================================
    }
}
