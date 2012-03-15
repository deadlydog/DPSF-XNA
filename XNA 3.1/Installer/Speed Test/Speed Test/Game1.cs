//===================================================================
// DPSF Tutorial 1 Source Code - This tutorial shows how to define, 
// update, draw, and destroy a DPSF particle system.
//
// Tutorial files of interest:
//  Game1.cs
//===================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

using DPSF;
using DPSF.ParticleSystems;

namespace Speed_Test
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager mcGraphics;       // Handle to the Graphics object
        SpriteBatch mcSpriteBatch;              // Batch used to draw Sprites
        SpriteFont mcFont;                      // Font used to draw text

        // The Width and Height of the application's window (default is 800x600)
        int miWINDOW_WIDTH = 800;
        int miWINDOW_HEIGHT = 600;

        // Variable used to keep track of how long the test has been running
        float mfTimer = 0;
        bool mbTestStarted = false;

        // How many times all of the runs (i.e. one test) should run
        const int miNUMBER_OF_TIMES_TO_PERFORM_TEST = 5;
        int miTestNumber = 0;

        // How many Runs a Test should make before taking the average
        const int miNUMBER_OF_TIMES_TO_RUN = 30;
        int miRunNumber = 0;

        // How long one Run should take (in seconds)
        const float mfRUN_TIME = 0.5f;

        // How long we should wait before starting the Test
        const float mfWAIT_TIME = 2.0f;

        // Variable to keep track of performance
        long[] mlNumberOfDraws = new long[miNUMBER_OF_TIMES_TO_RUN + 1];

        // Variables to hold the Average and Standard Deviation
        double mdAverage = 0;
        double mdStandardDeviation = 0;

        // Declare our Particle System variable
        ParticleSystem mcParticleSystem = null;

        public Game1()
        {
            mcGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Set the Title of the Window
            Window.Title = "DPSF Speed Test";

            // Make the game run as fast as possible (i.e. don't limit the FPS)
            this.IsFixedTimeStep = false;
            mcGraphics.SynchronizeWithVerticalRetrace = false;

            // Set the resolution
            mcGraphics.PreferredBackBufferWidth = miWINDOW_WIDTH;
            mcGraphics.PreferredBackBufferHeight = miWINDOW_HEIGHT;
            mcGraphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
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

            // Declare a new Particle System instance and Initialize it
            mcParticleSystem = new ParticleSystem(this);
            mcParticleSystem.AutoInitialize(this.GraphicsDevice, this.Content, null);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Destroy the Particle System
            mcParticleSystem.Destroy();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Exit the test if Escape is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            // Update the Particle System
            mcParticleSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            // Update our Timer
            mfTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Wait 2 seconds before Starting the Test
            if (mfTimer >= mfWAIT_TIME && miRunNumber == 0)
            {
                mbTestStarted = true;
                mfTimer = 0.0f;
                mlNumberOfDraws[0] = 0;
            }

            // If the Test has Started
            if (mbTestStarted)
            {
                // If one second has passed
                if (mfTimer >= mfRUN_TIME)
                {
                    // Reset the Timer and increment the Run counter
                    mfTimer -= mfRUN_TIME;
                    miRunNumber++;

                    // If the Test is done, make sure the Run Number doesn't get too high
                    if (miRunNumber >= miNUMBER_OF_TIMES_TO_RUN)
                    {
                        mbTestStarted = false;
                    }
                }

                // If the Test just completed
                if (!mbTestStarted)
                {
                    // Calculate the Average
                    for (int iIndex = 0; iIndex < miNUMBER_OF_TIMES_TO_RUN; iIndex++)
                    {
                        mdAverage += mlNumberOfDraws[iIndex];
                    }
                    mdAverage /= miNUMBER_OF_TIMES_TO_RUN;

                    // Calculate the Standard Deviation
                    for (int iIndex = 0; iIndex < miNUMBER_OF_TIMES_TO_RUN; iIndex++)
                    {
                        mdStandardDeviation += Math.Pow((mlNumberOfDraws[iIndex] - mdAverage), 2);
                    }
                    mdStandardDeviation /= miNUMBER_OF_TIMES_TO_RUN;
                    mdStandardDeviation = Math.Sqrt(mdStandardDeviation);

                    // If Random Lifetimes are being used
                    string sLifetimeType = "";
                    if (mcParticleSystem.mbUseRandomLifetimes)
                    {
                        sLifetimeType = "Random Lifetimes";
                    }
                    // Else Static Lifetimes are being used
                    else
                    {
                        sLifetimeType = "Static Lifetimes";
                    }

                    // Write the Results to a file
                    TextWriter cTextWriter = new StreamWriter("../../../../Results.txt", true);

                    // Write to the file
                    cTextWriter.WriteLine("DPSF Version " + DPSFHelper.Version + ": " + mdAverage.ToString("0.00") + " +/- " + mdStandardDeviation.ToString("0.00") + " (over " + miNUMBER_OF_TIMES_TO_RUN + " runs) (" + sLifetimeType + ")");

                    // Increment the number of Tests ran
                    miTestNumber++;

                    // If we are done running the Tests
                    if (miTestNumber >= miNUMBER_OF_TIMES_TO_PERFORM_TEST)
                    {
                        // If the Tests need to be ran using Random Lifetimes still
                        if (!mcParticleSystem.mbUseRandomLifetimes)
                        {
                            // Set the Particle System to use Random Lifetimes
                            mcParticleSystem.mbUseRandomLifetimes = true;

                            // Remove all of the Particles with Static Lifetimes
                            mcParticleSystem.RemoveAllParticles();

                            // Reset the Test variable
                            miTestNumber = 0;
                        }
                        // Else the Tests are completely done
                        else
                        {
                            // Write a blank line showning that this test sequence is done
                            cTextWriter.WriteLine();

                            // Close the file
                            cTextWriter.Close();

                            // Exit the application
                            this.Exit();
                        }
                    }
                    // Else we are not done running the Tests

                    // Close the file
                    cTextWriter.Close();

                    // Reset the Test variables for the next test
                    mbTestStarted = false;
                    mfTimer = mfWAIT_TIME;
                    miRunNumber = 0;
                    for (int iIndex = 0; iIndex < miNUMBER_OF_TIMES_TO_RUN; iIndex++)
                    {
                        mlNumberOfDraws[iIndex] = 0;
                    }

                    // If we are running all Tests a second time (for Random Lifetimes)
                    if (miTestNumber == 0)
                    {
                        // Make sure we WAIT before running the second tests
                        mfTimer = 0.0f;
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Set up the Camera's View matrix
            Matrix sViewMatrix = Matrix.CreateLookAt(new Vector3(0, 50, -200), new Vector3(0, 50, 0), Vector3.Up);

            // Setup the Camera's Projection matrix by specifying the field of view (1/4 pi), aspect ratio, and the near and far clipping planes
            Matrix sProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height, 1, 10000);


            // Draw the Particle System
            mcParticleSystem.SetWorldViewProjectionMatrices(Matrix.Identity, sViewMatrix, sProjectionMatrix);
            mcParticleSystem.Draw();


            // Increment our performance counter
            mlNumberOfDraws[miRunNumber]++;

            // Draw the Text to the screen
            DrawText();

            base.Draw(gameTime);
        }

        public void DrawText()
        {
            // Specify the text Colors to use
            Color sTextColor = Color.WhiteSmoke;
            Color sValueColor = Color.Yellow;

            int iTotalTests = miNUMBER_OF_TIMES_TO_PERFORM_TEST * 2;
            int iCurrentTest = miTestNumber + 1;

            if (mcParticleSystem.mbUseRandomLifetimes)
            {
                iCurrentTest += miNUMBER_OF_TIMES_TO_PERFORM_TEST;
            }

            mcSpriteBatch.Begin();

            mcSpriteBatch.DrawString(mcFont, "This window MUST stay in focus for accurate results", new Vector2(5, miWINDOW_HEIGHT - 50), sTextColor);

            mcSpriteBatch.DrawString(mcFont, "Test " + iCurrentTest.ToString() + " of " + iTotalTests.ToString(), new Vector2(5, miWINDOW_HEIGHT - 25), sTextColor);
            mcSpriteBatch.DrawString(mcFont, "Run " + (miRunNumber + 1).ToString() + " of " + miNUMBER_OF_TIMES_TO_RUN.ToString(), new Vector2(200, miWINDOW_HEIGHT - 25), sTextColor);

            mcSpriteBatch.DrawString(mcFont, "Average FPS:", new Vector2(400, miWINDOW_HEIGHT - 25), sTextColor);
            mcSpriteBatch.DrawString(mcFont, mdAverage.ToString("0.00") + " +/- " + mdStandardDeviation.ToString("0.00"), new Vector2(535, miWINDOW_HEIGHT - 25), sValueColor);

            mcSpriteBatch.End();
        }
    }
}
