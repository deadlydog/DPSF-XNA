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
	public class Orientation2DWithPreviousOrientation : Orientation2D
	{
		/// <summary>
		/// The object's last orientation (before Update() was called).
		/// </summary>
		public float PreviousOrientation;

		/// <summary>
		/// Set this to False to not have the Update() function update the PreviousOrientation value.
		/// Instead it will be up to an external object to update the PreviousOrientation property each frame.
		/// <para>Default value is True.</para>
		/// </summary>
		public bool UpdatePreviousOrientationAutomatically { get; set; }

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public Orientation2DWithPreviousOrientation() { UpdatePreviousOrientationAutomatically = true; }

		/// <summary>
		/// Copy Constructor.
		/// </summary>
		/// <param name="orienationToCopy">The Orientation2DWithPreviousOrientation object to copy.</param>
		public Orientation2DWithPreviousOrientation(Orientation2DWithPreviousOrientation orienationToCopy)
		{
			CopyFrom(orienationToCopy);
		}

		/// <summary>
		/// Copy Constructor.
		/// </summary>
		/// <param name="orienationToCopy">The Orientation2D object to copy.</param>
		public Orientation2DWithPreviousOrientation(Orientation2D orienationToCopy)
		{
			CopyFrom(orienationToCopy);
		}

		/// <summary>
		/// Copies the given Orientation2DWithPreviousOrientation object's data into this object's data.
		/// </summary>
		/// <param name="orientationToCopy">The Orientation2DWithPreviousOrientation object to copy from.</param>
		public void CopyFrom(Orientation2DWithPreviousOrientation orientationToCopy)
		{
			Orientation = orientationToCopy.Orientation;
			RotationalVelocity = orientationToCopy.RotationalVelocity;
			RotationalAcceleration = orientationToCopy.RotationalAcceleration;
			PreviousOrientation = orientationToCopy.PreviousOrientation;
			UpdatePreviousOrientationAutomatically = orientationToCopy.UpdatePreviousOrientationAutomatically;
		}

		/// <summary>
		/// Copies the given Orientation2D object's data into this object's data.
		/// </summary>
		/// <param name="orientationToCopy">The Orientation2D object to copy from.</param>
		public override void CopyFrom(Orientation2D orientationToCopy)
		{
			base.CopyFrom(orientationToCopy);
			PreviousOrientation = Orientation;
			UpdatePreviousOrientationAutomatically = true;
		}

		/// <summary>
		/// Update the Position and Velocity according to the Acceleration, as well as the Orientation
		/// according to the Rotational Velocity and Rotational Acceleration.
		/// </summary>
		/// <param name="elapsedTimeInSeconds">The time Elapsed in Seconds since the last Update.</param>
		public override void Update(float elapsedTimeInSeconds)
		{
			// Save the current orientation before updating it.
			if (UpdatePreviousOrientationAutomatically)
				PreviousOrientation = Orientation;

			base.Update(elapsedTimeInSeconds);
		}
	}
}