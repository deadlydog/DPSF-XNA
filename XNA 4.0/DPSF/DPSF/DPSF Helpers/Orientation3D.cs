#region Using Statements
using System;
using Microsoft.Xna.Framework;

#endregion

namespace DPSF
{
    /// <summary>
    /// Class to hold and update an object's 3D Orientation, Rotational Velocity, and Rotational Acceleration.
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    public class Orientation3D
    {
        /// <summary>
        /// The object's Orientation.
        /// </summary>
        public Quaternion Orientation = Quaternion.Identity;

        /// <summary>
        /// The object's Rotational Velocity around its center.
        /// <para>NOTE: Rotations are specified by giving a 3D Vector, where the direction is the axis to 
        /// rotate around, and the vector length is the amount (angle in radians) to rotate.
        /// It can also be thought of as Vector(PitchVelocity, YawVelocity, RollVelocity).</para>
        /// </summary>
        public Vector3 RotationalVelocity = Vector3.Zero;

        /// <summary>
        /// The object's Rotational Acceleration around its center.
        /// <para>NOTE: Rotations are specified by giving a 3D Vector, where the direction is the axis to 
        ///  rotate around, and the vector length is the amount (angle in radians) to rotate.
        ///  It can also be thought of as Vector(PitchAcceleration, YawAcceleration, RollAcceleration).</para>
        /// </summary>
        public Vector3 RotationalAcceleration = Vector3.Zero;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Orientation3D() { }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="cOrienationToCopy">The Orienation3D object to copy.</param>
        public Orientation3D(Orientation3D cOrienationToCopy)
        {
            CopyFrom(cOrienationToCopy);
        }

        /// <summary>
        /// Copies the given Orientation3D object's data into this object's data.
        /// </summary>
        /// <param name="cOrientationToCopy">The Orientation3D object to copy from.</param>
        public virtual void CopyFrom(Orientation3D cOrientationToCopy)
        {
            Orientation = cOrientationToCopy.Orientation;
            RotationalVelocity = cOrientationToCopy.RotationalVelocity;
            RotationalAcceleration = cOrientationToCopy.RotationalAcceleration;
        }

        /// <summary>
        /// Get / Set the Normal (i.e. Forward) direction of the object (i.e. which direction it is facing).
        /// </summary>
        public Vector3 Normal
        {
            get { return Orientation3D.GetNormalDirection(Orientation); }
            set { Orientation3D.SetNormalDirection(ref Orientation, value); }
        }

        /// <summary>
        /// Get / Set the Up direction of the object.
        /// </summary>
        public Vector3 Up
        {
            get { return Orientation3D.GetUpDirection(Orientation); }
            set { Orientation3D.SetUpDirection(ref Orientation, value); }
        }

        /// <summary>
        /// Get / Set the Right direction of the object.
        /// </summary>
        public Vector3 Right
        {
            get { return Orientation3D.GetRightDirection(Orientation); }
            set { Orientation3D.SetRightDirection(ref Orientation, value); }
        }

        /// <summary>
        /// Rotates the object about its center, changing its Orientation.
        /// </summary>
        /// <param name="sRotationMatrix">The Rotation to apply to the object.</param>
        public void Rotate(Matrix sRotationMatrix)
        {
            // Rotate the object about it's center to change its Orientation.
            Orientation = Orientation3D.Rotate(sRotationMatrix, Orientation);
        }

        /// <summary>
        /// Update the Position and Velocity according to the Acceleration, as well as the Orientation
        /// according to the Rotational Velocity and Rotational Acceleration.
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">The time Elapsed in Seconds since the last Update.</param>
        public virtual void Update(float fElapsedTimeInSeconds)
        {
            // If Rotational Acceleration is being used.
            if (RotationalAcceleration != Vector3.Zero)
            {
                // Update the Rotational Velocity according to the Rotational Acceleration.
                RotationalVelocity += (RotationalAcceleration * fElapsedTimeInSeconds);
            }

            // If Rotational Velocity is being used.
            if (RotationalVelocity != Vector3.Zero)
            {
                // Quaternion Rotation Formula: NewOrientation = OldNormalizedOrientation * Rotation(v*t) * 0.5,
                // where v is the angular(rotational) velocity vector, and t is the amount of time elapsed.
                // The 0.5 is used to scale the rotation so that Pi = 180 degree rotation.
                Orientation.Normalize();
                Quaternion sRotation = new Quaternion(RotationalVelocity * (fElapsedTimeInSeconds * 0.5f), 0);
                Orientation += Orientation * sRotation;
            }
        }

        #region Static Class Functions

        /// <summary>
        /// Returns the given Quaternion rotated about its center, changing its Orientation.
        /// </summary>
        /// <param name="sRotationMatrix">The Rotation to apply to the Quaternion.</param>
        /// <param name="sQuaterionToRotate">The Quaternion that should be Rotated.</param>
        /// <returns>Returns the given Quaternion rotated about its center, changing its Orientation.</returns>
        public static Quaternion Rotate(Matrix sRotationMatrix, Quaternion sQuaterionToRotate)
        {
            // Rotate the object about it's center to change its Orientation.
            sQuaterionToRotate.Normalize();
            return (sQuaterionToRotate *= Quaternion.CreateFromRotationMatrix(sRotationMatrix));
        }

        /// <summary>
        /// Returns the shortest arc Quaternion Rotation needed to rotate the CurrentDirection to
        /// be the same as the DestinationDirection.
        /// This method is based on Stan Melax's article in Game Programming Gems, and
        /// the code was referenced from OgreVector3.h of the Ogre library (www.Ogre3d.org).
        /// </summary>
        /// <param name="CurrentDirection">The current Direction the Vector is facing.</param>
        /// <param name="DesiredDirection">The Direction we want the Vector to face.</param>
        /// <returns>Returns the shortest arc Quaternion Rotation needed to rotate the CurrentDirection to
        /// be the same as the DestinationDirection.</returns>
        public static Quaternion GetRotationTo(Vector3 CurrentDirection, Vector3 DesiredDirection)
        {
            return GetRotationTo(CurrentDirection, DesiredDirection, Vector3.Zero);
        }

        /// <summary>
        /// Returns the shortest arc Quaternion Rotation needed to rotate the CurrentDirection to
        /// be the same as the DestinationDirection.
        /// This method is based on Stan Melax's article in Game Programming Gems, and
        /// the code was referenced from OgreVector3.h of the Ogre library (www.Ogre3d.org).
        /// </summary>
        /// <param name="CurrentDirection">The current Direction the Vector is facing.</param>
        /// <param name="DesiredDirection">The Direction we want the Vector to face.</param>
        /// <param name="sFallbackAxis">The Axis to rotate around if a 180 degree rotation is required.</param>
        /// <returns>Returns the shortest arc Quaternion Rotation needed to rotate the CurrentDirection to
        /// be the same as the DestinationDirection.</returns>
        public static Quaternion GetRotationTo(Vector3 CurrentDirection, Vector3 DesiredDirection, Vector3 sFallbackAxis)
        {
            // Initialize the Rotation Quaternion.
            Quaternion cRotationQuaternion = new Quaternion();

            // Get handles to the Current Direction and the Desired Direction.
            Vector3 sCurrentDirection = CurrentDirection;
            Vector3 sDestinationDirection = DesiredDirection;

            // Normalize both Vectors.
            sCurrentDirection.Normalize();
            sDestinationDirection.Normalize();

            // Get the Dot Product of the two Vectors.
            float fDotProduct = Vector3.Dot(sCurrentDirection, sDestinationDirection);

            // If the DotProduct == 1 the Vectors are the same, so no rotation is needed.
            if (fDotProduct >= 1.0f)
            {
                return Quaternion.Identity;
            }

            // If the Dot Product is close to -1 the Vectors are opposites/inverse (i.e. a 180 degree rotation is needed).
            // This will cause the cross product vector to approach zero, which would result in an unstable 
            // rotation. Since ANY axis will do when rotating 180 degrees (i.e. there is no shortest axis to 
            // rotate around), we'll pick an arbitrary one.
            if (fDotProduct <= -0.999999f)
            {
                // If a Fallback Axis was provided.
                if (sFallbackAxis != Vector3.Zero)
                {
                    // Create the Rotation Quaternion around the Fallback Axis.
                    sFallbackAxis.Normalize();
                    cRotationQuaternion = Quaternion.CreateFromAxisAngle(sFallbackAxis, MathHelper.Pi);
                }
                // Else Fallback Axis was not provided.
                else
                {
                    // Generate an arbitrary Axis perpendicular to the Current Direction to rotate around.
                    Vector3 sAxis = Vector3.Cross(Vector3.UnitX, sCurrentDirection);

                    // If the Axis chosen will still result in a cross product vector that approaches zero.
                    if (sAxis.LengthSquared() < (-0.999999f * -0.999999f))
                    {
                        // Pick another perpendicular arbitrary Axis to rotate around.
                        sAxis = Vector3.Cross(Vector3.UnitY, sCurrentDirection);
                    }

                    // Normalize the Axis.
                    sAxis.Normalize();

                    // Create the Rotation Quaternion.
                    cRotationQuaternion = Quaternion.CreateFromAxisAngle(sAxis, MathHelper.Pi);
                }
            }
            // Else the Vectors are not going in opposite directions.
            else
            {
                // Get the Cross Product of the Vectors.
                Vector3 sCrossProduct = Vector3.Cross(sCurrentDirection, sDestinationDirection);

                // Calculate some Math variables needed to create the Rotation Quaternion.
                double dS = Math.Sqrt((double)((1 + fDotProduct) * 2));
                double dInverseOfS = 1 / dS;

                // Create the Rotation Quaternion.
                cRotationQuaternion.X = (float)(sCrossProduct.X * dInverseOfS);
                cRotationQuaternion.Y = (float)(sCrossProduct.Y * dInverseOfS);
                cRotationQuaternion.Z = (float)(sCrossProduct.Z * dInverseOfS);
                cRotationQuaternion.W = (float)(dS * 0.5);
                cRotationQuaternion.Normalize();
            }

            // Return the Rotation Quaternion needed to make the Current Direction face the 
            // same direction as the Destination Direction.
            return cRotationQuaternion;
        }

        /// <summary>
        /// Returns a Quaternion orientated according to the given Normal and Up Directions.
        /// </summary>
        /// <param name="sNormalDirection">The Normal (forward) direction that the Quaternion should face.</param>
        /// <param name="sUpDirection">The Up direction that the Quaternion should have.</param>
        /// <returns>Returns a Quaternion orientated according to the given Normal and Up Directions.</returns>
        public static Quaternion GetQuaternionWithOrientation(Vector3 sNormalDirection, Vector3 sUpDirection)
        {
            Quaternion cQuaternion = Quaternion.Identity;

            // Rotate the Quaternion to face the Normal Direction.
            Quaternion cNormalRotation = GetRotationTo(Vector3.Forward, sNormalDirection);
            cQuaternion = cNormalRotation * cQuaternion;

            // Find the current Up Direction and rotate the Quaternion to use the specified Up Direction.
            Vector3 sCurrentUpDirection = Vector3.Transform(Vector3.Up, cQuaternion);
            Quaternion cUpRotation = GetRotationTo(sCurrentUpDirection, sUpDirection);
            cQuaternion = cUpRotation * cQuaternion;

            // Return the Quaternion with the specified orientation.
            return cQuaternion;
        }

        /// <summary>
        /// Returns the Normal (Forward) Direction of the given Quaternion.
        /// </summary>
        /// <param name="sOrientation">The Quaternion whose Direction we want.</param>
        /// <returns>Returns the Normal (Forward) Direction of the given Quaternion.</returns>
        public static Vector3 GetNormalDirection(Quaternion sOrientation)
        {
            return Vector3.Normalize(Vector3.Transform(Vector3.Forward, sOrientation));
        }

        /// <summary>
        /// Sets the Normal direction of the given Quaternion to be the given New Normal Direction.
        /// </summary>
        /// <param name="sOrientation">The Quaternion to modify.</param>
        /// <param name="sNewNormalDirection">The New Normal Direction the Quaternion should have.</param>
        public static void SetNormalDirection(ref Quaternion sOrientation, Vector3 sNewNormalDirection)
        {
            // Get the Rotation needed to make our Normal face the given Direction.
            Quaternion sRotation = Orientation3D.GetRotationTo(Orientation3D.GetNormalDirection(sOrientation), sNewNormalDirection);

            // Rotate the object to face the new Normal Direction.
            sOrientation.Normalize();
            sOrientation = sRotation * sOrientation;
        }

        /// <summary>
        /// Returns the Up Direction of the given Quaternion.
        /// </summary>
        /// <param name="sOrientation">The Quaternion whose Direction we want.</param>
        /// <returns>Returns the Up Direction of the given Quaternion.</returns>
        public static Vector3 GetUpDirection(Quaternion sOrientation)
        {
            return Vector3.Normalize(Vector3.Transform(Vector3.Up, sOrientation));
        }

        /// <summary>
        /// Sets the Up direction of the given Quaternion to be the given New Up Direction.
        /// </summary>
        /// <param name="sOrientation">The Quaternion to modify.</param>
        /// <param name="sNewUpDirection">The New Up Direction the Quaternion should have.</param>
        public static void SetUpDirection(ref Quaternion sOrientation, Vector3 sNewUpDirection)
        {
            // Get the Rotation needed to make our Up face the given Direction.
            Quaternion sRotation = Orientation3D.GetRotationTo(Orientation3D.GetUpDirection(sOrientation), sNewUpDirection);

            // Rotate the object.
            sOrientation.Normalize();
            sOrientation = sRotation * sOrientation;
        }

        /// <summary>
        /// Returns the Right Direction of the given Quaternion.
        /// </summary>
        /// <param name="sOrientation">The Quaternion whose Direction we want.</param>
        /// <returns>Returns the Right Direction of the given Quaternion.</returns>
        public static Vector3 GetRightDirection(Quaternion sOrientation)
        {
            return Vector3.Normalize(Vector3.Transform(Vector3.Right, sOrientation));
        }

        /// <summary>
        /// Sets the Right direction of the given Quaternion to be the given New Right Direction.
        /// </summary>
        /// <param name="sOrientation">The Quaternion to modify.</param>
        /// <param name="sNewRightDirection">The New Right Direction the Quaternion should have.</param>
        public static void SetRightDirection(ref Quaternion sOrientation, Vector3 sNewRightDirection)
        {
            // Get the Rotation needed to make our Right face the given Direction.
            Quaternion sRotation = Orientation3D.GetRotationTo(Orientation3D.GetRightDirection(sOrientation), sNewRightDirection);

            // Rotate the object.
            sOrientation.Normalize();
            sOrientation = sRotation * sOrientation;
        }
        #endregion
    }
}
