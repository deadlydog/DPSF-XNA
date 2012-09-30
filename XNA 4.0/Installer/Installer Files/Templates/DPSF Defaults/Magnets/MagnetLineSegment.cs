using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DPSF
{
	/// <summary>
	/// Magnet that attracts particles to/from a line with specified end points in 3D space
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class MagnetLineSegment : DefaultParticleSystemMagnet
	{
		/// <summary>
		/// The position of the first End Point
		/// </summary>
		public Vector3 EndPoint1 { get; set; }

		/// <summary>
		/// The position of the second End Point
		/// </summary>
		public Vector3 EndPoint2 { get; set; }

		/// <summary>
		/// Explicit Constructor
		/// </summary>
		/// <param name="sEndPoint1Position">The 3D position of the first End Point of the Line Segment Magnet</param>
		/// <param name="sEndPoint2Position">The 3D position of the second End Point of the Line Segment Magnet</param>
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
		public MagnetLineSegment(Vector3 sEndPoint1Position, Vector3 sEndPoint2Position, MagnetModes eMode, DistanceFunctions eDistanceFunction,
									float fMinDistance, float fMaxDistance, float fMaxForce, int iType)
			: base(eMode, eDistanceFunction, fMinDistance, fMaxDistance, fMaxForce, iType)
		{
			meMagnetType = MagnetTypes.LineSegmentMagnet;
			EndPoint1 = sEndPoint1Position;
			EndPoint2 = sEndPoint2Position;
		}

		/// <summary>
		/// Copy Constructor
		/// </summary>
		/// <param name="cMagnetToCopy">The Line Segment Magnet to copy from</param>
		public MagnetLineSegment(MagnetLineSegment cMagnetToCopy)
			: base(MagnetModes.Attract, DistanceFunctions.Constant, 0, 0, 0, 0)
		{
			CopyFrom(cMagnetToCopy);
		}

		/// <summary>
		/// Copies the given Line Segment Magnet's data into this Line Segment Magnet's data
		/// </summary>
		/// <param name="cMagnetToCopy">The Line Magnet to copy from</param>
		public void CopyFrom(MagnetLineSegment cMagnetToCopy)
		{
			base.CopyFrom(cMagnetToCopy);
			EndPoint1 = cMagnetToCopy.EndPoint1;
			EndPoint2 = cMagnetToCopy.EndPoint2;
		}
	}
}
