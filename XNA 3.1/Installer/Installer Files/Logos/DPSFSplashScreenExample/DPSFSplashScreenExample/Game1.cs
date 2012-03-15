//*******************************************************************
// This file shows how to use the DPSFSplashScreenParticleSystem class,
// contained in the DPSFSplashScreenPS.cs file, to to generate the DPSF 
// Splash Screen to be displayed during the opening sequence of your game.
//*******************************************************************

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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Declare our Splash Screen Particle System variable
        DPSFSplashScreenParticleSystem mcSplashScreenParticleSystem = null;

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

            // Declare the new Particle System instance and Initialize it
            mcSplashScreenParticleSystem = new DPSFSplashScreenParticleSystem(this);
            mcSplashScreenParticleSystem.AutoInitialize(this.GraphicsDevice, this.Content, null);
        }

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
            mcSplashScreenParticleSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);


            // If the user has pressed a button to skip the Splash Screen
            if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Space) || Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                // Mark that the Splash Screen should be skipped
                mcSplashScreenParticleSystem.SplashScreenIsDonePlaying = true;
            }

            // If the Splash Screen is Done Playing
            if (mcSplashScreenParticleSystem.SplashScreenIsDonePlaying)
            {
                // Destroy the Splash Screen Particle System to free the resources it's using
                mcSplashScreenParticleSystem.Destroy();

                // Exit the game.
                // This would typically be where you would switch to the next screen
                // to continue loading your game.
                this.Exit();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // This shows how to set up the Camera (View and Projection matrices)
            // with the proper settings to visualize the Splash Screen properly.

            // Clear the screen with the proper Background Color
            GraphicsDevice.Clear(mcSplashScreenParticleSystem.BackgroundColor);

            // Set up the Camera's View matrix
            Matrix sViewMatrix = Matrix.CreateLookAt(mcSplashScreenParticleSystem.CameraPosition, 
                        mcSplashScreenParticleSystem.CameraLookAtPosition, mcSplashScreenParticleSystem.CameraUpDirection);

            // Setup the Camera's Projection matrix
            Matrix sProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(mcSplashScreenParticleSystem.CameraFieldOfView, 
                        mcSplashScreenParticleSystem.CameraAspectRatio, 1, 10000);


            // Draw the Particle System using the proper Camera Settings
            mcSplashScreenParticleSystem.SetWorldViewProjectionMatrices(Matrix.Identity, sViewMatrix, sProjectionMatrix);
            mcSplashScreenParticleSystem.Draw();

            base.Draw(gameTime);
        }
    }
}
