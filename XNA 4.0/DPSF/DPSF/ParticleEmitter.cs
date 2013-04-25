using System;
using Microsoft.Xna.Framework;

namespace DPSF
{
	/// <summary>
	/// Class containing the information necessary to Linearly Interpolate a ParticleEmitter's position and orientation between Updates when adding many particles.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class ParticleEmitterLerpInfo
	{
		/// <summary>
		/// The Emitter's Previous Position (i.e. position at last Update).
		/// </summary>
		public Vector3 PreviousPosition = Vector3.Zero;

		/// <summary>
		/// The Emitter's Current Position.
		/// </summary>
		public Vector3 CurrentPosition = Vector3.Zero;

		/// <summary>
		/// The Emitter's Previous Orientation (i.e. orientation at last Update).
		/// </summary>
		public Quaternion PreviousOrientation = Quaternion.Identity;

		/// <summary>
		/// The Emitter's Current Orientation.
		/// </summary>
		public Quaternion CurrentOrientation = Quaternion.Identity;

		/// <summary>
		/// How many seconds have elapsed since the last Update, so we know how much to linearly interpolate the Emitter's Position and Orientation.
		/// </summary>
		public float ElapsedTimeInSeconds = 0.0f;
	};

	/// <summary>
	/// Class used to automatically create new Particles in a Particle System.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class ParticleEmitter
	{
		// Variables to hold the Position, Orientation, and Pivot Point information.
		private Position3D _positionData = null;
		private Orientation3D _orientationData = null;
		private PivotPoint3D _pivotPointData = null;

		// Private variables used to determine how many Particles should be emitted.
		private float _particlesPerSecond = 0.0f;
		private float _secondsPerParticle = 0.0f;
		private float _timeElapsedSinceGeneratingLastParticle = 0.0f;

		/// <summary>
		/// The total number of emitters created.
		/// This is also used to assign unique IDs to each ParticleEmitter.
		/// </summary>
		private static int _particleEmitterCount = 0;

		/// <summary>
		/// The position of the Emitter last frame.
		/// This is internal to prevent users from messing with this value, as this is needed to properly Lerp the Emitter's position between 2 frames during a particle system Update().
		/// </summary>
		internal Vector3 PreviousPosition = Vector3.Zero;

		/// <summary>
		/// The orientation of the Emitter last frame.
		/// This is internal to prevent users from messing with this value, as this is needed to properly Lerp the Emitter's orientation between 2 frames during a particle system Updater().
		/// </summary>
		internal Quaternion PreviousOrientation = Quaternion.Identity;

		/// <summary>
		/// Raised when a Burst property reaches (or is set to) zero.
		/// </summary>
		public event EventHandler BurstComplete = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticleEmitter"/> class.
		/// </summary>
		public ParticleEmitter()
		{
			// Initialize the Position, Orientation, and Pivot Point variables.
			_positionData = new Position3DWithPreviousPosition();
			_orientationData = new Orientation3DWithPreviousOrientation();
			_pivotPointData = new PivotPoint3D(_positionData, _orientationData);

			// Disable Lerping the Emitter on the first update, since the Previous Position and Orientation won't be set yet.
			LerpEmittersPositionAndOrientationOnNextUpdate = false;
			LerpEmittersPositionAndOrientation = true;

			// Default any other properties.
			Enabled = true;
			EmitParticlesAutomatically = true;

			// Assign a unique ID to this emitter.
			ID = _particleEmitterCount++;
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
			_positionData.CopyFrom(emitterToCopy._positionData);
			_orientationData.CopyFrom(emitterToCopy._orientationData);
			_pivotPointData = new PivotPoint3D(_positionData, _orientationData);

			Enabled = emitterToCopy.Enabled;
			EmitParticlesAutomatically = emitterToCopy.EmitParticlesAutomatically;

			_burstNumberOfParticles = emitterToCopy._burstNumberOfParticles;
			_burstTimeInSeconds = emitterToCopy._burstTimeInSeconds;

			_particlesPerSecond = emitterToCopy._particlesPerSecond;
			_secondsPerParticle = emitterToCopy._secondsPerParticle;
			_timeElapsedSinceGeneratingLastParticle = emitterToCopy._timeElapsedSinceGeneratingLastParticle;

			BurstComplete = emitterToCopy.BurstComplete;
		}

		/// <summary>
		/// Get the unique ID of this Emitter.
		/// </summary>
		public int ID { get; private set; }

		/// <summary>
		/// Get / Set if the Emitter is able to Emit Particles or not.
		/// <para>NOTE: If this is false, not even Bursts will Emit Particles.</para>
		/// <para>NOTE: The Position, Orientation, and Pivot Data will still be updated when this is false.</para>
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// Get the Position Data (Position, Velocity, and Acceleration)
		/// </summary>
		public Position3D PositionData
		{
			get { return _positionData; }
		}

		/// <summary>
		/// Get the Orientation Data (Orientation, Rotational Velocity, and Rotational Acceleration)
		/// </summary>
		public Orientation3D OrientationData
		{
			get { return _orientationData; }
		}

		/// <summary>
		/// Get the Pivot Point Data (Pivot Point, Pivot Rotational Velocity, and Pivot Rotational Acceleration)
		/// </summary>
		public PivotPoint3D PivotPointData
		{
			get { return _pivotPointData; }
		}

		/// <summary>
		/// Get / Set if the Emitter should Emit Particles Automatically or not.
		/// <para>NOTE: Particles will only be emitted if the Emitter is Enabled.</para>
		/// </summary>
		public bool EmitParticlesAutomatically { get; set; }

		/// <summary>
		/// This property tells if the we should Lerp (Linearly Interpolate) the Position and Orientation of the Emitter from one update to 
		/// the next. If the Emitter is moving very fast, this allows the particle system to spawn new particles in between the Emitter's old
		/// and new position, so that new particles are evenly spaced out between the Emitter's previous and current position, instead of all 
		/// of the particles being spawned at the Emitter's new position.
		/// <para>If this property is true, the Emitter's Position and Orientation will be Lerped while emitting particles.</para>
		/// <para>If this property is false, all of the particles will be emitted as the Emitter's current Position and Orientation.</para>
		/// <para>If you generally want Lerping enabled, but want to temporarily disable it to "teleport" the emitter from one position
		/// to another without particles being Lerped between the two positions, you can set this properly to false and then back to true 
		/// after the particle system's Update() function has been called, or you can simply set the LerpEmittersPositionAndOrientationOnNextUpdate
		/// to false, which will disable Lerping the position and orientation only for the next particle system Update().</para>
		/// <para>Default is true.</para>
		/// </summary>
		[DPSFViewerParameter(Description = "This property tells if the we should Lerp (Linearly Interpolate) the Position and Orientation of the Emitter from one update to the next.", Group = "DPSF")]
		public bool LerpEmittersPositionAndOrientation { get; set; }

		/// <summary>
		/// If this is true the Emitter's Position and Orientation will not be Lerped during the particle system's next Update() function call.
		/// The Update() function will always set this value back to false after all of the particle's have been emitted for that Update() call.
		/// <para>Setting this to true allows you to "teleport" the Emitter from one position to another without particles being released at any
		/// positions in between the Emitter's old and new Position and Orientation.</para>
		/// </summary>
		public bool LerpEmittersPositionAndOrientationOnNextUpdate { get; set; }

		/// <summary>
		/// Get / Set how many Particles should be emitted per Second
		/// </summary>
		public float ParticlesPerSecond
		{
			get { return _particlesPerSecond; }
			set
			{
				// Set how many Particles to emit per second
				_particlesPerSecond = value;

				// Set how many seconds one particle takes to be emitted
				if (_particlesPerSecond == 0.0f)
				{
					_secondsPerParticle = 0.0f;
				}
				else
				{
					_secondsPerParticle = 1.0f / _particlesPerSecond;
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
			get { return _burstNumberOfParticles; }
			set
			{
				// Make sure the specified Number Of Burst Particles is not negative
				if (value <= 0)
				{
					_burstNumberOfParticles = 0;

					// If there is a function to catch the event
					if (BurstComplete != null)
					{
						// Throw the event that the Burst is Complete
						BurstComplete(this, null);
					}
				}
				else
				{
					_burstNumberOfParticles = value;
				}
			}
		}
		private int _burstNumberOfParticles = 0;

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
			get { return _burstTimeInSeconds; }
			set
			{
				// Make sure the specified Time is not negative
				if (value <= 0.0f)
				{
					_burstTimeInSeconds = 0.0f;

					// If there is a function to catch the event
					if (BurstComplete != null)
					{
						// Throw the event that the Burst is Complete
						BurstComplete(this, null);
					}
				}
				else
				{
					_burstTimeInSeconds = value;
				}
			}
		}
		private float _burstTimeInSeconds = 0.0f;

		/// <summary>
		/// How many particles this emitter has added to the particle system.
		/// <para>NOTE: This value is not automatically updated by the emitter itself; it needs to be increased manually. This is because even though
		/// the emitter may say that it wants to emit 100 particles, the particle system may only have room for 50 particles, so we would only want this
		/// value increased by 50, not 100.</para>
		/// <para>NOTE: DPSF Particle System handle updating this value automatically for you, but if you are using an emitter outside of the particle system's
		/// Emitters collection and are just manually calling the AddParticles() function yourself, then you will need to manually increase this value
		/// with whatever number is returned by the AddParticles() function.</para>
		/// <para>NOTE: The max value of an int is 2,147,483,647 so if this reaches that value it will wrap around to -2,147,483,648.</para>
		/// </summary>
		public int NumberOfParticlesEmitted { get; set; }

		/// <summary>
		/// Updates the Emitter's Position and Orientation according to its Velocities and Accelerations, and returns how many Particles should be emitted this frame.
		/// </summary>
		/// <param name="fElapsedTimeInSeconds">How long (in seconds) it has been since this function was called.</param>
		/// <returns>Returns the number of Particles that should be emitted.</returns>
		public int UpdateAndGetNumberOfParticlesToEmit(float fElapsedTimeInSeconds)
		{
			// Update the Emitter's Positional, Rotational, and Pivotal Data
			PositionData.Update(fElapsedTimeInSeconds);
			OrientationData.Update(fElapsedTimeInSeconds);
			PivotPointData.Update(fElapsedTimeInSeconds);

			// Calculate how many Particles should be generated

			// If Particles should be emitted
			int iNumberOfParticlesToEmit = 0;
			if (_particlesPerSecond > 0.0f && Enabled)
			{
				// If the Emitter is emitting Particles Automatically
				if (EmitParticlesAutomatically)
				{
					// Get how many Particles should be emitted
					iNumberOfParticlesToEmit = CalculateHowManyParticlesToEmit(fElapsedTimeInSeconds);
				}
				// Else if Burst Particles should be emitted for a specific amount of Time
				else if (_burstTimeInSeconds > 0.0f)
				{
					// Make sure we do not emit Particles for too long
					float fModifiedElapsedTime = fElapsedTimeInSeconds;
					if (fModifiedElapsedTime > _burstTimeInSeconds)
					{
						// Only emit Particles for as long as the Burst lasts
						fModifiedElapsedTime = _burstTimeInSeconds;
					}

					// Get how many Particles should be emitted
					iNumberOfParticlesToEmit = CalculateHowManyParticlesToEmit(fModifiedElapsedTime);

					// Subtract the Elapsed Time from the Burst Time
					BurstTime -= fModifiedElapsedTime;
				}
				// Else if a specific Number Of Burst Particles should be emitted
				else if (_burstNumberOfParticles > 0)
				{
					// Get how many Particles should be emitted
					iNumberOfParticlesToEmit = CalculateHowManyParticlesToEmit(fElapsedTimeInSeconds);

					// If we are emitting too many Particles
					if (iNumberOfParticlesToEmit > _burstNumberOfParticles)
					{
						iNumberOfParticlesToEmit = _burstNumberOfParticles;
					}

					// Subtract the Number Of Particles being Emitted from the Number Of Particles that still need to be Emitted
					BurstParticles -= iNumberOfParticlesToEmit;
				}
			}

			// Return how many Particles should be emitted
			return iNumberOfParticlesToEmit;
		}

		/// <summary>
		/// Calculates how many Particles should be emitted based on the amount of Time Elapsed.
		/// </summary>
		/// <param name="fElapsedTimeInSeconds">How much Time has Elapsed (in seconds) since the last Update.</param>
		/// <returns>Returns how many Particles should be emitted.</returns>
		private int CalculateHowManyParticlesToEmit(float fElapsedTimeInSeconds)
		{
			// Variable to hold how many Particles To Emit
			int iNumberOfParticlesToEmit = 0;

			// Add the Elapsed Time since the last update to the Elapsed Time since the last Particle was generated
			_timeElapsedSinceGeneratingLastParticle += fElapsedTimeInSeconds;

			// Calculate how many Particles should be generated based on the Elapsed Time
			float fNumberOfParticlesToEmit = _timeElapsedSinceGeneratingLastParticle / _secondsPerParticle;
			iNumberOfParticlesToEmit = (int)Math.Floor(fNumberOfParticlesToEmit);

			// Calculate how much time should be carried over to the next Update
			_timeElapsedSinceGeneratingLastParticle = (fNumberOfParticlesToEmit - iNumberOfParticlesToEmit) * _secondsPerParticle;

			// Return how many Particles should be emitted
			return iNumberOfParticlesToEmit;
		}
	}
}
