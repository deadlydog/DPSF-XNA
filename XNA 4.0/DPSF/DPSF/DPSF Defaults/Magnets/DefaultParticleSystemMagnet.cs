using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DPSF
{
	/// <summary>
	/// The base class that all Magnet classes inherit from. This class cannot be instantiated directly.
	/// A Magnet of a Particle System has an affect on its Particles, such as attracting or repelling them.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DefaultParticleSystemMagnet
	{
		/// <summary>
		/// The Modes that the Magnet can be in
		/// </summary>
#if (WINDOWS)
		[Serializable]
#endif
		public enum MagnetModes
		{
			/// <summary>
			/// Attract Particles to the Magnet.
			/// </summary>
			Attract = 0,

			/// <summary>
			/// Repel Particles from the Magnet.
			/// </summary>
			Repel = 1,

			/// <summary>
			/// Have some other custom effect on the Particles
			/// </summary>
			Other = 2
		}

		/// <summary>
		/// Function used to determine how much the Magnet should affect a Particle, based on how far away the Particle is from the Magnet.
		/// </summary>
#if (WINDOWS)
		[Serializable]
#endif
		public enum DistanceFunctions
		{
			/// <summary>
			/// As long as the Particle's distance from the Magnet is between the Min and Max Distance of 
			/// the Magnet, the Max Force will be applied to the Particle.
			/// <para>Function Logic: y = 1, where y is the normalized Force applied and 1 is the Particle's
			/// normalized distance between the Magnet's Min and Max Distances.</para>
			/// </summary>
			Constant = 0,

			/// <summary>
			/// The Force applied to the Particle will be Linearly interpolated from zero to Max Force
			/// based on the Particle's distance from the Magnet between the Magnet's Min and Max Distances. 
			/// So when the Particle is at the Min Distance from the Magnet no force will be applied, when 
			/// the Particle is at half of the Max Distance from the Magnet 1/2 of the Max Force will be 
			/// applied, and when the Particle is at the Max Distance from the Magnet the Max Force will be 
			/// applied to the Particle.
			/// <para>Function Logic: y = x, where y is the normalized Force applied and x is the Particle's 
			/// normalized distance between the Magnet's Min and Max Distances.</para>
			/// </summary>
			Linear = 1,

			/// <summary>
			/// The Force applied to the Particle will be Squared interpolated from zero to Max Force
			/// based on the Particle's distance from the Magnet between the Magnet's Min and Max Distances. 
			/// So when the Particle is at the Min Distance from the Magnet no force will be applied, when 
			/// the Particle is at half of the Max Distance from the Magnet 1/4 of the Max Force will be 
			/// applied, and when the Particle is at the Max Distance from the Magnet the Max Force will be 
			/// applied to the Particle.
			/// <para>Function Logic: y = x * x, where y is the normalized Force applied and x is the Particle's
			/// normalized distance between the Magnet's Min and Max Distances.</para>
			/// </summary>
			Squared = 2,

			/// <summary>
			/// The Force applied to the Particle will be Cubed interpolated from zero to Max Force
			/// based on the Particle's distance from the Magnet between the Magnet's Min and Max Distances.
			/// So when the Particle is at the Min Distance from the Magnet no force will be applied, when 
			/// the Particle is at half of the Max Distance from the Magnet 1/8 of the Max Force will be 
			/// applied, and when the Particle is at the Max Distance from the Magnet the Max Force will be 
			/// applied to the Particle.
			/// <para>Function Logic: y = x * x * x, where y is the normalized Force applied and x is the Particle's
			/// normalized distance between the Magnet's Min and Max Distances.</para>
			/// </summary>
			Cubed = 3,

			/// <summary>
			/// The Inverse of the Linear function. That is, when the Particle is at the Min Distance from
			/// the Magnet the Max Force will be applied, when the Particle is at half of the Max Distance
			/// from the Magnet 1/2 of the Max Force will be applied, and when the Particle is at the Max Distance
			/// from the Magnet no force will be applied to the Particle.
			/// </summary>
			LinearInverse = 4,

			/// <summary>
			/// The Inverse of the Squared function. That is, when the Particle is at the Min Distance from
			/// the Magnet the Max Force will be applied, when the Particle is at half of the Max Distance
			/// from the Magnet 1/4 of the Max Force will be applied, and when the Particle is at the Max Distance
			/// from the Magnet no force will be applied to the Particle.
			/// </summary>
			SquaredInverse = 5,

			/// <summary>
			/// The Inverse of the Cubed function. That is, when the Particle is at the Min Distance from
			/// the Magnet the Max Force will be applied, when the Particle is at half of the Max Distance
			/// from the Magnet 1/8 of the Max Force will be applied, and when the Particle is at the Max Distance
			/// from the Magnet no force will be applied to the Particle.
			/// </summary>
			CubedInverse = 6
		}

		/// <summary>
		/// The Types of Magnets available to choose from (i.e. which Magnet class is being used)
		/// </summary>
#if (WINDOWS)
		[Serializable]
#endif
		public enum MagnetTypes
		{
			/// <summary>
			/// User-Defined Magnet Type (i.e. an instance of a user-defined Magnet class, not a Magnet class provided by DPSF)
			/// </summary>
			UserDefinedMagnet = 0,

			/// <summary>
			/// Point Magnet (i.e. an instance of the MagnetPoint class)
			/// </summary>
			PointMagnet = 1,

			/// <summary>
			/// Line Magnet (i.e. an instance of the MagnetLine class)
			/// </summary>
			LineMagnet = 2,

			/// <summary>
			/// Line Segment Magnet (i.e. an instance of the MagnetLineSegment class)
			/// </summary>
			LineSegmentMagnet = 3,

			/// <summary>
			/// Plane Magnet (i.e. an instance of the PlaneMagnet class)
			/// </summary>
			PlaneMagnet = 4
		}

		/// <summary>
		/// The current Mode that the Magnet is in
		/// </summary>
		public MagnetModes Mode = MagnetModes.Attract;

		/// <summary>
		/// The Function to use to determine how much a Particle should be affected by 
		/// the Magnet based on how far away from the Magnet it is.
		/// </summary>
		public DistanceFunctions DistanceFunction = DistanceFunctions.Linear;

		/// <summary>
		/// Holds the Type of Magnet this is
		/// </summary>
		protected MagnetTypes meMagnetType = MagnetTypes.UserDefinedMagnet;

		/// <summary>
		/// Gets what Type of Magnet this is
		/// </summary>
		public MagnetTypes MagnetType
		{
			get { return meMagnetType; }
		}

		/// <summary>
		/// The Min Distance that the Magnet should be able to affect Particles at. If the
		/// Particle is closer to the Magnet than this distance, the Magnet will not affect
		/// the Particle.
		/// </summary>
		public float MinDistance = 0.0f;

		/// <summary>
		/// The Max Distance that the Magnet should be able to affect Particles at. If the
		/// Particle is further away from the Magnet than this distance, the Magnet will not
		/// affect the Particle.
		/// </summary>
		public float MaxDistance = 100;

		/// <summary>
		/// The Maximum Force that the Magnet is able to exert on a Particle.
		/// <para>The Force exerted on a particle by a Magnet may vary depending on which Distance Function is used, 
		/// and the particle's distance from the Magnet.</para>
		/// </summary>
		public float MaxForce = 1;

		/// <summary>
		/// The Type of User-Defined Magnet this is. User-defined Magnet classes will all have a 
		/// MagnetType = MagnetTypes.UserDefined, so this field can be used to distinguish between 
		/// different user-defined Magnet classes.
		/// This may be used in conjunction with the "Other" Magnet Mode to distinguish which type of 
		/// custom user effect the Magnet should have on the Particles.
		/// </summary>
		public int UserDefinedMagnetType = 0;

		// Static variable to assign unique IDs to the Magnets
		static private int SmiCounter = 0;

		// The unique ID of the Magnet
		private int miID = SmiCounter++;

		/// <summary>
		/// Get the unique ID of this Magnet
		/// </summary>
		public int ID
		{
			get { return miID; }
		}

		/// <summary>
		/// Explicit Constructor
		/// </summary>
		/// <param name="eMode">The Mode that the Magnet should be in</param>
		/// <param name="eDistanceFunction">The Function to use to determine how much a Particle should be affected by 
		/// the Magnet based on how far away from the Magnet it is</param>
		/// <param name="fMinDistance">The Min Distance that the Magnet should be able to affect Particles at. If the
		/// Particle is closer to the Magnet than this distance, the Magnet will not affect the Particle.</param>
		/// <param name="fMaxDistance">The Max Distance that the Magnet should be able to affect Particles at. If the
		/// Particle is further away from the Magnet than this distance, the Magnet will not affect the Particle.</param>
		/// <param name="fMaxForce">The Max Force that the Magnet is able to exert on a Particle</param>
		/// <param name="iType">The Type of Magnet this is. This may be used in conjunction with the "Other" Magnet
		/// Mode to distinguish which type of custom user effect the Magnet should have on the Particles.</param>
		protected DefaultParticleSystemMagnet(MagnetModes eMode, DistanceFunctions eDistanceFunction,
										   float fMinDistance, float fMaxDistance, float fMaxForce, int iType)
		{
			Mode = eMode;
			DistanceFunction = eDistanceFunction;
			MinDistance = fMinDistance;
			MaxDistance = fMaxDistance;
			MaxForce = fMaxForce;
			UserDefinedMagnetType = iType;
		}

		/// <summary>
		/// Copy Constructor
		/// </summary>
		/// <param name="cMagnetToCopy">The Magnet to copy from</param>
		protected DefaultParticleSystemMagnet(DefaultParticleSystemMagnet cMagnetToCopy)
		{
			CopyFrom(cMagnetToCopy);
		}

		/// <summary>
		/// Copies the given Magnet's data into this Magnet's data
		/// </summary>
		/// <param name="cMagnetToCopy">The Magnet to copy from</param>
		public void CopyFrom(DefaultParticleSystemMagnet cMagnetToCopy)
		{
			Mode = cMagnetToCopy.Mode;
			DistanceFunction = cMagnetToCopy.DistanceFunction;
			meMagnetType = cMagnetToCopy.MagnetType;
			MinDistance = cMagnetToCopy.MinDistance;
			MaxDistance = cMagnetToCopy.MaxDistance;
			MaxForce = cMagnetToCopy.MaxForce;
			UserDefinedMagnetType = cMagnetToCopy.UserDefinedMagnetType;
		}
	}
}
