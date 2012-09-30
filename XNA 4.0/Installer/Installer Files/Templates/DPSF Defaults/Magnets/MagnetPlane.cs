using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DPSF
{
	/// <summary>
	/// Magnet that attracts particles to/from a plane in 3D space
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class MagnetPlane : DefaultParticleSystemMagnet
	{
		/// <summary>
		/// A 3D point on the Plane
		/// </summary>
		public Vector3 PositionOnPlane { get; set; }

		/// <summary>
		/// The Normal direction of the Plane
		/// </summary>
		private Vector3 msNormal = Vector3.Up;

		/// <summary>
		/// The Normal direction of the Plane (i.e. the up direction away from the plane). This value is 
		/// automatically normalized when it is set.
		/// </summary>
		public Vector3 Normal
		{
			get { return msNormal; }

			// Make sure the direction is normalized
			set
			{
				msNormal = value;
				msNormal.Normalize();
			}
		}

		/// <summary>
		/// Explicit Constructor
		/// </summary>
		/// <param name="sPositionOnPlane">A 3D Position on the Plane Magnet's Plane</param>
		/// <param name="sNormal">The Normal direction of the Plane (i.e. the up direction away from the plane)</param>
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
		public MagnetPlane(Vector3 sPositionOnPlane, Vector3 sNormal, MagnetModes eMode, DistanceFunctions eDistanceFunction,
							float fMinDistance, float fMaxDistance, float fMaxForce, int iType)
			: base(eMode, eDistanceFunction, fMinDistance, fMaxDistance, fMaxForce, iType)
		{
			meMagnetType = MagnetTypes.PlaneMagnet;
			PositionOnPlane = sPositionOnPlane;
			Normal = sNormal;
		}

		/// <summary>
		/// Copy Constructor
		/// </summary>
		/// <param name="cMagnetToCopy">The Plane Magnet to copy from</param>
		public MagnetPlane(MagnetPlane cMagnetToCopy)
			: base(MagnetModes.Attract, DistanceFunctions.Constant, 0, 0, 0, 0)
		{
			CopyFrom(cMagnetToCopy);
		}

		/// <summary>
		/// Copies the given Plane Magnet's data into this Plane Magnet's data
		/// </summary>
		/// <param name="cMagnetToCopy">The Plane Magnet to copy from</param>
		public void CopyFrom(MagnetPlane cMagnetToCopy)
		{
			base.CopyFrom(cMagnetToCopy);
			PositionOnPlane = cMagnetToCopy.PositionOnPlane;
			Normal = cMagnetToCopy.Normal;
		}
	}
}
