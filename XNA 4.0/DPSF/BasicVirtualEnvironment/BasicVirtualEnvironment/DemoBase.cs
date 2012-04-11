#region File Description
//===================================================================
// GameMain.cs
//
// This class provides a basic virtual environment as an easy starting 
// point to inherit from and build off of.
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using BasicVirtualEnvironment.Diagnostics;
using BasicVirtualEnvironment.Input;
using BasicVirtualEnvironment.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace BasicVirtualEnvironment
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
		/// Tells if the Common Controls should be shown or not.
		/// </summary>
		protected bool ShowCommonControls { get; set; }

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
		/// <param name="e">The <see cref="FPS.FPSEventArgs"/> instance containing the event data.</param>
		private void FPS_FPSUpdated(object sender, FPS.FPSEventArgs e)
		{
			// Get how much Garbage is waiting to be collected
			long currentGarbageAmount = GC.GetTotalMemory(false);

			// If the Garbage Collector did not run in the past second, calculate the average amount of garbage created per frame in the past second.
			if (currentGarbageAmount > _garbageAmountAtLastFPSUpdate)
			{
				float garbageCreatedInLastSecondInKB = (currentGarbageAmount - _garbageAmountAtLastFPSUpdate) / 1024f;
				_garbageAverageCreatedPerFrameInKB = garbageCreatedInLastSecondInKB / e.FPS;
				_garbageAverageCreatedPerUpdateInKB = garbageCreatedInLastSecondInKB / _updatesPerSecond;
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
		/// By default the base class has F1 toggle showing all text, F2 does environment text, and F3 camera controls, so this property returns "F3" by default.
		/// If you want to have F4, F5, etc. toggle other text, override this to have it return that last sequential F-key that toggles displaying text. 
		/// </summary>
		protected virtual string LastToggleTextFunctionKey { get { return "F3"; } }

		/// <summary>
		/// Tells if the Floor should be shown or not.
		/// </summary>
		protected bool ShowFloor { get; set; }
		private Model _floorModel { get; set; }	// Model of the Floor.

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

		/// <summary>
		/// The Camera used to view the virtual world from.
		/// </summary>
		protected Camera Camera { get; set; }

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
			ClearScreenEveryFrame = true;
			WorldMatrix = Matrix.Identity;
			ViewMatrix = Matrix.Identity;
			ProjectionMatrix = Matrix.Identity;

			// Initialize the Camera and use a Fixed camera by default.
			Camera = new Camera(true);
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

			// Specify vertices indicating positive axis directions.
			int iLineLength = 50;
			msaAxisDirectionVertices = new VertexPositionColor[6];
			msaAxisDirectionVertices[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);
			msaAxisDirectionVertices[1] = new VertexPositionColor(new Vector3(iLineLength, 1, 0), Color.Red);
			msaAxisDirectionVertices[2] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Green);
			msaAxisDirectionVertices[3] = new VertexPositionColor(new Vector3(0, iLineLength, 0), Color.Green);
			msaAxisDirectionVertices[4] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Blue);
			msaAxisDirectionVertices[5] = new VertexPositionColor(new Vector3(0, 1, iLineLength), Color.Blue);

			// Setup the effect used to draw the positive axis directions.
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
		/// Perform the base classes game logic.
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		protected override void Update(GameTime gameTime)
		{
			// Get and process user Input before anything else.
			ProcessInput(gameTime);

			// Allow the camera to be moved around, even if the particle systems are paused.
			// Update the World, View, and Projection matrices.
			UpdateWorldViewProjectionMatrices();

			// Perform any other game updates.
			UpdateGame(gameTime);

			// If the game is not paused, update any other Drawable Game Components.
			if (!Paused)
				base.Update(gameTime);

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
		/// Perform any game logic.
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		protected virtual void UpdateGame(GameTime gameTime) { }

		/// <summary>
		/// Updates the World, View, and Projection matrices according to the Camera's current position.
		/// </summary>
		private void UpdateWorldViewProjectionMatrices()
		{
			// Compute the Aspect Ratio of the window
			float fAspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;

			// Setup the View matrix depending on which Camera mode is being used.
			// If we are using the Fixed Camera.
			if (Camera.bUsingFixedCamera)
			{
				// Set up the View matrix according to the Camera's arc, rotation, and distance from the Offset position.
				ViewMatrix = Matrix.CreateTranslation(Camera.sFixedCameraLookAtPosition) *
									 Matrix.CreateRotationY(MathHelper.ToRadians(Camera.fCameraRotation)) *
									 Matrix.CreateRotationX(MathHelper.ToRadians(Camera.fCameraArc)) *
									 Matrix.CreateLookAt(new Vector3(0, 0, -Camera.fCameraDistance),
														 new Vector3(0, 0, 0), Vector3.Up);
			}
			// Else we are using the Free Camera
			else
			{
				// Set up our View matrix specifying the Camera position, a point to look-at, and a direction for which way is up.
				ViewMatrix = Matrix.CreateLookAt(Camera.sVRP, Camera.sVRP + Camera.cVPN, Camera.cVUP);
			}

			// Setup the Projection matrix by specifying the field of view (1/4 pi), aspect ratio, and the near and far clipping planes
			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, fAspectRatio, 1, 10000);
		}

		/// <summary>
		/// Draw the base class's components.
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		protected override void Draw(GameTime gameTime)
		{
			// Do any pre-Draw work and exit without drawing anything if told.
			if (!BeforeDrawGame(gameTime))
				return;

			// Clear the scene.
			GraphicsDevice.Clear(BACKGROUND_COLOR);

			// If the screen should NOT be cleared each frame, draw to a render target instead of right to the screen.
			if (!ClearScreenEveryFrame)
			{
				SetupToDrawToRenderTarget(_clearScreenEveryFrameJustToggled);
				_clearScreenEveryFrameJustToggled = false;
			}

			// Draw the Floor at the origin (0,0,0) and any other models.
			DrawModels(WorldMatrix, ViewMatrix, ProjectionMatrix);

			// If the Axis' should be drawn
			if (ShowPositiveDirectionAxis)
			{
				// Draw lines at the origin (0,0,0) indicating positive axis directions.
				DrawAxis(WorldMatrix, ViewMatrix, ProjectionMatrix);
			}

			// Draw any other Drawable Game Components that may need to be drawn.
			// Call this before drawing other things, so that any 2D sprites show up on top of the any other 2D Sprites already drawn.
			base.Draw(gameTime);


			// Draw everything else.
			DrawGame(gameTime);


			// Update the Frames Per Second to be displayed.
			FPS.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

			// Draw the Text to the screen last, so it is always on top.
			if (ShowText)
				DrawText();

			// If we were drawing this frame to the Render Target, draw the Render Target to the screen.
			if (!ClearScreenEveryFrame)
			{
				DrawRenderTargetToScreen();
			}

			// Do any post-draw work.
			AfterDrawGame(gameTime);
		}

		/// <summary>
		/// Called before anything is drawn to the screen.
		/// <para>NOTE: If this returns false the Draw() function will exit immediately without drawing anything.</para>
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		protected virtual bool BeforeDrawGame(GameTime gameTime) { return true; }

		/// <summary>
		/// Called after everything has been drawn to the screen.
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		protected virtual void AfterDrawGame(GameTime gameTime) { }

		/// <summary>
		/// Draw any game components to the screen.
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		protected virtual void DrawGame(GameTime gameTime) { }

		/// <summary>
		/// Sets up the app to draw to the render target instead of directly to the screen.
		/// </summary>
		/// <param name="clearRenderTarget">If true the Render Target will be cleared before anything else is drawn to it.</param>
		protected void SetupToDrawToRenderTarget(bool clearRenderTarget)
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
		protected void DrawRenderTargetToScreen()
		{
			GraphicsDevice.SetRenderTarget(null);		// Start drawing to the screen again instead of to the Render Target.
			GraphicsDevice.Clear(BACKGROUND_COLOR);
			SpriteBatch.Begin();
			// Draw the Render Target contents to the screen
			SpriteBatch.Draw(_renderTarget, new Rectangle(0, 0, _renderTarget.Width, _renderTarget.Height), Color.White);
			SpriteBatch.End();
		}

		/// <summary>
		/// Draw Text to the screen.
		/// </summary>
		void DrawText()
		{
			// Get area on screen that it is safe to draw text to (so that we are sure it will be displayed on the screen).
			Rectangle textSafeArea = GetTextSafeArea();

			// Convert values to strings
			string sFPSValue = FPS.CurrentFPS.ToString();
			string sAvgFPSValue = FPS.AverageFPS.ToString("0.0");
			string sCameraModeValue = Camera.bUsingFixedCamera ? "Fixed" : "Free";
			string sCameraPosition = "(" + Camera.Position.X.ToString("0") + "," + Camera.Position.Y.ToString("0") + "," + Camera.Position.Z.ToString("0") + ")";

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

			SpriteBatch.DrawString(Font, "Avg:", new Vector2(textSafeArea.Left + 5, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sAvgFPSValue, new Vector2(textSafeArea.Left + 50, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);
			
			//mcSpriteBatch.DrawString(mcFont, "Position:", new Vector2(textSafeArea.Left + 275, textSafeArea.Bottom - 75), sPropertyColor);
			SpriteBatch.DrawString(Font, sCameraPosition, new Vector2(textSafeArea.Left + 280, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Camera:", new Vector2(textSafeArea.Left + 660, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sCameraModeValue, new Vector2(textSafeArea.Left + 740, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Show/Hide Controls:", new Vector2(textSafeArea.Right - 260, textSafeArea.Top + 2), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, "F1 - " + LastToggleTextFunctionKey, new Vector2(textSafeArea.Right - 70, textSafeArea.Top + 2), CONTROL_TEXT_COLOR);

			// If the Common Controls should be shown, display them.
			if (ShowCommonControls)
			{
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

				SpriteBatch.DrawString(Font, "Pause Simulation:", new Vector2(485, 150), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "Spacebar", new Vector2(645, 150), CONTROL_TEXT_COLOR);

				SpriteBatch.DrawString(Font, "Clear Screen Each Frame:", new Vector2(485, 225), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "F5", new Vector2(730, 225), CONTROL_TEXT_COLOR);

				SpriteBatch.DrawString(Font, "Draw Performance Info:", new Vector2(485, 300), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "F10", new Vector2(705, 300), CONTROL_TEXT_COLOR);
			}

			// If the Camera Controls should be shown.
			if (ShowCameraControls)
			{
				// If we are using a Fixed Camera.
				if (Camera.bUsingFixedCamera)
				{
					SpriteBatch.DrawString(Font, "Fixed Camera Controls:", new Vector2(5, GraphicsDeviceManager.PreferredBackBufferHeight - 125), PROPERTY_TEXT_COlOR);
					SpriteBatch.DrawString(Font, "Keys: Left/Right Arrows, Up/Down Arrows, Num0/Num1", new Vector2(15, GraphicsDeviceManager.PreferredBackBufferHeight - 100), CONTROL_TEXT_COLOR);
					SpriteBatch.DrawString(Font, "Mouse: Left Button + X/Y Movement, Right Button + Y Movement", new Vector2(15, GraphicsDeviceManager.PreferredBackBufferHeight - 75), CONTROL_TEXT_COLOR);
				}
				// Else we are using a Free Camera.
				else
				{
					SpriteBatch.DrawString(Font, "Free Camera Controls", new Vector2(5, GraphicsDeviceManager.PreferredBackBufferHeight - 125), PROPERTY_TEXT_COlOR);
					SpriteBatch.DrawString(Font, "Keys: Left/Right Arrows, Up/Down Arrows, Num0/Num1, Num4/Num6, Num8/Num2", new Vector2(15, GraphicsDeviceManager.PreferredBackBufferHeight - 100), CONTROL_TEXT_COLOR);
					SpriteBatch.DrawString(Font, "Mouse: Left Button + X/Y Movement, Right Button + X/Y Movement, Scroll Wheel", new Vector2(15, GraphicsDeviceManager.PreferredBackBufferHeight - 75), CONTROL_TEXT_COLOR);
				}
			}

			// If performance related text should be drawn, draw the number of bytes allocated in memory.
			if (ShowPerformanceText)
			{
				SpriteBatch.DrawString(Font, "Garbage Allocated (KB): " + _garbageCurrentAmountInKB.ToString("0.0"), new Vector2(480, GraphicsDeviceManager.PreferredBackBufferHeight - 200), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "Avg Garbage Per Update (KB): " + _garbageAverageCreatedPerUpdateInKB.ToString("0.000"), new Vector2(440, GraphicsDeviceManager.PreferredBackBufferHeight - 175), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "Avg Garbage Per Frame (KB): " + _garbageAverageCreatedPerFrameInKB.ToString("0.000"), new Vector2(445, GraphicsDeviceManager.PreferredBackBufferHeight - 150), PROPERTY_TEXT_COlOR);
			}

			// Draw any text from inheriting classes.
			DrawGameText();

			// Stop drawing text
			SpriteBatch.End();
		}

		/// <summary>
		/// Draws any game text to the screen.
		/// <para>NOTE: SpriteBatch.Begin() has already been called, so all you need to do is call SpriteBatch.DrawString().</para>
		/// </summary>
		protected virtual void DrawGameText() { }

		/// <summary>
		/// Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).
		/// </summary>
		/// <returns>Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).</returns>
		protected Rectangle GetTextSafeArea()
		{
			return GetTextSafeArea(0.9f);
		}

		/// <summary>
		/// Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).
		/// </summary>
		/// <param name="fNormalizedPercent">The amount of screen space (normalized between 0.0 - 1.0) that should 
		/// safe to draw to (e.g. 0.8 to have a 10% border on all sides)</param>
		/// <returns>Returns the Area of the Screen that it is safe to draw Text to (as this differs on PC and TVs).</returns>
		protected Rectangle GetTextSafeArea(float fNormalizedPercent)
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
		/// Draw any models to the screen.
		/// </summary>
		/// <param name="worldMatrix">The world matrix.</param>
		/// <param name="viewMatrix">The view matrix.</param>
		/// <param name="projectionMatrix">The projection matrix.</param>
		protected virtual void DrawModels(Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
		{
			// Set our sampler state to allow the ground to have a repeated texture.
			GraphicsDevice.BlendState = BlendState.Opaque;
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

			// If the Floor should be drawn.
			if (ShowFloor)
			{
				_floorModel.Draw(worldMatrix, viewMatrix, projectionMatrix);
			}
		}

		/// <summary>
		/// Helper for drawing lines showing the positive axis directions.
		/// </summary>
		private void DrawAxis(Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
		{
			mcAxisEffect.World = worldMatrix;
			mcAxisEffect.View = viewMatrix;
			mcAxisEffect.Projection = projectionMatrix;

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
		/// Gets and processes all user input.
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		private void ProcessInput(GameTime gameTime)
		{
			// Save how long it's been since the last time Input was Handled.
			float timeInSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

			// Save the state of the input devices at this frame.
			KeyboardManager.UpdateKeyboardStateForThisFrame(gameTime.ElapsedGameTime);
			MouseManager.UpdateMouseStateForThisFrame(gameTime.ElapsedGameTime);
			GamePadsManager.UpdateGamePadStatesForThisFrame(gameTime.ElapsedGameTime);

			// If we should Exit.
			if (KeyboardManager.KeyIsDown(Keys.Escape) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.Back))
			{
				Exit();
			}

			// Handle any game-related input.
			if (!ProcessInputForGame(gameTime))
				return;

			// If we should toggle showing the Floor.
			if (KeyboardManager.KeyWasJustPressed(Keys.F))
			{
				ShowFloor = !ShowFloor;
			}

			// If we should toggle Pausing the game.
			if (KeyboardManager.KeyWasJustPressed(Keys.Space) || GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.Start))
			{
				Paused = !Paused;
			}

			// If we should toggle between Full Screen and Windowed mode.
			if (KeyboardManager.KeyWasJustPressed(Keys.End))
			{
				GraphicsDeviceManager.ToggleFullScreen();
			}

			// If we should toggle showing the Common Controls.
			if (KeyboardManager.KeyWasJustPressed(Keys.F1))
			{
				ShowText = !ShowText;
			}

			// If we should toggle showing the Common Controls.
			if (KeyboardManager.KeyWasJustPressed(Keys.F2))
			{
				ShowCommonControls = !ShowCommonControls;
			}

			// If we should toggle showing the Camera Controls.
			if (KeyboardManager.KeyWasJustPressed(Keys.F3))
			{
				ShowCameraControls = !ShowCameraControls;
			}

			// If we should toggle Clearing the Screen each Frame.
			if (KeyboardManager.KeyWasJustPressed(Keys.F5))
			{
				ClearScreenEveryFrame = !ClearScreenEveryFrame;
				_clearScreenEveryFrameJustToggled = true;
			}

			// If the Axis should be toggled on/off.
			if (KeyboardManager.KeyWasJustPressed(Keys.F7))
			{
				ShowPositiveDirectionAxis = !ShowPositiveDirectionAxis;
			}

			// If the Performance Profiling was toggled.
			if (KeyboardManager.KeyWasJustPressed(Keys.F10))
			{
				// Toggle if the Performance Profiling text should be drawn.
				ShowPerformanceText = !ShowPerformanceText;
			}

			// Handle input for moving the Camera.
			ProcessInputForCamera(timeInSeconds);
		}

		/// <summary>
		/// Process any input for game events.
		/// <para>This is called after the new input device states have been obtained, but before any other input is processed.</para>
		/// <para>NOTE: If this returns false any other input will not be processed.</para>
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		protected virtual bool ProcessInputForGame(GameTime gameTime) { return true; }

		/// <summary>
		/// Handle input for moving the Camera
		/// </summary>
		private void ProcessInputForCamera(float fTimeInSeconds)
		{
			// If we are using the Fixed Camera
			if (Camera.bUsingFixedCamera)
			{
				// If the Camera should be rotated vertically
				if (KeyboardManager.KeyIsDown(Keys.NumPad1) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D1)) ||
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickUp) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftStick)))
				{
					Camera.fCameraArc -= fTimeInSeconds * 25;
				}

				if (KeyboardManager.KeyIsDown(Keys.NumPad0) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D0)) ||
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickDown) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftStick)))
				{
					Camera.fCameraArc += fTimeInSeconds * 25;
				}

				// If the Camera should rotate horizontally
				if (KeyboardManager.KeyIsDown(Keys.Right) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickRight))
				{
					Camera.fCameraRotation -= fTimeInSeconds * 50;
				}

				if (KeyboardManager.KeyIsDown(Keys.Left) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickLeft))
				{
					Camera.fCameraRotation += fTimeInSeconds * 50;
				}

				// If Camera should be zoomed out
				if (KeyboardManager.KeyIsDown(Keys.Down) ||
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickDown) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftStick)))
				{
					Camera.fCameraDistance += fTimeInSeconds * 250;
				}

				// If Camera should be zoomed in
				if (KeyboardManager.KeyIsDown(Keys.Up) ||
					(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftThumbstickUp) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftStick)))
				{
					Camera.fCameraDistance -= fTimeInSeconds * 250;
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
						Camera.fCameraRotation -= (iXMovement * fMOUSE_ROTATION_SPEED);
					}

					// If the Camera should rotate vertically
					if (iYMovement != 0)
					{
						Camera.fCameraArc -= (-iYMovement * fMOUSE_ROTATION_SPEED);
					}
				}

				// If the Right Mouse Button is pressed
				if (MouseManager.CurrentMouseState.RightButton == ButtonState.Pressed)
				{
					// If the Camera should zoom in/out
					if (iYMovement != 0)
					{
						Camera.fCameraDistance += iYMovement * fMOUSE_MOVEMENT_SPEED;
					}
				}


				// Limit the Arc movement
				if (Camera.fCameraArc > 90.0f)
				{
					Camera.fCameraArc = 90.0f;
				}
				else if (Camera.fCameraArc < -90.0f)
				{
					Camera.fCameraArc = -90.0f;
				}

				// Limit the Camera zoom distance
				if (Camera.fCameraDistance > 2000)
				{
					Camera.fCameraDistance = 2000;
				}
				else if (Camera.fCameraDistance < 1)
				{
					Camera.fCameraDistance = 1;
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
					Camera.MoveCameraForwardOrBackward(iSPEED * fTimeInSeconds);
				}

				// If the Camera should move backwards
				if (KeyboardManager.KeyIsDown(Keys.Down))
				{
					Camera.MoveCameraForwardOrBackward(-iSPEED * fTimeInSeconds);
				}

				// If the Camera should strafe right
				if (KeyboardManager.KeyIsDown(Keys.Right))
				{
					Camera.MoveCameraHorizontally(-iSPEED * fTimeInSeconds);
				}

				// If the Camera should strafe left
				if (KeyboardManager.KeyIsDown(Keys.Left))
				{
					Camera.MoveCameraHorizontally(iSPEED * fTimeInSeconds);
				}

				// If the Camera should move upwards
				if (KeyboardManager.KeyIsDown(Keys.NumPad1) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D1)))
				{
					Camera.MoveCameraVertically((iSPEED / 2) * fTimeInSeconds);
				}

				// If the Camera should move downwards
				if (KeyboardManager.KeyIsDown(Keys.NumPad0) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D0)))
				{
					Camera.MoveCameraVertically((-iSPEED / 2) * fTimeInSeconds);
				}

				// If the Camera should yaw left
				if (KeyboardManager.KeyIsDown(Keys.NumPad4) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D4)))
				{
					Camera.RotateCameraHorizontally(fROTATE_SPEED * fTimeInSeconds);
				}

				// If the Camera should yaw right
				if (KeyboardManager.KeyIsDown(Keys.NumPad6) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D6)))
				{
					Camera.RotateCameraHorizontally(-fROTATE_SPEED * fTimeInSeconds);
				}

				// If the Camera should pitch up
				if (KeyboardManager.KeyIsDown(Keys.NumPad8) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D8)))
				{
					Camera.RotateCameraVertically(-fROTATE_SPEED * fTimeInSeconds);
				}

				// If the Camera should pitch down
				if (KeyboardManager.KeyIsDown(Keys.NumPad2) || (KeyboardManager.KeyIsUp(Keys.LeftShift) && KeyboardManager.KeyIsUp(Keys.RightShift) && KeyboardManager.KeyIsDown(Keys.D2)))
				{
					Camera.RotateCameraVertically(fROTATE_SPEED * fTimeInSeconds);
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
						Camera.RotateCameraHorizontally(-iXMovement * fMOUSE_ROTATION_SPEED);
					}

					// If the Camera should pitch
					if (iYMovement != 0)
					{
						Camera.RotateCameraVertically(iYMovement * fMOUSE_ROTATION_SPEED);
					}
				}

				// If the Right Mouse Button is pressed
				if (MouseManager.CurrentMouseState.RightButton == ButtonState.Pressed)
				{
					// If the Camera should strafe
					if (iXMovement != 0)
					{
						Camera.MoveCameraHorizontally(-iXMovement * fMOUSE_MOVEMENT_SPEED);
					}

					// If the Camera should move forward or backward
					if (iYMovement != 0)
					{
						Camera.MoveCameraForwardOrBackward(-iYMovement * (fMOUSE_MOVEMENT_SPEED * 2.0f));
					}
				}

				// If the Middle Mouse Button is scrolled
				if (iZMovement != 0)
				{
					Camera.MoveCameraVertically(iZMovement * (fMOUSE_MOVEMENT_SPEED / 10.0f));
				}
			}

			// If the Camera Mode should be switched
			if (KeyboardManager.KeyWasJustPressed(Keys.PageDown))
			{
				Camera.bUsingFixedCamera = !Camera.bUsingFixedCamera;
			}

			// If the Camera values should be Reset
			if (KeyboardManager.KeyIsDown(Keys.R) ||
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftShoulder) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightShoulder)))
			{
				Camera.ResetFreeCameraVariables();
				Camera.ResetFixedCameraVariables();
			}
		}

		#endregion
	}
}
