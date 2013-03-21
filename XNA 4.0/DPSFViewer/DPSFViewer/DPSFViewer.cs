#region Using Statements
using System;
using BasicVirtualEnvironment;
using BasicVirtualEnvironment.Input;
using DPSF;
using DPSFViewer.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace DPSFViewer
{
	/// <summary>
	/// 
	/// </summary>
	public class DPSFViewer : BasicVirtualEnvironmentBase
	{
		#region Fields

		//===========================================================
		// Global Constants - User Settings
		//===========================================================

		// How often the Particle Systems should be updated (zero = update as often as possible)
		const int PARTICLE_SYSTEM_UPDATES_PER_SECOND = 0;

		//===========================================================
		// Class Structures and Variables
		//===========================================================

		// List of all the textures
		enum Textures
		{
			AnimatedButterfly = 0,
			AnimatedExplosion,
			Arrow,
			Arrow2,
			Arrow3,
			Arrow4,
			Arrow5,
			Ball,
			Bubble,
			Cloud,
			CloudLight,
			CrossArrow,
			Dog,
			Donut,
			DPSFLogo,
			DPSFText,
			Fire,
			Flower,
			Flower2,
			Flower3,
			Flower4,
			Flower5,
			Gear,
			Gear2,
			Gear3,
			Gear4,
			Gear5,
			MoveArrow,
			Paper,
			Particle,
			RedCircle,
			Shape1,
			Shape2,
			Smoke,
			Spark,
			Splat,
			Star,
			Star2,
			Star3,
			Star4,
			Star5,
			Star6,
			Star7,
			Star8,
			Star9,
			StarFish,
			Sun,
			Sun2,
			Sun3,
			ThrowingStar,
			Wheel,
			WhiteSquare,
			WhiteSquare1,
			WhiteSquare2,
			WhiteSquare10,
			WordBubble,
			X,
			LastInList = 56     // This value should be the same as the Texture with the largest value
		}

		// Initialize the Texture to use.
		Textures _currentTexture = Textures.Bubble;

		/// <summary>
		/// Called whenever the value of ShowPerformanceText is changed.
		/// </summary>
		/// <param name="enabled">The new value of ShowPerformanceText.</param>
		protected override void ShowPerformanceTextToggled(bool enabled)
		{
			// Only enable the Performance Profiling if we are going to be displaying it.

			// Set it on the Defaults so it is enabled/disabled whenever we initialize a new particle system.
			DPSFDefaultSettings.PerformanceProfilingIsEnabled = enabled;

			// Set it on the Manager to enable/disable it for the particle system that is currently running.
			_particleSystemManager.SetPerformanceProfilingIsEnabledForAllParticleSystems(enabled);
		}

		// Declare the Particle System Manager to manage the Particle Systems.
		ParticleSystemManager _particleSystemManager = new ParticleSystemManager();

		// Declare a Particle System pointer to point to the Current Particle System being used
		IDPSFParticleSystem _currentParticleSystem;


		//===========================================================
		// Viewer specific variables.
		//===========================================================

		DPSFViewerForm _viewerForm = null;		// Handle to the WinForm that we are drawing to
		public bool MouseOverViewport { get; set; }	// Tells if the mouse is currently over the Viewport on the WinForm or not

		#endregion

		#region Initialization

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFViewer"/> class.
		/// </summary>
		public DPSFViewer()
		{
			GraphicsDeviceManager.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(GraphicsDeviceManager_PreparingDeviceSettings);
			System.Windows.Forms.Control.FromHandle((this.Window.Handle)).VisibleChanged += new EventHandler(DPSFViewer_VisibleChanged);

			_viewerForm = new DPSFViewerForm(this);
			_viewerForm.Show();

			// We want to show the mouse.
			this.IsMouseVisible = true;
		}

		void DPSFViewer_VisibleChanged(object sender, EventArgs e)
		{
			if (System.Windows.Forms.Control.FromHandle((this.Window.Handle)).Visible == true)
				System.Windows.Forms.Control.FromHandle((this.Window.Handle)).Visible = false;
		}

		void GraphicsDeviceManager_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = _viewerForm.Viewport.Handle;
			e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = _viewerForm.Viewport.Width;
			e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = _viewerForm.Viewport.Height;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			// Set how often the Particle Systems should be Updated
			_particleSystemManager.UpdatesPerSecond = PARTICLE_SYSTEM_UPDATES_PER_SECOND;

			_currentParticleSystem = new RandomParticleSystem(this);

			// Initialize the Current Particle System
			SetParticleSystem(_currentParticleSystem);
		}

		/// <summary>
		/// Sets the particle system to use in the Viewer.
		/// </summary>
		/// <param name="particleSystem">The particle system to use.</param>
		public void SetParticleSystem(IDPSFParticleSystem particleSystem)
		{
			// If the Current Particle System has been set
			if (_currentParticleSystem != null)
			{
				// Destroy the Current Particle System.
				// This frees up any resources/memory held by the Particle System, so it's
				// good to destroy them if we know they won't be used for a while
				_currentParticleSystem.Destroy();
			}

			_currentParticleSystem = particleSystem;

			// Initialize the Particle System
			_currentParticleSystem.AutoInitialize(this.GraphicsDevice, this.Content, null);

			_particleSystemManager.RemoveAllParticleSystems();
			_particleSystemManager.AddParticleSystem(_currentParticleSystem);
		}

		#endregion

		#region Update and Draw

		/// <summary>
		/// Perform any game logic.
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		protected override void UpdateGame(GameTime gameTime)
		{
			base.UpdateGame(gameTime);

			// Update the Quad Particle Systems to know where the Camera is so that they can display
			// the particles as billboards if needed (i.e. have particle always face the camera).
			_particleSystemManager.SetCameraPositionForAllParticleSystems(Camera.Position);

			// Set the World, View, and Projection Matrices for the Particle Systems.
			_particleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(WorldMatrix, ViewMatrix, ProjectionMatrix);

			// If the Game is Paused.
			if (Paused)
			{
				// Update the particle systems with 0 elapsed time, just to allow the particles to rotate to face the camera.
				_particleSystemManager.UpdateAllParticleSystems(0);

				// Exit without updating anything.
				return;
			}

			// If the Current Particle System is Initialized.
			if (_currentParticleSystem != null && _currentParticleSystem.IsInitialized)
			{
				// Update all Particle Systems manually
				_particleSystemManager.UpdateAllParticleSystems((float)gameTime.ElapsedGameTime.TotalSeconds);
			}
		}

		/// <summary>
		/// Draw any game components to the screen.
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		protected override void DrawGame(GameTime gameTime)
		{
			base.DrawGame(gameTime);

			// Draw the Particle Systems manually.
			_particleSystemManager.DrawAllParticleSystems();
		}

		/// <summary>
		/// Draws any game text to the screen.
		/// <para>NOTE: SpriteBatch.Begin() has already been called, so all you need to do is call SpriteBatch.DrawString().</para>
		/// </summary>
		protected override void DrawGameText()
		{
			base.DrawGameText();

			// Get area on screen that it is safe to draw text to (so that we are sure it will be displayed on the screen).
			Rectangle textSafeArea = GetTextSafeArea();

			// Get how many Particles are currently Active.
			int iNumberOfActiveParticles = _currentParticleSystem.TotalNumberOfActiveParticles;
			int iNumberOfParticlesAllocatedInMemory = _currentParticleSystem.TotalNumberOfParticlesAllocatedInMemory;

			// Convert values to strings.
			string sTotalParticleCountValue = iNumberOfActiveParticles.ToString();
			string sEmitterOnValue = (_currentParticleSystem.Emitter.EmitParticlesAutomatically ? "On" : "Off");
			string sParticlesPerSecondValue = _currentParticleSystem.Emitter.ParticlesPerSecond.ToString("0.00");
			string sParticleSystemSpeedScale = _particleSystemManager.SimulationSpeed.ToString("0.0");
			string sAllocatedParticles = iNumberOfParticlesAllocatedInMemory.ToString();
			string sTexture = "N/A";
			if (_currentParticleSystem.Texture != null)
			{
				sTexture = _currentParticleSystem.Texture.Name.TrimStart("Textures/".ToCharArray());
			}


			SpriteBatch.DrawString(Font, "Allocated:", new Vector2(textSafeArea.Left + 120, textSafeArea.Bottom - 50), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sAllocatedParticles, new Vector2(textSafeArea.Left + 210, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Texture:", new Vector2(textSafeArea.Left + 440, textSafeArea.Bottom - 50), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sTexture, new Vector2(textSafeArea.Left + 520, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Speed:", new Vector2(textSafeArea.Right - 100, textSafeArea.Bottom - 50), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sParticleSystemSpeedScale, new Vector2(textSafeArea.Right - 35, textSafeArea.Bottom - 50), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Particles:", new Vector2(textSafeArea.Left + 120, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sTotalParticleCountValue, new Vector2(textSafeArea.Left + 205, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Emitter:", new Vector2(textSafeArea.Left + 275, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sEmitterOnValue, new Vector2(textSafeArea.Left + 345, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			SpriteBatch.DrawString(Font, "Particles Per Second:", new Vector2(textSafeArea.Left + 390, textSafeArea.Bottom - 25), PROPERTY_TEXT_COlOR);
			SpriteBatch.DrawString(Font, sParticlesPerSecondValue, new Vector2(textSafeArea.Left + 585, textSafeArea.Bottom - 25), VALUE_TEXT_COLOR);

			// If performance related text should be drawn, display how long updates and draws of the current particle system take.
			if (ShowPerformanceText)
			{
				SpriteBatch.DrawString(Font, "Update Time (ms): " + _particleSystemManager.TotalPerformanceTimeToDoUpdatesInMilliseconds.ToString("0.000"), new Vector2(529, GraphicsDeviceManager.PreferredBackBufferHeight - 250), PROPERTY_TEXT_COlOR);
				SpriteBatch.DrawString(Font, "Draw Time (ms): " + _particleSystemManager.TotalPerformanceTimeToDoDrawsInMilliseconds.ToString("0.000"), new Vector2(545, GraphicsDeviceManager.PreferredBackBufferHeight - 225), PROPERTY_TEXT_COlOR);
			}
		}

		#endregion

		#region Handle Input

		/// <summary>
		/// Process any input for game events.
		/// <para>This is called after the new input device states have been obtained, but before any other input is processed.</para>
		/// <para>NOTE: If this returns false any other input will not be processed.</para>
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		protected override bool ProcessInputForGame(GameTime gameTime)
		{
			// If the Mouse is not over the Viewport, we don't want to process input, so just return.
			if (!MouseOverViewport)
				return false;

			base.ProcessInputForGame(gameTime);

			// TODO - REMOVE THIS. This is just here for testing and should be removed eventually.
			if (KeyboardManager.KeyWasJustPressed(Keys.V))
				_currentParticleSystem.Visible = !_currentParticleSystem.Visible;

			// Handle input for controlling the Particle Systems.
			return ProcessInputForParticleSystem(gameTime);
		}

		/// <summary>
		/// Function to control the Particle Systems based on user input.
		/// Returns false if no other input should be processed this frame.
		/// </summary>
		/// <param name="gameTime">How much time has elapsed in the game and between updates.</param>
		private bool ProcessInputForParticleSystem(GameTime gameTime)
		{
			// Save how long it's been since the last time Input was Handled.
			float elapsedTimeInSecondsSinceLastUpdate = (float)gameTime.ElapsedGameTime.TotalSeconds;

			// If the Current Particle System is not Initialized
			if (_currentParticleSystem == null || !_currentParticleSystem.IsInitialized)
			{
				return true;
			}

			// Define how fast the user can move and rotate the Emitter
			float fEmitterMoveDelta = 75 * elapsedTimeInSecondsSinceLastUpdate;
			float fEmitterRotateDelta = MathHelper.Pi * elapsedTimeInSecondsSinceLastUpdate;

			// If the Shift key is down, move faster
			if (KeyboardManager.KeyIsDown(Keys.LeftShift) || KeyboardManager.KeyIsDown(Keys.RightShift))
			{
				fEmitterMoveDelta *= 2;
			}

			// Check if the Emitter should be moved
			if (KeyboardManager.KeyIsDown(Keys.W) ||
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadUp) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				_currentParticleSystem.Emitter.PositionData.Position += Vector3.Up * fEmitterMoveDelta;
			}

			if (KeyboardManager.KeyIsDown(Keys.S) ||
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadDown) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				_currentParticleSystem.Emitter.PositionData.Position += Vector3.Down * fEmitterMoveDelta;
			}

			if (KeyboardManager.KeyIsDown(Keys.A) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadLeft))
			{
				_currentParticleSystem.Emitter.PositionData.Position += Vector3.Left * fEmitterMoveDelta;
			}

			if (KeyboardManager.KeyIsDown(Keys.D) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadRight))
			{
				_currentParticleSystem.Emitter.PositionData.Position += Vector3.Right * fEmitterMoveDelta;
			}

			if (KeyboardManager.KeyIsDown(Keys.E) ||
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadUp) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				_currentParticleSystem.Emitter.PositionData.Position += Vector3.Forward * fEmitterMoveDelta;
			}

			if (KeyboardManager.KeyIsDown(Keys.Q) ||
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.DPadDown) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				_currentParticleSystem.Emitter.PositionData.Position += Vector3.Backward * fEmitterMoveDelta;
			}

			// Check if the Emitter should be rotated
			if (KeyboardManager.KeyIsDown(Keys.J) ||
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickLeft) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				// If we should Rotate the Emitter around the Pivot Point
				if (KeyboardManager.KeyIsDown(Keys.Y))
				{
					_currentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(-fEmitterRotateDelta, 0.0f, 0.0f));
				}
				// Else we should just Rotate the Emitter about its center
				else
				{
					_currentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(-fEmitterRotateDelta, 0.0f, 0.0f));
				}
			}

			if (KeyboardManager.KeyIsDown(Keys.L) ||
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickRight) && !GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				// If we should Rotate the Emitter around the Pivot Point
				if (KeyboardManager.KeyIsDown(Keys.Y))
				{
					_currentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(fEmitterRotateDelta, 0.0f, 0.0f));
				}
				// Else we should just Rotate the Emitter about its center
				else
				{
					_currentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(fEmitterRotateDelta, 0.0f, 0.0f));
				}
			}

			if (KeyboardManager.KeyIsDown(Keys.I) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickUp))
			{
				// If we should Rotate the Emitter around the Pivot Point
				if (KeyboardManager.KeyIsDown(Keys.Y))
				{
					_currentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, -fEmitterRotateDelta, 0.0f));
				}
				// Else we should just Rotate the Emitter about its center
				else
				{
					_currentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, -fEmitterRotateDelta, 0.0f));
				}
			}

			if (KeyboardManager.KeyIsDown(Keys.K) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickDown))
			{
				// If we should Rotate the Emitter around the Pivot Point
				if (KeyboardManager.KeyIsDown(Keys.Y))
				{
					_currentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, fEmitterRotateDelta, 0.0f));
				}
				// Else we should just Rotate the Emitter about its center
				else
				{
					_currentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, fEmitterRotateDelta, 0.0f));
				}
			}

			if (KeyboardManager.KeyIsDown(Keys.U) ||
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickLeft) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				// If we should Rotate the Emitter around the Pivot Point
				if (KeyboardManager.KeyIsDown(Keys.Y))
				{
					_currentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, fEmitterRotateDelta));
				}
				// Else we should just Rotate the Emitter about its center
				else
				{
					_currentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, fEmitterRotateDelta));
				}
			}

			if (KeyboardManager.KeyIsDown(Keys.O) ||
				(GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightThumbstickRight) && GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightStick)))
			{
				// If we should Rotate the Emitter around the Pivot Point
				if (KeyboardManager.KeyIsDown(Keys.Y))
				{
					_currentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, -fEmitterRotateDelta));
				}
				// Else we should just Rotate the Emitter about its center
				else
				{
					_currentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, -fEmitterRotateDelta));
				}
			}

			// Check if the Emitter should be reset
			if (KeyboardManager.KeyWasJustPressed(Keys.Z))
			{
				_currentParticleSystem.Emitter.PositionData.Position = Vector3.Zero;
				_currentParticleSystem.Emitter.OrientationData.Orientation = Quaternion.Identity;
			}

			// If the Texture should be changed
			if (KeyboardManager.KeyWasJustPressed(Keys.T) || GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.Y))
			{
				if (_currentParticleSystem.Texture != null)
				{
					// Get which Texture is currently being used for sure
					for (int i = 0; i < (int)Textures.LastInList + 1; i++)
					{
						Textures eTexture = (Textures)i;
						string sName = eTexture.ToString();

						if (_currentParticleSystem.Texture.Name.Equals(sName))
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
					_currentParticleSystem.SetTexture("Textures/" + _currentTexture.ToString());
				}
			}

			if (KeyboardManager.KeyWasJustPressed(Keys.Insert))
			{
				// Add a single Particle
				_currentParticleSystem.AddParticle();
			}

			if (KeyboardManager.KeyIsDown(Keys.Home))
			{
				// Add Particles while the button is pressed
				_currentParticleSystem.AddParticle();
			}

			if (KeyboardManager.KeyIsDown(Keys.PageUp))
			{
				// Add the max number of Particles
				while (_currentParticleSystem.AddParticle()) { }
			}

			if (KeyboardManager.KeyWasJustPressed(Keys.Delete) || GamePadsManager.ButtonWasJustPressed(PlayerIndex.One, Buttons.X))
			{
				// Toggle emitting particles on/off
				_currentParticleSystem.Emitter.EmitParticlesAutomatically = !_currentParticleSystem.Emitter.EmitParticlesAutomatically;
			}

			if (KeyboardManager.KeyIsDown(Keys.Add, 0.02f) || KeyboardManager.KeyIsDown(Keys.OemPlus, 0.02f) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.RightShoulder, 0.02f))
			{
				// Increase the number of Particles being emitted
				_currentParticleSystem.Emitter.ParticlesPerSecond++;
			}

			if (KeyboardManager.KeyIsDown(Keys.Subtract, 0.02f) || KeyboardManager.KeyIsDown(Keys.OemMinus, 0.02f) || GamePadsManager.ButtonIsDown(PlayerIndex.One, Buttons.LeftShoulder, 0.02f))
			{
				if (_currentParticleSystem.Emitter.ParticlesPerSecond > 1)
				{
					// Decrease the number of Particles being emitted
					_currentParticleSystem.Emitter.ParticlesPerSecond--;
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

			return true;
		}

		#endregion
	}
}
