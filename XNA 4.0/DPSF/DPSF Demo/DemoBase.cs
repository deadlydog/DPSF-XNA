#region File Description
//===================================================================
// GameMain.cs
//
// This is the application file used to test the Dynamic Particle System Framework
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using DPSF_Demo.Input;
using DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo;
using DPSF_Demo.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DPSF;
using DPSF.ParticleSystems;
#endregion

namespace DPSF_Demo
{
	/// <summary>
	/// Class that provides a basic virtual environment with common requirements (floor, camera, process input, draw text, etc.).
	/// </summary>
	public class DemoBase : Microsoft.Xna.Framework.Game
	{
		#region Fields

		//===========================================================
		// Global Constants - User Settings
		//===========================================================
		
		/// <summary>
		/// If this is set to true, a MessageBox will display any unhandled exceptions that occur. This
		/// is useful when not debugging (i.e. when running directly from the executable) as it will still 
		/// tell you what type of exception occurred and what line of code produced it.
		/// Leave this set to false while debugging to have Visual Studio automatically take you to the 
		/// line that threw the exception.
		/// </summary>
		public const bool RELEASE_MODE = false;

		/// <summary>
		/// To allow the game to run as fast as possible, set this to false, otherwise the game will only run 
		/// as fast as the VSync allows (typically 60 fps).
		/// </summary>
		public const bool LIMIT_FPS = false;

		/// <summary>
		/// The Width of the application's window.
		/// Default resolution is 800x600.
		/// </summary>
		public const int WINDOW_WIDTH = 800;

		/// <summary>
		/// The Height of the application's window.
		/// Default resolution is 800x600.
		/// </summary>
		public const int WINDOW_HEIGHT = 600;

		/// <summary>
		/// The color to wipe the screen with each frame.
		/// </summary>
	    protected static Color BACKGROUND_COLOR = Color.Black;

	    /// <summary>
	    /// The color that information text and property descriptions should be drawn in.
	    /// </summary>
		protected static Color PROPERTY_TEXT_COlOR = Color.WhiteSmoke;

		/// <summary>
		/// The color that a property's value text should be drawn in.
		/// </summary>
		protected static Color VALUE_TEXT_COLOR = Color.Yellow;

		/// <summary>
		/// The color that an input control's text should be drawn in.
		/// </summary>
		protected static Color CONTROL_TEXT_COLOR = Color.PowderBlue;


		//===========================================================
		// Class Structures and Variables
		//===========================================================

		/// <summary>
		/// Handle to the game's Graphics Device Manager.
		/// </summary>
		protected GraphicsDeviceManager GraphicsDeviceManager { get; set; }

		/// <summary>
		/// A Sprite Batch that can be used to draw sprites and text.
		/// </summary>
		protected SpriteBatch SpriteBatch { get; set; }
		
		/// <summary>
		/// The default Font to draw text with.
		/// </summary>
		protected SpriteFont Font { get; set; }

		/// <summary>
		/// Tells if we should draw Performance info or not, such as how much memory is currently set to be collected by the Garbage Collector.
		/// </summary>
		protected bool ShowPerformanceText
		{
			get { return _showPerformanceText; }
			set
			{
				// Only bother doing anything if the value of this property is actually being changed.
				if (value == _showPerformanceText)
					return;

				// Set the new value.
				_showPerformanceText = value;

				// If this value was turned on, hook up the event handler to calculate garbage collection.
				if (_showPerformanceText)
				{
					FPS.FPSUpdated += new EventHandler<FPS.FPSEventArgs>(FPS_FPSUpdated);
				}
				// Else this value was turned off, so unhook the event handler.
				else
				{
					FPS.FPSUpdated -= new EventHandler<FPS.FPSEventArgs>(FPS_FPSUpdated);
				}

				// Call the virtual function to perform any extra work that inheriting classes may have.
				ShowPerformanceTextToggled(_showPerformanceText);
			}
		}
		private bool _showPerformanceText = false;

		/// <summary>
		/// Called whenever the value of ShowPerformanceText is changed.
		/// </summary>
		/// <param name="enabled"></param>
		protected virtual void ShowPerformanceTextToggled(bool enabled) { }

		/// <summary>
		/// Handles the FPSUpdated event of the FPS control to calculate the average amount of garbage created each frame in the last second.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DPSF_Demo.FPS.FPSEventArgs"/> instance containing the event data.</param>
		private void FPS_FPSUpdated(object sender, FPS.FPSEventArgs e)
		{
			// Get how much Garbage is waiting to be collected
			long currentGarbageAmount = GC.GetTotalMemory(false);

			// If the Garbage Collector did not run in the past second, calculate the average amount of garbage created per frame in the past second.
			if (currentGarbageAmount > _garbageAmountAtLastFPSUpdate)
			{
				float garbageCreatedInLastSecondInKB = (currentGarbageAmount - _garbageAmountAtLastFPSUpdate) / 1024f;
				_garbageAverageCreatedPerFrameInKB = garbageCreatedInLastSecondInKB / e.FPS;

				int updatesPerSecond = _currentDPSFDemoParticleSystemWrapper.UpdatesPerSecond;
				updatesPerSecond = updatesPerSecond > 0 ? updatesPerSecond : _updatesPerSecond;
				_garbageAverageCreatedPerUpdateInKB = garbageCreatedInLastSecondInKB / updatesPerSecond;
			}

			// Record the current amount of garbage to use to calculate the Garbage Created Per Second on the next update
			_garbageAmountAtLastFPSUpdate = currentGarbageAmount;

			// Reset how many updates have been done in the past second
			_updatesPerSecond = 0;
		}

		private long _garbageAmountAtLastFPSUpdate = 0;         // The amount of garbage waiting to be collected by the Garbage Collector the last time the FPS were updated (one second interval).
		private float _garbageCurrentAmountInKB = 0;            // The amount of garbage currently waiting to be collected by the Garbage Collector (in Kilobytes).
		private float _garbageAverageCreatedPerFrameInKB = 0;   // How much garbage was created in the past second (in Kilobytes).
		private float _garbageAverageCreatedPerUpdateInKB = 0;  // How much garbage was created during the last Update() (in Kilobytes).
		private int _updatesPerSecond = 0;                      // The number of times the Update() function was called in the past second.

		/// <summary>
		/// Tells if any Text should be shown or not.
		/// </summary>
		protected bool ShowText { get; set; }

		/// <summary>
		/// Tells if the Camera Controls should be shown or not.
		/// </summary>
		protected bool ShowCameraControls { get; set; }

		/// <summary>
		/// Tells if the Floor should be shown or not.
		/// </summary>
		protected bool ShowFloor { get; set; }

		/// <summary>
		/// Tells if the game should be paused or not.
		/// </summary>
		protected bool Paused { get; set; }

		/// <summary>
		/// Tells if the screen should be cleared every frame with the Background Color or not.
		/// </summary>
		protected bool ClearScreenEveryFrame { get; set; }
		private bool _clearScreenEveryFrameJustToggled = false;	// Tells if the ClearScreenEveryFrame variable was just toggled or not.
		private RenderTarget2D _renderTarget = null;			// Used to draw to when we want draws to persist across multiple frames.

