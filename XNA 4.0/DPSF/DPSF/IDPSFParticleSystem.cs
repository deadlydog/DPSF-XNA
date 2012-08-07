#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF
{
	/// <summary>
	/// Interface implemented by all Particle Systems.
	/// Variables of this type can point to any type of Particle System.
	/// </summary>
	public interface IDPSFParticleSystem
	{
		/// <summary>
		/// Event Handler that is raised when the UpdateOrder of the Particle System is changed
		/// </summary>
		event EventHandler<EventArgs> UpdateOrderChanged;

		/// <summary>
		/// Event Handler that is raised when the DrawOrder of the Particle System is changed
		/// </summary>
		event EventHandler<EventArgs> DrawOrderChanged;

		/// <summary>
		/// Event Handler that is raised when the Enabled status of the Particle System is changed
		/// </summary>
		event EventHandler<EventArgs> EnabledChanged;

		/// <summary>
		/// Event Handler that is raised when the Visible status of the Particle System is changed
		/// </summary>
		event EventHandler<EventArgs> VisibleChanged;

		/// <summary>
		/// Release all resources used by the Particle System and reset all properties to their default values
		/// </summary>
		void Destroy();

		/// <summary>
		/// This function should be called immediately after deserializing a particle system in order to reinitialize the properties 
		/// that could not be serialized.
		/// <para>NOTE: If this type of particle system requires a Texture, this function will attempt to load the Texture specified
		/// by the DeserializationTexturePath property. If it is unable to load a texture, a DPSFArgumentNullException will be thrown, so 
		/// this function should be wrapped in a try block, and when a DPSFArgumentNullException is caught then the particle system's
		/// texture should be manually set.</para>
		/// <para>NOTE: This will attempt to load the Effect and Technique specified by the DeserializationEffectPath and
		/// DeserializationTechniqueName properties. If either of these are null, the DPSFDefaultEffect will be used, and the default
		/// Technique for this type of particle system will be loaded.</para>
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. You may pass in null for this parameter if not using a Game object.</param>
		/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
		/// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
		void InitializeNonSerializableProperties(Game cGame, GraphicsDevice cGraphicsDevice, ContentManager cContentManager);

		/// <summary>
		/// The path used to load the Texture when the InitializeNonSerializableProperties() function is called.
		/// <para>NOTE: This is automatically set when the SetTexture() function is called.</para>
		/// </summary>
		string DeserializationTexturePath { get; set; }

		/// <summary>
		/// The path used to load the Effect when the InitializeNonSerializableProperties() function is called.
		/// <para>NOTE: This is automatically set when the SetEffectAndTechnique(string, string) function is called. </para>
		/// </summary>
		string DeserializationEffectPath { get; set; }

		/// <summary>
		/// The Name of the Technique to use when the InitializeNonSerializableProperties() function is called.
		/// <para>NOTE: This is automatically set when the SetEffectAndTechnique() and SetTechnique() functions are called.</para>
		/// </summary>
		string DeserializationTechniqueName { get; set; }

		/// <summary>
		/// Returns true if the Particle System is Initialized, false if not.
		/// </summary>
		/// <returns>Returns true if the Particle System is Initialized, false if not.</returns>
		bool IsInitialized { get; }

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
		/// not inheriting from DrawableGameComponent seamless; just be aware
		/// that the updates and draws are actually being performed when the
		/// Game object is told to update and draw (i.e. when base.Update() and base.Draw()
		/// are called), not when these functions are being called.</para>
		/// </summary>
		bool InheritsDrawableGameComponent { get; }

		/// <summary>
		/// Get the unique ID of this Particle System.
		/// <para>NOTE: Each Particle System is automatically assigned a unique ID when it is instantiated.</para>
		/// </summary>
		int ID { get; }

		/// <summary>
		/// Get / Set the Type of Particle System this is.
		/// </summary>
		int Type { get; set; }

		/// <summary>
		/// Get the Name of the Class that this Particle System is using. This can be used to 
		/// check what type of Particle System this is at run-time.
		/// </summary>
		string ClassName { get; }

		/// <summary>
		/// Get / Set the Content Manager to use to load Textures and Effects.
		/// </summary>
		ContentManager ContentManager { get; set; }

		/// <summary>
		/// Get the render properties used to draw the particles.
		/// </summary>
		RenderProperties RenderProperties { get; }

		/// <summary>
		/// Returns if this particle system is dependent on an external Sprite Batch to draw its particles or not.
		/// <para>If false, the particle system will use its own SpriteBatch to draw its particles.</para>
		/// <para>If true, then you must call SpriteBatch.Begin() before calling ParticleSystem.Draw() to
		/// draw the particle system, and then call SpriteBatch.End() when done drawing the particle system, where
		/// the SpriteBatch referred to here is the one you passed into the InitializeSpriteParticleSystem() function.</para>
		/// <para>NOTE: This property only applies to Sprite particle systems.</para>
		/// </summary>
		bool UsingExternalSpriteBatchToDrawParticles { get; }

		/// <summary>
		/// Returns the SpriteBatch used to draw the Sprite particles.
		/// <para>NOTE: If this is not a Sprite particle system, this will return null.</para>
		/// </summary>
		SpriteBatch SpriteBatch { get; }

		/// <summary>
		/// The Sprite Batch drawing Settings used in the Sprite Batch's Begin() function call.
		/// <para>NOTE: These settings only have effect if this is a Sprite particle system.</para>
		/// </summary>
		SpriteBatchSettings SpriteBatchSettings { get; }

		/// <summary>
		/// The Settings used to control the Automatic Memory Manager.
		/// </summary>
		AutoMemoryManagerSettings AutoMemoryManagerSettings { get; }

		/// <summary>
		/// The Emitter is used to automatically generate new Particles.
		/// <para>NOTE: This is just a pointer to one of the ParticleEmitters in the Emitters ParticleEmitterCollection.</para>
		/// <para>NOTE: If you set this to a ParticleEmitter that is not in the Emitters collection, it will be added to it.</para>
		/// <para>During the particle system Update() this Emitter property is updated to point to the ParticleEmitter in the Emitters collection that is being updated.</para>
		/// </summary>
		ParticleEmitter Emitter { get; set; }

		/// <summary>
		/// The Emitters used to automatically generate new Particles for this Particle System.
		/// <para>Each particle system Update() will loop through all Emitters in this collection and add their new particles to this particle system.</para>
		/// <para>During the particle system Update() the Emitter property is updated to point to the ParticleEmitter in this collection that is being updated.</para>
		/// </summary>
		ParticleEmitterCollection Emitters { get; }

		/// <summary>
		/// Get a Random object used to generate Random Numbers.
		/// </summary>
		RandomNumbers RandomNumber { get; }

		/// <summary>
		/// Get / Set the World Matrix to use for drawing 3D Particles.
		/// </summary>
		Matrix World { get; set; }

		/// <summary>
		/// Get / Set the View Matrix to use for drawing 3D Particles.
		/// </summary>
		Matrix View { get; set; }

		/// <summary>
		/// Get / Set the Projection Matrix to use for drawing 3D Particles.
		/// </summary>
		Matrix Projection { get; set; }

		/// <summary>
		/// Gets the result of multiplying the World, View, and Projection matrices.
		/// </summary>
		Matrix WorldViewProjection { get; }

		/// <summary>
		/// Set the World, View, and Projection matrices for this Particle System.
		/// </summary>
		/// <param name="cWorld">The World matrix</param>
		/// <param name="cView">The View matrix</param>
		/// <param name="cProjection">The Projection matrix</param>
		void SetWorldViewProjectionMatrices(Matrix cWorld, Matrix cView, Matrix cProjection);

		/// <summary>
		/// Sets the Effect to be the DPSFDefaultEffect, and the Technique to be the default technique for this type of particle system.
		/// This is done automatically when the particle system is initialized.
		/// </summary>
		void SetDefaultEffect();

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
		/// it look as desired, without having to rescale all of the particle velocities, etc. This allows
		/// you to use the exact same particle system class to create two particle systems, and then have one run
		/// slower or faster than the other, creating two different effects. If you then wanted to speed up or slow down
		/// both effects (i.e. particle systems), you could adjust the SimulationSpeed property on both particle systems 
		/// without having to worry about adjusting this property at all to get the effects back to normal speed; just reset 
		/// the SimulationSpeed property you changed back to 1.0.</para>
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
        /// Get / Set if performance timings should be measured or not, such as how long it takes to perform updates and draws.
        /// <para>This should be disabled before building a release version of your application.</para>
        /// <para>Note: Performance profiling is not available on the Reach profile, so this will always return False on the Reach profile.</para>
        /// </summary>
        bool PerformanceProfilingIsEnabled { get; set; }

        /// <summary>
        /// Get how long (in milliseconds) it took to perform the last Update() function call.
        /// <para>Returns 0 if Performance Profiling is not Enabled.</para>
        /// </summary>
        double PerformanceTimeToDoUpdateInMilliseconds { get; }

        /// <summary>
        /// Get how long (in milliseconds) it took to perform the last Draw() function call.
        /// <para>Returns 0 if Performance Profiling is not Enabled.</para>
        /// </summary>
        double PerformanceTimeToDoDrawInMilliseconds { get; }

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
        /// Get the number of particles that memory has been allocated for, for both this particle system and any particle systems contained within this one.
        /// <para>NOTE: Because a particle system may contain other particle systems, this is a virtual function that may be overridden to return the 
        /// NumberOfParticlesAllocatedInMemory for both this particle system and any child particle systems that are contained within this one.</para>
        /// <para>NOTE: By default this just returns this particle system's NumberOfParticlesAllocatedInMemory.</para>
        /// </summary>
        int TotalNumberOfParticlesAllocatedInMemory { get; }

		/// <summary>
		/// Get / Set the Max Number of Particles this Particle System is Allowed to contain at any given time.
		/// <para>NOTE: The Automatic Memory Manager will never allocate space for more Particles than this.</para>
		/// <para>NOTE: This value must be greater than or equal to zero.</para>
		/// </summary>
		int MaxNumberOfParticlesAllowed { get; set; }

		/// <summary>
		/// Get the number of Particles that are currently Active.
		/// </summary>
		int NumberOfActiveParticles { get; }

        /// <summary>
        /// Get the number of particles that are currently Active, in both this particle system and any particle systems contained within this one.
        /// <para>NOTE: Because a particle system may contain other particle systems, this is a virtual function that may be overridden to return the 
        /// NumberOfActiveParticles for both this particle system and any child particle systems that are contained within this one.</para>
        /// <para>NOTE: By default this just returns this particle system's NumberOfActiveParticles.</para>
        /// </summary>
        int TotalNumberOfActiveParticles { get; }

		/// <summary>
		/// Get the number of Particles being Drawn. That is, how many Particles are both Active AND Visible.
		/// </summary>
		int NumberOfParticlesBeingDrawn { get; }

        /// <summary>
        /// Get the number of particles that are being Drawn, in both this particle system and any particle systems contained within this one.
        /// <para>NOTE: Because a particle system may contain other particle systems, this is a virtual function that may be overridden to return the 
        /// NumberOfParticlesBeingDrawn for both this particle system and any child particle systems that are contained within this one.</para>
        /// <para>NOTE: By default this just returns this particle system's NumberOfParticlesBeingDrawn.</para>
        /// </summary>
        int TotalNumberOfParticlesBeingDrawn { get; }

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
		/// Particle system properties should not be set until after this is called, as 
		/// they are likely to be reset to their default values.
		/// </summary>
		/// <param name="cGraphicsDevice">The Graphics Device the Particle System should use</param>
		/// <param name="cContentManager">The Content Manager the Particle System should use to load resources</param>
		/// <param name="cSpriteBatch">The Sprite Batch that the Sprite Particle System should use to draw its particles.
		/// If this is not initializing a Sprite particle system, or you want the particle system to use its own Sprite Batch,
		/// pass in null.</param>
		void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch);

		/// <summary>
		/// Sets the Camera Position of the particle system, so that the particles know how to make themselves face the camera if needed.
		/// This virtual function does not do anything unless overridden, and all it should do is set an internal Vector3 variable
		/// (e.g. public Vector3 CameraPosition { get; set; }) to match the given Vector3.
		/// </summary>
		/// <param name="cameraPosition">The position that the camera is currently at.</param>
		void SetCameraPosition(Vector3 cameraPosition);
	}

	/// <summary>
	/// Interface that must be implemented by all Particle Vertex's
	/// </summary>
	public interface IDPSFParticleVertex : IVertexType
	{ }
}
