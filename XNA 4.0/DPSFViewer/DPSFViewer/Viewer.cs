//#region File Description
////===================================================================
//// Viewer.cs
////
//// This is the application file used to view a DPSF Particle System
////
//// Copyright Daniel Schroeder 2008
////===================================================================
//#endregion

//#region Using Statements
//using System;
//using System.Collections.Generic;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using DPSF;
//using DPSF.ParticleSystems;
//#endregion

//namespace DPSFViewer
//{
//    /// <summary>
//    /// Application class showing how to use the Dynamic Particle System Framework
//    /// </summary>
//    public class Viewer : Microsoft.Xna.Framework.Game
//    {
//        #region Fields

//        //===========================================================
//        // Global Constants - User Settings
//        //===========================================================

//        // To allow the game to run as fast as possible, set this to false
//        const bool mbLIMIT_FPS = false;

//        // How often the Particle Systems should be updated (zero = update as often as possible)
//        const int miPARTICLE_SYSTEM_UPDATES_PER_SECOND = 0;

//        // The background color to use
//        Color msBACKGROUND_COLOR = Color.Black;

//        // Static Particle Settings
//        float mfStaticParticleTimeStep = 1.0f / 30.0f; // The Time Step between the drawing of each frame of the Static Particles (1 / # of fps, example, 1 / 30 = 30fps)
//        float mfStaticParticleTotalTime = 3.0f; // The number of seconds that the Static Particle should be drawn over

//        // "Draw To Files" Settings
//        float mfDrawPSToFilesTimeStep = 1.0f / 10.0f;
//        float mfDrawPSToFilesTotalTime = 2.0f;
//        int miDrawPSToFilesImageWidth = 200;
//        int miDrawPSToFilesImageHeight = 150;
//        string msDrawPSToFilesDirectoryName = "AnimationFrames";
//        bool mbCreateAnimatedGIF = true;
//        bool mbCreateTileSetImage = false;

//        string msSerializedPSFileName = "SerializedParticleSystem.dat"; // The name of the file to serialize the particle system to

//        //===========================================================
//        // Class Structures and Variables
//        //===========================================================

//        // Structure to hold the Camera's position and orientation
//        struct SCamera
//        {
//            public Vector3 sVRP, cVPN, cVUP, cVLeft;                    // Free Camera Variables (View Reference Point, View Plane Normal, View Up, and View Left)
//            public float fCameraArc, fCameraRotation, fCameraDistance;  // Fixed Camera Variables
//            public Vector3 sFixedCameraLookAtPosition;                  // The Position that the Fixed Camera should rotate around
//            public bool bUsingFixedCamera;  // Variable indicating which type of Camera to use

//            /// <summary>
//            /// Explicit constructor
//            /// </summary>
//            /// <param name="bUseFixedCamera">True to use the Fixed Camera, false to use the Free Camera</param>
//            public SCamera(bool bUseFixedCamera)
//            {
//                // Initialize variables with dummy values so we can call the Reset functions
//                sVRP = cVPN = cVUP = cVLeft = sFixedCameraLookAtPosition = Vector3.Zero;
//                fCameraArc = fCameraRotation = fCameraDistance = 0.0f;

//                // Use the specified Camera type
//                bUsingFixedCamera = bUseFixedCamera;

//                // Initialize the variables with their proper values
//                ResetFreeCameraVariables();
//                ResetFixedCameraVariables();
//            }

//            /// <summary>
//            /// Get the current Position of the Camera
//            /// </summary>
//            public Vector3 Position
//            {
//                get
//                {
//                    // If we are using the Fixed Camera
//                    if (bUsingFixedCamera)
//                    {
//                        // Calculate the View Matrix
//                        Matrix cViewMatrix = Matrix.CreateTranslation(sFixedCameraLookAtPosition) *
//                                     Matrix.CreateRotationY(MathHelper.ToRadians(fCameraRotation)) *
//                                     Matrix.CreateRotationX(MathHelper.ToRadians(fCameraArc)) *
//                                     Matrix.CreateLookAt(new Vector3(0, 0, -fCameraDistance),
//                                                         new Vector3(0, 0, 0), Vector3.Up);

//                        // Invert the View Matrix
//                        cViewMatrix = Matrix.Invert(cViewMatrix);

//                        // Pull and return the Camera Coordinates from the inverted View Matrix
//                        return cViewMatrix.Translation;
//                    }
//                    // Else we are using the Free Camera
//                    else
//                    {
//                        return sVRP;
//                    }
//                }
//            }

//            /// <summary>
//            /// Reset the Fixed Camera Variables to their default values
//            /// </summary>
//            public void ResetFixedCameraVariables()
//            {
//                fCameraArc = 0.0f;
//                fCameraRotation = 180.0f;
//                fCameraDistance = 300.0f;
//                sFixedCameraLookAtPosition = new Vector3(0, -50, 0);
//            }

//            /// <summary>
//            /// Reset the Free Camera Variables to their default values
//            /// </summary>
//            public void ResetFreeCameraVariables()
//            {
//                sVRP = new Vector3(0.0f, 50.0f, 300.0f);
//                cVPN = Vector3.Forward;
//                cVUP = Vector3.Up;
//                cVLeft = Vector3.Left;
//            }

//            /// <summary>
//            /// Normalize the Camera Directions and maintain proper Right and Up directions
//            /// </summary>
//            public void NormalizeCameraAndCalculateProperUpAndRightDirections()
//            {
//                // Calculate the new Right and Up directions
//                cVPN.Normalize();
//                cVLeft = Vector3.Cross(cVUP, cVPN);
//                cVLeft.Normalize();
//                cVUP = Vector3.Cross(cVPN, cVLeft);
//                cVUP.Normalize();
//            }

//            /// <summary>
//            /// Move the Camera Forward or Backward
//            /// </summary>
//            /// <param name="fAmountToMove">The distance to Move</param>
//            public void MoveCameraForwardOrBackward(float fAmountToMove)
//            {
//                cVPN.Normalize();
//                sVRP += (cVPN * fAmountToMove);
//            }

//            /// <summary>
//            /// Move the Camera Horizontally
//            /// </summary>
//            /// <param name="fAmountToMove">The distance to move horizontally</param>
//            public void MoveCameraHorizontally(float fAmountToMove)
//            {
//                cVLeft.Normalize();
//                sVRP += (cVLeft * fAmountToMove);
//            }

//            /// <summary>
//            /// Move the Camera Vertically
//            /// </summary>
//            /// <param name="fAmountToMove">The distance to move Vertically</param>
//            public void MoveCameraVertically(float fAmountToMove)
//            {
//                // Move the Camera along the global Y axis
//                sVRP.Y += fAmountToMove;
//            }

//            /// <summary>
//            /// Rotate the Camera Horizontally
//            /// </summary>
//            /// <param name="fAmountToRotateInRadians">The amount to Rotate in radians</param>
//            public void RotateCameraHorizontally(float fAmountToRotateInRadians)
//            {
//                // Rotate the Camera about the global Y axis
//                Matrix cRotationMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, fAmountToRotateInRadians);
//                cVPN = Vector3.Transform(cVPN, cRotationMatrix);
//                cVUP = Vector3.Transform(cVUP, cRotationMatrix);

//                // Normalize all of the Camera directions since they have changed
//                NormalizeCameraAndCalculateProperUpAndRightDirections();
//            }

//            /// <summary>
//            /// Rotate the Camera Vertically
//            /// </summary>
//            /// <param name="fAmountToRotateInRadians">The amount to Rotate in radians</param>
//            public void RotateCameraVertically(float fAmountToRotateInRadians)
//            {
//                // Rotate the Camera
//                Matrix cRotationMatrix = Matrix.CreateFromAxisAngle(cVLeft, fAmountToRotateInRadians);
//                cVPN = Vector3.Transform(cVPN, cRotationMatrix);
//                cVUP = Vector3.Transform(cVUP, cRotationMatrix);

//                // Normalize all of the Camera directions since they have changed
//                NormalizeCameraAndCalculateProperUpAndRightDirections();
//            }
//        }

