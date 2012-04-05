using Microsoft.Xna.Framework;

namespace BasicVirtualEnvironment.View
{
	/// <summary>
	/// Class to hold the Camera's position and orientation.
	/// </summary>
	public class Camera
	{
		public Vector3 sVRP, cVPN, cVUP, cVLeft;	// Free Camera Variables (View Reference Point, View Plane Normal, View Up, and View Left).

		public float fCameraArc, fCameraRotation, fCameraDistance;	// Fixed Camera Variables.
		public Vector3 sFixedCameraLookAtPosition;					// The Position that the Fixed Camera should rotate around.
		public bool bUsingFixedCamera;								// Variable indicating which type of Camera to use.

		/// <summary>
		/// Explicit constructor
		/// </summary>
		/// <param name="bUseFixedCamera">True to use the Fixed Camera, false to use the Free Camera</param>
		public Camera(bool bUseFixedCamera)
		{
			// Initialize variables with dummy values so we can call the Reset functions
			sVRP = cVPN = cVUP = cVLeft = sFixedCameraLookAtPosition = Vector3.Zero;
			fCameraArc = fCameraRotation = fCameraDistance = 0.0f;

			// Use the specified Camera type
			bUsingFixedCamera = bUseFixedCamera;

			// Initialize the variables with their proper values
			ResetFreeCameraVariables();
			ResetFixedCameraVariables();
		}

		/// <summary>
		/// Get the current Position of the Camera
		/// </summary>
		public Vector3 Position
		{
			get
			{
				// If we are using the Fixed Camera
				if (bUsingFixedCamera)
				{
					// Calculate the View Matrix
					Matrix cViewMatrix = Matrix.CreateTranslation(sFixedCameraLookAtPosition)*
					                     Matrix.CreateRotationY(MathHelper.ToRadians(fCameraRotation))*
					                     Matrix.CreateRotationX(MathHelper.ToRadians(fCameraArc))*
					                     Matrix.CreateLookAt(new Vector3(0, 0, -fCameraDistance),
					                                         new Vector3(0, 0, 0), Vector3.Up);

					// Invert the View Matrix
					cViewMatrix = Matrix.Invert(cViewMatrix);

					// Pull and return the Camera Coordinates from the inverted View Matrix
					return cViewMatrix.Translation;
				}
					// Else we are using the Free Camera
				else
				{
					return sVRP;
				}
			}
		}

		/// <summary>
		/// Reset the Fixed Camera Variables to their default values
		/// </summary>
		public void ResetFixedCameraVariables()
		{
			fCameraArc = 0.0f;
			fCameraRotation = 180.0f;
			fCameraDistance = 300.0f;
			sFixedCameraLookAtPosition = new Vector3(0, -50, 0);
		}

		/// <summary>
		/// Reset the Free Camera Variables to their default values
		/// </summary>
		public void ResetFreeCameraVariables()
		{
			sVRP = new Vector3(0.0f, 50.0f, 300.0f);
			cVPN = Vector3.Forward;
			cVUP = Vector3.Up;
			cVLeft = Vector3.Left;
		}

		/// <summary>
		/// Normalize the Camera Directions and maintain proper Right and Up directions
		/// </summary>
		public void NormalizeCameraAndCalculateProperUpAndRightDirections()
		{
			// Calculate the new Right and Up directions
			cVPN.Normalize();
			cVLeft = Vector3.Cross(cVUP, cVPN);
			cVLeft.Normalize();
			cVUP = Vector3.Cross(cVPN, cVLeft);
			cVUP.Normalize();
		}

		/// <summary>
		/// Move the Camera Forward or Backward
		/// </summary>
		/// <param name="fAmountToMove">The distance to Move</param>
		public void MoveCameraForwardOrBackward(float fAmountToMove)
		{
			cVPN.Normalize();
			sVRP += (cVPN * fAmountToMove);
		}

		/// <summary>
		/// Move the Camera Horizontally
		/// </summary>
		/// <param name="fAmountToMove">The distance to move horizontally</param>
		public void MoveCameraHorizontally(float fAmountToMove)
		{
			cVLeft.Normalize();
			sVRP += (cVLeft * fAmountToMove);
		}

		/// <summary>
		/// Move the Camera Vertically
		/// </summary>
		/// <param name="fAmountToMove">The distance to move Vertically</param>
		public void MoveCameraVertically(float fAmountToMove)
		{
			// Move the Camera along the global Y axis
			sVRP.Y += fAmountToMove;
		}

		/// <summary>
		/// Rotate the Camera Horizontally
		/// </summary>
		/// <param name="fAmountToRotateInRadians">The amount to Rotate in radians</param>
		public void RotateCameraHorizontally(float fAmountToRotateInRadians)
		{
			// Rotate the Camera about the global Y axis
			Matrix cRotationMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, fAmountToRotateInRadians);
			cVPN = Vector3.Transform(cVPN, cRotationMatrix);
			cVUP = Vector3.Transform(cVUP, cRotationMatrix);

			// Normalize all of the Camera directions since they have changed
			NormalizeCameraAndCalculateProperUpAndRightDirections();
		}

		/// <summary>
		/// Rotate the Camera Vertically
		/// </summary>
		/// <param name="fAmountToRotateInRadians">The amount to Rotate in radians</param>
		public void RotateCameraVertically(float fAmountToRotateInRadians)
		{
			// Rotate the Camera
			Matrix cRotationMatrix = Matrix.CreateFromAxisAngle(cVLeft, fAmountToRotateInRadians);
			cVPN = Vector3.Transform(cVPN, cRotationMatrix);
			cVUP = Vector3.Transform(cVUP, cRotationMatrix);

			// Normalize all of the Camera directions since they have changed
			NormalizeCameraAndCalculateProperUpAndRightDirections();
		}
	}
}
