#region File Description
//===================================================================
// DPSFHelper.cs
// 
// This file provides some classes and functions used to perform some
// common or general purpose operations.
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF
{
    #region Helpful Functions

    /// <summary>
    /// Collection of static functions for performing common operations
    /// </summary>
    [Serializable]
    public class DPSFHelper
    {
        // Variable used to generate Random values
        private static RandomNumbers RandomNumber = new RandomNumbers();

        /// <summary>
        /// Return the version of the DPSF.dll being used. 
        /// This includes the Major, Minor, Build, and Revision numbers.
        /// </summary>
        public static string Version
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        /// <summary>
        /// Get if the Particle Systems are inheriting from DrawableGameComponent or not.
        /// <para>If inheriting from DrawableGameComponent, the Particle Systems
        /// are automatically added to the given Game object's Components and the
        /// Update() and Draw() functions are automatically called by the
        /// Game object when it updates and draws the rest of its Components.</para>
        /// <para>If the Update() and Draw() functions are called by the user anyways,
        /// they will exit without performing any operations, so it is suggested
        /// to include them anyways to make switching between inheriting and
        /// not inheriting from DrawableGameComponent seemless; just be aware
        /// that the updates and draws are actually being performed when the
        /// Game object is told to update and draw (i.e. when base.Update() and base.Draw()
        /// are called), not when the particle system functions are called.</para>
        /// </summary>
        public static bool DPSFInheritsDrawableGameComponent
        {
        // If DPSF is inheriting DrawableGameComponent
        #if (DPSFAsDrawableGameComponent)
            get { return true; }
        // Else DPSF is not inheriting DrawableGameComponent
        #else
            get { return false; }
        #endif
        }

        /// <summary>
        /// Returns a random number between the specified values
        /// </summary>
        /// <param name="fValue1">The first value</param>
        /// <param name="fValue2">The second value</param>
        /// <returns>Returns a random number between the specified values</returns>
        public static float RandomNumberBetween(float fValue1, float fValue2)
        {
            return RandomNumber.Between(fValue1, fValue2);
        }

        /// <summary>
        /// Returns how transparent a Particle should be, based on it's Normalized Elapsed Time, so that it
        /// fades in quickly and fades out slowly.
        /// </summary>
        /// <param name="fNormalizedElapsedTime">The current Normalized Elapsed Time (0.0 - 1.0) of a Particle</param>
        /// <returns>Returns the Alpha Color component that should be used for the Particle</returns>
        public static byte FadeInQuicklyAndFadeOutSlowlyBasedOnLifetime(float fNormalizedElapsedTime)
        {
            // Calculate how transparent the Particle should be based on it's NormalizedLifetime
            return (byte)((fNormalizedElapsedTime * (1 - fNormalizedElapsedTime) * (1 - fNormalizedElapsedTime) * 6.7) * 255);
        }

        /// <summary>
        /// Returns how transparent a Particle should be, based on it's Normalized Elapsed Time, so that it
        /// fades in quickly and fades out quickly.
        /// </summary>
        /// <param name="fNormalizedElapsedTime">The current Normalized Elapsed Time (0.0 - 1.0) of a Particle</param>
        /// <returns>Returns the Alpha Color component that should be used for the Particle</returns>
        public static byte FadeInQuicklyAndFadeOutQuicklyBasedOnLifetime(float fNormalizedElapsedTime)
        {
            // If the Particle should be fading in
            if (fNormalizedElapsedTime < 0.1f)
            {
                return (byte)(int)MathHelper.Lerp(0, 255, fNormalizedElapsedTime * 10);
            }
            // Else if the Particle should be fading out
            else if (fNormalizedElapsedTime > 0.8f)
            {
                return (byte)(int)MathHelper.Lerp(255, 0, (fNormalizedElapsedTime - 0.8f) * 5);
            }
            // Else the Particle should be fully opaque
            else
            {
                return (byte)255;
            }
        }

        /// <summary>
        /// Returns the interpolation amount (between 0.0 and 1.0) that should be used in a Lerp function to have a 
        /// property reach its full value when the NormalizedLifetime reaches 0.5, and go back to its original value 
        /// by the time the NormalizedLifetime reaches 1.0.
        /// <para>An example of where to use this would be if you wanted a particle to start off small, and reach
        /// its full size when the particle's lifetime is half over, and then to shrink back to being small by the
        /// time the particle dies. You would use the value returned by this function as the interpolation amount
        /// for the Lerp function. e.g. MathHelper.Lerp(SmallSize, LargeSize, ValueReturnedByThisFunction).</para>
        /// </summary>
        /// <param name="fNormalizedElapsedTime">The current Normalized Elapsed Time (0.0 - 1.0) of a Particle</param>
        /// <returns>Returns the interpolation amount (between 0.0 and 1.0) that should be used in a Lerp function to have a 
        /// property reach its full value when the NormalizedLifetime reaches 0.5, and go back to its original value 
        /// by the time the NormalizedLifetime reaches 1.0.</returns>
        public static float InterpolationAmountForEqualLerpInAndLerpOut(float fNormalizedElapsedTime)
        {
            // If the value should be approaching 1.0
            if (fNormalizedElapsedTime <= 0.5f)
            {
                return fNormalizedElapsedTime * 2.0f;
            }
            // Else the value should be approaching 0.0
            else
            {
                return 1.0f - ((fNormalizedElapsedTime - 0.5f) * 2.0f);
            }
        }

        /// <summary>
        /// Returns a vector with a Random direction that has been Normalized.
        /// </summary>
        /// <returns>Returns a vector with a Random direction that has been Normalized.</returns>
        public static Vector3 RandomNormalizedVector()
        {
            Vector3 sRandomVector = new Vector3(RandomNumber.Between(-1.0f, 1.0f), RandomNumber.Between(-1.0f, 1.0f), RandomNumber.Between(-1.0f, 1.0f));
            sRandomVector.Normalize();
            return sRandomVector;
        }

        /// <summary>
        /// Returns a vector representing the line from the source point to the target point.
        /// </summary>
        /// <param name="source">The source position.</param>
        /// <param name="target">The target position.</param>
        /// <returns>Returns a vector representing the line from the source point to the target point.</returns>
        public static Vector3 SourceToTargetVector(Vector3 source, Vector3 target)
        {
            return (target - source);
        }

        /// <summary>
        /// Returns a normalized vector representing the direction that points from the source point to the target point.
        /// </summary>
        /// <param name="source">The source position.</param>
        /// <param name="target">The target position.</param>
        /// <returns>Returns a normalized vector representing the direction that points from the source point to the target point.</returns>
        public static Vector3 SourceToTargetDirection(Vector3 source, Vector3 target)
        {
            Vector3 vector = SourceToTargetVector(source, target);
            vector.Normalize();
            return vector;
        }

        /// <summary>
        /// Returns a Vector whose individual XYZ components are each randomly chosen to be somewhere 
        /// between the two given Vectors' individual XYZ components. Unlike choosing a random Lerp value
        /// between two vectors, which would give a point somewhere on the LINE between the two points, this
        /// chooses a random Lerp value between each of the two vectors individual xyz components, returning
        /// a point somewhere in the cube-shaped AREA (i.e. Volume) between the two points.
        /// </summary>
        /// <param name="sVector1">The first Vector</param>
        /// <param name="sVector2">The second Vector</param>
        /// <returns>Returns a Vector whose individual XYZ components are each randomly chosen to be somewhere 
        /// between the two given Vectors' individual XYZ components.</returns>
        public static Vector3 RandomVectorBetweenTwoVectors(Vector3 sVector1, Vector3 sVector2)
        {
            return new Vector3(RandomNumber.Between(sVector1.X, sVector2.X),
                               RandomNumber.Between(sVector1.Y, sVector2.Y),
                               RandomNumber.Between(sVector1.Z, sVector2.Z));
        }

        /// <summary>
        /// Returns a random opaque Color (i.e. no transparency)
        /// </summary>
        /// <returns>Returns a random opaque Color (i.e. no transparency)</returns>
        public static Color RandomColor()
        {
            return new Color(RandomNumber.NextFloat(), RandomNumber.NextFloat(), RandomNumber.NextFloat());
        }

        /// <summary>
        /// Returns a random Color with a random alpha value as well (i.e. random transparency)
        /// </summary>
        /// <returns>Returns a random Color with a random alpha value as well (i.e. random transparency)</returns>
        public static Color RandomColorWithRandomTransparency()
        {
            return new Color(RandomNumber.NextFloat(), RandomNumber.NextFloat(), RandomNumber.NextFloat(), RandomNumber.NextFloat());
        }

        /// <summary>
        /// Returns the linearly interpolated Color between Color 1 and Color 2
        /// </summary>
        /// <param name="sColor1">The Color value used with InterpolationAmount 0.0</param>
        /// <param name="sColor2">The Color value used with InterpolationAmount 1.0</param>
        /// <param name="fInterpolationAmount">A value between 0.0 and 1.0 indicating how 
        /// much to interpolate the RGBA values between Color 1 and Color 2</param>
        /// <returns>Returns the interpolated Color</returns>
        public static Color LerpColor(Color sColor1, Color sColor2, float fInterpolationAmount)
        {
            return LerpColor(sColor1, sColor2, fInterpolationAmount, fInterpolationAmount, fInterpolationAmount, fInterpolationAmount);
        }

        /// <summary>
        /// Returns the linearly interpolated Color between Color 1 and Color 2
        /// </summary>
        /// <param name="sColor1">The Color value used with InterpolationAmount 0.0</param>
        /// <param name="sColor2">The Color value used with InterpolationAmount 1.0</param>
        /// <param name="fRInterpolationAmount">A value between 0.0 and 1.0 indicating how 
        /// much to interpolate the Red value between Color 1 and Color 2</param>
        /// <param name="fGInterpolationAmount">A value between 0.0 and 1.0 indicating how 
        /// much to interpolate the Green value between Color 1 and Color 2</param>
        /// <param name="fBInterpolationAmount">A value between 0.0 and 1.0 indicating how 
        /// much to interpolate the Blue value between Color 1 and Color 2</param>
        /// <param name="fAInterpolationAmount">A value between 0.0 and 1.0 indicating how 
        /// much to interpolate the Alpha value between Color 1 and Color 2</param>
        /// <returns>Returns the interpolated Color</returns>
        public static Color LerpColor(Color sColor1, Color sColor2, float fRInterpolationAmount, 
                        float fGInterpolationAmount, float fBInterpolationAmount, float fAInterpolationAmount)
        {
            byte bRed = (byte)(sColor1.R + (sColor2.R - sColor1.R) * fRInterpolationAmount);
            byte bGreen = (byte)(sColor1.G + (sColor2.G - sColor1.G) * fGInterpolationAmount);
            byte bBlue = (byte)(sColor1.B + (sColor2.B - sColor1.B) * fBInterpolationAmount);
            byte bAlpha = (byte)(sColor1.A + (sColor2.A - sColor1.A) * fAInterpolationAmount);
            return new Color(bRed, bGreen, bBlue, bAlpha);
        }

        /// <summary>
        /// Returns a point on a circle with a radius of one, on the X-Y axis plane. 
        /// To use a different radius simply multiply the returned value by the desired radius value.
        /// </summary>
        /// <param name="fAngle">The angle on the circle in radians</param>
        /// <returns>Returns a 2D position on a circle</returns>
        public static Vector2 PointOnCircle(float fAngle)
        {
            float fX = (float)Math.Cos(fAngle);
            float fY = (float)Math.Sin(fAngle);
            return new Vector2(fX, fY);
        }

        /// <summary>
        /// Returns a point on a circle with a radius of one, on the X-Y axis plane (Z value of zero). 
        /// To use a different radius simply multiply the returned value by the desired radius value.
        /// </summary>
        /// <param name="fAngle">The angle on the circle in radians</param>
        /// <returns>Returns a 3D position on a circle</returns>
        public static Vector3 PointOnCircleVector3(float fAngle)
        {
            return new Vector3(PointOnCircle(fAngle), 0);
        }

        /// <summary>
        /// Returns a random point on a circle with a radius of one, on the X-Y axis plane. 
        /// To use a different radius simply multiply the returned value by the desired radius value.
        /// </summary>
        /// <returns>Returns a random 2D position on a circle</returns>
        public static Vector2 RandomPointOnCircle()
        {
            return PointOnCircle(RandomNumber.Between(0, MathHelper.TwoPi));
        }

        /// <summary>
        /// Returns a random point on a circle with a radius of one, on the X-Y axis plane (Z value of zero). 
        /// To use a different radius simply multiply the returned value by the desired radius value.
        /// </summary>
        /// <returns>Returns a random 3D position on a circle, with a Z value of zero</returns>
        public static Vector3 RandomPointOnCircleVector3()
        {
            return new Vector3(RandomPointOnCircle(), 0);
        }

        /// <summary>
        /// Returns a point on a sphere with a radius of one.  To use a different radius simply 
        /// multiply the returned value by the desired radius value, before translating it
        /// to the sphere's position in world coordinates.  To create a circle simply use a
        /// constant value for one of the Angles while changing the other Angle.
        /// </summary>
        /// <param name="fYawAngle">Imagine a point on the surface of the sphere at the front 
        /// center of the sphere. This value will be how much to rotate that point around the 
        /// horizontal ring around the center of the sphere, in radians.</param>
        /// <param name="fPitchAngle">This value will be how much to rotate that point around 
        /// the vertical ring around the sphere, at the current position of the point, in radians.</param>
        /// <returns>Returns a point on a sphere with a radius of one</returns>
        public static Vector3 NormalizedPointOnSphere(float fYawAngle, float fPitchAngle)
        {
            return PivotPoint3D.RotatePosition(Matrix.CreateFromYawPitchRoll(fYawAngle, fPitchAngle, 0), Vector3.Zero, Vector3.Forward);
        }

        /// <summary>
        /// Returns a random point on a sphere with a radius of one.  To use a different radius simply 
        /// multiply the returned value by the desired radius value, before translating it
        /// to the sphere's position in world coordinates.
        /// </summary>
        /// <returns>Returns a random point on a sphere with a radius of one</returns>
        public static Vector3 RandomNormalizedPointOnSphere()
        {
            return NormalizedPointOnSphere(RandomNumber.Between(0, MathHelper.TwoPi), RandomNumber.Between(0, MathHelper.TwoPi));
        }

        /// <summary>
        /// Returns a point on a sphere with the given Radius. To create a circle simply use a
        /// constant value for one of the Angles while changing the other Angle.
        /// </summary>
        /// <param name="fYawAngle">Imagine a point on the surface of the sphere at the front 
        /// center of the sphere. This value will be how much to rotate that point around the 
        /// horizontal ring around the center of the sphere, in radians.</param>
        /// <param name="fPitchAngle">This value will be how much to rotate that point around 
        /// the vertical ring around the sphere, at the current position of the point, in radians.</param>
        /// <param name="fRadius">The radius the sphere should have</param>
        /// <returns>Returns a point on a sphere with the given radius</returns>
        public static Vector3 PointOnSphere(float fYawAngle, float fPitchAngle, float fRadius)
        {
            return (NormalizedPointOnSphere(fYawAngle, fPitchAngle) * fRadius);
        }

        /// <summary>
        /// Returns a random point on a sphere with the given Radius
        /// </summary>
        /// <param name="fRadius">The radius the sphere should have</param>
        /// <returns>Returns a random point on a sphere with the given Radius</returns>
        public static Vector3 RandomPointOnSphere(float fRadius)
        {
            return PointOnSphere(RandomNumber.Between(0, MathHelper.TwoPi), RandomNumber.Between(0, MathHelper.TwoPi), fRadius);
        }

        /// <summary>
        /// Returns a point on a sphere with the given Radius, at the specified Sphere Position.
        /// To create a circle simply use a constant value for one of the Angles while changing 
        /// the other Angle.
        /// </summary>
        /// <param name="fYawAngle">Imagine a point on the surface of the sphere at the front 
        /// center of the sphere. This value will be how much to rotate that point around the 
        /// horizontal ring around the center of the sphere, in radians.</param>
        /// <param name="fPitchAngle">This value will be how much to rotate that point around 
        /// the vertical ring around the sphere, at the current position of the point, in radians.</param>
        /// <param name="fRadius">The radius the sphere should have</param>
        /// <param name="sSpherePosition">The center position of the sphere in world coordinates</param>
        /// <returns>Returns a point on a sphere with the given Radius, at the specified Sphere Position</returns>
        public static Vector3 PointOnSphere(float fYawAngle, float fPitchAngle, float fRadius, Vector3 sSpherePosition)
        {
            return PointOnSphere(fYawAngle, fPitchAngle, fRadius) + sSpherePosition;
        }

        /// <summary>
        /// Returns a random point on a sphere with the given Radius, at the specified Sphere Position
        /// </summary>
        /// <param name="fRadius">The radius the sphere should have</param>
        /// <param name="sSpherePosition">The center position of the sphere in world coordinates</param>
        /// <returns>Returns a random point on a sphere with the given Radius, at the specified Sphere Position</returns>
        public static Vector3 RandomPointOnSphere(float fRadius, Vector3 sSpherePosition)
        {
            return PointOnSphere(RandomNumber.Between(0, MathHelper.TwoPi), RandomNumber.Between(0, MathHelper.TwoPi), fRadius, sSpherePosition);
        }

        /// <summary>
        /// Returns true if the difference between the individual XYZ components of the
        /// given Vectors are all less than the specified Tolerance
        /// </summary>
        /// <param name="sVector1">The first Vector</param>
        /// <param name="sVector2">The second Vector</param>
        /// <param name="fTolerance">How much of a difference there may be between the individual
        /// XYZ components for them to be considered equal</param>
        /// <returns>Returns true if the difference between the individual XYZ components of the
        /// given Vectors are each less than the specified Tolerance</returns>
        public static bool VectorsAreEqualWithinTolerance(Vector3 sVector1, Vector3 sVector2, float fTolerance)
        {
            // Normalize both Vectors first
            sVector1.Normalize();
            sVector2.Normalize();

            // Get the difference between the two Vectors
            Vector3 sSubtractedVector = sVector1 - sVector2;

            // If the Vectors are equal within the given Tolerance
            if (Math.Abs(sSubtractedVector.X) <= fTolerance &&
                Math.Abs(sSubtractedVector.X) <= fTolerance &&
                Math.Abs(sSubtractedVector.X) <= fTolerance)
            {
                // Return that they are equal
                return true;
            }
            // Else return that they are not equal
            return false;
        }
    }
    #endregion

    #region Helpful Classes

    /// <summary>
    /// Class that may be used to obtain random numbers. This class inherits the Random class
    /// and adds additional functionality.
    /// </summary>
    [Serializable]
    public class RandomNumbers : Random
    {
        /// <summary>
        /// Returns a random number between the specified values
        /// </summary>
        /// <param name="fValue1">The first value</param>
        /// <param name="fValue2">The second value</param>
        /// <returns>Returns a random number between the specified values</returns>
        public float Between(float fValue1, float fValue2)
        {
            return fValue1 + ((float)NextDouble() * (fValue2 - fValue1));
        }
        
        /// <summary>
        /// Returns a random number between 0.0f and 1.0f.
        /// </summary>
        /// <returns>Returns a random number between 0.0f and 1.0f.</returns>
        public float NextFloat()
        {
            return (float)NextDouble();
        }
    }

    /// <summary>
    /// Class used to hold and update an object's 2D Position, Velocity, and Acceleration
    /// </summary>
    [Serializable]
    public class Position2D
    {
        /// <summary>
        /// The object's 2D Position
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// The object's Position
        /// </summary>
        public Vector2 Velocity = Vector2.Zero;

        /// <summary>
        /// The object's Acceleration
        /// </summary>
        public Vector2 Acceleration = Vector2.Zero;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Position2D() { }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="cPositionToCopy">The Position2D object to copy</param>
        public Position2D(Position2D cPositionToCopy)
        {
            CopyFrom(cPositionToCopy);
        }

        /// <summary>
        /// Copies the given Position2D object's data into this objects data
        /// </summary>
        /// <param name="cPositionToCopy">The Position2D object to copy</param>
        public void CopyFrom(Position2D cPositionToCopy)
        {
            Position = cPositionToCopy.Position;
            Velocity = cPositionToCopy.Velocity;
            Acceleration = cPositionToCopy.Acceleration;
        }

        /// <summary>
        /// Get / Set the object's 2D Position using a Vector3.
        /// <para>NOTE: The Z-value is ignored when Setting, and is given a value of zero when Getting.</para>
        /// <para>NOTE: This function is provided for convenience when using 3D Vectors in 2D coordinate systems.</para>
        /// </summary>
        public Vector3 PositionVector3
        {
            get { return new Vector3(Position.X, Position.Y, 0); }
            set { Position = new Vector2(value.X, value.Y); }
        }

        /// <summary>
        /// Get / Set the object's 2D Velocity using a Vector3.
        /// <para>NOTE: The Z-value is ignored when Setting, and is given a value of zero when Getting.</para>
        /// <para>NOTE: This function is provided for convenience when using 3D Vectors in 2D coordinate systems.</para>para>
        /// </summary>
        public Vector3 VelocityVector3
        {
            get { return new Vector3(Velocity.X, Velocity.Y, 0); }
            set { Velocity = new Vector2(value.X, value.Y); }
        }

        /// <summary>
        /// Get / Set the object's 2D Acceleration using a Vector3.
        /// <para>NOTE: The Z-value is ignored when Setting, and is given a value of zero when Getting.</para>
        /// <para>NOTE: This function is provided for convenience when using 3D Vectors in 2D coordinate systems.</para>para>
        /// </summary>
        public Vector3 AccelerationVector3
        {
            get { return new Vector3(Acceleration.X, Acceleration.Y, 0); }
            set { Acceleration = new Vector2(value.X, value.Y); }
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

    /// <summary>
    /// Class used to hold and update an object's 3D Position, Velocity, and Acceleration
    /// </summary>
    [Serializable]
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
        public void CopyFrom(Position3D cPositionToCopy)
        {
            Position = cPositionToCopy.Position;
            Velocity = cPositionToCopy.Velocity;
            Acceleration = cPositionToCopy.Acceleration;
        }

        /// <summary>
        /// Update the Position and Velocity according to the Acceleration
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">The time Elapsed in Seconds since the last Update</param>
        public void Update(float fElapsedTimeInSeconds)
        {
            // Update the Velocity and Position according to how much Time has Elapsed
            Velocity += Acceleration * fElapsedTimeInSeconds;
            Position += Velocity * fElapsedTimeInSeconds;
        }
    }

    /// <summary>
    /// Class to hold and update an object's 2D Orientation, Rotational Velocity, and Rotational Acceleration
    /// </summary>
    [Serializable]
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
        public void CopyFrom(Orientation2D cOrientationToCopy)
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
        public void Update(float fElapsedTimeInSeconds)
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

    /// <summary>
    /// Class to hold and update an object's 3D Orientation, Rotational Velocity, and Rotational Acceleration
    /// </summary>
    [Serializable]
    public class Orientation3D
    {
        /// <summary>
        /// The object's Orientation
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
        /// Default Constructor
        /// </summary>
        public Orientation3D() { }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="cOrienationToCopy">The Orienation3D object to copy</param>
        public Orientation3D(Orientation3D cOrienationToCopy)
        {
            CopyFrom(cOrienationToCopy);
        }

        /// <summary>
        /// Copies the given Orientation3D object's data into this object's data
        /// </summary>
        /// <param name="cOrientationToCopy">The Orientation3D object to copy from</param>
        public void CopyFrom(Orientation3D cOrientationToCopy)
        {
            Orientation = cOrientationToCopy.Orientation;
            RotationalVelocity = cOrientationToCopy.RotationalVelocity;
            RotationalAcceleration = cOrientationToCopy.RotationalAcceleration;
        }

        /// <summary>
        /// Get / Set the Normal (i.e. Forward) direction of the object (i.e. which direction it is facing)
        /// </summary>
        public Vector3 Normal
        {
            get { return Orientation3D.GetNormalDirection(Orientation); }
            set { Orientation3D.SetNormalDirection(ref Orientation, value); }
        }

        /// <summary>
        /// Get / Set the Up direction of the object
        /// </summary>
        public Vector3 Up
        {
            get { return Orientation3D.GetUpDirection(Orientation); }
            set { Orientation3D.SetUpDirection(ref Orientation, value); }
        }

        /// <summary>
        /// Get / Set the Right direction of the object
        /// </summary>
        public Vector3 Right
        {
            get { return Orientation3D.GetRightDirection(Orientation); }
            set { Orientation3D.SetRightDirection(ref Orientation, value); }
        }

        /// <summary>
        /// Rotates the object about its center, changing its Orientation
        /// </summary>
        /// <param name="sRotationMatrix">The Rotation to apply to the object</param>
        public void Rotate(Matrix sRotationMatrix)
        {
            // Rotate the object about it's center to change its Orientation
            Orientation = Orientation3D.Rotate(sRotationMatrix, Orientation);
        }

        /// <summary>
        /// Update the Position and Velocity according to the Acceleration, as well as the Orientation
        /// according to the Rotational Velocity and Rotational Acceleration
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">The time Elapsed in Seconds since the last Update</param>
        public void Update(float fElapsedTimeInSeconds)
        {
            // If Rotational Acceleration is being used
            if (RotationalAcceleration != Vector3.Zero)
            {
                // Update the Rotational Velocity according to the Rotational Acceleration
                RotationalVelocity += (RotationalAcceleration * fElapsedTimeInSeconds);
            }

            // If Rotational Velocity is being used
            if (RotationalVelocity != Vector3.Zero)
            {
                // Quaternion Rotation Formula: NewOrientation = OldNormalizedOrientation * Rotation(v*t) * 0.5,
                // where v is the angular(rotational) velocity vector, and t is the amount of time elapsed.
                // The 0.5 is used to scale the rotation so that Pi = 180 degree rotation
                Orientation.Normalize();
                Quaternion sRotation = new Quaternion(RotationalVelocity * (fElapsedTimeInSeconds * 0.5f), 0);
                Orientation += Orientation * sRotation;
            }
        }

        #region Static Class Functions

        /// <summary>
        /// Returns the given Quaternion rotated about its center, changing its Orientation
        /// </summary>
        /// <param name="sRotationMatrix">The Rotation to apply to the Quaternion</param>
        /// <param name="sQuaterionToRotate">The Quaternion that should be Rotated</param>
        /// <returns>Returns the given Quaternion rotated about its center, changing its Orientation</returns>
        public static Quaternion Rotate(Matrix sRotationMatrix, Quaternion sQuaterionToRotate)
        {
            // Rotate the object about it's center to change its Orientation
            sQuaterionToRotate.Normalize();
            return (sQuaterionToRotate *= Quaternion.CreateFromRotationMatrix(sRotationMatrix));
        }

        /// <summary>
        /// Returns the shortest arc Quaternion Rotation needed to rotate the CurrentDirection to
        /// be the same as the DestinationDirection.
        /// This method is based on Stan Melax's article in Game Programming Gems, and
        /// the code was referenced from OgreVector3.h of the Ogre library (www.Ogre3d.org)
        /// </summary>
        /// <param name="CurrentDirection">The current Direction the Vector is facing</param>
        /// <param name="DesiredDirection">The Direction we want the Vector to face</param>
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
        /// the code was referenced from OgreVector3.h of the Ogre library (www.Ogre3d.org)
        /// </summary>
        /// <param name="CurrentDirection">The current Direction the Vector is facing</param>
        /// <param name="DesiredDirection">The Direction we want the Vector to face</param>
        /// <param name="sFallbackAxis">The Axis to rotate around if a 180 degree rotation is required</param>
        /// <returns>Returns the shortest arc Quaternion Rotation needed to rotate the CurrentDirection to
        /// be the same as the DestinationDirection.</returns>
        public static Quaternion GetRotationTo(Vector3 CurrentDirection, Vector3 DesiredDirection, Vector3 sFallbackAxis)
        {
            // Initialize the Rotation Quaternion
            Quaternion cRotationQuaternion = new Quaternion();

            // Get handles to the Current Direction and the Desired Direction
            Vector3 sCurrentDirection = CurrentDirection;
            Vector3 sDestinationDirection = DesiredDirection;

            // Normalize both Vectors 
            sCurrentDirection.Normalize();
            sDestinationDirection.Normalize();

            // Get the Dot Product of the two Vectors
            float fDotProduct = Vector3.Dot(sCurrentDirection, sDestinationDirection);

            // If the DotProduct == 1 the Vectors are the same, so no rotation is needed
            if (fDotProduct >= 1.0f)
            {
                return Quaternion.Identity;
            }

            // If the Dot Product is close to -1 the Vectors are opposites/inverse (i.e. a 180 degree rotation is needed).
            // This will cause the cross product vector to approach zero, which would result in an unstable 
            // rotation. Since ANY axis will do when rotating 180 degrees (i.e. there is no shortest axis to 
            // rotate around), we'll pick an arbitrary one
            if (fDotProduct <= -0.999999f)
            {
                // If a Fallback Axis was provided
                if (sFallbackAxis != Vector3.Zero)
                {
                    // Create the Rotation Quaternion around the Fallback Axis
                    sFallbackAxis.Normalize();
                    cRotationQuaternion = Quaternion.CreateFromAxisAngle(sFallbackAxis, MathHelper.Pi);
                }
                // Else Fallback Axis was not provided
                else
                {
                    // Generate an arbitrary Axis perpendicular to the Current Direction to rotate around
                    Vector3 sAxis = Vector3.Cross(Vector3.UnitX, sCurrentDirection);

                    // If the Axis chosen will still result in a cross product vector that approaches zero
                    if (sAxis.LengthSquared() < (-0.999999f * -0.999999f))
                    {
                        // Pick another perpendicular arbitrary Axis to rotate around
                        sAxis = Vector3.Cross(Vector3.UnitY, sCurrentDirection);
                    }

                    // Normalize the Axis
                    sAxis.Normalize();

                    // Create the Rotation Quaternion
                    cRotationQuaternion = Quaternion.CreateFromAxisAngle(sAxis, MathHelper.Pi);
                }
            }
            // Else the Vectors are not going in opposite directions
            else
            {
                // Get the Cross Product of the Vectors
                Vector3 sCrossProduct = Vector3.Cross(sCurrentDirection, sDestinationDirection);

                // Calculate some Math variables needed to create the Rotation Quaternion
                double dS = Math.Sqrt((double)((1 + fDotProduct) * 2));
                double dInverseOfS = 1 / dS;

                // Create the Rotation Quaternion
                cRotationQuaternion.X = (float)(sCrossProduct.X * dInverseOfS);
                cRotationQuaternion.Y = (float)(sCrossProduct.Y * dInverseOfS);
                cRotationQuaternion.Z = (float)(sCrossProduct.Z * dInverseOfS);
                cRotationQuaternion.W = (float)(dS * 0.5);
                cRotationQuaternion.Normalize();
            }

            // Return the Rotation Quaternion needed to make the Current Direction face the 
            // same direction as the Destination Direction
            return cRotationQuaternion;
        }

        /// <summary>
        /// Returns a Quaternion orientated according to the given Normal and Up Directions
        /// </summary>
        /// <param name="sNormalDirection">The Normal (forward) direction that the Quaternion should face</param>
        /// <param name="sUpDirection">The Up direction that the Quaternion should have</param>
        /// <returns>Returns a Quaternion orientated according to the given Normal and Up Directions</returns>
        public static Quaternion GetQuaternionWithOrientation(Vector3 sNormalDirection, Vector3 sUpDirection)
        {
            Quaternion cQuaternion = Quaternion.Identity;

            // Rotate the Quaternion to face the Normal Direction
            Quaternion cNormalRotation = GetRotationTo(Vector3.Forward, sNormalDirection);
            cQuaternion = cNormalRotation * cQuaternion;

            // Find the current Up Direction and rotate the Quaternion to use the specified Up Direction
            Vector3 sCurrentUpDirection = Vector3.Transform(Vector3.Up, cQuaternion);
            Quaternion cUpRotation = GetRotationTo(sCurrentUpDirection, sUpDirection);
            cQuaternion = cUpRotation * cQuaternion;

            // Return the Quaternion with the specified orientation
            return cQuaternion;
        }

        /// <summary>
        /// Returns the Normal (Forward) Direction of the given Quaternion
        /// </summary>
        /// <param name="sOrientation">The Quaternion whose Direction we want</param>
        /// <returns>Returns the Normal (Forward) Direction of the given Quaternion</returns>
        public static Vector3 GetNormalDirection(Quaternion sOrientation)
        {
            return Vector3.Normalize(Vector3.Transform(Vector3.Forward, sOrientation));
        }

        /// <summary>
        /// Sets the Normal direction of the given Quaternion to be the given New Normal Direction
        /// </summary>
        /// <param name="sOrientation">The Quaternion to modify</param>
        /// <param name="sNewNormalDirection">The New Normal Direction the Quaternion should have</param>
        public static void SetNormalDirection(ref Quaternion sOrientation, Vector3 sNewNormalDirection)
        {
            // Get the Rotation needed to make our Normal face the given Direction
            Quaternion sRotation = Orientation3D.GetRotationTo(Orientation3D.GetNormalDirection(sOrientation), sNewNormalDirection);

            // Rotate the object to face the new Normal Direction
            sOrientation.Normalize();
            sOrientation = sRotation * sOrientation;
        }

        /// <summary>
        /// Returns the Up Direction of the given Quaternion
        /// </summary>
        /// <param name="sOrientation">The Quaternion whose Direction we want</param>
        /// <returns>Returns the Up Direction of the given Quaternion</returns>
        public static Vector3 GetUpDirection(Quaternion sOrientation)
        {
            return Vector3.Normalize(Vector3.Transform(Vector3.Up, sOrientation));
        }

        /// <summary>
        /// Sets the Up direction of the given Quaternion to be the given New Up Direction
        /// </summary>
        /// <param name="sOrientation">The Quaternion to modify</param>
        /// <param name="sNewUpDirection">The New Up Direction the Quaternion should have</param>
        public static void SetUpDirection(ref Quaternion sOrientation, Vector3 sNewUpDirection)
        {
            // Get the Rotation needed to make our Up face the given Direction
            Quaternion sRotation = Orientation3D.GetRotationTo(Orientation3D.GetUpDirection(sOrientation), sNewUpDirection);

            // Rotate the object
            sOrientation.Normalize();
            sOrientation = sRotation * sOrientation;
        }

        /// <summary>
        /// Returns the Right Direction of the given Quaternion
        /// </summary>
        /// <param name="sOrientation">The Quaternion whose Direction we want</param>
        /// <returns>Returns the Right Direction of the given Quaternion</returns>
        public static Vector3 GetRightDirection(Quaternion sOrientation)
        {
            return Vector3.Normalize(Vector3.Transform(Vector3.Right, sOrientation));
        }

        /// <summary>
        /// Sets the Right direction of the given Quaternion to be the given New Right Direction
        /// </summary>
        /// <param name="sOrientation">The Quaternion to modify</param>
        /// <param name="sNewRightDirection">The New Right Direction the Quaternion should have</param>
        public static void SetRightDirection(ref Quaternion sOrientation, Vector3 sNewRightDirection)
        {
            // Get the Rotation needed to make our Right face the given Direction
            Quaternion sRotation = Orientation3D.GetRotationTo(Orientation3D.GetRightDirection(sOrientation), sNewRightDirection);

            // Rotate the object
            sOrientation.Normalize();
            sOrientation = sRotation * sOrientation;
        }
        #endregion
    }

    /// <summary>
    /// Class to hold and update an object's 2D Pivot Point (point to rotate around), Pivot Velocity, and 
    /// Pivot Acceleration. This class requires a Position2D object, and optionally a Orientation2D object,
    /// that should be affected by rotations around the Pivot Point.
    /// </summary>
    [Serializable]
    public class PivotPoint2D
    {
        /// <summary>
        /// The 2D Pivot Point that the object should rotate around.
        /// <para>NOTE: This only has effect when Rotational Pivot Velocity / Acceleration are used.</para>
        /// </summary>
        public Vector2 PivotPoint = Vector2.Zero;

        /// <summary>
        /// The object's Rotational Velocity around the Pivot Point (Position change).
        /// <para>NOTE: Roations are specified in radians.</para>
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
            // Call the 2D function to perfrom the operations
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
                // Update the Pivot Rotational Velocity accordint to the Pivot Rotational Acceleration
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

    /// <summary>
    /// Class to hold and update an object's 3D Pivot Point (point to rotate around), Pivot Velocity, and 
    /// Pivot Acceleration. This class requires a Position3D object, and optionally a Orientation3D object, 
    /// that should be affected by rotations around the Pivot Point.
    /// </summary>
    [Serializable]
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
                // Update the Pivot Rotational Velocity accordint to the Pivot Rotational Acceleration
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

    /// <summary>
    /// Class to hold a List of Animations and the texture coordintes of the Pictures used by the Animations.
    /// To start, Create Picture's of all images that will be used in any Animations. Then Create an Animation
    /// by specifying the order of the Picture IDs to go through, and the speed to flip through them at (i.e. frame-rate).
    /// </summary>
    [Serializable]
    public class Animations
    {
        /// <summary>
        /// Structure to store an individual Picture's position and dimensions within a texture
        /// </summary>
        [Serializable]
        struct SPicture
        {
            public int iID;                         // The Unique ID of this Picture (used as its Index in the Pictures List)
            public Rectangle sTextureCoordinates;   // The Position and Dimensions of this Picture in the Texture

            /// <summary>
            /// Explicit constructor
            /// </summary>
            /// <param name="iID">The ID of this Picture (this should be unique)</param>
            /// <param name="sTextureCoordinates">The top-left (x,y) position and (width,height) dimensions
            /// of the Picture within the texture</param>
            public SPicture(int iID, Rectangle sTextureCoordinates)
            {
                this.iID = iID;
                this.sTextureCoordinates = sTextureCoordinates;
            }
        }

        /// <summary>
        /// Class to hold a single Animation's (i.e. Walking, Running, Jumping, etc) sequence of 
        /// Pictures and how long to display each Picture in the Animation for
        /// </summary>
        [Serializable]
        class Animation
        {
            public int miID;                        // The unique ID of this Animation (used as its Index position in the Animation List)
            public List<int> mcPictureRotationOrder;// The Order to Rotate through the Pictures to make the Animation
            public int miCurrentPictureIndex;       // The Index of the Current Picture in the Picture List
            public float mfPictureRotationTime;     // The length of Time to wait before changing to the next Picture in the Animation
            public int miNumberOfTimesToPlay;       // The number of times the Animation should Play (repeats when it reaches the end)
            public int miNumberOfTimesPlayed;       // The number of times the Animation has Played already

            /// <summary>
            /// Explicit Constructor
            /// </summary>
            /// <param name="iID">The ID of this Animation (this should be unique)</param>
            /// <param name="cPictureRotationOrder">A List of Picture ID's which tell the sequence of 
            /// Pictures that make up the Animation</param>
            /// <param name="fPictureRotationTime">How long (in seconds) to wait before switching to the
            /// next Picture in the Picture Rotation Order</param>
            /// <param name="iNumberOfTimesToPlay">The Number of Times the Animation should Play before stopping. A value
            /// of zero means the Animation should repeat forever.</param>
            public Animation(int iID, List<int> cPictureRotationOrder, float fPictureRotationTime, int iNumberOfTimesToPlay)
            {
                miID = iID;
                mcPictureRotationOrder = new List<int>(cPictureRotationOrder);
                miCurrentPictureIndex = 0;
                mfPictureRotationTime = fPictureRotationTime;
                miNumberOfTimesToPlay = iNumberOfTimesToPlay;
                miNumberOfTimesPlayed = 0;
            }

            /// <summary>
            /// Returns the Picture ID of the Current Picture being displayed.
            /// </summary>
            public int CurrentPicturesID
            {
                get { return mcPictureRotationOrder[miCurrentPictureIndex]; }
            }

            /// <summary>
            /// Moves the Current Picture Index to the next element in the Picture Rotation Order, and loops
            /// if it reaches the end of the Animation
            /// </summary>
            public void MoveToNextPictureInAnimation()
            {
                // Increment the position in the Picture Rotation Order
                miCurrentPictureIndex++;

                // If we have reached the end of the Animation
                if (miCurrentPictureIndex >= mcPictureRotationOrder.Count)
                {
                    // If the Animation should Repeat
                    if (miNumberOfTimesToPlay == 0 ||
                        miNumberOfTimesPlayed < miNumberOfTimesToPlay)
                    {
                        // Start back at the beginning of the Animation
                        miCurrentPictureIndex = 0;

                        // Increment the Number of Times this Animation has Played
                        miNumberOfTimesPlayed++;
                    }
                    // Else the Animation shouldn't Repeat
                    else
                    {
                        // Record that the Animation has finished Playing
                        miNumberOfTimesPlayed = miNumberOfTimesToPlay;

                        // NOTE: miCurrentPictureIndex will now be invalid, as it
                        // will be equal to mcPictureRotationOrder.Count
                        miCurrentPictureIndex = mcPictureRotationOrder.Count;
                    }
                }
            }

            /// <summary>
            /// Get if the Animation has finished Playing or not.
            /// NOTE: Animations with Number Of Times To Play == 0 will never end
            /// </summary>
            public bool AnimationHasEnded
            {
                get { return ((miNumberOfTimesPlayed == miNumberOfTimesToPlay) && miNumberOfTimesToPlay != 0); }
            }
        }

        List<SPicture> mcPictures = new List<SPicture>();       // Holds all of the Pictures
        List<Animation> mcAnimations = new List<Animation>();   // Holds all of the Animations
        int miCurrentAnimationID = -1;      // The Index of the Animation that is Current being used
        float mfAnimationFrameTimer = 0.0f; // Used to determine when to move to the next Picture (i.e. Frame) in the Animation
        bool mbPaused = false;              // Tells if the Animation is Paused or not

        /// <summary>
        /// Copies the given Animations data into this Animation
        /// </summary>
        /// <param name="cAnimationToCopy">The Animation to Copy from</param>
        public void CopyFrom(Animations cAnimationToCopy)
        {
            int iIndex = 0;

            // Copy the simple class information
            miCurrentAnimationID = cAnimationToCopy.miCurrentAnimationID;
            mfAnimationFrameTimer = cAnimationToCopy.mfAnimationFrameTimer;

            // Copy the Pictures List
            mcPictures = new List<SPicture>(cAnimationToCopy.mcPictures);

            // Deep Copy the Animations List info (since it contains a reference types)
            int iNumberOfAnimations = cAnimationToCopy.mcAnimations.Count;
            mcAnimations = new List<Animation>(iNumberOfAnimations);
            for (iIndex = 0; iIndex < iNumberOfAnimations; iIndex++)
            {
                Animation cAnimation = new Animation(cAnimationToCopy.mcAnimations[iIndex].miID,
                                                     cAnimationToCopy.mcAnimations[iIndex].mcPictureRotationOrder,
                                                     cAnimationToCopy.mcAnimations[iIndex].mfPictureRotationTime,
                                                     cAnimationToCopy.mcAnimations[iIndex].miNumberOfTimesToPlay);

                mcAnimations.Add(cAnimation);
                mcAnimations[iIndex].miCurrentPictureIndex = cAnimationToCopy.mcAnimations[iIndex].miCurrentPictureIndex;
                mcAnimations[iIndex].miNumberOfTimesPlayed = cAnimationToCopy.mcAnimations[iIndex].miNumberOfTimesPlayed;
            }
        }

        /// <summary>
        /// Creates a Picture that can be used in a Animation, and returns its unique ID. 
        /// A Picture can be used multiple times in an Animation.
        /// </summary>
        /// <param name="sTextureCoordinates">The top-left (x,y) position and (width,height) dimensions
        /// in the Texture that form this Picture</param>
        /// <returns>Returns the new Picture's unique ID.</returns>
        public int CreatePicture(Rectangle sTextureCoordinates)
        {
            // Find where this Picture will be placed in the Pictures List
            int iPictureIndex = mcPictures.Count;

            // Create the new Picture, using the Index as the Pictures ID
            SPicture sPicture = new SPicture(iPictureIndex, sTextureCoordinates);

            // Add the new Picture to the Picture List
            mcPictures.Add(sPicture);

            // Return the unique ID of the new Picture
            return iPictureIndex;
        }

        /// <summary>
        /// Automatically creates the specified Total Number Of Pictures. All pictures are assumed to have 
        /// the same width and height, as specified in the First Picture rectangle. Also, the First Picture
        /// is assumed to be at the top-left corner of the Tileset.
        /// <para>Pictures are created in left-to-right, top-to-bottom order. The ID of the first Picture created
        /// is returned, with each new Picture created incrementing the ID value, so the last Picture created
        /// will have an ID of (returned ID + (Total Number Of Pictures - 1)).</para>
        /// </summary>
        /// <param name="iTotalNumberOfPictures">The Total Number Of Pictures in the Tileset</param>
        /// <param name="iPicturesPerRow">How many Pictures are in a row in the texture</param>
        /// <param name="sFirstPicture">The Position of the top-left Picture in the Tileset, and the
        /// width and height of each Picture in the Tileset</param>
        /// <returns>The ID of the first Picture created
        /// is returned, with each new Picture created incrementing the ID value, so the last Picture created
        /// will have an ID of (returned ID + (Total Number Of Pictures - 1)).</returns>
        public int CreatePicturesFromTileSet(int iTotalNumberOfPictures, int iPicturesPerRow, Rectangle sFirstPicture)
        {
            int iIndex = 0;
            int iLastPictureID = 0;

            // Loop through and create each Picture
            for (iIndex = 0; iIndex < iTotalNumberOfPictures; iIndex++)
            {
                // Calculate which Row and Column this Picture is at within the texture
                int iRow = iIndex / iPicturesPerRow;
                int iColumn = iIndex % iPicturesPerRow;

                // Calculate this Pictures Texture Coordinates
                Rectangle sRect = new Rectangle(sFirstPicture.X + (sFirstPicture.Width * iColumn),
                                                sFirstPicture.Y + (sFirstPicture.Height * iRow),
                                                sFirstPicture.Width, sFirstPicture.Height);

                // Create the new Picture
                iLastPictureID = CreatePicture(sRect);
            }

            // Calculate and return the ID of the first Picture created
            return (iLastPictureID - (iTotalNumberOfPictures - 1));
        }

        /// <summary>
        /// Creates a new Animation and returns the Animation's unique ID.
        /// <para>NOTE: Returns -1 if an invalid Picture ID was specified in the PictureRotationOrder.</para>
        /// <para>NOTE: Be sure to Create the Pictures before creating the Animation.</para>
        /// </summary>
        /// <param name="cPictureRotationOrder">A List of Picture IDs that specifies the Order of Pictures
        /// to Rotate through in order to produce the Animation. A single Picture ID can be used many times.</param>
        /// <param name="fPictureRotationTime">How long (in seconds) to wait before switching to the
        /// next Picture in the Picture Rotation Order (i.e. The frame-rate of the Animation)</param>
        /// <param name="iNumberOfTimesToPlay">The number of times this Animation should be played 
        /// (it replays when the end of the Animation is reached). Specify a value of zero to have the 
        /// Animation repeat forever</param>
        /// <returns>Returns the new Animation's unique ID.</returns>
        public int CreateAnimation(List<int> cPictureRotationOrder, float fPictureRotationTime, int iNumberOfTimesToPlay)
        {
            // Loop through the Picture Rotation Order
            int iNumberOfPictures = cPictureRotationOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfPictures; iIndex++)
            {
                // Get the specified Picture ID
                int iPictureID = cPictureRotationOrder[iIndex];

                // If the Picture ID is invalid
                if (iPictureID < 0 || iPictureID > mcPictures.Count)
                {
                    // Return that an invalid Picture ID was specified
                    return -1;
                }
            }
            
            // Find where this Animation will be placed in the Animations List
            int iAnimationIndex = mcAnimations.Count;

            // Create the new Animation
            Animation cAnimation = new Animation(iAnimationIndex, cPictureRotationOrder, fPictureRotationTime, iNumberOfTimesToPlay);

            // Add the new Animation to the Animations List
            mcAnimations.Add(cAnimation);

            // Return the ID of the new Animation
            return iAnimationIndex;
        }

        /// <summary>
        /// Creates a new Animation and returns the Animation's unique ID.
        /// <para>NOTE: Returns -1 if an invalid Picture ID was specified in the PictureRotationOrder.</para>
        /// <para>NOTE: Be sure to Create the Pictures before creating the Animation.</para>
        /// </summary>
        /// <param name="iaPictureRotationOrder">An array of Picture IDs that specifies the Order of Pictures
        /// to Rotate through in order to produce the Animation</param>
        /// <param name="fPictureRotationTime">How long (in seconds) to wait before switching to the
        /// next Picture in the Picture Rotation Order (i.e. The next Frame in the Animation)</param>
        /// <param name="iNumberOfTimesToPlay">The number of times this Animation should be played 
        /// (it replays when the end of the Animation is reached). Specify a value of zero to have the 
        /// Animation repeat forever</param>
        /// <returns>Returns the new Animation's unique ID.
        /// NOTE: Returns -1 if an invalid Picture ID was specified in the PictureRotationOrder.</returns>
        public int CreateAnimation(int[] iaPictureRotationOrder, float fPictureRotationTime, int iNumberOfTimesToPlay)
        {
            int iIndex = 0;
            
            // Get the Number of elements in the Picture Rotation Order array
            int iSizeOfArray = iaPictureRotationOrder.Length;

            // List to hold all of the Picture Rotation Order values
            List<int> cPictureRotationOrder = new List<int>(iSizeOfArray);

            // Loop through each of the elements in the Picture Rotation Order array and store them in the List
            for (iIndex = 0; iIndex < iSizeOfArray; iIndex++)
            {
                cPictureRotationOrder.Add(iaPictureRotationOrder[iIndex]);
            }

            // Create the Animation
            return CreateAnimation(cPictureRotationOrder, fPictureRotationTime, iNumberOfTimesToPlay);
        }

        /// <summary>
        /// Returns true if the given Picture ID is valid (i.e. A Picture with the same ID exists).
        /// </summary>
        /// <param name="iPictureID">The Picture ID to look for</param>
        /// <returns>Returns true if the given Picture ID is valid (i.e. A Picture with the same ID exists).</returns>
        public bool PictureIDIsValid(int iPictureID)
        {
            return (iPictureID >= 0 && iPictureID < mcPictures.Count);
        }

        /// <summary>
        /// Returns true if the given Animation ID is valid (i.e. An Animation with the same ID exists).
        /// </summary>
        /// <param name="iAnimationID">The Animation ID to look for</param>
        /// <returns>Returns true if the given Animation ID is valid (i.e. An Animation with the same ID exists).</returns>
        public bool AnimationIDIsValid(int iAnimationID)
        {
            return (iAnimationID >= 0 && iAnimationID < mcAnimations.Count);
        }

        /// <summary>
        /// Get / Set the Current Animation being used. The Animation is started at its beginning.
        /// <para>NOTE: If an invalid Animiation ID is given when Setting, the Animation will not be changed.</para>
        /// <para>NOTE: If an Animation has not beeng set yet when Getting, -1 is returned.</para>
        /// </summary>
        public int CurrentAnimationID
        {
            get { return miCurrentAnimationID; }
            set
            {
                // Temporarily store the given Animation Index
                int iNewAnimationIndex = value;

                // If the given Animation Index is valid
                if (iNewAnimationIndex < mcAnimations.Count && iNewAnimationIndex >= 0)
                {
                    // Use the new Animation
                    miCurrentAnimationID = iNewAnimationIndex;

                    // Start from the beginning of the Animation
                    mcAnimations[miCurrentAnimationID].miCurrentPictureIndex = 0;
                    mcAnimations[miCurrentAnimationID].miNumberOfTimesPlayed = 0;

                    // Reset the Animation Timer
                    mfAnimationFrameTimer = 0.0f;
                }
            }
        }

        /// <summary>
        /// Sets the Current Animation being used, as well as what index in the Animation's Picture Rotation
        /// Order the Animation should start at. 
        /// <para>NOTE: If the specified Animiation to use is not valid, the Current Animation will not be 
        /// changed, and if the specified Picture Rotation Order Index is not valid, the Animation will 
        /// start from the beginning of the Animation.</para>
        /// </summary>
        /// <param name="iAnimationID">The ID of the Animation to use</param>
        /// <param name="iPictureRotationOrderIndex">The Index in the Animation's Picture Rotation Order
        /// that the Animation should begin playing from</param>
        public void SetCurrentAnimationAndPositionInAnimation(int iAnimationID, int iPictureRotationOrderIndex)
        {
            // If the given Animation Index is valid
            if (AnimationIDIsValid(iAnimationID))
            {
                // Use the new Animation
                miCurrentAnimationID = iAnimationID;

                // If the specified Picture Rotation Order Index is valid
                if (iPictureRotationOrderIndex >= 0 &&
                    iPictureRotationOrderIndex < mcAnimations[miCurrentAnimationID].mcPictureRotationOrder.Count)
                {
                    // Start the Animation at the specified position in the Picture Rotation Order
                    mcAnimations[miCurrentAnimationID].miCurrentPictureIndex = iPictureRotationOrderIndex;
                }
                // Else the specified Picture Rotation Order is not valid
                else
                {
                    // So just start at the beginning of the Animation
                    mcAnimations[miCurrentAnimationID].miCurrentPictureIndex = 0;
                }

                // Mark the Animation as not having Played at all yet
                mcAnimations[miCurrentAnimationID].miNumberOfTimesPlayed = 0;

                // Reset the Animation Timer
                mfAnimationFrameTimer = 0.0f;
            }
        }

        /// <summary>
        /// Returns how much Time (in seconds) should elapse before switching frames in the Animation.
        /// <para>NOTE: Returns zero if the specified Animation ID is not valid.</para>
        /// </summary>
        /// <param name="iAnimationID">The ID of the Animation containing the Picture Rotation Time to retrive</param>
        /// <returns>Returns how much Time (in seconds) should elapse before switching frames in the Animation.
        /// NOTE: Returns zero if the specified Animation ID is not valid.</returns>
        public float GetAnimationsPictureRotationTime(int iAnimationID)
        {
            // Variable to hold the Animation's Picture Rotation Time
            float fPictureRotationTime = 0.0f;

            // If the specified Animation ID is valid
            if (AnimationIDIsValid(iAnimationID))
            {
                // Get the Animations Picture Rotation Time
                fPictureRotationTime =  mcAnimations[iAnimationID].mfPictureRotationTime;
            }
            
            // Return the Picture Rotation Time
            return fPictureRotationTime;
        }

        /// <summary>
        /// Sets how much Time should elapse before switching frames in the Animation
        /// </summary>
        /// <param name="iAnimationID">The ID of the Animation to update</param>
        /// <param name="fNewPictureRotationTime">The Time (in seconds) to wait before moving to the
        /// next Picture in the Animations Picture Rotation Order</param>
        public void SetAnimationsPictureRotationTime(int iAnimationID, float fNewPictureRotationTime)
        {
            // If the specified Animation ID is valid
            if (AnimationIDIsValid(iAnimationID))
            {
                // Set the Animations new Picture Rotation Time
                mcAnimations[iAnimationID].mfPictureRotationTime = fNewPictureRotationTime;
            }
        }

        /// <summary>
        /// Get / Set how much Time should elapsed before switching frames in the Current Animation. 
        /// <para>NOTE: If no Animation has been set yet, zero will be returned.</para>
        /// </summary>
        public float CurrentAnimationsPictureRotationTime
        {
            get { return GetAnimationsPictureRotationTime(miCurrentAnimationID); }
            set { SetAnimationsPictureRotationTime(miCurrentAnimationID, value); }
        }

        /// <summary>
        /// Get / Set the Current Index in the Current Animation's Picture Rotation Order. 
        /// <para>NOTE: If no Animation has been set yet, Get returns -1, and Set doesn't change anything 
        /// (as well as if the specified Index is invalid).</para>
        /// </summary>
        public int CurrentAnimationsPictureRotationOrderIndex
        {
            get
            {
                // If there is a Current Animation
                if (CurrentAnimationIsValid)
                {
                    // Return the Current position in the Picture Rotation Order
                    return mcAnimations[miCurrentAnimationID].miCurrentPictureIndex;
                }
                // Else no Animation has been set yet
                else
                {
                    return -1;
                }
            }

            set
            {
                // Temporarily store the given Index to use
                int iNewPictureRotationOrderIndex = value;

                // If there is a Current Animation
                if (CurrentAnimationIsValid)
                {
                    // If the new Index is valid
                    if (iNewPictureRotationOrderIndex >= 0 &&
                        iNewPictureRotationOrderIndex < mcAnimations[miCurrentAnimationID].mcPictureRotationOrder.Count)
                    {
                        // Set the New Current position in the Picture Rotation Order
                        mcAnimations[miCurrentAnimationID].miCurrentPictureIndex = iNewPictureRotationOrderIndex;

                        // Reset the Animation Timer
                        mfAnimationFrameTimer = 0.0f;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the Number of times the given Animation ID is set to Play.
        /// Zero means the Animation will repeat forever.
        /// <para>NOTE: If the given Animation ID is invalid, -1 is returned.</para>
        /// </summary>
        /// <param name="iAnimationID">The ID of the Animation to update</param>
        /// <returns>Returns the Number of times the given Animation ID is set to Play.
        /// Zero means the Animation will repeat forever.
        /// NOTE: If the given Animation ID is invalid, -1 is returned.</returns>
        public int GetAnimationsNumberOfTimesToPlay(int iAnimationID)
        {
            // Variable to hold the Animation's Number Of Times To Play
            int iNumberOfTimesToPlay = -1;

            // If the specified Animation ID is valid
            if (AnimationIDIsValid(iAnimationID))
            {
                // Get the Animations Number Of Times To Play
                iNumberOfTimesToPlay = mcAnimations[iAnimationID].miNumberOfTimesToPlay;
            }

            // Return the Number Of Times To Play
            return iNumberOfTimesToPlay;
        }

        /// <summary>
        /// Sets the Number of times the given Animation ID should Play
        /// (it replays when the end of the Animation is reached). 
        /// Specify a value of zero to have the Animation repeat forever.
        /// <para>NOTE: If the given Animation ID is invalid, no changes are made.</para>
        /// </summary>
        /// <param name="iAnimationID">The ID of the Animation to update</param>
        /// <param name="iNewNumberOfTimesToPlay">The New Number of times the Animation should Play</param>
        public void SetAnimationsNumberOfTimesToPlay(int iAnimationID, int iNewNumberOfTimesToPlay)
        {
            // If the specified Animation ID is valid
            if (AnimationIDIsValid(iAnimationID))
            {
                // Set the Animations Number Of Times To Play
                mcAnimations[iAnimationID].miNumberOfTimesToPlay = iNewNumberOfTimesToPlay;
            }
        }

        /// <summary>
        /// Get / Set the Number of times the Current Animation should Play
        /// (it replays when the end of the Animation is reached). 
        /// Specify a value of zero to have the Animation repeat forever.
        /// <para>NOTE: If no Animation has been set yet, no changes are made when
        /// Setting, and -1 is returned when Getting.</para>
        /// </summary>
        public int CurrentAnimationsNumberOfTimesToPlay
        {
            get { return GetAnimationsNumberOfTimesToPlay(miCurrentAnimationID); }
            set { SetAnimationsNumberOfTimesToPlay(miCurrentAnimationID, value); }
        }

        /// <summary>
        /// Get / Set the Number of times the Current Animation has Played already.
        /// <para>NOTE: If no Animation has been set yet, Get returns -1, and Set doesn't change anything.</para>
        /// </summary>
        public int CurrentAnimationsNumberOfTimesPlayed
        {
            get
            {
                // If there is a Current Animation
                if (CurrentAnimationIsValid)
                {
                    // Return the Current Animations Number of Times Played
                    return mcAnimations[miCurrentAnimationID].miNumberOfTimesPlayed;
                }
                // Else no Animation has been set yet
                else
                {
                    return -1;
                }
            }

            set
            {
                // Temporarily store the given Number of Times Played
                int iNewNumberOfTimesPlayed = value;

                // If there is a Current Animation
                if (CurrentAnimationIsValid)
                {
                    // Set the New Number Of Times the Current Animation has already Played
                    mcAnimations[miCurrentAnimationID].miNumberOfTimesPlayed = iNewNumberOfTimesPlayed;
                }
            }
        }

        /// <summary>
        /// Get if the Current Animation is Done Playing or not (i.e. Its Number Of Times Played is
        /// greater than or equal to its Number Of Times To Play). Returns true even if no
        /// Animation has been set to Play yet.
        /// </summary>
        public bool CurrentAnimationIsDonePlaying
        {
            get
            {
                // If there is a Current Animation
                if (CurrentAnimationIsValid)
                {
                    // Get how many Times the Current Animation should Play
                    int iNumberOfTimesToPlay = CurrentAnimationsNumberOfTimesToPlay;

                    // Return if the Current Animation has played the specified number of times already
                    // or not, and it's not set to play forever
                    return (iNumberOfTimesToPlay != 0 && CurrentAnimationsNumberOfTimesPlayed >= iNumberOfTimesToPlay);
                }
                // Else no Animation has been set yet
                else
                {
                    // So return that the Current Animatino is done Playing (since there isn't one)
                    return true;
                }
            }
        }

        /// <summary>
        /// Returns the amount of Time required to play the specified Animation.
        /// <para>NOTE: If an invalid AnimationID is specified, zero is returned.</para>
        /// </summary>
        /// <param name="iAnimationID">The ID of the Animation to check</param>
        /// <returns>Returns the amount of Time required to play the specified Animation.
        /// NOTE: If an invalid AnimationID is specified, zero is returned.</returns>
        public float TimeRequiredToPlayAnimation(int iAnimationID)
        {
            // If the specified Animation ID is valid
            if (AnimationIDIsValid(iAnimationID))
            {
                // Get a handle to the Animation to check
                Animation cAnimation = mcAnimations[iAnimationID];

                // Return how long it will take to Play the specified Animation
                return (cAnimation.mcPictureRotationOrder.Count * cAnimation.mfPictureRotationTime * cAnimation.miNumberOfTimesToPlay);
            }
            // Else the AnimationID was not valid, so return zero
            return 0.0f;
        }

        /// <summary>
        /// Gets the amount of Time (in seconds) required to play the Current Animation.
        /// <para>NOTE: If no Animation has been played yet, zero is returned.</para>
        /// </summary>
        public float TimeRequiredToPlayCurrentAnimation
        {
            get { return TimeRequiredToPlayAnimation(miCurrentAnimationID); }
        }

        /// <summary>
        /// Gets the amount of Time (in seconds) required to play the remainder of the Current Animation.
        /// <para>NOTE: If no Animation has been played yet, zero is returned.</para>
        /// </summary>
        public float TimeRequiredToPlayTheRestOfTheCurrentAnimation
        {
            get
            {
                // If the specified Animation ID is valid
                if (CurrentAnimationIsValid)
                {
                    // Get a handle to the Animation to check
                    Animation cAnimation = mcAnimations[miCurrentAnimationID];

                    // Calculate how much time is left in the current frame
                    float fFrameRemainderTime = cAnimation.mfPictureRotationTime - mfAnimationFrameTimer;

                    // Calculate how much time is left to finish the animation without repeats
                    float fNoRepeatRemainderTime = ((cAnimation.mcPictureRotationOrder.Count - (cAnimation.miCurrentPictureIndex + 1)) * cAnimation.mfPictureRotationTime) + fFrameRemainderTime;

                    // Calculate how much time it will take to play all of the repeats
                    float fRepeatsRemainingTime = (cAnimation.mcPictureRotationOrder.Count * cAnimation.mfPictureRotationTime) * (cAnimation.miNumberOfTimesToPlay - (cAnimation.miNumberOfTimesPlayed + 1));

                    // Return how long it will take to Play the remainder of the Animation
                    return (fNoRepeatRemainderTime + fRepeatsRemainingTime);
                }
                // Else no Animation has been set yet
                // So return zero
                return 0.0f;
            }
        }

        /// <summary>
        /// Returns the Rectangle representing the Texture Coordinates of the specified Picture.
        /// </summary>
        /// <param name="iPictureID">The Picture ID of the Picture whose Texture Coordinates 
        /// should be retrieved</param>
        /// <returns>Returns the Rectangle representing the Texture Coordinates of the specified Picture.</returns>
        public Rectangle GetPicturesTextureCoordinates(int iPictureID)
        {
            // The Rectangle containing the Texture Coordinates to be returned
            Rectangle cTextureCoordinates = new Rectangle();

            // If the Picture ID is valid
            if (iPictureID >= 0 && iPictureID < mcPictures.Count)
            {
                // Get the specified Picture's Texture Coordinates
                cTextureCoordinates = mcPictures[iPictureID].sTextureCoordinates;
            }

            // Return the Texture Coordinates
            return cTextureCoordinates;
        }

        /// <summary>
        /// Get the Rectangle representing the Texture Coordinates of the Picture 
        /// in the Animation that should be displayed at this point in time
        /// </summary>
        public Rectangle CurrentPicturesTextureCoordinates
        {
            get
            {
                // Temp variable to hold the Current Animation's Picture rectangle
                Rectangle sRect = new Rectangle();

                // If there is a Current Animation and it is not Done Playing yet
                if (CurrentAnimationIsValid && !CurrentAnimationIsDonePlaying)
                {
                    // Store the Rectangle 
                    sRect = mcPictures[mcAnimations[miCurrentAnimationID].CurrentPicturesID].sTextureCoordinates;
                }
                
                // Return the Rectangle containing the Texture Coordinates to use
                return sRect;
            }
        }

        /// <summary>
        /// Get / Set if the Animation should be Paused or not. If Paused, the Animation will
        /// not be Updated.
        /// </summary>
        public bool Paused
        {
            get { return mbPaused; }
            set { mbPaused = value; }
        }

        /// <summary>
        /// Updates the Animation according to how much time has elapsed
        /// </summary>
        /// <param name="fElapsedTime">The amount of Time (in seconds) since the last Update</param>
        public void Update(float fElapsedTime)
        {
            // If the Animation is Paused or no Animation has been set yet
            if (Paused || !CurrentAnimationIsValid)
            {
                // Do nothing
                return;
            }
            // Else an Animation should be playing

            // Add the Elapsed Time to the Animation Timer
            mfAnimationFrameTimer += fElapsedTime;

            // If it's time to move to the next Picture in the Animation
            if (mfAnimationFrameTimer >= mcAnimations[miCurrentAnimationID].mfPictureRotationTime)
            {
                // Subtract the Animation's Picture Rotation Time from the Animation Timer
                mfAnimationFrameTimer -= mcAnimations[miCurrentAnimationID].mfPictureRotationTime;

                // Move to the next Picture in the Animation
                mcAnimations[miCurrentAnimationID].MoveToNextPictureInAnimation();
            }
        }

        /// <summary>
        /// Get if the Current Animation has been set yet or not
        /// </summary>
        private bool CurrentAnimationIsValid
        {
            get { return AnimationIDIsValid(miCurrentAnimationID); }
        }
    }

    #endregion
}
