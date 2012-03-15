#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

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
			/// Attract Particles to the Magnet
			/// </summary>
			Attract = 0,

			/// <summary>
			/// Repel Particles from the Magnet
			/// </summary>
			Repel = 1,

			/// <summary>
			/// Have some other custom effect on the Particles
			/// </summary>
			Other = 2
		}

		/// <summary>
		/// How much the Magnet should affect the Particles based on the Magnet's
		/// Max Distance and Strength
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
			/// the Particle is at half of the Max Distance from the Manget 1/8 of the Max Force will be 
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
		/// the Magnet based on how far away from the Magnet it is
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
		/// Particle is further away from the Magnet tan this distance, the Manget will not
		/// affect the Particle.
		/// </summary>
		public float MaxDistance = 100;

		/// <summary>
		/// The Max Force that the Magnet is able to exert on a Particle
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
		/// Particle is further away from the Magnet tan this distance, the Manget will not affect the Particle.</param>
		/// <param name="fMaxForce">The Max Force that the Magnet is able to exert on a Particle</param>
		/// <param name="iType">The Type of Magnet this is. This may be used in conjunction with the "Other" Magnet
		/// Mode to distinguish which type of custom user effect the Magnet should have on the Particles.</param>
		public DefaultParticleSystemMagnet(MagnetModes eMode, DistanceFunctions eDistanceFunction,
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
		public DefaultParticleSystemMagnet(DefaultParticleSystemMagnet cMagnetToCopy)
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
		/// Particle is further away from the Magnet tan this distance, the Manget will not affect the Particle.</param>
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
		/// Particle is further away from the Magnet tan this distance, the Manget will not affect the Particle.</param>
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

	/// <summary>
	/// Magnet that attracts particles to/from an infinite line in 3D space
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class MagnetLine : DefaultParticleSystemMagnet
	{
		/// <summary>
		/// A 3D point that the Line passes through
		/// </summary>
		public Vector3 PositionOnLine { get; set; }

		/// <summary>
		/// The Direction that the Line points in
		/// </summary>
		private Vector3 msDirection = Vector3.Forward;

		/// <summary>
		/// The direction that the Line points in. This direction, along with the opposite (i.e. negative) of 
		/// this direction form the line, since a line has infinite length. This value is 
		/// automatically normalized when it is set.
		/// </summary>
		public Vector3 Direction
		{
			get { return msDirection; }

			// Make sure the direction is normalized
			set
			{
				msDirection = value;
				msDirection.Normalize();
			}
		}

		/// <summary>
		/// Explicit Constructor
		/// </summary>
		/// <param name="sPositionOnLine">A 3D Position that the Line Magnet passes through</param>
		/// <param name="sDirection">The Direction that the Line points in</param>
		/// <param name="eMode">The Mode that the Magnet should be in</param>
		/// <param name="eDistanceFunction">The Function to use to determine how much a Particle should be affected by 
		/// the Magnet based on how far away from the Magnet it is</param>
		/// <param name="fMinDistance">The Min Distance that the Magnet should be able to affect Particles at. If the
		/// Particle is closer to the Magnet than this distance, the Magnet will not affect the Particle.</param>
		/// <param name="fMaxDistance">The Max Distance that the Magnet should be able to affect Particles at. If the
		/// Particle is further away from the Magnet tan this distance, the Manget will not affect the Particle.</param>
		/// <param name="fMaxForce">The Max Force that the Magnet is able to exert on a Particle</param>
		/// <param name="iType">The Type of Magnet this is. This may be used in conjunction with the "Other" Magnet
		/// Mode to distinguish which type of custom user effect the Magnet should have on the Particles.</param>
		public MagnetLine(Vector3 sPositionOnLine, Vector3 sDirection, MagnetModes eMode, DistanceFunctions eDistanceFunction,
							float fMinDistance, float fMaxDistance, float fMaxForce, int iType)
			: base(eMode, eDistanceFunction, fMinDistance, fMaxDistance, fMaxForce, iType)
		{
			meMagnetType = MagnetTypes.LineMagnet;
			PositionOnLine = sPositionOnLine;
			Direction = sDirection;
		}

		/// <summary>
		/// Copy Constructor
		/// </summary>
		/// <param name="cMagnetToCopy">The Line Magnet to copy from</param>
		public MagnetLine(MagnetLine cMagnetToCopy)
			: base(MagnetModes.Attract, DistanceFunctions.Constant, 0, 0, 0, 0)
		{
			CopyFrom(cMagnetToCopy);
		}

		/// <summary>
		/// Copies the given Line Magnet's data into this Line Magnet's data
		/// </summary>
		/// <param name="cMagnetToCopy">The Line Magnet to copy from</param>
		public void CopyFrom(MagnetLine cMagnetToCopy)
		{
			base.CopyFrom(cMagnetToCopy);
			PositionOnLine = cMagnetToCopy.PositionOnLine;
			Direction = cMagnetToCopy.Direction;
		}

		/// <summary>
		/// Sets the Direction of the Line by specifying 2 points in 3D space that are on the Line.
		/// <para>NOTE: The 2 points cannot be the same.</para>
		/// </summary>
		/// <param name="sFirstPointOnTheLine">The first point that falls on the Line</param>
		/// <param name="sSecondPointOnTheLine">The second point that falls on the Line</param>
		public void SetDirection(Vector3 sFirstPointOnTheLine, Vector3 sSecondPointOnTheLine)
		{
			// If valid points were specified
			if (sFirstPointOnTheLine != sSecondPointOnTheLine && sFirstPointOnTheLine != null && sSecondPointOnTheLine != null)
			{
				// Set the Direction of the Line
				Direction = sSecondPointOnTheLine - sFirstPointOnTheLine;
			}
		}
	}

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
		/// Particle is further away from the Magnet tan this distance, the Manget will not affect the Particle.</param>
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
		/// Particle is further away from the Magnet tan this distance, the Manget will not affect the Particle.</param>
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
