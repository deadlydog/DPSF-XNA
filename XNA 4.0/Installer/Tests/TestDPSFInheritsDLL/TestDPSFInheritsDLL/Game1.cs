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

namespace TestDPSFInheritsDLL
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager mcGraphics;
		SpriteBatch mcSpriteBatch;
		KeyboardState mcKeyboard;
		SpriteFont mcFont;                      // Font used to draw text

		DefaultTexturedQuadParticleSystemTemplate mcParticleSystem = null;

		public Game1()
		{
			mcGraphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			// Make the game run as fast as possible (i.e. don't limit the FPS)
			this.IsFixedTimeStep = false;
			mcGraphics.SynchronizeWithVerticalRetrace = false;

			// Set the resolution
			mcGraphics.PreferredBackBufferWidth = 800;
			mcGraphics.PreferredBackBufferHeight = 600;
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
			mcParticleSystem = new DefaultTexturedQuadParticleSystemTemplate(null);
			mcParticleSystem.AutoInitialize(this.GraphicsDevice, this.Content, null);

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

			// TODO: use this.Content to load your game content here
			mcFont = Content.Load<SpriteFont>("Fonts/font");
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
		protected override void Update(GameTime gameTime)
		{
			mcKeyboard = Keyboard.GetState();

			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
				mcKeyboard.IsKeyDown(Keys.Escape))
				this.Exit();


			// Setup the Camera
			Vector3 sCameraPosition = new Vector3(0, 50, 300);
			Vector3 sCameraTarget = new Vector3(0, 50, 0);

			// Compute the Aspect Ratio of the window
			float fAspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;

			// Set up our View matrix specifying the Camera position, a point to look-at, and a direction for which way is up
			Matrix cViewMatrix = Matrix.CreateLookAt(sCameraPosition, sCameraTarget, Vector3.Up);

			// Setup the Projection matrix by specifying the field of view (1/4 pi), aspect ratio, and the near and far clipping planes
			Matrix cProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, fAspectRatio, 1, 10000);

			// If there is a particle system to update
			if (mcParticleSystem != null)
			{
				// TODO: Add your update logic here
				mcParticleSystem.CameraPosition = sCameraPosition;
				mcParticleSystem.SetWorldViewProjectionMatrices(Matrix.Identity, cViewMatrix, cProjectionMatrix);
				mcParticleSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
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

			// If there is a particle system to draw
			if (mcParticleSystem != null)
			{
				// TODO: Add your drawing code here
				mcParticleSystem.Draw();
			}

			FPS.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
			mcSpriteBatch.Begin();
			mcSpriteBatch.DrawString(mcFont, "FPS: " + FPS.CurrentFPS.ToString(), new Vector2(10, 10), Color.Black);
			mcSpriteBatch.DrawString(mcFont, "Avg: " + FPS.AverageFPS.ToString("0"), new Vector2(10, 40), Color.Black);
			mcSpriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
