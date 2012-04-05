using System;

namespace DPSF
{
	/// <summary>
	/// Class used to automatically create new Particles in a Particle System
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class ParticleEmitter
	{
		// Variables to hold the Position, Orientation, and Pivot Point information
		private Position3D mcPositionData = null;
		private Orientation3D mcOrientationData = null;
		private PivotPoint3D mcPivotPointData = null;

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
		/// Initializes a new instance of the <see cref="ParticleEmitter"/> class.
		/// </summary>
		public ParticleEmitter()
		{
			// Initialize the Position, Orientation, and Pivot Point variables
			mcPositionData = new Position3D();
			mcOrientationData = new Orientation3D();
			mcPivotPointData = new PivotPoint3D(mcPositionData, mcOrientationData);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticleEmitter"/> class.
		/// </summary>
		/// <param name="emitterToCopy">The emitter to copy from.</param>
		public ParticleEmitter(ParticleEmitter emitterToCopy) : this()
		{
			this.CopyFrom(emitterToCopy);
		}

		/// <summary>
		/// Copies the given Emitter's values into this instance.
		/// </summary>
		/// <param name="emitterToCopy">The emitter to copy from.</param>
		public void CopyFrom(ParticleEmitter emitterToCopy)
		{
			mcPositionData.CopyFrom(emitterToCopy.mcPositionData);
			mcOrientationData.CopyFrom(emitterToCopy.mcOrientationData);
			mcPivotPointData = new PivotPoint3D(mcPositionData, mcOrientationData);

			mbEnabled = emitterToCopy.mbEnabled;
			mbEmitParticlesAutomatically = emitterToCopy.mbEmitParticlesAutomatically;

			miBurstNumberOfParticles = emitterToCopy.miBurstNumberOfParticles;
			mfBurstTimeInSeconds = emitterToCopy.mfBurstTimeInSeconds;

			mfParticlesPerSecond = emitterToCopy.mfParticlesPerSecond;
			mfSecondsPerParticle = emitterToCopy.mfSecondsPerParticle;
			mfTimeElapsedSinceGeneratingLastParticle = emitterToCopy.mfTimeElapsedSinceGeneratingLastParticle;

			BurstComplete = emitterToCopy.BurstComplete;
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
		/// <para>NOTE: This will fire the BurstComplete event when it reaches (or is set to) zero.</para>
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
		/// <para>NOTE: This will fire the BurstComplete event when it reaches (or is set to) zero.</para>
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
			if (mfParticlesPerSecond > 0.0f && mbEnabled)
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
}
