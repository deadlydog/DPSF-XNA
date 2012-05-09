using System;
using Microsoft.Xna.Framework;

namespace DPSF
{
	/// <summary>
	/// Inherits from the Orientation2D class and adds functionality to remember the object's last orientation.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class Orientation2DWithLastOrientation : Orientation2D
	{
		/// <summary>
		/// The object's last orientation (before Update() was called).
		/// </summary>
		public float LastOrientation;

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public Orientation2DWithLastOrientation() { }

		/// <summary>
		/// Copy Constructor.
		/// </summary>
		/// <param name="orienationToCopy">The Orientation2DWithLastOrientation object to copy.</param>
		public Orientation2DWithLastOrientation(Orientation2DWithLastOrientation orienationToCopy)
		{
			CopyFrom(orienationToCopy);
		}

		/// <summary>
		/// Copy Constructor.
		/// </summary>
		/// <param name="orienationToCopy">The Orientation2D object to copy.</param>
		public Orientation2DWithLastOrientation(Orientation2D orienationToCopy)
		{
			CopyFrom(orienationToCopy);
		}

		/// <summary>
		/// Copies the given Orientation2DWithLastOrientation object's data into this object's data.
		/// </summary>
		/// <param name="orientationToCopy">The Orientation2DWithLastOrientation object to copy from.</param>
		public void CopyFrom(Orientation2DWithLastOrientation orientationToCopy)
		{
			Orientation = orientationToCopy.Orientation;
			RotationalVelocity = orientationToCopy.RotationalVelocity;
			RotationalAcceleration = orientationToCopy.RotationalAcceleration;
			LastOrientation = orientationToCopy.LastOrientation;
		}

		/// <summary>
		/// Copies the given Orientation2D object's data into this object's data.
		/// </summary>
		/// <param name="orientationToCopy">The Orientation2D object to copy from.</param>
		public void CopyFrom(Orientation2D orientationToCopy)
		{
			Orientation = orientationToCopy.Orientation;
			RotationalVelocity = orientationToCopy.RotationalVelocity;
			RotationalAcceleration = orientationToCopy.RotationalAcceleration;
			LastOrientation = Orientation;
		}

		/// <summary>
		/// Update the Position and Velocity according to the Acceleration, as well as the Orientation
		/// according to the Rotational Velocity and Rotational Acceleration.
		/// </summary>
		/// <param name="elapsedTimeInSeconds">The time Elapsed in Seconds since the last Update.</param>
		public override void Update(float elapsedTimeInSeconds)
		{
			LastOrientation = Orientation;
			base.Update(elapsedTimeInSeconds);
		}
	}
}