//        // List of all the textures
//        enum ETextures
//        {
//            AnimatedButterfly = 0,
//            AnimatedExplosion,
//            Arrow,
//            Arrow2,
//            Arrow3,
//            Arrow4,
//            Arrow5,
//            Ball,
//            Bubble,
//            Cloud,
//            CloudLight,
//            CrossArrow,
//            Dog,
//            Donut,
//            DPSFText,
//            Fire,
//            Flower,
//            Flower2,
//            Flower3,
//            Flower4,
//            Flower5,
//            Gear,
//            Gear2,
//            Gear3,
//            Gear4,
//            Gear5,
//            MoveArrow,
//            Paper,
//            Particle,
//            RedCircle,
//            Shape1,
//            Shape2,
//            Smoke,
//            Spark,
//            Splat,
//            Star,
//            Star2,
//            Star3,
//            Star4,
//            Star5,
//            Star6,
//            Star7,
//            Star8,
//            Star9,
//            StarFish,
//            Sun,
//            Sun2,
//            Sun3,
//            ThrowingStar,
//            Wheel,
//            WhiteSquare,
//            WhiteSquare1,
//            WhiteSquare2,
//            WhiteSquare10,
//            WordBubble,
//            X,
//            LastInList = 55     // This value should be the same as the Texture with the largest value
//        }

//        // Initialize the Texture to use
//        ETextures meCurrentTexture = ETextures.Bubble;

//        DPSFViewerForm _viewerForm = null;		// Handle to the WinForm that we are drawing to
//        public bool MouseOverViewport { get; set; }	// Tells if the mouse is currently over the Viewport on the WinForm or not

//        GraphicsDeviceManager mcGraphics;       // Handle to the Graphics object

//        SpriteBatch mcSpriteBatch;              // Batch used to draw Sprites
//        SpriteFont mcFont;                      // Font used to draw text
//        Model mcFloorModel;                     // Model of the Floor

//        Random mcRandom = new Random();         // Random number generator

//        // Input States
//        KeyboardState mcCurrentKeyboardState;   // Holds the Keyboard's Current State
//        KeyboardState mcPreviousKeyboardState;  // Holds the Keyboard's Previous State
//        MouseState mcCurrentMouseState;         // Holds the Mouse's Current State
//        MouseState mcPreviousMouseState;        // Holds the Mouse's Previous State

//        public bool ShowText { get; set; }      // Tells if Text should be shown or not
//        public bool ShowFloor { get; set; }		// Tells if the Floor should be Shown or not
//        public bool Paused { get; set; }		// Tells if the game should be Paused or not

//        bool mbShowCommonControls = false;      // Tells if the Common Controls should be shown or not
//        bool mbShowCameraControls = false;      // Tells if the Camera Controls should be shown or not
//        bool mbShowPerformanceText = false;     // Tells if we should draw Performance info or not, such as how much memory is currently set to be collected by the Garbage Collector.
//        TimeSpan mcInputTimeSpan = new TimeSpan();  // Used to control user input speed

//        bool DrawPerformanceText
//        {
//            get { return mbShowPerformanceText; }
//            set
//            {
//                bool previousValue = mbShowPerformanceText;
//                mbShowPerformanceText = value;

//                // Only enable the Performance Profiling if we are going to be displaying it.

//                // Set it on the Defaults so it is enabled/disabled whenever we initialize a new particle system.
//                DPSFDefaultSettings.PerformanceProfilingIsEnabled = mbShowPerformanceText;

//                // Set it on the Manager to enable/disable it for the particle system that is currently running.
//                mcParticleSystemManager.SetPerformanceProfilingIsEnabledForAllParticleSystems(mbShowPerformanceText);

//                // If this value was changed from off to on, hook up the event handler to calculate garbage collection
//                if (mbShowPerformanceText && !previousValue)
//                {
//                    FPS.FPSUpdated += new EventHandler<FPS.FPSEventArgs>(FPS_FPSUpdated);
//                }
//                // Else if this value was turned off, unhook the event handler
//                else if (!mbShowPerformanceText)
//                {
//                    FPS.FPSUpdated -= new EventHandler<FPS.FPSEventArgs>(FPS_FPSUpdated);
//                }
//            }
//        }

//        /// <summary>
//        /// Handles the FPSUpdated event of the FPS control to calculate the average amount of garbage created each frame in the last second.
//        /// </summary>
//        /// <param name="sender">The source of the event.</param>
//        /// <param name="e">The <see cref="DPSF.FPS.FPSEventArgs"/> instance containing the event data.</param>
//        void FPS_FPSUpdated(object sender, FPS.FPSEventArgs e)
//        {
//            // Get how much Garbage is waiting to be collected
//            long currentGarbageAmount = GC.GetTotalMemory(false);

//            // If the Garbage Collector did not run in the past second, calculate the average amount of garbage created per frame in the past second.
//            if (currentGarbageAmount > _garbageAmountAtLastFPSUpdate)
//            {
//                float garbageCreatedInLastSecondInKB = (currentGarbageAmount - _garbageAmountAtLastFPSUpdate) / 1024f;
//                _garbageAverageCreatedPerFrameInKB = garbageCreatedInLastSecondInKB / e.FPS;

//                int updatesPerSecond = mcCurrentParticleSystem.UpdatesPerSecond;
//                updatesPerSecond = updatesPerSecond > 0 ? updatesPerSecond : _updatesPerSecond;
//                _garbageAverageCreatedPerUpdateInKB = garbageCreatedInLastSecondInKB / updatesPerSecond;
//            }

//            // Record the current amount of garbage to use to calculate the Garbage Created Per Second on the next update
//            _garbageAmountAtLastFPSUpdate = currentGarbageAmount;

//            // Reset how many updates have been done in the past second
//            _updatesPerSecond = 0;
//        }

//        long _garbageAmountAtLastFPSUpdate = 0;         // The amount of garbage waiting to be collected by the Garbage Collector the last time the FPS were updated (one second interval).
//        float _garbageCurrentAmountInKB = 0;            // The amount of garbage currently waiting to be collected by the Garbage Collector (in Kilobytes).
//        float _garbageAverageCreatedPerFrameInKB = 0;   // How much garbage was created in the past second (in Kilobytes).
//        float _garbageAverageCreatedPerUpdateInKB = 0;  // How much garbage was created during the last Update() (in Kilobytes).
//        int _updatesPerSecond = 0;                      // The number of times the Updater() function was called in the past second.

//        // Draw Static Particle variables (2 of the variables are up in the User Settings)
//        bool mbDrawStaticPS = false;            // Tells if Static Particles should be drawn or not
//        bool mbStaticParticlesDrawn = false;    // Tells if the Static Particles have already been drawn or not
		

//        // The World, View, and Projection matrices
//        Matrix msWorldMatrix = Matrix.Identity;
//        Matrix msViewMatrix = Matrix.Identity;
//        Matrix msProjectionMatrix = Matrix.Identity;

//        // Variables used to draw the lines indicating positive axis directions
//        bool mbShowAxis = false;
//        VertexPositionColor[] msaAxisDirectionVertices; // Vertices to draw lines on the floor indicating positive axis directions
//        VertexDeclaration mcAxisVertexDeclaration;
//        BasicEffect mcAxisEffect;

//        // Initialize the Camera
//        SCamera msCamera = new SCamera(true);


//        // Declare the Particle System Manager to manage the Particle Systems
//        ParticleSystemManager mcParticleSystemManager = new ParticleSystemManager();

//        // Declare a Particle System pointer to point to the Current Particle System being used
//        IDPSFParticleSystem mcCurrentParticleSystem;
		
//        #endregion

//        #region Initialization

//        /// <summary>
//        /// Constructor
//        /// </summary>
//        public Viewer()
//        {
//            mcGraphics = new GraphicsDeviceManager(this);
//            Content.RootDirectory = "Content";

//            mcGraphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);
//            System.Windows.Forms.Control.FromHandle((this.Window.Handle)).VisibleChanged += new EventHandler(Game1_VisibleChanged);

//            _viewerForm = new DPSFViewerForm(this);
//            _viewerForm.Show();

//            // If we should not Limit the FPS
//            if (!mbLIMIT_FPS)
//            {
//                // Make the game run as fast as possible (i.e. don't limit the FPS)
//                this.IsFixedTimeStep = false;
//                mcGraphics.SynchronizeWithVerticalRetrace = false;
//            }

//            // Do we want to show the mouse
//            this.IsMouseVisible = true;
//        }

