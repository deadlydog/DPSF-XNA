using System;
using Microsoft.Xna.Framework;

namespace DPSF
{
	/// <summary>
	/// The possible Modes the Automatic Memory Manager can be in
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
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
		/// This is the same as Disabled, except that if a particle is trying to be added and there is no more memory available for a new particle, the
		/// particle at the end of the ActiveParticleList will be hard-killed and recycled and used for the new particle.
		/// <para>NOTE: If using Disabled mode instead of this mode, if a new particle tries to be added and no more memory is available, the new particle simply will not be added.</para>
		/// <para>NOTE: If trying to add a new particle, but the number of Active Particles has already hit the MaxNumberOfParticlesAllowed, the particle will not be added.
		/// This setting only takes effect when trying to add a new particle and we have reached the NumberOfParticlesAllocatedInMemory, but not the MaxNumberOfParticlesAllowed.
		/// This means that in order for this setting to be effective, the MaxNumberOfParticlesAllowed must be greater than the NumberOfParticlesAllocatedInMemory.</para>
		/// <para>NOTE: When a particle is hard-killed, no more processing at all is performed on it. For example, if the particle is supposed to 
		/// trigger some other event to happen when it dies (e.g. by using a NormalizedTimedEvent), that event will not be triggered.</para>
		/// <para>NOTE: Typically the oldest particles are at the end of the ActiveParticleList and the youngest ones are at the front, so the oldest
		/// particles (i.e. the ones with the greatest ElapsedTime and a Lifetime greater than zero) will be recycled first.</para>
		/// </summary>
		DisabledWithEarlyRecycling = 1,

		/// <summary>
		/// Allow the Automatic Memory Manager to allocate more memory when needed, and reduce it when not needed.
		/// </summary>
		IncreaseAndDecrease = 2,

		/// <summary>
		/// Only allow the Automatic Memory Manager to allocate more memory when needed (cannot reduce space).
		/// </summary>
		IncreaseOnly = 3,

		/// <summary>
		/// Only allow the Automatic Memory Manager to reduce the amount of memory allocated when it is not needed (cannot increase space).
		/// </summary>
		DecreaseOnly = 4
	}

	/// <summary>
	/// Class to hold the Automatic Memory Manager Settings
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class AutoMemoryManagerSettings
	{
		/// <summary>
		/// The Memory Management Mode being used.
		/// <para>NOTE: Default value is AutoMemoryManagerModes.IncreaseAndDecrease.</para>
		/// </summary>
		public AutoMemoryManagerModes MemoryManagementMode = AutoMemoryManagerModes.IncreaseOnly;

		// Declare private variables with default values
		private int miAbsoluteMinNumberOfParticles = 10;
		private float mfReduceAmount = 1.1f;
		private float mfIncreaseAmount = 2.0f;
		private float mfSecondsMaxNumberOfParticlesMustExistForBeforeReducingSize = 3.0f;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMemoryManagerSettings"/> class.
        /// </summary>
        public AutoMemoryManagerSettings() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMemoryManagerSettings"/> class, copying all of the settings from the given Settings To Copy.
        /// </summary>
        /// <param name="settingsToCopy">The settings to copy from.</param>
        public AutoMemoryManagerSettings(AutoMemoryManagerSettings settingsToCopy)
        {
            this.CopyFrom(settingsToCopy);
        }

        /// <summary>
        /// Copies the given Auto Memory Manager Settings into this instance.
        /// </summary>
        /// <param name="settingsToCopy">The settings to copy from.</param>
        public void CopyFrom(AutoMemoryManagerSettings settingsToCopy)
        {
            this.MemoryManagementMode = settingsToCopy.MemoryManagementMode;
            this.miAbsoluteMinNumberOfParticles = settingsToCopy.miAbsoluteMinNumberOfParticles;
            this.mfReduceAmount = settingsToCopy.mfReduceAmount;
            this.mfIncreaseAmount = settingsToCopy.mfIncreaseAmount;
            mfSecondsMaxNumberOfParticlesMustExistForBeforeReducingSize = settingsToCopy.mfSecondsMaxNumberOfParticlesMustExistForBeforeReducingSize;
        }

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
}
