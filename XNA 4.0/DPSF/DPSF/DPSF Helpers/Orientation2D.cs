#region Using Statements
using System;

#endregion

namespace DPSF
{
    /// <summary>
    /// Class to hold and update an object's 2D Orientation, Rotational Velocity, and Rotational Acceleration
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    public class Orientation2D
    {
        /// <summary>
        /// The object's Rotation (in radians) (i.e. How much it is currently rotated)
        /// </summary>
        public float Orientation;

        /// <summary>
        /// The object's Rotational Velocity (in radians)
        /// </summary>
        public float RotationalVelocity;

        /// <summary>
        /// The object's Rotational Acceleration (in radians)
        /// </summary>
        public float RotationalAcceleration;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Orientation2D() { }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="cOrienationToCopy">The Orientation2D object to copy</param>
        public Orientation2D(Orientation2D cOrienationToCopy)
        {
            CopyFrom(cOrienationToCopy);
        }

        /// <summary>
        /// Copies the given Orientation2D object's data into this object's data
        /// </summary>
        /// <param name="cOrientationToCopy">The Orientation2D object to copy from</param>
        public virtual void CopyFrom(Orientation2D cOrientationToCopy)
        {
            Orientation = cOrientationToCopy.Orientation;
            RotationalVelocity = cOrientationToCopy.RotationalVelocity;
            RotationalAcceleration = cOrientationToCopy.RotationalAcceleration;
        }

        /// <summary>
        /// Applies the given Rotation to the object's Orientation
        /// </summary>
        /// <param name="fRotation">The Rotation in radians that should be applied to the object</param>
        public void Rotate(float fRotation)
        {
            Orientation = Orientation2D.Rotate(fRotation, Orientation);
        }

        /// <summary>
        /// Update the Orientation and Rotational Velocity according to the Rotational Acceleration
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">The time Elapsed in Seconds since the last Update</param>
        public virtual void Update(float fElapsedTimeInSeconds)
        {
            // Update the Orientation and Rotational Velocity according to how much Time has Elapsed
            RotationalVelocity += RotationalAcceleration * fElapsedTimeInSeconds;
            Orientation += RotationalVelocity * fElapsedTimeInSeconds;
        }

        #region Static Class Functions

        /// <summary>
        /// Returns the new Orientation after applying the given Rotation
        /// </summary>
        /// <param name="fRotationToApply">The Rotation in radians to apply to the Current Orientation</param>
        /// <param name="fCurrentOrientation">The object's Current Orientation in radians</param>
        /// <returns>Returns the new Orientation after applying the given Rotation</returns>
        public static float Rotate(float fRotationToApply, float fCurrentOrientation)
        {
            // Rotate the object about it's center to change its Orientation
            return (fCurrentOrientation + fRotationToApply);
        }

        /// <summary>
        /// Returns the Rotation needed to rotate the object from the Current Rotation to
        /// the Desired Rotation
        /// </summary>
        /// <param name="fCurrentRotation">The object's Current Rotation in radians</param>
        /// <param name="fDesiredRotation">The object's Desired Rotation in radians</param>
        /// <returns>Returns the Rotation needed to rotate the object from the Current Rotation to
        /// the Desired Rotation</returns>
        public static float GetRotationTo(float fCurrentRotation, float fDesiredRotation)
        {
            return (fDesiredRotation - fCurrentRotation);
        }
        #endregion
    }
}
