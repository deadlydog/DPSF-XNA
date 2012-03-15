using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using DPSF;
using DPSF.ParticleSystems;

namespace Demo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Specify the Window's Title
        string sWindowTitle = "Demo Template";

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

        Terrain mcTerrain = new Terrain();
        SkyBox mcSkyBox = new SkyBox();

        Random mcRandom = new Random();         // Random number generator


        // Declare a Particle System variable
        MyParticleSystem mcMyParticleSystem = null;

        // Declare the Particle System Manager to manage the Particle Systems
        ParticleSystemManager mcParticleSystemManager = new ParticleSystemManager();


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
            mcGraphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;

            // Set the Title of the Window
            Window.Title = sWindowTitle;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mcSpriteBatch = new SpriteBatch(GraphicsDevice);
            mcFont = Content.Load<SpriteFont>("Fonts/font");

            // Setup the Terrain
            Texture2D cTerrainTexture = Content.Load<Texture2D>("Terrain/Desert");
            Texture2D cHeightMap = Content.Load<Texture2D>("Terrain/HeightMap");
            mcTerrain.Load(GraphicsDevice, cHeightMap, 3000, 3000, 200, cTerrainTexture, 3);

            // Setup the SkyBox
            Texture2D cBack = Content.Load<Texture2D>("SkyBox/sahara_south");
            Texture2D cFront = Content.Load<Texture2D>("SkyBox/sahara_north");
            Texture2D cBottom = Content.Load<Texture2D>("SkyBox/sahara_down");
            Texture2D cTop = Content.Load<Texture2D>("SkyBox/sahara_up");
            Texture2D cLeft = Content.Load<Texture2D>("SkyBox/sahara_west");
            Texture2D cRight = Content.Load<Texture2D>("SkyBox/sahara_east");
            mcSkyBox.Load(GraphicsDevice, 3000, 300, cBack, cFront, cBottom, cTop, cLeft, cRight);

            // Instance and Initialize the Particle System
            mcMyParticleSystem = new MyParticleSystem(this);
            mcMyParticleSystem.AutoInitialize(GraphicsDevice, Content, null);

            // Add the Particle System to the Particle System Manager
            mcParticleSystemManager.AddParticleSystem(mcMyParticleSystem);

            // Setup the Fog
            //mcTerrain.mcEffect.FogEnabled = true;
            mcTerrain.mcEffect.FogColor = Color.Gray.ToVector3();
            mcTerrain.mcEffect.FogEnd = 2000;

            //mcSkyBox.mcEffect.FogEnabled = true;
            mcSkyBox.mcEffect.FogColor = Color.Gray.ToVector3();
            mcSkyBox.mcEffect.FogEnd = 2000;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime cGameTime)
        {
            // Get and process user Input
            ProcessInput(cGameTime);

            // Update all of the Particle Systems
            mcParticleSystemManager.UpdateAllParticleSystems((float)cGameTime.ElapsedGameTime.TotalSeconds);

            base.Update(cGameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime cGameTime)
        {
            // Get a handle to the Graphics Device used for drawing
            GraphicsDevice cGraphicsDevice = mcGraphics.GraphicsDevice;

            // Clear the scene
            cGraphicsDevice.Clear(Color.Black);

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


            // Draw the Sky Box
            mcSkyBox.Draw(msCamera.Position, msWorldMatrix, msViewMatrix, msProjectionMatrix);

            // Draw the Terrain
            mcTerrain.Draw(msWorldMatrix, msViewMatrix, msProjectionMatrix);


            // Draw any other Drawable Game Components that may need to be drawn.
            // Call this before drawing our Particle Systems, so that our 2D Sprite particles
            // show up ontop of the any other 2D Sprites drawn.
            base.Draw(cGameTime);
            

            // Set the World, View, and Projection Matrices for the Particle Systems before Drawing them
            mcParticleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(msWorldMatrix, msViewMatrix, msProjectionMatrix);

            // Draw the Particle Systems
            mcParticleSystemManager.DrawAllParticleSystems();


            // Update the Frames Per Second to be displayed
            FPS.Update((float)cGameTime.ElapsedRealTime.TotalSeconds);

            // Draw the Text to the screen last, so it is always on top
            DrawText();
        }

        /// <summary>
        /// Function to draw Text to the screen
        /// </summary>
        void DrawText()
        {
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

            mcSpriteBatch.DrawString(mcFont, "FPS: ", new Vector2(5, miWINDOW_HEIGHT - 20), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, sFPS, new Vector2(50, miWINDOW_HEIGHT - 20), sValueColor);

            mcSpriteBatch.DrawString(mcFont, "Particles: ", new Vector2(130, miWINDOW_HEIGHT - 20), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, sTotalNumberOfParticles, new Vector2(215, miWINDOW_HEIGHT - 20), sValueColor);

            mcSpriteBatch.DrawString(mcFont, "Emitter: ", new Vector2(325, miWINDOW_HEIGHT - 20), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, sEmitterOnValue, new Vector2(400, miWINDOW_HEIGHT - 20), sValueColor);

            mcSpriteBatch.DrawString(mcFont, "Particles Per Second: ", new Vector2(480, miWINDOW_HEIGHT - 20), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, sParticlesPerSecond, new Vector2(680, miWINDOW_HEIGHT - 20), sValueColor);




            // Stop drawing text
            mcSpriteBatch.End();
        }


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

            // Handle input for moving the Camera
            ProcessInputForCamera(fTimeInSeconds);
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

        #endregion
    }
}
