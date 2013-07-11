//===================================================================
// DPSF Tutorial X Source Code - This tutorial shows how to...
//
// Tutorial files of interest:
//  Game1.cs
//  Particle Systems/MyParticleSystem.cs
//===================================================================

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Include the DPSF and DPSF.ParticleSystems namespaces
using DPSF;
using DPSF.ParticleSystems;
#endregion

namespace Tutorial
{
	/// <summary>
	/// Application class showing how to use Dynamic Particle System Framework
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		// Specify the Window's Title
		string sWindowTitle = "Tutorial 7 - Magnets";

		#region Non DPSF Related Fields

		//===========================================================
		// Class Structures and Variables
		//===========================================================

		// Structure to hold the Camera's position and orientation
		struct SCamera
		{
			public Vector3 sVRP, cVPN, cVUP, cVLeft;                    // Free Camera Variables (View Reference Point, View Plane Normal, View Up, and View Left)
			public float fCameraArc, fCameraRotation, fCameraDistance;  // Fixed Camera Variables
			public Vector3 sFixedCameraLookAtPosition;                  // The Position that the Fixed Camera should rotate around
			public bool bUsingFixedCamera;  // Variable indicating which type of Camera to use

			/// <summary>
			/// Explicit constructor
			/// </summary>
			/// <param name="bUseFixedCamera">True to use the Fixed Camera, false to use the Free Camera</param>
			public SCamera(bool bUseFixedCamera)
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
						Matrix cViewMatrix = Matrix.CreateTranslation(sFixedCameraLookAtPosition) *
									 Matrix.CreateRotationY(MathHelper.ToRadians(fCameraRotation)) *
									 Matrix.CreateRotationX(MathHelper.ToRadians(fCameraArc)) *
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

		// Specify the Width and Height of the Window
		const int miWINDOW_WIDTH = 800;
		const int miWINDOW_HEIGHT = 600;

		GraphicsDeviceManager mcGraphics;       // Handle to the Graphics object

		SpriteBatch mcSpriteBatch;              // Batch used to draw Sprites
		SpriteFont mcFont;                      // Font used to draw text

		Model mcFloorModel;                     // Model of the Floor

		bool mbShowFloor = true;                // Tells if the Floor should be visible or not
		bool mbShowText = true;                 // Tells if Text should be displayed or not

		Random mcRandom = new Random();         // Random number generator

		// Input States
		KeyboardState mcCurrentKeyboardState;   // Holds the Keyboard's Current State
		KeyboardState mcPreviousKeyboardState;  // Holds the Keyboard's Previous State
		MouseState mcCurrentMouseState;         // Holds the Mouse's Current State
		MouseState mcPreviousMouseState;        // Holds the Mouse's Previous State

		TimeSpan mcInputTimeSpan = new TimeSpan();  // Tells how long since the last button press

		// The World, View, and Projection matrices
		Matrix msWorldMatrix = Matrix.Identity;
		Matrix msViewMatrix = Matrix.Identity;
		Matrix msProjectionMatrix = Matrix.Identity;

		// Initialize the Camera
		SCamera msCamera = new SCamera(true);

		#endregion

		#region DSPF Related Fields

		// Declare a Particle System variable
		MyParticleSystem mcMyParticleSystem = null;

		// Declare the Particle System Manager to manage the Particle Systems
		ParticleSystemManager mcParticleSystemManager = new ParticleSystemManager();

		#endregion

		#region Initialization

		/// <summary>
		/// Constructor
		/// </summary>
		public Game1()
		{
			mcGraphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			// Make the game run as fast as possible (i.e. don't limit the FPS)
			//this.IsFixedTimeStep = false;
			//mcGraphics.SynchronizeWithVerticalRetrace = false;

			// Set the resolution
			mcGraphics.PreferredBackBufferWidth = miWINDOW_WIDTH;
			mcGraphics.PreferredBackBufferHeight = miWINDOW_HEIGHT;

			// Set the Title of the Window
			Window.Title = sWindowTitle;
		}

		/// <summary>
		/// Load your graphics content
		/// </summary>
		protected override void LoadContent()
		{
			mcSpriteBatch = new SpriteBatch(mcGraphics.GraphicsDevice);

			// Load fonts and models for test application
			mcFont = Content.Load<SpriteFont>("Fonts/font");
			mcFloorModel = Content.Load<Model>("grid");

			// Instance and Initialize the Particle System
			mcMyParticleSystem = new MyParticleSystem(this);
			mcMyParticleSystem.AutoInitialize(GraphicsDevice, Content, null);

			// Add the Particle System to the Particle System Manager
			mcParticleSystemManager.AddParticleSystem(mcMyParticleSystem);
		}