//        /// <summary>
//        /// Handles the PreparingDeviceSettings event of the graphics control.
//        /// </summary>
//        /// <param name="sender">The source of the event.</param>
//        /// <param name="e">The <see cref="Microsoft.Xna.Framework.PreparingDeviceSettingsEventArgs"/> instance containing the event data.</param>
//        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
//        {
//            e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = _viewerForm.Viewport.Handle;
//            e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = _viewerForm.Viewport.Width;
//            e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = _viewerForm.Viewport.Height;
//        }

//        /// <summary>
//        /// Handles the VisibleChanged event of the Game1 control.
//        /// </summary>
//        /// <param name="sender">The source of the event.</param>
//        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
//        private void Game1_VisibleChanged(object sender, EventArgs e)
//        {
//            if (System.Windows.Forms.Control.FromHandle((this.Window.Handle)).Visible == true)
//                System.Windows.Forms.Control.FromHandle((this.Window.Handle)).Visible = false;
//        }

//        /// <summary>
//        /// Load your graphics content
//        /// </summary>
//        protected override void LoadContent()
//        {
//            mcSpriteBatch = new SpriteBatch(mcGraphics.GraphicsDevice);

//            // Load fonts and models for test application
//            mcFont = Content.Load<SpriteFont>("Fonts/font");
//            mcFloorModel = Content.Load<Model>("grid");

//            // Specify vertices indicating positive axis directions
//            int iLineLength = 50;
//            msaAxisDirectionVertices = new VertexPositionColor[6];
//            msaAxisDirectionVertices[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);
//            msaAxisDirectionVertices[1] = new VertexPositionColor(new Vector3(iLineLength, 1, 0), Color.Red);
//            msaAxisDirectionVertices[2] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Green);
//            msaAxisDirectionVertices[3] = new VertexPositionColor(new Vector3(0, iLineLength, 0), Color.Green);
//            msaAxisDirectionVertices[4] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Blue);
//            msaAxisDirectionVertices[5] = new VertexPositionColor(new Vector3(0, 1, iLineLength), Color.Blue);
			
//            mcAxisEffect = new BasicEffect(GraphicsDevice);
//            mcAxisEffect.VertexColorEnabled = true;
//            mcAxisEffect.LightingEnabled = false;
//            mcAxisEffect.TextureEnabled = false;
//            mcAxisEffect.FogEnabled = false;
//            mcAxisVertexDeclaration = VertexPositionColor.VertexDeclaration;

//            // Set how often the Particle Systems should be Updated
//            mcParticleSystemManager.UpdatesPerSecond = miPARTICLE_SYSTEM_UPDATES_PER_SECOND;

//            mcCurrentParticleSystem = new RandomParticleSystem(this);

//            // Initialize the Current Particle System
//            SetParticleSystem(mcCurrentParticleSystem);
//        }

//        /// <summary>
//        /// Sets the particle system to use in the Viewer.
//        /// </summary>
//        /// <param name="particleSystem">The particle system to use.</param>
//        public void SetParticleSystem(IDPSFParticleSystem particleSystem)
//        {
//            // If the Current Particle System has been set
//            if (mcCurrentParticleSystem != null)
//            {
//                // Destroy the Current Particle System.
//                // This frees up any resources/memory held by the Particle System, so it's
//                // good to destroy them if we know they won't be used for a while
//                mcCurrentParticleSystem.Destroy();
//            }

//            mcCurrentParticleSystem = particleSystem;

//            // Initialize the Particle System
//            mcCurrentParticleSystem.AutoInitialize(this.GraphicsDevice, this.Content, null);

//            mcParticleSystemManager.RemoveAllParticleSystems();
//            mcParticleSystemManager.AddParticleSystem(mcCurrentParticleSystem);
//        }

//        #endregion

//        #region Update and Draw

//        /// <summary>
//        /// Allows the game to run logic
//        /// </summary>
//        protected override void Update(GameTime cGameTime)
//        {
//            // Get and process user Input
//            ProcessInput(cGameTime);

//            // Allow the camera to be moved around, even if the particle systems are paused

//            // Update the World, View, and Projection matrices
//            UpdateWorldViewProjectionMatrices();


//            // Update the Quad Particle Systems to know where the Camera is so that they can display
//            // the particles as billboards if needed (i.e. have particle always face the camera).
//            mcParticleSystemManager.SetCameraPositionForAllParticleSystems(msCamera.Position);

//            // Set the World, View, and Projection Matrices for the Particle Systems
//            mcParticleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(msWorldMatrix, msViewMatrix, msProjectionMatrix);

//            // If the Game is Paused
//            if (Paused)
//            {
//                // Update the particle systems with 0 elapsed time, just to allow the particles to rotate to face the camera.
//                mcParticleSystemManager.UpdateAllParticleSystems(0);

//                // Exit without updating anything
//                return;
//            }

//            // If the Current Particle System is Initialized
//            if (mcCurrentParticleSystem != null && mcCurrentParticleSystem.IsInitialized)
//            {
//                // If Static Particles should be drawn
//                if (mbDrawStaticPS)
//                {
//                    // If the Static Particles haven't been drawn yet
//                    if (!mbStaticParticlesDrawn)
//                    {
//                        // Set the World, View, and Projection Matrices for all Particle Systems
//                        mcParticleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(msWorldMatrix, msViewMatrix, msProjectionMatrix);

//                        // Update the Particle System iteratively by the Time Step amount until the 
//                        // Particle System behaviour over the Total Time has been drawn
//                        float fElapsedTime = 0;
//                        while (fElapsedTime < mfStaticParticleTotalTime)
//                        {
//                            // Update and draw this frame of the Particle System
//                            mcParticleSystemManager.UpdateAllParticleSystems(mfStaticParticleTimeStep);
//                            mcParticleSystemManager.DrawAllParticleSystems();
//                            fElapsedTime += mfStaticParticleTimeStep;
//                        }
//                        mbStaticParticlesDrawn = true;

//                        mcSpriteBatch.Begin();
//                        mcSpriteBatch.DrawString(mcFont, "F6 to continue", new Vector2(310, 25), Color.LawnGreen);
//                        mcSpriteBatch.End();
//                    }
//                }
//                // Else the Particle Systems should be drawn normally
//                else
//                {
//                    // Update all Particle Systems manually
//                    mcParticleSystemManager.UpdateAllParticleSystems((float)cGameTime.ElapsedGameTime.TotalSeconds);
//                }
//            }
		
//            // Update any other Drawable Game Components
//            base.Update(cGameTime);

//            // If we are drawing garbage collection info
//            if (DrawPerformanceText)
//            {
//                // Record how much Garbage is waiting to be collected in Kilobytes.
//                _garbageCurrentAmountInKB = GC.GetTotalMemory(false) / 1024f;

//                // Increment the number of updates that have been performed in the past second.
//                _updatesPerSecond++;
//            }
//        }

//        /// <summary>
//        /// Updates the World, View, and Projection matrices according to the Camera's current position.
//        /// </summary>
//        private void UpdateWorldViewProjectionMatrices()
//        {
//            // Compute the Aspect Ratio of the window
//            float fAspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;

//            // Setup the View matrix depending on which Camera mode is being used
//            // If we are using the Fixed Camera
//            if (msCamera.bUsingFixedCamera)
//            {
//                // Set up the View matrix according to the Camera's arc, rotation, and distance from the Offset position
//                msViewMatrix = Matrix.CreateTranslation(msCamera.sFixedCameraLookAtPosition) *
//                                     Matrix.CreateRotationY(MathHelper.ToRadians(msCamera.fCameraRotation)) *
//                                     Matrix.CreateRotationX(MathHelper.ToRadians(msCamera.fCameraArc)) *
//                                     Matrix.CreateLookAt(new Vector3(0, 0, -msCamera.fCameraDistance),
//                                                         new Vector3(0, 0, 0), Vector3.Up);
//            }
//            // Else we are using the Free Camera
//            else
//            {
//                // Set up our View matrix specifying the Camera position, a point to look-at, and a direction for which way is up
//                msViewMatrix = Matrix.CreateLookAt(msCamera.sVRP, msCamera.sVRP + msCamera.cVPN, msCamera.cVUP);
//            }

//            // Setup the Projection matrix by specifying the field of view (1/4 pi), aspect ratio, and the near and far clipping planes
//            msProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, fAspectRatio, 1, 10000);
//        }

//        /// <summary>
//        /// This is called when the game should draw itself
//        /// </summary>
//        protected override void Draw(GameTime cGameTime)
//        {
//            // Clear the scene
//            GraphicsDevice.Clear(msBACKGROUND_COLOR);