		/// <summary>
		/// The World matrix used for drawing.
		/// </summary>
		protected Matrix WorldMatrix { get; set; }

		/// <summary>
		/// The View matrix used for drawing.
		/// </summary>
		protected Matrix ViewMatrix { get; set; }

		/// <summary>
		/// The Projection matrix used for drawing.
		/// </summary>
		protected Matrix ProjectionMatrix { get; set; }

		// Variables used to draw the lines indicating positive axis directions
		protected bool ShowPositiveDirectionAxis { get; set; }
		private VertexPositionColor[] msaAxisDirectionVertices;	// Vertices to draw lines on the floor indicating positive axis directions.
		private VertexDeclaration mcAxisVertexDeclaration;
		private BasicEffect mcAxisEffect;

		private Model _floorModel { get; set; }	// Model of the Floor.

		// Initialize the Camera and use a Fixed camera by default.
		Camera msCamera = new Camera(true);

		#endregion

		#region Initialization

		/// <summary>
		/// Constructor
		/// </summary>
		public DemoBase()
		{
			GraphicsDeviceManager = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			// If we should not Limit the FPS.
			if (!LIMIT_FPS)
			{
				// Make the game run as fast as possible (i.e. don't limit the FPS).
				this.IsFixedTimeStep = false;
				GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
			}
			
			// Set the resolution.
			GraphicsDeviceManager.PreferredBackBufferWidth = WINDOW_WIDTH;
			GraphicsDeviceManager.PreferredBackBufferHeight = WINDOW_HEIGHT;

			// Set the Title of the Window.
			Window.Title = "Dynamic Particle System Framework Demo";

			// Hide the mouse cursor.
			this.IsMouseVisible = false;

			// Setup default property values.
			ShowText = true;
			ShowFloor = true;
			WorldMatrix = Matrix.Identity;
			ViewMatrix = Matrix.Identity;
			ProjectionMatrix = Matrix.Identity;
		}

		/// <summary>
		/// Load your graphics content
		/// </summary>
		protected override void LoadContent()
		{
			SpriteBatch = new SpriteBatch(GraphicsDevice);

			// Load fonts and models for test application
			Font = Content.Load<SpriteFont>("Fonts/font");
			_floorModel = Content.Load<Model>("grid");

			// Setup our render target to draw to when we want draws to persist across multiple frames
			_renderTarget = new RenderTarget2D(GraphicsDeviceManager.GraphicsDevice, GraphicsDeviceManager.PreferredBackBufferWidth, GraphicsDeviceManager.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

			// Specify vertices indicating positive axis directions
			int iLineLength = 50;
			msaAxisDirectionVertices = new VertexPositionColor[6];
			msaAxisDirectionVertices[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);
			msaAxisDirectionVertices[1] = new VertexPositionColor(new Vector3(iLineLength, 1, 0), Color.Red);
			msaAxisDirectionVertices[2] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Green);
			msaAxisDirectionVertices[3] = new VertexPositionColor(new Vector3(0, iLineLength, 0), Color.Green);
			msaAxisDirectionVertices[4] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Blue);
			msaAxisDirectionVertices[5] = new VertexPositionColor(new Vector3(0, 1, iLineLength), Color.Blue);
			
			mcAxisEffect = new BasicEffect(GraphicsDevice);
			mcAxisEffect.VertexColorEnabled = true;
			mcAxisEffect.LightingEnabled = false;
			mcAxisEffect.TextureEnabled = false;
			mcAxisEffect.FogEnabled = false;
			mcAxisVertexDeclaration = VertexPositionColor.VertexDeclaration;
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

			// Allow the camera to be moved around, even if the particle systems are paused

			// Update the World, View, and Projection matrices
			UpdateWorldViewProjectionMatrices();


			// Update the Quad Particle Systems to know where the Camera is so that they can display
			// the particles as billboards if needed (i.e. have particle always face the camera).
			_particleSystemManager.SetCameraPositionForAllParticleSystems(msCamera.Position);

			// Set the World, View, and Projection Matrices for the Particle Systems
			_particleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(WorldMatrix, ViewMatrix, ProjectionMatrix);

			// If the Game is Paused
			if (Paused)
			{
				// Update the particle systems with 0 elapsed time, just to allow the particles to rotate to face the camera.
				_particleSystemManager.UpdateAllParticleSystems(0);

				// Exit without updating anything
				return;
			}

			// If the Current Particle System is Initialized
			if (_currentDPSFDemoParticleSystemWrapper != null && _currentDPSFDemoParticleSystemWrapper.IsInitialized)
			{
				// If Static Particles should be drawn
				if (_drawStaticParticles)
				{
					// If the Static Particles haven't been drawn yet
					if (!_staticParticlesDrawn)
					{
						// Draw this frame to a Render Target so we can have it persist across frames.
						SetupToDrawToRenderTarget(true);

						// Update the Particle System iteratively by the Time Step amount until the 
						// Particle System behavior over the Total Time has been drawn
						float fElapsedTime = 0;
						while (fElapsedTime < _staticParticleTotalTime)
						{
							// Update and draw this frame of the Particle System
							_particleSystemManager.UpdateAllParticleSystems(_staticParticleTimeStep);
							_particleSystemManager.DrawAllParticleSystems();
							fElapsedTime += _staticParticleTimeStep;
						}
						_staticParticlesDrawn = true;

						SpriteBatch.Begin();
						SpriteBatch.DrawString(Font, "F6 to continue", new Vector2(310, 25), Color.LawnGreen);
						SpriteBatch.End();
					}
				}
				// Else the Particle Systems should be drawn normally
				else
				{
					// Update all Particle Systems manually
					_particleSystemManager.UpdateAllParticleSystems((float)cGameTime.ElapsedGameTime.TotalSeconds);
				}


				// If the Sphere is Visible and we are on the Smoke Particle System
				if (_sphereObject.bVisible && _currentParticleSystem == ParticleSystemEffects.Smoke)
				{
					// Update it
					_sphereObject.Update((float)cGameTime.ElapsedGameTime.TotalSeconds);

					// Update the PS's External Object Position to the Sphere's Position
					_mcSmokeDPSFDemoParticleSystemWrapper.mcExternalObjectPosition = _sphereObject.sPosition;

					// If the Sphere has been alive long enough
					if (_sphereObject.cTimeAliveInSeconds > TimeSpan.FromSeconds(6.0f))
					{
						_sphereObject.bVisible = false;
						_mcSmokeDPSFDemoParticleSystemWrapper.StopParticleAttractionAndRepulsionToExternalObject();
					}
				}
			}
		
			// Update any other Drawable Game Components.
			base.Update(cGameTime);

			// If we are drawing garbage collection info.
			if (ShowPerformanceText)
			{
				// Record how much Garbage is waiting to be collected in Kilobytes.
				_garbageCurrentAmountInKB = GC.GetTotalMemory(false) / 1024f;

				// Increment the number of updates that have been performed in the past second.
				_updatesPerSecond++;
			}
		}

