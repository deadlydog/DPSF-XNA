using System;
using Microsoft.Xna.Framework;

namespace DPSF
{
	/// <summary>
	/// Inherits from the Orientation3D class and adds functionality to remember the object's last orientation.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class Orientation3DWithPreviousOrientation : Orientation3D
	{
		/// <summary>
		/// The object's last orientation (before Update() was called).
		/// </summary>
		public Quaternion PreviousOrientation = Quaternion.Identity;

		/// <summary>
		/// Set this to False to not have the Update() function update the PreviousOrientation value.
		/// Instead it will be up to an external object to update the PreviousOrientation property each frame.
		/// <para>Default value is True.</para>
		/// </summary>
		public bool UpdatePreviousOrientationAutomatically { get; set; }

		/// <summary>
        /// Default Constructor.
        /// </summary>
		public Orientation3DWithPreviousOrientation() { UpdatePreviousOrientationAutomatically = true; }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="orienationToCopy">The Orientation3DWithPreviousOrientation object to copy.</param>
        public Orientation3DWithPreviousOrientation(Orientation3DWithPreviousOrientation orienationToCopy)
        {
            CopyFrom(orienationToCopy);
        }

		/// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="orienationToCopy">The Orienation3D object to copy.</param>
		public Orientation3DWithPreviousOrientation(Orientation3D orienationToCopy)
        {
            CopyFrom(orienationToCopy);
        }

        /// <summary>
		/// Copies the given Orientation3DWithPreviousOrientation object's data into this object's data.
        /// </summary>
        /// <param name="orientationToCopy">The Orientation3D object to copy from.</param>
		public void CopyFrom(Orientation3DWithPreviousOrientation orientationToCopy)
        {
            Orientation = orientationToCopy.Orientation;
            RotationalVelocity = orientationToCopy.RotationalVelocity;
            RotationalAcceleration = orientationToCopy.RotationalAcceleration;
			PreviousOrientation = orientationToCopy.PreviousOrientation;
			UpdatePreviousOrientationAutomatically = orientationToCopy.UpdatePreviousOrientationAutomatically;
        }

        /// <summary>
        /// Copies the given Orientation3D object's data into this object's data.
        /// </summary>
        /// <param name="orientationToCopy">The Orientation3D object to copy from.</param>
        public override void CopyFrom(Orientation3D orientationToCopy)
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