//            // Draw the Floor at the origin (0,0,0) and any other models
//            DrawModels(msWorldMatrix, msViewMatrix, msProjectionMatrix);

//            // If the Axis' should be drawn
//            if (mbShowAxis)
//            {
//                // Draw lines at the origin (0,0,0) indicating positive axis directions
//                DrawAxis(msWorldMatrix, msViewMatrix, msProjectionMatrix);
//            }


//            // Draw any other Drawable Game Components that may need to be drawn.
//            // Call this before drawing our Particle Systems, so that our 2D Sprite particles
//            // show up on top of the any other 2D Sprites drawn.
//            base.Draw(cGameTime);

//            // Draw the Particle Systems manually
//            mcParticleSystemManager.DrawAllParticleSystems();


//            // Update the Frames Per Second to be displayed
//            FPS.Update((float)cGameTime.ElapsedGameTime.TotalSeconds);

//            // Draw the Text to the screen last, so it is always on top
//            DrawText();
//        }

//        /// <summary>
//        /// Function to draw Text to the screen
//        /// </summary>
//        void DrawText()
//        {
//            // If no Text should be shown
//            if (!ShowText)
//            {
//                // Exit the function before drawing any Text
//                return;
//            }

//            // Specify the text Colors to use
//            Color sPropertyColor = Color.WhiteSmoke;
//            Color sValueColor = Color.Yellow;
//            Color sControlColor = Color.PowderBlue;

//            // If we don't have a handle to a particle system, it is because we serialized it
//            if (mcCurrentParticleSystem == null)
//            {
//                mcSpriteBatch.Begin();
//                mcSpriteBatch.DrawString(mcFont, "Particle system has been serialized to the file: " + msSerializedPSFileName + ".", new Vector2(25, 200), sPropertyColor);
//                mcSpriteBatch.DrawString(mcFont, "To deserialize the particle system from the file, restoring the instance of", new Vector2(25, 225), sPropertyColor);
//                mcSpriteBatch.DrawString(mcFont, "the particle system,", new Vector2(25, 250), sPropertyColor);
//                mcSpriteBatch.DrawString(mcFont, "press F9", new Vector2(210, 250), sControlColor);
//                mcSpriteBatch.End();
//                return;
//            }
			
//            // If the Particle System has been destroyed, just write that to the screen and exit
//            if (!mcCurrentParticleSystem.IsInitialized)
//            {
//                mcSpriteBatch.Begin();
//                mcSpriteBatch.DrawString(mcFont, "The current particle system has been destroyed.", new Vector2(140, 200), sPropertyColor);
//                mcSpriteBatch.DrawString(mcFont, "Press G / H to switch to a different particle system.", new Vector2(125, 225), sPropertyColor);
//                mcSpriteBatch.End();
//                return;
//            }

//            // Get the Name of the Particle System and how many Particles are currently Active
//            int iNumberOfActiveParticles = mcParticleSystemManager.TotalNumberOfActiveParticles;
//            int iNumberOfParticlesAllocatedInMemory = mcParticleSystemManager.TotalNumberOfParticlesAllocatedInMemory;

//            // Convert numbers to strings
//            string sFPSValue = FPS.CurrentFPS.ToString();
//            string sAvgFPSValue = FPS.AverageFPS.ToString("0.0");
//            string sTotalParticleCountValue = iNumberOfActiveParticles.ToString();
//            string sEmitterOnValue = (mcCurrentParticleSystem.Emitter.EmitParticlesAutomatically ? "On" : "Off");
//            string sParticlesPerSecondValue = mcCurrentParticleSystem.Emitter.ParticlesPerSecond.ToString("0.00");
//            string sCameraModeValue = msCamera.bUsingFixedCamera ? "Fixed" : "Free";
//            string sPSSpeedScale = mcParticleSystemManager.SimulationSpeed.ToString("0.0");
//            string sCameraPosition = "(" + msCamera.Position.X.ToString("0") + "," + msCamera.Position.Y.ToString("0") + "," + msCamera.Position.Z.ToString("0") + ")";
//            string sAllocatedParticles = iNumberOfParticlesAllocatedInMemory.ToString();
//            string sTexture = "N/A";
//            if (mcCurrentParticleSystem.Texture != null)
//            {
//                sTexture = mcCurrentParticleSystem.Texture.Name.TrimStart("Textures/".ToCharArray());
//            }

//            // Draw all of the text
//            mcSpriteBatch.Begin();

//            mcSpriteBatch.DrawString(mcFont, "FPS:", new Vector2(GetTextSafeArea().Left + 5, GetTextSafeArea().Bottom - 50), sPropertyColor);
//            mcSpriteBatch.DrawString(mcFont, sFPSValue, new Vector2(GetTextSafeArea().Left + 50, GetTextSafeArea().Bottom - 50), sValueColor);

//            mcSpriteBatch.DrawString(mcFont, "Allocated:", new Vector2(GetTextSafeArea().Left + 120, GetTextSafeArea().Bottom - 50), sPropertyColor);
//            mcSpriteBatch.DrawString(mcFont, sAllocatedParticles, new Vector2(GetTextSafeArea().Left + 210, GetTextSafeArea().Bottom - 50), sValueColor);

//            //mcSpriteBatch.DrawString(mcFont, "Position:", new Vector2(GetTextSafeArea().Left + 275, GetTextSafeArea().Bottom - 75), sPropertyColor);
//            mcSpriteBatch.DrawString(mcFont, sCameraPosition, new Vector2(GetTextSafeArea().Left + 280, GetTextSafeArea().Bottom - 50), sValueColor);

//            mcSpriteBatch.DrawString(mcFont, "Texture:", new Vector2(GetTextSafeArea().Left + 440, GetTextSafeArea().Bottom - 50), sPropertyColor);
//            mcSpriteBatch.DrawString(mcFont, sTexture, new Vector2(GetTextSafeArea().Left + 520, GetTextSafeArea().Bottom - 50), sValueColor);

//            mcSpriteBatch.DrawString(mcFont, "Speed:", new Vector2(GetTextSafeArea().Right - 100, GetTextSafeArea().Bottom - 50), sPropertyColor);
//            mcSpriteBatch.DrawString(mcFont, sPSSpeedScale, new Vector2(GetTextSafeArea().Right - 35, GetTextSafeArea().Bottom - 50), sValueColor);

//            mcSpriteBatch.DrawString(mcFont, "Avg:", new Vector2(GetTextSafeArea().Left + 5, GetTextSafeArea().Bottom - 25), sPropertyColor);
//            mcSpriteBatch.DrawString(mcFont, sAvgFPSValue, new Vector2(GetTextSafeArea().Left + 50, GetTextSafeArea().Bottom - 25), sValueColor);

//            mcSpriteBatch.DrawString(mcFont, "Particles:", new Vector2(GetTextSafeArea().Left + 120, GetTextSafeArea().Bottom - 25), sPropertyColor);
//            mcSpriteBatch.DrawString(mcFont, sTotalParticleCountValue, new Vector2(GetTextSafeArea().Left + 205, GetTextSafeArea().Bottom - 25), sValueColor);

//            mcSpriteBatch.DrawString(mcFont, "Emitter:", new Vector2(GetTextSafeArea().Left + 275, GetTextSafeArea().Bottom - 25), sPropertyColor);
//            mcSpriteBatch.DrawString(mcFont, sEmitterOnValue, new Vector2(GetTextSafeArea().Left + 345, GetTextSafeArea().Bottom - 25), sValueColor);

//            mcSpriteBatch.DrawString(mcFont, "Particles Per Second:", new Vector2(GetTextSafeArea().Left + 390, GetTextSafeArea().Bottom - 25), sPropertyColor);
//            mcSpriteBatch.DrawString(mcFont, sParticlesPerSecondValue, new Vector2(GetTextSafeArea().Left + 585, GetTextSafeArea().Bottom - 25), sValueColor);

//            mcSpriteBatch.DrawString(mcFont, "Camera:", new Vector2(GetTextSafeArea().Left + 660, GetTextSafeArea().Bottom - 25), sPropertyColor);
//            mcSpriteBatch.DrawString(mcFont, sCameraModeValue, new Vector2(GetTextSafeArea().Left + 740, GetTextSafeArea().Bottom - 25), sValueColor);

