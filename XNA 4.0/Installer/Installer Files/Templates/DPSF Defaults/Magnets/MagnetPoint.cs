using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DPSF
{
	/// <summary>
	/// Magnet that attracts particles to/from a single point in 3D space
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class MagnetPoint : DefaultParticleSystemMagnet
	{
		/// <summary>
		/// The Position, Velocity, and Acceleration of the Magnet
		/// </summary>
		public Position3D PositionData = new Position3D();

		/// <summary>
		/// Explicit Constructor
		/// </summary>
		/// <param name="sPosition">The 3D Position of the Magnet</param>
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
		public MagnetPoint(Vector3 sPosition, MagnetModes eMode, DistanceFunctions eDistanceFunction,
							float fMinDistance, float fMaxDistance, float fMaxForce, int iType)
			: base(eMode, eDistanceFunction, fMinDistance, fMaxDistance, fMaxForce, iType)
		{
			meMagnetType = MagnetTypes.PointMagnet;
			PositionData.Position = sPosition;
		}

		/// <summary>
		/// Explicit Constructor
		/// </summary>
		/// <param name="cPositionData">The 3D Position, Velocity, and Acceleration of the Magnet</param>
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
		public MagnetPoint(Position3D cPositionData, MagnetModes eMode, DistanceFunctions eDistanceFunction,
							float fMinDistance, float fMaxDistance, float fMaxForce, int iType)
			: base(eMode, eDistanceFunction, fMinDistance, fMaxDistance, fMaxForce, iType)
		{
			meMagnetType = MagnetTypes.PointMagnet;
			PositionData = cPositionData;
		}

		/// <summary>
		/// Copy Constructor
		/// </summary>
		/// <param name="cMagnetToCopy">The Point Magnet to copy from</param>
		public MagnetPoint(MagnetPoint cMagnetToCopy)
			: base(MagnetModes.Attract, DistanceFunctions.Constant, 0, 0, 0, 0)
		{
			CopyFrom(cMagnetToCopy);
		}

		/// <summary>
		/// Copies the given Point Magnet's data into this Point Magnet's data
		/// </summary>
		/// <param name="cMagnetToCopy">The Point Magnet to copy from</param>
		public void CopyFrom(MagnetPoint cMagnetToCopy)
		{
			base.CopyFrom(cMagnetToCopy);
			PositionData.CopyFrom(cMagnetToCopy.PositionData);
		}
	}
}
