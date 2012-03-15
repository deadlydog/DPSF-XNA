using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using DPSF;
using DPSF.ParticleSystems;

namespace DPSF_Demo_Phone
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;
		SpriteFont _font;                      // Font used to draw text

		DPSFSplashScreenParticleSystem _dpsfSplashScreenParticleSystem = null;
		SpriteParticleSystem _spriteParticleSystem = null;
		ParticleSystemManager _particleSystemManager = null;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			// Frame rate is 30 fps by default for Windows Phone.
			TargetElapsedTime = TimeSpan.FromTicks(333333);
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// Define the type of Gestures to accept
			TouchPanel.EnabledGestures = GestureType.DoubleTap | GestureType.Pinch;

			// Create the Particle Systems
			_dpsfSplashScreenParticleSystem = new DPSFSplashScreenParticleSystem(this);
			_spriteParticleSystem = new SpriteParticleSystem(this);
			_particleSystemManager = new ParticleSystemManager();
			_particleSystemManager.UpdatesPerSecond = 30;

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// Load the font to use for text
			_font = Content.Load<SpriteFont>("Fonts/font");

			// Setup the Splash Screen to be shown first
			_particleSystemManager.AddParticleSystem(_dpsfSplashScreenParticleSystem);
			_particleSystemManager.AutoInitializeAllParticleSystems(this.GraphicsDevice, this.Content, _spriteBatch);
			_dpsfSplashScreenParticleSystem.SplashScreenComplete += new EventHandler(_dpsfSplashScreenParticleSystem_SplashScreenComplete);
		}

		/// <summary>
		/// Handles the SplashScreenComplete event of the _dpsfSplashScreenParticleSystem control.
		/// This gets called when the Splash Screen is done playing, so we can then load the regular code to start the demo.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void _dpsfSplashScreenParticleSystem_SplashScreenComplete(object sender, EventArgs e)
		{
			// Now that the Splash Screen is done displaying, clean it up.
			_dpsfSplashScreenParticleSystem.SplashScreenComplete -= new EventHandler(_dpsfSplashScreenParticleSystem_SplashScreenComplete);
			_particleSystemManager.RemoveParticleSystem(_dpsfSplashScreenParticleSystem);
			_dpsfSplashScreenParticleSystem.Destroy();
			_dpsfSplashScreenParticleSystem = null;

			// Start displaying the demo's particle systems
			_particleSystemManager.AddParticleSystem(_spriteParticleSystem);
			_particleSystemManager.AutoInitializeAllParticleSystems(this.GraphicsDevice, this.Content, _spriteBatch);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// Destroy all of the Particle Systems
			_particleSystemManager.DestroyAndRemoveAllParticleSystems();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			// Process any user input
			ProcessInput(gameTime);

			// Update the Particle Systems
			_particleSystemManager.UpdateAllParticleSystems((float)gameTime.ElapsedGameTime.TotalSeconds);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			base.Draw(gameTime);

			// We are using this Sprite Batch to draw our Sprite particle systems, so call Begin() before drawing the particle systems.
			_spriteBatch.Begin();

			// Draw the Particle Systems
			_particleSystemManager.DrawAllParticleSystems();

			// We are done drawing the particle systems, so End() the sprite batch.
			_spriteBatch.End();

			// Draw the Text to the screen last, so it is always on top
			DrawText();
		}

		/// <summary>
		/// Draws any text.
		/// </summary>
		private void DrawText()
		{
			// Specify the text Colors to use
			Color sPropertyColor = Color.WhiteSmoke;
			Color sValueColor = Color.Yellow;
			Color sControlColor = Color.PowderBlue;

			// Begin drawing text
			_spriteBatch.Begin();

			_spriteBatch.DrawString(_font, "Force:", new Vector2(GetTextSafeArea().Left + 200, GetTextSafeArea().Top + 2), sPropertyColor);
			_spriteBatch.DrawString(_font, _spriteParticleSystem.AttractorMode.ToString(), new Vector2(GetTextSafeArea().Left + 260, GetTextSafeArea().Top + 2), sValueColor);

			_spriteBatch.DrawString(_font, "Strength:", new Vector2(GetTextSafeArea().Left + 410, GetTextSafeArea().Top + 2), sPropertyColor);
			_spriteBatch.DrawString(_font, _spriteParticleSystem.AttractorStrength.ToString("0.0"), new Vector2(GetTextSafeArea().Left + 495, GetTextSafeArea().Top + 2), sValueColor);

			// Finished drawing text
			_spriteBatch.End();
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
		/// Processes any input from the user.
		/// </summary>
		/// <param name="gameTime">The game time.</param>
		private void ProcessInput(GameTime gameTime)
		{
			TouchCollection touchLocations = TouchPanel.GetState();

			// If there is no input to process, just exit
			if (!TouchPanel.IsGestureAvailable && touchLocations.Count <= 0)
				return;

			// If we are currently showing the Splash Screen, skip the Splash Screen since input was given.
			if (_dpsfSplashScreenParticleSystem != null)
			{
				_dpsfSplashScreenParticleSystem.IsSplashScreenComplete = true;
				return;
			}

			// Save how long it's been since the last time Input was Handled
			float timeInSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

			if (TouchPanel.IsGestureAvailable)
			{
				// Read in the Gesture input
				GestureSample gesture = TouchPanel.ReadGesture();

				// Process Gestures
				if (gesture.GestureType == GestureType.Tap)
				{
					_spriteParticleSystem.AttractorPosition = new Vector3(gesture.Position, 0);
				}
				else if (gesture.GestureType == GestureType.DoubleTap)
				{
					_spriteParticleSystem.ToggleAttractorMode();
				}
				else if (gesture.GestureType == GestureType.Pinch)
				{
					if ((gesture.Delta.X < 0 && gesture.Delta2.X > 0) ||
						(gesture.Delta.Y < 0 && gesture.Delta2.Y > 0))
						_spriteParticleSystem.AttractorStrength += (Math.Abs(gesture.Delta.X) + Math.Abs(gesture.Delta2.X)) / 10f;
					else if ((gesture.Delta.X > 0 && gesture.Delta2.X < 0) ||
							 (gesture.Delta.Y > 0 && gesture.Delta2.Y < 0))
						_spriteParticleSystem.AttractorStrength -= (Math.Abs(gesture.Delta.X) + Math.Abs(gesture.Delta2.X)) / 10f;
				}
			}

			if (touchLocations.Count > 0)
			{
				// Process touch locations on the screen
				if (touchLocations.Count > 0)
				{
					foreach (TouchLocation touch in touchLocations.Where(t => t.State != TouchLocationState.Invalid))
						_spriteParticleSystem.AttractorPosition = new Vector3(touch.Position, 0);
				}
			}
		}
	}
}