//            mcSpriteBatch.DrawString(mcFont, "Show/Hide Controls:", new Vector2(GetTextSafeArea().Right - 260, GetTextSafeArea().Top + 2), sPropertyColor);
//            mcSpriteBatch.DrawString(mcFont, "F1 - F4", new Vector2(GetTextSafeArea().Right - 70, GetTextSafeArea().Top + 2), sControlColor);

//            // If the Particle System is Paused
//            if (Paused)
//            {
//                mcSpriteBatch.DrawString(mcFont, "Paused", new Vector2(GetTextSafeArea().Left + 350, GetTextSafeArea().Top + 25), sValueColor);
//            }

//            // If the Camera Controls should be shown
//            if (mbShowCameraControls)
//            {
//                // If we are using a Fixed Camera
//                if (msCamera.bUsingFixedCamera)
//                {
//                    mcSpriteBatch.DrawString(mcFont, "Fixed Camera Controls:", new Vector2(5, mcGraphics.PreferredBackBufferHeight - 125), sPropertyColor);
//                    mcSpriteBatch.DrawString(mcFont, "Keys: Left/Right Arrows, Up/Down Arrows, Num0/Num1", new Vector2(15, mcGraphics.PreferredBackBufferHeight - 100), sControlColor);
//                    mcSpriteBatch.DrawString(mcFont, "Mouse: Left Button + X/Y Movement, Right Button + Y Movement", new Vector2(15, mcGraphics.PreferredBackBufferHeight - 75), sControlColor);
//                }
//                // Else we are using a Free Camera
//                else
//                {
//                    mcSpriteBatch.DrawString(mcFont, "Free Camera Controls", new Vector2(5, mcGraphics.PreferredBackBufferHeight - 125), sPropertyColor);
//                    mcSpriteBatch.DrawString(mcFont, "Keys: Left/Right Arrows, Up/Down Arrows, Num0/Num1, Num4/Num6, Num8/Num2", new Vector2(15, mcGraphics.PreferredBackBufferHeight - 100), sControlColor);
//                    mcSpriteBatch.DrawString(mcFont, "Mouse: Left Button + X/Y Movement, Right Button + X/Y Movement, Scroll Wheel", new Vector2(15, mcGraphics.PreferredBackBufferHeight - 75), sControlColor);
//                }
//            }

//            // If we should draw the number of bytes allocated in memory
//            if (DrawPerformanceText)
//            {
//                mcSpriteBatch.DrawString(mcFont, "Update Time (ms): " + mcParticleSystemManager.TotalPerformanceTimeToDoUpdatesInMilliseconds.ToString("0.000"), new Vector2(529, mcGraphics.PreferredBackBufferHeight - 250), sPropertyColor);
//                mcSpriteBatch.DrawString(mcFont, "Draw Time (ms): " + mcParticleSystemManager.TotalPerformanceTimeToDoDrawsInMilliseconds.ToString("0.000"), new Vector2(545, mcGraphics.PreferredBackBufferHeight - 225), sPropertyColor);
//                mcSpriteBatch.DrawString(mcFont, "Garbage Allocated (KB): " + _garbageCurrentAmountInKB.ToString("0.0"), new Vector2(480, mcGraphics.PreferredBackBufferHeight - 200), sPropertyColor);
//                mcSpriteBatch.DrawString(mcFont, "Avg Garbage Per Update (KB): " + _garbageAverageCreatedPerUpdateInKB.ToString("0.000"), new Vector2(440, mcGraphics.PreferredBackBufferHeight - 175), sPropertyColor);
//                mcSpriteBatch.DrawString(mcFont, "Avg Garbage Per Frame (KB): " + _garbageAverageCreatedPerFrameInKB.ToString("0.000"), new Vector2(445, mcGraphics.PreferredBackBufferHeight - 150), sPropertyColor);
//            }

//            // Stop drawing text
//            mcSpriteBatch.End();
//        }

//        /// <summary>
//        /// Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).
//        /// </summary>
//        /// <returns>Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).</returns>
//        Rectangle GetTextSafeArea()
//        {
//            return GetTextSafeArea(0.9f);
//        }

//        /// <summary>
//        /// Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).
//        /// </summary>
//        /// <param name="fNormalizedPercent">The amount of screen space (normalized between 0.0 - 1.0) that should 
//        /// safe to draw to (e.g. 0.8 to have a 10% border on all sides)</param>
//        /// <returns>Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).</returns>
//        Rectangle GetTextSafeArea(float fNormalizedPercent)
//        {
//            Rectangle rTextSafeArea = new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y,
//                                            GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
//#if (XBOX)
//            // Find Title Safe area of Xbox 360.
//            float fBorder = (1 - fNormalizedPercent) / 2;
//            rTextSafeArea.X = (int)(fBorder * rTextSafeArea.Width);
//            rTextSafeArea.Y = (int)(fBorder * rTextSafeArea.Height);
//            rTextSafeArea.Width = (int)(fNormalizedPercent * rTextSafeArea.Width);
//            rTextSafeArea.Height = (int)(fNormalizedPercent * rTextSafeArea.Height);
//#endif
//            return rTextSafeArea;
//        }

//        /// <summary>
//        /// Helper for drawing the Models
//        /// </summary>
//        void DrawModels(Matrix cWorldMatrix, Matrix cViewMatrix, Matrix cProjectionMatrix)
//        {
//            // Set our sampler state to allow the ground to have a repeated texture
//            GraphicsDevice.BlendState = BlendState.Opaque;
//            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
//            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
//            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

//            // If the Floor should be drawn
//            if (ShowFloor)
//            {
//                mcFloorModel.Draw(cWorldMatrix, cViewMatrix, cProjectionMatrix);
//            }
//        }

//        /// <summary>
//        /// Helper for drawing lines showing the positive axis directions
//        /// </summary>
//        void DrawAxis(Matrix cWorldMatrix, Matrix cViewMatrix, Matrix cProjectionMatrix)
//        {
//            mcAxisEffect.World = cWorldMatrix;
//            mcAxisEffect.View = cViewMatrix;
//            mcAxisEffect.Projection = cProjectionMatrix;

//            // Draw the lines
//            foreach (EffectPass cPass in mcAxisEffect.CurrentTechnique.Passes)
//            {
//                cPass.Apply();
//                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, msaAxisDirectionVertices, 0, 3, mcAxisVertexDeclaration);
//            }
//        }


//        #endregion

//        #region Handle Input

//        /// <summary>
//        /// Returns true if the Key is being pressed down
//        /// </summary>
//        /// <param name="cKey">The Key to check</param>
//        /// <returns>Returns true if the Key is being pressed down</returns>
//        bool KeyIsDown(Keys cKey)
//        {
//            return mcCurrentKeyboardState.IsKeyDown(cKey);
//        }

//        /// <summary>
//        /// Returns true if the Key is being pressed down, and no other input was received in the 
//        /// last TimeInSeconds seconds
//        /// </summary>
//        /// <param name="cKey">The Key to check</param>
//        /// <param name="fTimeInSeconds">The amount of time in seconds that must have passed since the 
//        /// last input for this key to be considered pressed down</param>
//        /// <returns>Returns true if the Key is being pressed down, and no other input was received in
//        /// the last TimeInSeconds seconds</returns>
//        bool KeyIsDown(Keys cKey, float fTimeInSeconds)
//        {
//            // If the Key is being pressed down
//            if (KeyIsDown(cKey))
//            {
//                // If the specified Time In Seconds has passed since any input was recieved
//                if (mcInputTimeSpan.TotalSeconds >= fTimeInSeconds)
//                {
//                    // Reset the Input Timer
//                    mcInputTimeSpan = TimeSpan.Zero;

//                    // Rerun that the specified amount of Time has elapsed since the last input was received
//                    return true;
//                }
//            }

//            // Return that the key is not being pressed, or that a key was hit sooner than 
//            // the specified Time In Seconds
//            return false;
//        }

//        /// <summary>
//        /// Returns true if the Key is not pressed down
//        /// </summary>
//        /// <param name="cKey">The Key to check</param>
//        /// <returns>Returns true if the Key is not being pressed down</returns>
//        bool KeyIsUp(Keys cKey)
//        {
//            return mcCurrentKeyboardState.IsKeyUp(cKey);
//        }

