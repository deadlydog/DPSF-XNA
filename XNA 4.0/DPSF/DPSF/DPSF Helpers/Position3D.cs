#region Using Statements
using System;
using Microsoft.Xna.Framework;

#endregion

namespace DPSF
{
    /// <summary>
    /// Class used to hold and update an object's 3D Position, Velocity, and Acceleration.
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    public class Position3D
    {
        /// <summary>
        /// The object's Position
        /// </summary>
        public Vector3 Position = Vector3.Zero;

        /// <summary>
        /// The object's Velocity
        /// </summary>
        public Vector3 Velocity = Vector3.Zero;

        /// <summary>
        /// The object's Acceleration
        /// </summary>
        public Vector3 Acceleration = Vector3.Zero;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Position3D() { }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="cPositionToCopy">The Position3D object to copy</param>
        public Position3D(Position3D cPositionToCopy)
        {
            CopyFrom(cPositionToCopy);
        }

        /// <summary>
        /// Copy the given Position3D object's data into this objects data
        /// </summary>
        /// <param name="cPositionToCopy">The Position3D to copy from</param>
        public virtual void CopyFrom(Position3D cPositionToCopy)
        {
            Position = cPositionToCopy.Position;
            Velocity = cPositionToCopy.Velocity;
            Acceleration = cPositionToCopy.Acceleration;
        }

        /// <summary>
        /// Update the Position and Velocity according to the Acceleration
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">The time Elapsed in Seconds since the last Update</param>
        public virtual void Update(float fElapsedTimeInSeconds)
        {
            // Update the Velocity and Position according to how much Time has Elapsed
            Velocity += Acceleration * fElapsedTimeInSeconds;
            Position += Velocity * fElapsedTimeInSeconds;
        }
    }
}