		#endregion

		#region Update and Draw

		/// <summary>
		/// Allows the game to run logic
		/// </summary>
		protected override void Update(GameTime cGameTime)
		{
			// Get and process user Input
			ProcessInput(cGameTime);

			// Update the World, View, and Projection matrices
			UpdateWorldViewProjectionMatrices();

			// Set the World, View, and Projection Matrices for the Particle Systems before Drawing them
			mcParticleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(msWorldMatrix, msViewMatrix, msProjectionMatrix);

			// Let all of the particle systems know of the Camera's current position
			mcParticleSystemManager.SetCameraPositionForAllParticleSystems(msCamera.Position);

			// Update all of the Particle Systems
			mcParticleSystemManager.UpdateAllParticleSystems((float)cGameTime.ElapsedGameTime.TotalSeconds);

			// Update any other Drawable Game Components
			base.Update(cGameTime);
		}

		/// <summary>
		/// Updates the World, View, and Projection matrices according to the Camera's current position.
		/// </summary>
		private void UpdateWorldViewProjectionMatrices()
		{
			GraphicsDevice cGraphicsDevice = mcGraphics.GraphicsDevice;

			// Compute the Aspect Ratio of the window
			float fAspectRatio = (float)cGraphicsDevice.Viewport.Width / (float)cGraphicsDevice.Viewport.Height;

			// Setup the View matrix depending on which Camera mode is being used
			// If we are using the Fixed Camera
			if (msCamera.bUsingFixedCamera)
			{
				// Set up the View matrix according to the Camera's arc, rotation, and distance from the Offset position
				msViewMatrix = Matrix.CreateTranslation(msCamera.sFixedCameraLookAtPosition) *
									 Matrix.CreateRotationY(MathHelper.ToRadians(msCamera.fCameraRotation)) *
									 Matrix.CreateRotationX(MathHelper.ToRadians(msCamera.fCameraArc)) *
									 Matrix.CreateLookAt(new Vector3(0, 0, -msCamera.fCameraDistance),
														 new Vector3(0, 0, 0), Vector3.Up);
			}
			// Else we are using the Free Camera
			else
			{
				// Set up our View matrix specifying the Camera position, a point to look-at, and a direction for which way is up
				msViewMatrix = Matrix.CreateLookAt(msCamera.sVRP, msCamera.sVRP + msCamera.cVPN, msCamera.cVUP);
			}

			// Setup the Projection matrix by specifying the field of view (1/4 pi), aspect ratio, and the near and far clipping planes
			msProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, fAspectRatio, 1, 10000);
		}

		/// <summary>
		/// This is called when the game should draw itself
		/// </summary>
		protected override void Draw(GameTime cGameTime)
		{
			// Get a handle to the Graphics Device used for drawing
			GraphicsDevice cGraphicsDevice = mcGraphics.GraphicsDevice;

			// Clear the scene
			cGraphicsDevice.Clear(Color.Black);

			// Draw the Floor and any other models
			DrawModels(msWorldMatrix, msViewMatrix, msProjectionMatrix);

			// Draw any other Drawable Game Components that may need to be drawn.
			// Call this before drawing our Particle Systems, so that our 2D Sprite particles
			// show up ontop of the any other 2D Sprites drawn.
			base.Draw(cGameTime);


			// Draw the Particle Systems
			mcParticleSystemManager.DrawAllParticleSystems();


			// Update the Frames Per Second to be displayed
			FPS.Update((float)cGameTime.ElapsedGameTime.TotalSeconds);

			// Draw the Text to the screen last, so it is always on top
			DrawText();
		}

		/// <summary>
		/// Helper for drawing the Models
		/// </summary>
		void DrawModels(Matrix cWorldMatrix, Matrix cViewMatrix, Matrix cProjectionMatrix)
		{
			// Set our sampler state to allow the ground to have a repeated texture
			GraphicsDevice.BlendState = BlendState.Opaque;
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

			// If the Floor should be drawn
			if (mbShowFloor)
			{
				mcFloorModel.Draw(cWorldMatrix, cViewMatrix, cProjectionMatrix);
			}
		}

