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
	public class Orientation3DWithLastOrientation : Orientation3D
	{
		/// <summary>
		/// The object's last orientation (before Update() was called).
		/// </summary>
		public Quaternion LastOrientation = Quaternion.Identity;

		/// <summary>
        /// Default Constructor.
        /// </summary>
        public Orientation3DWithLastOrientation() { }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="orienationToCopy">The Orientation3DWithLastOrientation object to copy.</param>
        public Orientation3DWithLastOrientation(Orientation3DWithLastOrientation orienationToCopy)
        {
            CopyFrom(orienationToCopy);
        }

		/// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="orienationToCopy">The Orienation3D object to copy.</param>
		public Orientation3DWithLastOrientation(Orientation3D orienationToCopy)
        {
            CopyFrom(orienationToCopy);
        }

        /// <summary>
		/// Copies the given Orientation3DWithLastOrientation object's data into this object's data.
        /// </summary>
        /// <param name="orientationToCopy">The Orientation3D object to copy from.</param>
		public void CopyFrom(Orientation3DWithLastOrientation orientationToCopy)
        {
            Orientation = orientationToCopy.Orientation;
            RotationalVelocity = orientationToCopy.RotationalVelocity;
            RotationalAcceleration = orientationToCopy.RotationalAcceleration;
			LastOrientation = orientationToCopy.LastOrientation;
        }

        /// <summary>
        /// Copies the given Orientation3D object's data into this object's data.
        /// </summary>
        /// <param name="orientationToCopy">The Orientation3D object to copy from.</param>
        public void CopyFrom(Orientation3D orientationToCopy)
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