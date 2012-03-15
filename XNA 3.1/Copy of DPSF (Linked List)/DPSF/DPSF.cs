#region File Description
//===================================================================
// DPSF.cs
// 
// This is the base class for Dan's Particle System Framework. This
// class should be inherited by another Particle System class, and 
// the provided functions will take care of Particle management.
//
// If you want DPSF Particle Systems to run as DrawableGameComponents,
// add the symbol "DPSFAsDrawableGameComponent" to the Conditional
// Compilation Symbols in the Build options in the Project Properties.
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;       // Used for Conditional Attributes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace DPSF
{
    /// <summary>
    /// Class used to hold a Particle's properties.
    /// This class only holds a Particle's Lifetime information, but may be inherited from
    /// in order to specify additional Particle properties, such as position, size, color, etc.
    /// </summary>
    public class DPSFParticle
    {
        #region Fields

        //===========================================================
        // Automatic Variables - updated automatically by DPSF
        //===========================================================
        private float mfElapsedTime = 0.0f;         // How long (in seconds) since the Particle was Initialized
        private float mfLastElapsedTime = 0.0f;     // How long since the Particle was Initialized the last time Update() was called
        private float mfNormalizedElapsedTime = 0.0f;       // How far the Particle is into its life (0.0 = birth, 1.0 = death)
        private float mfLastNormalizedElapsedTime = 0.0f;   // How far the Particle was into its life the last time Update() was called

        //===========================================================
        // Manual Variables - must be updated manually by the user
        //===========================================================
        private float mfLifetime;       // How long the Particle should exist for (<= 0.0 means particle does not die)
        private bool mbVisible;         // If the Particle should be drawn or not    

        #endregion

        #region Methods

        /// <summary>
        /// Constructor to initialize Particle variables
        /// </summary>
        public DPSFParticle()
        {
            // Initialize the Manual Variables
            Reset();
        }

        /// <summary>
        /// Function to update the Elapsed Time associated variables of the Particle. This is done
        /// automatically by DPSF when the particle system's Update() function is called, so this
        /// function does not need to be manually called by the user.
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">The amount of time in seconds that 
        /// has passed since this function was last called</param>
        public void UpdateElapsedTimeVariables(float fElapsedTimeInSeconds)
        {
            // Backup the Elapsed Time of the Particle at the last Update()
            mfLastElapsedTime = mfElapsedTime;

            // Calculate the new Elapsed Time since the Particle was Initialized
            mfElapsedTime += fElapsedTimeInSeconds;

            // If the Particle should not live forever
            if (mfLifetime > 0.0f)
            {
                // Calculate the new Normalized Elapsed Time
                mfLastNormalizedElapsedTime = mfNormalizedElapsedTime;
                mfNormalizedElapsedTime = mfElapsedTime / mfLifetime;
            }
            // Else the Particle should live forever
            else
            {
                // Reset the Normalized Elapsed Time since the Lifetime cannot be Normalized
                mfLastNormalizedElapsedTime = 0.0f;
                mfNormalizedElapsedTime = 0.0f;
            }
        }

        /// <summary>
        /// Function to tell if a Particle is still Active (alive) or not
        /// </summary>
        /// <returns>Returns true if the Particle is Active (alive), false if it is Inactive (dead)</returns>
        public bool IsActive()
        {
            // Return if the Particle is still Active or not
            // NOTE: A Lifetime <= 0 means the particle should never die (i.e. is always Active)
            return (mfElapsedTime < mfLifetime || mfLifetime <= 0.0f);
        }

        /// <summary>
        /// Get / Set how much Time has Elapsed since this Particle was born.
        /// <para>NOTE: Setting this to be greater than or equal to Lifetime will
        /// cause the Particle to become InActive and be removed from the Particle 
        /// System (if Lifetime is greater than zero).</para>
        /// <para>NOTE: Setting this also sets the Last Elapsed Time to the given value.</para>
        /// </summary>
        public float ElapsedTime
        {
            get { return mfElapsedTime; }

            set
            {
                // Set the new Elapsed Time of the Particle
                mfElapsedTime = value;

                // Set the Last Elapsed Time to the current Elapsed Time
                mfLastElapsedTime = mfElapsedTime;

                // If the Particle should not live forever
                if (mfLifetime > 0.0f)
                {
                    // Calculate the new Normalized Elapsed Times
                    mfNormalizedElapsedTime = mfElapsedTime / mfLifetime;
                    mfLastNormalizedElapsedTime = mfNormalizedElapsedTime;
                }
                // Else the Particle should live forever
                else
                {
                    // Reset the Normalized Elapsed Time since the Lifetime cannot be Normalized
                    mfLastNormalizedElapsedTime = 0.0f;
                    mfNormalizedElapsedTime = 0.0f;
                }
            }
        }

        /// <summary>
        /// Get / Set the Normalized Elapsed Time (0.0 - 1.0) of this Particle (How far through its life it is).
        /// <para>NOTE: Setting this to be greater than or equal to 1.0 will cause the Particle to become InActive 
        /// and be removed from the Particle System (if Lifetime is greater than zero).</para>
        /// <para>NOTE: If the Particle has a Lifetime of zero (is set to live forever), Setting this has no effect,
        /// and Getting this will always return zero.</para>
        /// </summary>
        public float NormalizedElapsedTime
        {
            get { return mfNormalizedElapsedTime; }

            set
            {
                // Set the new Normalized Elapsed Time
                mfNormalizedElapsedTime = value;

                // If the Particle should not live forever
                if (mfLifetime > 0.0f)
                {
                    // Calculate the new Elapsed Times
                    mfElapsedTime = mfLastElapsedTime = mfNormalizedElapsedTime * mfLifetime;
                    mfLastNormalizedElapsedTime = mfNormalizedElapsedTime;
                }
                // Else the Particle should live forever
                else
                {
                    // Reset the Normalized Elapsed Time since the Lifetime cannot be Normalized
                    mfLastNormalizedElapsedTime = 0.0f;
                    mfNormalizedElapsedTime = 0.0f;
                }
            }
        }

        /// <summary>
        /// Get the Elapsed Time of the Particle at the previous frame
        /// </summary>
        public float LastElapsedTime
        {
            get { return mfLastElapsedTime; }
        }

        /// <summary>
        /// Get the Normalized Elapsed Time of the Particle at the previous frame
        /// </summary>
        public float LastNormalizedElapsedTime
        {
            get { return mfLastNormalizedElapsedTime; }
        }

        /// <summary>
        /// Get / Set the Lifetime of the Particle (How long it should live for).
        /// <para>NOTE: Setting this to zero will make the Particle live forever.</para>
        /// <para>NOTE: Negative Lifetimes are reset to zero.</para>
        /// </summary>
        public float Lifetime
        {
            get { return mfLifetime; }

            set
            {
                // Set the new Lifetime of the Particle
                mfLifetime = value;

                // If the Particle should not live forever
                if (mfLifetime > 0.0f)
                {
                    // Calculate the new Normalized Elapsed Times
                    mfNormalizedElapsedTime = mfElapsedTime / mfLifetime;
                    mfLastNormalizedElapsedTime = mfNormalizedElapsedTime;
                }
                // Else the Particle should live forever
                else
                {
                    // Reset the Normalized Elapsed Time since the Lifetime cannot be Normalized
                    mfLastNormalizedElapsedTime = 0.0f;
                    mfNormalizedElapsedTime = 0.0f;

                    // Make sure the Lifetime is set to zero
                    mfLifetime = 0.0f;
                }
            }
        }

        /// <summary>
        /// Get / Set if the Particle should be Visible (i.e. be drawn) or not
        /// </summary>
        public bool Visible
        {
            get { return mbVisible; }
            set { mbVisible = value; }
        }

        #endregion

        #region Methods that should be overridden by inheriting class

        /// <summary>
        /// Resets the Particles variables to default values
        /// </summary>
        public virtual void Reset()
        {
            // Reset the Automatic Variables
            mfElapsedTime = mfLastElapsedTime = 0.0f;
            mfNormalizedElapsedTime = mfLastNormalizedElapsedTime = 0.0f;

            // Reset the Manual Variables
            mfLifetime = 0.0f;
            mbVisible = true;
        }

        /// <summary>
        /// Deep copy the ParticleToCopy's values into this Particle
        /// </summary>
        /// <param name="ParticleToCopy">The Particle whose values should be Copied</param>
        public virtual void CopyFrom(DPSFParticle ParticleToCopy)
        {
            this.Lifetime = ParticleToCopy.Lifetime;
            this.Visible = ParticleToCopy.Visible;
            this.ElapsedTime = ParticleToCopy.ElapsedTime;     // This also copies the Automatic variables' values automatically
        }

        #endregion
    }

    /// <summary>
    /// Interface that must be implemented by all Particle Vertex's
    /// </summary>
    public interface IDPSFParticleVertex
    {
        /// <summary>
        /// An array describing the Elements of each Vertex
        /// </summary>
        VertexElement[] VertexElements { get; }

        /// <summary>
        /// The Size of one Vertex Element in Bytes
        /// </summary>
        int SizeInBytes { get; }
    }

    /// <summary>
    /// Interface implemented by all Particle Systems.
    /// Variables of this type can point to any type of Particle System.
    /// </summary>
    public interface IDPSFParticleSystem
    {
        /// <summary>
        /// Event Handler that is raised when the UpdateOrder of the Particle System is changed
        /// </summary>
        event EventHandler UpdateOrderChanged;

        /// <summary>
        /// Event Handler that is raised when the DrawOrder of the Particle System is changed
        /// </summary>
        event EventHandler DrawOrderChanged;

        /// <summary>
        /// Event Handler that is raised when the Enabled status of the Particle System is changed
        /// </summary>
        event EventHandler EnabledChanged;

        /// <summary>
        /// Event Handler that is raised when the Visible status of the Particle System is changed
        /// </summary>
        event EventHandler VisibleChanged;

        /// <summary>
        /// Release all resources used by the Particle System and reset all properties to their default values
        /// </summary>
        void Destroy();

        /// <summary>
        /// Returns true if the Particle System has been Initialized, false if not
        /// </summary>
        /// <returns>Returns true if the Particle System has been Initialized, false if not</returns>
        bool IsInitialized();

        /// <summary>
        /// Get / Set if this Particle System should Draw its Particles or not.
        /// <para>NOTE: Setting this to false causes the Draw() function to not draw anything.</para>
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Get / Set if this Particle System should Update itself and its Particles or not.
        /// <para>NOTE: Setting this to false causes the Update() function to not update anything.</para>
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// The Order in which the Particle System should be Updated relative to other 
        /// DPSF Particle Systems in the same Particle System Manager. Particle Systems 
        /// are Updated in ascending order according to their Update Order (i.e. lowest first).
        /// </summary>
        int UpdateOrder { get; set; }

        /// <summary>
        /// The Order in which the Particle System should be Drawn relative to other
        /// DPSF Particle Systems in the same Particle System Manager. Particle Systems
        /// are Drawn in ascending order according to their Draw Order (i.e. lowest first)
        /// </summary>
        int DrawOrder { get; set; }

        /// <summary>
        /// Get the Game object set in the constructor, if one was given.
        /// </summary>
        Game Game { get; }

        /// <summary>
        /// Get the Graphics Device to draw to
        /// </summary>
        GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        /// Get if the Particle System is inheriting from DrawableGameComponent or not.
        /// <para>If inheriting from DrawableGameComponent, the Particle Systems
        /// are automatically added to the given Game object's Components and the
        /// Update() and Draw() functions are automatically called by the
        /// Game object when it updates and draws the rest of its Components.
        /// If the Update() and Draw() functions are called by the user anyways,
        /// they will exit without performing any operations, so it is suggested
        /// to include them anyways to make switching between inheriting and
        /// not inheriting from DrawableGameComponent seemless; just be aware
        /// that the updates and draws are actually being performed when the
        /// Game object is told to update and draw (i.e. when base.Update() and base.Draw()
        /// are called), not when these functions are being called.</para>
        /// </summary>
        bool InheritsDrawableGameComponent { get; }

        /// <summary>
        /// Get the unique ID of this Particle System.
        /// <para>NOTE: Each Particle System is automatically assigned a unique ID when it is instanciated.</para>
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Get / Set the Type of Particle System this is
        /// </summary>
        int Type { get; set; }

        /// <summary>
        /// Get the Name of the Class that this Particle System is using. This can be used to 
        /// check what type of Particle System this is at run-time.
        /// </summary>
        string ClassName { get; }

        /// <summary>
        /// Get / Set the Content Manager to use to load Textures and Effects
        /// </summary>
        ContentManager ContentManager { get; set; }

        /// <summary>
        /// The Sprite Batch drawing Settings used in the Sprite Batch's Begin() function call.
        /// <para>NOTE: These settings only have effect if this is a Sprite particle system.</para>
        /// </summary>
        SpriteBatchSettings SpriteBatchSettings { get; }

        /// <summary>
        /// The Settings used to control the Automatic Memory Manager
        /// </summary>
        AutoMemoryManagerSettings AutoMemoryManagerSettings { get; }

        /// <summary>
        /// The Emitter is used to automatically generate new Particles
        /// </summary>
        ParticleEmitter Emitter { get; set; }

        /// <summary>
        /// Get a Random object used to generate Random Numbers
        /// </summary>
        RandomNumbers RandomNumber { get; }

        /// <summary>
        /// Get / Set the World Matrix to use for drawing 3D Particles
        /// </summary>
        Matrix World { get; set; }

        /// <summary>
        /// Get / Set the View Matrix to use for drawing 3D Particles
        /// </summary>
        Matrix View { get; set; }

        /// <summary>
        /// Get / Set the Projection Matrix to use for drawing 3D Particles
        /// </summary>
        Matrix Projection { get; set; }

        /// <summary>
        /// Set the World, View, and Projection matrices for this Particle System
        /// </summary>
        /// <param name="cWorld">The World matrix</param>
        /// <param name="cView">The View matrix</param>
        /// <param name="cProjection">The Projection matrix</param>
        void SetWorldViewProjectionMatrices(Matrix cWorld, Matrix cView, Matrix cProjection);

        /// <summary>
        /// Sets the Effect and Technique to use to draw the Particles
        /// </summary>
        /// <param name="sEffect">The Asset Name of the Effect to use</param>
        /// <param name="sTechnique">The name of the Effect's Technique to use</param>
        void SetEffectAndTechnique(string sEffect, string sTechnique);

        /// <summary>
        /// Sets the Effect and Technique to use to draw the Particles
        /// </summary>
        /// <param name="cEffect">The Effect to use</param>
        /// <param name="sTechnique">The name of the Effect's Technique to use</param>
        void SetEffectAndTechnique(Effect cEffect, string sTechnique);

        /// <summary>
        /// Get / Set the Effect to use to draw the Particles
        /// </summary>
        Effect Effect { get; set; }

        /// <summary>
        /// Set which Technique of the current Effect to use to draw the Particles
        /// </summary>
        /// <param name="sTechnique">The name of the Effect's Technique to use</param>
        void SetTechnique(string sTechnique);

        /// <summary>
        /// Get / Set which Technique of the current Effect to use to draw the Particles
        /// </summary>
        EffectTechnique Technique { get; set; }

        /// <summary>
        /// Set the Texture to use to draw the Particles
        /// </summary>
        /// <param name="sTexture">The Asset Name of the texture file to use (found in
        /// the XNA Properties of the file)</param>
        void SetTexture(string sTexture);

        /// <summary>
        /// Get / Set the Texture to use to draw the Particles
        /// </summary>
        Texture2D Texture { get; set; }

        /// <summary>
        /// Get / Set how fast the Particle System Simulation should run.
        /// <para>1.0 = normal speed, 0.5 = half speed, 2.0 = double speed.</para>
        /// <para>NOTE: If a negative value is specified, the Speed Scale is set 
        /// to zero (pauses the simulation; has same effect as Enabled = false).</para>
        /// </summary>
        float SimulationSpeed { get; set; }

        /// <summary>
        /// Get / Set how fast the Particle System Simulation should run to look "normal".
        /// <para>1.0 = normal speed, 0.5 = half speed, 2.0 = double speed.</para>
        /// <para>This is provided as a way of speeding up / slowing down the simulation to have 
        /// it look as desired, without having to rescale all of the particle velocities, etc.</para>
        /// <para>NOTE: If a negative value is specified, the Internal Simulation Speed is set to zero 
        /// (pauses the simulation; has the same effect as Enabled = false).</para>
        /// </summary>
        float InternalSimulationSpeed { get; set; }

        /// <summary>
        /// Specify how often the Particle System should be Updated.
        /// <para>NOTE: Specifying a value of zero (default) will cause the Particle 
        /// System to be Updated every time the Update() function is called 
        /// (i.e. as often as possible).</para>
        /// <para>NOTE: If the Update() function is not called often enough to
        /// keep up with this specified Update rate, the Update function
        /// updates the Particle Systems as often as possible.</para>
        /// </summary>
        int UpdatesPerSecond { get; set; }

        /// <summary>
        /// The Particle System Manager whose properties (SimulationSpeed and 
        /// UpdatesPerSecond) this particle system should follow.
        /// <para>NOTE: This Particle System's properties will only clone the Manager's properties
        /// if the Manager's properties are Enabled. For example, the Manager's SimulationSpeed
        /// will only be copied to this Particle System if the Manager's SimulationSpeedIsEnabled
        /// property is true.</para>
        /// <para>NOTE: This value is automatically set to the last Particle System Manager this 
        /// Particle System is added to.</para>
        /// </summary>
        ParticleSystemManager ParticleSystemManagerToCopyPropertiesFrom { get; set; }

        /// <summary>
        /// Get the type of Particles that this Particle System should draw.
        /// </summary>
        ParticleTypes ParticleType { get; }

        /// <summary>
        /// Get / Set the absolute Number of Particles to Allocate Memory for.
        /// <para>NOTE: This value must be greater than or equal to zero.</para>
        /// <para>NOTE: Even if this many particles aren't used, the space for this many Particles 
        /// is still allocated in memory.</para>
        /// </summary>
        int NumberOfParticlesAllocatedInMemory { get; set; }

        /// <summary>
        /// Get / Set the Max Number of Particles this Particle System is Allowed to contain at any given time.
        /// <para>NOTE: The Automatic Memory Manager will never allocate space for more Particles than this.</para>
        /// <para>NOTE: This value must be greater than or equal to zero.</para>
        /// </summary>
        int MaxNumberOfParticlesAllowed { get; set; }

        /// <summary>
        /// Get the number of Particles that are currently Active
        /// </summary>
        int NumberOfActiveParticles { get; }

        /// <summary>
        /// Get the number of Particles being Drawn. That is, how many Particles 
        /// are both Active AND Visible.
        /// </summary>
        int NumberOfParticlesBeingDrawn { get; }

        /// <summary>
        /// Get the number of Particles that may still be added before reaching the
        /// Max Number Of Particles Allowed. If the Max Number Of Particles Allowed is 
        /// greater than the Number Of Particles Allocated In Memory AND the Auto Memory Manager is
        /// set to not increase the amount of Allocated Memory, than this returns the number 
        /// of Particles that may still be added before running out of Memory.
        /// </summary>
        int NumberOfParticlesStillPossibleToAdd { get; }

        /// <summary>
        /// Adds a new Particle to the particle system, at the start of the Active Particle List. 
        /// This new Particle is initialized using the particle system's Particle Initialization Function
        /// </summary>
        /// <returns>True if a particle was added, False if there is not enough memory for another Particle</returns>
        bool AddParticle();

        /// <summary>
        /// Adds the specified number of new Particles to the particle system. 
        /// These new Particles are initialized using the particle systems Particle Initialization Function
        /// </summary>
        /// <param name="iNumberOfParticlesToAdd">How many Particles to Add to the particle system</param>
        /// <returns>Returns how many Particles were able to be added to the particle system</returns>
        int AddParticles(int iNumberOfParticlesToAdd);

        /// <summary>
        /// Removes all Active Particles from the Active Particle List and adds them 
        /// to the Inactive Particle List
        /// </summary>
        void RemoveAllParticles();

        /// <summary>
        /// Updates the Particle System. This involves executing the Particle System
        /// Events, updating all Active Particles according to the Particle Events, and 
        /// adding new Particles according to the Emitter settings.
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How much time in seconds has 
        /// elapsed since the last time this function was called</param>
        void Update(float fElapsedTimeInSeconds);

        /// <summary>
        /// Updates the Particle System, even if the the Particle Systems inherits from DrawableGameComponent.
        /// <para>Updating the Particle System involves executing the Particle System Events, updating all Active 
        /// Particles according to the Particle Events, and adding new Particles according to the Emitter settings.</para>
        /// <para>NOTE: If inheriting from DrawableGameComponent and this is called, the Particle System will be updated
        /// twice per frame; once when it is called here, and again when automatically called by the Game object.
        /// If not inheriting from DrawableGameComponent, this acts the same as calling Update().</para>
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How much time in seconds has 
        /// elapsed since the last time this function was called</param>
        void UpdateForced(float fElapsedTimeInSeconds);

        /// <summary>
        /// Draws all of the Active Particles to the Graphics Device
        /// </summary>
        void Draw();

        /// <summary>
        /// Draws all of the Active Particles to the Graphics Device, even if the the Particle Systems inherits
        /// from DrawableGameComponent.
        /// <para>NOTE: If inheriting from DrawableGameComponent and this is called, the Particle System will be drawn
        /// twice per frame; once when it is called here, and again when automatically called by the Game object.
        /// If not inheriting from DrawableGameComponent, this acts the same as calling Draw().</para>
        /// </summary>
        void DrawForced();

        /// <summary>
        /// Virtual function to Initialize the Particle System with default values.
        /// </summary>
        /// <param name="cGraphicsDevice">The Graphics Device the Particle System should use</param>
        /// <param name="cContentManager">The Content Manager the Particle System should use to load resources</param>
        void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager);
    }

    /// <summary>
    /// The Type of Particles that the Particle Systems can draw. Different Particle Types are drawn in 
    /// different ways. For example, four vertices are required to draw a Quad, and only one is required 
    /// to draw a Point Sprite.
    /// </summary>
    public enum ParticleTypes
    {
        /// <summary>
        /// This is the default settings when we don't know what Type of Particles are going to be used yet.
        /// A particle system is not considered Initialized until the Particle Type does not equal this.
        /// </summary>
        None = 0,

        /// <summary>
        /// Use this when you do not want to draw your particles to the screen, as no vertex buffer will be
        /// created, saving memory. Also, the Draw() function will do nothing when this Particle Type is used.
        /// This Particle Type is useful when you just want to collect and analyze particle information without
        /// visualizing the particles.
        /// </summary>
        NoDisplay = 1,

        /// <summary>
        /// One vertex in 3D world coordinates. These particles are always only 1 pixel in size, cannot be 
        /// rotated, and do not use a Texture.
        /// </summary>
        Pixel = 2,

        /// <summary>
        /// Texture in 2D screen coordinates. Drawn using a SpriteBatch object. Only allows for 2D roll
        /// rotations, always faces the camera, and must use a Texture.
        /// </summary>
        Sprite = 3,

        /// <summary>
        /// One vertex in 3D world coordinates. Only allows for 2D roll rotations, always faces the 
        /// camera, is always a perfect square (i.e. cannot be stretched or skewed), and must use a Texture.
        /// </summary>
        PointSprite = 4,

        /// <summary>
        /// Four vertices in 3D world coordinates. Allows for rotations in all 3 dimensions, does
        /// not have to always face the camera, may be skewed into any quadrilateral, such as
        /// a square, rectangle, or trapezoid, and do not use a Texture.
        /// </summary>
        Quad = 5,

        /// <summary>
        /// Four vertices in 3D world coordinates. Allows for rotations in all 3 dimensions, does
        /// not have to always face the camera, may be skewed into any quadrilateral, such as
        /// a square, rectangle, or trapezoid, and must use a Texture.
        /// </summary>
        TexturedQuad = 6
    }

    /// <summary>
    /// Class to hold all of the Sprite Batch drawing Settings
    /// </summary>
    public class SpriteBatchSettings
    {
        /// <summary>
        /// The Blend Mode used in the SpriteBatch.Begin() function call
        /// </summary>
        public SpriteBlendMode BlendMode = SpriteBlendMode.AlphaBlend;

        /// <summary>
        /// If you plan on using a Shader (i.e. Effect and Technique) you must use
        /// Immediate mode. This is a limitation of SpriteBatch.
        /// </summary>
        public SpriteSortMode SortMode = SpriteSortMode.Immediate;

        /// <summary>
        /// The Save State Mode used in the SpriteBatch.Begin() function call
        /// </summary>
        public SaveStateMode SaveStateMode = SaveStateMode.None;

        /// <summary>
        /// The Transformation Matrix used in the SpriteBatch.Begin() function call
        /// </summary>
        public Matrix TransformationMatrix = Matrix.Identity;
    }

    /// <summary>
    /// The possible Modes the Automatic Memory Manager can be in
    /// </summary>
    public enum AutoMemoryManagerModes
    {
        /// <summary>
        /// Do not use the Automatic Memory Manager. The Number Of Particles Allocated In Memory will not be changed dynamically at run-time.
        /// This is the best option if performance is critical, as it can be expensive to allocate and release large chunks of memory at 
        /// run-time. If using this mode you should be sure that the Number Of Particles Allocated In Memory is large enough to accommodate 
        /// the particle system, but small enough that it does not waste large amounts of memory.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Allow the Automatic Memory Manager to allocate more memory when needed, and reduce it when not needed.
        /// </summary>
        IncreaseAndDecrease = 1,

        /// <summary>
        /// Only allow the Automatic Memory Manager to allocate more memory when needed (cannot reduce space).
        /// </summary>
        IncreaseOnly = 2,

        /// <summary>
        /// Only allow the Automatic Memory Manager to reduce the amount of memory allocated when it is not needed (cannot increase space).
        /// </summary>
        DecreaseOnly = 3
    }

    /// <summary>
    /// Class to hold the Automatic Memory Manager Settings
    /// </summary>
    public class AutoMemoryManagerSettings
    {
        /// <summary>
        /// The Memory Management Mode being used
        /// </summary>
        public AutoMemoryManagerModes MemoryManagementMode = AutoMemoryManagerModes.IncreaseAndDecrease;

        // Declare private variables with default values
        private int miAbsoluteMinNumberOfParticles = 10;
        private float mfReduceAmount = 1.1f;
        private float mfIncreaseAmount = 2.0f;
        private float mfSecondsMaxNumberOfParticlesMustExistForBeforeReducingSize = 3.0f;

        /// <summary>
        /// The Absolute Minimum Number Of Particles this Particle System has to have memory allocated for.
        /// The Automatic Memory Manager will never allocate space for fewer Particles than this.
        /// <para>NOTE: This value must be greater than zero.</para>
        /// <para>NOTE: Default value is 10.</para>
        /// </summary>
        public int AbsoluteMinNumberOfParticles
        {
            get { return miAbsoluteMinNumberOfParticles; }
            set
            {
                // If the specified value is valid
                if (value > 0)
                {
                    miAbsoluteMinNumberOfParticles = value;
                }
            }
        }

        /// <summary>
        /// The Automatic Memory Manager keeps track of the Max Particles that were Active in a single
        /// frame over the last X seconds (call this number M). If the Max Number Of Particles is greater
        /// than M, the Automatic Memory Manager can de-allocate unused memory. The Reduce Amount determines
        /// how much more memory than M to allocate. For example, setting the Reduce Amount to 1.0 would set
        /// the Max Number Of Particles to M. Setting the Reduce Amount to 1.1 would set the Max Number Of
        /// Particles to M + 10%. Setting it to 2.0 would set the Max Number Of Particles to M + 100% (i.e. M * 2).
        /// <para>NOTE: This value is clamped to the range 1.0 - 2.0.</para>
        /// <para>NOTE: The Automatic Memory Manager will never reduce the amount of memory to be less than
        /// what is required for the Absolute Min Number Of Particles.</para>
        /// <para>NOTE: Default value is 1.1.</para>
        /// </summary>
        public float ReduceAmount
        {
            get { return mfReduceAmount; }
            set { mfReduceAmount = MathHelper.Clamp(value, 1.0f, 2.0f); }
        }

        /// <summary>
        /// The amount the Automatic Memory Manager increases the memory allocated for Particles by.
        /// When adding a new Particle, if we discover that the Number Of Active Particles has reached
        /// the Max Number Of Particles, the Automatic Memory Manager will increase the Max Number Of
        /// Particles by the Increase Amount. For example, if the Increase Amount is set to 2.0, then 
        /// the Max Number Of Particles will be doubled (200%). If it is set to 3.0 it will be tripled 
        /// (300%). If it is set to 0.5, the Max Number Of Particles will be increased to 150%.
        /// <para>NOTE: This value is clamped to the range 1.01 - 10.0 (i.e. 101% - 1000%).</para>
        /// <para>NOTE: The Automatic Memory Manager will never increase the amount of memory to be more than
        /// what is required by the Absolute Max Number Of Particles.</para>
        /// <para>NOTE: Default value is 2.0.</para>
        /// </summary>
        public float IncreaseAmount
        {
            get { return mfIncreaseAmount; }
            set { mfIncreaseAmount = MathHelper.Clamp(value, 1.01f, 10f); }
        }

        /// <summary>
        /// The Automatic Memory Manager keeps track of the Max Particles that were Active in a single
        /// frame over the last X seconds (call this number M). If the Max Number Of Particles is greater
        /// than M, the Automatic Memory Manager can de-allocate unused memory. The Seconds Max Number Of 
        /// Particles Must Exist For Before Reducing Size tells how long M must be unchanged for before
        /// the Automatic Memory Manager can reduce the amount of allocated memory.
        /// <para>NOTE: This value must be greater than zero.</para>
        /// <para>NOTE: Default value is 3.0.</para>
        /// </summary>
        public float SecondsMaxNumberOfParticlesMustExistForBeforeReducingSize
        {
            get { return mfSecondsMaxNumberOfParticlesMustExistForBeforeReducingSize; }
            set
            {
                // If the specified value is valid
                if (value > 0.0f)
                {
                    mfSecondsMaxNumberOfParticlesMustExistForBeforeReducingSize = value;
                }
            }
        }
    }

    /// <summary>
    /// Class used to assign Unique ID's to each DPSF Particle System that is instanciated
    /// </summary>
    class ParticleSystemCounter
    {
        private static int iCount = 0;  // A static int used to keep track of how many Particle Systems have been created

        /// <summary>
        /// Returns a Unique ID for a DPSF Particle System to use
        /// </summary>
        /// <returns>Returns a Unique ID for a DPSF Particle System to use</returns>
        public static int GetUniqueID()
        {
            return iCount++;
        }
    }

    /// <summary>
    /// This is a dummy class that is used to load Content from the DPSFResources, instead
    /// of the usual method of loading it from a file. This allows us to include Content such
    /// as the compiled DPSFEffects file directly in our .dll file.
    /// </summary>
    class DummyService : IServiceProvider, IGraphicsDeviceService
    {
        public object GetService(System.Type cType) { return this; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public event EventHandler DeviceDisposing;
        public event EventHandler DeviceReset;
        public event EventHandler DeviceResetting;
        public event EventHandler DeviceCreated;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGraphicsDevice">Sets the Graphics Devie of this Dummy Service</param>
        public DummyService(GraphicsDevice cGraphicsDevice)
        {
            GraphicsDevice = cGraphicsDevice;
        }
    }

    /// <summary>
    /// The Base Particle System Framework Class.
    /// This class contains the methods and properties needed to keep track of, update, and draw Particles
    /// </summary>
    /// <typeparam name="Particle">The Particle class used to hold a particle's information. The Particle class
    /// specified must be or inherit from the DPSFParticle class</typeparam>
    /// <typeparam name="Vertex">The Particle Vertex struct used to hold a vertex's information used for drawing</typeparam>
#if (DPSFAsDrawableGameComponent)
    public class DPSF<Particle, Vertex> : DrawableGameComponent, IDPSFParticleSystem 
        where Particle : DPSFParticle, new()
        where Vertex : struct, IDPSFParticleVertex
#else
    public class DPSF<Particle, Vertex> : IDPSFParticleSystem
        where Particle : DPSFParticle, new()
        where Vertex : struct, IDPSFParticleVertex
#endif
    {
        #region Fields

        /// <summary>
        /// The function prototype that Particle System Events must follow
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How much time in seconds has elapsed since the last update</param>
        public delegate void UpdateParticleSystemDelegate(float fElapsedTimeInSeconds);

        /// <summary>
        /// The function prototype that the Particle Events must follow
        /// </summary>
        /// <param name="cParticle">The Particle to be updated</param>
        /// <param name="fElapsedTimeInSeconds">How much time in seconds has elapsed since the last update</param>
        public delegate void UpdateParticleDelegate(Particle cParticle, float fElapsedTimeInSeconds);

        /// <summary>
        /// The function prototype that the Particle Initialization Functions must follow
        /// </summary>
        /// <param name="cParticle">The Particle to be initialized</param>
        public delegate void InitializeParticleDelegate(Particle cParticle);

        /// <summary>
        /// The function prototype that the Vertex Update Functions must follow
        /// </summary>
        /// <param name="sParticleVertexBuffer">The vertex buffer array</param>
        /// <param name="iIndexInVertexBuffer">The index in the vertex buffer that the Particle properties should be written to</param>
        /// <param name="cParticle">The Particle whose properties should be copied to the vertex buffer</param>
        public delegate void UpdateVertexDelegate(ref Vertex[] sParticleVertexBuffer, int iIndexInVertexBuffer, Particle cParticle);
        
        /// <summary>
        /// Class to hold all of the Particle Events
        /// </summary>
        public class CParticleEvents
        {
            /// <summary>
            /// The Particle Event Types
            /// </summary>
            enum EParticleEventTypes
            {
                EveryTime = 0,
                OneTime = 1,
                Timed = 2,
                NormalizedTimed = 3
            }

            /// <summary>
            /// Class to hold a Particle Event's information
            /// </summary>
            class CParticleEvent
            {
                public UpdateParticleDelegate cFunctionToCall;  // The Function to call when the Event fires
                public EParticleEventTypes eType;               // The Type of Event this is
                public int iExecutionOrder;                     // The Order, relative to other Events, of when this Event should Execute (i.e. Fire)
                public int iGroup;                              // The Group that this Event belongs to

                /// <summary>
                /// Explicit Constructor
                /// </summary>
                /// <param name="_cFunctionToCall">The Function the Event should call when it Fires</param>
                /// <param name="_eType">The Type of Event this is</param>
                /// <param name="_iExecutionOrder">The Order, relative to other Events, of when this Event should be Fired</param>
                /// <param name="_iGroup">The Group this Event should belong to</param>
                public CParticleEvent(UpdateParticleDelegate _cFunctionToCall, EParticleEventTypes _eType, int _iExecutionOrder, int _iGroup)
                {
                    cFunctionToCall = _cFunctionToCall;
                    eType = _eType;
                    iExecutionOrder = _iExecutionOrder;
                    iGroup = _iGroup;
                }

                /// <summary>
                /// Overload the == operator to test for value equality
                /// </summary>
                /// <returns>Returns true if the structures have the same values, false if not</returns>
                public static bool operator ==(CParticleEvent c1, CParticleEvent c2)
                {
                    if (c1.cFunctionToCall == c2.cFunctionToCall &&
                        c1.iExecutionOrder == c2.iExecutionOrder && 
                        c1.eType == c2.eType && c1.iGroup == c2.iGroup)
                    {
                        // Return that the structures have the same values
                        return true;
                    }
                    // Return that the structures have different values
                    return false;
                }

                /// <summary>
                /// Overload the != operator to test for value equality
                /// </summary>
                /// <returns>Returns true if the structures do not have the same values, false if they do</returns>
                public static bool operator !=(CParticleEvent c1, CParticleEvent c2)
                {
                    return !(c1 == c2);
                }

                /// <summary>
                /// Override the Equals method
                /// </summary>
                public override bool Equals(object obj)
                {
                    if (!(obj is CParticleEvent))
                    {
                        return false;
                    }
                    return this == (CParticleEvent)obj;
                }

                /// <summary>
                /// Override the GetHashCode method
                /// </summary>
                public override int GetHashCode()
                {
                    return iExecutionOrder;
                }
            }

            /// <summary>
            /// Class to hold a Timed Particle Event's information
            /// </summary>
            class CTimedParticleEvent : CParticleEvent
            {
                public float fTimeToFire;   // The Time at which this Event should Fire

                /// <summary>
                /// Explicit Constructor
                /// </summary>
                /// <param name="_cFunctionToCall">The Function the Event should call when it Fires</param>
                /// <param name="_eTimedType">The Type of Timed Event this is (Timed or NormalizedTimed)</param>
                /// <param name="_iExecutionOrder">The Order, relative to other Events, of when this Event should Fire</param>
                /// <param name="_iGroup">The Group this Event should belong to</param>
                /// <param name="_fTimeToFire">The Time at which this Event should Fire</param>
                public CTimedParticleEvent(UpdateParticleDelegate _cFunctionToCall, EParticleEventTypes _eTimedType, 
                                           int _iExecutionOrder, int _iGroup, float _fTimeToFire)
                    : base(_cFunctionToCall, _eTimedType, _iExecutionOrder, _iGroup)
                {
                    fTimeToFire = _fTimeToFire;
                }

                /// <summary>
                /// Overload the == operator to test for value equality
                /// </summary>
                /// <returns>Returns true if the structures have the same values, false if not</returns>
                public static bool operator ==(CTimedParticleEvent c1, CTimedParticleEvent c2)
                {
                    if (c1.cFunctionToCall == c2.cFunctionToCall &&
                        c1.iExecutionOrder == c2.iExecutionOrder && 
                        c1.eType == c2.eType && c1.iGroup == c2.iGroup &&
                        c1.fTimeToFire == c2.fTimeToFire)
                    {
                        // Return that the structures have the same values
                        return true;
                    }
                    // Return that the structures have different values
                    return false;
                }

                /// <summary>
                /// Overload the != operator to test for value equality
                /// </summary>
                /// <returns>Returns true if the structures do not have the same values, false if they do</returns>
                public static bool operator !=(CTimedParticleEvent c1, CTimedParticleEvent c2)
                {
                    return !(c1 == c2);
                }

                /// <summary>
                /// Override the Equals method
                /// </summary>
                public override bool Equals(object obj)
                {
                    if (!(obj is CTimedParticleEvent))
                    {
                        return false;
                    }
                    return this == (CTimedParticleEvent)obj;
                }

                /// <summary>
                /// Override the GetHashCode method
                /// </summary>
                public override int GetHashCode()
                {
                    return iExecutionOrder;
                }
            }

            // Function used to sort the Events by their Execution Order
            private int EventSorter(CParticleEvent c1, CParticleEvent c2)
            {
                return c1.iExecutionOrder.CompareTo(c2.iExecutionOrder);
            }

            // List to hold all of the Events
            private List<CParticleEvent> mcParticleEventList = new List<CParticleEvent>();


            /// <summary>
            /// Adds a new EveryTime Event with a default Execution Order and Group of zero. 
            /// EveryTime Events fire every frame (i.e. every time the Update() function is called).
            /// </summary>
            /// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
            public void AddEveryTimeEvent(UpdateParticleDelegate cFunctionToCall)
            {
                // Add the new EveryTime Event
                AddEveryTimeEvent(cFunctionToCall, 0, 0);
            }

            /// <summary>
            /// Adds a new EveryTime Event with a default Group of zero. 
            /// EveryTime Events fire every frame (i.e. every time the Update() function is called).
            /// </summary>
            /// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute. 
            /// <para>NOTE: Events with lower Execution Order are executed first.</para>
            /// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
            /// order they are added.</para></param>
            public void AddEveryTimeEvent(UpdateParticleDelegate cFunctionToCall, int iExecutionOrder)
            {
                // Add the new EveryTime Event
                AddEveryTimeEvent(cFunctionToCall, iExecutionOrder, 0);
            }

            /// <summary>
            /// Adds a new EveryTime Event. 
            /// EveryTime Events fire every frame (i.e. every time the Update() function is called).
            /// </summary>
            /// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute.
            /// <para>NOTE: Events with lower Execution Order are executed first.</para>
            /// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
            /// order they are added.</para></param>
            /// <param name="iGroup">The Group that this Event should belong to</param>
            public void AddEveryTimeEvent(UpdateParticleDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
            {
                // Add the Event to the Events List and re-sort it
                mcParticleEventList.Add(new CParticleEvent(cFunctionToCall, EParticleEventTypes.EveryTime, iExecutionOrder, iGroup));
                mcParticleEventList.Sort(EventSorter);
            }

            /// <summary>
            /// Removes all EveryTime Events with the specified Function.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="cFunction">The Function of the EveryTime Event to remove</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveEveryTimeEvents(UpdateParticleDelegate cFunction)
            {
                // Remove all EveryTime Events with the specified Function
                return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
                { return (cEvent.eType == EParticleEventTypes.EveryTime && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Removes all EveryTime Events in the specified Group.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="iGroup">The Group to remove the EveryTime Events from</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveEveryTimeEvents(int iGroup)
            {
                // Remove all EveryTime Events that belong to the specified Group
                return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
                { return (cEvent.eType == EParticleEventTypes.EveryTime && cEvent.iGroup == iGroup); });
            }

            /// <summary>
            /// Removes an EveryTime Event with the specified Function, Execution Order, and Group.
            /// Returns true if the Event was found and removed, false if not.
            /// </summary>
            /// <param name="cFunction">The Function of the EveryTime Event to remove</param>
            /// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
            /// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
            /// <returns>Returns true if the Event was found and removed, false if not.</returns>
            public bool RemoveEveryTimeEvent(UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
            {
                // Remove the specified Event
                return mcParticleEventList.Remove(new CParticleEvent(cFunction, EParticleEventTypes.EveryTime, iExecutionOrder, iGroup));
            }

            /// <summary>
            /// Removes all EveryTime Events.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveAllEveryTimeEvents()
            {
                // Remove all Events with a Particle Event Type of EveryTime
                return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
                { return cEvent.eType == EParticleEventTypes.EveryTime; });
            }

            /// <summary>
            /// Returns if there is an EveryTime Event with the specified Function or not.
            /// </summary>
            /// <param name="cFunction">The Function of the EveryTime Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
            public bool ContainsEveryTimeEvent(UpdateParticleDelegate cFunction)
            {
                // Return if there is an EveryTime Event with the specified Function
                return mcParticleEventList.Exists(delegate(CParticleEvent cEvent)
                { return (cEvent.eType == EParticleEventTypes.EveryTime && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Returns if there is an EveryTime Event with the specifed Function, Execution Order, and Group or not.
            /// </summary>
            /// <param name="cFunction">The Function of the EveryTime Event to look for</param>
            /// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
            /// <param name="iGroup">The Group of the Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
            public bool ContainsEveryTimeEvent(UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
            {
                // Return if there is an EveryTime Event with the specified criteria or not
                return mcParticleEventList.Contains(new CParticleEvent(cFunction, EParticleEventTypes.EveryTime, iExecutionOrder, iGroup));
            }

            /// <summary>
            /// Adds a new OneTime Event with a default Execution Order and Group of zero. 
            /// OneTime Events fire once then are automatically removed.
            /// </summary>
            /// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
            public void AddOneTimeEvent(UpdateParticleDelegate cFunctionToCall)
            {
                // Add the new OneTime Event with default Execution Order
                AddOneTimeEvent(cFunctionToCall, 0, 0);
            }

            /// <summary>
            /// Adds a new OneTime Event with a default Group of zero. 
            /// OneTime Events fire once then are automatically removed.
            /// </summary>
            /// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute.
            /// <para>NOTE: Events with lower Execution Order are executed first.</para>
            /// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
            /// order they are added.</para></param>
            public void AddOneTimeEvent(UpdateParticleDelegate cFunctionToCall, int iExecutionOrder)
            {
                // Add the new OneTime Event with default Execution Order
                AddOneTimeEvent(cFunctionToCall, iExecutionOrder, 0);
            }

            /// <summary>
            /// Adds a new OneTime Event. 
            /// OneTime Events fire once then are automatically removed.
            /// </summary>
            /// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute.
            /// <para>NOTE: Events with lower Execution Order are executed first.</para>
            /// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
            /// order they are added.</para></param>
            /// <param name="iGroup">The Group that this Event should belong to</param>
            public void AddOneTimeEvent(UpdateParticleDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
            {
                // Add the OneTime Event to the List and re-sort the List
                mcParticleEventList.Add(new CParticleEvent(cFunctionToCall, EParticleEventTypes.OneTime, iExecutionOrder, iGroup));
                mcParticleEventList.Sort(EventSorter);
            }

            /// <summary>
            /// Removes all OneTime Events with the specified Function.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="cFunction">The Function of the OneTime Event to remove</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveOneTimeEvents(UpdateParticleDelegate cFunction)
            {
                // Remove all OneTime Events with the specified Function
                return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
                { return (cEvent.eType == EParticleEventTypes.OneTime && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Removes all OneTime Events in the specified Group.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="iGroup">The Group to remove the OneTime Events from</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveOneTimeEvents(int iGroup)
            {
                // Remove all OneTime Events that belong to the specified Group
                return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
                { return (cEvent.eType == EParticleEventTypes.OneTime && cEvent.iGroup == iGroup); });
            }

            /// <summary>
            /// Removes a OneTime Event with the specified Function To Call, Execution Order, and Group.
            /// Returns true if the Event was found and removed, false if not.
            /// </summary>
            /// <param name="cFunction">The Function of the OneTime Event to remove</param>
            /// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
            /// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
            /// <returns>Returns true if the Event was found and removed, false if not.</returns>
            public bool RemoveOneTimeEvent(UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
            {
                // Remove the specified Event from the Event List
                return mcParticleEventList.Remove(new CParticleEvent(cFunction, EParticleEventTypes.OneTime, iExecutionOrder, iGroup));
            }

            /// <summary>
            /// Removes all OneTime Events.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveAllOneTimeEvents()
            {
                // Remove all Events with a Particle Event Type of OneTime
                return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
                { return cEvent.eType == EParticleEventTypes.OneTime; });
            }

            /// <summary>
            /// Returns if there is an OneTime Event with the specified Function or not.
            /// </summary>
            /// <param name="cFunction">The Function of the OneTime Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
            public bool ContainsOneTimeEvent(UpdateParticleDelegate cFunction)
            {
                // Return if there is an OneTime Event with the specified Function
                return mcParticleEventList.Exists(delegate(CParticleEvent cEvent)
                { return (cEvent.eType == EParticleEventTypes.OneTime && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Returns if there is an OneTime Event with the specifed Function, Execution Order, and Group or not.
            /// </summary>
            /// <param name="cFunction">The Function of the OneTime Event to look for</param>
            /// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
            /// <param name="iGroup">The Group of the Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
            public bool ContainsOneTimeEvent(UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
            {
                // Return if there is an OneTime Event with the specified criteria or not
                return mcParticleEventList.Contains(new CParticleEvent(cFunction, EParticleEventTypes.OneTime, iExecutionOrder, iGroup));
            }

            /// <summary>
            /// Adds a new Timed Event with a default Execution Order and Group of zero. 
            /// Timed Events fire when the Particle's Elapsed Time reaches the specified Time To Fire.
            /// </summary>
            /// <param name="fTimeToFire">The Time when the Event should fire
            /// (i.e. when the Function should be called)</param>
            /// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
            public void AddTimedEvent(float fTimeToFire, UpdateParticleDelegate cFunctionToCall)
            {
                // Add the Timed Event with default Execution Order
                AddTimedEvent(fTimeToFire, cFunctionToCall, 0, 0);   
            }

            /// <summary>
            /// Adds a new Timed Event with a default Group of zero. 
            /// Timed Events fire when the Particle's Elapsed Time reaches the specified Time To Fire.
            /// </summary>
            /// <param name="fTimeToFire">The Time when the Event should fire
            /// (i.e. when the Function should be called)</param>
            /// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute. 
            /// <para>NOTE: Events with lower Execution Order are executed first.</para>
            /// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
            /// order they are added.</para></param>
            public void AddTimedEvent(float fTimeToFire, UpdateParticleDelegate cFunctionToCall, int iExecutionOrder)
            {
                // Add the Timed Event with default Execution Order
                AddTimedEvent(fTimeToFire, cFunctionToCall, iExecutionOrder, 0);
            }

            /// <summary>
            /// Adds a new Timed Event. 
            /// Timed Events fire when the Particle's Elapsed Time reaches the specified Time To Fire.
            /// </summary>
            /// <param name="fTimeToFire">The Time when the Event should fire
            /// (i.e. when the Function should be called)</param>
            /// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute.
            /// <para>NOTE: Events with lower Execution Order are executed first.</para>
            /// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
            /// order they are added.</para></param>
            /// <param name="iGroup">The Group that this Event should belong to</param>
            public void AddTimedEvent(float fTimeToFire, UpdateParticleDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
            {
                // Add the Timed Event to the Event List and re-sort the List
                mcParticleEventList.Add(new CTimedParticleEvent(cFunctionToCall, EParticleEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
                mcParticleEventList.Sort(EventSorter);
            }

            /// <summary>
            /// Removes all Timed Events with the specified Function.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="cFunction">The Function that is called when the Event fires</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveTimedEvents(UpdateParticleDelegate cFunction)
            {
                // Remove all Timed Events with the specified Function
                return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
                { return (cEvent.eType == EParticleEventTypes.Timed && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Removes all Timed Events in the specified Group.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="iGroup">The Group to remove the Timed Events from</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveTimedEvents(int iGroup)
            {
                // Remove all Timed Events that belong to the specified Group
                return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
                { return (cEvent.eType == EParticleEventTypes.Timed && cEvent.iGroup == iGroup); });
            }

            /// <summary>
            /// Removes a Timed Event with the specified Function, Time To Fire, Execution Order, and Group.
            /// Returns true if the Event was found and removed, false if not.
            /// </summary>
            /// <param name="fTimeToFire">The Time the Event is scheduled to fire at</param>
            /// <param name="cFunction">The Function that is called when the Event fires</param>
            /// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
            /// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
            /// <returns>Returns true if the Event was found and removed, false if not.</returns>
            public bool RemoveTimedEvent(float fTimeToFire, UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
            {
                // Remove the Timed Event from the Event List
                return mcParticleEventList.Remove(new CTimedParticleEvent(cFunction, EParticleEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
            }

            /// <summary>
            /// Removes all Timed Events.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveAllTimedEvents()
            {
                // Remove all Events with a Particle Event Type of Timed
                return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
                { return cEvent.eType == EParticleEventTypes.Timed; });
            }

            /// <summary>
            /// Returns if there is a Timed Event with the specified Function or not.
            /// </summary>
            /// <param name="cFunction">The Function of the Timed Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
            public bool ContainsTimedEvent(UpdateParticleDelegate cFunction)
            {
                // Return if there is a Timed Event with the specified Function
                return mcParticleEventList.Exists(delegate(CParticleEvent cEvent)
                { return (cEvent.eType == EParticleEventTypes.Timed && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Returns if there is a Timed Event with the specifed Timed To Fire, Function, Execution Order, and Group or not.
            /// </summary>
            /// <param name="fTimeToFire">The Time the Event is scheduled to fire at</param>
            /// <param name="cFunction">The Function of the Timed Event to look for</param>
            /// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
            /// <param name="iGroup">The Group of the Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
            public bool ContainsTimedEvent(float fTimeToFire, UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
            {
                // Return if there is a Timed Event with the specified criteria or not
                return mcParticleEventList.Contains(new CTimedParticleEvent(cFunction, EParticleEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
            }

            /// <summary>
            /// Adds a new Normalized Timed Event with a default Execution Order and Group of zero. 
            /// Normalized Timed Events fire when the Particle's Normalized Elapsed Time reaches the specified Time To Fire.
            /// </summary>
            /// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire. 
            /// <para>NOTE: This is clamped to the range of 0.0 - 1.0.</para></param>
            /// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
            public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleDelegate cFunctionToCall)
            {
                // Add the Normalized Timed Event with default Execution Order
                AddNormalizedTimedEvent(fNormalizedTimeToFire, cFunctionToCall, 0, 0);                
            }

            /// <summary>
            /// Adds a new Normalized Timed Event with a default Group of zero. 
            /// Normalized Timed Events fire when the Particle's Normalized Elapsed Time reaches the specified Time To Fire.
            /// </summary>
            /// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire. 
            /// <para>NOTE: This is clamped to the range of 0.0 - 1.0.</para></param>
            /// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute. NOTE: Events with lower Execution Order are executed first. NOTE: Events
            /// with the same Execution Order are not guaranteed to be executed in the order they are added.</param>
            public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleDelegate cFunctionToCall, int iExecutionOrder)
            {
                // Add the Normalized Timed Event with default Execution Order
                AddNormalizedTimedEvent(fNormalizedTimeToFire, cFunctionToCall, iExecutionOrder, 0);
            }

            /// <summary>
            /// Adds a new Normalized Timed Event. 
            /// Normalized Timed Events fire when the Particle's Normalized Elapsed Time reaches the specified Time To Fire.
            /// </summary>
            /// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire
            /// (compared against the Particle's Normalized Elapsed Time). NOTE: This is clamped to the range of 0.0 - 1.0</param>
            /// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute.
            /// <para>NOTE: Events with lower Execution Order are executed first.</para>
            /// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
            /// order they are added.</para></param>
            /// <param name="iGroup">The Group that this Event should belong to</param>
            public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
            {
                // Clamp the NormalizedTimeToFire between 0.0 and 1.0
                fNormalizedTimeToFire = MathHelper.Clamp(fNormalizedTimeToFire, 0.0f, 1.0f);

                // Add the Normalized Timed Event to the Event List and re-sort the List
                mcParticleEventList.Add(new CTimedParticleEvent(cFunctionToCall, EParticleEventTypes.NormalizedTimed, iExecutionOrder, iGroup, fNormalizedTimeToFire));
                mcParticleEventList.Sort(EventSorter);
            }

            /// <summary>
            /// Removes all Normalized Timed Events with the specified Function.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="cFunction">The Function that is called when the Event fires</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveNormalizedTimedEvents(UpdateParticleDelegate cFunction)
            {
                // Remove all Normalized Timed Events with the specified Function
                return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
                { return (cEvent.eType == EParticleEventTypes.NormalizedTimed && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Removes all Normalized Timed Events in the specified Group.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="iGroup">The Group to remove the Normalized Timed Events from</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveNormalizedTimedEvents(int iGroup)
            {
                // Remove all Normalized Timed Events that belong to the specified Group
                return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
                { return (cEvent.eType == EParticleEventTypes.NormalizedTimed && cEvent.iGroup == iGroup); });
            }

            /// <summary>
            /// Removes a Normalized Timed Event with the specified Function, Time To Fire, Execution Order, and Group.
            /// Returns true if the Event was found and removed, false if not.
            /// </summary>
            /// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) the Event is scheduled to fire at</param>
            /// <param name="cFunction">The Function that is called when the Event fires</param>
            /// <param name="iExectionOrder">The Execution Order of the Event to remove. Default is zero.</param>
            /// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
            /// <returns>Returns true if the Event was found and removed, false if not.</returns>
            public bool RemoveNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleDelegate cFunction, int iExectionOrder, int iGroup)
            {
                // Remove the Normalized Timed Event from the Event List
                return mcParticleEventList.Remove(new CTimedParticleEvent(cFunction, EParticleEventTypes.NormalizedTimed, iExectionOrder, iGroup, fNormalizedTimeToFire));
            }

            /// <summary>
            /// Removes all Normalized Timed Events.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveAllNormalizedTimedEvents()
            {
                // Remove all Events with a Particle Event Type of NormalizedTimed
                return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
                { return cEvent.eType == EParticleEventTypes.NormalizedTimed; });
            }

            /// <summary>
            /// Returns if there is a NormalizedTimed Event with the specified Function or not.
            /// </summary>
            /// <param name="cFunction">The Function of the NormalizedTimed Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
            public bool ContainsNormalizedTimedEvent(UpdateParticleDelegate cFunction)
            {
                // Return if there is a NormalizedTimed Event with the specified Function
                return mcParticleEventList.Exists(delegate(CParticleEvent cEvent)
                { return (cEvent.eType == EParticleEventTypes.NormalizedTimed && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Returns if there is a NormalizedTimed Event with the specifed Normalized Time To Fire, Function, Execution Order, and Group or not.
            /// </summary>
            /// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) the Event is scheduled to fire at</param>
            /// <param name="cFunction">The Function of the NormalizedTimed Event to look for</param>
            /// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
            /// <param name="iGroup">The Group of the Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
            public bool ContainsNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
            {
                // Return if there is a NormalizedTimed Event with the specified criteria or not
                return mcParticleEventList.Contains(new CTimedParticleEvent(cFunction, EParticleEventTypes.NormalizedTimed, iExecutionOrder, iGroup, fNormalizedTimeToFire));
            }

            /// <summary>
            /// Removes all Timed Events and Normalized Timed Events.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveAllTimedAndNormalizedTimedEvents()
            {
                return (RemoveAllTimedEvents() + RemoveAllNormalizedTimedEvents());
            }

            /// <summary>
            /// Removes all Events.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveAllEvents()
            {
                // Store the Number of Events that were in the Event List
                int iNumberOfEvents = mcParticleEventList.Count;

                // Remove all Events from the Event List
                mcParticleEventList.Clear();

                // Return the Number of Events removed
                return iNumberOfEvents;
            }

            /// <summary>
            /// Removes all Events in the specified Group.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="iGroup">The Group to remove all Events from</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveAllEventsInGroup(int iGroup)
            {
                // Remove all Events that belong to the specified Group
                return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
                { return (cEvent.iGroup == iGroup); });
            }

            /// <summary>
            /// Updates the given Particle according to the Particle Events. This is called automatically
            /// every frame by the Particle System.
            /// </summary>
            /// <param name="cParticle">The Particle to update</param>
            /// <param name="fElapsedTimeInSeconds">The amount of Time Elapsed since the last Update</param>
            public void Update(Particle cParticle, float fElapsedTimeInSeconds)
            {
                // If there are no Events to process
                if (mcParticleEventList.Count <= 0)
                {
                    // Exit without doing anything
                    return;
                }

                // Loop through all of the Events (which are pre-sorted by Execution Order).
                // We check against mcParticleEventList.Count instead of storing the value in 
                // a local variable incase an Event removes other Events (may throw an 
                // index out of range exception if this happens).
                for (int iIndex = 0; iIndex < mcParticleEventList.Count; iIndex++)
                {
                    // If this is an EveryTime or OneTime Event
                    if (mcParticleEventList[iIndex].eType == EParticleEventTypes.EveryTime ||
                        mcParticleEventList[iIndex].eType == EParticleEventTypes.OneTime)
                    {
                        // Execute the Event's function
                        mcParticleEventList[iIndex].cFunctionToCall(cParticle, fElapsedTimeInSeconds);
                    }
                    // Else this is a Timed Event
                    else
                    {
                        // Get a handle to the Timed Event
                        CTimedParticleEvent cTimedEvent = (CTimedParticleEvent)mcParticleEventList[iIndex];

                        // If this is a normal Timed Event
                        if (cTimedEvent.eType == EParticleEventTypes.Timed)
                        {
                            // Store the Last Elapsed Time of the Particle. 
                            // If it's zero we need to change it to be before zero so that any Events set to fire at time zero will be fired.
                            float fLastElapsedTime = (cParticle.LastElapsedTime == 0.0f) ? -0.1f : cParticle.LastElapsedTime;

                            // If this Event should fire
                            if (fLastElapsedTime < cTimedEvent.fTimeToFire &&
                                cParticle.ElapsedTime >= cTimedEvent.fTimeToFire)
                            {
                                // Execute the Event's function
                                cTimedEvent.cFunctionToCall(cParticle, fElapsedTimeInSeconds);
                            }
                        }
                        // Else this is a Normalized Timed Event
                        else
                        {
                            // Store the Last Normalized Elapsed Time of the Particle. 
                            // If it's zero we need to change it to be before zero so that any Events set to fire at time zero will be fired.
                            float fLastNormalizedElapsedTime = (cParticle.LastNormalizedElapsedTime == 0.0f) ? -0.1f : cParticle.LastNormalizedElapsedTime;

                            // If this Event should fire
                            if (fLastNormalizedElapsedTime < cTimedEvent.fTimeToFire &&
                                cParticle.NormalizedElapsedTime >= cTimedEvent.fTimeToFire)
                            {
                                // Fire the Event
                                cTimedEvent.cFunctionToCall(cParticle, fElapsedTimeInSeconds);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Class to hold all of the Particle System Events and related info
        /// </summary>
        public class CParticleSystemEvents
        {
            /// <summary>
            /// The Particle System Event Types
            /// </summary>
            enum EParticleSystemEventTypes
            {
                EveryTime = 0,
                OneTime = 1,
                Timed = 2,
                NormalizedTimed = 3
            }

            /// <summary>
            /// Class to hold a Particle System Event's information
            /// </summary>
            class CParticleSystemEvent
            {
                public UpdateParticleSystemDelegate cFunctionToCall;    // The Function to call when the Event fires
                public EParticleSystemEventTypes eType;                 // The Type of Event this is
                public int iExecutionOrder;                             // The Order, relative to other Events, of when this Event should Execute (i.e. Fire)
                public int iGroup;                                      // The Group the Event belongs to

                /// <summary>
                /// Explicit Constructor
                /// </summary>
                /// <param name="_cFunctionToCall">The Function the Event should call when it Fires</param>
                /// <param name="_eType">The Type of Event this is</param>
                /// <param name="_iExecutionOrder">The Order, relative to other Events, of when this Event should be Fired</param>
                /// <param name="_iGroup">The Group this Event should belong to</param>
                public CParticleSystemEvent(UpdateParticleSystemDelegate _cFunctionToCall, EParticleSystemEventTypes _eType, int _iExecutionOrder, int _iGroup)
                {
                    cFunctionToCall = _cFunctionToCall;
                    eType = _eType;
                    iExecutionOrder = _iExecutionOrder;
                    iGroup = _iGroup;
                }

                /// <summary>
                /// Overload the == operator to test for value equality
                /// </summary>
                /// <returns>Returns true if the structures have the same values, false if not</returns>
                public static bool operator ==(CParticleSystemEvent c1, CParticleSystemEvent c2)
                {
                    if (c1.cFunctionToCall == c2.cFunctionToCall &&
                        c1.iExecutionOrder == c2.iExecutionOrder &&
                        c1.eType == c2.eType && c1.iGroup == c2.iGroup)
                    {
                        // Return that the structures have the same values
                        return true;
                    }
                    // Return that the structures have different values
                    return false;
                }

                /// <summary>
                /// Overload the != operator to test for value equality
                /// </summary>
                /// <returns>Returns true if the structures do not have the same values, false if they do</returns>
                public static bool operator !=(CParticleSystemEvent c1, CParticleSystemEvent c2)
                {
                    return !(c1 == c2);
                }

                /// <summary>
                /// Override the Equals method
                /// </summary>
                public override bool Equals(object obj)
                {
                    if (!(obj is CParticleSystemEvent))
                    {
                        return false;
                    }
                    return this == (CParticleSystemEvent)obj;
                }

                /// <summary>
                /// Override the GetHashCode method
                /// </summary>
                public override int GetHashCode()
                {
                    return iExecutionOrder;
                }
            }

            /// <summary>
            /// Class to hold a Timed Particle System Event's information
            /// </summary>
            class CTimedParticleSystemEvent : CParticleSystemEvent
            {
                public float fTimeToFire;   // The Time at which this Event should Fire

                /// <summary>
                /// Explicit Constructor
                /// </summary>
                /// <param name="_cFunctionToCall">The Function the Event should call when it Fires</param>
                /// <param name="_eTimedType">The Type of Timed Event this is (Timed or NormalizedTimed)</param>
                /// <param name="_iExecutionOrder">The Order, relative to other Events, of when this Event should Fire</param>
                /// <param name="_iGroup">The Group this Event should belong to</param>
                /// <param name="_fTimeToFire">The Time at which this Event should Fire</param>
                public CTimedParticleSystemEvent(UpdateParticleSystemDelegate _cFunctionToCall, EParticleSystemEventTypes _eTimedType,
                                                 int _iExecutionOrder, int _iGroup, float _fTimeToFire)
                    : base(_cFunctionToCall, _eTimedType, _iExecutionOrder, _iGroup)
                {
                    fTimeToFire = _fTimeToFire;
                }

                /// <summary>
                /// Overload the == operator to test for value equality
                /// </summary>
                /// <returns>Returns true if the structures have the same values, false if not</returns>
                public static bool operator ==(CTimedParticleSystemEvent c1, CTimedParticleSystemEvent c2)
                {
                    if (c1.cFunctionToCall == c2.cFunctionToCall &&
                        c1.iExecutionOrder == c2.iExecutionOrder &&
                        c1.eType == c2.eType && c1.iGroup == c2.iGroup &&
                        c1.fTimeToFire == c2.fTimeToFire)
                    {
                        // Return that the structures have the same values
                        return true;
                    }
                    // Return that the structures have different values
                    return false;
                }

                /// <summary>
                /// Overload the != operator to test for value equality
                /// </summary>
                /// <returns>Returns true if the structures do not have the same values, false if they do</returns>
                public static bool operator !=(CTimedParticleSystemEvent c1, CTimedParticleSystemEvent c2)
                {
                    return !(c1 == c2);
                }

                /// <summary>
                /// Override the Equals method
                /// </summary>
                public override bool Equals(object obj)
                {
                    if (!(obj is CTimedParticleSystemEvent))
                    {
                        return false;
                    }
                    return this == (CTimedParticleSystemEvent)obj;
                }

                /// <summary>
                /// Override the GetHashCode method
                /// </summary>
                public override int GetHashCode()
                {
                    return iExecutionOrder;
                }
            }

            /// <summary>
            /// The Options of what should happen when the Particle System reaches the end of its Lifetime
            /// </summary>
            public enum EParticleSystemEndOfLifeOptions
            {
                /// <summary>
                /// When the Particle System reaches the end of its Lifetime nothing special happens; It just
                /// continues to operate as normal.
                /// </summary>
                Nothing = 0,

                /// <summary>
                /// When the Particle System reaches the end of its Lifetime its Elapsed Time is reset to zero,
                /// so that all of the Timed Events will be repeated again.
                /// </summary>
                Repeat = 1,

                /// <summary>
                /// When the Particle System reaches the end of its Lifetime it calls its Destroy() function, so
                /// the Particle System releases all its resources and is no longer updated or drawn.
                /// </summary>
                Destroy = 2
            }

            /// <summary>
            /// Class to hold the Lifetime information of the Particle System
            /// </summary>
            public class CParticleSystemLifetimeData : DPSFParticle
            {
                private EParticleSystemEndOfLifeOptions meEndOfLifeOption;  // Tells what should happen when the Particle System reaches its Lifetime

                /// <summary>
                /// Get / Set what should happen when the Particle System reaches the end of its Lifetime
                /// </summary>
                public EParticleSystemEndOfLifeOptions EndOfLifeOption
                {
                    get { return meEndOfLifeOption; }
                    set { meEndOfLifeOption = value; }
                }

                /// <summary>
                /// Resets the class variables to their default values
                /// </summary>
                public override void Reset()
                {
                    base.Reset();
                    EndOfLifeOption = EParticleSystemEndOfLifeOptions.Nothing;
                }

                /// <summary>
                /// Deep copy the ParticleToCopy's values into this Particle
                /// </summary>
                /// <param name="ParticleToCopy">The Particle whose values should be Copied</param>
                public override void CopyFrom(DPSFParticle ParticleToCopy)
                {
                    CParticleSystemLifetimeData cParticleToCopy = (CParticleSystemLifetimeData)ParticleToCopy;
                    base.CopyFrom(cParticleToCopy);
                    EndOfLifeOption = cParticleToCopy.EndOfLifeOption;
                }
            }

            // Function used to sort the Events by their Execution Order
            private int EventSorter(CParticleSystemEvent c1, CParticleSystemEvent c2)
            {
                return c1.iExecutionOrder.CompareTo(c2.iExecutionOrder);
            }

            // List to hold all of the Events
            private List<CParticleSystemEvent> mcParticleSystemEventList = new List<CParticleSystemEvent>();

            // Variable to hold the Lifetime Data of the Particle System
            private CParticleSystemLifetimeData mcParticleSystemLifetimeData = new CParticleSystemLifetimeData();

            /// <summary>
            /// Get / Set the Lifetime information of the Particle System
            /// </summary>
            public CParticleSystemLifetimeData LifetimeData
            {
                get { return mcParticleSystemLifetimeData; }
                set { mcParticleSystemLifetimeData = value; }
            }

            /// <summary>
            /// Adds a new EveryTime Event with a default Execution Order and Group of zero. 
            /// EveryTime Events fire every frame (i.e. every time the Update() function is called).
            /// </summary>
            /// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
            public void AddEveryTimeEvent(UpdateParticleSystemDelegate cFunctionToCall)
            {
                // Add the new EveryTime Event
                AddEveryTimeEvent(cFunctionToCall, 0, 0);
            }

            /// <summary>
            /// Adds a new EveryTime Event with a default Group of zero. 
            /// EveryTime Events fire every frame (i.e. every time the Update() function is called).
            /// </summary>
            /// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute.
            /// <para>NOTE: Events with lower Execution Order are executed first.</para>
            /// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
            /// the order they are added.</para></param>
            public void AddEveryTimeEvent(UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder)
            {
                // Add the new EveryTime Event
                AddEveryTimeEvent(cFunctionToCall, iExecutionOrder, 0);
            }

            /// <summary>
            /// Adds a new EveryTime Event. 
            /// EveryTime Events fire every frame (i.e. every time the Update() function is called).
            /// </summary>
            /// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute.
            /// <para>NOTE: Events with lower Execution Order are executed first.</para>
            /// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
            /// the order they are added.</para></param>
            /// <param name="iGroup">The Group that this Event should belong to</param>
            public void AddEveryTimeEvent(UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
            {
                // Add the Event to the Events List and re-sort it
                mcParticleSystemEventList.Add(new CParticleSystemEvent(cFunctionToCall, EParticleSystemEventTypes.EveryTime, iExecutionOrder, iGroup));
                mcParticleSystemEventList.Sort(EventSorter);
            }

            /// <summary>
            /// Removes all EveryTime Events with the specified Function.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="cFunction">The Function of the EveryTime Event to remove</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveEveryTimeEvents(UpdateParticleSystemDelegate cFunction)
            {
                // Remove all EveryTime Events with the specified Function
                return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
                { return (cEvent.eType == EParticleSystemEventTypes.EveryTime && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Removes all EveryTime Events in the specified Group.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="iGroup">The Group to remove the EveryTime Events from</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveEveryTimeEvents(int iGroup)
            {
                // Remove all EveryTime Events that belong to the specified Group
                return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
                { return (cEvent.eType == EParticleSystemEventTypes.EveryTime && cEvent.iGroup == iGroup); });
            }

            /// <summary>
            /// Removes an EveryTime Event with the specified Function, Execution Order, and Group.
            /// Returns true if the Event was found and removed, false if not.
            /// </summary>
            /// <param name="cFunction">The Function of the EveryTime Event to remove</param>
            /// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
            /// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
            /// <returns>Returns true if the Event was found and removed, false if not.</returns>
            public bool RemoveEveryTimeEvent(UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
            {
                // Remove the specified Event
                return mcParticleSystemEventList.Remove(new CParticleSystemEvent(cFunction, EParticleSystemEventTypes.EveryTime, iExecutionOrder, iGroup));
            }

            /// <summary>
            /// Removes all EveryTime Events.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveAllEveryTimeEvents()
            {
                // Remove all Events with a Particle Event Type of EveryTime
                return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
                { return cEvent.eType == EParticleSystemEventTypes.EveryTime; });
            }

            /// <summary>
            /// Returns if there is an EveryTime Event with the specified Function or not.
            /// </summary>
            /// <param name="cFunction">The Function of the EveryTime Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
            public bool ContainsEveryTimeEvent(UpdateParticleSystemDelegate cFunction)
            {
                // Return if there is an EveryTime Event with the specified Function
                return mcParticleSystemEventList.Exists(delegate(CParticleSystemEvent cEvent)
                { return (cEvent.eType == EParticleSystemEventTypes.EveryTime && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Returns if there is an EveryTime Event with the specifed Function, Execution Order, and Group or not.
            /// </summary>
            /// <param name="cFunction">The Function of the EveryTime Event to look for</param>
            /// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
            /// <param name="iGroup">The Group of the Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
            public bool ContainsEveryTimeEvent(UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
            {
                // Return if there is an EveryTime Event with the specified criteria or not
                return mcParticleSystemEventList.Contains(new CParticleSystemEvent(cFunction, EParticleSystemEventTypes.EveryTime, iExecutionOrder, iGroup));
            }            

            /// <summary>
            /// Adds a new OneTime Event with a default Execution Order and Group of zero. 
            /// OneTime Events fire once then are automatically removed.
            /// </summary>
            /// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
            public void AddOneTimeEvent(UpdateParticleSystemDelegate cFunctionToCall)
            {
                // Add the new OneTime Event with default Execution Order
                AddOneTimeEvent(cFunctionToCall, 0, 0);
            }

            /// <summary>
            /// Adds a new OneTime Event with a default Group of zero. 
            /// OneTime Events fire once then are automatically removed.
            /// </summary>
            /// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute.
            /// <para>NOTE: Events with lower Execution Order are executed first.</para>
            /// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
            /// the order they are added.</para></param>
            public void AddOneTimeEvent(UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder)
            {
                // Add the new OneTime Event with default Execution Order
                AddOneTimeEvent(cFunctionToCall, iExecutionOrder, 0);
            }

            /// <summary>
            /// Adds a new OneTime Event. 
            /// OneTime Events fire once then are automatically removed.
            /// </summary>
            /// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute.
            /// <para>NOTE: Events with lower Execution Order are executed first.</para>
            /// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
            /// the order they are added.</para></param>
            /// <param name="iGroup">The Group that this Event should belong to</param>
            public void AddOneTimeEvent(UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
            {
                // Add the OneTime Event to the List and re-sort the List
                mcParticleSystemEventList.Add(new CParticleSystemEvent(cFunctionToCall, EParticleSystemEventTypes.OneTime, iExecutionOrder, iGroup));
                mcParticleSystemEventList.Sort(EventSorter);
            }

            /// <summary>
            /// Removes all OneTime Events with the specified Function.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="cFunction">The Function of the OneTime Event to remove</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveOneTimeEvents(UpdateParticleSystemDelegate cFunction)
            {
                // Remove all OneTime Events with the specified Function
                return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
                { return (cEvent.eType == EParticleSystemEventTypes.OneTime && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Removes all OneTime Events in the specified Group.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="iGroup">The Group to remove the OneTime Events from</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveOneTimeEvents(int iGroup)
            {
                // Remove all OneTime Events that belong to the specified Group
                return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
                { return (cEvent.eType == EParticleSystemEventTypes.OneTime && cEvent.iGroup == iGroup); });
            }

            /// <summary>
            /// Removes a OneTime Event with the specified Function To Call, Execution Order, and Group.
            /// Returns true if the Event was found and removed, false if not.
            /// </summary>
            /// <param name="cFunction">The Function of the OneTime Event to remove</param>
            /// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
            /// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
            /// <returns>Returns true if the Event was found and removed, false if not.</returns>
            public bool RemoveOneTimeEvent(UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
            {
                // Remove the specified Event from the Event List
                return mcParticleSystemEventList.Remove(new CParticleSystemEvent(cFunction, EParticleSystemEventTypes.OneTime, iExecutionOrder, iGroup));
            }

            /// <summary>
            /// Removes all OneTime Events.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveAllOneTimeEvents()
            {
                // Remove all Events with a Particle Event Type of OneTime
                return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
                { return cEvent.eType == EParticleSystemEventTypes.OneTime; });
            }

            /// <summary>
            /// Returns if there is an OneTime Event with the specified Function or not.
            /// </summary>
            /// <param name="cFunction">The Function of the OneTime Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
            public bool ContainsOneTimeEvent(UpdateParticleSystemDelegate cFunction)
            {
                // Return if there is an OneTime Event with the specified Function
                return mcParticleSystemEventList.Exists(delegate(CParticleSystemEvent cEvent)
                { return (cEvent.eType == EParticleSystemEventTypes.OneTime && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Returns if there is an OneTime Event with the specifed Function, Execution Order, and Group or not.
            /// </summary>
            /// <param name="cFunction">The Function of the OneTime Event to look for</param>
            /// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
            /// <param name="iGroup">The Group of the Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
            public bool ContainsOneTimeEvent(UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
            {
                // Return if there is an OneTime Event with the specified criteria or not
                return mcParticleSystemEventList.Contains(new CParticleSystemEvent(cFunction, EParticleSystemEventTypes.OneTime, iExecutionOrder, iGroup));
            }

            /// <summary>
            /// Adds a new Timed Event with a default Execution Order and Group of zero. 
            /// Timed Events fire when the Particle's Elapsed Time reaches the specified Time To Fire.
            /// </summary>
            /// <param name="fTimeToFire">The Time when the Event should fire
            /// (i.e. when the Function should be called)</param>
            /// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
            public void AddTimedEvent(float fTimeToFire, UpdateParticleSystemDelegate cFunctionToCall)
            {
                // Add the Timed Event with default Execution Order
                AddTimedEvent(fTimeToFire, cFunctionToCall, 0, 0);
            }

            /// <summary>
            /// Adds a new Timed Event with a default Group of zero. 
            /// Timed Events fire when the Particle's Elapsed Time reaches the specified Time To Fire.
            /// </summary>
            /// <param name="fTimeToFire">The Time when the Event should fire
            /// (i.e. when the Function should be called)</param>
            /// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute.
            /// <para>NOTE: Events with lower Execution Order are executed first.</para>
            /// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
            /// the order they are added.</para></param>
            public void AddTimedEvent(float fTimeToFire, UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder)
            {
                // Add the Timed Event with default Execution Order
                AddTimedEvent(fTimeToFire, cFunctionToCall, iExecutionOrder, 0);
            }

            /// <summary>
            /// Adds a new Timed Event. 
            /// Timed Events fire when the Particle's Elapsed Time reaches the specified Time To Fire.
            /// </summary>
            /// <param name="fTimeToFire">The Time when the Event should fire
            /// (i.e. when the Function should be called)</param>
            /// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute.
            /// <para>NOTE: Events with lower Execution Order are executed first.</para>
            /// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
            /// the order they are added.</para></param>
            /// <param name="iGroup">The Group that this Event should belong to</param>
            public void AddTimedEvent(float fTimeToFire, UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
            {
                // Add the Timed Event to the Event List and re-sort the List
                mcParticleSystemEventList.Add(new CTimedParticleSystemEvent(cFunctionToCall, EParticleSystemEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
                mcParticleSystemEventList.Sort(EventSorter);
            }

            /// <summary>
            /// Removes all Timed Events with the specified Function.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="cFunction">The Function that is called when the Event fires</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveTimedEvents(UpdateParticleSystemDelegate cFunction)
            {
                // Remove all Timed Events with the specified Function
                return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
                { return (cEvent.eType == EParticleSystemEventTypes.Timed && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Removes all Timed Events in the specified Group.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="iGroup">The Group to remove the Timed Events from</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveTimedEvents(int iGroup)
            {
                // Remove all Timed Events that belong to the specified Group
                return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
                { return (cEvent.eType == EParticleSystemEventTypes.Timed && cEvent.iGroup == iGroup); });
            }

            /// <summary>
            /// Removes a Timed Event with the specified Function, Time To Fire, Execution Order, and Group.
            /// Returns true if the Event was found and removed, false if not.
            /// </summary>
            /// <param name="fTimeToFire">The Time the Event is scheduled to fire at</param>
            /// <param name="cFunction">The Function that is called when the Event fires</param>
            /// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
            /// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
            /// <returns>Returns true if the Event was found and removed, false if not.</returns>
            public bool RemoveTimedEvent(float fTimeToFire, UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
            {
                // Remove the Timed Event from the Event List
                return mcParticleSystemEventList.Remove(new CTimedParticleSystemEvent(cFunction, EParticleSystemEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
            }

            /// <summary>
            /// Removes all Timed Events.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveAllTimedEvents()
            {
                // Remove all Events with a Particle Event Type of Timed
                return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
                { return cEvent.eType == EParticleSystemEventTypes.Timed; });
            }

            /// <summary>
            /// Returns if there is a Timed Event with the specified Function or not.
            /// </summary>
            /// <param name="cFunction">The Function of the Timed Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
            public bool ContainsTimedEvent(UpdateParticleSystemDelegate cFunction)
            {
                // Return if there is a Timed Event with the specified Function
                return mcParticleSystemEventList.Exists(delegate(CParticleSystemEvent cEvent)
                { return (cEvent.eType == EParticleSystemEventTypes.Timed && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Returns if there is a Timed Event with the specifed Timed To Fire, Function, Execution Order, and Group or not.
            /// </summary>
            /// <param name="fTimeToFire">The Time the Event is scheduled to fire at</param>
            /// <param name="cFunction">The Function of the Timed Event to look for</param>
            /// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
            /// <param name="iGroup">The Group of the Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
            public bool ContainsTimedEvent(float fTimeToFire, UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
            {
                // Return if there is a Timed Event with the specified criteria or not
                return mcParticleSystemEventList.Contains(new CTimedParticleSystemEvent(cFunction, EParticleSystemEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
            }

            /// <summary>
            /// Adds a new Normalized Timed Event with a default Execution Order and Group of zero. 
            /// Normalized Timed Events fire when the Particle's Normalized Elapsed Time reaches the specified Time To Fire.
            /// </summary>
            /// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire. 
            /// <para>NOTE: This is clamped to the range of 0.0 - 1.0.</para></param>
            /// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
            public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleSystemDelegate cFunctionToCall)
            {
                // Add the Normalized Timed Event with default Execution Order
                AddNormalizedTimedEvent(fNormalizedTimeToFire, cFunctionToCall, 0, 0);
            }

            /// <summary>
            /// Adds a new Normalized Timed Event with a default Group of zero. 
            /// Normalized Timed Events fire when the Particle's Normalized Elapsed Time reaches the specified Time To Fire.
            /// </summary>
            /// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire. 
            /// <para>NOTE: This is clamped to the range of 0.0 - 1.0.</para></param>
            /// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute. NOTE: Events with lower Execution Order are executed first. NOTE: Events
            /// with the same Execution Order are not guaranteed to be executed in the order they are added.</param>
            public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder)
            {
                // Add the Normalized Timed Event with default Execution Order
                AddNormalizedTimedEvent(fNormalizedTimeToFire, cFunctionToCall, iExecutionOrder, 0);
            }

            /// <summary>
            /// Adds a new Normalized Timed Event. 
            /// Normalized Timed Events fire when the Particle's Normalized Elapsed Time reaches the specified Time To Fire.
            /// </summary>
            /// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire
            /// (compared against the Particle's Normalized Elapsed Time). NOTE: This is clamped to the range of 0.0 - 1.0</param>
            /// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
            /// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
            /// should Execute.
            /// <para>NOTE: Events with lower Execution Order are executed first.</para>
            /// NOTE: Events with the same Execution Order are not guaranteed to be executed in 
            /// the order they are added.</para></param>
            /// <param name="iGroup">The Group that this Event should belong to</param>
            public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
            {
                // Clamp the NormalizedTimeToFire between 0.0 and 1.0
                fNormalizedTimeToFire = MathHelper.Clamp(fNormalizedTimeToFire, 0.0f, 1.0f);

                // Add the Normalized Timed Event to the Event List and re-sort the List
                mcParticleSystemEventList.Add(new CTimedParticleSystemEvent(cFunctionToCall, EParticleSystemEventTypes.NormalizedTimed, iExecutionOrder, iGroup, fNormalizedTimeToFire));
                mcParticleSystemEventList.Sort(EventSorter);
            }

            /// <summary>
            /// Removes all Normalized Timed Events with the specified Function.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="cFunction">The Function that is called when the Event fires</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveNormalizedTimedEvents(UpdateParticleSystemDelegate cFunction)
            {
                // Remove all Normalized Timed Events with the specified Function
                return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
                { return (cEvent.eType == EParticleSystemEventTypes.NormalizedTimed && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Removes all Normalized Timed Events in the specified Group.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="iGroup">The Group to remove the Normalized Timed Events from</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveNormalizedTimedEvents(int iGroup)
            {
                // Remove all Normalized Timed Events that belong to the specified Group
                return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
                { return (cEvent.eType == EParticleSystemEventTypes.NormalizedTimed && cEvent.iGroup == iGroup); });
            }

            /// <summary>
            /// Removes a Normalized Timed Event with the specified Function, Time To Fire, Execution Order, and Group.
            /// Returns true if the Event was found and removed, false if not.
            /// </summary>
            /// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) the Event is scheduled to fire at</param>
            /// <param name="cFunction">The Function that is called when the Event fires</param>
            /// <param name="iExectionOrder">The Execution Order of the Event to remove. Default is zero.</param>
            /// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
            /// <returns>Returns true if the Event was found and removed, false if not.</returns>
            public bool RemoveNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleSystemDelegate cFunction, int iExectionOrder, int iGroup)
            {
                // Remove the Normalized Timed Event from the Event List
                return mcParticleSystemEventList.Remove(new CTimedParticleSystemEvent(cFunction, EParticleSystemEventTypes.NormalizedTimed, iExectionOrder, iGroup, fNormalizedTimeToFire));
            }

            /// <summary>
            /// Removes all Normalized Timed Events.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveAllNormalizedTimedEvents()
            {
                // Remove all Events with a Particle Event Type of NormalizedTimed
                return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
                { return cEvent.eType == EParticleSystemEventTypes.NormalizedTimed; });
            }

            /// <summary>
            /// Returns if there is a NormalizedTimed Event with the specified Function or not.
            /// </summary>
            /// <param name="cFunction">The Function of the NormalizedTimed Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
            public bool ContainsNormalizedTimedEvent(UpdateParticleSystemDelegate cFunction)
            {
                // Return if there is a NormalizedTimed Event with the specified Function
                return mcParticleSystemEventList.Exists(delegate(CParticleSystemEvent cEvent)
                { return (cEvent.eType == EParticleSystemEventTypes.NormalizedTimed && cEvent.cFunctionToCall == cFunction); });
            }

            /// <summary>
            /// Returns if there is a NormalizedTimed Event with the specifed Normalized Time To Fire, Function, Execution Order, and Group or not.
            /// </summary>
            /// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) the Event is scheduled to fire at</param>
            /// <param name="cFunction">The Function of the NormalizedTimed Event to look for</param>
            /// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
            /// <param name="iGroup">The Group of the Event to look for</param>
            /// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
            public bool ContainsNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
            {
                // Return if there is a NormalizedTimed Event with the specified criteria or not
                return mcParticleSystemEventList.Contains(new CTimedParticleSystemEvent(cFunction, EParticleSystemEventTypes.NormalizedTimed, iExecutionOrder, iGroup, fNormalizedTimeToFire));
            }

            /// <summary>
            /// Removes all Timed Events and Normalized Timed Events.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveAllTimedAndNormalizedTimedEvents()
            {
                return (RemoveAllTimedEvents() + RemoveAllNormalizedTimedEvents());
            }

            /// <summary>
            /// Removes all Events.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveAllEvents()
            {
                // Store the Number of Events that were in the Event List
                int iNumberOfEvents = mcParticleSystemEventList.Count;

                // Remove all Events from the Event List
                mcParticleSystemEventList.Clear();

                // Return the Number of Events removed
                return iNumberOfEvents;
            }

            /// <summary>
            /// Removes all Events in the specified Group.
            /// Returns the number of Events that were removed.
            /// </summary>
            /// <param name="iGroup">The Group to remove all Events from</param>
            /// <returns>Returns the number of Events that were removed.</returns>
            public int RemoveAllEventsInGroup(int iGroup)
            {
                // Remove all Events that belong to the specified Group
                return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
                { return (cEvent.iGroup == iGroup); });
            }

            /// <summary>
            /// Updates the Particle System according to the Particle System Events. This is done automatically
            /// by the Particle System every frame (i.e. Everytime the Update() function is called).
            /// </summary>
            /// <param name="fElapsedTimeInSeconds">How much Time has passed, in seconds, 
            /// since the last Update</param>
            public void Update(float fElapsedTimeInSeconds)
            {
                // Update the Particle System
                Update(fElapsedTimeInSeconds, fElapsedTimeInSeconds);

                // Remove all of the OneTime Events now that they've been processed
                RemoveAllOneTimeEvents();
            }

            /// <summary>
            /// Updates the Particle System according to the Particle System Events
            /// </summary>
            /// <param name="fElapsedTimeThisPass">The amount of Elapsed Time to pass
            /// into the Event Functions being called on this Pass</param>
            /// <param name="fTotalElapsedTimeThisFrame">How much Time has passed, in seconds, 
            /// since the last Frame</param>
            private void Update(float fElapsedTimeThisPass, float fTotalElapsedTimeThisFrame)
            {
                // Variable to tell if the Lifetime data needs to be reset so the Events can be Repeated or not
                bool bEventsNeedToBeRepeated = false;
                float fNextPassElapsedTime = 0.0f;

                // Update the Particle System's Elapsed Time
                LifetimeData.UpdateElapsedTimeVariables(fElapsedTimeThisPass);

                // If there are no Events to process
                if (mcParticleSystemEventList.Count <= 0)
                {
                    // Exit without doing anything
                    return;
                }

                // If the Particle System has reached the end of its Lifetime AND 
                // the Particle System Events are set to Repeat
                if (LifetimeData.ElapsedTime >= LifetimeData.Lifetime &&
                    LifetimeData.EndOfLifeOption == EParticleSystemEndOfLifeOptions.Repeat)
                {
                    // Record that the Lifetime data will need to be reset
                    bEventsNeedToBeRepeated = true;

                    // Save how far past the Lifetime the Elapsed Time has gone
                    fNextPassElapsedTime = LifetimeData.ElapsedTime - LifetimeData.Lifetime;

                    // Adjust the Elapsed Time to be the amount of time it would take to reach the Lifetime
                    fElapsedTimeThisPass -= fNextPassElapsedTime;

                    // Make sure we do not have a negative Elapsed Time, as this would be the
                    // case if the Elapsed Time was multiple times greater than the Lifetime
                    fElapsedTimeThisPass = Math.Max(fElapsedTimeThisPass, 0.0f);

                    // If the Elapsed Time was multiple times greater than the Lifetime, ignore all
                    // of the full Lifetimes we would process and skip to the last one
                    fNextPassElapsedTime = fNextPassElapsedTime % LifetimeData.Lifetime;

                    // Set the Particle System's Elapsed Time to its Lifetime
                    // We first set the Elapsed Time and then Update the Automatic Variables in order
                    // to make sure that the Last Elapsed Time is correct as well, since we can't 
                    // set it directly (we have to use the UpdateElapsedTimeVariables() function).
                    LifetimeData.ElapsedTime = LifetimeData.LastElapsedTime;
                    LifetimeData.UpdateElapsedTimeVariables(LifetimeData.Lifetime - LifetimeData.ElapsedTime);
                }

                // If we should process the Events this pass
                if (fElapsedTimeThisPass > 0.0f)
                {
                    // Loop through all of the Events (which are pre-sorted by Execution Order).
                    // We check against mcParticleSystemEventList.Count instead of storing the value 
                    // in a local variable incase an Event removes other Events (may throw an 
                    // index out of range exception if this happens).
                    for (int iIndex = 0; iIndex < mcParticleSystemEventList.Count; iIndex++)
                    {
                        // If this is an EveryTime Event
                        if (mcParticleSystemEventList[iIndex].eType == EParticleSystemEventTypes.EveryTime)
                        {
                            // Execute the Event's function
                            mcParticleSystemEventList[iIndex].cFunctionToCall(fElapsedTimeThisPass);
                        }
                        // Else if this is a OneTime Event
                        else if (mcParticleSystemEventList[iIndex].eType == EParticleSystemEventTypes.OneTime)
                        {
                            // If this is the last time this function will be called this frame.
                            // This check is here because OneTime Events should only execute once, so we
                            // only execute the function if we know this function will not be recursively
                            // calling itself again, and we use the total Elapsed Time of this frame instead
                            // of the Elapsed Time for this pass.
                            if (!bEventsNeedToBeRepeated)
                            {
                                // Execute the OneTime Event using the Total Elapsed Time for this frame
                                mcParticleSystemEventList[iIndex].cFunctionToCall(fTotalElapsedTimeThisFrame);
                            }
                        }
                        // Else this is a Timed Event
                        else
                        {
                            // Get a handle to the Timed Event
                            CTimedParticleSystemEvent cTimedEvent = (CTimedParticleSystemEvent)mcParticleSystemEventList[iIndex];

                            // If this is a normal Timed Event
                            if (cTimedEvent.eType == EParticleSystemEventTypes.Timed)
                            {
                                // Store the Last Elapsed Time of the Particle. 
                                // If it's zero we need to change it to be before zero so that any Events set to fire at time zero will be fired.
                                float fLastElapsedTime = (LifetimeData.LastElapsedTime == 0.0f) ? -0.1f : LifetimeData.LastElapsedTime;

                                // If this Event should fire
                                if (fLastElapsedTime < cTimedEvent.fTimeToFire &&
                                    LifetimeData.ElapsedTime >= cTimedEvent.fTimeToFire)
                                {
                                    // Execute the Event's function
                                    cTimedEvent.cFunctionToCall(fElapsedTimeThisPass);
                                }
                            }
                            // Else this is a Normalized Timed Event
                            else
                            {
                                // Store the Last Normalized Elapsed Time of the Particle. 
                                // If it's zero we need to change it to be before zero so that any Events set to fire at time zero will be fired.
                                float fLastNormalizedElapsedTime = (LifetimeData.LastNormalizedElapsedTime == 0.0f) ? -0.1f : LifetimeData.LastNormalizedElapsedTime;

                                // If this Event should fire
                                if (fLastNormalizedElapsedTime < cTimedEvent.fTimeToFire &&
                                    LifetimeData.NormalizedElapsedTime >= cTimedEvent.fTimeToFire)
                                {
                                    // Fire the Event
                                    cTimedEvent.cFunctionToCall(fElapsedTimeThisPass);
                                }
                            }
                        }
                    }
                }

                // If the Particle System Lifetime should be reset and Timed Events repeated
                if (bEventsNeedToBeRepeated)
                {
                    // Reset the Particle System's Elapsed Time
                    LifetimeData.ElapsedTime = 0.0f;

                    // Call this function again recursively to process the Timed Events properly
                    Update(fNextPassElapsedTime, fTotalElapsedTimeThisFrame);
                }
            }
        }

        // Define the Delegates used to Initialize a Particle's properties and Update a Vertex's properties
        private InitializeParticleDelegate mcParticleInitializationFunction = null;
        private UpdateVertexDelegate mcVertexUpdateFunction = null;

        private ParticleTypes meParticleType = ParticleTypes.None;   // Tells which type of Particle is being used so we know how to draw it (e.g. Point Sprite, Quad, etc)
        private Particle[] mcParticles = null;      // Array used to hold all Particles
        private int miNumberOfParticlesToDraw = 0;  // Tells how many Particles in the Vertex Buffer should be drawn (i.e. How many are Active and Visible)

        private Vertex[] mcParticleVerticesToDraw = null;   // Array used to hold Particle Vertex information (i.e. The Vertex Buffer used to draw the Point Sprites and Quads)
        private int[] miIndexBufferArray = null;            // The Index Buffer used along with the Vertex Buffer for drawing Quads
        private int miIndexBufferIndex = 0;                 // The current Index we are at in the Index Buffer
        private SpriteBatch mcSpriteBatch = null;                   // The Sprite Batch used to draw Sprites
        private LinkedList<Particle> mcParticleSpritesToDraw = null;// The List of Sprites to Draw (i.e. The Vertex Buffer used to draw Sprites)
        private SpriteBatchSettings mcSpriteBatchSettings = null;  // Variable used to hold the Sprite Batch's drawing options

        private LinkedList<Particle> mcActiveParticlesList = null;      // List used to hold Active (alive) Particles
        private LinkedList<Particle> mcInactiveParticlesList = null;    // List used to hold Inactive (dead) Particles

        private VertexDeclaration mcVertexDeclaration = null;   // Description of the Vertex Structure used to draw Particles

        private Effect mcEffect = null;         // The Effect file to use to draw the Particles
        private Texture2D mcTexture = null;     // The Texture used to draw Particles

        private int miMaxNumberOfParticlesAllowed = 0;  // The Max Number of Particles that this Particle System is Allowed to contain at a given time
        
    // If not inheriting from DrawableGameComponent, we need to define the variables that
    // are included in DrawableGameComponent ourself
    #if (!DPSFAsDrawableGameComponent)
        private bool mbPerformDraws = true;         // Tells if the Particles should be Drawn or not
        private bool mbPerformUpdates = true;       // Tells if the Particle System and its Particles should be Updated or not

        private int miDrawOrder = 0;                // Tells where in the Order this particle system should be Drawn
        private int miUpdateOrder = 0;              // Tells where in the Order this particle system should be Updated

        private Game mcGame = null;                 // The Game object passed into the constructor (Not used by DPSF at all; Just here for consistency with DrawableGameComponent)
        private GraphicsDevice mcGraphicsDevice = null; // Handle to the Graphics Device to draw to

        /// <summary>
        /// Raised when the UpdateOrder property changes
        /// </summary>
        public event EventHandler UpdateOrderChanged = null;

        /// <summary>
        /// Raised when the DrawOrder property changes
        /// </summary>
        public event EventHandler DrawOrderChanged = null;

        /// <summary>
        /// Raised when the Enabled property changes
        /// </summary>
        public event EventHandler EnabledChanged = null;

        /// <summary>
        /// Raised when the Visible property changes
        /// </summary>
        public event EventHandler VisibleChanged = null;
    #endif

        private ContentManager mcContentManager = null; // Handle to the Content Manager used to load Textures and Effects

        private float mfSimulationSpeed = 1.0f;         // How much to scale the Simulation Speed by (1.0 = normal speed, 0.5 = half speed, 2.0 = double speed)
        private float mfInternalSimulationSpeed = 1.0f; // How much to scale the Simulation Speed by to make the effect run at "normal" speed

        // Variables used to control how often the Particle System is updated
        private float mfTimeToWaitBetweenUpdates = 0.0f;    // How much time should elapse between Update()s
        private float mfTimeElapsedSinceLastUpdate = 0.0f;  // How much time has elapsed since the last Update()

        private CParticleEvents mcParticleEvents = null;            // Variable used to access the Particle Events
        private CParticleSystemEvents mcParticleSystemEvents = null;// Variable used to access the Particle System Events

        // Auto Memory Manager variables
        private AutoMemoryManagerSettings mcAutoMemoryManagerSettings = null;  // Auto Memory Manager Settings
        private float mfAutoMemoryManagersElapsedTime = 0.0f;                   // The Automatic Memory Manager's Timer
        private int miAutoMemoryManagerMaxNumberOfParticlesActiveAtOnceOverTheLastXSeconds = 0; // The Max Number of Particles that were Active in a single frame Over The Last X Seconds (X is specified in the Auto Memory Manager Settings)

        private ParticleEmitter mcEmitter = null;   // The Emitter used to automatically generate Particles

        private int miID = 0;                       // Set this Particle Systems unique ID
        private int miType = 0;                     // The Type of Particle System this is

        private RandomNumbers mcRandom = null;      // Random number generator

        private Matrix mcWorld = Matrix.Identity;       // The World Matrix
        private Matrix mcView = Matrix.Identity;        // The View Matrix
        private Matrix mcProjection = Matrix.Identity;  // The Projection Matrix

        private ParticleSystemManager mcParticleSystemManager = null;  // The Manager whose properties should be used for this Particle System

        private static Effect SmcDPSFEffect = null;     // Handle to the default DPSF Effect

        #endregion

        #region Methods that cannot be overridden

    // If we are inheriting DrawableGameComponent
    #if (DPSFAsDrawableGameComponent)
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">The Game object to attach this Particle System to</param>
        public DPSF(Game cGame) : base(cGame)
        {
            // If the Game object given in the constructor was invalid
            if (Game == null)
            {
                // Throw an exception
                throw new ArgumentNullException("cGame", "Game parameter specified in Particle System constructor is null. The constructor's Game parameter cannot be null when DPSF is inheriting from DrawableGameComponent.");
            }
    #else
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DPSF(Game cGame)
        {
            // Save a handle to the Game if one was given
            mcGame = cGame;
    #endif
            // Set this Particle Systems unique ID
            miID = ParticleSystemCounter.GetUniqueID();
        }

        /// <summary>
        /// Initializes a new No Display Particle System. This type of Particle System does not allow any of the Particles
        /// to be drawn to a Graphics Device (e.g. the screen).
        /// </summary>
        /// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
        /// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
        /// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
        /// may also be adjusted manually at run-time.</param>
        /// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
        /// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
        /// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
        /// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
        /// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
        /// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
        /// Particles will be allowed, even though there is memory allocated for more Particles. This value 
        /// may also be adjusted manually at run-time.</param>
        public void InitializeNoDisplayParticleSystem(int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow)
        {
            try
            {
                // Initialize the variables that all Particle Systems have in common
                InitializeCommonVariables(null, null, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.NoDisplay);
            }
            catch (Exception e)
            {
                // Specify that the Particle System is not yet Initialized and re-throw the exception
                ParticleType = ParticleTypes.None;
                throw new Exception("A problem occurred while Initializing the Particle System.", e);
            }

            // Perform any user operations now that the Particle System is Initialized
            AfterInitialize();
        }

        /// <summary>
        /// Initializes a new Pixel Particle System
        /// </summary>
        /// <param name="cGraphicsDevice">Graphics Device to draw to</param>
        /// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
        /// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
        /// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
        /// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
        /// may also be adjusted manually at run-time.</param>
        /// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
        /// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
        /// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
        /// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
        /// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
        /// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
        /// Particles will be allowed, even though there is memory allocated for more Particles. This value 
        /// may also be adjusted manually at run-time.</param>
        /// <param name="cVertexUpdateFunction">Function used to copy a Particle's drawable properties into the vertex buffer</param>
        public void InitializePixelParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager,
                                int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow,
                                UpdateVertexDelegate cVertexUpdateFunction)
        {
            try
            {
                // Initialize the variables that all Particle Systems have in common
                InitializeCommonVariables(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.Pixel);

                // Set the Vertex Update Function to use
                VertexUpdateFunction = cVertexUpdateFunction;

                // Set the Effect and Technique to use
                SetEffectAndTechnique(DPSFDefaultEffect, "Pixels");

                // Make sure the Texture is null since it is not required
                Texture = null;
            }
            catch (Exception e)
            {
                // Specify that the Particle System is not yet Initialized and re-throw the exception
                ParticleType = ParticleTypes.None;
                throw new Exception("A problem occurred while Initializing the Particle System.", e);
            }

            // Perform any user operations now that the Particle System is Initialized
            AfterInitialize();
        }

        /// <summary>
        /// Initializes a new Sprite Particle System
        /// </summary>
        /// <param name="cGraphicsDevice">Graphics Device to draw to</param>
        /// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
        /// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
        /// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
        /// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
        /// may also be adjusted manually at run-time.</param>
        /// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
        /// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
        /// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
        /// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
        /// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
        /// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
        /// Particles will be allowed, even though there is memory allocated for more Particles. This value 
        /// may also be adjusted manually at run-time.</param>
        /// <param name="sTexture">The asset name of the Texture to use to visualize the Particles</param>
        public void InitializeSpriteParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager,
                                int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, string sTexture)
        {
            try
            {
                // Initialize the variables that all Particle Systems have in common
                InitializeCommonVariables(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.Sprite);

                // Set the Effect and Technique to use
                SetEffectAndTechnique(DPSFDefaultEffect, "Sprites");

                // Set the Texture to use
                SetTexture(sTexture);
            }
            catch (Exception e)
            {
                // Specify that the Particle System is not yet Initialized and re-throw the exception
                ParticleType = ParticleTypes.None;
                throw new Exception("A problem occurred while Initializing the Particle System.", e);
            }

            // Perform any user operations now that the Particle System is Initialized
            AfterInitialize();
        }

        /// <summary>
        /// Initializes a new Point Sprite Particle System
        /// </summary>
        /// <param name="cGraphicsDevice">Graphics Device to draw to</param>
        /// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
        /// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
        /// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
        /// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
        /// may also be adjusted manually at run-time.</param>
        /// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
        /// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
        /// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
        /// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
        /// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
        /// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
        /// Particles will be allowed, even though there is memory allocated for more Particles. This value 
        /// may also be adjusted manually at run-time.</param>
        /// <param name="cVertexUpdateFunction">Function used to copy a Particle's drawable properties into the vertex buffer</param>
        /// <param name="sTexture">The asset name of the Texture to use to visualize the Particles</param>
        public void InitializePointSpriteParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager,
                                int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, 
                                UpdateVertexDelegate cVertexUpdateFunction, string sTexture)
        {
            try
            {
                // Initialize the variables that all Particle Systems have in common
                InitializeCommonVariables(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.PointSprite);

                // Set the Vertex Update Function to use
                VertexUpdateFunction = cVertexUpdateFunction;

                // Set the Effect and Technique to use
                SetEffectAndTechnique(DPSFDefaultEffect, "PointSprites");

                // Set the Texture to use
                SetTexture(sTexture);
            }
            catch (Exception e)
            {
                // Specify that the Particle System is not yet Initialized and re-throw the exception
                ParticleType = ParticleTypes.None;
                throw new Exception("A problem occurred while Initializing the Particle System.", e);
            }

            // Perform any user operations now that the Particle System is Initialized
            AfterInitialize();
        }

        /// <summary>
        /// Initializes a new Quad Particle System
        /// </summary>
        /// <param name="cGraphicsDevice">Graphics Device to draw to</param>
        /// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
        /// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
        /// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
        /// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
        /// may also be adjusted manually at run-time.</param>
        /// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
        /// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
        /// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
        /// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
        /// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
        /// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
        /// Particles will be allowed, even though there is memory allocated for more Particles. This value 
        /// may also be adjusted manually at run-time.</param>
        /// <param name="cVertexUpdateFunction">Function used to copy a Particle's drawable properties into the vertex buffer</param>
        public void InitializeQuadParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager,
                                int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow,
                                UpdateVertexDelegate cVertexUpdateFunction)
        {
            try
            {
                // Initialize the variables that all Particle Systems have in common
                InitializeCommonVariables(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.Quad);

                // Set the Vertex Update Function to use
                VertexUpdateFunction = cVertexUpdateFunction;

                // Set the Effect and Technique to use
                SetEffectAndTechnique(DPSFDefaultEffect, "Quads");

                // Make sure the Texture is null since it is not required
                Texture = null;
            }
            catch (Exception e)
            {
                // Specify that the Particle System is not yet Initialized and re-throw the exception
                ParticleType = ParticleTypes.None;
                throw new Exception("A problem occurred while Initializing the Particle System.", e);
            }

            // Perform any user operations now that the Particle System is Initialized
            AfterInitialize();
        }

        /// <summary>
        /// Initializes a new Textured Quad Particle System
        /// </summary>
        /// <param name="cGraphicsDevice">Graphics Device to draw to</param>
        /// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
        /// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
        /// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
        /// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
        /// may also be adjusted manually at run-time.</param>
        /// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
        /// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
        /// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
        /// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
        /// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
        /// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
        /// Particles will be allowed, even though there is memory allocated for more Particles. This value 
        /// may also be adjusted manually at run-time.</param>
        /// <param name="cVertexUpdateFunction">Function used to copy a Particle's drawable properties into the vertex buffer</param>
        /// <param name="sTexture">The asset name of the Texture to use to visualize the Particles</param>
        public void InitializeTexturedQuadParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager,
                                int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, 
                                UpdateVertexDelegate cVertexUpdateFunction, string sTexture)
        {
            try
            {
                // Initialize the variables that all Particle Systems have in common
                InitializeCommonVariables(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.TexturedQuad);

                // Set the Vertex Update Function to use
                VertexUpdateFunction = cVertexUpdateFunction;

                // Set the Effect and Technique to use
                SetEffectAndTechnique(DPSFDefaultEffect, "TexturedQuads");

                // Set the Texture to use
                SetTexture(sTexture);
            }
            catch (Exception e)
            {
                // Specify that the Particle System is not yet Initialized and re-throw the exception
                ParticleType = ParticleTypes.None;
                throw new Exception("A problem occurred while Initializing the Particle System.", e);
            }

            // Perform any user operations now that the Particle System is Initialized
            AfterInitialize();
        }

        /// <summary>
        /// Initialize the variables common to all Particle Systems
        /// </summary>
        /// <param name="cGraphicsDevice">Graphics Device to draw to</param>
        /// <param name="cContentManager">Content Manager to use to load Effect files and Textures</param>
        /// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
        /// be Allocated for. The Maximum Number Of Particles the Particle System should Allow is also
        /// set to this value initially.</param>
        /// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
        /// Allowed at a single point in time. This should be less than or equal to the Number Of Particles 
        /// To Allocate Memory For if the Auto Memory Manager will not be used, as the Particle System 
        /// can only handle as many Particles as it has Memory Allocated For.</param>
        /// <param name="eParticleType">The Type of Particles this Particle System should draw</param>
        private void InitializeCommonVariables(GraphicsDevice cGraphicsDevice, ContentManager cContentManager,
                                        int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, 
                                        ParticleTypes eParticleType)
        {
            // Destroy the Particle System before Initializing to make sure it is a fresh Initialize
            Destroy();

            // Set the Particle Type before anything else (Required for GraphicsDevice and ContentManager property checks)
            ParticleType = eParticleType;

        // If we inherit from DrawableGameComponent
        #if (DPSFAsDrawableGameComponent)
            // If the Game object given in the constructor was invalid
            if (Game == null)
            {
                // Throw an exception
                throw new ArgumentNullException("The Game object given in the constructor is now null in the Initialization function and no longer valid.");
            }
            // Else a valid Game object was given in the constructor
            else
            {
                // Initialize the Drawable Game Component
                Initialize();

                // If the Game does not already Contain this Component
                if (!Game.Components.Contains(this))
                {
                    // Add this Particle System to the Game Components to be Updated and Drawn
                    Game.Components.Add(this);
                }
            }
        // Else we do not inherit from DrawableGameComponent
        #else
            // Save a handle to the Graphics Device
            GraphicsDevice = cGraphicsDevice;
        #endif

            // Save a handle to the Content Manager
            ContentManager = cContentManager;

            // If the Default Effect file hasn't been loaded yet
            if (SmcDPSFEffect == null && GraphicsDevice != null)
            {
                DummyService cService = new DummyService(GraphicsDevice);
                ResourceContentManager cResources = new ResourceContentManager(cService, DPSFResources.ResourceManager);
// If we are on Windows
#if (!XBOX)
                SmcDPSFEffect = cResources.Load<Effect>("DPSFDefaultEffect");
// Else we are on the Xbox 360
#else
                SmcDPSFEffect = cResources.Load<Effect>("DPSFDefaultEffectXbox360");
#endif
            }

            // Set the Number of Particles to Allocate Memory for (which Initializes the Particle Arrays)
            NumberOfParticlesAllocatedInMemory = iNumberOfParticlesToAllocateMemoryFor;

            // Set the Max Number Of Particles To Allow
            MaxNumberOfParticlesAllowed = iMaxNumberOfParticlesToAllow;

            // Specify the VertexElement to use for each Vertex of a Particle
            Vertex cVertex = new Vertex();
            VertexElement = cVertex.VertexElements;

            // Specify to Draw and Update Particles by default
            Visible = true;
            Enabled = true;

            /////////////////////////////////////////////////////////
            // Do not reset the Draw and Update Orders.
            // We don't want users to have to respecify them every time they reinitialize the particle system.
            //UpdateOrder = 0;
            //DrawOrder = 0;
            /////////////////////////////////////////////////////////

            // Set the simulation to run at normal speed
            SimulationSpeed = 1.0f;
            InternalSimulationSpeed = 1.0f;

            // Seed the random number generator
            mcRandom = new RandomNumbers();

            // Initialize the Particle Events
            mcParticleEvents = new CParticleEvents();

            // Initialize the Particle System Events
            mcParticleSystemEvents = new CParticleSystemEvents();

            // Initialize the Auto Memory Manager Settings
            mcAutoMemoryManagerSettings = new AutoMemoryManagerSettings();

            // Initialize the Emitter
            mcEmitter = new ParticleEmitter();

            // Copy any properties from the Particle System Manager into this Particle System
            ParticleSystemManagerToCopyPropertiesFrom = ParticleSystemManagerToCopyPropertiesFrom;
        }

        /// <summary>
        /// Release all resources used by the Particle System and reset all properties to their default values
        /// </summary>
        public void Destroy()
        {
            // Perform any other user operations
            BeforeDestroy();

        // If we inherit from DrawableGameComponent
        #if (DPSFAsDrawableGameComponent)
            // If a valid Game object was given in the constructor
            if (Game != null)
            {
                // If the Game Contains this Component
                if (Game.Components.Contains(this))
                {
                    // Remove this Particle System from the Game Components to be Updated and Drawn
                    Game.Components.Remove(this);
                }
            }
        // Else we do not inherit from DrawableGameComponent
        #else
            // Release the handle to the Graphics Device
            mcGraphicsDevice = null;
        #endif

            // Set the Particle Type to None, as some other properties can only be set to null when this is None
            meParticleType = ParticleTypes.None;

            // Release the Content Manager
            mcContentManager = null;

            // Release all data used by this class instance
            mcParticleInitializationFunction = null;
            mcVertexUpdateFunction = null;
            
            mcParticles = null;
            miNumberOfParticlesToDraw = 0;

            mcParticleVerticesToDraw = null;
            miIndexBufferArray = null;
            miIndexBufferIndex = 0;
            mcSpriteBatch = null;
            mcParticleSpritesToDraw = null;
            mcSpriteBatchSettings = null;

            // If the Active Particle List still exists
            if (mcActiveParticlesList != null)
            {
                mcActiveParticlesList.Clear();
            }
            mcActiveParticlesList = null;

            // If the Inactive Particle List still exists
            if (mcInactiveParticlesList != null)
            {
                mcInactiveParticlesList.Clear();
            }
            mcInactiveParticlesList = null;

            miMaxNumberOfParticlesAllowed = 0;

            mcVertexDeclaration = null;

            mcEffect = null;
            mcTexture = null;

            Visible = false;
            Enabled = false;

            /////////////////////////////////////////////////////////
            // Do not reset the Draw and Update Orders.
            // We don't want users to have to respecify them every time they reinitialize the particle system.
            //UpdateOrder = 0;
            //DrawOrder = 0;
            /////////////////////////////////////////////////////////

            SimulationSpeed = 1.0f;
            InternalSimulationSpeed = 1.0f;

            mfTimeToWaitBetweenUpdates = 0.0f;
            mfTimeElapsedSinceLastUpdate = 0.0f;

            World = Matrix.Identity;
            View = Matrix.Identity;
            Projection = Matrix.Identity;

            mcRandom = null;

            mcParticleEvents = null;
            mcParticleSystemEvents = null;

            mcAutoMemoryManagerSettings = null;

            mcEmitter = null;

            // Perform any other user operations
            AfterDestroy();
        }

        /// <summary>
        /// Returns true if the Particle System has been Initialized, false if not
        /// </summary>
        /// <returns>Returns true if the Particle System has been Initialized, false if not</returns>
        public bool IsInitialized()
        {
            // Return if the Particle System has been initialized or not
            return (mcParticles != null && meParticleType != ParticleTypes.None);
        }

        /// <summary>
        /// Get the DPSF Default Effect. This Effect is used by default if no other Effect and Technique are
        /// specified when calling the InitializeXParticleSystem() functions.
        /// <para>This Effect has 5 techniques that may be used: Pixels, Sprites, PointSprites, Quads, and TexturedQuads.
        /// Pixel is for drawing pixels, Sprites is for drawing sprites, PointSprites is for drawing point sprites,
        /// Quads is for drawing colored quads, and TexturedQuads is for drawing textured quads.</para>
        /// <para>This Effect also has a number of parameters that may be set:</para>
        /// <para>1. float xColorBlendAmount may be set to specify how much of the Particle's Color should be blended in 
        /// with the Texture's Color (0.0 = use Texture's color, 1.0 = use Particle's color, 0.5 (default) = use equal amounts
        /// of Texture's and Particle's color), and this has an effect when using the Sprites, PointSprites, and 
        /// TexturedQuads techniques.</para>
        /// <para>2. float4x4 xView, float4x4 xProjection, float4x4 xWorld should be set when using the Pixels, PointSprites, 
        /// Quads, and TexturedQuads techniques to specify the View, Projection, and World matrices respectively so that
        /// the vertices may be transformed propertly from 3D world space to 2D screen space.</para>
        /// <para>3. texture xTexture should be set when using the Sprites, PointSprites, and TexturedQuads techniques to set
        /// the texture that should be used to represent the particles.</para>
        /// <para>4. xViewportHeight may be set to the height of the viewport when using the PointSprites technique to simulate 
        /// perspective scaling (i.e. the texture gets smaller as it gets further from the camera). Set this to zero (default)
        /// to not use perspective scaling and have the texture size remain fixed regardless of its distance from the camera.</para>
        /// </summary>
        public Effect DPSFDefaultEffect
        {
            get { return SmcDPSFEffect; }
        }

    // If not inheriting DrawableGameComponent, we need to define the Properties
    // implemented by DrawableGameComponent ourself.
    #if (!DPSFAsDrawableGameComponent)
        /// <summary>
        /// Get / Set if this Particle System should Draw its Particles or not.
        /// <para>NOTE: Setting this to false causes the Draw() function to not draw anything.</para>
        /// </summary>
        public bool Visible
        {
            get { return mbPerformDraws; }
            set
            { 
                mbPerformDraws = value; 

                // If there is a function to catch the event
                if (VisibleChanged != null)
                {
                    // Throw the event that the Visibility was changed
                    VisibleChanged(this, null);
                }
            }
        }

        /// <summary>
        /// Get / Set if this Particle System should Update itself and its Particles or not.
        /// <para>NOTE: Setting this to false causes the Update() function to not update anything.</para>
        /// </summary>
        public bool Enabled
        {
            get { return mbPerformUpdates; }
            set
            { 
                mbPerformUpdates = value;

                // If there is a function to catch the event
                if (EnabledChanged != null)
                {
                    // Throw the event that the Enabled state was changed
                    EnabledChanged(this, null);
                }
            }
        }

        /// <summary>
        /// The Order in which the Particle System should be Updated relative to other 
        /// DPSF Particle Systems in the same Particle System Manager. Particle Systems 
        /// are Updated in ascending order according to their Update Order (i.e. lowest first).
        /// <para>NOTE: The Update Order is one of the few properties that is not reset when
        /// the particle system is initialized or destroyed.</para>
        /// </summary>
        public int UpdateOrder
        {
            get { return miUpdateOrder; }
            set
            { 
                miUpdateOrder = value;

                // If there is a function to catch the event
                if (UpdateOrderChanged != null)
                {
                    // Throw the event that the Update Order was changed
                    UpdateOrderChanged(this, null);
                }
            }
        }

        /// <summary>
        /// The Order in which the Particle System should be Drawn relative to other
        /// DPSF Particle Systems in the same Particle System Manager. Particle Systems
        /// are Drawn in ascending order according to their Draw Order (i.e. lowest first).
        /// <para>NOTE: The Draw Order is one of the few properties that is not reset when
        /// the particle system is initialized or destroyed.</para>
        /// </summary>
        public int DrawOrder
        {
            get { return miDrawOrder; }
            set 
            { 
                miDrawOrder = value; 

                // If there is a function to catch the event
                if (DrawOrderChanged != null)
                {
                    // Throw the event that the Draw Order was changed
                    DrawOrderChanged(this, null);
                }
            }
        }

        /// <summary>
        /// Get the Game object set in the constructor, if one was given.
        /// </summary>
        public Game Game
        {
            get { return mcGame; }
        }

        /// <summary>
        /// Get / Set the Graphics Device to draw to
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return mcGraphicsDevice; }
            set
            {
                // If a valid Graphics Device was specified
                if (value != null || ParticleType == ParticleTypes.NoDisplay)
                {
                    mcGraphicsDevice = value;
                }
                // Else an invalid Graphics Device was specified
                else
                {
                    throw new ArgumentNullException("GraphicsDevice", "The specified Graphics Device is null. A valid Graphics Device is required.");
                }
            }
        }
    // Else we are inheriting DrawableGameComponent
    #else
        // Hide the Initialize() function
        private new void Initialize() { base.Initialize(); }
    #endif

        /// <summary>
        /// Get if the Particle System is inheriting from DrawableGameComponent or not.
        /// <para>If inheriting from DrawableGameComponent, the Particle Systems
        /// are automatically added to the given Game object's Components and the
        /// Update() and Draw() functions are automatically called by the
        /// Game object when it updates and draws the rest of its Components.
        /// If the Update() and Draw() functions are called by the user anyways,
        /// they will exit without performing any operations, so it is suggested
        /// to include them anyways to make switching between inheriting and
        /// not inheriting from DrawableGameComponent seemless; just be aware
        /// that the updates and draws are actually being performed when the
        /// Game object is told to update and draw (i.e. when base.Update() and base.Draw()
        /// are called), not when these functions are being called.</para>
        /// </summary>
        public bool InheritsDrawableGameComponent
        {
        // If inheriting DrawableGameComponent
        #if (DPSFAsDrawableGameComponent)
            get { return true; }
        // Else not inheriting DrawableGameComponent
        #else
            get { return false; }
        #endif
        }

        /// <summary>
        /// Get the unique ID of this Particle System.
        /// <para>NOTE: Each Particle System is automatically assigned a unique ID when it is instanciated.</para>
        /// </summary>
        public int ID
        {
            get { return miID; }
        }

        /// <summary>
        /// Get / Set the Type of Particle System this is
        /// </summary>
        public int Type
        {
            get { return miType; }
            set { miType = value; }
        }

        /// <summary>
        /// Get the Name of the Class that this Particle System is using. This can be used to 
        /// check what type of Particle System this is at run-time.
        /// </summary>
        public string ClassName
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// Get / Set the Content Manager to use to load Textures and Effects
        /// </summary>
        public ContentManager ContentManager
        {
            get { return mcContentManager; }
            set
            {
                // If a valid Content Manager was given
                if (value != null || ParticleType == ParticleTypes.NoDisplay)
                {
                    mcContentManager = value;
                }
                // Else an invalid Content Manager was given
                else
                {
                    throw new ArgumentNullException("ContentManager", "The specified Content Manager is null. A valid Content Manager is required.");
                }
            }
        }

        /// <summary>
        /// Get / Set the Index Buffer values. The Index Buffer is used when drawing Quads
        /// </summary>
        protected int[] IndexBuffer
        {
            get { return miIndexBufferArray; }
            set { miIndexBufferArray = value; }
        }

        /// <summary>
        /// Get / Set the current position in the Index Buffer
        /// </summary>
        protected int IndexBufferIndex
        {
            get { return miIndexBufferIndex; }
            set { miIndexBufferIndex = value; }
        }

        /// <summary>
        /// Particle Events may be used to update Particles
        /// </summary>
        public CParticleEvents ParticleEvents
        {
            get { return mcParticleEvents; }
        }

        /// <summary>
        /// Particle System Events may be used to update the Particle System
        /// </summary>
        public CParticleSystemEvents ParticleSystemEvents
        {
            get { return mcParticleSystemEvents; }
        }

        /// <summary>
        /// The Sprite Batch drawing Settings used in the Sprite Batch's Begin() function call.
        /// <para>NOTE: These settings only have effect if this is a Sprite particle system.</para>
        /// </summary>
        public SpriteBatchSettings SpriteBatchSettings
        {
            get { return mcSpriteBatchSettings; }
        }

        /// <summary>
        /// The Settings used to control the Automatic Memory Manager
        /// </summary>
        public AutoMemoryManagerSettings AutoMemoryManagerSettings
        {
            get { return mcAutoMemoryManagerSettings; }
        }

        /// <summary>
        /// The Emitter is used to automatically generate new Particles
        /// </summary>
        public ParticleEmitter Emitter
        {
            get { return mcEmitter; }
            set { mcEmitter = value; }
        }

        /// <summary>
        /// Get a RandomNumbers object used to generate Random Numbers
        /// </summary>
        public RandomNumbers RandomNumber
        {
            get { return mcRandom; }
        }

        /// <summary>
        /// Get / Set the World Matrix to use for drawing 3D Particles
        /// </summary>
        public Matrix World
        {
            get { return mcWorld; }
            set { mcWorld = value; }
        }

        /// <summary>
        /// Get / Set the View Matrix to use for drawing 3D Particles
        /// </summary>
        public Matrix View
        {
            get { return mcView; }
            set { mcView = value; }
        }

        /// <summary>
        /// Get / Set the Projection Matrix to use for drawing 3D Particles
        /// </summary>
        public Matrix Projection
        {
            get { return mcProjection; }
            set { mcProjection = value; }
        }

        /// <summary>
        /// Set the World, View, and Projection matrices for this Particle System.
        /// <para>NOTE: Sprite particle systems are not affected by the World, View, and Projection matrices.</para>
        /// </summary>
        /// <param name="cWorld">The World matrix</param>
        /// <param name="cView">The View matrix</param>
        /// <param name="cProjection">The Projection matrix</param>
        public void SetWorldViewProjectionMatrices(Matrix cWorld, Matrix cView, Matrix cProjection)
        {
            World = cWorld;
            View = cView;
            Projection = cProjection;
        }

        /// <summary>
        /// Set the VertexElement (i.e. Vertex Format) to use for each vertex of a Particle.
        /// <para>NOTE: VertexElement will not be changed if null value is given.</para>
        /// </summary>
        private VertexElement[] VertexElement
        {
            set 
            {
                // If a valid Vertex Element was given, and we have a valid Graphics Device to use
                if (value != null && GraphicsDevice != null)
                {
                    // Initialize the Vertex Declaration used to draw Particles
                    mcVertexDeclaration = new VertexDeclaration(GraphicsDevice, value);
                }
                // Else an invalid Vertex Element was given
                else
                {
                    // If this Type of Particle does not require a Vertex Element
                    if (ParticleType == ParticleTypes.None || ParticleType == ParticleTypes.NoDisplay ||
                        ParticleType == ParticleTypes.Sprite)
                    {
                        // Set the Vertex Declaration to null
                        mcVertexDeclaration = null;
                    }
                    // Else if we do not have a valid Graphics Device to create the Vertex Declaration with
                    else if (GraphicsDevice == null)
                    {
                        throw new ArgumentNullException("GraphicsDevice", "The current Graphics Device is null. A valid Graphics Device is required to create a new Vertex Declaration in order to draw the current type of Particles.");
                    }
                    // Else a valid Vertex Element is required
                    else
                    {
                        throw new ArgumentNullException("VertexElement", "The specified Vertex Element is null. A valid Vertex Element is required to draw the current Type of Particles.");
                    }
                }
            }
        }

        /// <summary>
        /// Set the function to use to copy a Particle's renderable properties into the Vertex Buffer.
        /// <para>NOTE: VertexUpdateFunction will not be changed if null value is given.</para>
        /// </summary>
        public UpdateVertexDelegate VertexUpdateFunction
        {
            set
            {
                // If an invalid Vertex Update Function is given
                if (value == null)
                {
                    // If this Type of Particle doesn't require a Vertex Update Function
                    if (ParticleType == ParticleTypes.None || ParticleType == ParticleTypes.NoDisplay ||
                        ParticleType == ParticleTypes.Sprite)
                    {
                        // Set the Vertex Update Function to null
                        mcVertexUpdateFunction = null;
                    }
                    // Else a valid Vertex Update Function is required
                    else
                    {
                        throw new ArgumentNullException("VertexUpdateFunction", "The specified Vertex Update Function is null. A valid Vertex Update Function is required to draw the current Type of Particles.");
                    }
                }
                // Else a valid Vertex Update Function was given
                else
                {
                    // Set the function used to Update a Vertex
                    mcVertexUpdateFunction = new UpdateVertexDelegate(value);
                }
            }
        }

        /// <summary>
        /// Sets the function to use to Initialize a Particle's properties.
        /// </summary>
        public InitializeParticleDelegate ParticleInitializationFunction
        {
            set
            {
                // If a valid Function was given
                if (value != null)
                {
                    // Set the Function to use to Initialize Particles
                    mcParticleInitializationFunction = new InitializeParticleDelegate(value);
                }
                // Else no Function was given
                else
                {
                    // So set to not use a Particle Initialization Function
                    mcParticleInitializationFunction = null;
                }
            }
        }

        /// <summary>
        /// Sets the Effect and Technique to use to draw the Particles
        /// </summary>
        /// <param name="sEffect">The Asset Name of the Effect to use</param>
        /// <param name="sTechnique">The name of the Effect's Technique to use</param>
        public void SetEffectAndTechnique(string sEffect, string sTechnique)
        {
            // If the Effect to use is invalid
            if (sEffect == null || sEffect.Equals(""))
            {
                throw new ArgumentNullException("sEffect", "The Effect string supplied is null or an empty string. The Effect to use cannot be null.");
            }

            // Load the Effect
            Effect cEffect = ContentManager.Load<Effect>(sEffect);

            // Set the Effect and Technique to use
            SetEffectAndTechnique(cEffect, sTechnique);
        }

        /// <summary>
        /// Sets the Effect and Technique to use to draw the Particles
        /// </summary>
        /// <param name="cEffect">The Effect to use</param>
        /// <param name="sTechnique">The name of the Effect's Technique to use</param>
        public void SetEffectAndTechnique(Effect cEffect, string sTechnique)
        {
            // Set the Effect
            Effect = cEffect;

            // Set the Technique
            SetTechnique(sTechnique);
        }

        /// <summary>
        /// Get / Set the Effect to use to draw the Particles
        /// </summary>
        public Effect Effect
        {
            get { return mcEffect; }
            set
            {
                // If the Effect should be set to null
                if (value == null)
                {
                    // If this Type of Particle does not require an Effect to be used
                    if (ParticleType == ParticleTypes.None ||
                        ParticleType == ParticleTypes.Sprite)
                    {
                        // Set the Effect to null
                        mcEffect = null;
                    }
                    // Else an Effect must be specified for this Type of Particle
                    else
                    {
                        throw new ArgumentNullException("Effect", "Specified Effect to use is null. A valid Effect must be used to draw the current Type of Particles.");
                    }
                }
                // Else a valid Effect was specified
                else
                {
                    // If we have several particle systems, the content manager will return
                    // a single shared effect instance to them all. But we want to preconfigure
                    // the effect with parameters that are specific to this particular
                    // particle system. By cloning the effect, we prevent one particle system
                    // from stomping over the parameter settings of another.
                    mcEffect = value.Clone(GraphicsDevice);
                }
            }
        }

        /// <summary>
        /// Set which Technique of the current Effect to use to draw the Particles
        /// </summary>
        /// <param name="sTechnique">The name of the Effect's Technique to use</param>
        public void SetTechnique(string sTechnique)
        {
            // If the Effect hasn't been set yet
            if (mcEffect == null)
            {
                throw new Exception("Effect is null when trying to specify the Technique to use. The Effect must be set before specifying the Technique.");
            }

            // If the specified Technique is invalid
            if (sTechnique == null || sTechnique.Equals(""))
            {
                throw new ArgumentException("The specified Technique to use is invalid. This parameter cannot be null or an empty string.", "sTechnique");
            }

            // Else both the Effect and Technique to use are valid
            // Store which Technique to use to draw
            mcEffect.CurrentTechnique = mcEffect.Techniques[sTechnique];
        }

        /// <summary>
        /// Get / Set which Technique of the current Effect to use to draw the Particles
        /// </summary>
        public EffectTechnique Technique
        {
            get 
            {
                // If the Effect has been set already
                if (mcEffect != null)
                {
                    return mcEffect.CurrentTechnique;
                }
                // Else the Effect has not been set yet
                // So return null
                return null;
            }
            set 
            {
                // If the Effect hasn't been set yet
                if (mcEffect == null)
                {
                    throw new Exception("Effect is null when trying to specify the Technique to use. The Effect must be set before specifying the Technique.");
                }

                // If the Technique to use is invalid
                if (value == null)
                {
                    throw new ArgumentNullException("Technique", "An invalid Technique to use was specified. The Technique to use cannot be null.");
                }

                // Set the Technique to use
                mcEffect.CurrentTechnique = value;
            }
        }

        /// <summary>
        /// Set the Texture to use to draw the Particles
        /// </summary>
        /// <param name="sTexture">The Asset Name of the texture file to use (found in
        /// the XNA Properties of the file)</param>
        public void SetTexture(string sTexture)
        {
            // If the Texture to use is invalid
            if (sTexture == null || sTexture.Equals(""))
            {
                // If a Texture is required for this Type of Particle
                if (ParticleType == ParticleTypes.Sprite || ParticleType == ParticleTypes.PointSprite ||
                    ParticleType == ParticleTypes.TexturedQuad)
                {
                    throw new ArgumentNullException("sTexture", "Specified Texture to use is null. A valid Texture must be set to draw the current Type of Particles.");
                }
                // Else a Texture is not required
                else
                {
                    // Set the Texture to null
                    Texture = null;
                }
            }
            // Else the Texture to use is valid
            else
            {
                // Save a handle to the Texture and save the Name of the Texture
                mcTexture = ContentManager.Load<Texture2D>(sTexture);
                mcTexture.Name = sTexture;
            }
        }

        /// <summary>
        /// Get / Set the Texture to use to draw the Particles
        /// </summary>
        public Texture2D Texture
        {
            get { return mcTexture; }
            set
            {
                // If the Texture should be set to null
                if (value == null)
                {
                    // If a Texture is required for this Type of Particle
                    if (ParticleType == ParticleTypes.Sprite || ParticleType == ParticleTypes.PointSprite ||
                    ParticleType == ParticleTypes.TexturedQuad)
                    {
                        throw new ArgumentNullException("sTexture", "Specified Texture to use is null. A valid Texture must be set to draw the current Type of Particles.");
                    }
                    // Else a Texture is not required
                    else
                    {
                        // Set the Texture to null
                        mcTexture = null;
                    }
                }
                // Else a valid Texture was specified
                else
                {
                    mcTexture = value;
                }
            }
        }

        /// <summary>
        /// Get / Set how fast the Particle System Simulation should run.
        /// <para>1.0 = normal speed, 0.5 = half speed, 2.0 = double speed.</para>
        /// <para>NOTE: If a negative value is specified, the Speed Scale is set 
        /// to zero (pauses the simulation; has same effect as Enabled = false).</para>
        /// </summary>
        public float SimulationSpeed
        {
            get { return mfSimulationSpeed; }
            set
            {
                // Make sure the Simulation Speed is not negative
                if (value < 0.0f)
                {
                    mfSimulationSpeed = 0.0f;
                }
                else
                {
                    mfSimulationSpeed = value;
                }
            }
        }

        /// <summary>
        /// Get / Set how fast the Particle System Simulation should run to look "normal".
        /// <para>1.0 = normal speed, 0.5 = half speed, 2.0 = double speed.</para>
        /// <para>This is provided as a way of speeding up / slowing down the simulation to have 
        /// it look as desired, without having to rescale all of the particle velocities, etc.</para>
        /// <para>NOTE: If a negative value is specified, the Internal Simulation Speed is set to zero 
        /// (pauses the simulation; has the same effect as Enabled = false).</para>
        /// </summary>
        public float InternalSimulationSpeed
        {
            get { return mfInternalSimulationSpeed; }
            set
            {
                // Make sure the Internal Simulation Speed is not negative
                if (value < 0.0f)
                {
                    mfInternalSimulationSpeed = 0.0f;
                }
                else
                {
                    mfInternalSimulationSpeed = value;
                }
            }
        }

        /// <summary>
        /// Specify how often the Particle System should be Updated.
        /// <para>NOTE: Specifying a value of zero (default) will cause the Particle 
        /// System to be Updated every time the Update() function is called 
        /// (i.e. as often as possible).</para>
        /// <para>NOTE: If the Update() function is not called often enough to
        /// keep up with this specified Update rate, the Update function
        /// updates the Particle Systems as often as possible.</para>
        /// </summary>
        public int UpdatesPerSecond
        {
            get
            {
                // If the Particle Systems should be Updated as often as possible
                if (mfTimeToWaitBetweenUpdates == 0.0f)
                {
                    return 0;
                }
                // Else return how many times the Particle Systems are being Updated Per Second
                else
                {
                    return (int)(1.0f / mfTimeToWaitBetweenUpdates);
                }
            }

            set
            {
                // If the Particle Systems should be Updated as often as possible
                if (value <= 0)
                {
                    mfTimeToWaitBetweenUpdates = 0.0f;
                }
                // Else calculate how much Time should elapse between Particle System Updates
                else
                {
                    mfTimeToWaitBetweenUpdates = (1.0f / value);
                }
            }
        }

        /// <summary>
        /// The Particle System Manager whose properties (SimulationSpeed and 
        /// UpdatesPerSecond) this particle system should follow.  If null is not specified,
        /// the Manager's properties will be copied into this particle system immediately.
        /// <para>NOTE: This Particle System's properties will only clone the Manager's properties
        /// if the Manager's properties are Enabled. For example, the Manager's SimulationSpeed
        /// will only be copied to this Particle System if the Manager's SimulationSpeedIsEnabled
        /// property is true.</para>
        /// <para>NOTE: This value is automatically set to the last Particle System Manager the 
        /// Particle System is added to.</para>
        /// </summary>
        public ParticleSystemManager ParticleSystemManagerToCopyPropertiesFrom
        {
            get { return mcParticleSystemManager; }
            set
            {
                // Save a handle to the Particle System Manager to copy from
                mcParticleSystemManager = value;

                // If there is a Particle System Manager whose properties should be copied
                if (mcParticleSystemManager != null)
                {
                    // If the Particle System Manager's SimulationSpeed should be copied to this Particle System
                    if (mcParticleSystemManager.SimulationSpeedIsEnabled)
                    {
                        SimulationSpeed = mcParticleSystemManager.SimulationSpeed;
                    }

                    // If the Particle System Manager's UpdatesPerSecond should be copied to this Particle System
                    if (mcParticleSystemManager.UpdatesPerSecondIsEnabled)
                    {
                        UpdatesPerSecond = mcParticleSystemManager.UpdatesPerSecond;
                    }
                }
            }
        }

        /// <summary>
        /// Get the type of Particles that this Particle System should draw.
        /// </summary>
        public ParticleTypes ParticleType
        {
            get { return meParticleType; }

            // This allocates the proper amount of space for the Particles and initializes the 
            // variables used to draw the Type of Particles specified. For example, if 
            // using Textured Quads extra space will need to be allocated to hold the Particles, 
            // as each Quad Particle requires four vertices, not one like Point Sprites. Also, 
            // the Index Buffer would be initialized, as it is required to draw Quads.
            private set
            {
                // Set the type of Particles that should be drawn
                meParticleType = value;

                // If the Max Number of Particles has been specified
                if (mcParticles != null)
                {
                    // Get the Maximum Number of Particles this Particle System can hold
                    int iMaxNumberOfParticles = mcParticles.Length;

                    switch (meParticleType)
                    {
                        // If the Particle Type is not set or is set to NoDisplay
                        default:
                        case ParticleTypes.None:
                        case ParticleTypes.NoDisplay:
                            // Destroy the Vertex Buffer and related variables
                            mcParticleVerticesToDraw = null;
                            miIndexBufferArray = null;
                            mcSpriteBatch = null;
                            mcParticleSpritesToDraw = null;
                            mcSpriteBatchSettings = null;
                        break;

                        // If we are using Point Sprites or Pixels
                        case ParticleTypes.Pixel:
                        case ParticleTypes.PointSprite:
                            // Initialize the Vertex Buffer
                            mcParticleVerticesToDraw = new Vertex[iMaxNumberOfParticles];

                            // Destroy the Index Buffer and Sprite Batch variables since
                            // they are not used with Point Sprites or Pixels
                            miIndexBufferArray = null;
                            mcSpriteBatch = null;
                            mcParticleSpritesToDraw = null;
                            mcSpriteBatchSettings = null;
                        break;

                        // If we are using Quads or Textured Quads
                        case ParticleTypes.Quad:
                        case ParticleTypes.TexturedQuad:
                            // Initialize the Vertex and Index Buffer
                            mcParticleVerticesToDraw = new Vertex[iMaxNumberOfParticles * 4];
                            miIndexBufferArray = new int[iMaxNumberOfParticles * 6];

                            // Destroy the Sprite Batch variables since they are not needed with Quads
                            mcSpriteBatch = null;
                            mcParticleSpritesToDraw = null;
                            mcSpriteBatchSettings = null;
                        break;

                        // If we are using the Sprite Batch to draw Particles
                        case ParticleTypes.Sprite:
                            // Initialize the Sprite Batch
                            mcSpriteBatch = new SpriteBatch(GraphicsDevice);

                            // Initialize the Linked List used to hold the Sprites to draw
                            mcParticleSpritesToDraw = new LinkedList<Particle>();

                            // Initialize the Sprite Batch drawing Settings
                            mcSpriteBatchSettings = new SpriteBatchSettings();

                            // Destroy the Particle Vertices and Index Buffer since they are not needed
                            mcParticleVerticesToDraw = null;
                            miIndexBufferArray = null;
                        break;
                    }

                    // Initialize all Particles and add them to the Inactive Particles List
                    int iNumberOfParticles = mcParticles.Length;
                    for (int iIndex = 0; iIndex < iNumberOfParticles; iIndex++)
                    {
                        // Initialize the Particle
                        mcParticles[iIndex] = new Particle();
                        mcInactiveParticlesList.AddFirst(mcParticles[iIndex]);
                    }

                    // If the Vertex Buffer is used for the current Type of Particles
                    if (mcParticleVerticesToDraw != null)
                    {
                        // Initialize the Vertex Buffer
                        int iNumberOfVertices = mcParticleVerticesToDraw.Length;
                        for (int iIndex = 0; iIndex < iNumberOfVertices; iIndex++)
                        {
                            // Initialize the Particle's Vertex
                            mcParticleVerticesToDraw[iIndex] = new Vertex();
                        }
                    }
                }
                // Else we do not know the Max Number of Particles yet
                else
                {
                    // So make sure the vertex buffer variables are not initialized
                    mcParticles = null;
                    mcParticleVerticesToDraw = null;
                    miIndexBufferArray = null;
                    mcSpriteBatch = null;
                    mcParticleSpritesToDraw = null;
                    mcSpriteBatchSettings = null;
                }

                // Make sure we don't try to Draw any Particles until the Vertex Buffer has been refilled
                miNumberOfParticlesToDraw = 0;
            }
        }

        /// <summary>
        /// Get / Set the absolute Number of Particles to Allocate Memory for.
        /// <para>NOTE: This value must be greater than or equal to zero.</para>
        /// <para>NOTE: Even if this many particles aren't used, the space for this many Particles 
        /// is still allocated in memory.</para>
        /// </summary>
        public int NumberOfParticlesAllocatedInMemory
        {
            get 
            {
                // If the Max Number Of Particles has been set
                if (mcParticles != null)
                {
                    // Return the Max Number Of Particles allowed by this Particle System
                    return mcParticles.Length;
                }
                // Else the Max Number Of Particles hasn't been set yet
                else
                {
                    // So return zero
                    return 0;
                }
            }

            set
            {
                // Temporarily store the New Max Number of Particles
                int iNewMaxNumberOfParticles = value;

                // If a valid Max Number of Particles was specified
                if (iNewMaxNumberOfParticles >= 0)
                {
                    // Varaible to tell if there are Particles that need to be copied to the new Active Particles List
                    bool bThereAreOldParticlesToCopy = false;
                    
                    // If there are Active Particles to backup
                    if (ActiveParticles != null && ActiveParticles.Count > 0)
                    {
                        // Save that the Particles will need to be copied over to the new Particle array
                        bThereAreOldParticlesToCopy = true;
                    }

                    // Save handles to the current Particles array and Active Particles List so they aren't
                    // lost right away, in case we need to copy Particles over to the new Active Particles List
                    Particle[] cOldParticles = mcParticles;
                    LinkedList<Particle> cOldActiveParticles = ActiveParticles;

                    // Save handles to the Vertex and Index Buffers as well to restore any vertices that
                    // are currently being drawn. This is done to avoid a flicker, since these buffers
                    // won't be populated again until the next Update() call, but Draw() may get called
                    // several times before then.
                    Vertex[] cOldParticleVertices = mcParticleVerticesToDraw;
                    int[] iOldIndexBuffer = miIndexBufferArray;
                    int iNumberOfParticlesBeingDrawn = miNumberOfParticlesToDraw;
                    LinkedList<Particle> cOldParticleSpritesToDraw = mcParticleSpritesToDraw;
                    SpriteBatchSettings cOldSpriteBatchSettings = mcSpriteBatchSettings;


                    // Initialize the size of the array to the maximum number of Particles
                    mcParticles = new Particle[iNewMaxNumberOfParticles];

                    // Initialize the Particle lists
                    mcActiveParticlesList = new LinkedList<Particle>();
                    mcInactiveParticlesList = new LinkedList<Particle>();

                    // Allocate the right amount of space for this Type of Particle
                    // and initialize the variables to hold the Particle vertices
                    ParticleType = meParticleType;


                    // If there are Particles to copy to the new Active Particle List
                    if (bThereAreOldParticlesToCopy)
                    {
                        // If there are more Particles Being Drawn than there are now allowed
                        if (iNumberOfParticlesBeingDrawn > iNewMaxNumberOfParticles)
                        {
                            // Set the Number Of Particles Being Drawn to the new Max Number Of Particles
                            iNumberOfParticlesBeingDrawn = iNewMaxNumberOfParticles;
                        }

                        // Loop through all of the old Active Particles and add them to the
                        // new Active Particles List. We loop through them in reverse order
                        // to maintain the order of the Active Particles, since the AddParticle() 
                        // function adds new Particles to the front of the List.
                        LinkedListNode<Particle> cNode = cOldActiveParticles.Last;
                        while (cNode != null && AddParticle(cNode.Value))
                        {
                            // Move to the previous Active Particle
                            cNode = cNode.Previous;
                        }

                        // If there are Particles currently being drawn
                        if (iNumberOfParticlesBeingDrawn > 0)
                        {
                            // Copy the old draw information to the new draw buffers
                            switch (meParticleType)
                            {
                                // If the Particle Type is not set
                                default:
                                case ParticleTypes.None:
                                    // Do nothing
                                break;

                                // If we are using Point Sprites or Pixels
                                case ParticleTypes.Pixel:
                                case ParticleTypes.PointSprite:
                                    // Copy the old Vertices into the new Vertices array
                                    for (int iIndex = 0; iIndex < iNumberOfParticlesBeingDrawn; iIndex++)
                                    {
                                        mcParticleVerticesToDraw[iIndex] = cOldParticleVertices[iIndex];
                                    }
                                break;

                                // If we are using Quads or Textured Quads
                                case ParticleTypes.Quad:
                                case ParticleTypes.TexturedQuad:
                                    // Copy the old Vertices into the new Vertices array
                                    int iNumberOfVerticesToCopy = iNumberOfParticlesBeingDrawn * 4;
                                    for (int iIndex = 0; iIndex < iNumberOfVerticesToCopy; iIndex++)
                                    {
                                        mcParticleVerticesToDraw[iIndex] = cOldParticleVertices[iIndex];
                                    }

                                    // Copy the old Index Buffer into the new one
                                    int iNumberOfIndicesToCopy = iNumberOfParticlesBeingDrawn * 6;
                                    for (int iIndex = 0; iIndex < iNumberOfIndicesToCopy; iIndex++)
                                    {
                                        miIndexBufferArray[iIndex] = iOldIndexBuffer[iIndex];
                                    }
                                break;

                                // If we are using the Sprite Batch to draw Particles
                                case ParticleTypes.Sprite:
                                    // Restore the Sprites being Drawn
                                    mcParticleSpritesToDraw = cOldParticleSpritesToDraw;

                                    // Restore the Sprite Batch drawing Settings if it existed
                                    if (cOldSpriteBatchSettings != null)
                                    {
                                        mcSpriteBatchSettings = cOldSpriteBatchSettings;
                                    }
                                break;
                            }

                            // Record how many Particles should still be drawn
                            miNumberOfParticlesToDraw = iNumberOfParticlesBeingDrawn;
                        }
                    }
                }
                // Else an invalid Max Number of Particles was specified
                else
                {
                    throw new ArgumentException("MaxNumberOfParticles", "The specified Max Number Of Particles is less than or equal to zero. The Max Number Of Particles must be greater than zero.");
                }
            }
        }

        /// <summary>
        /// Get / Set the Max Number of Particles this Particle System is Allowed to contain at any given time.
        /// <para>NOTE: The Automatic Memory Manager will never allocate space for more Particles than this.</para>
        /// <para>NOTE: This value must be greater than or equal to zero.</para>
        /// </summary>
        public int MaxNumberOfParticlesAllowed
        {
            get { return miMaxNumberOfParticlesAllowed; }
            set
            {
                // Get the specified Number of Allowed Particles
                int iNewNumberOfParticlesAllowed = value;

                // Make sure the New Virtual Max is valid
                if (iNewNumberOfParticlesAllowed < 0)
                {
                    iNewNumberOfParticlesAllowed = 0;
                }

                // Set the new Number of Particles Allowed
                miMaxNumberOfParticlesAllowed = iNewNumberOfParticlesAllowed;
            }
        }

        /// <summary>
        /// Get the number of Particles that are currently Active
        /// </summary>
        public int NumberOfActiveParticles
        {
            get
            {
                // If the Particle System is Initialized
                if (IsInitialized())
                {
                    // Return the number of Active Particles
                    return mcActiveParticlesList.Count;
                }
                // Else the Active Particles List does not exist
                else
                {
                    // Return that there are no Active Particles
                    return 0;
                }
            }
        }

        /// <summary>
        /// Get the number of Particles being Drawn. That is, how many Particles 
        /// are both Active AND Visible.
        /// </summary>
        public int NumberOfParticlesBeingDrawn
        {
            get { return miNumberOfParticlesToDraw; }
        }

        /// <summary>
        /// Get the number of Particles that may still be added before reaching the
        /// Max Number Of Particles Allowed. If the Max Number Of Particles Allowed is 
        /// greater than the Number Of Particles Allocated In Memory AND the Auto Memory Manager is
        /// set to not increase the amount of Allocated Memory, than this returns the number 
        /// of Particles that may still be added before running out of Memory.
        /// </summary>
        public int NumberOfParticlesStillPossibleToAdd
        {
            get 
            {
                // If the Memory Allocated is less than the Max Number Of Particles Allowed AND
                // the Particle System cannot automatically increase the amount of Memory Allocated
                if (NumberOfParticlesAllocatedInMemory < MaxNumberOfParticlesAllowed &&
                    AutoMemoryManagerSettings.MemoryManagementMode != AutoMemoryManagerModes.IncreaseAndDecrease &&
                    AutoMemoryManagerSettings.MemoryManagementMode != AutoMemoryManagerModes.IncreaseOnly)
                {
                    // Return how many Particles can be added before we run out of Memory
                    return NumberOfParticlesAllocatedInMemory - NumberOfActiveParticles;
                }
                // Else we have enough Memory, or the Memory can be increased if needed
                else
                {
                    // Return how many Particles can be added before reaching the Max Number Of Paticles Allowed
                    return (MaxNumberOfParticlesAllowed - NumberOfActiveParticles);
                }
            }
        }

        /// <summary>
        /// Get / Protected Set a Linked List whose Nodes point to the Active Particles.
        /// <para>NOTE: The Protected Set option is only provided to allow the order of the 
        /// LinkedListNodes to be changed, changing the update and drawing 
        /// order of the Particles. Be sure that all of the original LinkedListNodes 
        /// (and only the original LinkedListNodes, no more) obtained from the 
        /// Get are included; they may only be rearranged. If they are not, 
        /// there may (and probably will) be unexpected results.</para>
        /// </summary>
        public LinkedList<Particle> ActiveParticles
        {
            get { return mcActiveParticlesList; }
            protected set { mcActiveParticlesList = value; }
        }

        /// <summary>
        /// Returns a Linked List whose Nodes point to the Inactive Particles
        /// </summary>
        public LinkedList<Particle> InactiveParticles
        {
            get { return mcInactiveParticlesList; }
        }

        /// <summary>
        /// Returns the array of all Particle objects
        /// </summary>
        public Particle[] Particles
        {
            get { return mcParticles; }
        }

        /// <summary>
        /// Initialize the given Particle using the current Initialization Function
        /// </summary>
        /// <param name="cParticle">The Particle to Initialize</param>
        public void InitializeParticle(Particle cParticle)
        {
            mcParticleInitializationFunction(cParticle);
        }

        /// <summary>
        /// Adds a new Particle to the particle system, at the start of the Active Particle List. 
        /// This new Particle is initialized using the particle system's Particle Initialization Function
        /// </summary>
        /// <returns>True if a particle was added, False if there is not enough memory for another Particle</returns>
        public bool AddParticle()
        {
            return AddParticle(null);
        }

        /// <summary>
        /// Adds a new Particle to the particle system, at the start of the Active Particle List. Returns true if
        /// the Particle was added, false if there is not enough memory for another Particle.
        /// </summary>
        /// <param name="cParticleToCopy">The Particle to add to the Particle System. If this is null then a
        /// new Particle is initialized using the particle system's Particle Initialization Function</param>
        /// <returns>True if a particle was added, False if there is not enough memory for another Particle</returns>
        public bool AddParticle(Particle cParticleToCopy)
        {
            // If there are no more Particles that can be initialized OR
            // we have reached the Max Number of Particles Allowed
            if (mcInactiveParticlesList.Count <= 0 || mcActiveParticlesList.Count >= miMaxNumberOfParticlesAllowed)
            {
                // If the problem is that we have reached the Max Number Of Particles AND 
                // the Max Number Of Particles is not set to zero (i.e. not allowed to increase it)
                if (mcInactiveParticlesList.Count <= 0 && NumberOfParticlesAllocatedInMemory != 0)
                {
                    // If Auto Memory Management is Enabled to allow the increasing of memory AND 
                    // we haven't reached the Absolute Max Number Of Particles yet
                    if (NumberOfParticlesAllocatedInMemory < MaxNumberOfParticlesAllowed &&
                        (AutoMemoryManagerSettings.MemoryManagementMode == AutoMemoryManagerModes.IncreaseAndDecrease ||
                         AutoMemoryManagerSettings.MemoryManagementMode == AutoMemoryManagerModes.IncreaseOnly))
                    {
                        // Calculate what the new increased Max Number Of Particles should be
                        int iNewMaxNumberOfParticles = (int)(Math.Ceiling((float)(NumberOfParticlesAllocatedInMemory * AutoMemoryManagerSettings.IncreaseAmount)));

                        // Make sure we do not allocate more than the Max Number Of Particles
                        iNewMaxNumberOfParticles = (int)MathHelper.Min(iNewMaxNumberOfParticles, MaxNumberOfParticlesAllowed);

                        // Set the new Max Number Of Particles
                        NumberOfParticlesAllocatedInMemory = iNewMaxNumberOfParticles;
                    }
                    // Else we cannot increase the Max Number Of Particles
                    else
                    {
                        // Return that we cannot add a new Particle at this time
                        return false;
                    }
                }
                // Else the problem is that we have hit our Number Of Particles Allowed (and it is not set to the Max Number Of Particles)
                else
                {
                    // Return that we cannot add a new Particle at this time
                    return false;
                }
            }

            // Do any user operations before Adding the new Particle
            BeforeAddParticle();

            // Get an Inactive Particle Node and remove it from the Inactive List
            LinkedListNode<Particle> cNode = mcInactiveParticlesList.Last;
            mcInactiveParticlesList.RemoveLast();

            // Get a handle to the Particle 
            Particle cParticle = cNode.Value;

            // Initialize the Particle
            cParticle.Reset();

            // If a Particle was not given to add
            if (cParticleToCopy == null)
            {
                // If a Particle Initialization Function has been specified
                if (mcParticleInitializationFunction != null)
                {
                    // Initialize the Particle using the Initialization Function
                    mcParticleInitializationFunction(cParticle);
                }
            }
            // Else a specific Particle was given to add
            else
            {
                // So Copy the given Particle properties into this Particle
                cParticle.CopyFrom(cParticleToCopy);
            }

            // Add the Particle to the Active Particles List
            mcActiveParticlesList.AddFirst(cNode);

            // Do any user operations after Adding the new Particle
            AfterAddParticle();

            // Return that a Particle was added
            return true;
        }

        /// <summary>
        /// Adds the specified number of new Particles to the particle system. 
        /// These new Particles are initialized using the particle systems Particle Initialization Function
        /// </summary>
        /// <param name="iNumberOfParticlesToAdd">How many Particles to Add to the particle system</param>
        /// <returns>Returns how many Particles were able to be added to the particle system</returns>
        public int AddParticles(int iNumberOfParticlesToAdd)
        {
            return AddParticles(iNumberOfParticlesToAdd, null);
        }

        /// <summary>
        /// Adds the specified number of new Particles to the particle system, copying the 
        /// properties of the given Particle To Copy
        /// </summary>
        /// <param name="iNumberOfParticlesToAdd">How many copyies of the Particle To Copy to Add 
        /// to the particle system</param>
        /// <param name="cParticleToCopy">The Particle to copy from when Adding the Particles to the 
        /// Particle System. If this is null then the new Particles will be initialized using the 
        /// particle system's Particle Initialization Function</param>
        /// <returns>Returns how many Particles were able to be added to the particle system</returns>
        public int AddParticles(int iNumberOfParticlesToAdd, Particle cParticleToCopy)
        {
            // The number of Particles added to the Particle System
            int iParticleCount = 0;

            // While we haven't added the specified number of Particles, and the Particle System is not full
            while (iParticleCount < iNumberOfParticlesToAdd && AddParticle(cParticleToCopy))
            {
                // Increment the number of Particles we were able to add so far
                iParticleCount++;
            }

            // Return how many Particles we were ablet to add to the Particle System
            return iParticleCount;
        }

        /// <summary>
        /// Removes all Active Particles from the Active Particle List and adds them 
        /// to the Inactive Particle List
        /// </summary>
        public void RemoveAllParticles()
        {
            // Loop until all Active Particles have been removed
            while (mcActiveParticlesList.Count > 0)
            {
                // Save a handle to the Node to remove
                LinkedListNode<Particle> cNodeToRemove = mcActiveParticlesList.First;

                // Remove the Node from the Active Particles List
                mcActiveParticlesList.Remove(cNodeToRemove);

                // Add it to the Inactive Particles List
                mcInactiveParticlesList.AddFirst(cNodeToRemove);
            }
        }

    // If inheriting from DrawableGameComponent, we need to override the Update() function
    #if (DPSFAsDrawableGameComponent)
        /// <summary>
        /// Overloaded DrawableGameComponent Update function.
        /// <para>Updates the Particle System. This involves executing the Particle System
        /// Events, updating all Active Particles according to the Particle Events, and 
        /// adding new Particles according to the Emitter settings.</para>
        /// <para>NOTE: This function should never be called manually by the user. It should
        /// only be called automatically by the Game object.</para>
        /// </summary>
        /// <param name="cGameTime">GameTime object used to determine how
        /// much time has elapsed since the last frame</param>
        public override void Update(GameTime cGameTime)
        {
            // Update the Particle System
            base.Update(cGameTime);
            Update((float)cGameTime.ElapsedGameTime.TotalSeconds, true);
        }
#endif

        /// <summary>
        /// Updates the Particle System. This involves executing the Particle System
        /// Events, updating all Active Particles according to the Particle Events, and 
        /// adding new Particles according to the Emitter settings.
        /// <para>NOTE: This will only Update the Particle System if it does not inherit from DrawableGameComponent, 
        /// since if it does it will be updated automatically by the Game object.</para>
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How much time in seconds has 
        /// elapsed since the last time this function was called</param>
        public void Update(float fElapsedTimeInSeconds)
        {
            Update(fElapsedTimeInSeconds, false);
        }

        /// <summary>
        /// Updates the Particle System, even if the the Particle Systems inherits from DrawableGameComponent.
        /// <para>Updating the Particle System involves executing the Particle System Events, updating all Active 
        /// Particles according to the Particle Events, and adding new Particles according to the Emitter settings.</para>
        /// <para>NOTE: If inheriting from DrawableGameComponent and this is called, the Particle System will be updated
        /// twice per frame; once when it is called here, and again when automatically called by the Game object.
        /// If not inheriting from DrawableGameComponent, this acts the same as calling Update().</para>
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How much time in seconds has 
        /// elapsed since the last time this function was called</param>
        public void UpdateForced(float fElapsedTimeInSeconds)
        {
            Update(fElapsedTimeInSeconds, true);
        }

        /// <summary>
        /// Updates the Particle System. This involves executing the Particle System
        /// Events, updating all Active Particles according to the Particle Events, and 
        /// adding new Particles according to the Emitter's settings.
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How much time in seconds has 
        /// elapsed since the last time this function was called</param>
        /// <param name="bCalledByDrawableGameComponent">Indicates if this function was
        /// called manually by the user or called automatically by the Drawable Game Component.
        /// If this function Inherits Drawable Game Component, but was not called by
        /// Drawable Game Component, nothing will be updated since the Particle System will
        /// automatically be updated when the Game Component's Update() function is called.</param>
        private void Update(float fElapsedTimeInSeconds, bool bCalledByDrawableGameComponent)
        {
            // If the Particle System is not Initialized OR no Particles should be updated OR
            // the Particle System is a Drawable Game Component and was called manually by the user
            // (we do nothing in this case because the Particle System will automatically be Updated 
            // by the Drawable Game Component).
            if (!IsInitialized() || !Enabled || 
                (InheritsDrawableGameComponent && !bCalledByDrawableGameComponent))
            {
                // Exit the function without updating anything
                return;
            }

            // Add the Elapsed Time since the last frame to our cumulative Time Elapsed Since Last Update
            mfTimeElapsedSinceLastUpdate += fElapsedTimeInSeconds;

            // If it's not time for the Particle System to be Updated yet
            if (mfTimeElapsedSinceLastUpdate < mfTimeToWaitBetweenUpdates)
            {
                // Exit the function without updating anything
                return;
            }


            // Update the cumulative Elpased Time
            mfTimeElapsedSinceLastUpdate -= mfTimeToWaitBetweenUpdates;

            // Calculate by how much Time the Particle System should be Updated
            float fParticleSystemUpdateTime = 0.0f;

            // If the Particle System is supposed to be Updated as often as possible
            if (mfTimeToWaitBetweenUpdates <= 0.0f)
            {
                // Use the actual Elapsed Time for the Particle System Update Time
                fParticleSystemUpdateTime = fElapsedTimeInSeconds;
            }
            // Else If this function is not being called fast enough to keep up
            // with how often the Particle Systems are supposed to be Updated
            // (i.e. There is a very low frame-rate, or the user specified a
            // very large number of Updates Per Second, such as 1000)
            else if (mfTimeElapsedSinceLastUpdate >= mfTimeToWaitBetweenUpdates)
            {
                // Use the amount of Time that has passed since the last time this function
                // was called for the Particle System Update Time, and reset the cumulative Elapsed Time
                fParticleSystemUpdateTime = (mfTimeElapsedSinceLastUpdate + mfTimeToWaitBetweenUpdates);
                mfTimeElapsedSinceLastUpdate = 0.0f;
            }
            // Else this function is being called often enough to keep up with 
            // the specified Update rate
            else
            {
                // Use the specified Update rate for the Particle System Update Time
                fParticleSystemUpdateTime = mfTimeToWaitBetweenUpdates;
            }
            

            // Calculate the scaled Elapsed Time
            float fScaledElapsedTimeInSeconds = fParticleSystemUpdateTime * SimulationSpeed * InternalSimulationSpeed;

            // Perform other functions before this Particle System is updated
            BeforeUpdate(fScaledElapsedTimeInSeconds);

            // Reset the count of how many Particles are still Active and Visible
            miNumberOfParticlesToDraw = 0;

            // If we are drawing Sprites
            if (mcParticleSpritesToDraw != null)
            {
                // Clear the list of Sprites to draw
                mcParticleSpritesToDraw.Clear();
            }

            // Reset the Index Buffer Index in case we are drawing Quads
            miIndexBufferIndex = 0;

            // Variable to keep track of how many Particles are added this frame
            int iNumberOfNewParticlesAdded = 0;


            // Update the Particle System according to its Events (before updating the Particles)
            ParticleSystemEvents.Update(fScaledElapsedTimeInSeconds);

            // Backup the Emitter's Position and Rotation before updating it
            Vector3 sOldEmitterPosition = mcEmitter.PositionData.Position;
            Quaternion sOldEmitterOrientation = mcEmitter.OrientationData.Orientation;

            // Update the Emitter and get how many Particles should be emitted
            int iNumberOfParticlesToEmit = mcEmitter.UpdateAndGetNumberOfParticlesToEmit(fScaledElapsedTimeInSeconds);

            // If some Particles should be emitted
            if (iNumberOfParticlesToEmit > 0)
            {
                // If the emitter is moving very fast, or the frame rate drops for
                // some reason, then many particles may be released in one location
                // (i.e. the emitter's new location) when they really should be spread
                // out between the emitters old location and the new one. To spread the
                // particles out, the location that particles are released is 
                // Linearly Interpolated (Lerp) between the emitters old and new location.
                // The orientation of the emitter is also Spherically Linearly Interpolated (Slerp)
                // for the similar case that the emitter is rotating very fast

                // Store the new Position and Orientation of the Emitter after Updating
                Vector3 sNewEmitterPosition = mcEmitter.PositionData.Position;
                Quaternion cNewEmitterOrientation = mcEmitter.OrientationData.Orientation;

                // Calculate the Step Size between releasing Particles
                float fStepSizeFraction = 1.0f / iNumberOfParticlesToEmit;

                // Emit the Particles
                int iParticlesEmitted = 0;
                bool bCanStillAddParticles = true;
                while (iParticlesEmitted < iNumberOfParticlesToEmit && bCanStillAddParticles)
                {
                    // Update the Number of Particles Emitted (even though we haven't actually added it yet)
                    iParticlesEmitted++;

                    // Update the Interpolated Position of the Emitter for the next Particle Emitted
                    mcEmitter.PositionData.Position = Vector3.Lerp(sOldEmitterPosition, sNewEmitterPosition, fStepSizeFraction * iParticlesEmitted);

                    // Update the Interpolated Orienation of the Emitter for the next Particle Emitted
                    mcEmitter.OrientationData.Orientation = Quaternion.Slerp(sOldEmitterOrientation, cNewEmitterOrientation, fStepSizeFraction * iParticlesEmitted);

                    // Add the new Particle, and record if we can still add more Particles or not
                    bCanStillAddParticles = AddParticle();

                    // If the Particle was added successfully
                    if (bCanStillAddParticles)
                    {
                        // Get a handle to the newly added Particle
                        Particle cParticle = ActiveParticles.First.Value;

                        // Calculate how much the just added Particle should be updated ("oldest" particles are added first)
                        float fUpdateAmount = MathHelper.Lerp(fScaledElapsedTimeInSeconds, 0.0f, fStepSizeFraction * iParticlesEmitted);

                        // Update the newly added Particle
                        cParticle.UpdateElapsedTimeVariables(fUpdateAmount);
                        ParticleEvents.Update(cParticle, fUpdateAmount);

                        // If the Particle is no longer Active already
                        if (!cParticle.IsActive())
                        {
                            // Remove the Particle from the Active Particle List
                            ActiveParticles.RemoveFirst();
                        }
                        // Else the Particle is still Active
                        else
                        {
                            // Add the Particle to the list of Particles to be drawn
                            AddParticleToVertexBuffer(cParticle);

                            // Increment the Number Of New Particles Added this frame
                            iNumberOfNewParticlesAdded++;
                        }
                    }
                }

                // Set the Emitter back to the Position and Orientation it should be at
                mcEmitter.PositionData.Position = sNewEmitterPosition;
                mcEmitter.OrientationData.Orientation = cNewEmitterOrientation;
            }

            // Get a handle to the first Active Particle Node
            LinkedListNode<Particle> cNode = mcActiveParticlesList.First;            

            // Skip any Particles that were just added to the Particle System, since they've already been updated
            for (int iIndex = 0; iIndex < iNumberOfNewParticlesAdded; iIndex++)
            {
                // Move to the Next Particle in the list
                cNode = cNode.Next;
            }

            // Loop until all Nodes in the Active Particle List have been processed
            while (cNode != null)
            {
                // Get a handle to the Particle
                Particle cParticle = cNode.Value;

                // Update the Particle's Elapsed Time Variables
                cParticle.UpdateElapsedTimeVariables(fScaledElapsedTimeInSeconds);

                // Update the Particle according to the Particle Events
                ParticleEvents.Update(cParticle, fScaledElapsedTimeInSeconds);

                // If the Particle is no longer Active
                if (!cParticle.IsActive())
                {
                    // Save a handle to the Node to remove
                    LinkedListNode<Particle> cNodeToRemove = cNode;

                    // Move to the Next Node in the Active Particle List
                    cNode = cNode.Next;

                    // Remove the Node from the Active Particles List
                    mcActiveParticlesList.Remove(cNodeToRemove);

                    // Add it to the Inactive Particles List
                    mcInactiveParticlesList.AddFirst(cNodeToRemove);
                }
                // Else the Particle is still Active
                else
                {
                    // Add the Particle to the list of Particles to be drawn
                    AddParticleToVertexBuffer(cParticle);

                    // Move to the Next Node in the Active Particle List
                    cNode = cNode.Next;
                }
            }

            // Remove all OneTime Events now that they've been executed
            ParticleEvents.RemoveAllOneTimeEvents();

            // If the Auto Memory Manager is Enabled to allow Decrease of Memory
            if (AutoMemoryManagerSettings.MemoryManagementMode == AutoMemoryManagerModes.IncreaseAndDecrease ||
                AutoMemoryManagerSettings.MemoryManagementMode == AutoMemoryManagerModes.DecreaseOnly)
            {
                // Update the Auto Memory Manager Timer
                mfAutoMemoryManagersElapsedTime += fElapsedTimeInSeconds;

                // If there are more Active Particles than our previous Max
                if (NumberOfActiveParticles > miAutoMemoryManagerMaxNumberOfParticlesActiveAtOnceOverTheLastXSeconds)
                {
                    // Save the new Max value and reset the Timer
                    miAutoMemoryManagerMaxNumberOfParticlesActiveAtOnceOverTheLastXSeconds = NumberOfActiveParticles;
                    mfAutoMemoryManagersElapsedTime = 0.0f;
                }

                // If it is time to check if the memory should be reduced
                if (mfAutoMemoryManagersElapsedTime >= AutoMemoryManagerSettings.SecondsMaxNumberOfParticlesMustExistForBeforeReducingSize)
                {
                    // Calculate what the new reduced Max Number Of Particles should be
                    int iNewMaxNumberOfParticles = (int)(Math.Ceiling((float)(miAutoMemoryManagerMaxNumberOfParticlesActiveAtOnceOverTheLastXSeconds * AutoMemoryManagerSettings.ReduceAmount)));

                    // Make sure we do not Decrease it less than the Absolute Min
                    iNewMaxNumberOfParticles = (int)MathHelper.Max(iNewMaxNumberOfParticles, AutoMemoryManagerSettings.AbsoluteMinNumberOfParticles);

                    // If the new Max Number Of Particles is less than what we have currently
                    if (iNewMaxNumberOfParticles < NumberOfParticlesAllocatedInMemory)
                    {
                        // Set the new Max Number Of Particles
                        NumberOfParticlesAllocatedInMemory = iNewMaxNumberOfParticles;
                    }

                    // Reset the Max Number Of Active Particles Over The Last X Seconds
                    miAutoMemoryManagerMaxNumberOfParticlesActiveAtOnceOverTheLastXSeconds = 0;
                }
            }

            // If the Particle System has reached the end of its Lifetime and is supposed to Destroy itself
            if (ParticleSystemEvents.LifetimeData.NormalizedElapsedTime >= 1.0 &&
                ParticleSystemEvents.LifetimeData.EndOfLifeOption == CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Destroy)
            {
                // Destroy the Particle System
                Destroy();
            }

            // Perform any other functions now that this Particle System has been Updated
            AfterUpdate(fScaledElapsedTimeInSeconds);
        }

        /// <summary>
        /// Adds the given Particle to the list of Particles to be Drawn (i.e. the Vertex Buffer), if it is Visible
        /// </summary>
        /// <param name="cParticle">The Particle to add to the Vertex Buffer</param>
        private void AddParticleToVertexBuffer(Particle cParticle)
        {
            // If the Particle should be drawn
            if (cParticle.Visible)
            {
                switch (meParticleType)
                {
                    default:
                    case ParticleTypes.NoDisplay:
                        // Do Nothing
                    break;

                    case ParticleTypes.Pixel:
                    case ParticleTypes.PointSprite:
                        // Set the Particle Vertex's properties
                        mcVertexUpdateFunction(ref mcParticleVerticesToDraw, miNumberOfParticlesToDraw, cParticle);
                    break;

                    case ParticleTypes.Quad:
                    case ParticleTypes.TexturedQuad:
                        // Set the Quad's 4 Vertex properties
                        // A Quad uses 4 Vertices, so we must offset the index for this
                        mcVertexUpdateFunction(ref mcParticleVerticesToDraw, miNumberOfParticlesToDraw * 4, cParticle);
                    break;

                    case ParticleTypes.Sprite:
                        // Add this Particle to the list of Sprites to Draw
                        mcParticleSpritesToDraw.AddLast(cParticle);
                    break;
                }

                // Increment the number of Particles found to still be Active and Visible
                miNumberOfParticlesToDraw++;
            }
        }

    // If inheriting from DrawableGameComponent, we need to override the Draw() function
    #if (DPSFAsDrawableGameComponent)
        /// <summary>
        /// Overloaded DrawableGameComponent Draw function.
        /// Draws all of the Active Particles to the Graphics Device.
        /// <para>NOTE: This function should never be called manually by the user. It should
        /// only be called automatically by the Game object.</para>
        /// </summary>
        /// <param name="cGameTime">GameTime object used to determine
        /// how much time has elapsed since the last frame</param>
        public override void Draw(GameTime cGameTime)
        {
            // Draw the Particle System
            base.Draw(cGameTime);
            Draw(true);
        }
#endif

        /// <summary>
        /// Draws all of the Active Particles to the Graphics Device.
        /// <para>NOTE: This will only Draw the Particle System if it does not inherit from DrawableGameComponent, 
        /// since if it does it will be drawn automatically by the Game object.</para>
        /// </summary>
        public void Draw()
        {
            // Draw the Particle System
            Draw(false);
        }

        /// <summary>
        /// Draws all of the Active Particles to the Graphics Device, even if the the Particle Systems inherits
        /// from DrawableGameComponent.
        /// <para>NOTE: If inheriting from DrawableGameComponent and this is called, the Particle System will be drawn
        /// twice per frame; once when it is called here, and again when automatically called by the Game object.
        /// If not inheriting from DrawableGameComponent, this acts the same as calling Draw().</para>
        /// </summary>
        public void DrawForced()
        {
            // Force the drawing of the Particle System
            Draw(true);
        }

        /// <summary>
        /// Draws all of the Active Particles to the Graphics Device
        /// </summary>
        /// <param name="bCalledByDrawableGameComponent">Indicates if this function was
        /// called manually by the user or called automatically by the Drawable Game Component.
        /// If this function Inherits Drawable Game Component, but was not called by
        /// Drawable Game Component, nothing will be drawn since the Particle System will
        /// automatically be drawn when the Game Component's Draw() function is called.</param>
        private void Draw(bool bCalledByDrawableGameComponent)
        {
            // If the Partilce System is not Initialized OR no Particles should be drawn OR
            // the Particle System is a Drawable Game Component and was called manually by the user
            // (we do nothing in this case because the Particle System will automatically be Drawn 
            // by the Drawable Game Component).
            if (!IsInitialized() || !Visible || ParticleType == ParticleTypes.NoDisplay ||
                (InheritsDrawableGameComponent && !bCalledByDrawableGameComponent))
            {
                // Exit the function without drawing anything
                return;
            }

            // Perform other functions before this Particle System is drawn
            BeforeDraw();

            // If there are no Particles to draw
            if (miNumberOfParticlesToDraw <= 0)
            {
                // Perform the AfterDraw() operations before exiting
                AfterDraw();

                // Exit without drawing anything
                return;
            }

            // If an Effect is not being used (can only be done with the SpriteBatch)
            if (meParticleType == ParticleTypes.Sprite &&
                (Effect == null || Technique == null ||
                 SpriteBatchSettings.SortMode != SpriteSortMode.Immediate))
            {
                // Start the SpriteBatch for drawing
                mcSpriteBatch.Begin(SpriteBatchSettings.BlendMode, SpriteBatchSettings.SortMode, SpriteBatchSettings.SaveStateMode, SpriteBatchSettings.TransformationMatrix);

                // Loop through all of the Sprites to Draw
                LinkedListNode<Particle> cNode = mcParticleSpritesToDraw.First;
                while (cNode != null)
                {
                    // Draw this Sprite using the overloaded Draw Sprite function
                    DrawSprite(cNode.Value, mcSpriteBatch);

                    // Move to the next Sprite to Draw
                    cNode = cNode.Next;
                }

                // End the SpriteBatch since we are done drawing
                mcSpriteBatch.End();
            }
            // Else an Effect is being used
            else
            {
                // If we are not using the SpriteBatch
                if (meParticleType != ParticleTypes.Sprite)
                {
                    // Set the Render State for drawing
                    SetRenderState(GraphicsDevice.RenderState);

                    // Specify the Vertex Buffer to use (not used with the SpriteBatch)
                    GraphicsDevice.VertexDeclaration = mcVertexDeclaration;
                }

                // Set the Effect Parameters
                SetEffectParameters();

                // Begin the Effect
                mcEffect.Begin();

                // Loop through each Pass of the Current Technique
                foreach (EffectPass cPass in mcEffect.CurrentTechnique.Passes)
                {
                    // If we are drawing the Particles using a SpriteBatch
                    if (meParticleType == ParticleTypes.Sprite)
                    {
                        // Start the SpriteBatch for drawing before we start the Pass
                        mcSpriteBatch.Begin(SpriteBatchSettings.BlendMode, SpriteBatchSettings.SortMode, SpriteBatchSettings.SaveStateMode, SpriteBatchSettings.TransformationMatrix);

                        // When SpriteBatch.Begin() is called it sets the RenderState, so we must reset 
                        // the RenderState after starting the SpriteBatch if we want to change it at all
                        SetRenderState(GraphicsDevice.RenderState);
                    }

                    // Begin the Pass
                    cPass.Begin();

                    // Draw the Particles based on what Type of Particles they are
                    switch (meParticleType)
                    {
                        default:
                        case ParticleTypes.Pixel:
                        case ParticleTypes.PointSprite:
                            // Draw the Particles as Point Sprites / Pixels
                            GraphicsDevice.DrawUserPrimitives<Vertex>(PrimitiveType.PointList, mcParticleVerticesToDraw, 0, miNumberOfParticlesToDraw);
                        break;

                        case ParticleTypes.Quad:
                        case ParticleTypes.TexturedQuad:
                            // Draw the Particles as Quads
                            GraphicsDevice.DrawUserIndexedPrimitives<Vertex>(PrimitiveType.TriangleList, mcParticleVerticesToDraw, 0, miNumberOfParticlesToDraw * 4, miIndexBufferArray, 0, miNumberOfParticlesToDraw * 2);
                        break;

                        case ParticleTypes.Sprite:
                            // Loop through all of the Sprites to Draw
                            LinkedListNode<Particle> cNode = mcParticleSpritesToDraw.First;
                            while (cNode != null)
                            {
                                // Draw this Sprite using the overloaded Draw Sprite function
                                DrawSprite(cNode.Value, mcSpriteBatch);

                                // Move to the next Sprite to Draw
                                cNode = cNode.Next;
                            }

                            // End the SpriteBatch since we are done drawing for this pass
                            mcSpriteBatch.End();
                        break;
                    }

                    // End the Pass
                    cPass.End();
                }

                // End the Effect
                mcEffect.End();

                // Reset any necessary Render State settings
                ResetRenderState(GraphicsDevice.RenderState);
            }

            // Perform any other functions now that this Particle System has been Drawn
            AfterDraw();
        }

        #endregion

        #region Virtual Methods that may be overridden

        /// <summary>
        /// Virtual function to Initialize the Particle System with default values.
        /// </summary>
        /// <param name="cGraphicsDevice">The Graphics Device the Particle System should use</param>
        /// <param name="cContentManager">The Content Manager the Particle System should use to load resources</param>
        public virtual void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager)
        { }

        /// <summary>
        /// Virtual function to draw a Sprite Particle. This function should be used to draw the given
        /// Particle with the provided SpriteBatch.
        /// </summary>
        /// <param name="Particle">The Particle Sprite to Draw</param>
        /// <param name="cSpriteBatch">The SpriteBatch to use to doing the Drawing</param>
        protected virtual void DrawSprite(DPSFParticle Particle, SpriteBatch cSpriteBatch)
        { }

        /// <summary>
        /// Virtual function that is called at the end of the Initialize() function.
        /// This may be used to perform operates after the Particle System has been Initialized, such as 
        /// initializing other Particle Systems nested within this Particle System.
        /// </summary>
        protected virtual void AfterInitialize()
        { }

        /// <summary>
        /// Virtual function that is called at the beginning of the Destroy() function.
        /// This may be used to perform operations before the Destroy() code is executed.
        /// </summary>
        protected virtual void BeforeDestroy()
        { }

        /// <summary>
        /// Virtual function that is called at the end of the Destroy() function.
        /// This may be used to perform operations after the Particle System has been Destroyed, such as 
        /// to destroy other Particle Systems nested within this Particle System.
        /// </summary>
        protected virtual void AfterDestroy()
        { }

        /// <summary>
        /// Virtual function that is called at the beginning of the Update() function.
        /// This may be used to perform operations before the Update() code is executed.
        /// </summary>
        protected virtual void BeforeUpdate(float fElapsedTimeInSeconds)
        { }

        /// <summary>
        /// Virtual function that is called at the end of the Update() function.
        /// This may be used to perform operations after the Particle System has been updated, such as 
        /// to Update Particle Systems nested within this Particle System.
        /// </summary>
        protected virtual void AfterUpdate(float fElapsedTimeInSeconds)
        { }

        /// <summary>
        /// Virtual function that is called at the beginning of the Draw() function.
        /// This may be used to perform operations before the Draw() code is executed.
        /// </summary>
        protected virtual void BeforeDraw()
        { }

        /// <summary>
        /// Virtual function that is called at the end of the Draw() function.
        /// This may be used to perform operations after the Particle System has been drawn, such as 
        /// to Draw Particle Systems nested within this Particle System.
        /// </summary>
        protected virtual void AfterDraw()
        { }

        /// <summary>
        /// Virtual function that is called at the beginning of the AddParticle() function.
        /// This may be used to execute some code before a new Particle is initialized and added.
        /// </summary>
        protected virtual void BeforeAddParticle()
        { }

        /// <summary>
        /// Virtual function that is called at the end of the AddParticle() function.
        /// This may be used to execute some code after a new Particle is initialized and added.
        /// </summary>
        protected virtual void AfterAddParticle()
        { }

        /// <summary>
        /// Virtual function to Set the Graphics Device properties for rendering the Particles
        /// </summary>
        /// <param name="cRenderState">The Render State of the Graphics Device to change</param>
        protected virtual void SetRenderState(RenderState cRenderState)
        { }

        /// <summary>
        /// Virtual function to Reset the Graphics Device properties after rendering the Particles
        /// </summary>
        /// <param name="cRenderState">The Render State of the Graphics Device to change</param>
        protected virtual void ResetRenderState(RenderState cRenderState)
        { }

        /// <summary>
        /// Virtual function to Set the Effect's Parameters before drawing the Particles
        /// </summary>
        protected virtual void SetEffectParameters()
        { }

        #endregion
    }

    /// <summary>
    /// Class used to automatically create new Particles in a Particle System
    /// </summary>
    public class ParticleEmitter
    {
        // Variables to hold the Position, Orientation, and Pivot Point information
        private Position3D mcPositionData;
        private Orientation3D mcOrientationData;
        private PivotPoint3D mcPivotPointData;

        // Variable to tell if the Emitter should be able to Emit Particles or not
        private bool mbEnabled = true;

        // Variable to tell if the Emitter should Emit Particles Automatically or not
        private bool mbEmitParticlesAutomatically = true;

        // Variables used to release a Burst of Particles
        private int miBurstNumberOfParticles = 0;
        private float mfBurstTimeInSeconds = 0.0f;

        // Private variables used to determine how many Particles should be emitted
        private float mfParticlesPerSecond = 0.0f;
        private float mfSecondsPerParticle = 0.0f;
        private float mfTimeElapsedSinceGeneratingLastParticle = 0.0f;

        /// <summary>
        /// Raised when a Burst property reaches (or is set to) zero 
        /// </summary>
        public event EventHandler BurstComplete = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public ParticleEmitter()
        {
            // Initialize the Position, Orientation, and Pivot Point variables
            mcPositionData = new Position3D();
            mcOrientationData = new Orientation3D();
            mcPivotPointData = new PivotPoint3D(mcPositionData, mcOrientationData);
        }

        /// <summary>
        /// Get / Set if the Emitter is able to Emit Particles or not.
        /// <para>NOTE: If this is false, not even Bursts will Emit Particles.</para>
        /// <para>NOTE: The Position, Orientation, and Pivot Data will still be updated when this is false.</para>
        /// </summary>
        public bool Enabled
        {
            get { return mbEnabled; }
            set { mbEnabled = value; }
        }

        /// <summary>
        /// Get the Position Data (Position, Velocity, and Acceleration)
        /// </summary>
        public Position3D PositionData
        {
            get { return mcPositionData; }
        }

        /// <summary>
        /// Get the Orientation Data (Orientation, Rotational Velocity, and Rotational Acceleration)
        /// </summary>
        public Orientation3D OrientationData
        {
            get { return mcOrientationData; }
        }

        /// <summary>
        /// Get the Pivot Point Data (Pivot Point, Pivot Rotational Velocity, and Pivot Rotational Acceleration)
        /// </summary>
        public PivotPoint3D PivotPointData
        {
            get { return mcPivotPointData; }
        }

        /// <summary>
        /// Get / Set if the Emitter should Emit Particles Automatically or not.
        /// <para>NOTE: Particles will only be emitted if the Emitter is Enabled.</para>
        /// </summary>
        public bool EmitParticlesAutomatically
        {
            get { return mbEmitParticlesAutomatically; }
            set { mbEmitParticlesAutomatically = value; }
        }

        /// <summary>
        /// Get / Set how many Particles should be emitted per Second
        /// </summary>
        public float ParticlesPerSecond
        {
            get { return mfParticlesPerSecond; }
            set
            {
                // Set how many Particles to emit per second
                mfParticlesPerSecond = value;

                // Set how many seconds one particle takes to be emitted
                if (mfParticlesPerSecond == 0.0f)
                {
                    mfSecondsPerParticle = 0.0f;
                }
                else
                {
                    mfSecondsPerParticle = 1.0f / mfParticlesPerSecond;
                }
            }
        }

        /// <summary>
        /// Get / Set how many Particles the Emitter should Burst. The Emitter will emit
        /// Particles, at the speed corresponding to its Particles Per Second rate, until this amount 
        /// of Particles have been emitted.
        /// <para>NOTE: Bursts are only processed when the Emit Particles Automatically property is false.</para>
        /// <para>NOTE: Bursts will only emit Particles if the Emitter is Enabled.</para>
        /// <para>NOTE: This will be set to zero if a negative value is specified.</para>
        /// <para>NOTE: This will fire the BurstComplete event when set to zero.</para>
        /// </summary>
        public int BurstParticles
        {
            get { return miBurstNumberOfParticles; }
            set
            {
                // Make sure the specified Number Of Burst Particles is not negative
                if (value <= 0)
                {
                    miBurstNumberOfParticles = 0;

                    // If there is a function to catch the event
                    if (BurstComplete != null)
                    {
                        // Throw the event that the Burst is Complete
                        BurstComplete(this, null);
                    }
                }
                else
                {
                    miBurstNumberOfParticles = value;
                }
            }
        }

        /// <summary>
        /// Get / Set how long the Emitter should Burst for (in seconds). The Emitter will emit
        /// Particles, at the speed corresponding to its Particles Per Second rate, until this amount 
        /// of time in seconds has elapsed.
        /// <para>NOTE: Bursts are only processed when the Emit Particles Automatically property is false.</para>
        /// <para>NOTE: Bursts will only emit Particles if the Emitter is Enabled.</para>
        /// <para>NOTE: This will be set to zero if a negative value is specified.</para>
        /// <para>NOTE: This will fire the BurstComplete event when set to zero.</para>
        /// </summary>
        public float BurstTime
        {
            get { return mfBurstTimeInSeconds; }
            set
            {
                // Make sure the specified Time is not negative
                if (value <= 0.0f)
                {
                    mfBurstTimeInSeconds = 0.0f;

                    // If there is a function to catch the event
                    if (BurstComplete != null)
                    {
                        // Throw the event that the Burst is Complete
                        BurstComplete(this, null);
                    }
                }
                else
                {
                    mfBurstTimeInSeconds = value;
                }
            }
        }

        /// <summary>
        /// Updates the Emitter's Position and Orientation according to its 
        /// Velocities and Accelerations, and returns how many Particles should 
        /// be emitted this frame.
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long (in seconds) it has been 
        /// since this function was called</param>
        /// <returns>Returns the number of Particles that should be emitted</returns>
        public int UpdateAndGetNumberOfParticlesToEmit(float fElapsedTimeInSeconds)
        {
            // Update the Emitter's Positional, Rotational, and Pivotal Data
            PositionData.Update(fElapsedTimeInSeconds);
            OrientationData.Update(fElapsedTimeInSeconds);
            PivotPointData.Update(fElapsedTimeInSeconds);

            // Calculate how many Particles should be generated

            // If Particles should be emitted
            int iNumberOfParticlesToEmit = 0;
            if (mfParticlesPerSecond > 0.0f)
            {
                // If the Emitter is emitting Particles Automatically
                if (mbEmitParticlesAutomatically)
                {
                    // Get how many Particles should be emitted
                    iNumberOfParticlesToEmit = CalculateHowManyParticlesToEmit(fElapsedTimeInSeconds);
                }
                // Else if Burst Particles should be emitted for a specific amount of Time
                else if (mfBurstTimeInSeconds > 0.0f)
                {
                    // Make sure we do not emit Particles for too long
                    float fModifiedElapsedTime = fElapsedTimeInSeconds;
                    if (fModifiedElapsedTime > mfBurstTimeInSeconds)
                    {
                        // Only emit Particles for as long as the Burst lasts
                        fModifiedElapsedTime = mfBurstTimeInSeconds;
                    }

                    // Get how many Particles should be emitted
                    iNumberOfParticlesToEmit = CalculateHowManyParticlesToEmit(fModifiedElapsedTime);

                    // Subtract the Elapsed Time from the Burst Time
                    BurstTime -= fModifiedElapsedTime;
                }  
                // Else if a specific Number Of Burst Particles should be emitted
                else if (miBurstNumberOfParticles > 0)
                {
                    // Get how many Particles should be emitted
                    iNumberOfParticlesToEmit = CalculateHowManyParticlesToEmit(fElapsedTimeInSeconds);

                    // If we are emitting too many Particles
                    if (iNumberOfParticlesToEmit > miBurstNumberOfParticles)
                    {
                        iNumberOfParticlesToEmit = miBurstNumberOfParticles;
                    }

                    // Subtract the Number Of Particles being Emitted from the Number Of Particles that still need to be Emitted
                    BurstParticles -= iNumberOfParticlesToEmit;
                } 
            }

            // Return how many Particles should be emitted
            return iNumberOfParticlesToEmit;
        }

        /// <summary>
        /// Calculates how many Particles should be emitted based on the amount of Time Elapsed
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How much Time has Elapsed (in seconds) since the last Update</param>
        /// <returns>Returns how many Particles should be emitted</returns>
        private int CalculateHowManyParticlesToEmit(float fElapsedTimeInSeconds)
        {
            // Variable to hold how many Particles To Emit
            int iNumberOfParticlesToEmit = 0;

            // Add the Elapsed Time since the last update to the Elapsed Time since the last Particle was generated
            mfTimeElapsedSinceGeneratingLastParticle += fElapsedTimeInSeconds;

            // Calculate how many Particles should be generated based on the Elapsed Time
            float fNumberOfParticlesToEmit = mfTimeElapsedSinceGeneratingLastParticle / mfSecondsPerParticle;
            iNumberOfParticlesToEmit = (int)Math.Floor(fNumberOfParticlesToEmit);

            // Calculate how much time should be carried over to the next Update
            mfTimeElapsedSinceGeneratingLastParticle = (fNumberOfParticlesToEmit - iNumberOfParticlesToEmit) * mfSecondsPerParticle;

            // Return how many Particles should be emitted
            return iNumberOfParticlesToEmit;
        }
    }

    /// <summary>
    /// Class to manage the Updating and Drawing of DPSF Particle Systems each frame
    /// </summary>
    public class ParticleSystemManager
    {
        // List of the Particle Systems being Managed by this Manager
        private List<IDPSFParticleSystem> mcParticleSystemListSortedByUpdateOrder = new List<IDPSFParticleSystem>();
        private List<IDPSFParticleSystem> mcParticleSystemListSortedByDrawOrder = new List<IDPSFParticleSystem>();
        
        // Variables used to control if Updates and Draws are Performed or not
        private bool mbPerformUpdates = true;
        private bool mbPerfomDraws = true;

        // Variables used to tell when the Particle System Lists need to be resorted
        private bool mbAParticleSystemsUpdateOrderWasChanged = false;
        private bool mbAParticleSystemsDrawOrderWasChanged = false;

        // Variable used to control how fast the simulations run
        private float mfSimulationSpeed = 1.0f;
        private bool mbUseManagersSimulationSpeed = true;

        // Variable used to control how often the particle systems are udpated
        private int miUpdatesPerSecond = 0;
        private bool mbUseManagersUpdatesPerSecond = true;

        /// <summary>
        /// Copies the given DPSF Particle System Manager's information into this Manager
        /// </summary>
        /// <param name="cManagerToCopy">The Manager To Copy from</param>
        public void CopyFrom(ParticleSystemManager cManagerToCopy)
        {
            // Copy the Value types
            mbPerformUpdates = cManagerToCopy.mbPerformUpdates;
            mbPerfomDraws = cManagerToCopy.mbPerfomDraws;
            mbAParticleSystemsUpdateOrderWasChanged = cManagerToCopy.mbAParticleSystemsUpdateOrderWasChanged;
            mbAParticleSystemsDrawOrderWasChanged = cManagerToCopy.mbAParticleSystemsDrawOrderWasChanged;
            mfSimulationSpeed = cManagerToCopy.mfSimulationSpeed;

            // Deep copy the Reference types

            // Loop through all of the Manager To Copy's Particle Systems
            int iNumberOfParticleSystems = cManagerToCopy.ParticleSystems.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // Add the Particle System to this List of Particle Systems
                mcParticleSystemListSortedByUpdateOrder.Add(cManagerToCopy.ParticleSystems[iIndex]);
            }
        }

        /// <summary>
        /// Get if the Particle Systems are inheriting from DrawableGameComponent or not
        /// </summary>
        public bool ParticleSystemsInheritDrawableGameComponent
        {
        // If inheriting DrawableGameComponent
        #if (DPSFAsDrawableGameComponent)
            get { return true; }
            // Else not inheriting DrawableGameComponent
        #else
            get { return false; }
        #endif
        }

        /// <summary>
        /// Get / Set if the Particle Systems should be Updated or not
        /// </summary>
        public bool Enabled
        {
            get { return mbPerformUpdates; }
            set { mbPerformUpdates = true; }
        }

        /// <summary>
        /// Get / Set if the Particle Systems should be Drawn or not
        /// </summary>
        public bool Visible
        {
            get { return mbPerfomDraws; }
            set { mbPerfomDraws = value; }
        }

        /// <summary>
        /// Get / Set if the Particle System Manager's SimulationSpeed property
        /// should be used for each of the particle systems it contains or not.
        /// <para>Default value is true.</para>
        /// </summary>
        public bool SimulationSpeedIsEnabled
        {
            get { return mbUseManagersSimulationSpeed; }
            set { mbUseManagersSimulationSpeed = value; }
        }

        /// <summary>
        /// Get / Set if the Particle System Manager's UpdatesPerSecond property
        /// should be used for each of the particle systems it contains or not.
        /// <para>Default value is true.</para>
        /// </summary>
        public bool UpdatesPerSecondIsEnabled
        {
            get { return mbUseManagersUpdatesPerSecond; }
            set { mbUseManagersUpdatesPerSecond = value; }
        }

        /// <summary>
        /// Get / Set how fast the Particle System Simulations should run. 
        /// <para>1.0 = normal speed, 0.5 = half speed, 2.0 = double speed.</para>
        /// <para>NOTE: This sets the SimulationSpeed property of each individual Particle
        /// System in this Manager to the given value. It will also set a particle system's
        /// Simulation Speed when the particle system is re-initialized, 
        /// and when a new Particle System is added to the Manager in the future.</para>
        /// <para>NOTE: Setting this property only has an effect if the SimulationSpeedIsEnabled property is true.</para>
        /// <para>NOTE: This will be set to zero if a negative value is specified.</para>
        /// </summary>
        public float SimulationSpeed
        {
            get { return mfSimulationSpeed; }
            set
            {
                // If an invalid Simulation Speed was specified
                if (value < 0.0f)
                {
                    mfSimulationSpeed = 0.0f;
                }
                // Else the given value is valid
                else
                {
                    mfSimulationSpeed = value;
                }

                // If the Manager's Simulation Speed is Enabled
                if (mbUseManagersSimulationSpeed)
                {
                    // Set the Simulation Speed of All Particle Systems to the given value
                    SetSimulationSpeedForAllParticleSystems(mfSimulationSpeed);
                }
            }
        }

        /// <summary>
        /// Get / Set how often the Particle Systems should be Updated. 
        /// <para>NOTE: This sets the UpdatesPerSecond property of each individual Particle
        /// System in this Manager to the given value. It will also set a particle system's
        /// Updates Per Second when the particle system is re-initialized, 
        /// and when a new Particle System is added to the Manager in the future.</para>
        /// <para>NOTE: Setting this property only has an effect if the UpdatesPerSecondIsEnabled property is true.</para>
        /// <para>NOTE: A value of zero means update the particle systems everytime Update() is called.</para>
        /// <para>NOTE: This will be set to zero if a negative value is specified.</para>
        /// </summary>
        public int UpdatesPerSecond
        {
            get { return miUpdatesPerSecond; }
            set
            {
                // If an invalid Updates Per Second was specified
                if (value < 0)
                {
                    miUpdatesPerSecond = 0;
                }
                // Else the given value is valid, so use it
                else
                {
                    miUpdatesPerSecond = value;
                }

                // If the Manager's Updates Per Second is Enabled
                if (mbUseManagersUpdatesPerSecond)
                {
                    // Set the Updates Per Second of All Particle Systems to the given value
                    SetUpdatesPerSecondForAllParticleSystems(miUpdatesPerSecond);
                }
            }
        }
        
        /// <summary>
        /// Get the cumulative Number Of Active Particles of all Particle Systems in this Manager
        /// </summary>
        public int TotalNumberOfActiveParticles
        {
            get
            {
                // Variable to keep track of the Total Number of Active Particles
                int iActiveParticleCount = 0;

                // Loop through all of the Particle Systems
                int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
                for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
                {
                    // Add this Particle System's Active Particles to the count
                    iActiveParticleCount += mcParticleSystemListSortedByUpdateOrder[iIndex].NumberOfActiveParticles;
                }

                // Return the Total Number of Active Particles
                return iActiveParticleCount;
            }
        }

        /// <summary>
        /// Get the cumulative Number Of Particles Being Drawn by all Particle Systems in this Manager.
        /// This is the total number of Active AND Visible Particles.
        /// <para>NOTE: This ignores whether the Manager is Visible or not.</para>
        /// </summary>
        public int TotalNumberOfParticlesBeingDrawn
        {
            get
            {
                // Variable to keep track of the Total Number of Active Particles
                int iActiveAndVisibleParticleCount = 0;

                // Loop through all of the Particle Systems
                int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
                for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
                {
                    // Add this Particle System's Active and Visible Particles to the count
                    iActiveAndVisibleParticleCount += mcParticleSystemListSortedByUpdateOrder[iIndex].NumberOfParticlesBeingDrawn;
                }

                // Return the Total Number of Particles being Drawn
                return iActiveAndVisibleParticleCount;
            }
        }

        /// <summary>
        /// Get the cumulative Max Number Of Particles allocated in memory by all Particle Systems in the Manager.
        /// </summary>
        public int TotalNumberOfParticlesAllocatedInMemory
        {
            get
            {
                // Variable to keep track of the Total Number of Particles Allocated In Memory
                int iNumberOfParticlesAllocatedInMemory = 0;

                // Loop through all of the Particle Systems
                int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
                for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
                {
                    // Add this Particle System's Active and Visible Particles to the count
                    iNumberOfParticlesAllocatedInMemory += mcParticleSystemListSortedByUpdateOrder[iIndex].NumberOfParticlesAllocatedInMemory;
                }

                // Return the Total Number of Particles Allocated In Memory
                return iNumberOfParticlesAllocatedInMemory;
            }
        }

        /// <summary>
        /// Sets each individual Particle Systems' Simulation Speed to the specified Simulation Speed.
        /// </summary>
        /// <param name="fSimulationSpeed">The new Simulation Speed that all Particle Systems 
        /// currently in this Manager should have</param>
        public void SetSimulationSpeedForAllParticleSystems(float fSimulationSpeed)
        {
            // Loop through all of the Particle Systems
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this Particle System is Initialized
                if (mcParticleSystemListSortedByUpdateOrder[iIndex].IsInitialized())
                {
                    // Set this Particle System's Simulation Speed
                    mcParticleSystemListSortedByUpdateOrder[iIndex].SimulationSpeed = fSimulationSpeed;
                }
            }
        }

        /// <summary>
        /// Sets each individual Particle Systems' Updates Per Second to the specified Updates Per Second.
        /// </summary>
        /// <param name="iUpdatesPerSecond">The new Updates Per Second that all particle systems
        /// currently in this Manager should have</param>
        public void SetUpdatesPerSecondForAllParticleSystems(int iUpdatesPerSecond)
        {
            // Loop through all of the Particle Systems
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this Particle System is Initialized
                if (mcParticleSystemListSortedByUpdateOrder[iIndex].IsInitialized())
                {
                    // Set this Particle System's Simulation Speed
                    mcParticleSystemListSortedByUpdateOrder[iIndex].UpdatesPerSecond = iUpdatesPerSecond;
                }
            }
        }

        /// <summary>
        /// Sets the World, View, and Projection Matrices for all of the Particle Systems in this Manager.
        /// <para>NOTE: Sprite particle systems are not affected by the World, View, and Projection matrices.</para>
        /// </summary>
        /// <param name="cWorld">The World Matrix</param>
        /// <param name="cView">The View Matrix</param>
        /// <param name="cProjection">The Projection Matrix</param>
        public void SetWorldViewProjectionMatricesForAllParticleSystems(Matrix cWorld, Matrix cView, Matrix cProjection)
        {
            // Loop through all of the Particle Systems
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this Particle System is Initialized
                if (mcParticleSystemListSortedByUpdateOrder[iIndex].IsInitialized())
                {
                    // Set the World, View, and Projection Matrices for this Particle System
                    mcParticleSystemListSortedByUpdateOrder[iIndex].SetWorldViewProjectionMatrices(cWorld, cView, cProjection);
                }
            }
        }

        /// <summary>
        /// Sets the SpriteBatchSettings.TransformationMatrix for all Sprite Particle Systems in this Manager.
        /// </summary>
        /// <param name="sTransformationMatrix">The Transformation Matrix to apply to the Sprite Particle Systems</param>
        public void SetTransformationMatrixForAllSpriteParticleSystems(Matrix sTransformationMatrix)
        {
            // Loop through all of the Particle Systems
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this is a Sprite Particle System and it is Initialized
                if (mcParticleSystemListSortedByUpdateOrder[iIndex].ParticleType == ParticleTypes.Sprite &&
                    mcParticleSystemListSortedByUpdateOrder[iIndex].IsInitialized())
                {
                    // Set the Transformation Matrix for this Particle System
                    mcParticleSystemListSortedByUpdateOrder[iIndex].SpriteBatchSettings.TransformationMatrix = sTransformationMatrix;
                }
            }
        }

        /// <summary>
        /// Returns true if the given Particle System is in the Manager, false if not.
        /// </summary>
        /// <param name="cParticleSystemToFind">The Particle System to look for</param>
        /// <returns>Returns true if the given Particle System is in the Manager, false if not.</returns>
        public bool ContainsParticleSystem(IDPSFParticleSystem cParticleSystemToFind)
        {
            // Return if the Particle System is in the List or not
            return mcParticleSystemListSortedByUpdateOrder.Contains(cParticleSystemToFind);
        }

        /// <summary>
        /// Returns true if the Particle System with the given ID is in the Manager, false if not.
        /// </summary>
        /// <param name="iIDOfParticleSystemToFind">The ID of the Particle System to find</param>
        /// <returns>Returns true if the Particle System with the given ID is in the Manager, false if not.</returns>
        public bool ContainsParticleSystem(int iIDOfParticleSystemToFind)
        {
            // Loop through all of the Particle Systems
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this is the Particle System we are looking for
                if (mcParticleSystemListSortedByUpdateOrder[iIndex].ID == iIDOfParticleSystemToFind)
                {
                    // Return that the Particle System is in this Manager
                    return true;
                }
            }

            // Return that the Particle System was not found
            return false;
        }

        /// <summary>
        /// Add an initialized Particle System to the Particle System Manager.
        /// <para>NOTE: This sets the Particle System's ParticleSystemManagerToCopyPropertiesFrom
        /// property to this Particle System Manager.</para>
        /// </summary>
        /// <param name="cParticleSystemToAdd">The initialized Particle System to add</param>
        public void AddParticleSystem(IDPSFParticleSystem cParticleSystemToAdd)
        {
            // Add the Particle System to both Lists
            mcParticleSystemListSortedByUpdateOrder.Add(cParticleSystemToAdd);
            mcParticleSystemListSortedByDrawOrder.Add(cParticleSystemToAdd);

            // Add to the Particle System's UpdateOrderChanged and DrawOrderChanged events
            cParticleSystemToAdd.UpdateOrderChanged += new EventHandler(RecordThatAnUpdateOrderHasBeenChanged);
            cParticleSystemToAdd.DrawOrderChanged += new EventHandler(RecordThatADrawOrderHasBeenChanged);

            // Let the Particle System know it should use This Manager's properties when re-initializing.
            // This also copies the Manager's properties into the Particle System now as well.
            cParticleSystemToAdd.ParticleSystemManagerToCopyPropertiesFrom = this;

            // Make sure the Particle System Lists are still sorted
            SortParticleSystemLists();
        }

        /// <summary>
        /// Removes the specified Particle System from the Particle System Manager.
        /// Returns true if the Particle System was found and removed, false if it was not found.
        /// </summary>
        /// <param name="cParticleSystemToRemove">A handle to the Particle System to Remove</param>
        /// <returns>Returns true if the Particle System was found and removed, false if it was not found.</returns>
        public bool RemoveParticleSystem(IDPSFParticleSystem cParticleSystemToRemove)
        {
            // Remove the Event Handlers we attached to the Particle System
            cParticleSystemToRemove.UpdateOrderChanged -= RecordThatAnUpdateOrderHasBeenChanged;
            cParticleSystemToRemove.DrawOrderChanged -= RecordThatADrawOrderHasBeenChanged;

            // Remove the Particle System from the Lists
            mcParticleSystemListSortedByUpdateOrder.Remove(cParticleSystemToRemove);
            return mcParticleSystemListSortedByDrawOrder.Remove(cParticleSystemToRemove);
        }

        /// <summary>
        /// Removes the specified Particle System from the Particle System Manager.
        /// Returns true if the Particle System was found and removed, false if it was not found.
        /// </summary>
        /// <param name="iIDOfParticleSystemToRemove">The ID of the Particle System to Remove</param>
        /// <returns>Returns true if the Particle System was found and removed, false if it was not found.</returns>
        public bool RemoveParticleSystem(int iIDOfParticleSystemToRemove)
        {
            // Loop through all of the Particle Systems
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this is the Particle System to remove
                if (mcParticleSystemListSortedByUpdateOrder[iIndex].ID == iIDOfParticleSystemToRemove)
                {
                    // Remove this Particle System from the Particle System Manager
                    // NOTE: We call the other Remove function since it removes the Event Handlers as well
                    return RemoveParticleSystem(mcParticleSystemListSortedByUpdateOrder[iIndex]);
                }
            }

            // Return that the Particle System was not found
            return false;
        }

        /// <summary>
        /// Removes all Particle Systems from the Particle System Manager
        /// </summary>
        public void RemoveAllParticleSystems()
        {
            // Normally we would just Clear() the Particle System Lists, but because we attached
            // Event Handlers to the Particle Systems we want to call the RemoveParticleSystem()
            // function to remove these Event Handlers, since a handle to the Particle System
            // may still exist somewhere else and we don't want it to still reference this
            // Particle System Manager.

            // Loop through all of the Particle Systems
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // Remove this Particle System
                RemoveParticleSystem(mcParticleSystemListSortedByUpdateOrder[iIndex]);
            }

            // Make sure the Lists are empty
            mcParticleSystemListSortedByUpdateOrder.Clear();
            mcParticleSystemListSortedByDrawOrder.Clear();
        }

        /// <summary>
        /// Calls the AutoInitialize() function for every Particle System in this Manager
        /// </summary>
        /// <param name="cGraphicsDevice">The Graphics Device that the Particle Systems should be drawn to</param>
        /// <param name="cContentManager">The Content Manager used to load Effect files and Textures</param>
        public void AutoInitializeAllParticleSystems(GraphicsDevice cGraphicsDevice, ContentManager cContentManager)
        {
            // Loop through all of the Particle Systems
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // AutoInitialize this Particle System
                mcParticleSystemListSortedByUpdateOrder[iIndex].AutoInitialize(cGraphicsDevice, cContentManager);
            }
        }

        /// <summary>
        /// Calls the Destroy() function for every Particle System in this Manager
        /// </summary>
        public void DestroyAllParticleSystems()
        {
            // Loop through all of the Particle Systems
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // Destroy this Particle System
                mcParticleSystemListSortedByUpdateOrder[iIndex].Destroy();
            }
        }

        /// <summary>
        /// Destroys each Particle System in the Manager, then removes them from the Manager
        /// </summary>
        public void DestroyAndRemoveAllParticleSystem()
        {
            // Destroy and Remove all Particle Systems
            DestroyAllParticleSystems();
            RemoveAllParticleSystems();
        }

        /// <summary>
        /// Returns a Linked List of handles to the Particle Systems in this Manager
        /// </summary>
        public List<IDPSFParticleSystem> ParticleSystems
        {
            get { return mcParticleSystemListSortedByUpdateOrder; }
        }

        /// <summary>
        /// Updates all of the Particle Systems.
        /// <para>NOTE: This will only Update the Particle Systems if they do not inherit from DrawableGameComponent, 
        /// since if they do they will be updated automatically by the Game object.</para>
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">The amount of Time in seconds that has passed since
        /// the last Update</param>
        public void UpdateAllParticleSystems(float fElapsedTimeInSeconds)
        {
            // If Updates should not be performed (DrawableGameComponents will Update the Particle Systems automatically)
            if (ParticleSystemsInheritDrawableGameComponent)
            {
                // Exit without doing any Updates
                return;
            }

            // Update All of the Particle Systems
            UpdateAllParticleSystemsForced(fElapsedTimeInSeconds);
        }

        /// <summary>
        /// Updates all of the Particle Systems.
        /// <para>NOTE: If the Particle Systems inherit from DrawableGameComponent and this is called, the Particle
        /// Systems will be updated twice each frame; once here and once when called automatically by the game object.
        /// If not inheriting from DrawableGameComponent, this function acts the same as calling UpdateAllParticleSystems().</para>
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">The amount of Time in seconds that has passed since
        /// the last Update</param>
        public void UpdateAllParticleSystemsForced(float fElapsedTimeInSeconds)
        {
            // If Updates should not be performed
            if (!Enabled)
            {
                // Exit without doing any Updates
                return;
            }

            // If the Particle System List needs to be resorted before Updating
            if (mbAParticleSystemsUpdateOrderWasChanged)
            {
                // Resort the Particle System List
                SortParticleSystemsByUpdateOrderList();

                // Record that the Particle System List is now sorted
                mbAParticleSystemsUpdateOrderWasChanged = false;
            }

            // Loop through all of the Particle Systems
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If the Particle System is Initialized
                if (mcParticleSystemListSortedByUpdateOrder[iIndex].IsInitialized())
                {
                    // Update the Particle System
                    mcParticleSystemListSortedByUpdateOrder[iIndex].Update(fElapsedTimeInSeconds);
                }
            }
        }

        /// <summary>
        /// Draws all of the Particle Systems.
        /// <para>NOTE: This will only Draw the Particle Systems if they do not inherit from DrawableGameComponent, 
        /// since if they do they will be drawn automatically by the Game object.</para>
        /// </summary>
        public void DrawAllParticleSystems()
        {
            // If the Particle Systems should not be Drawn (DrawableGameComponents will Draw the Particle Systems automatically)
            if (ParticleSystemsInheritDrawableGameComponent)
            {
                // Exit without Drawing anything
                return;
            }

            // Draw all of the Particle Systems
            DrawAllParticleSystemsForced();
        }

        /// <summary>
        /// Draws all of the Particle Systems, even if they inherit from DrawableGameComponent.
        /// <para>NOTE: If the Particle Systems inherit from DrawableGameComponent and this is called, the Particle
        /// Systems will be drawn twice each frame; once here and once when called automatically by the game object.
        /// If not inheriting from DrawableGameComponent, this function acts the same as calling DrawAllParticleSystems().</para>
        /// </summary>
        public void DrawAllParticleSystemsForced()
        {
            // If the Particle Systems should not be Drawn
            if (!Visible)
            {
                // Exit without Drawing anything
                return;
            }

            // If the Particle System List needs to be resorted before Drawing
            if (mbAParticleSystemsDrawOrderWasChanged)
            {
                // Resort the Particle System List
                SortParticleSystemsByDrawOrderList();

                // Record that the Particle System List is now sorted
                mbAParticleSystemsDrawOrderWasChanged = false;
            }

            // Loop through all of the Particle Systems
            int iNumberOfParticleSystems = mcParticleSystemListSortedByDrawOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If the Particle System is Initialized
                if (mcParticleSystemListSortedByDrawOrder[iIndex].IsInitialized())
                {
                    // Draw the Particle System
                    mcParticleSystemListSortedByDrawOrder[iIndex].DrawForced();
                }
            }
        }

        /// <summary>
        /// Draws all of the Particle Systems to a Texture and returns the Texture, which has a Transparent Black background
        /// </summary>
        /// <param name="cGraphicsDevice">A Graphics Device to use for drawing; The Graphics Device contents will not be overwritten.
        /// <para>NOTE: The size of the Texture before scaling will be the size of the Graphics Device's Viewport.</para></param>
        /// <param name="iTextureWidth">The desired Width of the Texture</param>
        /// <param name="iTextureHeight">The desired Height of the Texture</param>
        /// <returns>Returns a Texture with the Particle Systems in their current state drawn on it</returns>
        public Texture2D DrawAllParticleSystemsToTexture(GraphicsDevice cGraphicsDevice, int iTextureWidth, int iTextureHeight)
        {
            // Variable to hold the Texture To Return
            Texture2D cTextureToReturn = null;

            // Store the Width and Height of the viewport
            int iViewportWidth = cGraphicsDevice.Viewport.Width;
            int iViewportHeight = cGraphicsDevice.Viewport.Height;

            // Backup the given Graphics Device's Render Target
            RenderTarget2D cOldRenderTarget = (RenderTarget2D)cGraphicsDevice.GetRenderTarget(0);

            // Create the new Render Target for the Particle Systems to draw to
            RenderTarget2D cNewPSRenderTarget = new RenderTarget2D(cGraphicsDevice, iViewportWidth, iViewportHeight, 1, SurfaceFormat.Color);

            // Set the Render Target the given Graphics Device should draw to
            cGraphicsDevice.SetRenderTarget(0, cNewPSRenderTarget);

            // Clear the scene
            cGraphicsDevice.Clear(Color.TransparentBlack);

            // Loop through all of the Particle Systems
            int iNumberOfParticleSystems = mcParticleSystemListSortedByDrawOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If the Particle System is Initialized
                if (mcParticleSystemListSortedByDrawOrder[iIndex].IsInitialized())
                {
                    // Get a handle to this Particle System
                    IDPSFParticleSystem cParticleSystem = mcParticleSystemListSortedByDrawOrder[iIndex];

                    // Backup the Particle System's current Render Target
                    RenderTarget2D cOldPSRenderTarget = (RenderTarget2D)cParticleSystem.GraphicsDevice.GetRenderTarget(0);

                    // Set the Particle System's new Rendet Target to use
                    cParticleSystem.GraphicsDevice.SetRenderTarget(0, cNewPSRenderTarget);

                    // Draw the Particle System
                    cParticleSystem.DrawForced();

                    // Restore the Particle System's old Render Target
                    cParticleSystem.GraphicsDevice.SetRenderTarget(0, cOldPSRenderTarget);
                }
            }

            // Restore the given Graphics Device's Render Target
            cGraphicsDevice.SetRenderTarget(0, cOldRenderTarget);

            // Get the Texture with the Particle Systems drawn on it
            Texture2D cPSTexture = cNewPSRenderTarget.GetTexture();

            // If the Texture does not need to be scaled
            if (iViewportWidth == iTextureWidth && iViewportHeight == iTextureHeight)
            {
                // Set the Texture To Return to the Particle System Texture
                cTextureToReturn = cPSTexture;
            }
            // Else the Texture needs to be scaled
            else
            {
                // Backup the Graphics Device's Depth Stencil Buffer
                DepthStencilBuffer cOldDepthStencilBuffer = cGraphicsDevice.DepthStencilBuffer;
                
                // Create the Render Target to draw the scaled Particle System Texture to
                RenderTarget2D cNewRenderTarget = new RenderTarget2D(cGraphicsDevice, iTextureWidth, iTextureHeight, 1, SurfaceFormat.Color);

                // Set the given Graphics Device to draw to the new Render Target
                cGraphicsDevice.SetRenderTarget(0, cNewRenderTarget);

                // Make sure the Graphic Device's Depth Stencil Buffer is large enough
                cGraphicsDevice.DepthStencilBuffer = new DepthStencilBuffer(cGraphicsDevice, iTextureWidth, iTextureHeight, cGraphicsDevice.DepthStencilBuffer.Format);

                // Clear the scene
                cGraphicsDevice.Clear(Color.TransparentBlack);

                // Create the new SpriteBatch that will be used to scale the Texture
                SpriteBatch cSpriteBatch = new SpriteBatch(cGraphicsDevice);

                // Draw the scaled Texture
                cSpriteBatch.Begin(SpriteBlendMode.None);
                cSpriteBatch.Draw(cPSTexture, new Rectangle(0, 0, iTextureWidth, iTextureHeight), Color.White);
                cSpriteBatch.End();

                // Restore the given Graphics Device's Render Target
                cGraphicsDevice.SetRenderTarget(0, cOldRenderTarget);

                // Restore the given Graphics Device's Depth Stencil
                cGraphicsDevice.DepthStencilBuffer = cOldDepthStencilBuffer;

                // Set the Texture To Return to the scaled Texture
                cTextureToReturn = cNewRenderTarget.GetTexture();
            }

            // Return the Texture
            return cTextureToReturn;
        }

// The Xbox 360 doesn't have access to the System.Drawing namespace, so we can't perfom these functions on it
#if (!XBOX)
        /// <summary>
        /// Draws the Particle Systems' Animation over the given timespan to a sequence of Image files.
        /// <para>NOTE: This function is not available on the Xbox 360.</para>
        /// </summary>
        /// <param name="cGraphicsDevice">A Graphics Device to use for drawing; The Graphics Device contents will not be overwritten.
        /// <para>NOTE: The size of the Texture before scaling will be the size of the Graphics Device's Viewport.</para></param>
        /// <param name="iImageWidth">The desired Width of the Image files generated</param>
        /// <param name="iImageHeight">The desired Height of the Image files generated</param>
        /// <param name="sDirectoryName">The Directory to store the generated Image files in.
        /// <para>NOTE: This Directory will be created in the same directory as the application's executable.</para></param>
        /// <param name="fTotalAnimationTime">The amount of Time in seconds that the Animation should run for</param>
        /// <param name="fTimeStep">The amount of Time that should elapse between frames (i.e. 1.0 / 30.0 = 30fps)</param>
        /// <param name="eImageFormat">The type of Images to generate</param>
        /// <param name="bCreateAnimatedGIF">Set this to true to also produce an animated GIF from the Image files generated</param>
        /// <param name="bCreateTileSetImage">Set this to true to also produce a Tile Set from the Image files generated.
        /// Be careful when setting this to true as the system may run out of memory and throw an exception if the Tile Set
        /// image to generate is too large. Exactly how large it is allowed to be differs from system to system.</param>
        public void DrawAllParticleSystemsAnimationToFiles(GraphicsDevice cGraphicsDevice, int iImageWidth, int iImageHeight,
                                                            string sDirectoryName, float fTotalAnimationTime, float fTimeStep,
                                                            ImageFileFormat eImageFormat, bool bCreateAnimatedGIF, bool bCreateTileSetImage)
        {
            // Create a List to hold all of the FileNames created
            List<string> sFileNamesList = new List<string>();

            // Find a Directory name that doesn't exist already
            int iDirectoryNumber = 0;
            string sDirectory = sDirectoryName;
            while (System.IO.Directory.Exists(sDirectory))
            {
                // Since the DirectoryName already exists, increment and append a number to it
                iDirectoryNumber++;
                sDirectory = sDirectoryName + iDirectoryNumber.ToString();
            }

            // Create the new Directory
            System.IO.Directory.CreateDirectory(sDirectory);


            // Calculate how large the Tile Set texture will need to be to hold all of the frames
            int iNumberOfImages = (int)Math.Ceiling(fTotalAnimationTime / fTimeStep);

            // Calculate how many columns the texture should have to try and make it a square texture
            int iTileSetColumns = (int)Math.Ceiling(Math.Sqrt(iNumberOfImages));

            // Calculate how many rows the texture should have
            int iTileSetRows = (int)Math.Ceiling((float)iNumberOfImages / (float)iTileSetColumns);

            // If we will be creating a Tile Set Image, set up its Render Target
            RenderTarget2D cTileSetRenderTarget = null;
            if (bCreateTileSetImage)
            {
                // Calculate the required Width and Height of the Tile Set texture
                int iTileSetWidth = iTileSetColumns * iImageWidth;
                int iTileSetHeight = iTileSetRows * iImageHeight;

                // Backup the given Graphics Device's Render Target
                RenderTarget2D cOldRenderTarget = (RenderTarget2D)cGraphicsDevice.GetRenderTarget(0);

                // Backup the Graphics Device's Depth Stencil Buffer
                DepthStencilBuffer cOldDepthStencilBuffer = cGraphicsDevice.DepthStencilBuffer;

                // Create a new RenderTarget to draw the Tile Set to (specifying to preserve it's contents)
                cTileSetRenderTarget = new RenderTarget2D(cGraphicsDevice, iTileSetWidth, iTileSetHeight, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);

                // Set the given Graphics Device to draw to the new Render Target
                cGraphicsDevice.SetRenderTarget(0, cTileSetRenderTarget);

                // Make sure the Graphic Device's Depth Stencil Buffer is large enough
                cGraphicsDevice.DepthStencilBuffer = new DepthStencilBuffer(cGraphicsDevice, iTileSetWidth, iTileSetHeight, cGraphicsDevice.DepthStencilBuffer.Format);

                // Clear the scene
                cGraphicsDevice.Clear(Color.TransparentBlack);

                // Restore the given Graphics Device's Render Target
                cGraphicsDevice.SetRenderTarget(0, cOldRenderTarget);

                // Restore the given Graphics Device's Depth Stencil
                cGraphicsDevice.DepthStencilBuffer = cOldDepthStencilBuffer;
            }

            // Update the Particle System iteratively by the Time Step amount until the 
            // Particle System behaviour over the Total Time has been drawn to files
            int iFrameNumber = 0;
            float fElapsedTime = 0;
            while (fElapsedTime < fTotalAnimationTime)
            {
                // Update the Particle Systems by the Time Step amount
                UpdateAllParticleSystemsForced(fTimeStep);

                // Draw all of the Particle Systems to a Texture
                Texture2D cTexture = DrawAllParticleSystemsToTexture(cGraphicsDevice, iImageWidth, iImageHeight);

                // Add the TimeStep to the total Elapsed Time
                fElapsedTime += fTimeStep;

                // If a Tile Set Image should be created
                if (bCreateTileSetImage)
                {
                    // Add the Texture to the Tile Set at the proper position
                    int iColumn = iFrameNumber % iTileSetColumns;
                    int iRow = iFrameNumber / iTileSetColumns;
                    AddImageToTileSet(cGraphicsDevice, ref cTileSetRenderTarget, cTexture, new Rectangle(iColumn * iImageWidth, iRow * iImageHeight, iImageWidth, iImageHeight));
                }

                // Determine what this Texture's File Path should be
                string sFilePath = sDirectory + "/Frame" + iFrameNumber.ToString() + "." + eImageFormat.ToString().ToLower();

                // Save the Texture to a file
                cTexture.Save(sFilePath, eImageFormat);

                // Add the new file to the FileNames List
                sFileNamesList.Add(sFilePath);

                // Increment the Frame Number
                iFrameNumber++;
            }

            // If an Animated GIF should be created as well
            if (bCreateAnimatedGIF)
            {
                // Save the animation as a GIF as well
                CreateAnimatedGIFFromImageFiles(sFileNamesList, fTimeStep, sDirectory);
            }

            // If a Tile Set Image should be created
            if (bCreateTileSetImage)
            {
                // Save the Tile Set image
                string sTileSetFilePath = sDirectory + "/TileSet." + eImageFormat.ToString().ToLower();
                cTileSetRenderTarget.GetTexture().Save(sTileSetFilePath, eImageFormat);
            }
        }

        /// <summary>
        /// Creates an Animated GIF from the files with the given File Paths and stores it in the given Directory 
        /// relative to the application's executable.
        /// <para>NOTE: This function is not available on the Xbox 360.</para>
        /// </summary>
        /// <param name="cFilePathsList">The Paths of the image Files to create the animated GIF from</param>
        /// <param name="fTimeStep">How long between switching animation frames, in seconds</param>
        /// <param name="sDirectory">The Directory to store the Animated GIF in</param>
        private void CreateAnimatedGIFFromImageFiles(List<string> cFilePathsList, float fTimeStep, string sDirectory)
        {
            // This code for creating an animated GIF was taken from 
            // http://bloggingabout.net/blogs/rick/archive/2005/05/10/3830.aspx

            // Note: The GIF animation time is very slow and maxes out around 10fps,
            // so we use the TimeStep to try and get as close to the actual animation
            // time as possible.

            // Convert the TimeStep into 1/100 of a second, as an integer
            int iTimeStep = (int)(fTimeStep * 100.0f);

            // Calculate what the Low Byte of the TimeStep would be
            byte bTimeStepLowByte = Convert.ToByte(iTimeStep);

            // Calculate what the High Byte of the TimeStep would be
            byte bTimeStepHighByte = 0;
            iTimeStep -= 255;
            if (iTimeStep > 0)
            {
                bTimeStepHighByte = Convert.ToByte(iTimeStep);
            }

            //Variable declaration
            System.IO.MemoryStream memoryStream;
            System.IO.BinaryWriter binaryWriter;
            System.Drawing.Image image;
            Byte[] buf1;
            Byte[] buf2;
            Byte[] buf3;
            //Variable declaration

            memoryStream = new System.IO.MemoryStream();
            buf2 = new Byte[19];
            buf3 = new Byte[8];
            buf2[0] = 33;  //extension introducer
            buf2[1] = 255; //application extension
            buf2[2] = 11;  //size of block
            buf2[3] = 78;  //N
            buf2[4] = 69;  //E
            buf2[5] = 84;  //T
            buf2[6] = 83;  //S
            buf2[7] = 67;  //C
            buf2[8] = 65;  //A
            buf2[9] = 80;  //P
            buf2[10] = 69; //E
            buf2[11] = 50; //2
            buf2[12] = 46; //.
            buf2[13] = 48; //0
            buf2[14] = 3;  //Size of block
            buf2[15] = 1;  //
            buf2[16] = 0;  //
            buf2[17] = 0;  //
            buf2[18] = 0;  //Block terminator
            buf3[0] = 33;  //Extension introducer
            buf3[1] = 249; //Graphic control extension
            buf3[2] = 4;   //Size of block
            buf3[3] = 9;   //Flags: reserved, disposal method, user input, transparent color
            buf3[4] = bTimeStepLowByte;  //Delay time low byte
            buf3[5] = bTimeStepHighByte;   //Delay time high byte
            buf3[6] = 255; //Transparent color index
            buf3[7] = 0;   //Block terminator
            binaryWriter = new System.IO.BinaryWriter(System.IO.File.Open(sDirectory + "/Animation.gif", System.IO.FileMode.CreateNew));
            for (int picCount = 0; picCount < cFilePathsList.Count; picCount++)
            {
                image = System.Drawing.Bitmap.FromFile(cFilePathsList[picCount]);
                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Gif);
                buf1 = memoryStream.ToArray();

                if (picCount == 0)
                {
                    //only write these the first time....
                    binaryWriter.Write(buf1, 0, 781); //Header & global color table
                    binaryWriter.Write(buf2, 0, 19); //Application extension
                }

                binaryWriter.Write(buf3, 0, 8); //Graphic extension
                binaryWriter.Write(buf1, 789, buf1.Length - 790); //Image data

                if (picCount == cFilePathsList.Count - 1)
                {
                    //only write this one the last time....
                    binaryWriter.Write(";"); //Image terminator
                }

                memoryStream.SetLength(0);
            }
            binaryWriter.Close();
        }
#endif

        /// <summary>
        /// Draws the given Texture to the given Tile Set Render Target at the specified Position.
        /// </summary>
        /// <param name="cGraphicsDevice">The Graphics Device used to do the drawing</param>
        /// <param name="cTileSetRenderTarget">The Tile Set Render Target to draw to</param>
        /// <param name="cTexture">The Texture to draw</param>
        /// <param name="sPositionAndDimensionsInTileSetToAddImage">The Position where the Texture should be drawn
        /// on the Tile Set Render Target, and its Dimensions</param>
        private void AddImageToTileSet(GraphicsDevice cGraphicsDevice, ref RenderTarget2D cTileSetRenderTarget,
                                        Texture2D cTexture, Rectangle sPositionAndDimensionsInTileSetToAddImage)
        {
            // Backup the given Graphics Device's Render Target
            RenderTarget2D cOldRenderTarget = (RenderTarget2D)cGraphicsDevice.GetRenderTarget(0);

            // Backup the Graphics Device's Depth Stencil Buffer
            DepthStencilBuffer cOldDepthStencilBuffer = cGraphicsDevice.DepthStencilBuffer;

            // Set the given Graphics Device to draw to the Tile Set's Render Target
            cGraphicsDevice.SetRenderTarget(0, cTileSetRenderTarget);

            // Make sure the Graphic Device's Depth Stencil Buffer is large enough
            cGraphicsDevice.DepthStencilBuffer = new DepthStencilBuffer(cGraphicsDevice, cTileSetRenderTarget.Width, cTileSetRenderTarget.Height, cGraphicsDevice.DepthStencilBuffer.Format);

            // Create the new SpriteBatch that will be used to draw the Texture into the Tile Set
            SpriteBatch cSpriteBatch = new SpriteBatch(cGraphicsDevice);

            // Draw the Texture to the Tile Set Texture
            cSpriteBatch.Begin(SpriteBlendMode.None);
            cSpriteBatch.Draw(cTexture, sPositionAndDimensionsInTileSetToAddImage, Color.White);
            cSpriteBatch.End();

            // Restore the given Graphics Device's Render Target
            cGraphicsDevice.SetRenderTarget(0, cOldRenderTarget);

            // Restore the given Graphics Device's Depth Stencil
            cGraphicsDevice.DepthStencilBuffer = cOldDepthStencilBuffer;
        }

        /// <summary>
        /// Sort the two Particle System Lists
        /// </summary>
        private void SortParticleSystemLists()
        {
            SortParticleSystemsByUpdateOrderList();
            SortParticleSystemsByDrawOrderList();
        }

        /// <summary>
        /// Sorts the Particle System List Sorted By Update Order
        /// </summary>
        private void SortParticleSystemsByUpdateOrderList()
        {
            mcParticleSystemListSortedByUpdateOrder.Sort(delegate(IDPSFParticleSystem cPS1, IDPSFParticleSystem cPS2)
                                                         { return cPS1.UpdateOrder.CompareTo(cPS2.UpdateOrder); });
        }

        /// <summary>
        /// Sorts the Particle System List Sorted By Draw Order
        /// </summary>
        private void SortParticleSystemsByDrawOrderList()
        {
            mcParticleSystemListSortedByDrawOrder.Sort(delegate(IDPSFParticleSystem cPS1, IDPSFParticleSystem cPS2)
                                                       { return cPS1.DrawOrder.CompareTo(cPS2.DrawOrder); });
        }

        /// <summary>
        /// Records that the Particle Systems need to be resorted before doing the next Updates
        /// </summary>
        /// <param name="sender">The Object that sent the event</param>
        /// <param name="e">Extra information</param>
        private void RecordThatAnUpdateOrderHasBeenChanged(object sender, EventArgs e)
        {
            mbAParticleSystemsUpdateOrderWasChanged = true;
        }

        /// <summary>
        /// Records that the Particle Systems need to be resorted before doing the next Draws
        /// </summary>
        /// <param name="sender">The Object that sent the event</param>
        /// <param name="e">Extra information</param>
        private void RecordThatADrawOrderHasBeenChanged(object sender, EventArgs e)
        {
            mbAParticleSystemsDrawOrderWasChanged = true;
        }
    }
}
