#region Using Statements
using System;
using Microsoft.Xna.Framework;

#endregion

namespace DPSF
{
    /// <summary>
    /// Class to hold and update an object's 3D Pivot Point (point to rotate around), Pivot Velocity, and 
    /// Pivot Acceleration. This class requires a Position3D object, and optionally a Orientation3D object, 
    /// that should be affected by rotations around the Pivot Point.
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    public class PivotPoint3D
    {
        /// <summary>
        /// The 3D Pivot Point that the object should rotate around.
        /// <para>NOTE: This only has effect when Rotational Pivot Velocity / Acceleration are used.</para>
        /// </summary>
        public Vector3 PivotPoint = Vector3.Zero;

        /// <summary>
        /// The object's Rotational Velocity around the Pivot Point (Position change).
        /// <para>NOTE: Rotations are specified by giving a 3D Vector, where the direction is the axis to 
        /// rotate around, and the vector length is the amount (angle in radians) to rotate.
        /// It can also be thought of as Vector(PitchVelocity, YawVelocity, RollVelocity).</para>
        /// </summary>
        public Vector3 PivotRotationalVelocity = Vector3.Zero;

        /// <summary>
        /// Get / Set the object's Rotational Acceleration around the Pivot Point (Position change).
        /// <para>NOTE: Rotations are specified by giving a 3D Vector, where the direction is the axis to 
        /// rotate around, and the vector length is the amount (angle in radians) to rotate.
        /// It can also be thought of as Vector(PitchAcceleration, YawAcceleration, RollAcceleration).</para>
        /// </summary>
        public Vector3 PivotRotationalAcceleration = Vector3.Zero;

        // The Position and Orientation objects that should be affected
        private Position3D mcPositionData = null;
        private Orientation3D mcOrientationData = null;

        // If the object's Orientation should be Rotated Too or not
        private bool mbRotateOrientationToo = true;

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="cPivotPointToCopy">The PivotPoint3D object to copy</param>
        public PivotPoint3D(PivotPoint3D cPivotPointToCopy)
        {
            CopyFrom(cPivotPointToCopy);
        }

        /// <summary>
        /// Copy the given PivotPoint3D object's data into this object's data
        /// </summary>
        /// <param name="cPivotPointToCopy">The PivotPoint3D object to copy from</param>
        public void CopyFrom(PivotPoint3D cPivotPointToCopy)
        {
            PivotPoint = cPivotPointToCopy.PivotPoint;
            PivotRotationalVelocity = cPivotPointToCopy.PivotRotationalVelocity;
            PivotRotationalAcceleration = cPivotPointToCopy.PivotRotationalAcceleration;
            mbRotateOrientationToo = cPivotPointToCopy.RotateOrientationToo;

            mcPositionData = new Position3D(cPivotPointToCopy.PositionData);
            mcOrientationData = new Orientation3D(cPivotPointToCopy.OrientationData);
        }

        /// <summary>
        /// Explicit Constructor. Set the Position3D object that should be affected by rotations around
        /// this Pivot Point.
        /// </summary>
        /// <param name="cPosition">Handle to the Position3D object to update</param>
        public PivotPoint3D(Position3D cPosition)
        {
            // Save a handle to the Position that this should update
            mcPositionData = cPosition;

            // Set the handle to the Orientation to null so no Orientation operations are performed
            mcOrientationData = null;
        }

        /// <summary>
        /// Explicit Constructor. Set the Position3D and Orientation3D objects that should be affected by 
        /// rotational around this Pivot Point.
        /// </summary>
        /// <param name="cPosition">Handle to the Position3D object to update</param>
        /// <param name="cOrientation">Handle to the Orienetation3D object to update</param>
        public PivotPoint3D(Position3D cPosition, Orientation3D cOrientation)
        {
            // Save handles to the Position and Orientation that this should update
            mcPositionData = cPosition;
            mcOrientationData = cOrientation;
        }

        /// <summary>
        /// Get / Set the Position3D object that the Pivot Point should affect
        /// </summary>
        public Position3D PositionData
        {
            get { return mcPositionData; }
            set { mcPositionData = value; }
        }

        /// <summary>
        /// Get / Set the Orientation3D object that the Pivot Point should affect
        /// </summary>
        public Orientation3D OrientationData
        {
            get { return mcOrientationData; }
            set { mcOrientationData = value; }
        }

        /// <summary>
        /// Specify if the Update() function should Rotate the object's Orientation too when it
        /// rotates the object around the Pivot Point
        /// </summary>
        public bool RotateOrientationToo
        {
            get { return mbRotateOrientationToo; }
            set { mbRotateOrientationToo = value; }
        }

        /// <summary>
        /// Rotates the object about its center, changing its Orientation, as well as around the 
        /// Pivot Point, changing its Position
        /// </summary>
        /// <param name="sRotationMatrix">The Rotation to apply to the object, rotating it 
        /// around the Pivot Point</param>
        public void RotatePositionAndOrientation(Matrix sRotationMatrix)
        {
            // If we have a handle to the Orientation
            if (mcOrientationData != null)
            {
                // Rotate the Orientation about it's center to change its Orientation
                mcOrientationData.Rotate(sRotationMatrix);
            }

            // Rotate the Position around the specified Pivot Point
            RotatePosition(sRotationMatrix);
        }

        /// <summary>
        /// Rotates the object around the Pivot Point, changing its Position, without 
        /// changing its Orientation
        /// </summary>
        /// <param name="sRotationMatrix">The Rotation to apply to the object, rotating it 
        /// around the Pivot Point</param>
        public void RotatePosition(Matrix sRotationMatrix)
        {
            // If the object should rotate around a point other than it's center
            if (PivotPoint != mcPositionData.Position)
            {
                // Rotate the Position around the Pivot Point
                mcPositionData.Position = PivotPoint3D.RotatePosition(sRotationMatrix, PivotPoint, mcPositionData.Position);
            }
        }

        /// <summary>
        /// Update the Position and Orientation according to the Pivot Rotational Velocity / Acceleration
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">The time Elapsed in Seconds since the last Update</param>
        public void Update(float fElapsedTimeInSeconds)
        {
            // If Pivot Rotational Acceleration is being used
            if (PivotRotationalAcceleration != Vector3.Zero)
            {
                // Update the Pivot Rotational Velocity according to the Pivot Rotational Acceleration
                PivotRotationalVelocity += (PivotRotationalAcceleration * fElapsedTimeInSeconds);
            }

            // If Pivot Rotational Velocity is being used
            if (PivotRotationalVelocity != Vector3.Zero)
            {
                // Get the rotation needed to Rotate the Position around the specified Pivot Point
                Vector3 sRotation = PivotRotationalVelocity * fElapsedTimeInSeconds;

                // If the Orientation should be updated as well
                if (mbRotateOrientationToo)
                {
                    RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(sRotation.Y, sRotation.X, sRotation.Z));
                }
                // Else Rotate the object around the Pivot Point without changing the object's Orientation
                else
                {
                    RotatePosition(Matrix.CreateFromYawPitchRoll(sRotation.Y, sRotation.X, sRotation.Z));
                }
            }
        }

