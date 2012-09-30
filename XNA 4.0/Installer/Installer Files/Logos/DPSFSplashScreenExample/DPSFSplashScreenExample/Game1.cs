//===================================================================
// This file shows how to use the DPSFSplashScreenParticleSystem class,
// contained in the DPSFSplashScreenPS.cs file, to to generate the DPSF 
// Splash Screen to be displayed during the opening sequence of your game.
//
// There are 2 methods that can be used to check when the Splash Screen is done playing,
// both of which are shown in this file and commented with "METHOD 1" and "METHOD 2".
// METHOD 1: Subscribe to the SplashScreenComplete event to let us know when the splash screen is done playing.
// METHOD 2: Manually check every frame if the splash screen is done playing or not.
//===================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Specify that we want to include the DPSF and DPSF.SplashScreen namespaces.
using DPSF;
using DPSF.SplashScreen;

namespace DPSFSplashScreenExample
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Declare our Splash Screen Particle System variable
        DPSFSplashScreenParticleSystem _splashScreenParticleSystem = null;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Declare the new Particle System instance and Auto Initialize it.
            _splashScreenParticleSystem = new DPSFSplashScreenParticleSystem(this);
            _splashScreenParticleSystem.AutoInitialize(this.GraphicsDevice, this.Content, null);

            // Normally you would leave this set to true as a convenience for yourself, but I want to make sure 
            // you see the splash screen when you run this splash screen example solution.
            _splashScreenParticleSystem.SkipSplashScreenWhenDebugging = false;

            //=====================================================================================
            // METHOD 1 (part 1 of 2):
            // Subscribe to the SplashScreenComplete event that fires when the splash screen is done.
            // NOTE: The line below is commented out because this file uses Method 2 by default. To use Method 1
            //       uncomment the line below and comment out the Method 2 code.
            //_splashScreenParticleSystem.SplashScreenComplete += new EventHandler(_splashScreenParticleSystem_SplashScreenComplete);
            //=====================================================================================
        }

        //=====================================================================================
        /// <summary>
        /// METHOD 1 (part 2 of 2):
        /// Define in this function what should happen when the Splash Screen completes.
        /// Typically you would switch to the next screen to continue loading your game.
        /// </summary>
        void _splashScreenParticleSystem_SplashScreenComplete(object sender, EventArgs e)
        {
            // Unhook the event handler that we attached to avoid a memory leak.
            _splashScreenParticleSystem.SplashScreenComplete -= new EventHandler(_splashScreenParticleSystem_SplashScreenComplete);

            // Destroy the Splash Screen Particle System to free the resources it's using.
            _splashScreenParticleSystem.Destroy();

            // Exit the game.
            // This would typically be where you would switch to the next screen to continue loading your game.
            this.Exit();
        }
        //=====================================================================================

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

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

            // Update the Particle System
            _splashScreenParticleSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);


            // If the user has pressed a button to skip the Splash Screen
            if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Space) || Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                // Mark that the Splash Screen should be skipped
                _splashScreenParticleSystem.IsSplashScreenComplete = true;
            }

            //=====================================================================================
            // METHOD 2:
            // Every update check if the Splash Screen is done playing yet or not.
            // If the Splash Screen is Done Playing
            if (_splashScreenParticleSystem.IsSplashScreenComplete)
            {
                // Destroy the Splash Screen Particle System to free the resources it's using.
                _splashScreenParticleSystem.Destroy();

                // Exit the game.
                // This would typically be where you would switch to the next screen to continue loading your game.
                this.Exit();
            }
            //=====================================================================================

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen with the proper Background Color
            GraphicsDevice.Clear(_splashScreenParticleSystem.BackgroundColor);

            // Draw the Splash Screen.
            // The World, View, and Projection matrices are set automatically when
            // the particle system is Auto Initialized.
            _splashScreenParticleSystem.Draw();

            base.Draw(gameTime);
        }
    }
}