		/// <summary>
		/// Function to draw Text to the screen
		/// </summary>
		void DrawText()
		{
			// If Text should not be Shown
			if (!mbShowText)
			{
				// Exit the function without drawing anything
				return;
			}

			// Specify the text Colors to use
			Color sPropertyColor = Color.WhiteSmoke;
			Color sValueColor = Color.Yellow;
			Color sControlColor = Color.PowderBlue;

			string sFPS = FPS.CurrentFPS.ToString();
			string sTotalNumberOfParticles = mcParticleSystemManager.TotalNumberOfActiveParticles.ToString();
			string sEmitterOnValue = (mcMyParticleSystem.Emitter.EmitParticlesAutomatically ? "On" : "Off");
			string sParticlesPerSecond = mcMyParticleSystem.Emitter.ParticlesPerSecond.ToString();

			// Draw all of the text
			mcSpriteBatch.Begin();

			mcSpriteBatch.DrawString(mcFont, "Toggle Text:", new Vector2(miWINDOW_WIDTH - 150, 2), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "F1", new Vector2(miWINDOW_WIDTH - 30, 2), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Toggle Floor:", new Vector2(miWINDOW_WIDTH - 150, 25), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "F", new Vector2(miWINDOW_WIDTH - 30, 25), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Reset Camera:", new Vector2(miWINDOW_WIDTH - 170, 50), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "R", new Vector2(miWINDOW_WIDTH - 30, 50), sControlColor);


			mcSpriteBatch.DrawString(mcFont, "Move Emitter:", new Vector2(5, 2), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "A/D, W/S, Q/E", new Vector2(135, 2), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Rotate Emitter:", new Vector2(5, 25), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "J/L(yaw), I/Vertex(pitch), U/O(roll)", new Vector2(150, 25), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Rotate Emitter Around Pivot:", new Vector2(5, 50), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "Y + Rotate Emitter", new Vector2(275, 50), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Reset Emitter Position/Orientation:", new Vector2(5, 75), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "Z", new Vector2(320, 75), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Increase/Decrease Emitter Speed:", new Vector2(5, 100), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "+ / -", new Vector2(320, 100), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Toggle Emitter:", new Vector2(5, 125), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "Delete", new Vector2(145, 125), sControlColor);


			mcSpriteBatch.DrawString(mcFont, "FPS: ", new Vector2(5, miWINDOW_HEIGHT - 20), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, sFPS, new Vector2(50, miWINDOW_HEIGHT - 20), sValueColor);

			mcSpriteBatch.DrawString(mcFont, "Particles: ", new Vector2(130, miWINDOW_HEIGHT - 20), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, sTotalNumberOfParticles, new Vector2(215, miWINDOW_HEIGHT - 20), sValueColor);

			mcSpriteBatch.DrawString(mcFont, "Emitter: ", new Vector2(325, miWINDOW_HEIGHT - 20), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, sEmitterOnValue, new Vector2(400, miWINDOW_HEIGHT - 20), sValueColor);

			mcSpriteBatch.DrawString(mcFont, "Particles Per Second: ", new Vector2(480, miWINDOW_HEIGHT - 20), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, sParticlesPerSecond, new Vector2(680, miWINDOW_HEIGHT - 20), sValueColor);



			int iYPosition = 400;

			mcSpriteBatch.DrawString(mcFont, "Point Magnet: ", new Vector2(5, iYPosition + 25), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "C", new Vector2(130, iYPosition + 25), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Line Magnet: ", new Vector2(5, iYPosition + 50), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "V", new Vector2(120, iYPosition + 50), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Line Segment Magnet: ", new Vector2(5, iYPosition + 75), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "B", new Vector2(210, iYPosition + 75), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Plane Magnet: ", new Vector2(5, iYPosition + 100), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "N", new Vector2(135, iYPosition + 100), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Magnet Type: ", new Vector2(320, iYPosition + 100), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, mcMyParticleSystem.MagnetList[0].MagnetType.ToString(), new Vector2(450, iYPosition + 100), sValueColor);

			mcSpriteBatch.DrawString(mcFont, "Toggle Distance Function: ", new Vector2(5, iYPosition + 125), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "M", new Vector2(240, iYPosition + 125), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Function: ", new Vector2(320, iYPosition + 125), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, mcMyParticleSystem.MagnetsDistanceFunction.ToString(), new Vector2(400, iYPosition + 125), sValueColor);

			mcSpriteBatch.DrawString(mcFont, "Increase / Decrease Force: ", new Vector2(5, iYPosition + 150), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "G / T", new Vector2(255, iYPosition + 150), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Force: ", new Vector2(320, iYPosition + 150), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, mcMyParticleSystem.MagnetsForce.ToString(), new Vector2(380, iYPosition + 150), sValueColor);


			mcSpriteBatch.DrawString(mcFont, "Toggle Random Direction: ", new Vector2(535, iYPosition + 25), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "X", new Vector2(770, iYPosition + 25), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Toggle Attract / Repel: ", new Vector2(555, iYPosition + 50), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "H", new Vector2(770, iYPosition + 50), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Mode: ", new Vector2(320, iYPosition + 50), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, mcMyParticleSystem.MagnetsMode.ToString(), new Vector2(375, iYPosition + 50), sValueColor);

			mcSpriteBatch.DrawString(mcFont, "Affect Position / Velocity: ", new Vector2(525, iYPosition + 75), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, "P", new Vector2(770, iYPosition + 75), sControlColor);

			mcSpriteBatch.DrawString(mcFont, "Affects: ", new Vector2(320, iYPosition + 75), sPropertyColor);
			mcSpriteBatch.DrawString(mcFont, mcMyParticleSystem.MagnetsAffectPosition ? "Position" : "Velocity", new Vector2(400, iYPosition + 75), sValueColor);

			// Stop drawing text
			mcSpriteBatch.End();
		}

		#endregion

		#region Handle Input

		/// <summary>
		/// Returns true if the Key is being pressed down
		/// </summary>
		/// <param name="cKey">The Key to check</param>
		/// <returns>Returns true if the Key is being pressed down</returns>
		bool KeyIsDown(Keys cKey)
		{
			return mcCurrentKeyboardState.IsKeyDown(cKey);
		}

		/// <summary>
		/// Returns true if the Key is being pressed down, and no other input was received in the 
		/// last TimeInSeconds seconds
		/// </summary>
		/// <param name="cKey">The Key to check</param>
		/// <param name="fTimeInSeconds">The amount of time in seconds that must have passed since the 
		/// last input for this key to be considered pressed down</param>
		/// <returns>Returns true if the Key is being pressed down, and no other input was received in
		/// the last TimeInSeconds seconds</returns>
		bool KeyIsDown(Keys cKey, float fTimeInSeconds)
		{
			// If the Key is being pressed down
			if (KeyIsDown(cKey))
			{
				// If the specified Time In Seconds has passed since any input was recieved
				if (mcInputTimeSpan.TotalSeconds >= fTimeInSeconds)
				{
					// Reset the Input Timer
					mcInputTimeSpan = TimeSpan.Zero;

					// Rerun that the specified amount of Time has elapsed since the last input was received
					return true;
				}
			}

			// Return that the key is not being pressed, or that a key was hit sooner than 
			// the specified Time In Seconds
			return false;
		}

		/// <summary>
		/// Returns true if the Key is not pressed down
		/// </summary>
		/// <param name="cKey">The Key to check</param>
		/// <returns>Returns true if the Key is not being pressed down</returns>
		bool KeyIsUp(Keys cKey)
		{
			return mcCurrentKeyboardState.IsKeyUp(cKey);
		}

		/// <summary>
		/// Returns true if the Key was just pressed down
		/// </summary>
		/// <param name="cKey">The Key to check</param>
		/// <returns>Returns true if the Key is being pressed now, but was not being pressed last frame</returns>
		bool KeyWasJustPressed(Keys cKey)
		{
			return (mcCurrentKeyboardState.IsKeyDown(cKey) && !mcPreviousKeyboardState.IsKeyDown(cKey));
		}

		/// <summary>
		/// Returns true if the Key was just released
		/// </summary>
		/// <param name="cKey">The Key to check</param>
		/// <returns>Returns true if the Key is not being pressed now, but was being pressed last frame</returns>
		bool KeyWasJustReleased(Keys cKey)
		{
			return (mcCurrentKeyboardState.IsKeyUp(cKey) && !mcPreviousKeyboardState.IsKeyUp(cKey));
		}

		/// <summary>
		/// Gets and processes all user input
		/// </summary>
		void ProcessInput(GameTime cGameTime)
		{
			// Save how long it's been since the last time Input was Handled
			float fTimeInSeconds = (float)cGameTime.ElapsedGameTime.TotalSeconds;

			// Add how long it's been since the last user input was received
			mcInputTimeSpan += TimeSpan.FromSeconds(fTimeInSeconds);

			// Save the Keyboard State and get its new State
			mcPreviousKeyboardState = mcCurrentKeyboardState;
			mcCurrentKeyboardState = Keyboard.GetState();

			// Save the Mouse State and get its new State
			mcPreviousMouseState = mcCurrentMouseState;
			mcCurrentMouseState = Mouse.GetState();

			// If we should Exit
			if (KeyIsDown(Keys.Escape))
			{
				Exit();
			}

			// Toggle showing the Text
			if (KeyWasJustPressed(Keys.F1))
			{
				mbShowText = !mbShowText;
			}

			// Toggle showing the Floor
			if (KeyWasJustPressed(Keys.F))
			{
				mbShowFloor = !mbShowFloor;
			}


			// Handle input for moving the Camera
			ProcessInputForCamera(fTimeInSeconds);

			// Handle input for controlling the Particle Systems
			ProcessInputForParticleSystem(fTimeInSeconds);
		}

		/// <summary>
		/// Handle input for moving the Camera
		/// </summary>
		public void ProcessInputForCamera(float fTimeInSeconds)
		{
			// If we are using the Fixed Camera
			if (msCamera.bUsingFixedCamera)
			{
				// If the Camera should be rotated vertically
				if (KeyIsDown(Keys.NumPad1))
				{
					msCamera.fCameraArc -= fTimeInSeconds * 25;
				}

				if (KeyIsDown(Keys.NumPad0))
				{
					msCamera.fCameraArc += fTimeInSeconds * 25;
				}

				// If the Camera should rotate horizontally
				if (KeyIsDown(Keys.Right))
				{
					msCamera.fCameraRotation -= fTimeInSeconds * 50;
				}

				if (KeyIsDown(Keys.Left))
				{
					msCamera.fCameraRotation += fTimeInSeconds * 50;
				}

				// If Camera should be zoomed out
				if (KeyIsDown(Keys.Down))
				{
					msCamera.fCameraDistance += fTimeInSeconds * 250;
				}

				// If Camera should be zoomed in
				if (KeyIsDown(Keys.Up))
				{
					msCamera.fCameraDistance -= fTimeInSeconds * 250;
				}


				// Calculate how much the Mouse was moved
				int iXMovement = mcCurrentMouseState.X - mcPreviousMouseState.X;
				int iYMovement = mcCurrentMouseState.Y - mcPreviousMouseState.Y;
				int iZMovement = mcCurrentMouseState.ScrollWheelValue - mcPreviousMouseState.ScrollWheelValue;

				const float fMOUSE_MOVEMENT_SPEED = 0.5f;
				const float fMOUSE_ROTATION_SPEED = 0.5f;

				// If the Left Mouse Button is pressed
				if (mcCurrentMouseState.LeftButton == ButtonState.Pressed)
				{
					// If the Camera should rotate horizontally
					if (iXMovement != 0)
					{
						msCamera.fCameraRotation -= (iXMovement * fMOUSE_ROTATION_SPEED);
					}

					// If the Camera should rotate vertically
					if (iYMovement != 0)
					{
						msCamera.fCameraArc -= (-iYMovement * fMOUSE_ROTATION_SPEED);
					}
				}

				// If the Right Mouse Button is pressed
				if (mcCurrentMouseState.RightButton == ButtonState.Pressed)
				{
					// If the Camera should zoom in/out
					if (iYMovement != 0)
					{
						msCamera.fCameraDistance += iYMovement * fMOUSE_MOVEMENT_SPEED;
					}
				}


				// Limit the Arc movement
				if (msCamera.fCameraArc > 90.0f)
				{
					msCamera.fCameraArc = 90.0f;
				}
				else if (msCamera.fCameraArc < -90.0f)
				{
					msCamera.fCameraArc = -90.0f;
				}

				// Limit the Camera zoom distance
				if (msCamera.fCameraDistance > 2000)
				{
					msCamera.fCameraDistance = 2000;
				}
				else if (msCamera.fCameraDistance < 1)
				{
					msCamera.fCameraDistance = 1;
				}
			}
			// Else we are using the Free Camera
			else
			{
				int iSPEED = 200;
				float fROTATE_SPEED = MathHelper.PiOver4;

				if (KeyIsDown(Keys.Decimal))
				{
					iSPEED = 100;
				}

				// If the Camera should move forward
				if (KeyIsDown(Keys.Up))
				{
					msCamera.MoveCameraForwardOrBackward(iSPEED * fTimeInSeconds);
				}

				// If the Camera should move backwards
				if (KeyIsDown(Keys.Down))
				{
					msCamera.MoveCameraForwardOrBackward(-iSPEED * fTimeInSeconds);
				}

				// If the Camera should strafe right
				if (KeyIsDown(Keys.Right))
				{
					msCamera.MoveCameraHorizontally(-iSPEED * fTimeInSeconds);
				}

				// If the Camera should strafe left
				if (KeyIsDown(Keys.Left))
				{
					msCamera.MoveCameraHorizontally(iSPEED * fTimeInSeconds);
				}

				// If the Camera should move upwards
				if (KeyIsDown(Keys.NumPad1))
				{
					msCamera.MoveCameraVertically((iSPEED / 2) * fTimeInSeconds);
				}

				// If the Camera should move downwards
				if (KeyIsDown(Keys.NumPad0))
				{
					msCamera.MoveCameraVertically((-iSPEED / 2) * fTimeInSeconds);
				}

				// If the Camera should yaw left
				if (KeyIsDown(Keys.NumPad4))
				{
					msCamera.RotateCameraHorizontally(fROTATE_SPEED * fTimeInSeconds);
				}

				// If the Camera should yaw right
				if (KeyIsDown(Keys.NumPad6))
				{
					msCamera.RotateCameraHorizontally(-fROTATE_SPEED * fTimeInSeconds);
				}

				// If the Camera should pitch up
				if (KeyIsDown(Keys.NumPad8))
				{
					msCamera.RotateCameraVertically(-fROTATE_SPEED * fTimeInSeconds);
				}

				// If the Camera should pitch down
				if (KeyIsDown(Keys.NumPad2))
				{
					msCamera.RotateCameraVertically(fROTATE_SPEED * fTimeInSeconds);
				}


				// Calculate how much the Mouse was moved
				int iXMovement = mcCurrentMouseState.X - mcPreviousMouseState.X;
				int iYMovement = mcCurrentMouseState.Y - mcPreviousMouseState.Y;
				int iZMovement = mcCurrentMouseState.ScrollWheelValue - mcPreviousMouseState.ScrollWheelValue;

				const float fMOUSE_MOVEMENT_SPEED = 0.5f;
				const float fMOUSE_ROTATION_SPEED = 0.005f;

				// If the Left Mouse Button is pressed
				if (mcCurrentMouseState.LeftButton == ButtonState.Pressed)
				{
					// If the Camera should yaw
					if (iXMovement != 0)
					{
						msCamera.RotateCameraHorizontally(-iXMovement * fMOUSE_ROTATION_SPEED);
					}

					// If the Camera should pitch
					if (iYMovement != 0)
					{
						msCamera.RotateCameraVertically(iYMovement * fMOUSE_ROTATION_SPEED);
					}
				}

				// If the Right Mouse Button is pressed
				if (mcCurrentMouseState.RightButton == ButtonState.Pressed)
				{
					// If the Camera should strafe
					if (iXMovement != 0)
					{
						msCamera.MoveCameraHorizontally(-iXMovement * fMOUSE_MOVEMENT_SPEED);
					}

					// If the Camera should move forward or backward
					if (iYMovement != 0)
					{
						msCamera.MoveCameraForwardOrBackward(-iYMovement * (fMOUSE_MOVEMENT_SPEED * 2.0f));
					}
				}

				// If the Middle Mouse Button is scrolled
				if (iZMovement != 0)
				{
					msCamera.MoveCameraVertically(iZMovement * (fMOUSE_MOVEMENT_SPEED / 10.0f));
				}
			}

			// If the Camera Mode should be switched
			if (KeyWasJustPressed(Keys.PageDown))
			{
				msCamera.bUsingFixedCamera = !msCamera.bUsingFixedCamera;
			}

			// If the Camera values should be Reset
			if (KeyIsDown(Keys.R))
			{
				msCamera.ResetFreeCameraVariables();
				msCamera.ResetFixedCameraVariables();
			}
		}

		/// <summary>
		/// Function to control the Particle Systems based on user input
		/// </summary>
		public void ProcessInputForParticleSystem(float fElapsedTimeInSeconds)
		{
			// Toggle the Emitter On / Off
			if (KeyWasJustPressed(Keys.Delete))
			{
				mcMyParticleSystem.Emitter.EmitParticlesAutomatically = !mcMyParticleSystem.Emitter.EmitParticlesAutomatically;
			}

			// If the Emitter's emission rate should be turned up
			if (KeyIsDown(Keys.Add, 0.02f))
			{
				mcMyParticleSystem.Emitter.ParticlesPerSecond++;
			}

			// If the Emitter's emission rate should be turned down
			if (KeyIsDown(Keys.Subtract, 0.02f))
			{
				// Make sure we always emit at least one particle per second
				if (mcMyParticleSystem.Emitter.ParticlesPerSecond > 1)
				{
					mcMyParticleSystem.Emitter.ParticlesPerSecond--;
				}
			}

			// Define how fast the user can move and rotate the Emitter
			float fEmitterMoveDelta = 75 * fElapsedTimeInSeconds;
			float fEmitterRotateDelta = MathHelper.Pi * fElapsedTimeInSeconds;

			// Check if the Emitter should be moved
			if (KeyIsDown(Keys.W))
			{
				mcMyParticleSystem.Emitter.PositionData.Position += Vector3.Up * fEmitterMoveDelta;
			}

			if (KeyIsDown(Keys.S))
			{
				mcMyParticleSystem.Emitter.PositionData.Position += Vector3.Down * fEmitterMoveDelta;
			}

			if (KeyIsDown(Keys.A))
			{
				mcMyParticleSystem.Emitter.PositionData.Position += Vector3.Left * fEmitterMoveDelta;
			}

			if (KeyIsDown(Keys.D))
			{
				mcMyParticleSystem.Emitter.PositionData.Position += Vector3.Right * fEmitterMoveDelta;
			}

			if (KeyIsDown(Keys.E))
			{
				mcMyParticleSystem.Emitter.PositionData.Position += Vector3.Forward * fEmitterMoveDelta;
			}

			if (KeyIsDown(Keys.Q))
			{
				mcMyParticleSystem.Emitter.PositionData.Position += Vector3.Backward * fEmitterMoveDelta;
			}

			// Check if the Emitter should be rotated
			if (KeyIsDown(Keys.J))
			{
				// If we should Rotate the Emitter around the Pivot Point
				if (KeyIsDown(Keys.Y))
				{
					mcMyParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(-fEmitterRotateDelta, 0.0f, 0.0f));
				}
				// Else we should just Rotate the Emitter about its center
				else
				{
					mcMyParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(-fEmitterRotateDelta, 0.0f, 0.0f));
				}
			}

			if (KeyIsDown(Keys.L))
			{
				// If we should Rotate the Emitter around the Pivot Point
				if (KeyIsDown(Keys.Y))
				{
					mcMyParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(fEmitterRotateDelta, 0.0f, 0.0f));
				}
				// Else we should just Rotate the Emitter about its center
				else
				{
					mcMyParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(fEmitterRotateDelta, 0.0f, 0.0f));
				}
			}