        #region Static Class Functions

        /// <summary>
        /// Rotates the given Position and Orientation around the Pivot Point, changing the Position and Orientation
        /// </summary>
        /// <param name="sRotationMatrix">The Rotation to apply to the object</param>
        /// <param name="sPivotPoint">The Point to rotate the object around</param>
        /// <param name="srPosition">The Position of the object (to be modified)</param>
        /// <param name="srOrientation">The Orientation of the object (to be modified)</param>
        public static void RotatePositionAndOrientation(Matrix sRotationMatrix, Vector3 sPivotPoint, ref Vector3 srPosition, ref Quaternion srOrientation)
        {
            // Rotate the Orientation about it's center to change its Orientation
            srOrientation = Orientation3D.Rotate(sRotationMatrix, srOrientation);

            // Rotate the Position around the specified Pivot Point
            srPosition = PivotPoint3D.RotatePosition(sRotationMatrix, sPivotPoint, srPosition);
        }

        /// <summary>
        /// Returns the new Position after Rotating the given Position around the specified Pivot Point
        /// </summary>
        /// <param name="sRotationMatrix">The Rotation to apply to the Emitter</param>
        /// <param name="sPivotPoint">The Point to rotate the Emitter around</param>
        /// <param name="sPosition">The Position to be rotated around the Pivot Point</param>
        /// <returns>Returns the new Position after Rotating the given Position around the specified Pivot Point</returns>
        public static Vector3 RotatePosition(Matrix sRotationMatrix, Vector3 sPivotPoint, Vector3 sPosition)
        {
            // If the object should rotate around a point other than it's center
            if (sPivotPoint != sPosition)
            {
                // Rotate the Position around the specified Pivot Point
                sPosition = Vector3.Transform(sPosition - sPivotPoint, sRotationMatrix);
                sPosition += sPivotPoint;
            }

            // Return the new Position
            return sPosition;
        }
        #endregion
    }
}