//        /// <summary>
//        /// Returns true if the Key was just pressed down
//        /// </summary>
//        /// <param name="cKey">The Key to check</param>
//        /// <returns>Returns true if the Key is being pressed now, but was not being pressed last frame</returns>
//        bool KeyWasJustPressed(Keys cKey)
//        {
//            return (mcCurrentKeyboardState.IsKeyDown(cKey) && !mcPreviousKeyboardState.IsKeyDown(cKey));
//        }

//        /// <summary>
//        /// Returns true if the Key was just released
//        /// </summary>
//        /// <param name="cKey">The Key to check</param>
//        /// <returns>Returns true if the Key is not being pressed now, but was being pressed last frame</returns>
//        bool KeyWasJustReleased(Keys cKey)
//        {
//            return (mcCurrentKeyboardState.IsKeyUp(cKey) && !mcPreviousKeyboardState.IsKeyUp(cKey));
//        }

//        /// <summary>
//        /// Gets and processes all user input
//        /// </summary>
//        void ProcessInput(GameTime cGameTime)
//        {
//            // Save how long it's been since the last time Input was Handled
//            float fTimeInSeconds = (float)cGameTime.ElapsedGameTime.TotalSeconds;

//            // Add how long it's been since the last user input was received
//            mcInputTimeSpan += TimeSpan.FromSeconds(fTimeInSeconds);

//            // Save the Keyboard State and get its new State
//            mcPreviousKeyboardState = mcCurrentKeyboardState;
//            mcCurrentKeyboardState = Keyboard.GetState();

//            // Save the Mouse State and get its new State
//            mcPreviousMouseState = mcCurrentMouseState;
//            mcCurrentMouseState = Mouse.GetState();

//            // If the Mouse is not over the Viewport, we don't want to process input, so exit
//            if (!MouseOverViewport)
//                return;

//            // If we should Exit
//            if (KeyIsDown(Keys.Escape))
//            {
//                Exit();
//            }


//// This is just here for testing and should be removed eventually.
//            if (KeyWasJustPressed(Keys.V))
//                mcCurrentParticleSystem.Visible = !mcCurrentParticleSystem.Visible;



//            // If we should toggle between Full Screen and Windowed mode
//            if (KeyWasJustPressed(Keys.End))
//            {
//                mcGraphics.ToggleFullScreen();
//            }

//            // If we should toggle showing the Common Controls
//            if (KeyWasJustPressed(Keys.F1))
//            {
//                mbShowCommonControls = !mbShowCommonControls;
//            }

//            // If we should toggle showing the Camera Controls
//            if (KeyWasJustPressed(Keys.F3))
//            {
//                mbShowCameraControls = !mbShowCameraControls;
//            }

//            // If we should toggle showing the Common Controls
//            if (KeyWasJustPressed(Keys.F4))
//            {
//                ShowText = !ShowText;
//            }

//            // If the particle lifetimes should be drawn in one frame
//            if (KeyWasJustPressed(Keys.F6))
//            {
//                mbDrawStaticPS = !mbDrawStaticPS;
//                mbStaticParticlesDrawn = false;
//            }

//            // If the Axis should be toggled on/off
//            if (KeyWasJustPressed(Keys.F7))
//            {
//                mbShowAxis = !mbShowAxis;
//            }
//#if (WINDOWS)
//            // If the PS should be drawn to files
//            if (KeyWasJustPressed(Keys.F8))
//            {
//                // Draw the Particle System Animation to a series of Image Files
//                mcParticleSystemManager.DrawAllParticleSystemsAnimationToFiles(GraphicsDevice, miDrawPSToFilesImageWidth, miDrawPSToFilesImageHeight, 
//                            msDrawPSToFilesDirectoryName, mfDrawPSToFilesTotalTime, mfDrawPSToFilesTimeStep, mbCreateAnimatedGIF, mbCreateTileSetImage);
//            }

//            // If the PS should be serialized to a file
//            if (KeyWasJustPressed(Keys.F9))
//            {
//                // Only particle systems that do not inherit the DrawableGameComponent can be serialized.
//                if (!DPSFHelper.DPSFInheritsDrawableGameComponent)
//                {
//                    // If we have the particle system right now
//                    if (mcCurrentParticleSystem != null)
//                    {
//                        // Serialize the particle system into a file
//                        System.IO.Stream stream = System.IO.File.Open("SerializedParticleSystem.dat", System.IO.FileMode.Create);
//                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
//                        formatter.Serialize(stream, mcCurrentParticleSystem);
//                        stream.Close();

//                        // Remove the particle system from the manager and destroy the particle system in memory
//                        mcParticleSystemManager.RemoveParticleSystem(mcCurrentParticleSystem);
//                        mcCurrentParticleSystem.Destroy();
//                        mcCurrentParticleSystem = null;
//                    }
//                    // Else we don't have the particle system right now
//                    else
//                    {
//                        // Deserialize the particle system from a file
//                        System.IO.Stream stream = System.IO.File.Open("SerializedParticleSystem.dat", System.IO.FileMode.Open);
//                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
//                        mcCurrentParticleSystem = (IDPSFParticleSystem)formatter.Deserialize(stream);
//                        stream.Close();

//                        try
//                        {
//                            // Setup the particle system properties that couldn't be serialized
//                            mcCurrentParticleSystem.InitializeNonSerializableProperties(this, this.GraphicsDevice, this.Content);
//                        }
//                        // Catch the case where the Particle System requires a texture, but one wasn't loaded
//                        catch (ArgumentNullException)
//                        {
//                            // Assign the particle system a texture to use
//                            mcCurrentParticleSystem.SetTexture("Textures/Bubble");
//                        }

//                        // Re-add the particle system to the particle system manager
//                        mcParticleSystemManager.AddParticleSystem(mcCurrentParticleSystem);
//                    }
//                }
//            }
//#endif
//            // If the Performance Profiling was toggled
//            if (KeyWasJustPressed(Keys.F10))
//            {
//                // Toggle if the Performance Profiling text should be drawn
//                DrawPerformanceText = !DrawPerformanceText;
//            }

//            // Handle input for moving the Camera
//            ProcessInputForCamera(fTimeInSeconds);

//            // Handle input for controlling the Particle Systems
//            ProcessInputForParticleSystem(fTimeInSeconds);
//        }

//        /// <summary>
//        /// Handle input for moving the Camera
//        /// </summary>
//        public void ProcessInputForCamera(float fTimeInSeconds)
//        {
//            // If we are using the Fixed Camera
//            if (msCamera.bUsingFixedCamera)
//            {
//                // If the Camera should be rotated vertically
//                if (KeyIsDown(Keys.NumPad1) || (KeyIsUp(Keys.LeftShift) && KeyIsUp(Keys.RightShift) && KeyIsDown(Keys.D1)))
//                {
//                    msCamera.fCameraArc -= fTimeInSeconds * 25;
//                }

//                if (KeyIsDown(Keys.NumPad0) || (KeyIsUp(Keys.LeftShift) && KeyIsUp(Keys.RightShift) && KeyIsDown(Keys.D0)))
//                {
//                    msCamera.fCameraArc += fTimeInSeconds * 25;
//                }

//                // If the Camera should rotate horizontally
//                if (KeyIsDown(Keys.Right))
//                {
//                    msCamera.fCameraRotation -= fTimeInSeconds * 50;
//                }

//                if (KeyIsDown(Keys.Left))
//                {
//                    msCamera.fCameraRotation += fTimeInSeconds * 50;
//                }

//                // If Camera should be zoomed out
//                if (KeyIsDown(Keys.Down))
//                {
//                    msCamera.fCameraDistance += fTimeInSeconds * 250;
//                }

//                // If Camera should be zoomed in
//                if (KeyIsDown(Keys.Up))
//                {
//                    msCamera.fCameraDistance -= fTimeInSeconds * 250;
//                }


//                // Calculate how much the Mouse was moved
//                int iXMovement = mcCurrentMouseState.X - mcPreviousMouseState.X;
//                int iYMovement = mcCurrentMouseState.Y - mcPreviousMouseState.Y;
//                int iZMovement = mcCurrentMouseState.ScrollWheelValue - mcPreviousMouseState.ScrollWheelValue;

//                const float fMOUSE_MOVEMENT_SPEED = 0.5f;
//                const float fMOUSE_ROTATION_SPEED = 0.5f;

