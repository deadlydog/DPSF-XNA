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
	public class Position2DWithPreviousPosition : Position2D
	{
		/// <summary>
		/// The object's last position (before Update() was called).
		/// </summary>
		public Vector2 PreviousPosition = Vector2.Zero;

		/// <summary>
		/// Set this to False to not have the Update() function update the PreviousPosition value.
		/// Instead it will be up to an external object to update the PreviousPosition property each frame.
		/// <para>Default value is True.</para>
		/// </summary>
		public bool UpdatePreviousPositionAutomatically { get; set; }

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public Position2DWithPreviousPosition() { UpdatePreviousPositionAutomatically = true; }

		/// <summary>
		/// Copy Constructor.
		/// </summary>
		/// <param name="positionToCopy">The Position2DWithPreviousPosition object to copy.</param>
		public Position2DWithPreviousPosition(Position2DWithPreviousPosition positionToCopy)
		{
			CopyFrom(positionToCopy);
		}

		/// <summary>
		/// Copy Constructor.
		/// </summary>
		/// <param name="positionToCopy">The Position2D object to copy.</param>
		public Position2DWithPreviousPosition(Position2D positionToCopy)
		{
			CopyFrom(positionToCopy);
		}

		/// <summary>
		/// Copy the given Position2DWithPreviousPosition object's data into this objects data.
		/// </summary>
		/// <param name="positionToCopy">The Position2DWithPreviousPosition to copy from.</param>
		public void CopyFrom(Position2DWithPreviousPosition positionToCopy)
		{
			Position = positionToCopy.Position;
			Velocity = positionToCopy.Velocity;
			Acceleration = positionToCopy.Acceleration;
			PreviousPosition = positionToCopy.PreviousPosition;
			UpdatePreviousPositionAutomatically = positionToCopy.UpdatePreviousPositionAutomatically;
		}

		/// <summary>
		/// Copy the given Position2D object's data into this objects data.
		/// </summary>
		/// <param name="positionToCopy">The Position2D to copy from.</param>
		public override void CopyFrom(Position2D positionToCopy)
		{
			base.CopyFrom(positionToCopy);
			PreviousPosition = Position;
			UpdatePreviousPositionAutomatically = true;
		}

		/// <summary>
		/// Update the Position and Velocity according to the Acceleration.
		/// </summary>
		/// <param name="elapsedTimeInSeconds">The time Elapsed in Seconds since the last Update.</param>
		public override void Update(float elapsedTimeInSeconds)
		{
			// Save the current position before updating it.
			if (UpdatePreviousPositionAutomatically)
				PreviousPosition = Position;

			base.Update(elapsedTimeInSeconds);
		}
	}
}