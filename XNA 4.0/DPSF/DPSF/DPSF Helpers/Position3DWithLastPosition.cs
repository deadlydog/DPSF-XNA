using System;
using Microsoft.Xna.Framework;

namespace DPSF
{
	/// <summary>
	/// Inherits from the Position3D class and adds functionality to remember the object's last position.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class Position3DWithLastPosition : Position3D
	{
		/// <summary>
		/// The object's last position (before Update() was called).
		/// </summary>
		public Vector3 LastPosition = Vector3.Zero;

		/// <summary>
        /// Default Constructor.
        /// </summary>
        public Position3DWithLastPosition() { }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
		/// <param name="positionToCopy">The Position3DWithLastPosition object to copy.</param>
		public Position3DWithLastPosition(Position3DWithLastPosition positionToCopy)
        {
            CopyFrom(positionToCopy);
        }

		/// <summary>
		/// Copy Constructor.
		/// </summary>
		/// <param name="positionToCopy">The Position3D object to copy.</param>
		public Position3DWithLastPosition(Position3D positionToCopy)
		{
			CopyFrom(positionToCopy);
		}

		/// <summary>
		/// Copy the given Position3DWithLastPosition object's data into this objects data.
		/// </summary>
		/// <param name="positionToCopy">The Position3DWithLastPosition to copy from.</param>
		public void CopyFrom(Position3DWithLastPosition positionToCopy)
		{
			Position = positionToCopy.Position;
			Velocity = positionToCopy.Velocity;
			Acceleration = positionToCopy.Acceleration;
			LastPosition = positionToCopy.LastPosition;
		}

        /// <summary>
        /// Copy the given Position3D object's data into this objects data.
        /// </summary>
        /// <param name="positionToCopy">The Position3D to copy from.</param>
        public void CopyFrom(Position3D positionToCopy)
        {
            Position = positionToCopy.Position;
            Velocity = positionToCopy.Velocity;
            Acceleration = positionToCopy.Acceleration;
			LastPosition = Position;
        }

        /// <summary>
        /// Update the Position and Velocity according to the Acceleration.
        /// </summary>
        /// <param name="elapsedTimeInSeconds">The time Elapsed in Seconds since the last Update.</param>
        public override void Update(float elapsedTimeInSeconds)
        {
			// Save the current position before updating it.
			LastPosition = Position;
			base.Update(elapsedTimeInSeconds);
        }
	}
}