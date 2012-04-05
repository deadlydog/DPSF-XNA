using System;

namespace DPSF
{
	/// <summary>
	/// Class used to hold a Particle's properties.
	/// This class only holds a Particle's Lifetime information, but may be inherited from
	/// in order to specify additional Particle properties, such as position, size, color, etc.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
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
}