		/// <summary>
		/// Updates the World, View, and Projection matrices according to the Camera's current position.
		/// </summary>
		private void UpdateWorldViewProjectionMatrices()
		{
			// Compute the Aspect Ratio of the window
			float fAspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;

			// Setup the View matrix depending on which Camera mode is being used.
			// If we are using the Fixed Camera.
			if (msCamera.bUsingFixedCamera)
			{
				// Set up the View matrix according to the Camera's arc, rotation, and distance from the Offset position.
				ViewMatrix = Matrix.CreateTranslation(msCamera.sFixedCameraLookAtPosition) *
									 Matrix.CreateRotationY(MathHelper.ToRadians(msCamera.fCameraRotation)) *
									 Matrix.CreateRotationX(MathHelper.ToRadians(msCamera.fCameraArc)) *
									 Matrix.CreateLookAt(new Vector3(0, 0, -msCamera.fCameraDistance),
														 new Vector3(0, 0, 0), Vector3.Up);
			}
			// Else we are using the Free Camera
			else
			{
				// Set up our View matrix specifying the Camera position, a point to look-at, and a direction for which way is up.
				ViewMatrix = Matrix.CreateLookAt(msCamera.sVRP, msCamera.sVRP + msCamera.cVPN, msCamera.cVUP);
			}

			// Setup the Projection matrix by specifying the field of view (1/4 pi), aspect ratio, and the near and far clipping planes
			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, fAspectRatio, 1, 10000);
		}

		/// <summary>
		/// This is called when the game should draw itself
		/// </summary>
		protected override void Draw(GameTime cGameTime)
		{
			// If Static Particles were drawn to the Render Target, draw the Render Target to the screen and exit without drawing anything else.
			if (_drawStaticParticles)
			{
				DrawRenderTargetToScreen();
				return;
			}

			// Clear the scene
			GraphicsDevice.Clear(BACKGROUND_COLOR);

			// If the screen should NOT be cleared each frame, draw to a render target instead of right to the screen.
			if (!ClearScreenEveryFrame)
			{
				SetupToDrawToRenderTarget(_clearScreenEveryFrameJustToggled);
				_clearScreenEveryFrameJustToggled = false;
			}
			
			// Draw the Floor at the origin (0,0,0) and any other models
			DrawModels(WorldMatrix, ViewMatrix, ProjectionMatrix);

			// If the Axis' should be drawn
			if (ShowPositiveDirectionAxis)
			{
				// Draw lines at the origin (0,0,0) indicating positive axis directions
				DrawAxis(WorldMatrix, ViewMatrix, ProjectionMatrix);
			}


			// Draw any other Drawable Game Components that may need to be drawn.
			// Call this before drawing our Particle Systems, so that our 2D Sprite particles
			// show up on top of the any other 2D Sprites drawn.
			base.Draw(cGameTime);

			// Draw the Particle Systems manually
			_particleSystemManager.DrawAllParticleSystems();


			// Update the Frames Per Second to be displayed
			FPS.Update((float)cGameTime.ElapsedGameTime.TotalSeconds);

			// Draw the Text to the screen last, so it is always on top
			DrawText();


			// If we were drawing this frame to the Render Target, draw the Render Target to the screen.
			if (!ClearScreenEveryFrame)
			{
				DrawRenderTargetToScreen();
			}
		}

		/// <summary>
		/// Sets up the app to draw to the render target instead of directly to the screen.
		/// </summary>
		/// <param name="clearRenderTarget">If true the Render Target will be cleared before anything else is drawn to it.</param>
		private void SetupToDrawToRenderTarget(bool clearRenderTarget)
		{
			// Draw this frame to a Render Target so we can have it persist across frames.
			GraphicsDevice.SetRenderTarget(_renderTarget);

			// If we are about to start drawing to the Render Target, clear out any contents it may already have.
			if (clearRenderTarget)
				GraphicsDevice.Clear(BACKGROUND_COLOR);
		}

		/// <summary>
		/// Draws the Render Target contents to the screen.
		/// </summary>
		private void DrawRenderTargetToScreen()
		{
			GraphicsDevice.SetRenderTarget(null);		// Start drawing to the screen again instead of to the Render Target.
			GraphicsDevice.Clear(BACKGROUND_COLOR);
			SpriteBatch.Begin();
			// Draw the Render Target contents to the screen
			SpriteBatch.Draw(_renderTarget, new Rectangle(0, 0, _renderTarget.Width, _renderTarget.Height), Color.White);
			SpriteBatch.End();
		}

		/// <summary>
		/// Function to draw Text to the screen
		/// </summary>
		void DrawText()
		{
			// If no Text should be shown
			if (!ShowText)
			{
				// Exit the function before drawing any Text
				return;
			}

            // Get area on screen that it is safe to draw text to (so that we are sure it will be displayed on the screen).
		    Rectangle textSafeArea = GetTextSafeArea();

            // Setup the 
		    var toolsToDrawText = new DrawTextRequirements()
		                              {
                                          TextWriter = SpriteBatch,
		                                  Font = Font,
		                                  TextSafeArea = textSafeArea,
		                                  ControlTextColor = CONTROL_TEXT_COLOR,
		                                  PropertyTextColor = PROPERTY_TEXT_COlOR,
		                                  ValueTextColor = VALUE_TEXT_COLOR
		                              };

			// If we don't have a handle to a particle system, it is because we serialized it
			if (_currentDPSFDemoParticleSystemWrapper == null)
			{
				SpriteBatch.Begin();
				SpriteBatch.DrawString(Font, "Particle system has been serialized to the file: " + _serializedParticleSystemFileName + ".", new Vector2(25, 200), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "To deserialize the particle system from the file, restoring the instance of", new Vector2(25, 225), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "the particle system,", new Vector2(25, 250), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "press F9", new Vector2(210, 250), CONTROL_TEXT_COLOR);
				SpriteBatch.End();
				return;
			}
			
			// If the Particle System has been destroyed, just write that to the screen and exit
			if (!_currentDPSFDemoParticleSystemWrapper.IsInitialized)
			{
				SpriteBatch.Begin();
				SpriteBatch.DrawString(Font, "The current particle system has been destroyed.", new Vector2(140, 200), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "Press G / H to switch to a different particle system.", new Vector2(125, 225), PROPERTY_TEXT_COlOR);
				SpriteBatch.End();
				return;
			}

			// Get the Name of the Particle System and how many Particles are currently Active
			string sEffectName = _currentDPSFDemoParticleSystemWrapper.Name;
            int iNumberOfActiveParticles = _currentDPSFDemoParticleSystemWrapper.TotalNumberOfActiveParticles;
            int iNumberOfParticlesAllocatedInMemory = _currentDPSFDemoParticleSystemWrapper.TotalNumberOfParticlesAllocatedInMemory;

			// Convert values to strings
			string sFPSValue = FPS.CurrentFPS.ToString();
			string sAvgFPSValue = FPS.AverageFPS.ToString("0.0");
			string sTotalParticleCountValue = iNumberOfActiveParticles.ToString();
			string sEmitterOnValue = (_currentDPSFDemoParticleSystemWrapper.Emitter.EmitParticlesAutomatically ? "On" : "Off");
			string sParticleSystemEffectValue = sEffectName;
			string sParticlesPerSecondValue = _currentDPSFDemoParticleSystemWrapper.Emitter.ParticlesPerSecond.ToString("0.00");
			string sCameraModeValue = msCamera.bUsingFixedCamera ? "Fixed" : "Free";
			string sPSSpeedScale = _particleSystemManager.SimulationSpeed.ToString("0.0");
			string sCameraPosition = "(" + msCamera.Position.X.ToString("0") + "," + msCamera.Position.Y.ToString("0") + "," + msCamera.Position.Z.ToString("0") + ")";
			string sAllocatedParticles = iNumberOfParticlesAllocatedInMemory.ToString();
			string sTexture = "N/A";
			if (_currentDPSFDemoParticleSystemWrapper.Texture != null)
			{
				sTexture = _currentDPSFDemoParticleSystemWrapper.Texture.Name.TrimStart("Textures/".ToCharArray());
			}

			// Draw all of the text.
			SpriteBatch.Begin();

            // If the Particle System is Paused, draw a Paused message.
            if (Paused)
            {
                SpriteBatch.DrawString(Font, "Paused", new Vector2(textSafeArea.Left + 350, textSafeArea.Top + 25), VALUE_TEXT_COLOR);
            }

            // Draw text that is always displayed.
			SpriteBatch.DrawString(Font, "FPS:", new Vector2(textSafeArea.Left + 5, textSafeArea.Bottom - 50), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sFPSValue, new Vector2(textSafeArea.Left + 50, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Allocated:", new Vector2(textSafeArea.Left + 120, textSafeArea.Bottom - 50), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sAllocatedParticles, new Vector2(textSafeArea.Left + 210, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			//mcSpriteBatch.DrawString(mcFont, "Position:", new Vector2(textSafeArea.Left + 275, textSafeArea.Bottom - 75), sPropertyColor);
			SpriteBatch.DrawString(Font, sCameraPosition, new Vector2(textSafeArea.Left + 280, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Texture:", new Vector2(textSafeArea.Left + 440, textSafeArea.Bottom - 50), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sTexture, new Vector2(textSafeArea.Left + 520, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Speed:", new Vector2(textSafeArea.Right - 100, textSafeArea.Bottom - 50), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sPSSpeedScale, new Vector2(textSafeArea.Right - 35, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Avg:", new Vector2(textSafeArea.Left + 5, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sAvgFPSValue, new Vector2(textSafeArea.Left + 50, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Particles:", new Vector2(textSafeArea.Left + 120, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sTotalParticleCountValue, new Vector2(textSafeArea.Left + 205, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Emitter:", new Vector2(textSafeArea.Left + 275, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sEmitterOnValue, new Vector2(textSafeArea.Left + 345, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Particles Per Second:", new Vector2(textSafeArea.Left + 390, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sParticlesPerSecondValue, new Vector2(textSafeArea.Left + 585, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Camera:", new Vector2(textSafeArea.Left + 660, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sCameraModeValue, new Vector2(textSafeArea.Left + 740, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Effect:", new Vector2(textSafeArea.Left + 5, textSafeArea.Top + 2), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sParticleSystemEffectValue, new Vector2(textSafeArea.Left + 70, textSafeArea.Top + 2), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Show/Hide Controls:", new Vector2(textSafeArea.Right - 260, textSafeArea.Top + 2), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, "F1 - F4", new Vector2(textSafeArea.Right - 70, textSafeArea.Top + 2), CONTROL_TEXT_COLOR);


            // Display particle system specific values.
            _currentDPSFDemoParticleSystemWrapper.DrawStatusText(toolsToDrawText);

            // If the Particle System specific Controls should be shown, display them.
            if (_showParticleSystemControls)
            {
                _currentDPSFDemoParticleSystemWrapper.DrawInputControlsText(toolsToDrawText);
            }

            // If the Common Controls should be shown, display them.
            if (_showCommonControls)
            {
                SpriteBatch.DrawString(Font, "Change Particle System:", new Vector2(5, 25), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "G / H", new Vector2(235, 25), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Toggle Emitter On/Off:", new Vector2(5, 50), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "Delete", new Vector2(220, 50), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Increase/Decrease Emitter Speed:", new Vector2(5, 75), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "+ / -", new Vector2(320, 75), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Add Particle:", new Vector2(5, 100), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "Insert(one), Home(many), PgUp(max)", new Vector2(130, 100), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Move Emitter:", new Vector2(5, 125), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "A/D, W/S, Q/E", new Vector2(135, 125), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Rotate Emitter:", new Vector2(5, 150), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "J/L(yaw), I/Vertex(pitch), U/O(roll)", new Vector2(150, 150), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Rotate Emitter Around Pivot:", new Vector2(5, 175), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "Y + Rotate Emitter", new Vector2(275, 175), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Reset Emitter's Position and Orientation:", new Vector2(5, 200), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "Z", new Vector2(375, 200), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Toggle Floor:", new Vector2(485, 25), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "F", new Vector2(610, 25), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Toggle Axis:", new Vector2(650, 25), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "F7", new Vector2(770, 25), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Toggle Full Screen:", new Vector2(485, 50), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "End", new Vector2(665, 50), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Toggle Camera Mode:", new Vector2(485, 75), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "PgDown", new Vector2(690, 75), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Reset Camera Position:", new Vector2(485, 100), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "R", new Vector2(705, 100), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Change Texture:", new Vector2(485, 125), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "T / Shift + T", new Vector2(640, 125), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Pause Particle System:", new Vector2(485, 150), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "Spacebar", new Vector2(700, 150), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Speed Up/Down PS:", new Vector2(485, 175), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "* / /", new Vector2(680, 175), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Draw Static Particles:", new Vector2(485, 200), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "F6", new Vector2(690, 200), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Clear Screen Each Frame:", new Vector2(485, 225), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "F5", new Vector2(730, 225), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Create Animation Images:", new Vector2(485, 250), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "F8", new Vector2(725, 250), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Serialize Particle System:", new Vector2(485, 275), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "F9", new Vector2(725, 275), CONTROL_TEXT_COLOR);

                SpriteBatch.DrawString(Font, "Draw Performance Info:", new Vector2(485, 300), PROPERTY_TEXT_COlOR);
                SpriteBatch.DrawString(Font, "F10", new Vector2(705, 300), CONTROL_TEXT_COLOR);
            }

			// If the Camera Controls should be shown
			if (ShowCameraControls)
			{
				// If we are using a Fixed Camera
				if (msCamera.bUsingFixedCamera)
				{
					SpriteBatch.DrawString(Font, "Fixed Camera Controls:", new Vector2(5, GraphicsDeviceManager.PreferredBackBufferHeight - 125), PROPERTY_TEXT_COlOR);
					SpriteBatch.DrawString(Font, "Keys: Left/Right Arrows, Up/Down Arrows, Num0/Num1", new Vector2(15, GraphicsDeviceManager.PreferredBackBufferHeight - 100), CONTROL_TEXT_COLOR);
					SpriteBatch.DrawString(Font, "Mouse: Left Button + X/Y Movement, Right Button + Y Movement", new Vector2(15, GraphicsDeviceManager.PreferredBackBufferHeight - 75), CONTROL_TEXT_COLOR);
				}
				// Else we are using a Free Camera
				else
				{
					SpriteBatch.DrawString(Font, "Free Camera Controls", new Vector2(5, GraphicsDeviceManager.PreferredBackBufferHeight - 125), PROPERTY_TEXT_COlOR);
					SpriteBatch.DrawString(Font, "Keys: Left/Right Arrows, Up/Down Arrows, Num0/Num1, Num4/Num6, Num8/Num2", new Vector2(15, GraphicsDeviceManager.PreferredBackBufferHeight - 100), CONTROL_TEXT_COLOR);
					SpriteBatch.DrawString(Font, "Mouse: Left Button + X/Y Movement, Right Button + X/Y Movement, Scroll Wheel", new Vector2(15, GraphicsDeviceManager.PreferredBackBufferHeight - 75), CONTROL_TEXT_COLOR);
				}
			}

			// If we should draw the number of bytes allocated in memory
			if (ShowPerformanceText)
			{
				SpriteBatch.DrawString(Font, "Update Time (ms): " + _particleSystemManager.TotalPerformanceTimeToDoUpdatesInMilliseconds.ToString("0.000"), new Vector2(529, GraphicsDeviceManager.PreferredBackBufferHeight - 250), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "Draw Time (ms): " + _particleSystemManager.TotalPerformanceTimeToDoDrawsInMilliseconds.ToString("0.000"), new Vector2(545, GraphicsDeviceManager.PreferredBackBufferHeight - 225), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "Garbage Allocated (KB): " + _garbageCurrentAmountInKB.ToString("0.0"), new Vector2(480, GraphicsDeviceManager.PreferredBackBufferHeight - 200), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "Avg Garbage Per Update (KB): " + _garbageAverageCreatedPerUpdateInKB.ToString("0.000"), new Vector2(440, GraphicsDeviceManager.PreferredBackBufferHeight - 175), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "Avg Garbage Per Frame (KB): " + _garbageAverageCreatedPerFrameInKB.ToString("0.000"), new Vector2(445, GraphicsDeviceManager.PreferredBackBufferHeight - 150), PROPERTY_TEXT_COlOR);
			}

			// Stop drawing text
			SpriteBatch.End();
		}

		/// <summary>
		/// Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).
		/// </summary>
		/// <returns>Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).</returns>
		Rectangle GetTextSafeArea()
		{
			return GetTextSafeArea(0.9f);
		}

		/// <summary>
		/// Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).
		/// </summary>
		/// <param name="fNormalizedPercent">The amount of screen space (normalized between 0.0 - 1.0) that should 
		/// safe to draw to (e.g. 0.8 to have a 10% border on all sides)</param>
		/// <returns>Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).</returns>
		Rectangle GetTextSafeArea(float fNormalizedPercent)
		{
			Rectangle rTextSafeArea = new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y,
											GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
#if (XBOX)
			// Find Title Safe area of Xbox 360.
			float fBorder = (1 - fNormalizedPercent) / 2;
			rTextSafeArea.X = (int)(fBorder * rTextSafeArea.Width);
			rTextSafeArea.Y = (int)(fBorder * rTextSafeArea.Height);
			rTextSafeArea.Width = (int)(fNormalizedPercent * rTextSafeArea.Width);
			rTextSafeArea.Height = (int)(fNormalizedPercent * rTextSafeArea.Height);
#endif
			return rTextSafeArea;
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
			if (ShowFloor)
			{
				_floorModel.Draw(cWorldMatrix, cViewMatrix, cProjectionMatrix);
			}

			// If the Sphere should be visible
			if (_sphereObject.bVisible)
			{
				_sphereModel.Draw(Matrix.CreateScale(_sphereObject.fSize) * Matrix.CreateTranslation(_sphereObject.sPosition), cViewMatrix, cProjectionMatrix);
			}
		}

		/// <summary>
		/// Helper for drawing lines showing the positive axis directions
		/// </summary>
		void DrawAxis(Matrix cWorldMatrix, Matrix cViewMatrix, Matrix cProjectionMatrix)
		{
			mcAxisEffect.World = cWorldMatrix;
			mcAxisEffect.View = cViewMatrix;
			mcAxisEffect.Projection = cProjectionMatrix;

			// Draw the lines
			foreach (EffectPass cPass in mcAxisEffect.CurrentTechnique.Passes)
			{
				cPass.Apply();
				GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, msaAxisDirectionVertices, 0, 3, mcAxisVertexDeclaration);
			}
		}


		#endregion

		#region Handle Input

		/// <summary>
		/// Gets and processes all user input
		/// </summary>
		void ProcessInput(GameTime cGameTime)
		{
			// Save how long it's been since the last time Input was Handled
			float fTimeInSeconds = (float)cGameTime.ElapsedGameTime.TotalSeconds;

			// Save the state of the input devices at this frame
			KeyboardManager.UpdateKeyboardStateForThisFrame(cGameTime.ElapsedGameTime);
			MouseManager.UpdateMouseStateForThisFrame(cGameTime.ElapsedGameTime);
			GamePadsManager.UpdateGamePadStatesForThisFrame(cGameTime.ElapsedGameTime);

			// If we should Exit
			if (KeyboardManager.KeyIsDown(Keys.Escape) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.Back))
			{
				Exit();
			}

			// If we are currently showing the Splash Screen and a key was pressed, skip the Splash Screen.
			if (_mcDPSFSplashScreenDPSFDemoParticleSystemWrapper != null && 
				((KeyboardManager.CurrentKeyboardState.GetPressedKeys().Length > 0 && KeyboardManager.CurrentKeyboardState.GetPressedKeys()[0] != Keys.None) || 
				(MouseManager.CurrentMouseState.LeftButton == ButtonState.Pressed || MouseManager.CurrentMouseState.RightButton == ButtonState.Pressed) ||
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.A | Buttons.B | Buttons.X | Buttons.Y | Buttons.Start))))
			{
				_mcDPSFSplashScreenDPSFDemoParticleSystemWrapper.IsSplashScreenComplete = true;
				return;
			}

			// If we should toggle showing the Floor
			if (KeyboardManager.KeyWasJustPressed(Keys.F))
			{
				ShowFloor = !ShowFloor;
			}

			// If we should toggle Pausing the game
			if (KeyboardManager.KeyWasJustPressed(Keys.Space) || GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.Start))
			{
				Paused = !Paused;
			}

			// If we should toggle between Full Screen and Windowed mode
			if (KeyboardManager.KeyWasJustPressed(Keys.End))
			{
				GraphicsDeviceManager.ToggleFullScreen();
			}

			// If we should toggle showing the Common Controls
			if (KeyboardManager.KeyWasJustPressed(Keys.F1))
			{
				_showCommonControls = !_showCommonControls;
			}

			// If we should toggle showing the Particle System specific Controls
			if (KeyboardManager.KeyWasJustPressed(Keys.F2))
			{
				_showParticleSystemControls = !_showParticleSystemControls;
			}

			// If we should toggle showing the Camera Controls
			if (KeyboardManager.KeyWasJustPressed(Keys.F3))
			{
				ShowCameraControls = !ShowCameraControls;
			}

			// If we should toggle showing the Common Controls
			if (KeyboardManager.KeyWasJustPressed(Keys.F4))
			{
				ShowText = !ShowText;
			}

			// If we should toggle Clearing the Screen each Frame
			if (KeyboardManager.KeyWasJustPressed(Keys.F5))
			{
				ClearScreenEveryFrame = !ClearScreenEveryFrame;
				_clearScreenEveryFrameJustToggled = true;
			}
			
			// If the particle lifetimes should be drawn in one frame
			if (KeyboardManager.KeyWasJustPressed(Keys.F6))
			{
				_drawStaticParticles = !_drawStaticParticles;
				_staticParticlesDrawn = false;
			}

			// If the Axis should be toggled on/off
			if (KeyboardManager.KeyWasJustPressed(Keys.F7))
			{
				ShowPositiveDirectionAxis = !ShowPositiveDirectionAxis;
			}
#if (WINDOWS)
			// If the PS should be drawn to files
			if (KeyboardManager.KeyWasJustPressed(Keys.F8))
			{
				// Draw the Particle System Animation to a series of Image Files
				_particleSystemManager.DrawAllParticleSystemsAnimationToFiles(GraphicsDevice, _drawPSToFilesImageWidth, _drawPSToFilesImageHeight, 
							_drawPSToFilesDirectoryName, _drawPSToFilesTotalTime, _drawPSToFilesTimeStep, _createAnimatedGIF, _createTileSetImage);
			}

			// If the PS should be serialized to a file
			if (KeyboardManager.KeyWasJustPressed(Keys.F9))
			{
				// Only particle systems that do not inherit the DrawableGameComponent can be serialized.
				if (!DPSFHelper.DPSFInheritsDrawableGameComponent)
				{
					// If we have the particle system right now
					if (_currentDPSFDemoParticleSystemWrapper != null)
					{
						// Serialize the particle system into a file
						System.IO.Stream stream = System.IO.File.Open("SerializedParticleSystem.dat", System.IO.FileMode.Create);
						System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
						formatter.Serialize(stream, _currentDPSFDemoParticleSystemWrapper);
						stream.Close();

						// Remove the particle system from the manager and destroy the particle system in memory
						_particleSystemManager.RemoveParticleSystem(_currentDPSFDemoParticleSystemWrapper);
						_currentDPSFDemoParticleSystemWrapper.Destroy();
						_currentDPSFDemoParticleSystemWrapper = null;
					}
					// Else we don't have the particle system right now
					else
					{
						// Deserialize the particle system from a file
						System.IO.Stream stream = System.IO.File.Open("SerializedParticleSystem.dat", System.IO.FileMode.Open);
						System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        _currentDPSFDemoParticleSystemWrapper = (IWrapDPSFDemoParticleSystems)formatter.Deserialize(stream);
						stream.Close();

						try
						{
							// Setup the particle system properties that couldn't be serialized
							_currentDPSFDemoParticleSystemWrapper.InitializeNonSerializableProperties(this, this.GraphicsDevice, this.Content);
						}
						// Catch the case where the Particle System requires a texture, but one wasn't loaded
						catch (ArgumentNullException)
						{
							// Assign the particle system a texture to use
							_currentDPSFDemoParticleSystemWrapper.SetTexture("Textures/Bubble");
						}

						// Readd the particle system to the particle system manager
						_particleSystemManager.AddParticleSystem(_currentDPSFDemoParticleSystemWrapper);
					}
				}
			}
#endif
			// If the Performance Profiling was toggled
			if (KeyboardManager.KeyWasJustPressed(Keys.F10))
			{
				// Toggle if the Performance Profiling text should be drawn
				ShowPerformanceText = !ShowPerformanceText;
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
				if (KeyboardManager.KeyIsDown(Keys.NumPad1) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D1)) ||
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickUp) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftStick)))
				{
					msCamera.fCameraArc -= fTimeInSeconds * 25;
				}

				if (KeyboardManager.KeyIsDown(Keys.NumPad0) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D0)) ||
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickDown) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftStick)))
				{
					msCamera.fCameraArc += fTimeInSeconds * 25;
				}

				// If the Camera should rotate horizontally
				if (KeyboardManager.KeyIsDown(Keys.Right) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickRight))
				{
					msCamera.fCameraRotation -= fTimeInSeconds * 50;
				}

				if (KeyboardManager.KeyIsDown(Keys.Left) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickLeft))
				{
					msCamera.fCameraRotation += fTimeInSeconds * 50;
				}

				// If Camera should be zoomed out
				if (KeyboardManager.KeyIsDown(Keys.Down) || 
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickDown) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftStick)))
				{
					msCamera.fCameraDistance += fTimeInSeconds * 250;
				}

				// If Camera should be zoomed in
				if (KeyboardManager.KeyIsDown(Keys.Up) || 
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickUp) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftStick)))
				{
					msCamera.fCameraDistance -= fTimeInSeconds * 250;
				}


				// Calculate how much the Mouse was moved
				int iXMovement = MouseManager.CurrentMouseState.X - MouseManager.PreviousMouseState.X;
				int iYMovement = MouseManager.CurrentMouseState.Y - MouseManager.PreviousMouseState.Y;
				int iZMovement = MouseManager.CurrentMouseState.ScrollWheelValue - MouseManager.PreviousMouseState.ScrollWheelValue;

				const float fMOUSE_MOVEMENT_SPEED = 0.5f;
				const float fMOUSE_ROTATION_SPEED = 0.5f;

				// If the Left Mouse Button is pressed
				if (MouseManager.CurrentMouseState.LeftButton == ButtonState.Pressed)
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
				if (MouseManager.CurrentMouseState.RightButton == ButtonState.Pressed)
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

				if (KeyboardManager.KeyIsDown(Keys.Decimal))
				{
					iSPEED = 100;
				}

				// If the Camera should move forward
				if (KeyboardManager.KeyIsDown(Keys.Up))
				{
					msCamera.MoveCameraForwardOrBackward(iSPEED * fTimeInSeconds);
				}

				// If the Camera should move backwards
				if (KeyboardManager.KeyIsDown(Keys.Down))
				{
					msCamera.MoveCameraForwardOrBackward(-iSPEED * fTimeInSeconds);
				}

				// If the Camera should strafe right
				if (KeyboardManager.KeyIsDown(Keys.Right))
				{
					msCamera.MoveCameraHorizontally(-iSPEED * fTimeInSeconds);
				}

				// If the Camera should strafe left
				if (KeyboardManager.KeyIsDown(Keys.Left))
				{
					msCamera.MoveCameraHorizontally(iSPEED * fTimeInSeconds);
				}

				// If the Camera should move upwards
				if (KeyboardManager.KeyIsDown(Keys.NumPad1) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D1)))
				{
					msCamera.MoveCameraVertically((iSPEED / 2) * fTimeInSeconds);
				}

				// If the Camera should move downwards
				if (KeyboardManager.KeyIsDown(Keys.NumPad0) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D0)))
				{
					msCamera.MoveCameraVertically((-iSPEED / 2) * fTimeInSeconds);
				}

				// If the Camera should yaw left
				if (KeyboardManager.KeyIsDown(Keys.NumPad4) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D4)))
				{
					msCamera.RotateCameraHorizontally(fROTATE_SPEED * fTimeInSeconds);
				}

				// If the Camera should yaw right
				if (KeyboardManager.KeyIsDown(Keys.NumPad6) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D6)))
				{
					msCamera.RotateCameraHorizontally(-fROTATE_SPEED * fTimeInSeconds);
				}

				// If the Camera should pitch up
				if (KeyboardManager.KeyIsDown(Keys.NumPad8) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D8)))
				{
					msCamera.RotateCameraVertically(-fROTATE_SPEED * fTimeInSeconds);
				}

				// If the Camera should pitch down
				if (KeyboardManager.KeyIsDown(Keys.NumPad2) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D2)))
				{
					msCamera.RotateCameraVertically(fROTATE_SPEED * fTimeInSeconds);
				}


				// Calculate how much the Mouse was moved
				int iXMovement = MouseManager.CurrentMouseState.X - MouseManager.PreviousMouseState.X;
				int iYMovement = MouseManager.CurrentMouseState.Y - MouseManager.PreviousMouseState.Y;
				int iZMovement = MouseManager.CurrentMouseState.ScrollWheelValue - MouseManager.PreviousMouseState.ScrollWheelValue;

				const float fMOUSE_MOVEMENT_SPEED = 0.5f;
				const float fMOUSE_ROTATION_SPEED = 0.005f;

				// If the Left Mouse Button is pressed
				if (MouseManager.CurrentMouseState.LeftButton == ButtonState.Pressed)
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
				if (MouseManager.CurrentMouseState.RightButton == ButtonState.Pressed)
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
			if (KeyboardManager.KeyWasJustPressed(Keys.PageDown))
			{
				msCamera.bUsingFixedCamera = !msCamera.bUsingFixedCamera;
			}

			// If the Camera values should be Reset
			if (KeyboardManager.KeyIsDown(Keys.R) || 
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftShoulder) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightShoulder)))
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
			// If the Current Particle System should be changed to the next Particle System
			if (KeyboardManager.KeyWasJustPressed(Keys.H) || GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.RightTrigger))
			{
				_currentParticleSystem++;
				if (_currentParticleSystem > ParticleSystemEffects.LastInList)
				{
					_currentParticleSystem = 0;
				}

				// Initialize the new Particle System
				InitializeCurrentParticleSystem();
			}
			// Else if the Current Particle System should be changed back to the previous Particle System
			else if (KeyboardManager.KeyWasJustPressed(Keys.G) || GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.LeftTrigger))
			{
				_currentParticleSystem--;
				if (_currentParticleSystem < 0)
				{
					_currentParticleSystem = ParticleSystemEffects.LastInList;
				}

				// Initialize the new Particle System
				InitializeCurrentParticleSystem();
			}

			// If the Current Particle System is not Initialized
			if (_currentDPSFDemoParticleSystemWrapper == null || !_currentDPSFDemoParticleSystemWrapper.IsInitialized)
			{
				return;
			}

			// Define how fast the user can move and rotate the Emitter
			float fEmitterMoveDelta = 75 * fElapsedTimeInSeconds;
			float fEmitterRotateDelta = MathHelper.Pi * fElapsedTimeInSeconds;

			// If the Shift key is down, move faster
			if (KeyboardManager.KeyIsDown(Keys.LeftShift) || KeyboardManager.KeyIsDown(Keys.RightShift))
			{
				fEmitterMoveDelta *= 2;
			}

			// Check if the Emitter should be moved
			if (KeyboardManager.KeyIsDown(Keys.W) || 
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadUp) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				_currentDPSFDemoParticleSystemWrapper.Emitter.PositionData.Position += Vector3.Up * fEmitterMoveDelta;
			}

			if (KeyboardManager.KeyIsDown(Keys.S) || 
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadDown) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				_currentDPSFDemoParticleSystemWrapper.Emitter.PositionData.Position += Vector3.Down * fEmitterMoveDelta;
			}

			if (KeyboardManager.KeyIsDown(Keys.A) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadLeft))
			{
				_currentDPSFDemoParticleSystemWrapper.Emitter.PositionData.Position += Vector3.Left * fEmitterMoveDelta;
			}

			if (KeyboardManager.KeyIsDown(Keys.D) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadRight))
			{
				_currentDPSFDemoParticleSystemWrapper.Emitter.PositionData.Position += Vector3.Right * fEmitterMoveDelta;
			}

			if (KeyboardManager.KeyIsDown(Keys.E) || 
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadUp) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				_currentDPSFDemoParticleSystemWrapper.Emitter.PositionData.Position += Vector3.Forward * fEmitterMoveDelta;
			}

			if (KeyboardManager.KeyIsDown(Keys.Q) ||
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadDown) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				_currentDPSFDemoParticleSystemWrapper.Emitter.PositionData.Position += Vector3.Backward * fEmitterMoveDelta;
			}

			// Check if the Emitter should be rotated
			if ((_currentParticleSystem != ParticleSystemEffects.Star && _currentParticleSystem != ParticleSystemEffects.Ball) || 
				(!KeyboardManager.KeyIsDown(Keys.V) && !KeyboardManager.KeyIsDown(Keys.B) && !KeyboardManager.KeyIsDown(Keys.P)))
			{
				if (KeyboardManager.KeyIsDown(Keys.J) || 
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickLeft) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
				{
					// If we should Rotate the Emitter around the Pivot Point
					if (KeyboardManager.KeyIsDown(Keys.Y))
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(-fEmitterRotateDelta, 0.0f, 0.0f));
					}
					// Else we should just Rotate the Emitter about its center
					else
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(-fEmitterRotateDelta, 0.0f, 0.0f));
					}
				}

				if (KeyboardManager.KeyIsDown(Keys.L) || 
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickRight) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
				{
					// If we should Rotate the Emitter around the Pivot Point
					if (KeyboardManager.KeyIsDown(Keys.Y))
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(fEmitterRotateDelta, 0.0f, 0.0f));
					}
					// Else we should just Rotate the Emitter about its center
					else
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(fEmitterRotateDelta, 0.0f, 0.0f));
					}
				}

				if (KeyboardManager.KeyIsDown(Keys.I) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickUp))
				{
					// If we should Rotate the Emitter around the Pivot Point
					if (KeyboardManager.KeyIsDown(Keys.Y))
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, -fEmitterRotateDelta, 0.0f));
					}
					// Else we should just Rotate the Emitter about its center
					else
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, -fEmitterRotateDelta, 0.0f));
					}
				}

				if (KeyboardManager.KeyIsDown(Keys.K) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickDown))
				{
					// If we should Rotate the Emitter around the Pivot Point
					if (KeyboardManager.KeyIsDown(Keys.Y))
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, fEmitterRotateDelta, 0.0f));
					}
					// Else we should just Rotate the Emitter about its center
					else
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, fEmitterRotateDelta, 0.0f));
					}
				}

				if (KeyboardManager.KeyIsDown(Keys.U) ||
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickLeft) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
				{
					// If we should Rotate the Emitter around the Pivot Point
					if (KeyboardManager.KeyIsDown(Keys.Y))
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, fEmitterRotateDelta));
					}
					// Else we should just Rotate the Emitter about its center
					else
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, fEmitterRotateDelta));
					}
				}

				if (KeyboardManager.KeyIsDown(Keys.O) ||
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickRight) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
				{
					// If we should Rotate the Emitter around the Pivot Point
					if (KeyboardManager.KeyIsDown(Keys.Y))
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, -fEmitterRotateDelta));
					}
					// Else we should just Rotate the Emitter about its center
					else
					{
						_currentDPSFDemoParticleSystemWrapper.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, -fEmitterRotateDelta));
					}
				}
			}

			// Check if the Emitter should be reset
			if (KeyboardManager.KeyWasJustPressed(Keys.Z))
			{
				_currentDPSFDemoParticleSystemWrapper.Emitter.PositionData.Position = Vector3.Zero;
				_currentDPSFDemoParticleSystemWrapper.Emitter.OrientationData.Orientation = Quaternion.Identity;
			}

			// If the Texture should be changed
			if (KeyboardManager.KeyWasJustPressed(Keys.T) || GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.Y))
			{
				if (_currentDPSFDemoParticleSystemWrapper.Texture != null)
				{
					// Get which Texture is currently being used for sure
					for (int i = 0; i < (int)Textures.LastInList + 1; i++)
					{
						Textures eTexture = (Textures)i;
						string sName = eTexture.ToString();

						if (_currentDPSFDemoParticleSystemWrapper.Texture.Name.Equals(sName))
						{
							_currentTexture = (Textures)i;
						}
					}

					// If we should go to the previous Texture
					if (KeyboardManager.KeyIsDown(Keys.LeftShift) || KeyboardManager.KeyIsDown(Keys.RightShift))
					{
						_currentTexture--;
						if (_currentTexture < 0)
						{
							_currentTexture = Textures.LastInList;
						}
					}
					// Else we should go to the next Texture
					else
					{
						_currentTexture++;
						if (_currentTexture > Textures.LastInList)
						{
							_currentTexture = 0;
						}
					}

					// Change the Texture being used to draw the Particles
					_currentDPSFDemoParticleSystemWrapper.SetTexture("Textures/" + _currentTexture.ToString());
				}
			}

			if (KeyboardManager.KeyWasJustPressed(Keys.Insert))
			{
				// Add a single Particle
				_currentDPSFDemoParticleSystemWrapper.AddParticle();
			}

			if (KeyboardManager.KeyIsDown(Keys.Home))
			{
				// Add Particles while the button is pressed
				_currentDPSFDemoParticleSystemWrapper.AddParticle();
			}

			if (KeyboardManager.KeyIsDown(Keys.PageUp))
			{
				// Add the max number of Particles
				while (_currentDPSFDemoParticleSystemWrapper.AddParticle()) { }
			}

			if (KeyboardManager.KeyWasJustPressed(Keys.Delete) || GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.X))
			{
				// Toggle emitting particles on/off
				_currentDPSFDemoParticleSystemWrapper.Emitter.EmitParticlesAutomatically = !_currentDPSFDemoParticleSystemWrapper.Emitter.EmitParticlesAutomatically;
			}

			if (KeyboardManager.KeyIsDown(Keys.Add, 0.02f) || KeyboardManager.KeyIsDown(Keys.OemPlus, 0.02f) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightShoulder, 0.02f))
			{
				// Increase the number of Particles being emitted
				_currentDPSFDemoParticleSystemWrapper.Emitter.ParticlesPerSecond++;
			}

			if (KeyboardManager.KeyIsDown(Keys.Subtract, 0.02f) || KeyboardManager.KeyIsDown(Keys.OemMinus, 0.02f) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftShoulder, 0.02f))
			{
				if (_currentDPSFDemoParticleSystemWrapper.Emitter.ParticlesPerSecond > 1)
				{
					// Decrease the number of Particles being emitted
					_currentDPSFDemoParticleSystemWrapper.Emitter.ParticlesPerSecond--;
				}
			}

			if (KeyboardManager.KeyWasJustPressed(Keys.Multiply) || 
				((KeyboardManager.KeyIsDown(Keys.LeftShift) || KeyboardManager.KeyIsDown(Keys.RightShift)) && KeyboardManager.KeyWasJustPressed(Keys.D8)) || 
				GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.B))
			{
				// Increase the Speed of the Particle System simulation
				_particleSystemManager.SimulationSpeed += 0.1f;

				if (_particleSystemManager.SimulationSpeed > 5.0f)
				{
					_particleSystemManager.SimulationSpeed = 5.0f;
				}

				// If DPSF is not inheriting from DrawableGameComponent then we need
				// to set the individual particle system's Simulation Speeds
				if (DPSFHelper.DPSFInheritsDrawableGameComponent)
				{
					_particleSystemManager.SetSimulationSpeedForAllParticleSystems(_particleSystemManager.SimulationSpeed);
				}
			}

			if (KeyboardManager.KeyWasJustPressed(Keys.Divide) ||
				(KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyWasJustPressed(Keys.OemQuestion)) || 
				GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.A))
			{
				// Decrease the Speed of the Particle System simulation
				_particleSystemManager.SimulationSpeed -= 0.1f;

				if (_particleSystemManager.SimulationSpeed < 0.1f)
				{
					_particleSystemManager.SimulationSpeed = 0.1f;
				}

				// If DPSF is not inheriting from DrawableGameComponent then we need
				// to set the individual particle system's Simulation Speeds
				if (DPSFHelper.DPSFInheritsDrawableGameComponent)
				{
					_particleSystemManager.SetSimulationSpeedForAllParticleSystems(_particleSystemManager.SimulationSpeed);
				}
			}

            // Perform particle system-specific input processing.
		    _currentDPSFDemoParticleSystemWrapper.ProcessInput();

			// Perform any particle system-specific input processing that affects objects external to the particle system.
			switch (_currentParticleSystem)
			{
				case ParticleSystemEffects.Smoke:
					if (KeyboardManager.KeyWasJustPressed(Keys.V))
					{
						SmokeParticleSystem smokeParticleSystem = _currentDPSFDemoParticleSystemWrapper as SmokeParticleSystem;

						// Setup the sphere to pass by the particle system.
						_sphereObject.bVisible = true;
						_sphereObject.sPosition = smokeParticleSystem.mcExternalObjectPosition = new Vector3(-125, 50, 0);
						_sphereObject.sVelocity = new Vector3(50, 0, 0);
						_sphereObject.cTimeAliveInSeconds = TimeSpan.Zero;

						// Setup the particle system to be affected by the sphere.
						smokeParticleSystem.mfAttractRepelRange = _sphereObject.fSize * 2;
						smokeParticleSystem.mfAttractRepelForce = 3.0f;
						smokeParticleSystem.MakeParticlesAttractToExternalObject();
					}

					if (KeyboardManager.KeyWasJustPressed(Keys.B))
					{
						SmokeParticleSystem smokeParticleSystem = _currentDPSFDemoParticleSystemWrapper as SmokeParticleSystem;

						// Setup the sphere to pass by the particle system.
						_sphereObject.bVisible = true;
						_sphereObject.sPosition = smokeParticleSystem.mcExternalObjectPosition = new Vector3(-125, 50, 0);
						_sphereObject.sVelocity = new Vector3(50, 0, 0);
						_sphereObject.cTimeAliveInSeconds = TimeSpan.Zero;

						// Setup the particle system to be affected by the sphere.
						smokeParticleSystem.mfAttractRepelRange = _sphereObject.fSize * 2f;
						smokeParticleSystem.mfAttractRepelForce = 0.5f;
						smokeParticleSystem.MakeParticlesRepelFromExternalObject();
					}
					break;
			}
		}

		#endregion
	}
}