			if (KeyIsDown(Keys.I))
			{
				// If we should Rotate the Emitter around the Pivot Point
				if (KeyIsDown(Keys.Y))
				{
					mcMyParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, -fEmitterRotateDelta, 0.0f));
				}
				// Else we should just Rotate the Emitter about its center
				else
				{
					mcMyParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, -fEmitterRotateDelta, 0.0f));
				}
			}

			if (KeyIsDown(Keys.K))
			{
				// If we should Rotate the Emitter around the Pivot Point
				if (KeyIsDown(Keys.Y))
				{
					mcMyParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, fEmitterRotateDelta, 0.0f));
				}
				// Else we should just Rotate the Emitter about its center
				else
				{
					mcMyParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, fEmitterRotateDelta, 0.0f));
				}
			}

			if (KeyIsDown(Keys.U))
			{
				// If we should Rotate the Emitter around the Pivot Point
				if (KeyIsDown(Keys.Y))
				{
					mcMyParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, fEmitterRotateDelta));
				}
				// Else we should just Rotate the Emitter about its center
				else
				{
					mcMyParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, fEmitterRotateDelta));
				}
			}

			if (KeyIsDown(Keys.O))
			{
				// If we should Rotate the Emitter around the Pivot Point
				if (KeyIsDown(Keys.Y))
				{
					mcMyParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, -fEmitterRotateDelta));
				}
				// Else we should just Rotate the Emitter about its center
				else
				{
					mcMyParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, -fEmitterRotateDelta));
				}
			}

			// If the Emitter should be reset
			if (KeyWasJustPressed(Keys.Z))
			{
				mcMyParticleSystem.Emitter.PositionData.Position = Vector3.Zero;
				mcMyParticleSystem.Emitter.OrientationData.Orientation = Quaternion.Identity;
			}






			//=================================================================
			// Tutorial 7 Specific Code
			//=================================================================

			if (KeyWasJustPressed(Keys.X))
			{
				mcMyParticleSystem.ToggleInitialRandomDirection();
			}

			if (KeyWasJustPressed(Keys.C))
			{
				mcMyParticleSystem.UseMagnetType(DefaultParticleSystemMagnet.MagnetTypes.PointMagnet);
			}

			if (KeyWasJustPressed(Keys.V))
			{
				mcMyParticleSystem.UseMagnetType(DefaultParticleSystemMagnet.MagnetTypes.LineMagnet);
			}

			if (KeyWasJustPressed(Keys.B))
			{
				mcMyParticleSystem.UseMagnetType(DefaultParticleSystemMagnet.MagnetTypes.LineSegmentMagnet);
			}

			if (KeyWasJustPressed(Keys.N))
			{
				mcMyParticleSystem.UseMagnetType(DefaultParticleSystemMagnet.MagnetTypes.PlaneMagnet);
			}

			if (KeyWasJustPressed(Keys.M))
			{
				switch (mcMyParticleSystem.MagnetsDistanceFunction)
				{
					case DefaultParticleSystemMagnet.DistanceFunctions.Constant: 
						mcMyParticleSystem.MagnetsDistanceFunction = DefaultParticleSystemMagnet.DistanceFunctions.Linear;
						break;
					case DefaultParticleSystemMagnet.DistanceFunctions.Linear:
						mcMyParticleSystem.MagnetsDistanceFunction = DefaultParticleSystemMagnet.DistanceFunctions.LinearInverse;
						break;
					case DefaultParticleSystemMagnet.DistanceFunctions.LinearInverse:
						mcMyParticleSystem.MagnetsDistanceFunction = DefaultParticleSystemMagnet.DistanceFunctions.Squared;
						break;
					case DefaultParticleSystemMagnet.DistanceFunctions.Squared:
						mcMyParticleSystem.MagnetsDistanceFunction = DefaultParticleSystemMagnet.DistanceFunctions.SquaredInverse;
						break;
					case DefaultParticleSystemMagnet.DistanceFunctions.SquaredInverse:
						mcMyParticleSystem.MagnetsDistanceFunction = DefaultParticleSystemMagnet.DistanceFunctions.Cubed;
						break;
					case DefaultParticleSystemMagnet.DistanceFunctions.Cubed:
						mcMyParticleSystem.MagnetsDistanceFunction = DefaultParticleSystemMagnet.DistanceFunctions.CubedInverse;
						break;
					case DefaultParticleSystemMagnet.DistanceFunctions.CubedInverse:
					default:
						mcMyParticleSystem.MagnetsDistanceFunction = DefaultParticleSystemMagnet.DistanceFunctions.Constant;
						break;
				}
			}

			if (KeyWasJustPressed(Keys.G))
			{
				mcMyParticleSystem.MagnetsForce -= 2;
			}

			if (KeyWasJustPressed(Keys.T))
			{
				mcMyParticleSystem.MagnetsForce += 2;
			}

			if (KeyWasJustPressed(Keys.H))
			{
				if (mcMyParticleSystem.MagnetsMode == DefaultParticleSystemMagnet.MagnetModes.Attract)
				{
					mcMyParticleSystem.MagnetsMode = DefaultParticleSystemMagnet.MagnetModes.Repel;
				}
				else
				{
					mcMyParticleSystem.MagnetsMode = DefaultParticleSystemMagnet.MagnetModes.Attract;
				}
			}

			if (KeyWasJustPressed(Keys.P))
			{
				mcMyParticleSystem.ToogleMagnetsAffectPositionOrVelocity();
			}
		}

		#endregion
	}
}
