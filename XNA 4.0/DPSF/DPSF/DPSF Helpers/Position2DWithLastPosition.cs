using System;
using Microsoft.Xna.Framework;

namespace DPSF
{
	/// <summary>
	/// Inherits from the Position2D class and adds functionality to remember the object's last position.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class Position2DWithLastPosition : Position2D
	{
		/// <summary>
		/// The object's last position (before Update() was called).
		/// </summary>
		public Vector2 LastPosition = Vector2.Zero;

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public Position2DWithLastPosition() { }

		/// <summary>
		/// Copy Constructor.
		/// </summary>
		/// <param name="positionToCopy">The Position2DWithLastPosition object to copy.</param>
		public Position2DWithLastPosition(Position2DWithLastPosition positionToCopy)
		{
			CopyFrom(positionToCopy);
		}

		/// <summary>
		/// Copy Constructor.
		/// </summary>
		/// <param name="positionToCopy">The Position2D object to copy.</param>
		public Position2DWithLastPosition(Position2D positionToCopy)
		{
			CopyFrom(positionToCopy);
		}

		/// <summary>
		/// Copy the given Position2DWithLastPosition object's data into this objects data.
		/// </summary>
		/// <param name="positionToCopy">The Position2DWithLastPosition to copy from.</param>
		public void CopyFrom(Position2DWithLastPosition positionToCopy)
		{
			Position = positionToCopy.Position;
			Velocity = positionToCopy.Velocity;
			Acceleration = positionToCopy.Acceleration;
			LastPosition = positionToCopy.LastPosition;
		}

		/// <summary>
		/// Copy the given Position2D object's data into this objects data.
		/// </summary>
		/// <param name="positionToCopy">The Position2D to copy from.</param>
		public void CopyFrom(Position2D positionToCopy)
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