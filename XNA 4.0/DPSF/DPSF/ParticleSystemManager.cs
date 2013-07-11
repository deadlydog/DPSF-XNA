#region Using Statements
using System;
using System.Collections.Generic;
using DPSF.Exceptions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF
{
    /// <summary>
    /// Class to manage the Updating and Drawing of DPSF Particle Systems each frame
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
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

        // Variable used to control how often the particle systems are updated
        private int miUpdatesPerSecond = 0;
        private bool mbUseManagersUpdatesPerSecond = true;

		/// <summary>
		/// Handle to the particle system whose Update() function is currently being called.
		/// We need this in case a PS removes itself from the Manager during its Update() function.
		/// </summary>
		private IDPSFParticleSystem _particleSystemBeingUpdated = null;
		private bool _isParticleSystemBeingUpdatedRemovedFromManager = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleSystemManager"/> class.
        /// </summary>
        public ParticleSystemManager()
        {
            this.UpdatesPerSecond = DPSFDefaultSettings.UpdatesPerSecond;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleSystemManager"/> class, copying the settings of the given Particle System Manager.
        /// </summary>
        /// <param name="managerToCopy">The Particle System Manager to copy from.</param>
        public ParticleSystemManager(ParticleSystemManager managerToCopy)
            : this()
        {
            CopyFrom(managerToCopy);
        }

        /// <summary>
        /// Copies the given DPSF Particle System Manager's information into this Manager.
        /// </summary>
        /// <param name="cManagerToCopy">The Particle System Manager to copy from.</param>
        public void CopyFrom(ParticleSystemManager cManagerToCopy)
        {
            // Copy the Value types
            mbPerformUpdates = cManagerToCopy.mbPerformUpdates;
            mbPerfomDraws = cManagerToCopy.mbPerfomDraws;
            mbAParticleSystemsUpdateOrderWasChanged = cManagerToCopy.mbAParticleSystemsUpdateOrderWasChanged;
            mbAParticleSystemsDrawOrderWasChanged = cManagerToCopy.mbAParticleSystemsDrawOrderWasChanged;
            mfSimulationSpeed = cManagerToCopy.mfSimulationSpeed;
            mbUseManagersSimulationSpeed = cManagerToCopy.mbUseManagersSimulationSpeed;
            miUpdatesPerSecond = cManagerToCopy.miUpdatesPerSecond;
            mbUseManagersUpdatesPerSecond = cManagerToCopy.mbUseManagersUpdatesPerSecond;

            // Deep copy the Reference types

            // Loop through all of the Manager To Copy's Particle Systems
            int iNumberOfParticleSystems = cManagerToCopy.ParticleSystems.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // Add the Particle System to this List of Particle Systems
                AddParticleSystem(cManagerToCopy.ParticleSystems[iIndex]);
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
        /// Get / Set if the Particle Systems should be Updated or not.
        /// </summary>
        public bool Enabled
        {
            get { return mbPerformUpdates; }
            set { mbPerformUpdates = true; }
        }

        /// <summary>
        /// Get / Set if this Particle Systems should be drawn or not.
        /// <para>NOTE: Setting this to false causes the particle systems' Draw() function to not be called, including the 
        /// particle systems' BeforeDraw() and AfterDraw() functions.</para>
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
            set 
            { 
                mbUseManagersSimulationSpeed = value; 
            
                // Apply the Particle System Manager's Simulation Speed to all of the particle systems
                if (mbUseManagersSimulationSpeed)
                    SimulationSpeed = SimulationSpeed;
            }
        }

        /// <summary>
        /// Get / Set if the Particle System Manager's UpdatesPerSecond property
        /// should be used for each of the particle systems it contains or not.
        /// <para>Default value is true.</para>
        /// </summary>
        public bool UpdatesPerSecondIsEnabled
        {
            get { return mbUseManagersUpdatesPerSecond; }
            set 
            { 
                mbUseManagersUpdatesPerSecond = value;

                // Apply the Particle System Manager's Updates Per Second to all of the particle systems
                if (mbUseManagersUpdatesPerSecond)
                    UpdatesPerSecond = UpdatesPerSecond;
            }
        }

        /// <summary>
        /// Get / Set how fast the Particle System Simulations should run. 
        /// <para>Example: 1.0 = normal speed, 0.5 = half speed, 2.0 = double speed.</para>
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
        /// <para>NOTE: A value of zero means update the particle systems every time Update() is called.</para>
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
                    iActiveParticleCount += mcParticleSystemListSortedByUpdateOrder[iIndex].TotalNumberOfActiveParticles;
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
                    iActiveAndVisibleParticleCount += mcParticleSystemListSortedByUpdateOrder[iIndex].TotalNumberOfParticlesBeingDrawn;
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
                    iNumberOfParticlesAllocatedInMemory += mcParticleSystemListSortedByUpdateOrder[iIndex].TotalNumberOfParticlesAllocatedInMemory;
                }

                // Return the Total Number of Particles Allocated In Memory
                return iNumberOfParticlesAllocatedInMemory;
            }
        }

        /// <summary>
        /// Gets the cumulative time (in milliseconds) it took to perform the Update() function on each particle system in this manager.
        /// <para>Note: Only particle systems that have their PerformanceProfilingIsEnabled property set to true will be included in this total.</para>
        /// </summary>
        public double TotalPerformanceTimeToDoUpdatesInMilliseconds { get; private set; }

        /// <summary>
        /// Gets the cumulative time (in milliseconds) it took to perform the Draw() function on each particle system in this manager.
        /// <para>Note: Only particle systems that have their PerformanceProfilingIsEnabled property set to true will be included in this total.</para>
        /// </summary>
        public double TotalPerformanceTimeToDoDrawsInMilliseconds { get; private set; }

        /// <summary>
        /// Sets each individual Particle Systems' Simulation Speed to the specified Simulation Speed.
        /// </summary>
        /// <param name="fSimulationSpeed">The new Simulation Speed that all Particle Systems 
        /// currently in this Manager should have</param>
        public void SetSimulationSpeedForAllParticleSystems(float fSimulationSpeed)
        {
            // Loop through all of the Particle Systems
            IDPSFParticleSystem particleSystem = null;
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this Particle System is Initialized
                particleSystem = mcParticleSystemListSortedByUpdateOrder[iIndex];
                if (particleSystem.IsInitialized)
                {
                    // Set this Particle System's Simulation Speed
                    particleSystem.SimulationSpeed = fSimulationSpeed;
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
            IDPSFParticleSystem particleSystem = null;
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this Particle System is Initialized
                particleSystem = mcParticleSystemListSortedByUpdateOrder[iIndex];
                if (particleSystem.IsInitialized)
                {
                    // Set this Particle System's Simulation Speed
                    particleSystem.UpdatesPerSecond = iUpdatesPerSecond;
                }
            }
        }

        /// <summary>
        /// Sets the PerformanceProfilingIsEnabled property of all particle systems in this manager to the given value.
        /// </summary>
        /// <param name="performanceProfilingIsEnabled">Set if Performance Profiling should be enabled or not.</param>
        public void SetPerformanceProfilingIsEnabledForAllParticleSystems(bool performanceProfilingIsEnabled)
        {
            // Loop through all of the Particle Systems
            IDPSFParticleSystem particleSystem = null;
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this Particle System is Initialized
                particleSystem = mcParticleSystemListSortedByUpdateOrder[iIndex];
                if (particleSystem.IsInitialized)
                {
                    // Update its Camera Position
                    particleSystem.PerformanceProfilingIsEnabled = performanceProfilingIsEnabled;
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
            IDPSFParticleSystem particleSystem = null;
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this Particle System is Initialized
                particleSystem = mcParticleSystemListSortedByUpdateOrder[iIndex];
                if (particleSystem.IsInitialized)
                {
                    // Set the World, View, and Projection Matrices for this Particle System
                    particleSystem.SetWorldViewProjectionMatrices(cWorld, cView, cProjection);
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
            IDPSFParticleSystem particleSystem = null;
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this is a Sprite Particle System and it is Initialized
                particleSystem = mcParticleSystemListSortedByUpdateOrder[iIndex];
                if (particleSystem.ParticleType == ParticleTypes.Sprite && particleSystem.IsInitialized)
                {
                    // Set the Transformation Matrix for this Particle System
                    particleSystem.SpriteBatchSettings.TransformationMatrix = sTransformationMatrix;
                }
            }
        }

        /// <summary>
        /// Sets the CameraPosition property of all particle systems in this manager to the given Camera Position.
        /// This is done by calling the particle system's virtual SetCameraPosition() function.
        /// </summary>
        /// <param name="cameraPosition">The current position of the Camera.</param>
        public void SetCameraPositionForAllParticleSystems(Vector3 cameraPosition)
        {
            // Loop through all of the Particle Systems
            IDPSFParticleSystem particleSystem = null;
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this Particle System is Initialized
                particleSystem = mcParticleSystemListSortedByUpdateOrder[iIndex];
                if (particleSystem.IsInitialized)
                {
                    // Update its Camera Position
                    particleSystem.SetCameraPosition(cameraPosition);
                }
            }
        }

        /// <summary>
        /// Sets the Enabled property of all particle systems in this manager to the given state.
        /// </summary>
        /// <param name="isEnabled">If the particle systems should be enabled or not.</param>
        public void SetEnabledStateForAllParticleSystems(bool isEnabled)
        {
            // Loop through all of the Particle Systems
            IDPSFParticleSystem particleSystem = null;
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this Particle System is Initialized
                particleSystem = mcParticleSystemListSortedByUpdateOrder[iIndex];
                if (particleSystem.IsInitialized)
                {
                    // Update its Enabled state.
                    particleSystem.Enabled = isEnabled;
                }
            }
        }

        /// <summary>
        /// Sets the Visible property of all particle systems in this manager to the given state.
        /// </summary>
        /// <param name="isVisible">If the particle systems should be visible or not.</param>
        public void SetVisibleStateForAllParticleSystems(bool isVisible)
        {
            // Loop through all of the Particle Systems
            IDPSFParticleSystem particleSystem = null;
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this Particle System is Initialized
                particleSystem = mcParticleSystemListSortedByUpdateOrder[iIndex];
                if (particleSystem.IsInitialized)
                {
                    // Update its Visible state.
                    particleSystem.Visible = isVisible;
                }
            }
        }

        /// <summary>
        /// Sets the Enabled and Visible properties of all particle systems in this manager to the given states.
        /// </summary>
        /// <param name="isEnabled">If the particle systems should be enabled or not.</param>
        /// <param name="isVisible">If the particle systems should be visible or not.</param>
        public void SetEnabledAndVisibleStatesForAllParticleSystems(bool isEnabled, bool isVisible)
        {
            // Loop through all of the Particle Systems
            IDPSFParticleSystem particleSystem = null;
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this Particle System is Initialized
                particleSystem = mcParticleSystemListSortedByUpdateOrder[iIndex];
                if (particleSystem.IsInitialized)
                {
                    // Update its Enabled and Visible state.
                    particleSystem.Enabled = isEnabled;
                    particleSystem.Visible = isVisible;
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
            // If an invalid particle system was given, throw an exception indicating it
            if (cParticleSystemToAdd == null)
                throw new DPSFArgumentNullException("cParticleSystemToAdd", "A particle system with a value of null cannot be added to the particle system manager.");

            // Add the Particle System to both Lists
            mcParticleSystemListSortedByUpdateOrder.Add(cParticleSystemToAdd);
            mcParticleSystemListSortedByDrawOrder.Add(cParticleSystemToAdd);

            // Add to the Particle System's UpdateOrderChanged and DrawOrderChanged events
            cParticleSystemToAdd.UpdateOrderChanged += new EventHandler<EventArgs>(ParticleSystem_UpdateOrderChanged);
            cParticleSystemToAdd.DrawOrderChanged += new EventHandler<EventArgs>(ParticleSystem_DrawOrderChanged);

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
            // If an invalid particle system was given, throw an exception indicating it
            if (cParticleSystemToRemove == null)
                throw new DPSFArgumentNullException("cParticleSystemToRemove", "A particle system with a value of null cannot be removed from the particle system manager.");

			// If the particle system being removed is actually removing itself right now from its Update() function which this PS Manager just called.
			if (cParticleSystemToRemove == _particleSystemBeingUpdated)
			{
				// Record that the PS being Updated right now has been removed from this Manager.
				_isParticleSystemBeingUpdatedRemovedFromManager = true;
			}

            // Remove the Event Handlers we attached to the Particle System
            cParticleSystemToRemove.UpdateOrderChanged -= new EventHandler<EventArgs>(ParticleSystem_UpdateOrderChanged);
            cParticleSystemToRemove.DrawOrderChanged -= new EventHandler<EventArgs>(ParticleSystem_DrawOrderChanged);

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
            IDPSFParticleSystem particleSystem = null;
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If this is the Particle System to remove
                particleSystem = mcParticleSystemListSortedByUpdateOrder[iIndex];
                if (particleSystem.ID == iIDOfParticleSystemToRemove)
                {
                    // Remove this Particle System from the Particle System Manager
                    // NOTE: We call the other Remove function since it removes the Event Handlers as well
                    return RemoveParticleSystem(particleSystem);
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

            // Loop through all of the Particle Systems, in reverse order since we are removing from the list we are iterating through
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = (iNumberOfParticleSystems - 1); iIndex >= 0; iIndex--)
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
        /// <param name="cSpriteBatch">The Sprite Batch that the Sprite Particle System should use to draw its particles.
        /// If this is not initializing a Sprite particle system, or you want the particle system to use its own Sprite Batch,
        /// pass in null.</param>
        public void AutoInitializeAllParticleSystems(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            // Loop through all of the Particle Systems
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // AutoInitialize this Particle System
                mcParticleSystemListSortedByUpdateOrder[iIndex].AutoInitialize(cGraphicsDevice, cContentManager, cSpriteBatch);
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
        public void DestroyAndRemoveAllParticleSystems()
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

            // Reset how long it takes to Update all particle systems
            TotalPerformanceTimeToDoUpdatesInMilliseconds = 0;

			// Reset the variables used for determining if a PS removed itself from the PS Manager during its Update() function call.
			ResetParticleSystemBeingUpdatedVariables();

            // Loop through all of the Particle Systems
            int iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If the Particle System is Initialized
				_particleSystemBeingUpdated = mcParticleSystemListSortedByUpdateOrder[iIndex];
				if (_particleSystemBeingUpdated.IsInitialized)
                {
                    // Update the Particle System
					_particleSystemBeingUpdated.Update(fElapsedTimeInSeconds);

                    // Add how long it took to update this particle system to the cumulative total
					TotalPerformanceTimeToDoUpdatesInMilliseconds += _particleSystemBeingUpdated.PerformanceTimeToDoUpdateInMilliseconds;

					// Updating the particle system potentially may have removed the particle system (or others)
					// from the particle system manager, so make sure we don't go outside of the array bounds.
					iNumberOfParticleSystems = mcParticleSystemListSortedByUpdateOrder.Count;

					// If the particle system being updated to remove itself from the PS Manager.
					if (_isParticleSystemBeingUpdatedRemovedFromManager)
					{
						// Decrement our PS index so that the next PS doesn't get skipped, as it will now have this PS's index.
						iIndex--;
						_isParticleSystemBeingUpdatedRemovedFromManager = false;
					}
                }
            }

			// Reset the PS Update variables now that we are done updating the Particle Systems.
			ResetParticleSystemBeingUpdatedVariables();
        }

		/// <summary>
		/// Resets the variables used for determining if a PS removed itself from the PS Manager during its Update() function call.
		/// </summary>
		private void ResetParticleSystemBeingUpdatedVariables()
		{
			_particleSystemBeingUpdated = null;
			_isParticleSystemBeingUpdatedRemovedFromManager = false;
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

            // Reset how long it takes to Draw all particle systems
            TotalPerformanceTimeToDoDrawsInMilliseconds = 0;

            // Loop through all of the Particle Systems
            IDPSFParticleSystem particleSystem = null;
            int iNumberOfParticleSystems = mcParticleSystemListSortedByDrawOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
            {
                // If the Particle System is Initialized
                particleSystem = mcParticleSystemListSortedByDrawOrder[iIndex];
                if (particleSystem.IsInitialized)
                {
                    // Draw the Particle System
                    particleSystem.DrawForced();

                    // Add how long it took to update this particle system to the cumulative total
                    TotalPerformanceTimeToDoDrawsInMilliseconds += particleSystem.PerformanceTimeToDoDrawInMilliseconds;
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
			RenderTarget2D cOldRenderTarget = GetGraphicsDevicesCurrentRenderTarget(cGraphicsDevice);

			// Create the new Render Target for the Particle Systems to draw to
			RenderTarget2D cNewPSRenderTarget = new RenderTarget2D(cGraphicsDevice, iViewportWidth, iViewportHeight);

			// Set the Render Target the given Graphics Device should draw to
			cGraphicsDevice.SetRenderTarget(cNewPSRenderTarget);

			// Clear the scene
			cGraphicsDevice.Clear(Color.Transparent);

			// Loop through all of the Particle Systems
			IDPSFParticleSystem cParticleSystem = null;
			int iNumberOfParticleSystems = mcParticleSystemListSortedByDrawOrder.Count;
			for (int iIndex = 0; iIndex < iNumberOfParticleSystems; iIndex++)
			{
				// Get a handle to the Particle System
				cParticleSystem = mcParticleSystemListSortedByDrawOrder[iIndex];

				// If the Particle System is Initialized
				if (cParticleSystem.IsInitialized)
				{
					// Backup the Particle System's current Render Target
					RenderTarget2D cOldPSRenderTarget = GetGraphicsDevicesCurrentRenderTarget(cParticleSystem.GraphicsDevice);

					// Set the Particle System's new Render Target to use
					cParticleSystem.GraphicsDevice.SetRenderTarget(cNewPSRenderTarget);

					// Draw the Particle System
					cParticleSystem.DrawForced();

					// Restore the Particle System's old Render Target
					cParticleSystem.GraphicsDevice.SetRenderTarget(cOldPSRenderTarget);
				}
			}

			// Restore the given Graphics Device's Render Target
			cGraphicsDevice.SetRenderTarget(cOldRenderTarget);

			// Get the Texture with the Particle Systems drawn on it
			Texture2D cPSTexture = cNewPSRenderTarget;

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
				DepthStencilState cOldDepthStencilState = cGraphicsDevice.DepthStencilState;

				// Create the Render Target to draw the scaled Particle System Texture to
				RenderTarget2D cNewRenderTarget = new RenderTarget2D(cGraphicsDevice, iTextureWidth, iTextureHeight);

				// Set the given Graphics Device to draw to the new Render Target
				cGraphicsDevice.SetRenderTarget(cNewRenderTarget);

				// Make sure the Graphic Device's Depth Stencil Buffer is large enough
				cGraphicsDevice.DepthStencilState = DepthStencilState.Default;// = new DepthStencilBuffer(cGraphicsDevice, iTextureWidth, iTextureHeight, cGraphicsDevice.DepthStencilBuffer.Format);
				
				// Clear the scene
				cGraphicsDevice.Clear(Color.Transparent);

				// Create the new SpriteBatch that will be used to scale the Texture
				SpriteBatch cSpriteBatch = new SpriteBatch(cGraphicsDevice);

				// Draw the scaled Texture
				cSpriteBatch.Begin();
				cSpriteBatch.Draw(cPSTexture, new Rectangle(0, 0, iTextureWidth, iTextureHeight), Color.White);
				cSpriteBatch.End();

				// Restore the given Graphics Device's Render Target
				cGraphicsDevice.SetRenderTarget(cOldRenderTarget);

				// Restore the given Graphics Device's Depth Stencil
				cGraphicsDevice.DepthStencilState = cOldDepthStencilState;

				// Set the Texture To Return to the scaled Texture
				cTextureToReturn = cNewRenderTarget;
			}

            // Return the Texture
            return cTextureToReturn;
        }

        // The Xbox 360 doesn't have access to the System.Drawing namespace, so we can't perfom these functions on it
#if (WINDOWS)
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
        /// <param name="bCreateAnimatedGIF">Set this to true to also produce an animated GIF from the Image files generated</param>
        /// <param name="bCreateTileSetImage">Set this to true to also produce a Tile Set from the Image files generated.
        /// Be careful when setting this to true as the system may run out of memory and throw an exception if the Tile Set
        /// image to generate is too large. Exactly how large it is allowed to be differs from system to system.</param>
        public void DrawAllParticleSystemsAnimationToFiles(GraphicsDevice cGraphicsDevice, int iImageWidth, int iImageHeight,
                                                            string sDirectoryName, float fTotalAnimationTime, float fTimeStep,
                                                            bool bCreateAnimatedGIF, bool bCreateTileSetImage)
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

			// Calculate the required Width and Height of the Tile Set texture
            int iTileSetWidth = iTileSetColumns * iImageWidth;
            int iTileSetHeight = iTileSetRows * iImageHeight;

            // If we will be creating a Tile Set Image, set up its Render Target
            RenderTarget2D cTileSetRenderTarget = null;
            if (bCreateTileSetImage)
            {
                // Backup the given Graphics Device's Render Target
				RenderTarget2D cOldRenderTarget = GetGraphicsDevicesCurrentRenderTarget(cGraphicsDevice);

                // Backup the Graphics Device's Depth Stencil Buffer
                DepthStencilState cOldDepthStencilState = cGraphicsDevice.DepthStencilState;

                // Create a new RenderTarget to draw the Tile Set to (specifying to preserve it's contents)
                cTileSetRenderTarget = new RenderTarget2D(cGraphicsDevice, iTileSetWidth, iTileSetHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                // Set the given Graphics Device to draw to the new Render Target
                cGraphicsDevice.SetRenderTarget(cTileSetRenderTarget);

                // Make sure the Graphic Device's Depth Stencil Buffer is large enough
				cGraphicsDevice.DepthStencilState = DepthStencilState.Default;// new DepthStencilBuffer(cGraphicsDevice, iTileSetWidth, iTileSetHeight, cGraphicsDevice.DepthStencilBuffer.Format);

                // Clear the scene
                cGraphicsDevice.Clear(Color.Transparent);

                // Restore the given Graphics Device's Render Target
                cGraphicsDevice.SetRenderTarget(cOldRenderTarget);

                // Restore the given Graphics Device's Depth Stencil
                cGraphicsDevice.DepthStencilState = cOldDepthStencilState;
            }

            // Update the Particle System iteratively by the Time Step amount until the 
            // Particle System behavior over the Total Time has been drawn to files
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
                string sFilePath = sDirectory + "/Frame" + iFrameNumber.ToString() + ".png";

                // Save the Texture to a file
				using (System.IO.StreamWriter stream = new System.IO.StreamWriter(sFilePath))
				{
					cTexture.SaveAsPng(stream.BaseStream, iImageWidth, iImageHeight);
					stream.Close();
				}

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
                string sTileSetFilePath = sDirectory + "/TileSet.png";
				using (System.IO.StreamWriter stream = new System.IO.StreamWriter(sTileSetFilePath, false))
				{
					((Texture2D)cTileSetRenderTarget).SaveAsPng(stream.BaseStream, iTileSetWidth, iTileSetHeight);
					stream.Close();
				}
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
			RenderTarget2D cOldRenderTarget = GetGraphicsDevicesCurrentRenderTarget(cGraphicsDevice);

            // Backup the Graphics Device's Depth Stencil Buffer
            DepthStencilState cOldDepthStencilState = cGraphicsDevice.DepthStencilState;

            // Set the given Graphics Device to draw to the Tile Set's Render Target
            cGraphicsDevice.SetRenderTarget(cTileSetRenderTarget);

            // Make sure the Graphic Device's Depth Stencil Buffer is large enough
			cGraphicsDevice.DepthStencilState = DepthStencilState.Default; // new DepthStencilBuffer(cGraphicsDevice, cTileSetRenderTarget.Width, cTileSetRenderTarget.Height, cGraphicsDevice.DepthStencilBuffer.Format);

            // Create the new SpriteBatch that will be used to draw the Texture into the Tile Set
            SpriteBatch cSpriteBatch = new SpriteBatch(cGraphicsDevice);

            // Draw the Texture to the Tile Set Texture
            cSpriteBatch.Begin();
            cSpriteBatch.Draw(cTexture, sPositionAndDimensionsInTileSetToAddImage, Color.White);
            cSpriteBatch.End();

            // Restore the given Graphics Device's Render Target
            cGraphicsDevice.SetRenderTarget(cOldRenderTarget);

            // Restore the given Graphics Device's Depth Stencil
            cGraphicsDevice.DepthStencilState = cOldDepthStencilState;
        }

		/// <summary>
		/// Gets the graphics device's current render target, or null if it is not set.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device.</param>
		/// <returns></returns>
		private RenderTarget2D GetGraphicsDevicesCurrentRenderTarget(GraphicsDevice graphicsDevice)
		{
			var renderTargets = graphicsDevice.GetRenderTargets();
			if (renderTargets.Length <= 0)
				return null;
			return (RenderTarget2D)renderTargets[0].RenderTarget;
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
        private void ParticleSystem_UpdateOrderChanged(object sender, EventArgs e)
        {
            mbAParticleSystemsUpdateOrderWasChanged = true;
        }

        /// <summary>
        /// Records that the Particle Systems need to be resorted before doing the next Draws
        /// </summary>
        /// <param name="sender">The Object that sent the event</param>
        /// <param name="e">Extra information</param>
        private void ParticleSystem_DrawOrderChanged(object sender, EventArgs e)
        {
            mbAParticleSystemsDrawOrderWasChanged = true;
        }
    }
}
