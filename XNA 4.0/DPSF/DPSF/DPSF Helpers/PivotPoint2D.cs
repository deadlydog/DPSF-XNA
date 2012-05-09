#region Using Statements
using System;
using Microsoft.Xna.Framework;

#endregion

namespace DPSF
{
    /// <summary>
    /// Class to hold and update an object's 2D Pivot Point (point to rotate around), Pivot Velocity, and 
    /// Pivot Acceleration. This class requires a Position2D object, and optionally a Orientation2D object,
    /// that should be affected by rotations around the Pivot Point.
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    public class PivotPoint2D
    {
        /// <summary>
        /// The 2D Pivot Point that the object should rotate around.
        /// <para>NOTE: This only has effect when Rotational Pivot Velocity / Acceleration are used.</para>
        /// </summary>
        public Vector2 PivotPoint = Vector2.Zero;

        /// <summary>
        /// The object's Rotational Velocity around the Pivot Point (Position change).
        /// <para>NOTE: Rotations are specified in radians.</para>
        /// </summary>
        public float PivotRotationalVelocity = 0.0f;

        /// <summary>
        /// The object's Rotational Acceleration around the Pivot Point (Position change).
        /// <para>NOTE: Rotations are specified in radians.</para>
        /// </summary>
        public float PivotRotationalAcceleration = 0.0f;

        // The Position and Orientation objects that should be affected
        private Position2D mcPositionData = null;
        private Orientation2D mcOrientationData = null;

        // If the object's Orientation should be Rotated Too or not
        private bool mbRotateOrientationToo = true;

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="cPivotPointToCopy">The PivotPoint2D object to copy</param>
        public PivotPoint2D(PivotPoint2D cPivotPointToCopy)
        {
            CopyFrom(cPivotPointToCopy);
        }

        /// <summary>
        /// Copies the given PivotPoint2D object's data into this object's data
        /// </summary>
        /// <param name="cPivotPointToCopy"></param>
        public void CopyFrom(PivotPoint2D cPivotPointToCopy)
        {
            PivotPoint = cPivotPointToCopy.PivotPoint;
            PivotRotationalVelocity = cPivotPointToCopy.PivotRotationalVelocity;
            PivotRotationalAcceleration = cPivotPointToCopy.PivotRotationalAcceleration;
            mbRotateOrientationToo = cPivotPointToCopy.RotateOrientationToo;

            mcPositionData = new Position2D(cPivotPointToCopy.PositionData);
            mcOrientationData = new Orientation2D(cPivotPointToCopy.OrientationData);
        }

        /// <summary>
        /// Explicit Constructor. Set the Position2D object that should be affected by rotations around
        /// this Pivot Point.
        /// </summary>
        /// <param name="cPosition">Handle to the Position2D object to update</param>
        public PivotPoint2D(Position2D cPosition)
        {
            // Save a handle to the Position that this should update
            mcPositionData = cPosition;

            // Set the handle to the Orientation to null so no Orientation operations are performed
            mcOrientationData = null;
        }

        /// <summary>
        /// Explicit Constructor. Set the Position2D and Orientation2D objects that should be affected by 
        /// rotational around this Pivot Point.
        /// </summary>
        /// <param name="cPosition">Handle to the Position2D object to update</param>
        /// <param name="cOrientation">Handle to the Orienetation2D object to update</param>
        public PivotPoint2D(Position2D cPosition, Orientation2D cOrientation)
        {
            // Save handles to the Position and Orientation that this should update
            mcPositionData = cPosition;
            mcOrientationData = cOrientation;
        }

        /// <summary>
        /// Get / Set the Position2D object that the Pivot Point should affect
        /// </summary>
        public Position2D PositionData
        {
            get { return mcPositionData; }
            set { mcPositionData = value; }
        }

        /// <summary>
        /// Get / Set the Orientation2D object that the Pivot Point should affect
        /// </summary>
        public Orientation2D OrientationData
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
        /// specified Pivot Point, changing its Position
        /// </summary>
        /// <param name="fRotation">The Rotation in radians to apply to the object</param>
        /// <param name="sPivotPoint">The Point to rotate the object around</param>
        public void RotatePositionAndOrientation(float fRotation, Vector2 sPivotPoint)
        {
            // If we have a handle to the Orientation
            if (mcOrientationData != null)
            {
                // Rotate the Orientation about it's center to change its Orientation
                mcOrientationData.Rotate(fRotation);
            }

            // Rotate the Position around the specified Pivot Point
            RotatePosition(fRotation, sPivotPoint);
        }

        /// <summary>
        /// Rotates the object about its center, changing its Orientation, as well as around the 
        /// specified 3D Pivot Point, changing its 2D Position.
        /// <para>NOTE: The Pivot Point's Z-value is ignored.</para>
        /// <para>NOTE: This function is provided for convenience when using 3D Vectors in 2D coordinate systems.</para>
        /// </summary>
        /// <param name="fRotation">The Rotation in radians to apply to the object</param>
        /// <param name="sPivotPoint">The Point to rotate the object around. 
        /// NOTE: The Z-value is ignored, since this is a 2D rotation.</param>
        public void RotatePositionAndOrientationVector3(float fRotation, Vector3 sPivotPoint)
        {
            // Call the 2D function to perform the operations
            RotatePositionAndOrientation(fRotation, new Vector2(sPivotPoint.X, sPivotPoint.Y));
        }

        /// <summary>
        /// Rotates the object around the specified Pivot Point, changing its Position, without 
        /// changing its Orientation.
        /// </summary>
        /// <param name="fRotation">The Rotation in radians to apply to the object</param>
        /// <param name="sPivotPoint">The Point to rotate the object around</param>
        public void RotatePosition(float fRotation, Vector2 sPivotPoint)
        {
            // Rotate the object around the Pivot Point by the given Rotation angle
            mcPositionData.Position = PivotPoint2D.RotatePosition(fRotation, sPivotPoint, mcPositionData.Position);
        }

        /// <summary>
        /// Rotates the object around the specified Pivot Point, changing its Position, without 
        /// changing its Orientation.
        /// <para>NOTE: The Pivot Point's Z-value is ignored.</para>
        /// <para>NOTE: This function is provided for convenience when using 3D Vectors in 2D coordinate systems.</para>
        /// </summary>
        /// <param name="fRotation">The Rotation in radians to apply to the object</param>
        /// <param name="sPivotPoint">The Point to rotate the object around.
        /// NOTE: The Z-value is ignored, since this is a 2D rotation.</param>
        public void RotatePositionVector3(float fRotation, Vector3 sPivotPoint)
        {
            // Call the 2D function to perform the operations
            RotatePosition(fRotation, new Vector2(sPivotPoint.X, sPivotPoint.Y));
        }

        /// <summary>
        /// Update the Position and Orientation according to the Pivot Rotational Velocity / Acceleration
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">The time Elapsed in Seconds since the last Update</param>
        public void Update(float fElapsedTimeInSeconds)
        {
            // If Pivot Rotational Acceleration is being used
            if (PivotRotationalAcceleration != 0.0f)
            {
                // Update the Pivot Rotational Velocity according to the Pivot Rotational Acceleration
                PivotRotationalVelocity += (PivotRotationalAcceleration * fElapsedTimeInSeconds);
            }

            // If Pivot Rotational Velocity is being used
            if (PivotRotationalVelocity != 0.0f)
            {
                // Get the rotation needed to Rotate the Position around the specified Pivot Point
                float fRotation = PivotRotationalVelocity * fElapsedTimeInSeconds;

                // If the Orientation should be updated as well
                if (mbRotateOrientationToo)
                {
                    RotatePositionAndOrientation(fRotation, PivotPoint);
                }
                // Else Rotate the object around the Pivot Point without changing the object's Orientation
                else
                {
                    RotatePosition(fRotation, PivotPoint);
                }
            }
        }

        #region Static Class Functions

        /// <summary>
        /// Rotates the given Position and Orientation around the Pivot Point, changing the Position and Orientation
        /// </summary>
        /// <param name="fRotation">The Rotation in radians to apply to the object</param>
        /// <param name="sPivotPoint">The Point to rotate the object around</param>
        /// <param name="srPosition">The Position of the object to be modified</param>
        /// <param name="frOrientation">The Orientation (rotation) of the object to be modified</param>
        public static void RotatePositionAndOrientation(float fRotation, Vector2 sPivotPoint, ref Vector2 srPosition, ref float frOrientation)
        {
            // Rotate the Orientation about it's center to change its Orientation
            frOrientation = Orientation2D.Rotate(fRotation, frOrientation);

            // Rotate the Position around the specified Pivot Point
            srPosition = PivotPoint2D.RotatePosition(fRotation, sPivotPoint, srPosition);
        }

        /// <summary>
        /// Rotates the given Position and Orientation around the Pivot Point, changing the Position and Orientation.
        /// <para>NOTE: The Pivot Point and Position's Z-values are ignored.</para>
        /// <para>NOTE: This function is provided for convenience when using 3D Vectors in 2D coordinate systems.</para>
        /// </summary>
        /// <param name="fRotation">The Rotation in radians to apply to the object</param>
        /// <param name="sPivotPoint">The Point to rotate the object around.
        /// NOTE: The Z-value is ignored, since this is a 2D rotation.</param>
        /// <param name="srPosition">The Position of the object to be modified.
        /// NOTE: The Z-value is ignored and will not be changed, since this is a 2D rotation.</param>
        /// <param name="frOrientation">The Orientation (rotation) of the object to be modified</param>
        public static void RotatePositionAndOrientationVector3(float fRotation, Vector3 sPivotPoint, ref Vector3 srPosition, ref float frOrientation)
        {
            // Set up the Position vector to pass by reference
            Vector2 sPosition2D = new Vector2(srPosition.X, srPosition.Y);

            // Call the 2D function to perform the operations
            PivotPoint2D.RotatePositionAndOrientation(fRotation, new Vector2(sPivotPoint.X, sPivotPoint.Y),
                                                      ref sPosition2D, ref frOrientation);

            // Set the Position's 2D position to the new Rotated Position (leaving the Z-value unchanged)
            srPosition.X = sPosition2D.X;
            srPosition.Y = sPosition2D.Y;
        }

        /// <summary>
        /// Returns the new Position after Rotating the given Position around the specified Pivot Point
        /// </summary>
        /// <param name="fRotation">The Rotation in radians to rotate around the Pivot Point by</param>
        /// <param name="sPivotPoint">The Point to Rotate around</param>
        /// <param name="sPosition">The current Position of the object</param>
        /// <returns>Returns the new Position after Rotating the given Position around the specified Pivot Point</returns>
        public static Vector2 RotatePosition(float fRotation, Vector2 sPivotPoint, Vector2 sPosition)
        {
            // If the object should rotate around a point other than it's center
            if (sPivotPoint != sPosition)
            {
                // We can calculate the object's new Position after rotating around the
                // Pivot Point by using right-angle triangle trigonometry

                // Get the object's Position and Rotation Angle relative to the Pivot Point
                Vector2 sRelativePosition = sPosition - sPivotPoint;
                float fRelativeAngle = (float)Math.Atan(sRelativePosition.Y / sRelativePosition.X);

                // Calculate the new Relative Rotation Angle after applying the Rotation
                float fNewRelativeAngle = fRelativeAngle + fRotation;

                // Calculate the object's new Relative Position after the Rotation
                float fHypotenuse = sRelativePosition.Length();
                if (sRelativePosition.X < 0) { fHypotenuse *= -1; }
                float fRelativeX = (float)Math.Cos(fNewRelativeAngle) * fHypotenuse;
                float fRelativeY = (float)Math.Sin(fNewRelativeAngle) * fHypotenuse;

                // Set the new Position after being Rotated around the specified Pivot Point
                sPosition = sPivotPoint + new Vector2(fRelativeX, fRelativeY);
            }

            // Return the Rotated Position
            return sPosition;
        }

        /// <summary>
        /// Returns the new Position after Rotating the given Position around the specified Pivot Point.
        /// <para>NOTE: The Pivot Point and Position's Z-values are ignored.</para>
        /// <para>This function is provided for convenience when using 3D Vectors in 2D coordinate systems.</para>
        /// </summary>
        /// <param name="fRotation">The Rotation in radians to rotate around the Pivot Point by</param>
        /// <param name="sPivotPoint">The Point to Rotate around.
        /// NOTE: The Z-value is ignored, since this is a 2D rotation.</param>
        /// <param name="sPosition">The current Position of the object.
        /// NOTE: The Z-value is ignored and will not be changed, since this is a 2D rotation.</param>
        /// <returns>Returns the new Position after Rotating the given Position around the specified Pivot Point.</returns>
        public static Vector3 RotatePositionVector3(float fRotation, Vector3 sPivotPoint, Vector3 sPosition)
        {
            // Call the 2D function to perform the operations
            Vector2 cNewPosition = PivotPoint2D.RotatePosition(fRotation, new Vector2(sPivotPoint.X, sPivotPoint.Y),
                                                               new Vector2(sPosition.X, sPosition.Y));

            // Return the Rotated Position (with the original Z-value)
            return new Vector3(cNewPosition.X, cNewPosition.Y, sPosition.Z);
        }
        #endregion
    }
}