//                // If the Left Mouse Button is pressed
//                if (mcCurrentMouseState.LeftButton == ButtonState.Pressed)
//                {
//                    // If the Camera should rotate horizontally
//                    if (iXMovement != 0)
//                    {
//                        msCamera.fCameraRotation -= (iXMovement * fMOUSE_ROTATION_SPEED);
//                    }

//                    // If the Camera should rotate vertically
//                    if (iYMovement != 0)
//                    {
//                        msCamera.fCameraArc -= (-iYMovement * fMOUSE_ROTATION_SPEED);
//                    }
//                }

//                // If the Right Mouse Button is pressed
//                if (mcCurrentMouseState.RightButton == ButtonState.Pressed)
//                {
//                    // If the Camera should zoom in/out
//                    if (iYMovement != 0)
//                    {
//                        msCamera.fCameraDistance += iYMovement * fMOUSE_MOVEMENT_SPEED;
//                    }
//                }


//                // Limit the Arc movement
//                if (msCamera.fCameraArc > 90.0f)
//                {
//                    msCamera.fCameraArc = 90.0f;
//                }
//                else if (msCamera.fCameraArc < -90.0f)
//                {
//                    msCamera.fCameraArc = -90.0f;
//                }

//                // Limit the Camera zoom distance
//                if (msCamera.fCameraDistance > 2000)
//                {
//                    msCamera.fCameraDistance = 2000;
//                }
//                else if (msCamera.fCameraDistance < 1)
//                {
//                    msCamera.fCameraDistance = 1;
//                }
//            }
//            // Else we are using the Free Camera
//            else
//            {
//                int iSPEED = 200;
//                float fROTATE_SPEED = MathHelper.PiOver4;

//                if (KeyIsDown(Keys.Decimal))
//                {
//                    iSPEED = 100;
//                }

//                // If the Camera should move forward
//                if (KeyIsDown(Keys.Up))
//                {
//                    msCamera.MoveCameraForwardOrBackward(iSPEED * fTimeInSeconds);
//                }

//                // If the Camera should move backwards
//                if (KeyIsDown(Keys.Down))
//                {
//                    msCamera.MoveCameraForwardOrBackward(-iSPEED * fTimeInSeconds);
//                }

//                // If the Camera should strafe right
//                if (KeyIsDown(Keys.Right))
//                {
//                    msCamera.MoveCameraHorizontally(-iSPEED * fTimeInSeconds);
//                }

//                // If the Camera should strafe left
//                if (KeyIsDown(Keys.Left))
//                {
//                    msCamera.MoveCameraHorizontally(iSPEED * fTimeInSeconds);
//                }

//                // If the Camera should move upwards
//                if (KeyIsDown(Keys.NumPad1) || (KeyIsUp(Keys.LeftShift) && KeyIsUp(Keys.RightShift) && KeyIsDown(Keys.D1)))
//                {
//                    msCamera.MoveCameraVertically((iSPEED / 2) * fTimeInSeconds);
//                }

//                // If the Camera should move downwards
//                if (KeyIsDown(Keys.NumPad0) || (KeyIsUp(Keys.LeftShift) && KeyIsUp(Keys.RightShift) && KeyIsDown(Keys.D0)))
//                {
//                    msCamera.MoveCameraVertically((-iSPEED / 2) * fTimeInSeconds);
//                }

//                // If the Camera should yaw left
//                if (KeyIsDown(Keys.NumPad4) || (KeyIsUp(Keys.LeftShift) && KeyIsUp(Keys.RightShift) && KeyIsDown(Keys.D4)))
//                {
//                    msCamera.RotateCameraHorizontally(fROTATE_SPEED * fTimeInSeconds);
//                }

//                // If the Camera should yaw right
//                if (KeyIsDown(Keys.NumPad6) || (KeyIsUp(Keys.LeftShift) && KeyIsUp(Keys.RightShift) && KeyIsDown(Keys.D6)))
//                {
//                    msCamera.RotateCameraHorizontally(-fROTATE_SPEED * fTimeInSeconds);
//                }

//                // If the Camera should pitch up
//                if (KeyIsDown(Keys.NumPad8) || (KeyIsUp(Keys.LeftShift) && KeyIsUp(Keys.RightShift) && KeyIsDown(Keys.D8)))
//                {
//                    msCamera.RotateCameraVertically(-fROTATE_SPEED * fTimeInSeconds);
//                }

//                // If the Camera should pitch down
//                if (KeyIsDown(Keys.NumPad2) || (KeyIsUp(Keys.LeftShift) && KeyIsUp(Keys.RightShift) && KeyIsDown(Keys.D2)))
//                {
//                    msCamera.RotateCameraVertically(fROTATE_SPEED * fTimeInSeconds);
//                }


//                // Calculate how much the Mouse was moved
//                int iXMovement = mcCurrentMouseState.X - mcPreviousMouseState.X;
//                int iYMovement = mcCurrentMouseState.Y - mcPreviousMouseState.Y;
//                int iZMovement = mcCurrentMouseState.ScrollWheelValue - mcPreviousMouseState.ScrollWheelValue;

//                const float fMOUSE_MOVEMENT_SPEED = 0.5f;
//                const float fMOUSE_ROTATION_SPEED = 0.005f;

//                // If the Left Mouse Button is pressed
//                if (mcCurrentMouseState.LeftButton == ButtonState.Pressed)
//                {
//                    // If the Camera should yaw
//                    if (iXMovement != 0)
//                    {
//                        msCamera.RotateCameraHorizontally(-iXMovement * fMOUSE_ROTATION_SPEED);
//                    }

//                    // If the Camera should pitch
//                    if (iYMovement != 0)
//                    {
//                        msCamera.RotateCameraVertically(iYMovement * fMOUSE_ROTATION_SPEED);
//                    }
//                }

//                // If the Right Mouse Button is pressed
//                if (mcCurrentMouseState.RightButton == ButtonState.Pressed)
//                {
//                    // If the Camera should strafe
//                    if (iXMovement != 0)
//                    {
//                        msCamera.MoveCameraHorizontally(-iXMovement * fMOUSE_MOVEMENT_SPEED);
//                    }

//                    // If the Camera should move forward or backward
//                    if (iYMovement != 0)
//                    {
//                        msCamera.MoveCameraForwardOrBackward(-iYMovement * (fMOUSE_MOVEMENT_SPEED * 2.0f));
//                    }
//                }

//                // If the Middle Mouse Button is scrolled
//                if (iZMovement != 0)
//                {
//                    msCamera.MoveCameraVertically(iZMovement * (fMOUSE_MOVEMENT_SPEED / 10.0f));
//                }
//            }

//            // If the Camera Mode should be switched
//            if (KeyWasJustPressed(Keys.PageDown))
//            {
//                msCamera.bUsingFixedCamera = !msCamera.bUsingFixedCamera;
//            }

//            // If the Camera values should be Reset
//            if (KeyIsDown(Keys.R))
//            {
//                msCamera.ResetFreeCameraVariables();
//                msCamera.ResetFixedCameraVariables();
//            }
//        }

//        /// <summary>
//        /// Function to control the Particle Systems based on user input
//        /// </summary>
//        public void ProcessInputForParticleSystem(float fElapsedTimeInSeconds)
//        {
//            // If the Current Particle System is not Initialized
//            if (mcCurrentParticleSystem == null || !mcCurrentParticleSystem.IsInitialized)
//            {
//                return;
//            }

//            // Define how fast the user can move and rotate the Emitter
//            float fEmitterMoveDelta = 75 * fElapsedTimeInSeconds;
//            float fEmitterRotateDelta = MathHelper.Pi * fElapsedTimeInSeconds;

//            // If the Shift key is down, move faster
//            if (KeyIsDown(Keys.LeftShift) || KeyIsDown(Keys.RightShift))
//            {
//                fEmitterMoveDelta *= 2;
//            }

//            // Check if the Emitter should be moved
//            if (KeyIsDown(Keys.W))
//            {
//                mcCurrentParticleSystem.Emitter.PositionData.Position += Vector3.Up * fEmitterMoveDelta;
//            }

//            if (KeyIsDown(Keys.S))
//            {
//                mcCurrentParticleSystem.Emitter.PositionData.Position += Vector3.Down * fEmitterMoveDelta;
//            }

//            if (KeyIsDown(Keys.A))
//            {
//                mcCurrentParticleSystem.Emitter.PositionData.Position += Vector3.Left * fEmitterMoveDelta;
//            }

