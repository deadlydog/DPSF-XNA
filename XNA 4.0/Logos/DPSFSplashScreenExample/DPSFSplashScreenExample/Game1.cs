//===================================================================
// This file shows how to use the DPSFSplashScreenParticleSystem class,
// contained in the DPSFSplashScreenPS.cs file, to to generate the DPSF 
// Splash Screen to be displayed during the opening sequence of your game.
//===================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Specify that we want to include the DPSF and DPSF.ParticleSystems namespaces
using DPSF;
using DPSF.ParticleSystems;

namespace DPSFSplashScreenExample
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        // Declare our Splash Screen Particle System variable
        DPSFSplashScreenParticleSystem _dpsfSplashScreenParticleSystem = null;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Set the Title of the Window
            Window.Title = "Example of how to display the DPSF Logo Splash Screen";
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
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Declare the new Particle System instance and Auto Initialize it
            _dpsfSplashScreenParticleSystem = new DPSFSplashScreenParticleSystem(this);
			_dpsfSplashScreenParticleSystem.AutoInitialize(this.GraphicsDevice, this.Content, null);

			// Hookup the function that defines what to do when the Splash Screen finishes
			_dpsfSplashScreenParticleSystem.SplashScreenComplete += new EventHandler(_splashScreenParticleSystem_SplashScreenComplete);
        }

		/// <summary>
		/// Handles the SplashScreenComplete event of the _splashScreenParticleSystem control.
		/// This gets called when the Splash Screen is done playing, so we can then load the regular code to start the game.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void _splashScreenParticleSystem_SplashScreenComplete(object sender, EventArgs e)
		{
			// Destroy the Splash Screen Particle System now that it is done playing, to free the resources it's using.
			_dpsfSplashScreenParticleSystem.SplashScreenComplete -= new EventHandler(_splashScreenParticleSystem_SplashScreenComplete);
			_dpsfSplashScreenParticleSystem.Destroy();
			_dpsfSplashScreenParticleSystem = null;

			// Exit the game.
			// This would typically be where you would switch to the next screen to continue loading your game.
			this.Exit();
		}

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
		protected override void UnloadContent()
		{ }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

			// If we are still displaying the DPSF Splash Screen, update it.
			if (_dpsfSplashScreenParticleSystem != null)
			{
				UpdateDPSFSplashScreen(gameTime);
			}

			// Else update the game as usual here....

            base.Update(gameTime);
        }

		/// <summary>
		/// Updates the DPSF splash screen.
		/// </summary>
		/// <param name="gameTime">The game time.</param>
		private void UpdateDPSFSplashScreen(GameTime gameTime)
		{
			if (_dpsfSplashScreenParticleSystem == null)
				return;

			// Update the Splash Screen
			_dpsfSplashScreenParticleSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

			// If the Splash Screen is still playing and the user has pressed a button (keyboard, mouse, or gamepad) to skip the Splash Screen.
			if (_dpsfSplashScreenParticleSystem != null &&
				((Keyboard.GetState().GetPressedKeys().Length > 0 && Keyboard.GetState().GetPressedKeys()[0] != Keys.None) ||
				(Mouse.GetState().LeftButton == ButtonState.Pressed || Mouse.GetState().RightButton == ButtonState.Pressed) ||
				(GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A | Buttons.B | Buttons.X | Buttons.Y | Buttons.Start))))
			{
				// Mark that the Splash Screen should be skipped
				_dpsfSplashScreenParticleSystem.IsSplashScreenComplete = true;
			}
		}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
			// If we are still displaying the DPSF Splash Screen, draw it and exit.
			if (_dpsfSplashScreenParticleSystem != null)
			{
				DrawDPSFSplashScreen(gameTime);
				return;
			}

			// Else draw the game as usual here....

            base.Draw(gameTime);
        }

		/// <summary>
		/// Draws the DPSF splash screen.
		/// </summary>
		/// <param name="gameTime">The game time.</param>
		private void DrawDPSFSplashScreen(GameTime gameTime)
		{
			if (_dpsfSplashScreenParticleSystem == null)
				return;

			// Clear the screen with the proper Background Color
			GraphicsDevice.Clear(_dpsfSplashScreenParticleSystem.BackgroundColor);

			// The World, View, and Projection matrices are set automatically when the particle system is Auto Initialized,
			// so there is no need to set them here.

			// Draw the Splash Screen
			_dpsfSplashScreenParticleSystem.Draw();
		}
    }
}
