using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DPSF_Demo.ParticleSystems;
using DPSF;

namespace DPSF_Demo_for_WinRT
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        ParticleSystemManager particleSystemManager = null;

        DefaultQuadParticleSystemTemplate quadParticleSystem = null;
		DefaultTexturedQuadParticleSystemTemplate texturedQuadParticleSystem = null;
        DefaultSpriteParticleSystemTemplate spriteParticleSystem = null;
        DefaultSprite3DBillboardParticleSystemTemplate sprite3DBillboardParticleSystem = null;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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

            quadParticleSystem = new DefaultQuadParticleSystemTemplate(null);
            texturedQuadParticleSystem = new DefaultTexturedQuadParticleSystemTemplate(null);
            spriteParticleSystem = new DefaultSpriteParticleSystemTemplate(null);
            sprite3DBillboardParticleSystem = new DefaultSprite3DBillboardParticleSystemTemplate(null);

            particleSystemManager = new ParticleSystemManager();
            //particleSystemManager.AddParticleSystem(quadParticleSystem);
            //particleSystemManager.AddParticleSystem(texturedQuadParticleSystem);
            particleSystemManager.AddParticleSystem(spriteParticleSystem);
            //particleSystemManager.AddParticleSystem(sprite3DBillboardParticleSystem);
            particleSystemManager.AutoInitializeAllParticleSystems(this.GraphicsDevice, this.Content, null);

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

            // TODO: use this.Content to load your game content here
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
            // TODO: Add your update logic here

			// Setup the Camera
			Vector3 sCameraPosition = new Vector3(0, 50, 300);
			Vector3 sCameraTarget = new Vector3(0, 50, 0);

			// Compute the Aspect Ratio of the window
			float fAspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;

			// Set up our View matrix specifying the Camera position, a point to look-at, and a direction for which way is up
			Matrix cViewMatrix = Matrix.CreateLookAt(sCameraPosition, sCameraTarget, Vector3.Up);

			// Setup the Projection matrix by specifying the field of view (1/4 pi), aspect ratio, and the near and far clipping planes
			Matrix cProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, fAspectRatio, 1, 10000);

			// TODO: Add your update logic here
            particleSystemManager.SetCameraPositionForAllParticleSystems(sCameraPosition);
			particleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(Matrix.Identity, cViewMatrix, cProjectionMatrix);
			particleSystemManager.UpdateAllParticleSystems((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
            particleSystemManager.DrawAllParticleSystems();	

            base.Draw(gameTime);
        }
    }
}