//            if (KeyIsDown(Keys.D))
//            {
//                mcCurrentParticleSystem.Emitter.PositionData.Position += Vector3.Right * fEmitterMoveDelta;
//            }

//            if (KeyIsDown(Keys.E))
//            {
//                mcCurrentParticleSystem.Emitter.PositionData.Position += Vector3.Forward * fEmitterMoveDelta;
//            }

//            if (KeyIsDown(Keys.Q))
//            {
//                mcCurrentParticleSystem.Emitter.PositionData.Position += Vector3.Backward * fEmitterMoveDelta;
//            }

//            // Check if the Emitter should be rotated
//            if (!KeyIsDown(Keys.V) && !KeyIsDown(Keys.B) && !KeyIsDown(Keys.P))
//            {
//                if (KeyIsDown(Keys.J))
//                {
//                    // If we should Rotate the Emitter around the Pivot Point
//                    if (KeyIsDown(Keys.Y))
//                    {
//                        mcCurrentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(-fEmitterRotateDelta, 0.0f, 0.0f));
//                    }
//                    // Else we should just Rotate the Emitter about its center
//                    else
//                    {
//                        mcCurrentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(-fEmitterRotateDelta, 0.0f, 0.0f));
//                    }
//                }

//                if (KeyIsDown(Keys.L))
//                {
//                    // If we should Rotate the Emitter around the Pivot Point
//                    if (KeyIsDown(Keys.Y))
//                    {
//                        mcCurrentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(fEmitterRotateDelta, 0.0f, 0.0f));
//                    }
//                    // Else we should just Rotate the Emitter about its center
//                    else
//                    {
//                        mcCurrentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(fEmitterRotateDelta, 0.0f, 0.0f));
//                    }
//                }

//                if (KeyIsDown(Keys.I))
//                {
//                    // If we should Rotate the Emitter around the Pivot Point
//                    if (KeyIsDown(Keys.Y))
//                    {
//                        mcCurrentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, -fEmitterRotateDelta, 0.0f));
//                    }
//                    // Else we should just Rotate the Emitter about its center
//                    else
//                    {
//                        mcCurrentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, -fEmitterRotateDelta, 0.0f));
//                    }
//                }

//                if (KeyIsDown(Keys.K))
//                {
//                    // If we should Rotate the Emitter around the Pivot Point
//                    if (KeyIsDown(Keys.Y))
//                    {
//                        mcCurrentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, fEmitterRotateDelta, 0.0f));
//                    }
//                    // Else we should just Rotate the Emitter about its center
//                    else
//                    {
//                        mcCurrentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, fEmitterRotateDelta, 0.0f));
//                    }
//                }

//                if (KeyIsDown(Keys.U))
//                {
//                    // If we should Rotate the Emitter around the Pivot Point
//                    if (KeyIsDown(Keys.Y))
//                    {
//                        mcCurrentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, fEmitterRotateDelta));
//                    }
//                    // Else we should just Rotate the Emitter about its center
//                    else
//                    {
//                        mcCurrentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, fEmitterRotateDelta));
//                    }
//                }

//                if (KeyIsDown(Keys.O))
//                {
//                    // If we should Rotate the Emitter around the Pivot Point
//                    if (KeyIsDown(Keys.Y))
//                    {
//                        mcCurrentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, -fEmitterRotateDelta));
//                    }
//                    // Else we should just Rotate the Emitter about its center
//                    else
//                    {
//                        mcCurrentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, -fEmitterRotateDelta));
//                    }
//                }
//            }

//            // Check if the Emitter should be reset
//            if (KeyWasJustPressed(Keys.Z))
//            {
//                mcCurrentParticleSystem.Emitter.PositionData.Position = Vector3.Zero;
//                mcCurrentParticleSystem.Emitter.OrientationData.Orientation = Quaternion.Identity;
//            }

//            // If the Texture should be changed
//            if (KeyWasJustPressed(Keys.T))
//            {
//                if (mcCurrentParticleSystem.Texture != null)
//                {
//                    // Get which Texture is currently being used for sure
//                    for (int i = 0; i < (int)ETextures.LastInList + 1; i++)
//                    {
//                        ETextures eTexture = (ETextures)i;
//                        string sName = eTexture.ToString();

//                        if (mcCurrentParticleSystem.Texture.Name.Equals(sName))
//                        {
//                            meCurrentTexture = (ETextures)i;
//                        }
//                    }

//                    // If we should go to the previous Texture
//                    if (KeyIsDown(Keys.LeftShift) || KeyIsDown(Keys.RightShift))
//                    {
//                        meCurrentTexture--;
//                        if (meCurrentTexture < 0)
//                        {
//                            meCurrentTexture = ETextures.LastInList;
//                        }
//                    }
//                    // Else we should go to the next Texture
//                    else
//                    {
//                        meCurrentTexture++;
//                        if (meCurrentTexture > ETextures.LastInList)
//                        {
//                            meCurrentTexture = 0;
//                        }
//                    }

//                    // Change the Texture being used to draw the Particles
//                    mcCurrentParticleSystem.SetTexture("Textures/" + meCurrentTexture.ToString());
//                }
//            }

//            if (KeyWasJustPressed(Keys.Insert))
//            {
//                // Add a single Particle
//                mcCurrentParticleSystem.AddParticle();
//            }

//            if (KeyIsDown(Keys.Home))
//            {
//                // Add Particles while the button is pressed
//                mcCurrentParticleSystem.AddParticle();
//            }

//            if (KeyIsDown(Keys.PageUp))
//            {
//                // Add the max number of Particles
//                while (mcCurrentParticleSystem.AddParticle()) { }
//            }

//            if (KeyWasJustPressed(Keys.Delete))
//            {
//                // Toggle emitting particles on/off
//                mcCurrentParticleSystem.Emitter.EmitParticlesAutomatically = !mcCurrentParticleSystem.Emitter.EmitParticlesAutomatically;
//            }

//            if (KeyIsDown(Keys.Add, 0.02f) || KeyIsDown(Keys.OemPlus, 0.02f))
//            {
//                // Increase the number of Particles being emitted
//                mcCurrentParticleSystem.Emitter.ParticlesPerSecond++;
//            }

//            if (KeyIsDown(Keys.Subtract, 0.02f) || KeyIsDown(Keys.OemMinus, 0.02f))
//            {
//                if (mcCurrentParticleSystem.Emitter.ParticlesPerSecond > 1)
//                {
//                    // Decrease the number of Particles being emitted
//                    mcCurrentParticleSystem.Emitter.ParticlesPerSecond--;
//                }
//            }

//            if (KeyWasJustPressed(Keys.Multiply) || 
//                ((KeyIsDown(Keys.LeftShift) || KeyIsDown(Keys.RightShift)) && KeyWasJustPressed(Keys.D8)))
//            {
//                // Increase the Speed of the Particle System simulation
//                mcParticleSystemManager.SimulationSpeed += 0.1f;

//                if (mcParticleSystemManager.SimulationSpeed > 5.0f)
//                {
//                    mcParticleSystemManager.SimulationSpeed = 5.0f;
//                }

//                // If DPSF is not inheriting from DrawableGameComponent then we need
//                // to set the individual particle system's Simulation Speeds
//                if (DPSFHelper.DPSFInheritsDrawableGameComponent)
//                {
//                    mcParticleSystemManager.SetSimulationSpeedForAllParticleSystems(mcParticleSystemManager.SimulationSpeed);
//                }
//            }

//            if (KeyWasJustPressed(Keys.Divide) ||
//                (KeyIsUp(Keys.LeftShift) && KeyIsUp(Keys.RightShift) && KeyWasJustPressed(Keys.OemQuestion)))
//            {
//                // Decrease the Speed of the Particle System simulation
//                mcParticleSystemManager.SimulationSpeed -= 0.1f;

//                if (mcParticleSystemManager.SimulationSpeed < 0.1f)
//                {
//                    mcParticleSystemManager.SimulationSpeed = 0.1f;
//                }

//                // If DPSF is not inheriting from DrawableGameComponent then we need
//                // to set the individual particle system's Simulation Speeds
//                if (DPSFHelper.DPSFInheritsDrawableGameComponent)
//                {
//                    mcParticleSystemManager.SetSimulationSpeedForAllParticleSystems(mcParticleSystemManager.SimulationSpeed);
//                }
//            }
//        }

//        #endregion
//    }
//}
