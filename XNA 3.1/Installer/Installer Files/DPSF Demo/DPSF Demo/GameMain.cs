#region File Description
//===================================================================
// GameMain.cs
//
// This is the application file used to test Dans Particle System Framework
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DPSF;
using DPSF.ParticleSystems;
#endregion

namespace Demo
{
    /// <summary>
    /// Application class showing how to use Dans Particle System Framework
    /// </summary>
    public class GameMain : Microsoft.Xna.Framework.Game
    {
        #region Fields

        //===========================================================
        // Global Constants - User Settings
        //===========================================================
        
        // If this is set to true, a MessageBox will display any unhandled exceptions that occur. This
        // is useful when not debugging (i.e. when running directly from the executable) as it will still 
        // tell you what type of exception occurred and what line of code produced it.
        // Leave this set to false while debugging to have Visual Studio automatically take you to the 
        // line that threw the exception.
        public const bool mbRELEASE_MODE = true;

        // To allow the game to run as fast as possible, set this to false
        const bool mbLIMIT_FPS = false;

		// Whether we should draw how much memory is set to be collected by the Garbage Collector or not
		bool mbDRAW_GARBAGE_COLLECTOR_MEMORY_ALLOCATED = true;

        // How often the Particle Systems should be updated (zero = update as often as possible)
        const int miPARTICLE_SYSTEM_UPDATES_PER_SECOND = 0;

        // The Width and Height of the application's window (default is 800x600)
        int miWINDOW_WIDTH = 800;
        int miWINDOW_HEIGHT = 600;

        // The background color to use
        Color msBACKGROUND_COLOR = Color.Black;

        // Static Particle Settings
        float mfStaticParticleTimeStep = 1.0f / 30.0f; // The Time Step between the drawing of each frame of the Static Particles (1 / # of fps, example, 1 / 30 = 30fps)
        float mfStaticParticleTotalTime = 3.0f; // The number of seconds that the Static Particle should be drawn over

        // "Draw To Files" Settings
        float mfDrawPSToFilesTimeStep = 1.0f / 10.0f;
        float mfDrawPSToFilesTotalTime = 2.0f;
#if (!XBOX)
        ImageFileFormat meDrawPSToFilesImageFormat = ImageFileFormat.Png;
#endif
        int miDrawPSToFilesImageWidth = 200;
        int miDrawPSToFilesImageHeight = 150;
        string msDrawPSToFilesDirectoryName = "AnimationFrames";
        bool mbCreateAnimatedGIF = true;
        bool mbCreateTileSetImage = false;

        string msSerializedPSFileName = "SerializedParticleSystem.dat"; // The name of the file to serialize the particle system to

        //===========================================================
        // Class Structures and Variables
        //===========================================================

        // Structure to hold the Camera's position and orientation
        struct SCamera
        {
            public Vector3 sVRP, cVPN, cVUP, cVLeft;                    // Free Camera Variables (View Reference Point, View Plane Normal, View Up, and View Left)
            public float fCameraArc, fCameraRotation, fCameraDistance;  // Fixed Camera Variables
            public Vector3 sFixedCameraLookAtPosition;                  // The Position that the Fixed Camera should rotate around
            public bool bUsingFixedCamera;  // Variable indicating which type of Camera to use

            /// <summary>
            /// Explicit constructor
            /// </summary>
            /// <param name="bUseFixedCamera">True to use the Fixed Camera, false to use the Free Camera</param>
            public SCamera(bool bUseFixedCamera)
            {
                // Initialize variables with dummy values so we can call the Reset functions
                sVRP = cVPN = cVUP = cVLeft = sFixedCameraLookAtPosition = Vector3.Zero;
                fCameraArc = fCameraRotation = fCameraDistance = 0.0f;

                // Use the specified Camera type
                bUsingFixedCamera = bUseFixedCamera;

                // Initialize the variables with their proper values
                ResetFreeCameraVariables();
                ResetFixedCameraVariables();
            }

            /// <summary>
            /// Get the current Position of the Camera
            /// </summary>
            public Vector3 Position
            {
                get
                {
                    // If we are using the Fixed Camera
                    if (bUsingFixedCamera)
                    {
                        // Calculate the View Matrix
                        Matrix cViewMatrix = Matrix.CreateTranslation(sFixedCameraLookAtPosition) *
                                     Matrix.CreateRotationY(MathHelper.ToRadians(fCameraRotation)) *
                                     Matrix.CreateRotationX(MathHelper.ToRadians(fCameraArc)) *
                                     Matrix.CreateLookAt(new Vector3(0, 0, -fCameraDistance),
                                                         new Vector3(0, 0, 0), Vector3.Up);

                        // Invert the View Matrix
                        cViewMatrix = Matrix.Invert(cViewMatrix);

                        // Pull and return the Camera Coordinates from the inverted View Matrix
                        return cViewMatrix.Translation;
                    }
                    // Else we are using the Free Camera
                    else
                    {
                        return sVRP;
                    }
                }
            }

            /// <summary>
            /// Reset the Fixed Camera Variables to their default values
            /// </summary>
            public void ResetFixedCameraVariables()
            {
                fCameraArc = 0.0f;
                fCameraRotation = 180.0f;
                fCameraDistance = 300.0f;
                sFixedCameraLookAtPosition = new Vector3(0, -50, 0);
            }

            /// <summary>
            /// Reset the Free Camera Variables to their default values
            /// </summary>
            public void ResetFreeCameraVariables()
            {
                sVRP = new Vector3(0.0f, 50.0f, 300.0f);
                cVPN = Vector3.Forward;
                cVUP = Vector3.Up;
                cVLeft = Vector3.Left;
            }

            /// <summary>
            /// Normalize the Camera Directions and maintain proper Right and Up directions
            /// </summary>
            public void NormalizeCameraAndCalculateProperUpAndRightDirections()
            {
                // Calculate the new Right and Up directions
                cVPN.Normalize();
                cVLeft = Vector3.Cross(cVUP, cVPN);
                cVLeft.Normalize();
                cVUP = Vector3.Cross(cVPN, cVLeft);
                cVUP.Normalize();
            }

            /// <summary>
            /// Move the Camera Forward or Backward
            /// </summary>
            /// <param name="fAmountToMove">The distance to Move</param>
            public void MoveCameraForwardOrBackward(float fAmountToMove)
            {
                cVPN.Normalize();
                sVRP += (cVPN * fAmountToMove);
            }

            /// <summary>
            /// Move the Camera Horizontally
            /// </summary>
            /// <param name="fAmountToMove">The distance to move horizontally</param>
            public void MoveCameraHorizontally(float fAmountToMove)
            {
                cVLeft.Normalize();
                sVRP += (cVLeft * fAmountToMove);
            }

            /// <summary>
            /// Move the Camera Vertically
            /// </summary>
            /// <param name="fAmountToMove">The distance to move Vertically</param>
            public void MoveCameraVertically(float fAmountToMove)
            {
                // Move the Camera along the global Y axis
                sVRP.Y += fAmountToMove;
            }

            /// <summary>
            /// Rotate the Camera Horizontally
            /// </summary>
            /// <param name="fAmountToRotateInRadians">The amount to Rotate in radians</param>
            public void RotateCameraHorizontally(float fAmountToRotateInRadians)
            {
                // Rotate the Camera about the global Y axis
                Matrix cRotationMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, fAmountToRotateInRadians);
                cVPN = Vector3.Transform(cVPN, cRotationMatrix);
                cVUP = Vector3.Transform(cVUP, cRotationMatrix);

                // Normalize all of the Camera directions since they have changed
                NormalizeCameraAndCalculateProperUpAndRightDirections();
            }

            /// <summary>
            /// Rotate the Camera Vertically
            /// </summary>
            /// <param name="fAmountToRotateInRadians">The amount to Rotate in radians</param>
            public void RotateCameraVertically(float fAmountToRotateInRadians)
            {
                // Rotate the Camera
                Matrix cRotationMatrix = Matrix.CreateFromAxisAngle(cVLeft, fAmountToRotateInRadians);
                cVPN = Vector3.Transform(cVPN, cRotationMatrix);
                cVUP = Vector3.Transform(cVUP, cRotationMatrix);

                // Normalize all of the Camera directions since they have changed
                NormalizeCameraAndCalculateProperUpAndRightDirections();
            }
        }

        // Class to hold information about the Position, Size, and Visibility of an Object
        class CObject
        {
            public Vector3 sPosition = Vector3.Zero;
            public Vector3 sVelocity = Vector3.Zero;
            public float fSize = 20.0f;
            public bool bVisible = false;
            public TimeSpan cTimeAliveInSeconds = new TimeSpan();

            public void Update(float fElapsedTimeInSeconds)
            {
                sPosition += sVelocity * fElapsedTimeInSeconds;
                cTimeAliveInSeconds += TimeSpan.FromSeconds(fElapsedTimeInSeconds);
            }
        }

        // Enumeration of all the Particle System Effects
        enum EPSEffects
        {
            Random = 0,
            Fire,
            Smoke,
            Snow,
            SquarePattern,
            Fountain,
            Random2D,
            GasFall,
            Dot,
            Fireworks,
            Figure8,
            Star,
            Ball,
            RotatingQuad,
            Box,
            Image,
            AnimatedTexturedQuad,
            Sprite,
            AnimatedSprite,
            Pixel,
            Magnets,
            Sparkler,
            Grid,
            GridQuad,
            Sphere,
			MultipleParticleImages,
            ExplosionFireSmoke,
            ExplosionFlash,
            ExplosionFlyingSparks,
            ExplosionSmokeTrails,
            ExplosionRoundSparks,
            ExplosionDebris,
            ExplosionShockwave,
            Explosion,
            PixelParticleSystemTemplate,
            SpriteParticleSystemTemplate,
            PointSpriteParticleSystemTemplate,
            QuadParticleSystemTemplate,
            TexturedQuadParticleSystemTemplate,
            DefaultPixelParticleSystemTemplate,
            DefaultSpriteParticleSystemTemplate,
            DefaultPointSpriteParticleSystemTemplate,
            DefaultQuadParticleSystemTemplate,
            DefaultTexturedQuadParticleSystemTemplate,
            LastInList = 43     // This value should be the same as the Effect with the largest value
        }

		// List of all the textures
        enum ETextures
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
            CrossArrow,
            Dog,
            Donut,
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
            LastInList = 54     // This value should be the same as the Texture with the largest value
        }

        // Initialize which Particle System to use
        EPSEffects meCurrentPS = EPSEffects.Random;

        // Initialize the Texture to use
        ETextures meCurrentTexture = ETextures.Bubble;

        GraphicsDeviceManager mcGraphics;       // Handle to the Graphics object

        SpriteBatch mcSpriteBatch;              // Batch used to draw Sprites
        SpriteFont mcFont;                      // Font used to draw text
        Model mcFloorModel;                     // Model of the Floor
        Model mcSphereModel;                    // Model of a sphere

        // Initialize the Sphere Object
        CObject mcSphere = new CObject();

        Random mcRandom = new Random();         // Random number generator

        // Input States
        KeyboardState mcCurrentKeyboardState;   // Holds the Keyboard's Current State
        KeyboardState mcPreviousKeyboardState;  // Holds the Keyboard's Previous State
        MouseState mcCurrentMouseState;         // Holds the Mouse's Current State
        MouseState mcPreviousMouseState;        // Holds the Mouse's Previous State
        GamePadState mcCurrentGamePadState;     // Holds the GamePad's Current State
        GamePadState mcPreviousGamePadState;    // Holds the GamePad's Previous State

        bool mbShowText = true;                 // Tells if Text should be shown or not
        bool mbShowCommonControls = false;      // Tells if the Common Controls should be shown or not
        bool mbShowPSControls = false;          // Tells if the Particle System specific Controls should be shown or not
        bool mbShowCameraControls = false;      // Tells if the Camera Controls should be shown or not
        TimeSpan mcInputTimeSpan = new TimeSpan();  // Used to control user input speed

        bool mbShowFloor = true;                // Tells if the Floor should be Shown or not
        bool mbPaused = false;                  // Tells if the game should be Paused or not
        bool mbClearScreenEveryFrame = true;    // Tells if the Screen should be Cleared every Frame or not

        // Draw Static Particle variables (2 of the variables are up in the User Settings)
        bool mbDrawStaticPS = false;            // Tells if Static Particles should be drawn or not
        bool mbStaticParticlesDrawn = false;    // Tells if the Static Particles have already been drawn or not
        

        // The World, View, and Projection matrices
        Matrix msWorldMatrix = Matrix.Identity;
        Matrix msViewMatrix = Matrix.Identity;
        Matrix msProjectionMatrix = Matrix.Identity;

        // Variables used to draw the lines indicating positive axis directions
        bool mbShowAxis = false;
        VertexPositionColor[] msaAxisDirectionVertices; // Vertices to draw lines on the floor indicating positive axis directions
        VertexDeclaration mcAxisVertexDeclaration;
        BasicEffect mcAxisEffect;

        // Initialize the Camera
        SCamera msCamera = new SCamera(true);


        // Declare the Particle System Manager to manage the Particle Systems
        ParticleSystemManager mcParticleSystemManager = new ParticleSystemManager();

        // Declare the Particle System Variables
        RandomParticleSystem mcRandomParticleSystem = null;
        FireParticleSystem mcFireParticleSystem = null;
        SmokeParticleSystem mcSmokeParticleSystem = null;
        SnowParticleSystem mcSnowParticleSystem = null;
        SquarePatternParticleSystem mcSquarePatternParticleSystem = null;
        FountainParticleSystem mcFountainParticleSystem = null;
        Random2DParticleSystem mcRandom2DParticleSystem = null;
        GasFallParticleSystem mcGasFallParticleSystem = null;
        DotParticleSystem mcDotParticleSystem = null;
        FireworksParticleSystem mcFireworksParticleSystem = null;
        Figure8ParticleSystem mcFigure8ParticleSystem = null;
        StarParticleSystem mcStarParticleSystem = null;
        BallParticleSystem mcBallParticleSystem = null;
        RotatingQuadsParticleSystem mcRotatingQuadParticleSystem = null;
        BoxParticleSystem mcBoxParticleSystem = null;
        ImageParticleSystem mcImageParticleSystem = null;
        AnimatedQuadParticleSystem mcAnimatedQuadParticleSystem = null;
        SpriteParticleSystem mcSpriteParticleSystem = null;
        AnimatedSpriteParticleSystem mcAnimatedSpriteParticleSystem = null;
        PixelParticleSystem mcPixelParticleSystem = null;
        MagnetsParticleSystem mcMagnetParticleSystem = null;
        SparklerParticleSystem mcSparklerParticleSystem = null;
        GridParticleSystem mcGridParticleSystem = null;
        GridQuadParticleSystem mcGridQuadParticleSystem = null;
        SphereParticleSystem mcSphereParticleSystem = null;
		MultipleParticleImagesParticleSystem mcMultipleImagesParticleSystem = null;
        ExplosionFireSmokeParticleSystem mcExplosionFireSmokeParticleSystem = null;
        ExplosionFlashParticleSystem mcExplosionFlashParticleSystem = null;
        ExplosionFlyingSparksParticleSystem mcExplosionFlyingSparksParticleSystem = null;
        ExplosionSmokeTrailsParticleSystem mcExplosionSmokeTrailsParticleSystem = null;
        ExplosionRoundSparksParticleSystem mcExplosionRoundSparksParticleSystem = null;
        ExplosionDebrisParticleSystem mcExplosionDebrisParticleSystem = null;
        ExplosionShockwaveParticleSystem mcExplosionShockwaveParticleSystem = null;
        ExplosionParticleSystem mcExplosionParticleSystem = null;
        PixelParticleSystemTemplate mcPixelParticleSystemTemplate = null;
        SpriteParticleSystemTemplate mcSpriteParticleSystemTemplate = null;
        PointSpriteParticleSystemTemplate mcPointSpriteParticleSystemTemplate = null;
        QuadParticleSystemTemplate mcQuadParticleSystemTemplate = null;
        TexturedQuadParticleSystemTemplate mcTexturedQuadParticleSystemTemplate = null;
        DefaultPixelParticleSystemTemplate mcDefaultPixelParticleSystemTemplate = null;
        DefaultSpriteParticleSystemTemplate mcDefaultSpriteParticleSystemTemplate = null;
        DefaultPointSpriteParticleSystemTemplate mcDefaultPointSpriteParticleSystemTemplate = null;
        DefaultQuadParticleSystemTemplate mcDefaultQuadParticleSystemTemplate = null;
        DefaultTexturedQuadParticleSystemTemplate mcDefaultTexturedQuadParticleSystemTemplate = null;

        // Declare a Particle System pointer to point to the Current Particle System being used
        IDPSFParticleSystem mcCurrentParticleSystem;
        
        #endregion

        #region Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public GameMain()
        {
            mcGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // If we should not Limit the FPS
            if (!mbLIMIT_FPS)
            {
                // Make the game run as fast as possible (i.e. don't limit the FPS)
                this.IsFixedTimeStep = false;
                mcGraphics.SynchronizeWithVerticalRetrace = false;
            }
            
            // Set the resolution
            mcGraphics.PreferredBackBufferWidth = miWINDOW_WIDTH;
            mcGraphics.PreferredBackBufferHeight = miWINDOW_HEIGHT;
            mcGraphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;

            // Set the Title of the Window
            Window.Title = "Dynamic Particle System Framework Demo";

            // Do we want to show the mouse
            this.IsMouseVisible = false;
        }

        /// <summary>
        /// Load your graphics content
        /// </summary>
        protected override void LoadContent()
        {
            mcSpriteBatch = new SpriteBatch(mcGraphics.GraphicsDevice);

            // Load fonts and models for test application
            mcFont = Content.Load<SpriteFont>("Fonts/font");
            mcFloorModel = Content.Load<Model>("grid");
            mcSphereModel = Content.Load<Model>("SphereHighPoly");

            // Specify vertices indicating positive axis directions
            int iLineLength = 50;
            msaAxisDirectionVertices = new VertexPositionColor[6];
            msaAxisDirectionVertices[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);
            msaAxisDirectionVertices[1] = new VertexPositionColor(new Vector3(iLineLength, 1, 0), Color.Red);
            msaAxisDirectionVertices[2] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Green);
            msaAxisDirectionVertices[3] = new VertexPositionColor(new Vector3(0, iLineLength, 0), Color.Green);
            msaAxisDirectionVertices[4] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Blue);
            msaAxisDirectionVertices[5] = new VertexPositionColor(new Vector3(0, 1, iLineLength), Color.Blue);
            
            mcAxisEffect = new BasicEffect(GraphicsDevice, null);
            mcAxisEffect.VertexColorEnabled = true;
            mcAxisEffect.LightingEnabled = false;
            mcAxisEffect.TextureEnabled = false;
            mcAxisEffect.FogEnabled = false;
            mcAxisVertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);

            // Instanciate all of the Particle Systems
            mcRandomParticleSystem = new RandomParticleSystem(this);
            mcFireParticleSystem = new FireParticleSystem(this);
            mcSmokeParticleSystem = new SmokeParticleSystem(this);
            mcSnowParticleSystem = new SnowParticleSystem(this);
            mcSquarePatternParticleSystem = new SquarePatternParticleSystem(this);
            mcFountainParticleSystem = new FountainParticleSystem(this);
            mcRandom2DParticleSystem = new Random2DParticleSystem(this);
            mcGasFallParticleSystem = new GasFallParticleSystem(this);
            mcDotParticleSystem = new DotParticleSystem(this);
            mcFireworksParticleSystem = new FireworksParticleSystem(this);
            mcFigure8ParticleSystem = new Figure8ParticleSystem(this);
            mcStarParticleSystem = new StarParticleSystem(this);
            mcBallParticleSystem = new BallParticleSystem(this);
            mcRotatingQuadParticleSystem = new RotatingQuadsParticleSystem(this);
            mcBoxParticleSystem = new BoxParticleSystem(this);
            mcImageParticleSystem = new ImageParticleSystem(this);
            mcAnimatedQuadParticleSystem = new AnimatedQuadParticleSystem(this);
            mcSpriteParticleSystem = new SpriteParticleSystem(this);
            mcAnimatedSpriteParticleSystem = new AnimatedSpriteParticleSystem(this);
            mcPixelParticleSystem = new PixelParticleSystem(this);
            mcMagnetParticleSystem = new MagnetsParticleSystem(this);
            mcSparklerParticleSystem = new SparklerParticleSystem(this);
            mcGridParticleSystem = new GridParticleSystem(this);
            mcGridQuadParticleSystem = new GridQuadParticleSystem(this);
            mcSphereParticleSystem = new SphereParticleSystem(this);
			mcMultipleImagesParticleSystem = new MultipleParticleImagesParticleSystem(this);
            mcExplosionFireSmokeParticleSystem = new ExplosionFireSmokeParticleSystem(this);
            mcExplosionFlashParticleSystem = new ExplosionFlashParticleSystem(this);
            mcExplosionFlyingSparksParticleSystem = new ExplosionFlyingSparksParticleSystem(this);
            mcExplosionSmokeTrailsParticleSystem = new ExplosionSmokeTrailsParticleSystem(this);
            mcExplosionRoundSparksParticleSystem = new ExplosionRoundSparksParticleSystem(this);
            mcExplosionDebrisParticleSystem = new ExplosionDebrisParticleSystem(this);
            mcExplosionShockwaveParticleSystem = new ExplosionShockwaveParticleSystem(this);
            mcExplosionParticleSystem = new ExplosionParticleSystem(this);
            mcPixelParticleSystemTemplate = new PixelParticleSystemTemplate(this);
            mcSpriteParticleSystemTemplate = new SpriteParticleSystemTemplate(this);
            mcPointSpriteParticleSystemTemplate = new PointSpriteParticleSystemTemplate(this);
            mcQuadParticleSystemTemplate = new QuadParticleSystemTemplate(this);
            mcTexturedQuadParticleSystemTemplate = new TexturedQuadParticleSystemTemplate(this);
            mcDefaultPixelParticleSystemTemplate = new DefaultPixelParticleSystemTemplate(this);
            mcDefaultSpriteParticleSystemTemplate = new DefaultSpriteParticleSystemTemplate(this);
            mcDefaultPointSpriteParticleSystemTemplate = new DefaultPointSpriteParticleSystemTemplate(this);
            mcDefaultQuadParticleSystemTemplate = new DefaultQuadParticleSystemTemplate(this);
            mcDefaultTexturedQuadParticleSystemTemplate = new DefaultTexturedQuadParticleSystemTemplate(this);

            mcRandomParticleSystem.DrawOrder = 100;
            mcFireParticleSystem.DrawOrder = 200;
            mcSmokeParticleSystem.DrawOrder = 300;
            mcSnowParticleSystem.DrawOrder = 400;
            mcSquarePatternParticleSystem.DrawOrder = 500;
            mcFountainParticleSystem.DrawOrder = 600;
            mcRandom2DParticleSystem.DrawOrder = 700;
            mcGasFallParticleSystem.DrawOrder = 800;
            mcDotParticleSystem.DrawOrder = 900;
            mcFireworksParticleSystem.DrawOrder = 1000;
            mcFigure8ParticleSystem.DrawOrder = 1100;
            mcStarParticleSystem.DrawOrder = 1200;
            mcBallParticleSystem.DrawOrder = 1300;
            mcRotatingQuadParticleSystem.DrawOrder = 1400;
            mcBoxParticleSystem.DrawOrder = 1500;
            mcImageParticleSystem.DrawOrder = 1600;
            mcAnimatedQuadParticleSystem.DrawOrder = 1700;
            mcSpriteParticleSystem.DrawOrder = 1800;
            mcAnimatedSpriteParticleSystem.DrawOrder = 1900;
			mcPixelParticleSystem.DrawOrder = 1925;
            mcMagnetParticleSystem.DrawOrder = 1950;
            mcSparklerParticleSystem.DrawOrder = 1960;
            mcGridParticleSystem.DrawOrder = 1970;
            mcGridQuadParticleSystem.DrawOrder = 1980;
            mcSphereParticleSystem.DrawOrder = 1990;
			mcMultipleImagesParticleSystem.DrawOrder = 2000;
            mcExplosionFireSmokeParticleSystem.DrawOrder = 2010;
            mcExplosionFlashParticleSystem.DrawOrder = 2020;
            mcExplosionFlyingSparksParticleSystem.DrawOrder = 2030;
            mcExplosionSmokeTrailsParticleSystem.DrawOrder = 2040;
            mcExplosionRoundSparksParticleSystem.DrawOrder = 2050;
            mcExplosionDebrisParticleSystem.DrawOrder = 2060;
            mcExplosionShockwaveParticleSystem.DrawOrder = 2070;
            mcExplosionParticleSystem.DrawOrder = 2080;
            mcPixelParticleSystemTemplate.DrawOrder = 2100;
            mcSpriteParticleSystemTemplate.DrawOrder = 2200;
            mcPointSpriteParticleSystemTemplate.DrawOrder = 2300;
            mcQuadParticleSystemTemplate.DrawOrder = 2350;
            mcTexturedQuadParticleSystemTemplate.DrawOrder = 2400;
            mcDefaultPixelParticleSystemTemplate.DrawOrder = 2500;
            mcDefaultSpriteParticleSystemTemplate.DrawOrder = 2600;
            mcDefaultPointSpriteParticleSystemTemplate.DrawOrder = 2700;
            mcDefaultQuadParticleSystemTemplate.DrawOrder = 2750;
            mcDefaultTexturedQuadParticleSystemTemplate.DrawOrder = 2800;

            // Add all Particle Systems to the Particle System Manager
            mcParticleSystemManager.AddParticleSystem(mcRandomParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcFireParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcSmokeParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcSnowParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcSquarePatternParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcFountainParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcRandom2DParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcGasFallParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcDotParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcFireworksParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcFigure8ParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcStarParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcBallParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcRotatingQuadParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcBoxParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcImageParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcAnimatedQuadParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcSpriteParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcAnimatedSpriteParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcPixelParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcMagnetParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcSparklerParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcGridParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcGridQuadParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcSphereParticleSystem);
			mcParticleSystemManager.AddParticleSystem(mcMultipleImagesParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcExplosionFireSmokeParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcExplosionFlashParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcExplosionFlyingSparksParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcExplosionSmokeTrailsParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcExplosionRoundSparksParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcExplosionDebrisParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcExplosionShockwaveParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcExplosionParticleSystem);
            mcParticleSystemManager.AddParticleSystem(mcPixelParticleSystemTemplate);
            mcParticleSystemManager.AddParticleSystem(mcSpriteParticleSystemTemplate);
            mcParticleSystemManager.AddParticleSystem(mcPointSpriteParticleSystemTemplate);
            mcParticleSystemManager.AddParticleSystem(mcQuadParticleSystemTemplate);
            mcParticleSystemManager.AddParticleSystem(mcTexturedQuadParticleSystemTemplate);
            mcParticleSystemManager.AddParticleSystem(mcDefaultPixelParticleSystemTemplate);
            mcParticleSystemManager.AddParticleSystem(mcDefaultSpriteParticleSystemTemplate);
            mcParticleSystemManager.AddParticleSystem(mcDefaultPointSpriteParticleSystemTemplate);
            mcParticleSystemManager.AddParticleSystem(mcDefaultQuadParticleSystemTemplate);
            mcParticleSystemManager.AddParticleSystem(mcDefaultTexturedQuadParticleSystemTemplate);

            // Set how often the Particle Systems should be Updated
            mcParticleSystemManager.UpdatesPerSecond = miPARTICLE_SYSTEM_UPDATES_PER_SECOND;

            // Initialize the Current Particle System
            InitializeCurrentParticleSystem();
        }

        public void InitializeCurrentParticleSystem()
        {
            // If the Current Particle System has been set
            if (mcCurrentParticleSystem != null)
            {
                // Destroy the Current Particle System.
                // This frees up any resources/memory held by the Particle System, so it's
                // good to destroy them if we know they won't be used for a while
                mcCurrentParticleSystem.Destroy();
            }

            // Initialize the Current Particle System
            switch (meCurrentPS)
            {
                default:
                case EPSEffects.Random: mcCurrentParticleSystem = mcRandomParticleSystem; break;
                case EPSEffects.Fire: mcCurrentParticleSystem = mcFireParticleSystem; break;
                case EPSEffects.Smoke: mcCurrentParticleSystem = mcSmokeParticleSystem; break;
                case EPSEffects.Snow: mcCurrentParticleSystem = mcSnowParticleSystem; break;
                case EPSEffects.SquarePattern: mcCurrentParticleSystem = mcSquarePatternParticleSystem; break;
                case EPSEffects.Fountain: mcCurrentParticleSystem = mcFountainParticleSystem; break;
                case EPSEffects.Random2D: mcCurrentParticleSystem = mcRandom2DParticleSystem; break;
                case EPSEffects.GasFall: mcCurrentParticleSystem = mcGasFallParticleSystem; break;
                case EPSEffects.Dot: mcCurrentParticleSystem = mcDotParticleSystem; break;
                case EPSEffects.Fireworks: mcCurrentParticleSystem = mcFireworksParticleSystem; break;
                case EPSEffects.Figure8: mcCurrentParticleSystem = mcFigure8ParticleSystem; break;
                case EPSEffects.Star: mcCurrentParticleSystem = mcStarParticleSystem; break;
                case EPSEffects.Ball: mcCurrentParticleSystem = mcBallParticleSystem; break;
                case EPSEffects.RotatingQuad: mcCurrentParticleSystem = mcRotatingQuadParticleSystem; break;
                case EPSEffects.Box: mcCurrentParticleSystem = mcBoxParticleSystem; break;
                case EPSEffects.Image: mcCurrentParticleSystem = mcImageParticleSystem; break;
                case EPSEffects.AnimatedTexturedQuad: mcCurrentParticleSystem = mcAnimatedQuadParticleSystem; break;
                case EPSEffects.Sprite: mcCurrentParticleSystem = mcSpriteParticleSystem; break;
                case EPSEffects.AnimatedSprite: mcCurrentParticleSystem = mcAnimatedSpriteParticleSystem; break;
                case EPSEffects.Pixel: mcCurrentParticleSystem = mcPixelParticleSystem; break;
                case EPSEffects.Magnets: mcCurrentParticleSystem = mcMagnetParticleSystem; break;
                case EPSEffects.Sparkler: mcCurrentParticleSystem = mcSparklerParticleSystem; break;
                case EPSEffects.Grid: mcCurrentParticleSystem = mcGridParticleSystem; break;
                case EPSEffects.GridQuad: mcCurrentParticleSystem = mcGridQuadParticleSystem; break;
                case EPSEffects.Sphere: mcCurrentParticleSystem = mcSphereParticleSystem; break;
				case EPSEffects.MultipleParticleImages:	mcCurrentParticleSystem = mcMultipleImagesParticleSystem; break;
                case EPSEffects.ExplosionFireSmoke: mcCurrentParticleSystem = mcExplosionFireSmokeParticleSystem; break;
                case EPSEffects.ExplosionFlash: mcCurrentParticleSystem = mcExplosionFlashParticleSystem; break;
                case EPSEffects.ExplosionFlyingSparks: mcCurrentParticleSystem = mcExplosionFlyingSparksParticleSystem; break;
                case EPSEffects.ExplosionSmokeTrails: mcCurrentParticleSystem = mcExplosionSmokeTrailsParticleSystem; break;
                case EPSEffects.ExplosionRoundSparks: mcCurrentParticleSystem = mcExplosionRoundSparksParticleSystem; break;
                case EPSEffects.ExplosionDebris: mcCurrentParticleSystem = mcExplosionDebrisParticleSystem; break;
                case EPSEffects.ExplosionShockwave: mcCurrentParticleSystem = mcExplosionShockwaveParticleSystem; break;
                case EPSEffects.Explosion: mcCurrentParticleSystem = mcExplosionParticleSystem; break;
                case EPSEffects.PixelParticleSystemTemplate: mcCurrentParticleSystem = mcPixelParticleSystemTemplate; break;
                case EPSEffects.SpriteParticleSystemTemplate: mcCurrentParticleSystem = mcSpriteParticleSystemTemplate; break;
                case EPSEffects.PointSpriteParticleSystemTemplate: mcCurrentParticleSystem = mcPointSpriteParticleSystemTemplate; break;
                case EPSEffects.QuadParticleSystemTemplate: mcCurrentParticleSystem = mcQuadParticleSystemTemplate; break;
                case EPSEffects.TexturedQuadParticleSystemTemplate: mcCurrentParticleSystem = mcTexturedQuadParticleSystemTemplate; break;
                case EPSEffects.DefaultPixelParticleSystemTemplate: mcCurrentParticleSystem = mcDefaultPixelParticleSystemTemplate; break;
                case EPSEffects.DefaultSpriteParticleSystemTemplate: mcCurrentParticleSystem = mcDefaultSpriteParticleSystemTemplate; break;
                case EPSEffects.DefaultPointSpriteParticleSystemTemplate: mcCurrentParticleSystem = mcDefaultPointSpriteParticleSystemTemplate; break;
                case EPSEffects.DefaultQuadParticleSystemTemplate: mcCurrentParticleSystem = mcDefaultQuadParticleSystemTemplate; break;
                case EPSEffects.DefaultTexturedQuadParticleSystemTemplate: mcCurrentParticleSystem = mcDefaultTexturedQuadParticleSystemTemplate; break;
            }

            // Initialize the Particle System
            mcCurrentParticleSystem.AutoInitialize(this.GraphicsDevice, this.Content, null);

            // Do any necessary after initialization work
            switch (meCurrentPS)
            {
                case EPSEffects.ExplosionFireSmoke: mcExplosionFireSmokeParticleSystem.SetupToAutoExplodeEveryInterval(1); break;
                case EPSEffects.ExplosionFlash: mcExplosionFlashParticleSystem.SetupToAutoExplodeEveryInterval(1); break;
                case EPSEffects.ExplosionFlyingSparks: mcExplosionFlyingSparksParticleSystem.SetupToAutoExplodeEveryInterval(2); break;
                case EPSEffects.ExplosionSmokeTrails: mcExplosionSmokeTrailsParticleSystem.SetupToAutoExplodeEveryInterval(2); break;
                case EPSEffects.ExplosionRoundSparks: mcExplosionRoundSparksParticleSystem.SetupToAutoExplodeEveryInterval(2); break;
                case EPSEffects.ExplosionDebris: mcExplosionDebrisParticleSystem.SetupToAutoExplodeEveryInterval(2); break;
                case EPSEffects.ExplosionShockwave: mcExplosionShockwaveParticleSystem.SetupToAutoExplodeEveryInterval(2); break;
                case EPSEffects.Explosion: mcExplosionParticleSystem.SetupToAutoExplodeEveryInterval(2); break;
            }
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
            
            // If the Game is Paused
            if (mbPaused)
            {
                // Exit without updating anything
                return;
            }

            // If the Current Particle System is Initialized
            if (mcCurrentParticleSystem != null && mcCurrentParticleSystem.IsInitialized)
            {
				// Update the Quad Particle Systems to know where the Camera is so that they can display
				// the particles as billboards if needed (i.e. have particle always face the camera).
				mcParticleSystemManager.SetCameraPositionForAllParticleSystems(msCamera.Position);

                // If Static Particles should be drawn
                if (mbDrawStaticPS)
                {
                    // If the Static Particles haven't been drawn yet
                    if (!mbStaticParticlesDrawn)
                    {
                        // Set the World, View, and Projection Matrices for all Particle Systems
                        mcParticleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(msWorldMatrix, msViewMatrix, msProjectionMatrix);

                        // Update the Particle System iteratively by the Time Step amount until the 
                        // Particle System behaviour over the Total Time has been drawn
                        float fElapsedTime = 0;
                        while (fElapsedTime < mfStaticParticleTotalTime)
                        {
                            // Update and draw this frame of the Particle System
                            mcParticleSystemManager.UpdateAllParticleSystems(mfStaticParticleTimeStep);
                            mcParticleSystemManager.DrawAllParticleSystems();
                            fElapsedTime += mfStaticParticleTimeStep;
                        }
                        mbStaticParticlesDrawn = true;

                        mcSpriteBatch.Begin();
                        mcSpriteBatch.DrawString(mcFont, "F6 to continue", new Vector2(310, 25), Color.LawnGreen);
                        mcSpriteBatch.End();
                    }
                }
                // Else the Particle Systems should be drawn normally
                else
                {
                    // Update all Particle Systems manually
                    mcParticleSystemManager.UpdateAllParticleSystems((float)cGameTime.ElapsedGameTime.TotalSeconds);
                }


                // If the Sphere is Visible and we are on the Smoke Particle System
                if (mcSphere.bVisible && meCurrentPS == EPSEffects.Smoke)
                {
                    // Update it
                    mcSphere.Update((float)cGameTime.ElapsedGameTime.TotalSeconds);

                    // Update the PS's External Object Position to the Sphere's Position
                    mcSmokeParticleSystem.mcExternalObjectPosition = mcSphere.sPosition;

                    // If the Sphere has been alive long enough
                    if (mcSphere.cTimeAliveInSeconds > TimeSpan.FromSeconds(6.0f))
                    {
                        mcSphere.bVisible = false;
                        mcSmokeParticleSystem.StopParticleAttractionAndRepulsionToExternalObject();
                    }
                }
            }
        
            // Update any other Drawable Game Components
            base.Update(cGameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself
        /// </summary>
        protected override void Draw(GameTime cGameTime)
        {
            // Get a handle to the Graphics Device used for drawing
            GraphicsDevice cGraphicsDevice = mcGraphics.GraphicsDevice;

            // If Static Particles should be drawn, exit without redrawing over them
            if (mbDrawStaticPS)
            {
                return;
            }

            // If the Screen should be Cleared each frame
            if (mbClearScreenEveryFrame)
            {
                // Clear the scene
                cGraphicsDevice.Clear(msBACKGROUND_COLOR);
            }

            // Compute the Aspect Ratio of the window
            float fAspectRatio = (float)cGraphicsDevice.Viewport.Width / (float)cGraphicsDevice.Viewport.Height;

            // Setup the View matrix depending on which Camera mode is being used
            // If we are using the Fixed Camera
            if (msCamera.bUsingFixedCamera)
            {
                // Set up the View matrix according to the Camera's arc, rotation, and distance from the Offset position
                msViewMatrix = Matrix.CreateTranslation(msCamera.sFixedCameraLookAtPosition) *
                                     Matrix.CreateRotationY(MathHelper.ToRadians(msCamera.fCameraRotation)) *
                                     Matrix.CreateRotationX(MathHelper.ToRadians(msCamera.fCameraArc)) *
                                     Matrix.CreateLookAt(new Vector3(0, 0, -msCamera.fCameraDistance),
                                                         new Vector3(0, 0, 0), Vector3.Up);
            }
            // Else we are using the Free Camera
            else
            {
                // Set up our View matrix specifying the Camera position, a point to look-at, and a direction for which way is up
                msViewMatrix = Matrix.CreateLookAt(msCamera.sVRP, msCamera.sVRP + msCamera.cVPN, msCamera.cVUP);
            }

            // Setup the Projection matrix by specifying the field of view (1/4 pi), aspect ratio, and the near and far clipping planes
            msProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, fAspectRatio, 1, 10000);


            // Draw the Floor and Sphere
            DrawModels(msWorldMatrix, msViewMatrix, msProjectionMatrix);

            // If the Axis' should be drawn
            if (mbShowAxis)
            {
                // Draw lines indicating positive axis directions
                DrawAxis(msWorldMatrix, msViewMatrix, msProjectionMatrix);
            }


            // Draw any other Drawable Game Components that may need to be drawn.
            // Call this before drawing our Particle Systems, so that our 2D Sprite particles
            // show up ontop of the any other 2D Sprites drawn.
            base.Draw(cGameTime);

            // Set the World, View, and Projection Matrices for the Particle Systems
            mcParticleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(msWorldMatrix, msViewMatrix, msProjectionMatrix);

            // Draw the Particle Systems manually
            mcParticleSystemManager.DrawAllParticleSystems();


            // Update the Frames Per Second to be displayed
            FPS.Update((float)cGameTime.ElapsedRealTime.TotalSeconds);

            // Draw the Text to the screen last, so it is always on top
            DrawText();
        }

        /// <summary>
        /// Function to draw Text to the screen
        /// </summary>
        void DrawText()
        {
            // If no Text should be shown
            if (!mbShowText)
            {
                // Exit the function before drawing any Text
                return;
            }

            // Specify the text Colors to use
            Color sPropertyColor = Color.WhiteSmoke;
            Color sValueColor = Color.Yellow;
            Color sControlColor = Color.PowderBlue;

            // If we don't have a handle to a particle system, it is because we serialized it
            if (mcCurrentParticleSystem == null)
            {
                mcSpriteBatch.Begin();
                mcSpriteBatch.DrawString(mcFont, "Particle system has been serialized to the file: " + msSerializedPSFileName + ".", new Vector2(25, 200), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "To deserialize the particle system from the file, restoring the instance of", new Vector2(25, 225), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "the particle system,", new Vector2(25, 250), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "press F9", new Vector2(210, 250), sControlColor);
                mcSpriteBatch.End();
                return;
            }
			
			// If the Particle System has been destroyed, just write that to the screen and exit
			if (!mcCurrentParticleSystem.IsInitialized)
			{
				mcSpriteBatch.Begin();
				mcSpriteBatch.DrawString(mcFont, "The current particle system has been destroyed.", new Vector2(140, 200), sPropertyColor);
				mcSpriteBatch.DrawString(mcFont, "Press G / H to switch to a different particle system.", new Vector2(125, 225), sPropertyColor);
				mcSpriteBatch.End();
				return;
			}

            // Get the Name of the Particle System and how many Particles are currently Active
            string sEffectName = "";
            int iNumberOfActiveParticles = mcParticleSystemManager.TotalNumberOfActiveParticles;
            int iNumberOfParticlesAllocatedInMemory = mcParticleSystemManager.TotalNumberOfParticlesAllocatedInMemory;
            switch (meCurrentPS)
            {
                default: break;
                case EPSEffects.Random: sEffectName = mcRandomParticleSystem.Name; break;
                case EPSEffects.Fire:
                    sEffectName = mcFireParticleSystem.Name;

                    // Include the Embedded Particle System Particles
                    iNumberOfActiveParticles += mcFireParticleSystem.mcSmokeParticleSystem.NumberOfActiveParticles;
                    iNumberOfParticlesAllocatedInMemory += mcFireParticleSystem.mcSmokeParticleSystem.NumberOfParticlesAllocatedInMemory;
                break;
                case EPSEffects.Smoke: sEffectName = mcSmokeParticleSystem.Name; break;
                case EPSEffects.Snow: sEffectName = mcSnowParticleSystem.Name; break;
                case EPSEffects.SquarePattern: sEffectName = mcSquarePatternParticleSystem.Name; break;
                case EPSEffects.Fountain: sEffectName = mcFountainParticleSystem.Name; break;
                case EPSEffects.Random2D: sEffectName = mcRandom2DParticleSystem.Name; break;
                case EPSEffects.GasFall: sEffectName = mcGasFallParticleSystem.Name; break;
                case EPSEffects.Dot: sEffectName = mcDotParticleSystem.Name; break;
                case EPSEffects.Fireworks:
                    sEffectName = mcFireworksParticleSystem.Name;

                    // Included the Embedded Particle System Particles
                    iNumberOfActiveParticles += mcFireworksParticleSystem.mcFireworksExplosionParticleSystem1.NumberOfActiveParticles;
                    iNumberOfActiveParticles += mcFireworksParticleSystem.mcFireworksExplosionParticleSystem2.NumberOfActiveParticles;
                    iNumberOfActiveParticles += mcFireworksParticleSystem.mcFireworksExplosionParticleSystem3.NumberOfActiveParticles;
                    iNumberOfActiveParticles += mcFireworksParticleSystem.mcFireworksExplosionParticleSystem4.NumberOfActiveParticles;
                    iNumberOfActiveParticles += mcFireworksParticleSystem.mcFireworksExplosionSmokeParticleSystem.NumberOfActiveParticles;

                    iNumberOfParticlesAllocatedInMemory += mcFireworksParticleSystem.mcFireworksExplosionParticleSystem1.NumberOfParticlesAllocatedInMemory;
                    iNumberOfParticlesAllocatedInMemory += mcFireworksParticleSystem.mcFireworksExplosionParticleSystem2.NumberOfParticlesAllocatedInMemory;
                    iNumberOfParticlesAllocatedInMemory += mcFireworksParticleSystem.mcFireworksExplosionParticleSystem3.NumberOfParticlesAllocatedInMemory;
                    iNumberOfParticlesAllocatedInMemory += mcFireworksParticleSystem.mcFireworksExplosionParticleSystem4.NumberOfParticlesAllocatedInMemory;
                    iNumberOfParticlesAllocatedInMemory += mcFireworksParticleSystem.mcFireworksExplosionSmokeParticleSystem.NumberOfParticlesAllocatedInMemory;
                break;
                case EPSEffects.Figure8: sEffectName = mcFigure8ParticleSystem.Name; break;
                case EPSEffects.Star: sEffectName = mcStarParticleSystem.Name; break;
                case EPSEffects.Ball: sEffectName = mcBallParticleSystem.Name; break;
                case EPSEffects.RotatingQuad: sEffectName = mcRotatingQuadParticleSystem.Name; break;
                case EPSEffects.Box: sEffectName = mcBoxParticleSystem.Name; break;
                case EPSEffects.Image: sEffectName = mcImageParticleSystem.Name; break;
                case EPSEffects.AnimatedTexturedQuad: sEffectName = mcAnimatedQuadParticleSystem.Name; break;
                case EPSEffects.Sprite: sEffectName = mcSpriteParticleSystem.Name; break;
                case EPSEffects.AnimatedSprite: sEffectName = mcAnimatedSpriteParticleSystem.Name; break;
                case EPSEffects.Pixel: sEffectName = mcPixelParticleSystem.Name; break;
                case EPSEffects.Magnets: sEffectName = mcMagnetParticleSystem.Name; break;
                case EPSEffects.Sparkler: sEffectName = mcSparklerParticleSystem.Name; break;
                case EPSEffects.Grid: sEffectName = mcGridParticleSystem.Name; break;
                case EPSEffects.GridQuad: sEffectName = mcGridQuadParticleSystem.Name; break;
                case EPSEffects.Sphere: sEffectName = mcSphereParticleSystem.Name; break;
				case EPSEffects.MultipleParticleImages:	sEffectName = mcMultipleImagesParticleSystem.Name; break;
                case EPSEffects.ExplosionFireSmoke: sEffectName = mcExplosionFireSmokeParticleSystem.Name; break;
                case EPSEffects.ExplosionFlash: sEffectName = mcExplosionFlashParticleSystem.Name; break;
                case EPSEffects.ExplosionFlyingSparks: sEffectName = mcExplosionFlyingSparksParticleSystem.Name; break;
                case EPSEffects.ExplosionSmokeTrails: sEffectName = mcExplosionSmokeTrailsParticleSystem.Name; break;
                case EPSEffects.ExplosionRoundSparks: sEffectName = mcExplosionRoundSparksParticleSystem.Name; break;
                case EPSEffects.ExplosionDebris: sEffectName = mcExplosionDebrisParticleSystem.Name; break;
                case EPSEffects.ExplosionShockwave: sEffectName = mcExplosionShockwaveParticleSystem.Name; break;
                case EPSEffects.Explosion: 
                    sEffectName = mcExplosionParticleSystem.Name;

                    // Included the Embedded Particle System Particles
                    iNumberOfActiveParticles += mcExplosionParticleSystem.TotalNumberOfActiveParticles;
                    iNumberOfParticlesAllocatedInMemory += mcExplosionParticleSystem.TotalNumberOfParticlesAllocatedInMemory;
                break;
                case EPSEffects.PixelParticleSystemTemplate: sEffectName = "Pixel Particle System Template"; break;
                case EPSEffects.SpriteParticleSystemTemplate: sEffectName = "Sprite Particle System Template"; break;
                case EPSEffects.PointSpriteParticleSystemTemplate: sEffectName = "Point Sprite Particle System Template"; break;
                case EPSEffects.QuadParticleSystemTemplate: sEffectName = "Quad Particle System Template"; break;
                case EPSEffects.TexturedQuadParticleSystemTemplate: sEffectName = "Textured Quad Particle System Template"; break;
                case EPSEffects.DefaultPixelParticleSystemTemplate: sEffectName = mcDefaultPixelParticleSystemTemplate.Name; break;
                case EPSEffects.DefaultSpriteParticleSystemTemplate: sEffectName = mcDefaultSpriteParticleSystemTemplate.Name; break;
                case EPSEffects.DefaultPointSpriteParticleSystemTemplate: sEffectName = mcDefaultPointSpriteParticleSystemTemplate.Name; break;
                case EPSEffects.DefaultQuadParticleSystemTemplate: sEffectName = mcDefaultQuadParticleSystemTemplate.Name; break;
                case EPSEffects.DefaultTexturedQuadParticleSystemTemplate: sEffectName = mcDefaultTexturedQuadParticleSystemTemplate.Name; break;
            }

            // Convert numbers to strings
            string sFPSValue = FPS.CurrentFPS.ToString();
            string sAvgFPSValue = FPS.AverageFPS.ToString("0.00");
            string sTotalParticleCountValue = iNumberOfActiveParticles.ToString();
            string sEmitterOnValue = (mcCurrentParticleSystem.Emitter.EmitParticlesAutomatically ? "On" : "Off");
            string sParticleSystemEffectValue = sEffectName;
            string sParticlesPerSecondValue = mcCurrentParticleSystem.Emitter.ParticlesPerSecond.ToString();
            string sCameraModeValue = msCamera.bUsingFixedCamera ? "Fixed" : "Free";
            string sPSSpeedScale = mcParticleSystemManager.SimulationSpeed.ToString("0.0");
            string sCameraPosition = "(" + msCamera.Position.X.ToString("0") + "," + msCamera.Position.Y.ToString("0") + "," + msCamera.Position.Z.ToString("0") + ")";
            string sAllocatedParticles = iNumberOfParticlesAllocatedInMemory.ToString();
            string sTexture = "N/A";
            if (mcCurrentParticleSystem.Texture != null)
            {
                sTexture = mcCurrentParticleSystem.Texture.Name.TrimStart("Textures/".ToCharArray());
            }

            // Draw all of the text
            mcSpriteBatch.Begin();

            mcSpriteBatch.DrawString(mcFont, "FPS:", new Vector2(GetTextSafeArea().Left + 5, GetTextSafeArea().Bottom - 50), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, sFPSValue, new Vector2(GetTextSafeArea().Left + 50, GetTextSafeArea().Bottom - 50), sValueColor);

            mcSpriteBatch.DrawString(mcFont, "Allocated:", new Vector2(GetTextSafeArea().Left + 120, GetTextSafeArea().Bottom - 50), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, sAllocatedParticles, new Vector2(GetTextSafeArea().Left + 210, GetTextSafeArea().Bottom - 50), sValueColor);

            //mcSpriteBatch.DrawString(mcFont, "Position:", new Vector2(GetTextSafeArea().Left + 275, GetTextSafeArea().Bottom - 75), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, sCameraPosition, new Vector2(GetTextSafeArea().Left + 280, GetTextSafeArea().Bottom - 50), sValueColor);

            mcSpriteBatch.DrawString(mcFont, "Texture:", new Vector2(GetTextSafeArea().Left + 440, GetTextSafeArea().Bottom - 50), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, sTexture, new Vector2(GetTextSafeArea().Left + 520, GetTextSafeArea().Bottom - 50), sValueColor);

            mcSpriteBatch.DrawString(mcFont, "Speed:", new Vector2(GetTextSafeArea().Right - 100, GetTextSafeArea().Bottom - 50), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, sPSSpeedScale, new Vector2(GetTextSafeArea().Right - 35, GetTextSafeArea().Bottom - 50), sValueColor);

            mcSpriteBatch.DrawString(mcFont, "Avg:", new Vector2(GetTextSafeArea().Left + 5, GetTextSafeArea().Bottom - 25), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, sAvgFPSValue, new Vector2(GetTextSafeArea().Left + 50, GetTextSafeArea().Bottom - 25), sValueColor);

            mcSpriteBatch.DrawString(mcFont, "Particles:", new Vector2(GetTextSafeArea().Left + 120, GetTextSafeArea().Bottom - 25), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, sTotalParticleCountValue, new Vector2(GetTextSafeArea().Left + 205, GetTextSafeArea().Bottom - 25), sValueColor);

            mcSpriteBatch.DrawString(mcFont, "Emitter:", new Vector2(GetTextSafeArea().Left + 275, GetTextSafeArea().Bottom - 25), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, sEmitterOnValue, new Vector2(GetTextSafeArea().Left + 345, GetTextSafeArea().Bottom - 25), sValueColor);

            mcSpriteBatch.DrawString(mcFont, "Particles Per Second:", new Vector2(GetTextSafeArea().Left + 390, GetTextSafeArea().Bottom - 25), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, sParticlesPerSecondValue, new Vector2(GetTextSafeArea().Left + 590, GetTextSafeArea().Bottom - 25), sValueColor);

            mcSpriteBatch.DrawString(mcFont, "Camera:", new Vector2(GetTextSafeArea().Left + 660, GetTextSafeArea().Bottom - 25), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, sCameraModeValue, new Vector2(GetTextSafeArea().Left + 740, GetTextSafeArea().Bottom - 25), sValueColor);

            mcSpriteBatch.DrawString(mcFont, "Effect:", new Vector2(GetTextSafeArea().Left + 5, GetTextSafeArea().Top + 2), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, sParticleSystemEffectValue, new Vector2(GetTextSafeArea().Left + 70, GetTextSafeArea().Top + 2), sValueColor);

            mcSpriteBatch.DrawString(mcFont, "Show/Hide Controls:", new Vector2(GetTextSafeArea().Right - 260, GetTextSafeArea().Top + 2), sPropertyColor);
            mcSpriteBatch.DrawString(mcFont, "F1 - F4", new Vector2(GetTextSafeArea().Right - 70, GetTextSafeArea().Top + 2), sControlColor);

            // Display particle system specific values
            switch (meCurrentPS)
            {
                default: break;

                case EPSEffects.Random:
                    mcSpriteBatch.DrawString(mcFont, "Particle Size:", new Vector2(GetTextSafeArea().Left + 300, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcRandomParticleSystem.InitialProperties.StartSizeMin.ToString(), new Vector2(GetTextSafeArea().Left + 425, GetTextSafeArea().Top + 2), sValueColor);
                break;

                case EPSEffects.Fire:
                    mcSpriteBatch.DrawString(mcFont, "Smokiness:", new Vector2(GetTextSafeArea().Left + 300, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcFireParticleSystem.GetAmountOfSmokeBeingReleased().ToString("0.00"), new Vector2(GetTextSafeArea().Left + 410, GetTextSafeArea().Top + 2), sValueColor);
                break;

                case EPSEffects.Fountain:
                    mcSpriteBatch.DrawString(mcFont, "Bounciness:", new Vector2(GetTextSafeArea().Left + 300, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcFountainParticleSystem.mfBounciness.ToString("0.00"), new Vector2(GetTextSafeArea().Left + 410, GetTextSafeArea().Top + 2), sValueColor);
                break;

                case EPSEffects.Star:
                    mcSpriteBatch.DrawString(mcFont, "Emitter Intermittance Mode:", new Vector2(GetTextSafeArea().Left + 180, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcStarParticleSystem.miIntermittanceTimeMode.ToString("0"), new Vector2(GetTextSafeArea().Left + 435, GetTextSafeArea().Top + 2), sValueColor);
                break;

                case EPSEffects.Image:
                    mcSpriteBatch.DrawString(mcFont, "Rows:", new Vector2(GetTextSafeArea().Left + 260, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcImageParticleSystem.miNumberOfRows.ToString("0"), new Vector2(GetTextSafeArea().Left + 320, GetTextSafeArea().Top + 2), sValueColor);

                    mcSpriteBatch.DrawString(mcFont, "Columns:", new Vector2(GetTextSafeArea().Left + 350, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcImageParticleSystem.miNumberOfColumns.ToString("0"), new Vector2(GetTextSafeArea().Left + 435, GetTextSafeArea().Top + 2), sValueColor);

                    mcSpriteBatch.DrawString(mcFont, "Spin Mode:", new Vector2(GetTextSafeArea().Left + 5, GetTextSafeArea().Top + 450), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcImageParticleSystem.msSpinMode, new Vector2(GetTextSafeArea().Left + 105, GetTextSafeArea().Top + 450), sValueColor);

                    mcSpriteBatch.DrawString(mcFont, "Uniform:", new Vector2(GetTextSafeArea().Left + 170, GetTextSafeArea().Top + 450), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcImageParticleSystem.mbUniformSpin.ToString(), new Vector2(GetTextSafeArea().Left + 245, GetTextSafeArea().Top + 450), sValueColor);
                break;

                case EPSEffects.Sprite:
                    if (mcSpriteParticleSystem.Name.Equals("Sprite Force") || mcSpriteParticleSystem.Name.Equals("Sprite Cloud"))
                    {
                        mcSpriteBatch.DrawString(mcFont, "Force:", new Vector2(GetTextSafeArea().Left + 200, GetTextSafeArea().Top + 2), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, mcSpriteParticleSystem.AttractorMode.ToString(), new Vector2(GetTextSafeArea().Left + 260, GetTextSafeArea().Top + 2), sValueColor);

                        mcSpriteBatch.DrawString(mcFont, "Strength:", new Vector2(GetTextSafeArea().Left + 410, GetTextSafeArea().Top + 2), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, mcSpriteParticleSystem.AttractorStrength.ToString("0.0"), new Vector2(GetTextSafeArea().Left + 495, GetTextSafeArea().Top + 2), sValueColor);
                    }
                break;

                case EPSEffects.Magnets:
                    mcSpriteBatch.DrawString(mcFont, "Magnets Affect:", new Vector2(GetTextSafeArea().Left + 260, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcMagnetParticleSystem.mbMagnetsAffectPosition ? "Position" : "Velocity", new Vector2(GetTextSafeArea().Left + 410, GetTextSafeArea().Top + 2), sValueColor);
                break;

                case EPSEffects.ExplosionFireSmoke:
                    mcSpriteBatch.DrawString(mcFont, "Intensity:", new Vector2(GetTextSafeArea().Left + 330, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcExplosionFireSmokeParticleSystem.ExplosionIntensity.ToString(), new Vector2(GetTextSafeArea().Left + 410, GetTextSafeArea().Top + 2), sValueColor);

                    mcSpriteBatch.DrawString(mcFont, "Size:", new Vector2(GetTextSafeArea().Left + 450, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcExplosionFireSmokeParticleSystem.ExplosionParticleSize.ToString(), new Vector2(GetTextSafeArea().Left + 495, GetTextSafeArea().Top + 2), sValueColor);
                break;

                case EPSEffects.ExplosionFlash:
                    mcSpriteBatch.DrawString(mcFont, "Intensity:", new Vector2(GetTextSafeArea().Left + 330, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcExplosionFlashParticleSystem.ExplosionIntensity.ToString(), new Vector2(GetTextSafeArea().Left + 410, GetTextSafeArea().Top + 2), sValueColor);

                    mcSpriteBatch.DrawString(mcFont, "Size:", new Vector2(GetTextSafeArea().Left + 450, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcExplosionFlashParticleSystem.ExplosionParticleSize.ToString(), new Vector2(GetTextSafeArea().Left + 495, GetTextSafeArea().Top + 2), sValueColor);
                break;

                case EPSEffects.ExplosionFlyingSparks:
                    mcSpriteBatch.DrawString(mcFont, "Intensity:", new Vector2(GetTextSafeArea().Left + 330, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcExplosionFlyingSparksParticleSystem.ExplosionIntensity.ToString(), new Vector2(GetTextSafeArea().Left + 410, GetTextSafeArea().Top + 2), sValueColor);

                    mcSpriteBatch.DrawString(mcFont, "Size:", new Vector2(GetTextSafeArea().Left + 450, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcExplosionFlyingSparksParticleSystem.ExplosionParticleSize.ToString(), new Vector2(GetTextSafeArea().Left + 495, GetTextSafeArea().Top + 2), sValueColor);
                break;

                case EPSEffects.ExplosionSmokeTrails:
                    mcSpriteBatch.DrawString(mcFont, "Intensity:", new Vector2(GetTextSafeArea().Left + 330, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcExplosionSmokeTrailsParticleSystem.ExplosionIntensity.ToString(), new Vector2(GetTextSafeArea().Left + 410, GetTextSafeArea().Top + 2), sValueColor);

                    mcSpriteBatch.DrawString(mcFont, "Size:", new Vector2(GetTextSafeArea().Left + 450, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcExplosionSmokeTrailsParticleSystem.ExplosionParticleSize.ToString(), new Vector2(GetTextSafeArea().Left + 495, GetTextSafeArea().Top + 2), sValueColor);
                break;

                case EPSEffects.ExplosionRoundSparks:
                    mcSpriteBatch.DrawString(mcFont, "Intensity:", new Vector2(GetTextSafeArea().Left + 330, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcExplosionRoundSparksParticleSystem.ExplosionIntensity.ToString(), new Vector2(GetTextSafeArea().Left + 410, GetTextSafeArea().Top + 2), sValueColor);

                    mcSpriteBatch.DrawString(mcFont, "Size:", new Vector2(GetTextSafeArea().Left + 450, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcExplosionRoundSparksParticleSystem.ExplosionParticleSize.ToString(), new Vector2(GetTextSafeArea().Left + 495, GetTextSafeArea().Top + 2), sValueColor);
                break;

                case EPSEffects.ExplosionDebris:
                    mcSpriteBatch.DrawString(mcFont, "Intensity:", new Vector2(GetTextSafeArea().Left + 330, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcExplosionDebrisParticleSystem.ExplosionIntensity.ToString(), new Vector2(GetTextSafeArea().Left + 410, GetTextSafeArea().Top + 2), sValueColor);

                    mcSpriteBatch.DrawString(mcFont, "Size:", new Vector2(GetTextSafeArea().Left + 450, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcExplosionDebrisParticleSystem.ExplosionParticleSize.ToString(), new Vector2(GetTextSafeArea().Left + 495, GetTextSafeArea().Top + 2), sValueColor);
                break;

                case EPSEffects.ExplosionShockwave:
                    mcSpriteBatch.DrawString(mcFont, "Size:", new Vector2(GetTextSafeArea().Left + 450, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcExplosionShockwaveParticleSystem.ShockwaveSize.ToString(), new Vector2(GetTextSafeArea().Left + 495, GetTextSafeArea().Top + 2), sValueColor);
                break;

                case EPSEffects.Explosion:
                    mcSpriteBatch.DrawString(mcFont, "Size:", new Vector2(GetTextSafeArea().Left + 450, GetTextSafeArea().Top + 2), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, mcExplosionParticleSystem.ExplosionParticleSize.ToString(), new Vector2(GetTextSafeArea().Left + 495, GetTextSafeArea().Top + 2), sValueColor);
                break;
            }

            // If the Particle System is Paused
            if (mbPaused)
            {
                mcSpriteBatch.DrawString(mcFont, "Paused", new Vector2(GetTextSafeArea().Left + 350, GetTextSafeArea().Top + 25), sValueColor);
            }

            // If the Common Controls should be shown
            if (mbShowCommonControls)
            {
                mcSpriteBatch.DrawString(mcFont, "Change Particle System:", new Vector2(5, 25), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "G / H", new Vector2(235, 25), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Toggle Emitter On/Off:", new Vector2(5, 50), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "Delete", new Vector2(220, 50), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Increase/Decrease Emitter Speed:", new Vector2(5, 75), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "+ / -", new Vector2(320, 75), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Add Particle:", new Vector2(5, 100), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "Insert(one), Home(many), PgUp(max)", new Vector2(130, 100), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Move Emitter:", new Vector2(5, 125), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "A/D, W/S, Q/E", new Vector2(135, 125), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Rotate Emitter:", new Vector2(5, 150), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "J/L(yaw), I/Vertex(pitch), U/O(roll)", new Vector2(150, 150), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Rotate Emitter Around Pivot:", new Vector2(5, 175), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "Y + Rotate Emitter", new Vector2(275, 175), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Reset Emitter's Position and Orientation:", new Vector2(5, 200), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "Z", new Vector2(375, 200), sControlColor);

                
                mcSpriteBatch.DrawString(mcFont, "Toggle Floor:", new Vector2(485, 25), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "F", new Vector2(610, 25), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Toggle Axis:", new Vector2(650, 25), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "F7", new Vector2(770, 25), sControlColor); 

                mcSpriteBatch.DrawString(mcFont, "Toggle Full Screen:", new Vector2(485, 50), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "End", new Vector2(665, 50), sControlColor);      

                mcSpriteBatch.DrawString(mcFont, "Toggle Camera Mode:", new Vector2(485, 75), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "PgDown", new Vector2(690, 75), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Reset Camera Position:", new Vector2(485, 100), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "R", new Vector2(705, 100), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Change Texture:", new Vector2(485, 125), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "T / Shift + T", new Vector2(640, 125), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Pause Particle System:", new Vector2(485, 150), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "Spacebar", new Vector2(700, 150), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Speed Up/Down PS:", new Vector2(485, 175), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "* / /", new Vector2(680, 175), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Draw Static Particles:", new Vector2(485, 200), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "F6", new Vector2(690, 200), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Clear Screen Each Frame:", new Vector2(485, 225), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "F5", new Vector2(730, 225), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Create Animation Images:", new Vector2(485, 250), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "F8", new Vector2(725, 250), sControlColor);

                mcSpriteBatch.DrawString(mcFont, "Serialize Particle System:", new Vector2(485, 275), sPropertyColor);
                mcSpriteBatch.DrawString(mcFont, "F9", new Vector2(725, 275), sControlColor);
            }

            // If the Particle System specific Controls should be shown
            if (mbShowPSControls)
            {
                // Display particle system specific controls
                switch (meCurrentPS)
                {
                    default: break;

                    case EPSEffects.Random:
                        mcSpriteBatch.DrawString(mcFont, "Random Pattern:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(160, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Spiral Pattern:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(140, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Size:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(135, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Decrease Size:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(145, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Change Start Color:", new Vector2(5, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(190, 350), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Change End Color:", new Vector2(5, 375), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "M", new Vector2(180, 375), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Tube Mode:", new Vector2(5, 400), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "P", new Vector2(115, 400), sControlColor);
                    break;

                    case EPSEffects.Fire:
                        mcSpriteBatch.DrawString(mcFont, "Vertical Ring:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(130, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Horizontal Ring:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(150, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Decrease Smoke:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(170, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Smoke:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(160, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Toggle Additive Blending:", new Vector2(5, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(240, 350), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Toggle Ring Movement:", new Vector2(5, 375), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "M", new Vector2(225, 375), sControlColor);
                    break;

                    case EPSEffects.Smoke:
                        mcSpriteBatch.DrawString(mcFont, "Rising Smoke Cloud:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(195, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Dispersed Smoke:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(175, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Suction Orb:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(120, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Repel Orb:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(105, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Change Color:", new Vector2(5, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(135, 350), sControlColor);
                    break;

                    case EPSEffects.Snow:
                        mcSpriteBatch.DrawString(mcFont, "Apply Wind Force:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(180, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Remove Wind Force:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(200, 275), sControlColor);
                    break;

                    case EPSEffects.SquarePattern:
                        mcSpriteBatch.DrawString(mcFont, "Square Pattern:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(150, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Multiple Color Changes:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(220, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Change Color:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(140, 300), sControlColor);
                    break;

                    case EPSEffects.Fountain:
                        mcSpriteBatch.DrawString(mcFont, "Floor Collision On:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(170, 250), sControlColor);
                        
                        mcSpriteBatch.DrawString(mcFont, "Floor Collision Off:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(180, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Decrease Bounciness:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(205, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Bounciness:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(195, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Shrinking On:", new Vector2(5, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(130, 350), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Shrinking Off:", new Vector2(5, 375), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "M", new Vector2(135, 375), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Toggle Additive Blending:", new Vector2(5, 400), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "P", new Vector2(240, 400), sControlColor);
                    break;

                    case EPSEffects.Random2D:
                        mcSpriteBatch.DrawString(mcFont, "Straight:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(80, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Random Direction Changes:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(255, 275), sControlColor);
                    break;

                    case EPSEffects.GasFall:
                        mcSpriteBatch.DrawString(mcFont, "Wall:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(50, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Split:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(50, 275), sControlColor);
                    break;

                    case EPSEffects.Dot: break;

                    case EPSEffects.Fireworks:
                        mcSpriteBatch.DrawString(mcFont, "Common Origin:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(150, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Spread Out Origin:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(180, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Explosions On:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(140, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Explosions Off:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(145, 325), sControlColor);
                    break;

                    case EPSEffects.Figure8: break;

                    case EPSEffects.Star:
                        mcSpriteBatch.DrawString(mcFont, "2D Star:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(85, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "3D Star:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(85, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Hold and Rotate Emitter:", new Vector2(5, 300), sPropertyColor);

                        mcSpriteBatch.DrawString(mcFont, "Adjust Rotational Velocity:", new Vector2(15, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(260, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Adjust Rotational Acceleration:", new Vector2(15, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(300, 350), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Wiggle Mode:", new Vector2(15, 375), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "P", new Vector2(140, 375), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Reset Rotational Forces:", new Vector2(5, 400), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(230, 400), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Highlight Axis:", new Vector2(5, 425), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "M", new Vector2(135, 425), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Toggle Emitter Intermittance:", new Vector2(5, 450), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "[", new Vector2(270, 450), sControlColor);
                    break;

                    case EPSEffects.Ball:
                        mcSpriteBatch.DrawString(mcFont, "Increase Radius:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(155, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Decrease Radius:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(165, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Hold and Rotate Emitter:", new Vector2(5, 300), sPropertyColor);

                        mcSpriteBatch.DrawString(mcFont, "Adjust Rotational Velocity:", new Vector2(15, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(260, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Adjust Rotational Acceleration:", new Vector2(15, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(300, 350), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Reset Rotational Forces:", new Vector2(5, 375), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(230, 375), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Less Particles:", new Vector2(5, 400), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "P", new Vector2(140, 400), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "More Particles:", new Vector2(5, 425), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "[", new Vector2(140, 425), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Rebuild Ball:", new Vector2(5, 450), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "M", new Vector2(120, 450), sControlColor);
                    break;

                    case EPSEffects.RotatingQuad:
                        mcSpriteBatch.DrawString(mcFont, "Normal:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(75, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Billboards:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(100, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Ball:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(45, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Number of Particles:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(195, 325), sControlColor);
                    break;

                    case EPSEffects.Box:
                        mcSpriteBatch.DrawString(mcFont, "Box:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(50, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Bars:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(50, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Toggle Change Colors:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(210, 300), sControlColor);
                    break;

                    case EPSEffects.Image:
                        mcSpriteBatch.DrawString(mcFont, "Image:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(65, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Spiral:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(60, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Vortex:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(75, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Toggle Spin Mode:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(175, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Toggle Uniform Spin:", new Vector2(5, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(195, 350), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Scatter Image:", new Vector2(5, 375), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "M", new Vector2(140, 375), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Toggle Rows:", new Vector2(5, 400), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "P", new Vector2(130, 400), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Toggle Columns:", new Vector2(5, 425), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "[", new Vector2(155, 425), sControlColor);
                    break;

                    case EPSEffects.AnimatedTexturedQuad: break;

                    case EPSEffects.Sprite:
                        mcSpriteBatch.DrawString(mcFont, "Mouse Attraction:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(167, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Mouse Cloud:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(125, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Grid:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(50, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Rotators:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(87, 325), sControlColor);

                        if (mcSpriteParticleSystem.Name.Equals("Sprite Force") || mcSpriteParticleSystem.Name.Equals("Sprite Cloud"))
                        {
                            mcSpriteBatch.DrawString(mcFont, "Toggle Force:", new Vector2(5, 350), sPropertyColor);
                            mcSpriteBatch.DrawString(mcFont, "Left Mouse Button", new Vector2(130, 350), sControlColor);

                            mcSpriteBatch.DrawString(mcFont, "Toggle Strength:", new Vector2(5, 375), sPropertyColor);
                            mcSpriteBatch.DrawString(mcFont, "Right Mouse Button", new Vector2(155, 375), sControlColor);
                        }
                    break;

                    case EPSEffects.AnimatedSprite:
                        mcSpriteBatch.DrawString(mcFont, "Explosion:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(95, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Butterfly:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(90, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Toggle Color Mode:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "Left Mouse Button", new Vector2(183, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Add Particle:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "Right Mouse Button", new Vector2(125, 325), sControlColor);
                    break;

                    case EPSEffects.Pixel:
                        mcSpriteBatch.DrawString(mcFont, "Spray:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(65, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Wall:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(55, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Toggle Gravity:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(150, 300), sControlColor);
                    break;

                    case EPSEffects.Magnets:
                        mcSpriteBatch.DrawString(mcFont, "Emitter Magnet:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(150, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Multiple Magnets:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(160, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Toggle Affecting Position vs Velocity:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(355, 300), sControlColor);
                    break;

                    case EPSEffects.Sparkler:
                        mcSpriteBatch.DrawString(mcFont, "Simple:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(70, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Complex:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(90, 275), sControlColor);
                    break;

                    case EPSEffects.Grid: break;

                    case EPSEffects.Sphere:
                        mcSpriteBatch.DrawString(mcFont, "Decrease Radius:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(165, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Radius:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(155, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Same Direction:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(150, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Random Direction:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(170, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Less Particles:", new Vector2(5, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(140, 350), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "More Particles:", new Vector2(5, 375), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "M", new Vector2(140, 375), sControlColor);
                    break;

                    case EPSEffects.MultipleParticleImages: break;

                    case EPSEffects.ExplosionFireSmoke:
                        mcSpriteBatch.DrawString(mcFont, "Decrease Intensity:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(180, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Intensity:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(170, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Change Color:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(135, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Decrease Particle Size:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(220, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Particle Size:", new Vector2(5, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(210, 350), sControlColor);
                    break;

                    case EPSEffects.ExplosionFlash:
                        mcSpriteBatch.DrawString(mcFont, "Decrease Intensity:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(180, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Intensity:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(170, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Change Color:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(135, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Decrease Particle Size:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(220, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Particle Size:", new Vector2(5, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(210, 350), sControlColor);
                    break;

                    case EPSEffects.ExplosionFlyingSparks:
                        mcSpriteBatch.DrawString(mcFont, "Decrease Intensity:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(180, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Intensity:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(170, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Change Color:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(135, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Decrease Particle Size:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(220, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Particle Size:", new Vector2(5, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(210, 350), sControlColor);
                    break;

                    case EPSEffects.ExplosionSmokeTrails:
                        mcSpriteBatch.DrawString(mcFont, "Decrease Intensity:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(180, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Intensity:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(170, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Change Color:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(135, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Decrease Particle Size:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(220, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Particle Size:", new Vector2(5, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(210, 350), sControlColor);
                    break;

                    case EPSEffects.ExplosionRoundSparks:
                        mcSpriteBatch.DrawString(mcFont, "Decrease Intensity:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(180, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Intensity:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(170, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Change Color:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(135, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Decrease Particle Size:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(220, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Particle Size:", new Vector2(5, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(210, 350), sControlColor);
                    break;

                    case EPSEffects.ExplosionDebris:
                        mcSpriteBatch.DrawString(mcFont, "Decrease Intensity:", new Vector2(5, 250), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "X", new Vector2(180, 250), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Intensity:", new Vector2(5, 275), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "C", new Vector2(170, 275), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Change Color:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(135, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Decrease Particle Size:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(220, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Particle Size:", new Vector2(5, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(210, 350), sControlColor);
                    break;

                    case EPSEffects.ExplosionShockwave:
                        mcSpriteBatch.DrawString(mcFont, "Change Color:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(135, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Decrease Particle Size:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(220, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Particle Size:", new Vector2(5, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(210, 350), sControlColor);
                    break;

                    case EPSEffects.Explosion:
                        mcSpriteBatch.DrawString(mcFont, "Change Color:", new Vector2(5, 300), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "V", new Vector2(135, 300), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Decrease Particle Size:", new Vector2(5, 325), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "B", new Vector2(220, 325), sControlColor);

                        mcSpriteBatch.DrawString(mcFont, "Increase Particle Size:", new Vector2(5, 350), sPropertyColor);
                        mcSpriteBatch.DrawString(mcFont, "N", new Vector2(210, 350), sControlColor);
                    break;

                    case EPSEffects.PixelParticleSystemTemplate: break;

                    case EPSEffects.SpriteParticleSystemTemplate: break;
                        
                    case EPSEffects.PointSpriteParticleSystemTemplate: break;

                    case EPSEffects.QuadParticleSystemTemplate: break;

                    case EPSEffects.TexturedQuadParticleSystemTemplate: break;

                    case EPSEffects.DefaultPixelParticleSystemTemplate: break;

                    case EPSEffects.DefaultSpriteParticleSystemTemplate: break;

                    case EPSEffects.DefaultPointSpriteParticleSystemTemplate: break;

                    case EPSEffects.DefaultQuadParticleSystemTemplate: break;

                    case EPSEffects.DefaultTexturedQuadParticleSystemTemplate: break;
                }
            }

            // If the Camera Controls should be shown
            if (mbShowCameraControls)
            {
                // If we are using a Fixed Camera
                if (msCamera.bUsingFixedCamera)
                {
                    mcSpriteBatch.DrawString(mcFont, "Fixed Camera Controls:", new Vector2(5, mcGraphics.PreferredBackBufferHeight - 125), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, "Keys: Left/Right Arrows, Up/Down Arrows, Num0/Num1", new Vector2(15, mcGraphics.PreferredBackBufferHeight - 100), sControlColor);
                    mcSpriteBatch.DrawString(mcFont, "Mouse: Left Button + X/Y Movement, Right Button + Y Movement", new Vector2(15, mcGraphics.PreferredBackBufferHeight - 75), sControlColor);
                }
                // Else we are using a Free Camera
                else
                {
                    mcSpriteBatch.DrawString(mcFont, "Free Camera Controls", new Vector2(5, mcGraphics.PreferredBackBufferHeight - 125), sPropertyColor);
                    mcSpriteBatch.DrawString(mcFont, "Keys: Left/Right Arrows, Up/Down Arrows, Num0/Num1, Num4/Num6, Num8/Num2", new Vector2(15, mcGraphics.PreferredBackBufferHeight - 100), sControlColor);
                    mcSpriteBatch.DrawString(mcFont, "Mouse: Left Button + X/Y Movement, Right Button + X/Y Movement, Scroll Wheel", new Vector2(15, mcGraphics.PreferredBackBufferHeight - 75), sControlColor);
                }
            }

			// If we should draw the number of bytes allocated in memory
			if (mbDRAW_GARBAGE_COLLECTOR_MEMORY_ALLOCATED)
				mcSpriteBatch.DrawString(mcFont, "GC KB Allocated: " + (GC.GetTotalMemory(false) / 1024.0f).ToString("0.0"), new Vector2(550, mcGraphics.PreferredBackBufferHeight - 125), sPropertyColor);

            // Stop drawing text
            mcSpriteBatch.End();
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
            Rectangle rTextSafeArea = new Rectangle(mcGraphics.GraphicsDevice.Viewport.X, mcGraphics.GraphicsDevice.Viewport.Y,
                                            mcGraphics.GraphicsDevice.Viewport.Width, mcGraphics.GraphicsDevice.Viewport.Height);
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
            GraphicsDevice cGraphicsDevice = mcGraphics.GraphicsDevice;

            cGraphicsDevice.RenderState.AlphaBlendEnable = false;
            cGraphicsDevice.RenderState.AlphaTestEnable = false;
            cGraphicsDevice.RenderState.DepthBufferEnable = true;

            cGraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            cGraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

            // If the Floor should be drawn
            if (mbShowFloor)
            {
                foreach (ModelMesh mesh in mcFloorModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = cWorldMatrix;
                        effect.View = cViewMatrix;
                        effect.Projection = cProjectionMatrix;
                    }

                    mesh.Draw();
                }
            }

            // If the Sphere should be visible
            if (mcSphere.bVisible)
            {
                foreach (ModelMesh mesh in mcSphereModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = Matrix.CreateScale(mcSphere.fSize) * Matrix.CreateTranslation(mcSphere.sPosition);
                        effect.View = cViewMatrix;
                        effect.Projection = cProjectionMatrix;
                    }

                    mesh.Draw();
                }
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
            GraphicsDevice.VertexDeclaration = mcAxisVertexDeclaration;

            // Draw the lines
            mcAxisEffect.Begin();
            foreach (EffectPass cPass in mcAxisEffect.CurrentTechnique.Passes)
            {
                cPass.Begin();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, msaAxisDirectionVertices, 0, 3);
                cPass.End();
            }
            mcAxisEffect.End();
        }


        #endregion

        #region Handle Input

        /// <summary>
        /// Returns true if the Key is being pressed down
        /// </summary>
        /// <param name="cKey">The Key to check</param>
        /// <returns>Returns true if the Key is being pressed down</returns>
        bool KeyIsDown(Keys cKey)
        {
            return mcCurrentKeyboardState.IsKeyDown(cKey);
        }

        /// <summary>
        /// Returns true if the Key is being pressed down, and no other input was received in the 
        /// last TimeInSeconds seconds
        /// </summary>
        /// <param name="cKey">The Key to check</param>
        /// <param name="fTimeInSeconds">The amount of time in seconds that must have passed since the 
        /// last input for this key to be considered pressed down</param>
        /// <returns>Returns true if the Key is being pressed down, and no other input was received in
        /// the last TimeInSeconds seconds</returns>
        bool KeyIsDown(Keys cKey, float fTimeInSeconds)
        {
            // If the Key is being pressed down
            if (KeyIsDown(cKey))
            {
                // If the specified Time In Seconds has passed since any input was recieved
                if (mcInputTimeSpan.TotalSeconds >= fTimeInSeconds)
                {
                    // Reset the Input Timer
                    mcInputTimeSpan = TimeSpan.Zero;

                    // Rerun that the specified amount of Time has elapsed since the last input was received
                    return true;
                }
            }

            // Return that the key is not being pressed, or that a key was hit sooner than 
            // the specified Time In Seconds
            return false;
        }

        /// <summary>
        /// Returns true if the Key is not pressed down
        /// </summary>
        /// <param name="cKey">The Key to check</param>
        /// <returns>Returns true if the Key is not being pressed down</returns>
        bool KeyIsUp(Keys cKey)
        {
            return mcCurrentKeyboardState.IsKeyUp(cKey);
        }

        /// <summary>
        /// Returns true if the Key was just pressed down
        /// </summary>
        /// <param name="cKey">The Key to check</param>
        /// <returns>Returns true if the Key is being pressed now, but was not being pressed last frame</returns>
        bool KeyWasJustPressed(Keys cKey)
        {
            return (mcCurrentKeyboardState.IsKeyDown(cKey) && !mcPreviousKeyboardState.IsKeyDown(cKey));
        }

        /// <summary>
        /// Returns true if the Key was just released
        /// </summary>
        /// <param name="cKey">The Key to check</param>
        /// <returns>Returns true if the Key is not being pressed now, but was being pressed last frame</returns>
        bool KeyWasJustReleased(Keys cKey)
        {
            return (mcCurrentKeyboardState.IsKeyUp(cKey) && !mcPreviousKeyboardState.IsKeyUp(cKey));
        }

        /// <summary>
        /// Returns true if the GamePad Button is down
        /// </summary>
        /// <param name="cButton">The Button to check</param>
        /// <returns>Returns true if the GamePad Button is down, false if not</returns>
        bool ButtonIsDown(Buttons cButton)
        {
            return mcCurrentGamePadState.IsButtonDown(cButton);
        }

        /// <summary>
        /// Returns true if the Button is being pressed down, and no other input was received in the
        /// last TimeInSeconds seconds
        /// </summary>
        /// <param name="cButton">The Button to check</param>
        /// <param name="fTimeInSeconds">The amount of time in seconds that must have passed since the
        /// last input for the Button to be considered pressed down</param>
        /// <returns>Returns true if the Button is being pressed down, and no other input was received in the
        /// last TimeInSeconds seconds</returns>
        bool ButtonIsDown(Buttons cButton, float fTimeInSeconds)
        {
            // If the Button is being pressed down
            if (ButtonIsDown(cButton))
            {
                // If the specified Time In Seconds has passed since any input was recieved
                if (mcInputTimeSpan.TotalSeconds >= fTimeInSeconds)
                {
                    // Reset the Input Timer
                    mcInputTimeSpan = TimeSpan.Zero;

                    // Rerun that the specified amount of Time has elapsed since the last input was received
                    return true;
                }
            }

            // Return that the Button is not being pressed, or that a Button was hit sooner than 
            // the specified Time In Seconds
            return false;
        }

        /// <summary>
        /// Returns true if the GamePad Button is up
        /// </summary>
        /// <param name="cButton">The Button to check</param>
        /// <returns>Returns true if the GamePad Button is up, false if not</returns>
        bool ButtonIsUp(Buttons cButton)
        {
            return mcCurrentGamePadState.IsButtonUp(cButton);
        }

        /// <summary>
        /// Returns true if the GamePad Button was just pressed
        /// </summary>
        /// <param name="cButton">The Button to check</param>
        /// <returns>Returns true if the GamePad Button was just pressed, false if not</returns>
        bool ButtonWasJustPressed(Buttons cButton)
        {
            return (mcCurrentGamePadState.IsButtonDown(cButton) && !mcPreviousGamePadState.IsButtonDown(cButton));
        }

        /// <summary>
        /// Returns true if the GamePad Button was just released
        /// </summary>
        /// <param name="cButton">The Button to check</param>
        /// <returns>Returns true if the GamePad Button was just released, false if not</returns>
        bool ButtonWasJustReleased(Buttons cButton)
        {
            return (mcCurrentGamePadState.IsButtonUp(cButton) && !mcPreviousGamePadState.IsButtonUp(cButton));
        }

        /// <summary>
        /// Gets and processes all user input
        /// </summary>
        void ProcessInput(GameTime cGameTime)
        {
            // Save how long it's been since the last time Input was Handled
            float fTimeInSeconds = (float)cGameTime.ElapsedGameTime.TotalSeconds;

            // Add how long it's been since the last user input was received
            mcInputTimeSpan += TimeSpan.FromSeconds(fTimeInSeconds);

            // Save the Keyboard State and get its new State
            mcPreviousKeyboardState = mcCurrentKeyboardState;
            mcCurrentKeyboardState = Keyboard.GetState();

            // Save the Mouse State and get its new State
            mcPreviousMouseState = mcCurrentMouseState;
            mcCurrentMouseState = Mouse.GetState();

            // Save the GamePad State and get its new State
            mcPreviousGamePadState = mcCurrentGamePadState;
            mcCurrentGamePadState = GamePad.GetState(PlayerIndex.One);

            // If we should Exit
            if (KeyIsDown(Keys.Escape) || ButtonIsDown(Buttons.Back))
            {
                Exit();
            }

            // If we should toggle between Full Screen and Windowed mode
            if (KeyWasJustPressed(Keys.End))
            {
                mcGraphics.ToggleFullScreen();
            }

            // If we should toggle showing the Common Controls
            if (KeyWasJustPressed(Keys.F1))
            {
                mbShowCommonControls = !mbShowCommonControls;
            }

            // If we should toggle showing the Particle System specific Controls
            if (KeyWasJustPressed(Keys.F2))
            {
                mbShowPSControls = !mbShowPSControls;
            }

            // If we should toggle showing the Camera Controls
            if (KeyWasJustPressed(Keys.F3))
            {
                mbShowCameraControls = !mbShowCameraControls;
            }

            // If we should toggle showing the Common Controls
            if (KeyWasJustPressed(Keys.F4))
            {
                mbShowText = !mbShowText;
            }

            // If we should toggle showing the Floor
            if (KeyWasJustPressed(Keys.F))
            {
                mbShowFloor = !mbShowFloor;
            }

            // If we should toggle Pausing the game
            if (KeyWasJustPressed(Keys.Space) || ButtonWasJustPressed(Buttons.Start))
            {
                mbPaused = !mbPaused;
            }

            // If we should toggle Clearing the Screen each Frame
            if (KeyWasJustPressed(Keys.F5))
            {
                mbClearScreenEveryFrame = !mbClearScreenEveryFrame;
            }

            // If the particle lifetimes should be drawn in one frame
            if (KeyWasJustPressed(Keys.F6))
            {
                mbDrawStaticPS = !mbDrawStaticPS;
                mbStaticParticlesDrawn = false;
            }

            // If the Axis should be toggled on/off
            if (KeyWasJustPressed(Keys.F7))
            {
                mbShowAxis = !mbShowAxis;
            }
#if (!XBOX)
            // If the PS should be drawn to files
            if (KeyWasJustPressed(Keys.F8))
            {
                // Draw the Particle System Animation to a series of Image Files
                mcParticleSystemManager.DrawAllParticleSystemsAnimationToFiles(GraphicsDevice, miDrawPSToFilesImageWidth, miDrawPSToFilesImageHeight, 
                            msDrawPSToFilesDirectoryName, mfDrawPSToFilesTotalTime, mfDrawPSToFilesTimeStep, meDrawPSToFilesImageFormat, mbCreateAnimatedGIF, mbCreateTileSetImage);
            }

            // If the PS should be serialized to a file
            if (KeyWasJustPressed(Keys.F9))
            {
                // Only particle systems that do not inherit the DrawableGameComponent can be serialized.
                if (!DPSFHelper.DPSFInheritsDrawableGameComponent)
                {
                    // If we have the particle system right now
                    if (mcCurrentParticleSystem != null)
                    {
                        // Serialize the particle system into a file
                        System.IO.Stream stream = System.IO.File.Open("SerializedParticleSystem.dat", System.IO.FileMode.Create);
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        formatter.Serialize(stream, mcCurrentParticleSystem);
                        stream.Close();

                        // Remove the particle system from the manager and destroy the particle system in memory
                        mcParticleSystemManager.RemoveParticleSystem(mcCurrentParticleSystem);
                        mcCurrentParticleSystem.Destroy();
                        mcCurrentParticleSystem = null;
                    }
                    // Else we don't have the particle system right now
                    else
                    {
                        // Deserialize the particle system from a file
                        System.IO.Stream stream = System.IO.File.Open("SerializedParticleSystem.dat", System.IO.FileMode.Open);
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        mcCurrentParticleSystem = (IDPSFParticleSystem)formatter.Deserialize(stream);
                        stream.Close();

                        try
                        {
                            // Setup the particle system properties that couldn't be serialized
                            mcCurrentParticleSystem.InitializeNonSerializableProperties(this, this.GraphicsDevice, this.Content);
                        }
                        // Catch the case where the Particle System requires a texture, but one wasn't loaded
                        catch (ArgumentNullException)
                        {
                            // Assign the particle system a texture to use
                            mcCurrentParticleSystem.SetTexture("Textures/Bubble");
                        }

                        // Readd the particle system to the particle system manager
                        mcParticleSystemManager.AddParticleSystem(mcCurrentParticleSystem);
                    }
                }
            }
#endif
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
                if (KeyIsDown(Keys.NumPad1) ||
                    (ButtonIsDown(Buttons.LeftThumbstickUp) && ButtonIsDown(Buttons.LeftStick)))
                {
                    msCamera.fCameraArc -= fTimeInSeconds * 25;
                }

                if (KeyIsDown(Keys.NumPad0) ||
                    (ButtonIsDown(Buttons.LeftThumbstickDown) && ButtonIsDown(Buttons.LeftStick)))
                {
                    msCamera.fCameraArc += fTimeInSeconds * 25;
                }

                // If the Camera should rotate horizontally
                if (KeyIsDown(Keys.Right) || ButtonIsDown(Buttons.LeftThumbstickRight))
                {
                    msCamera.fCameraRotation -= fTimeInSeconds * 50;
                }

                if (KeyIsDown(Keys.Left) || ButtonIsDown(Buttons.LeftThumbstickLeft))
                {
                    msCamera.fCameraRotation += fTimeInSeconds * 50;
                }

                // If Camera should be zoomed out
                if (KeyIsDown(Keys.Down) || 
                    (ButtonIsDown(Buttons.LeftThumbstickDown) && !ButtonIsDown(Buttons.LeftStick)))
                {
                    msCamera.fCameraDistance += fTimeInSeconds * 250;
                }

                // If Camera should be zoomed in
                if (KeyIsDown(Keys.Up) || 
                    (ButtonIsDown(Buttons.LeftThumbstickUp) && !ButtonIsDown(Buttons.LeftStick)))
                {
                    msCamera.fCameraDistance -= fTimeInSeconds * 250;
                }


                // Calculate how much the Mouse was moved
                int iXMovement = mcCurrentMouseState.X - mcPreviousMouseState.X;
                int iYMovement = mcCurrentMouseState.Y - mcPreviousMouseState.Y;
                int iZMovement = mcCurrentMouseState.ScrollWheelValue - mcPreviousMouseState.ScrollWheelValue;

                const float fMOUSE_MOVEMENT_SPEED = 0.5f;
                const float fMOUSE_ROTATION_SPEED = 0.5f;

                // If the Left Mouse Button is pressed
                if (mcCurrentMouseState.LeftButton == ButtonState.Pressed)
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
                if (mcCurrentMouseState.RightButton == ButtonState.Pressed)
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

                if (KeyIsDown(Keys.Decimal))
                {
                    iSPEED = 100;
                }

                // If the Camera should move forward
                if (KeyIsDown(Keys.Up))
                {
                    msCamera.MoveCameraForwardOrBackward(iSPEED * fTimeInSeconds);
                }

                // If the Camera should move backwards
                if (KeyIsDown(Keys.Down))
                {
                    msCamera.MoveCameraForwardOrBackward(-iSPEED * fTimeInSeconds);
                }

                // If the Camera should strafe right
                if (KeyIsDown(Keys.Right))
                {
                    msCamera.MoveCameraHorizontally(-iSPEED * fTimeInSeconds);
                }

                // If the Camera should strafe left
                if (KeyIsDown(Keys.Left))
                {
                    msCamera.MoveCameraHorizontally(iSPEED * fTimeInSeconds);
                }

                // If the Camera should move upwards
                if (KeyIsDown(Keys.NumPad1))
                {
                    msCamera.MoveCameraVertically((iSPEED / 2) * fTimeInSeconds);
                }

                // If the Camera should move downwards
                if (KeyIsDown(Keys.NumPad0))
                {
                    msCamera.MoveCameraVertically((-iSPEED / 2) * fTimeInSeconds);
                }

                // If the Camera should yaw left
                if (KeyIsDown(Keys.NumPad4))
                {
                    msCamera.RotateCameraHorizontally(fROTATE_SPEED * fTimeInSeconds);
                }

                // If the Camera should yaw right
                if (KeyIsDown(Keys.NumPad6))
                {
                    msCamera.RotateCameraHorizontally(-fROTATE_SPEED * fTimeInSeconds);
                }

                // If the Camera should pitch up
                if (KeyIsDown(Keys.NumPad8))
                {
                    msCamera.RotateCameraVertically(-fROTATE_SPEED * fTimeInSeconds);
                }

                // If the Camera should pitch down
                if (KeyIsDown(Keys.NumPad2))
                {
                    msCamera.RotateCameraVertically(fROTATE_SPEED * fTimeInSeconds);
                }


                // Calculate how much the Mouse was moved
                int iXMovement = mcCurrentMouseState.X - mcPreviousMouseState.X;
                int iYMovement = mcCurrentMouseState.Y - mcPreviousMouseState.Y;
                int iZMovement = mcCurrentMouseState.ScrollWheelValue - mcPreviousMouseState.ScrollWheelValue;

                const float fMOUSE_MOVEMENT_SPEED = 0.5f;
                const float fMOUSE_ROTATION_SPEED = 0.005f;

                // If the Left Mouse Button is pressed
                if (mcCurrentMouseState.LeftButton == ButtonState.Pressed)
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
                if (mcCurrentMouseState.RightButton == ButtonState.Pressed)
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
            if (KeyWasJustPressed(Keys.PageDown))
            {
                msCamera.bUsingFixedCamera = !msCamera.bUsingFixedCamera;
            }

            // If the Camera values should be Reset
            if (KeyIsDown(Keys.R) || 
                (ButtonIsDown(Buttons.LeftShoulder) && ButtonIsDown(Buttons.RightShoulder)))
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
            if (KeyWasJustPressed(Keys.H) || ButtonWasJustPressed(Buttons.RightTrigger))
            {
                meCurrentPS++;
                if (meCurrentPS > EPSEffects.LastInList)
                {
                    meCurrentPS = 0;
                }

                // Initialize the new Particle System
                InitializeCurrentParticleSystem();
            }
            // Else if the Current Particle System should be changed back to the previous Particle System
            else if (KeyWasJustPressed(Keys.G) || ButtonWasJustPressed(Buttons.LeftTrigger))
            {
                meCurrentPS--;
                if (meCurrentPS < 0)
                {
                    meCurrentPS = EPSEffects.LastInList;
                }

                // Initialize the new Particle System
                InitializeCurrentParticleSystem();
            }

            // If the Current Particle System is not Initialized
            if (mcCurrentParticleSystem == null || !mcCurrentParticleSystem.IsInitialized)
            {
                return;
            }

            // Define how fast the user can move and rotate the Emitter
            float fEmitterMoveDelta = 75 * fElapsedTimeInSeconds;
            float fEmitterRotateDelta = MathHelper.Pi * fElapsedTimeInSeconds;

            // Check if the Emitter should be moved
            if (KeyIsDown(Keys.W) || 
                (ButtonIsDown(Buttons.DPadUp) && !ButtonIsDown(Buttons.RightStick)))
            {
                mcCurrentParticleSystem.Emitter.PositionData.Position += Vector3.Up * fEmitterMoveDelta;
            }

            if (KeyIsDown(Keys.S) || 
                (ButtonIsDown(Buttons.DPadDown) && !ButtonIsDown(Buttons.RightStick)))
            {
                mcCurrentParticleSystem.Emitter.PositionData.Position += Vector3.Down * fEmitterMoveDelta;
            }

            if (KeyIsDown(Keys.A) || ButtonIsDown(Buttons.DPadLeft))
            {
                mcCurrentParticleSystem.Emitter.PositionData.Position += Vector3.Left * fEmitterMoveDelta;
            }

            if (KeyIsDown(Keys.D) || ButtonIsDown(Buttons.DPadRight))
            {
                mcCurrentParticleSystem.Emitter.PositionData.Position += Vector3.Right * fEmitterMoveDelta;
            }

            if (KeyIsDown(Keys.E) || 
                (ButtonIsDown(Buttons.DPadUp) && ButtonIsDown(Buttons.RightStick)))
            {
                mcCurrentParticleSystem.Emitter.PositionData.Position += Vector3.Forward * fEmitterMoveDelta;
            }

            if (KeyIsDown(Keys.Q) ||
                (ButtonIsDown(Buttons.DPadDown) && ButtonIsDown(Buttons.RightStick)))
            {
                mcCurrentParticleSystem.Emitter.PositionData.Position += Vector3.Backward * fEmitterMoveDelta;
            }

            // Check if the Emitter should be rotated
            if ((meCurrentPS != EPSEffects.Star && meCurrentPS != EPSEffects.Ball) || 
                (!KeyIsDown(Keys.V) && !KeyIsDown(Keys.B) && !KeyIsDown(Keys.P)))
            {
                if (KeyIsDown(Keys.J) || 
                    (ButtonIsDown(Buttons.RightThumbstickLeft) && !ButtonIsDown(Buttons.RightStick)))
                {
                    // If we should Rotate the Emitter around the Pivot Point
                    if (KeyIsDown(Keys.Y))
                    {
                        mcCurrentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(-fEmitterRotateDelta, 0.0f, 0.0f));
                    }
                    // Else we should just Rotate the Emitter about its center
                    else
                    {
                        mcCurrentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(-fEmitterRotateDelta, 0.0f, 0.0f));
                    }
                }

                if (KeyIsDown(Keys.L) || 
                    (ButtonIsDown(Buttons.RightThumbstickRight) && !ButtonIsDown(Buttons.RightStick)))
                {
                    // If we should Rotate the Emitter around the Pivot Point
                    if (KeyIsDown(Keys.Y))
                    {
                        mcCurrentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(fEmitterRotateDelta, 0.0f, 0.0f));
                    }
                    // Else we should just Rotate the Emitter about its center
                    else
                    {
                        mcCurrentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(fEmitterRotateDelta, 0.0f, 0.0f));
                    }
                }

                if (KeyIsDown(Keys.I) || ButtonIsDown(Buttons.RightThumbstickUp))
                {
                    // If we should Rotate the Emitter around the Pivot Point
                    if (KeyIsDown(Keys.Y))
                    {
                        mcCurrentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, -fEmitterRotateDelta, 0.0f));
                    }
                    // Else we should just Rotate the Emitter about its center
                    else
                    {
                        mcCurrentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, -fEmitterRotateDelta, 0.0f));
                    }
                }

                if (KeyIsDown(Keys.K) || ButtonIsDown(Buttons.RightThumbstickDown))
                {
                    // If we should Rotate the Emitter around the Pivot Point
                    if (KeyIsDown(Keys.Y))
                    {
                        mcCurrentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, fEmitterRotateDelta, 0.0f));
                    }
                    // Else we should just Rotate the Emitter about its center
                    else
                    {
                        mcCurrentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, fEmitterRotateDelta, 0.0f));
                    }
                }

                if (KeyIsDown(Keys.U) ||
                    (ButtonIsDown(Buttons.RightThumbstickLeft) && ButtonIsDown(Buttons.RightStick)))
                {
                    // If we should Rotate the Emitter around the Pivot Point
                    if (KeyIsDown(Keys.Y))
                    {
                        mcCurrentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, fEmitterRotateDelta));
                    }
                    // Else we should just Rotate the Emitter about its center
                    else
                    {
                        mcCurrentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, fEmitterRotateDelta));
                    }
                }

                if (KeyIsDown(Keys.O) ||
                    (ButtonIsDown(Buttons.RightThumbstickRight) && ButtonIsDown(Buttons.RightStick)))
                {
                    // If we should Rotate the Emitter around the Pivot Point
                    if (KeyIsDown(Keys.Y))
                    {
                        mcCurrentParticleSystem.Emitter.PivotPointData.RotatePositionAndOrientation(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, -fEmitterRotateDelta));
                    }
                    // Else we should just Rotate the Emitter about its center
                    else
                    {
                        mcCurrentParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, -fEmitterRotateDelta));
                    }
                }
            }

            // Check if the Emitter should be reset
            if (KeyWasJustPressed(Keys.Z))
            {
                mcCurrentParticleSystem.Emitter.PositionData.Position = Vector3.Zero;
                mcCurrentParticleSystem.Emitter.OrientationData.Orientation = Quaternion.Identity;
            }

            // If the Texture should be changed
            if (KeyWasJustPressed(Keys.T) || ButtonWasJustPressed(Buttons.Y))
            {
                if (mcCurrentParticleSystem.Texture != null)
                {
                    // Get which Texture is currently being used for sure
                    for (int i = 0; i < (int)ETextures.LastInList + 1; i++)
                    {
                        ETextures eTexture = (ETextures)i;
                        string sName = eTexture.ToString();

                        if (mcCurrentParticleSystem.Texture.Name.Equals(sName))
                        {
                            meCurrentTexture = (ETextures)i;
                        }
                    }

                    // If we should go to the previous Texture
                    if (KeyIsDown(Keys.LeftShift) || KeyIsDown(Keys.RightShift))
                    {
                        meCurrentTexture--;
                        if (meCurrentTexture < 0)
                        {
                            meCurrentTexture = ETextures.LastInList;
                        }
                    }
                    // Else we should go to the next Texture
                    else
                    {
                        meCurrentTexture++;
                        if (meCurrentTexture > ETextures.LastInList)
                        {
                            meCurrentTexture = 0;
                        }
                    }

                    // Change the Texture being used to draw the Particles
                    mcCurrentParticleSystem.SetTexture("Textures/" + meCurrentTexture.ToString());
                }
            }

            if (KeyWasJustPressed(Keys.Insert))
            {
                // Add a single Particle
                mcCurrentParticleSystem.AddParticle();
            }

            if (KeyIsDown(Keys.Home))
            {
                // Add Particles while the button is pressed
                mcCurrentParticleSystem.AddParticle();
            }

            if (KeyIsDown(Keys.PageUp))
            {
                // Add the max number of Particles
                while (mcCurrentParticleSystem.AddParticle()) { }
            }

            if (KeyWasJustPressed(Keys.Delete) || ButtonWasJustPressed(Buttons.X))
            {
                // Toggle emitting particles on/off
                mcCurrentParticleSystem.Emitter.EmitParticlesAutomatically = !mcCurrentParticleSystem.Emitter.EmitParticlesAutomatically;
            }

            if (KeyIsDown(Keys.Add, 0.02f) || ButtonIsDown(Buttons.RightShoulder, 0.02f))
            {
                // Increase the number of Particles being emitted
                mcCurrentParticleSystem.Emitter.ParticlesPerSecond++;
            }

            if (KeyIsDown(Keys.Subtract, 0.02f) || ButtonIsDown(Buttons.LeftShoulder, 0.02f))
            {
                if (mcCurrentParticleSystem.Emitter.ParticlesPerSecond > 1)
                {
                    // Decrease the number of Particles being emitted
                    mcCurrentParticleSystem.Emitter.ParticlesPerSecond--;
                }
            }

            if (KeyWasJustPressed(Keys.Multiply) || ButtonWasJustPressed(Buttons.B))
            {
                // Increase the Speed of the Particle System simulation
                mcParticleSystemManager.SimulationSpeed += 0.1f;

                if (mcParticleSystemManager.SimulationSpeed > 5.0f)
                {
                    mcParticleSystemManager.SimulationSpeed = 5.0f;
                }

                // If DPSF is not inheriting from DrawableGameComponent then we need
                // to set the individual particle system's Simulation Speeds
                if (DPSFHelper.DPSFInheritsDrawableGameComponent)
                {
                    mcParticleSystemManager.SetSimulationSpeedForAllParticleSystems(mcParticleSystemManager.SimulationSpeed);
                }
            }

            if (KeyWasJustPressed(Keys.Divide) || ButtonWasJustPressed(Buttons.A))
            {
                // Decrease the Speed of the Particle System simulation
                mcParticleSystemManager.SimulationSpeed -= 0.1f;

                if (mcParticleSystemManager.SimulationSpeed < 0.1f)
                {
                    mcParticleSystemManager.SimulationSpeed = 0.1f;
                }

                // If DPSF is not inheriting from DrawableGameComponent then we need
                // to set the individual particle system's Simulation Speeds
                if (DPSFHelper.DPSFInheritsDrawableGameComponent)
                {
                    mcParticleSystemManager.SetSimulationSpeedForAllParticleSystems(mcParticleSystemManager.SimulationSpeed);
                }
            }


            // Check which Particle System is being used
            switch (meCurrentPS)
            {
                default: break;

                case EPSEffects.Random:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcRandomParticleSystem.LoadRandomEvents();
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcRandomParticleSystem.LoadRandomSpiralEvents();
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcRandomParticleSystem.InitialProperties.StartSizeMin += 2.0f;

                        if (mcRandomParticleSystem.InitialProperties.StartSizeMin > 100.0f)
                        {
                            mcRandomParticleSystem.InitialProperties.StartSizeMin = 100.0f;
                        }

                        mcRandomParticleSystem.InitialProperties.StartSizeMax =
                            mcRandomParticleSystem.InitialProperties.EndSizeMin =
                            mcRandomParticleSystem.InitialProperties.EndSizeMax =
                            mcRandomParticleSystem.InitialProperties.StartSizeMin;
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        mcRandomParticleSystem.InitialProperties.StartSizeMin -= 2.0f;

                        if (mcRandomParticleSystem.InitialProperties.StartSizeMin < 2.0f)
                        {
                            mcRandomParticleSystem.InitialProperties.StartSizeMin = 2.0f;
                        }

                        mcRandomParticleSystem.InitialProperties.StartSizeMax =
                            mcRandomParticleSystem.InitialProperties.EndSizeMin =
                            mcRandomParticleSystem.InitialProperties.EndSizeMax =
                            mcRandomParticleSystem.InitialProperties.StartSizeMin;
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcRandomParticleSystem.InitialProperties.StartColorMin =
                            mcRandomParticleSystem.InitialProperties.StartColorMax =
                            DPSFHelper.LerpColor(Color.Black, Color.White, (float)mcRandom.NextDouble(), (float)mcRandom.NextDouble(), (float)mcRandom.NextDouble(), (float)mcRandom.NextDouble());
                    }

                    if (KeyWasJustPressed(Keys.M))
                    {
                        mcRandomParticleSystem.InitialProperties.EndColorMin =
                            mcRandomParticleSystem.InitialProperties.EndColorMax =
                            DPSFHelper.LerpColor(Color.Black, Color.White, (float)mcRandom.NextDouble(), (float)mcRandom.NextDouble(), (float)mcRandom.NextDouble(), (float)mcRandom.NextDouble());
                    }

                    if (KeyWasJustPressed(Keys.P))
                    {
                        mcRandomParticleSystem.LoadTubeEvents();
                    }
                break;

                case EPSEffects.Fire:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcFireParticleSystem.ParticleInitializationFunction = mcFireParticleSystem.InitializeParticleFireOnVerticalRing;
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcFireParticleSystem.ParticleInitializationFunction = mcFireParticleSystem.InitializeParticleFireOnHorizontalRing;
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        float fAmount = mcFireParticleSystem.GetAmountOfSmokeBeingReleased();
                        if (fAmount > 0.0f)
                        {
                            mcFireParticleSystem.SetAmountOfSmokeToRelease(fAmount - 0.05f);
                        }
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        float fAmount = mcFireParticleSystem.GetAmountOfSmokeBeingReleased();
                        if (fAmount < 1.0f)
                        {
                            mcFireParticleSystem.SetAmountOfSmokeToRelease(fAmount + 0.05f);
                        }
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcFireParticleSystem.mbUseAdditiveBlending = !mcFireParticleSystem.mbUseAdditiveBlending;
                    }

                    if (KeyWasJustPressed(Keys.M))
                    {
                        if (mcFireParticleSystem.Emitter.PositionData.Velocity == Vector3.Zero)
                        {
                            mcFireParticleSystem.Emitter.PositionData.Velocity = new Vector3(30, 0, 0);
                        }
                        else
                        {
                            mcFireParticleSystem.Emitter.PositionData.Velocity = Vector3.Zero;
                        }
                    }
                break;

                case EPSEffects.Smoke:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcSmokeParticleSystem.LoadSmokeEvents();
                        mcSmokeParticleSystem.ParticleInitializationFunction = mcSmokeParticleSystem.InitializeParticleRisingSmoke;
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcSmokeParticleSystem.LoadSmokeEvents();
                        mcSmokeParticleSystem.ParticleInitializationFunction = mcSmokeParticleSystem.InitializeParticleFoggySmoke;
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcSphere.bVisible = true;
                        mcSphere.sPosition = mcSmokeParticleSystem.mcExternalObjectPosition = new Vector3(-125, 50, 0);
                        mcSphere.sVelocity = new Vector3(50, 0, 0);
                        mcSphere.cTimeAliveInSeconds = TimeSpan.Zero;
                        mcSmokeParticleSystem.mfAttractRepelRange = mcSphere.fSize * 2;
                        mcSmokeParticleSystem.mfAttractRepelForce = 3.0f;

                        mcSmokeParticleSystem.MakeParticlesAttractToExternalObject();
                    }
                    
                    if (KeyWasJustPressed(Keys.B))
                    {
                        mcSphere.bVisible = true;
                        mcSphere.sPosition = mcSmokeParticleSystem.mcExternalObjectPosition = new Vector3(-125, 50, 0);
                        mcSphere.sVelocity = new Vector3(50, 0, 0);
                        mcSphere.cTimeAliveInSeconds = TimeSpan.Zero;
                        mcSmokeParticleSystem.mfAttractRepelRange = mcSphere.fSize * 2f;
                        mcSmokeParticleSystem.mfAttractRepelForce = 0.5f;

                        mcSmokeParticleSystem.MakeParticlesRepelFromExternalObject();
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcSmokeParticleSystem.ChangeColor();
                    }
                break;

                case EPSEffects.Snow:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcSnowParticleSystem.AddWindForce();
                    }

                    if (KeyWasJustReleased(Keys.C))
                    {
                        mcSnowParticleSystem.RemoveWindForce();
                    }
                break;

                case EPSEffects.SquarePattern:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcSquarePatternParticleSystem.LoadSquarePatternEvents();
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcSquarePatternParticleSystem.LoadChangeColorEvents();
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcSquarePatternParticleSystem.ChangeParticlesToRandomColors();
                    }
                break;

                case EPSEffects.Fountain:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcFountainParticleSystem.MakeParticlesBounceOffFloor();
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcFountainParticleSystem.MakeParticlesNotBounceOffFloor();
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcFountainParticleSystem.mfBounciness -= 0.05f;

                        if (mcFountainParticleSystem.mfBounciness < 0.0f)
                        {
                            mcFountainParticleSystem.mfBounciness = 0.0f;
                        }
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        mcFountainParticleSystem.mfBounciness += 0.05f;

                        if (mcFountainParticleSystem.mfBounciness > 2.0f)
                        {
                            mcFountainParticleSystem.mfBounciness = 2.0f;
                        }
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcFountainParticleSystem.MakeParticlesShrink();
                    }

                    if (KeyWasJustPressed(Keys.M))
                    {
                        mcFountainParticleSystem.MakeParticlesNotShrink();
                    }

                    if (KeyWasJustPressed(Keys.P))
                    {
                        mcFountainParticleSystem.mbUseAdditiveBlending = !mcFountainParticleSystem.mbUseAdditiveBlending;
                    }
                break;

                case EPSEffects.Random2D:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcRandom2DParticleSystem.LoadEvents();
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcRandom2DParticleSystem.LoadExtraEvents();
                    }
                break;

                case EPSEffects.GasFall:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcGasFallParticleSystem.LoadEvents();
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcGasFallParticleSystem.LoadExtraEvents();
                    }
                break;

                case EPSEffects.Dot: break;

                case EPSEffects.Fireworks:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcFireworksParticleSystem.InitialProperties.PositionMin = new Vector3();
                        mcFireworksParticleSystem.InitialProperties.PositionMax = new Vector3();
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcFireworksParticleSystem.InitialProperties.PositionMin = new Vector3(-100, 0, 0);
                        mcFireworksParticleSystem.InitialProperties.PositionMax = new Vector3(100, 0, 0);    
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcFireworksParticleSystem.TurnExplosionsOn();
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        mcFireworksParticleSystem.TurnExplosionsOff();
                    }
                break;

                case EPSEffects.Figure8: break;

                case EPSEffects.Star:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcStarParticleSystem.ParticleInitializationFunction = mcStarParticleSystem.InitializeParticleStar2D;
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcStarParticleSystem.ParticleInitializationFunction = mcStarParticleSystem.InitializeParticleStar3D;
                    }

                    float fRotationScale = MathHelper.Pi / 18.0f;

                    if (KeyIsDown(Keys.V))
                    {
                        // Check if the Emitter is being rotated
                        if (KeyWasJustPressed(Keys.J))
                        {
                            mcStarParticleSystem.Emitter.OrientationData.RotationalVelocity += Vector3.Down * fRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.L))
                        {
                            mcStarParticleSystem.Emitter.OrientationData.RotationalVelocity += Vector3.Up * fRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.I))
                        {
                            mcStarParticleSystem.Emitter.OrientationData.RotationalVelocity += Vector3.Left * fRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.K))
                        {
                            mcStarParticleSystem.Emitter.OrientationData.RotationalVelocity += Vector3.Right * fRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.U))
                        {
                            mcStarParticleSystem.Emitter.OrientationData.RotationalVelocity += Vector3.Backward * fRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.O))
                        {
                            mcStarParticleSystem.Emitter.OrientationData.RotationalVelocity += Vector3.Forward * fRotationScale;
                        }
                    }

                    if (KeyIsDown(Keys.B))
                    {
                        // Check if the Emitter is being rotated
                        if (KeyWasJustPressed(Keys.J))
                        {
                            mcStarParticleSystem.Emitter.OrientationData.RotationalAcceleration += Vector3.Down * fRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.L))
                        {
                            mcStarParticleSystem.Emitter.OrientationData.RotationalAcceleration += Vector3.Up * fRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.I))
                        {
                            mcStarParticleSystem.Emitter.OrientationData.RotationalAcceleration += Vector3.Left * fRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.K))
                        {
                            mcStarParticleSystem.Emitter.OrientationData.RotationalAcceleration += Vector3.Right * fRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.U))
                        {
                            mcStarParticleSystem.Emitter.OrientationData.RotationalAcceleration += Vector3.Backward * fRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.O))
                        {
                            mcStarParticleSystem.Emitter.OrientationData.RotationalAcceleration += Vector3.Forward * fRotationScale;
                        }
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcStarParticleSystem.Emitter.PositionData.Velocity = Vector3.Zero;
                        mcStarParticleSystem.Emitter.PositionData.Acceleration = Vector3.Zero;
                        mcStarParticleSystem.Emitter.OrientationData.RotationalVelocity = Vector3.Zero;
                        mcStarParticleSystem.Emitter.OrientationData.RotationalAcceleration = Vector3.Zero;
                        mcStarParticleSystem.ParticleSystemEvents.RemoveAllEvents();
                    }

                    if (KeyWasJustPressed(Keys.M))
                    {
                        mcStarParticleSystem.mbHighlightAxis = !mcStarParticleSystem.mbHighlightAxis;
                    }

                    if (KeyIsDown(Keys.P))
                    {
                        // Check if the Emitter is being rotated
                        if (KeyWasJustPressed(Keys.J))
                        {
                            mcStarParticleSystem.LoadSlowRotationalWiggle();
                        }

                        if (KeyWasJustPressed(Keys.L))
                        {
                            mcStarParticleSystem.LoadFastRotationalWiggle();
                        }

                        if (KeyWasJustPressed(Keys.I))
                        {
                            mcStarParticleSystem.LoadMediumWiggle();
                        }

                        if (KeyWasJustPressed(Keys.K))
                        {
                            mcStarParticleSystem.LoadMediumRotationalWiggle();
                        }

                        if (KeyWasJustPressed(Keys.U))
                        {
                            mcStarParticleSystem.LoadSlowWiggle();
                        }

                        if (KeyWasJustPressed(Keys.O))
                        {
                            mcStarParticleSystem.LoadFastWiggle();
                        }
                    }

                    if (KeyWasJustPressed(Keys.OemOpenBrackets))
                    {
                        mcStarParticleSystem.ToggleEmitterIntermittance();
                    }
                break;

                case EPSEffects.Ball:
                    if (KeyIsDown(Keys.X, 0.02f))
                    {
                        mcBallParticleSystem.IncreaseRadius();
                    }

                    if (KeyIsDown(Keys.C, 0.02f))
                    {
                        mcBallParticleSystem.DecreaseRadius();
                    }

                    float fBallRotationScale = MathHelper.Pi / 18.0f;

                    if (KeyIsDown(Keys.V))
                    {
                        // Check if the Emitter is being rotated
                        if (KeyWasJustPressed(Keys.J))
                        {
                            mcBallParticleSystem.Emitter.OrientationData.RotationalVelocity += Vector3.Down * fBallRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.L))
                        {
                            mcBallParticleSystem.Emitter.OrientationData.RotationalVelocity += Vector3.Up * fBallRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.I))
                        {
                            mcBallParticleSystem.Emitter.OrientationData.RotationalVelocity += Vector3.Left * fBallRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.K))
                        {
                            mcBallParticleSystem.Emitter.OrientationData.RotationalVelocity += Vector3.Right * fBallRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.U))
                        {
                            mcBallParticleSystem.Emitter.OrientationData.RotationalVelocity += Vector3.Backward * fBallRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.O))
                        {
                            mcBallParticleSystem.Emitter.OrientationData.RotationalVelocity += Vector3.Forward * fBallRotationScale;
                        }
                    }

                    if (KeyIsDown(Keys.B))
                    {
                        // Check if the Emitter is being rotated
                        if (KeyWasJustPressed(Keys.J))
                        {
                            mcBallParticleSystem.Emitter.OrientationData.RotationalAcceleration += Vector3.Down * fBallRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.L))
                        {
                            mcBallParticleSystem.Emitter.OrientationData.RotationalAcceleration += Vector3.Up * fBallRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.I))
                        {
                            mcBallParticleSystem.Emitter.OrientationData.RotationalAcceleration += Vector3.Left * fBallRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.K))
                        {
                            mcBallParticleSystem.Emitter.OrientationData.RotationalAcceleration += Vector3.Right * fBallRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.U))
                        {
                            mcBallParticleSystem.Emitter.OrientationData.RotationalAcceleration += Vector3.Backward * fBallRotationScale;
                        }

                        if (KeyWasJustPressed(Keys.O))
                        {
                            mcBallParticleSystem.Emitter.OrientationData.RotationalAcceleration += Vector3.Forward * fBallRotationScale;
                        }
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcBallParticleSystem.Emitter.OrientationData.RotationalVelocity = Vector3.Zero;
                        mcBallParticleSystem.Emitter.OrientationData.RotationalAcceleration = Vector3.Zero;
                    }

                    if (KeyIsDown(Keys.OemOpenBrackets, 0.04f))
                    {
                        mcBallParticleSystem.MoreParticles();
                    }

                    if (KeyIsDown(Keys.P, 0.04f))
                    {
                        mcBallParticleSystem.LessParticles();
                    }

                    if (KeyWasJustPressed(Keys.M))
                    {
                        mcBallParticleSystem.RemoveAllParticles();
                    }
                break;

                case EPSEffects.RotatingQuad:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcRotatingQuadParticleSystem.MakeParticlesFaceWhateverDirection();
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcRotatingQuadParticleSystem.MakeParticlesFaceTheCamera();
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcRotatingQuadParticleSystem.MakeParticlesFaceTheCenter();
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        mcRotatingQuadParticleSystem.RemoveAllParticles();
                        int iNumberOfParticles = mcRotatingQuadParticleSystem.MaxNumberOfParticlesAllowed;

                        switch (iNumberOfParticles)
                        {
                            default:
                            case 1:
                                mcRotatingQuadParticleSystem.MaxNumberOfParticlesAllowed = 10;
                            break;

                            case 10:
                                mcRotatingQuadParticleSystem.MaxNumberOfParticlesAllowed = 100;
                            break;

                            case 100:
                                mcRotatingQuadParticleSystem.MaxNumberOfParticlesAllowed = 500;
                            break;

                            case 500:
                                mcRotatingQuadParticleSystem.MaxNumberOfParticlesAllowed = 1000;
                            break;

                            case 1000:
                                mcRotatingQuadParticleSystem.MaxNumberOfParticlesAllowed = 1;
                            break;
                        }
                    }
                break;

                case EPSEffects.Box:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcBoxParticleSystem.LoadBox();
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcBoxParticleSystem.LoadBoxBars();
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcBoxParticleSystem.ToggleColorChanges();
                    }
                break;

                case EPSEffects.Image:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcImageParticleSystem.LoadImage();
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcImageParticleSystem.LoadSpiralIntoFinalImage();
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcImageParticleSystem.LoadVortexIntoFinalImage();
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        string sSpinMode = mcImageParticleSystem.msSpinMode;

                        switch (sSpinMode)
                        {
                            case "Pitch":
                                sSpinMode = "Yaw";
                            break;

                            case "Yaw":
                                sSpinMode = "Roll";
                            break;

                            case "Roll":
                                sSpinMode = "All";
                            break;

                            case "All":
                                sSpinMode = "None";
                            break;

                            default:
                            case "None":
                                sSpinMode = "Pitch";
                            break;
                        }

                        mcImageParticleSystem.ToggleSpin(sSpinMode);
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcImageParticleSystem.ToggleUniformSpin();
                    }

                    if (KeyWasJustPressed(Keys.M))
                    {
                        mcImageParticleSystem.Scatter();
                    }

                    if (KeyWasJustPressed(Keys.P))
                    {
                        int iRows = mcImageParticleSystem.miNumberOfRows;
                        int iColumns = mcImageParticleSystem.miNumberOfColumns;

                        switch (iRows)
                        {
                            default:
                            case 2:
                                iRows = 4;
                            break;

                            case 4:
                                iRows = 8;
                            break;

                            case 8:
                                iRows = 16;
                            break;

                            case 16:
                                iRows = 32;
                            break;

                            case 32:
                                iRows = 64;
                            break;

                            case 64:
                                iRows = 2;
                            break;
                        }

                        mcImageParticleSystem.SetNumberOfRowsAndColumns(iRows, iColumns);
                    }

                    if (KeyWasJustPressed(Keys.OemOpenBrackets))
                    {
                        int iRows = mcImageParticleSystem.miNumberOfRows;
                        int iColumns = mcImageParticleSystem.miNumberOfColumns;

                        switch (iColumns)
                        {
                            default:
                            case 2:
                                iColumns = 4;
                            break;

                            case 4:
                                iColumns = 8;
                            break;

                            case 8:
                                iColumns = 16;
                            break;

                            case 16:
                                iColumns = 32;
                            break;

                            case 32:
                                iColumns = 64;
                            break;

                            case 64:
                                iColumns = 2;
                            break;
                        }

                        mcImageParticleSystem.SetNumberOfRowsAndColumns(iRows, iColumns);
                    }
                break;

                case EPSEffects.AnimatedTexturedQuad: break;

                case EPSEffects.Sprite:
                    // If the mouse was moved
                    if (mcCurrentMouseState.X != mcPreviousMouseState.X ||
                        mcCurrentMouseState.Y != mcPreviousMouseState.Y)
                    {
                        mcSpriteParticleSystem.AttractorPosition = new Vector3(mcCurrentMouseState.X, mcCurrentMouseState.Y, 0);
                    }

                    // If the left mouse button was just pressed
                    if (mcCurrentMouseState.LeftButton == ButtonState.Pressed &&
                        mcPreviousMouseState.LeftButton == ButtonState.Released)
                    {
                        mcSpriteParticleSystem.ToggleAttractorMode();
                    }

                    // If the right mouse button was just pressed
                    if (mcCurrentMouseState.RightButton == ButtonState.Pressed &&
                        mcPreviousMouseState.RightButton == ButtonState.Released)
                    {
                        mcSpriteParticleSystem.ToggleAttractorStrength();
                    }

                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcSpriteParticleSystem.LoadAttractionEvents();
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcSpriteParticleSystem.LoadCloudEvents();
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcSpriteParticleSystem.LoadGridEvents();
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        mcSpriteParticleSystem.LoadRotatorsEvents();
                    }
                break;

                case EPSEffects.AnimatedSprite:
                    // If the mouse was moved
                    if (mcCurrentMouseState.X != mcPreviousMouseState.X ||
                        mcCurrentMouseState.Y != mcPreviousMouseState.Y)
                    {
                        mcAnimatedSpriteParticleSystem.MousePosition = new Vector3(mcCurrentMouseState.X, mcCurrentMouseState.Y, 0);
                    }

                    // If the left mouse button was just pressed
                    if (mcCurrentMouseState.LeftButton == ButtonState.Pressed &&
                        mcPreviousMouseState.LeftButton == ButtonState.Released)
                    {
                        mcAnimatedSpriteParticleSystem.ToggleColorAmount();
                    }

                    // If the right mouse button was just pressed
                    if (mcCurrentMouseState.RightButton == ButtonState.Pressed &&
                        mcPreviousMouseState.RightButton == ButtonState.Released)
                    {
                        mcAnimatedSpriteParticleSystem.AddParticle();
                    }

                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcAnimatedSpriteParticleSystem.LoadExplosionEvents();
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcAnimatedSpriteParticleSystem.LoadButterflyEvents();
                    }
                break;

                case EPSEffects.Pixel:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcPixelParticleSystem.LoadSprayEvents();
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcPixelParticleSystem.LoadWallEvents();
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcPixelParticleSystem.mbGravityEnabled = !mcPixelParticleSystem.mbGravityEnabled;
                    }
                break;

                case EPSEffects.Magnets:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcMagnetParticleSystem.LoadEmitterMagnetParticleSystem();
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcMagnetParticleSystem.LoadSeparateEmitterMagnetsParticleSystem();
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcMagnetParticleSystem.ToggleMagnetsAffectingPositionVsVelocity();
                    }
                break;

                case EPSEffects.Sparkler:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcSparklerParticleSystem.LoadSimpleParticleSystem();
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcSparklerParticleSystem.LoadComplexParticleSystem();
                    }
                break;

                case EPSEffects.Grid: break;

                case EPSEffects.Sphere:
                    if (KeyIsDown(Keys.X, 0.02f))
                    {
                        mcSphereParticleSystem.ChangeSphereRadius(-5);
                    }

                    if (KeyIsDown(Keys.C, 0.02f))
                    {
                        mcSphereParticleSystem.ChangeSphereRadius(5);
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcSphereParticleSystem.MakeParticlesTravelInTheSameDirection();
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        mcSphereParticleSystem.MakeParticlesTravelInRandomDirections();
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcSphereParticleSystem.ChangeNumberOfParticles(-50);
                    }

                    if (KeyWasJustPressed(Keys.M))
                    {
                        mcSphereParticleSystem.ChangeNumberOfParticles(50);
                    }
                break;

                case EPSEffects.MultipleParticleImages: break;

                case EPSEffects.ExplosionFireSmoke:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcExplosionFireSmokeParticleSystem.ExplosionIntensity -= 5;
                        mcExplosionFireSmokeParticleSystem.ExplosionIntensity = (mcExplosionFireSmokeParticleSystem.ExplosionIntensity < 1 ? 1 : mcExplosionFireSmokeParticleSystem.ExplosionIntensity);
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcExplosionFireSmokeParticleSystem.ExplosionIntensity += 5;
                        mcExplosionFireSmokeParticleSystem.ExplosionIntensity = (mcExplosionFireSmokeParticleSystem.ExplosionIntensity > 200 ? 200 : mcExplosionFireSmokeParticleSystem.ExplosionIntensity);
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcExplosionFireSmokeParticleSystem.ChangeExplosionColor();
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        mcExplosionFireSmokeParticleSystem.ExplosionParticleSize -= 5;
                        mcExplosionFireSmokeParticleSystem.ExplosionParticleSize = (mcExplosionFireSmokeParticleSystem.ExplosionParticleSize < 1 ? 1 : mcExplosionFireSmokeParticleSystem.ExplosionParticleSize);
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcExplosionFireSmokeParticleSystem.ExplosionParticleSize += 5;
                        mcExplosionFireSmokeParticleSystem.ExplosionParticleSize = (mcExplosionFireSmokeParticleSystem.ExplosionParticleSize > 100 ? 100 : mcExplosionFireSmokeParticleSystem.ExplosionParticleSize);
                    }
                break;

                case EPSEffects.ExplosionFlash:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcExplosionFlashParticleSystem.ExplosionIntensity -= 5;
                        mcExplosionFlashParticleSystem.ExplosionIntensity = (mcExplosionFlashParticleSystem.ExplosionIntensity < 1 ? 1 : mcExplosionFlashParticleSystem.ExplosionIntensity);
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcExplosionFlashParticleSystem.ExplosionIntensity += 5;
                        mcExplosionFlashParticleSystem.ExplosionIntensity = (mcExplosionFlashParticleSystem.ExplosionIntensity > 100 ? 100 : mcExplosionFlashParticleSystem.ExplosionIntensity);
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcExplosionFlashParticleSystem.ChangeExplosionColor();
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        mcExplosionFlashParticleSystem.ExplosionParticleSize -= 5;
                        mcExplosionFlashParticleSystem.ExplosionParticleSize = (mcExplosionFlashParticleSystem.ExplosionParticleSize < 1 ? 1 : mcExplosionFlashParticleSystem.ExplosionParticleSize);
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcExplosionFlashParticleSystem.ExplosionParticleSize += 5;
                        mcExplosionFlashParticleSystem.ExplosionParticleSize = (mcExplosionFlashParticleSystem.ExplosionParticleSize > 100 ? 100 : mcExplosionFlashParticleSystem.ExplosionParticleSize);
                    }
                break;

                case EPSEffects.ExplosionFlyingSparks:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcExplosionFlyingSparksParticleSystem.ExplosionIntensity -= 5;
                        mcExplosionFlyingSparksParticleSystem.ExplosionIntensity = (mcExplosionFlyingSparksParticleSystem.ExplosionIntensity < 1 ? 1 : mcExplosionFlyingSparksParticleSystem.ExplosionIntensity);
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcExplosionFlyingSparksParticleSystem.ExplosionIntensity += 5;
                        mcExplosionFlyingSparksParticleSystem.ExplosionIntensity = (mcExplosionFlyingSparksParticleSystem.ExplosionIntensity > 200 ? 200 : mcExplosionFlyingSparksParticleSystem.ExplosionIntensity);
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcExplosionFlyingSparksParticleSystem.ChangeExplosionColor();
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        mcExplosionFlyingSparksParticleSystem.ExplosionParticleSize -= 5;
                        mcExplosionFlyingSparksParticleSystem.ExplosionParticleSize = (mcExplosionFlyingSparksParticleSystem.ExplosionParticleSize < 1 ? 1 : mcExplosionFlyingSparksParticleSystem.ExplosionParticleSize);
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcExplosionFlyingSparksParticleSystem.ExplosionParticleSize += 5;
                        mcExplosionFlyingSparksParticleSystem.ExplosionParticleSize = (mcExplosionFlyingSparksParticleSystem.ExplosionParticleSize > 100 ? 100 : mcExplosionFlyingSparksParticleSystem.ExplosionParticleSize);
                    }
                break;

                case EPSEffects.ExplosionSmokeTrails:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcExplosionSmokeTrailsParticleSystem.ExplosionIntensity -= 5;
                        mcExplosionSmokeTrailsParticleSystem.ExplosionIntensity = (mcExplosionSmokeTrailsParticleSystem.ExplosionIntensity < 1 ? 1 : mcExplosionSmokeTrailsParticleSystem.ExplosionIntensity);
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcExplosionSmokeTrailsParticleSystem.ExplosionIntensity += 5;
                        mcExplosionSmokeTrailsParticleSystem.ExplosionIntensity = (mcExplosionSmokeTrailsParticleSystem.ExplosionIntensity > 200 ? 200 : mcExplosionSmokeTrailsParticleSystem.ExplosionIntensity);
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcExplosionSmokeTrailsParticleSystem.ChangeExplosionColor();
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        mcExplosionSmokeTrailsParticleSystem.ExplosionParticleSize -= 5;
                        mcExplosionSmokeTrailsParticleSystem.ExplosionParticleSize = (mcExplosionSmokeTrailsParticleSystem.ExplosionParticleSize < 1 ? 1 : mcExplosionSmokeTrailsParticleSystem.ExplosionParticleSize);
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcExplosionSmokeTrailsParticleSystem.ExplosionParticleSize += 5;
                        mcExplosionSmokeTrailsParticleSystem.ExplosionParticleSize = (mcExplosionSmokeTrailsParticleSystem.ExplosionParticleSize > 100 ? 100 : mcExplosionSmokeTrailsParticleSystem.ExplosionParticleSize);
                    }
                break;

                case EPSEffects.ExplosionRoundSparks:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcExplosionRoundSparksParticleSystem.ExplosionIntensity -= 5;
                        mcExplosionRoundSparksParticleSystem.ExplosionIntensity = (mcExplosionRoundSparksParticleSystem.ExplosionIntensity < 1 ? 1 : mcExplosionRoundSparksParticleSystem.ExplosionIntensity);
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcExplosionRoundSparksParticleSystem.ExplosionIntensity += 5;
                        mcExplosionRoundSparksParticleSystem.ExplosionIntensity = (mcExplosionRoundSparksParticleSystem.ExplosionIntensity > 100 ? 100 : mcExplosionRoundSparksParticleSystem.ExplosionIntensity);
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcExplosionRoundSparksParticleSystem.ChangeExplosionColor();
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        mcExplosionRoundSparksParticleSystem.ExplosionParticleSize -= 5;
                        mcExplosionRoundSparksParticleSystem.ExplosionParticleSize = (mcExplosionRoundSparksParticleSystem.ExplosionParticleSize < 1 ? 1 : mcExplosionRoundSparksParticleSystem.ExplosionParticleSize);
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcExplosionRoundSparksParticleSystem.ExplosionParticleSize += 5;
                        mcExplosionRoundSparksParticleSystem.ExplosionParticleSize = (mcExplosionRoundSparksParticleSystem.ExplosionParticleSize > 100 ? 100 : mcExplosionRoundSparksParticleSystem.ExplosionParticleSize);
                    }
                break;

                case EPSEffects.ExplosionDebris:
                    if (KeyWasJustPressed(Keys.X))
                    {
                        mcExplosionDebrisParticleSystem.ExplosionIntensity -= 5;
                        mcExplosionDebrisParticleSystem.ExplosionIntensity = (mcExplosionDebrisParticleSystem.ExplosionIntensity < 1 ? 1 : mcExplosionDebrisParticleSystem.ExplosionIntensity);
                    }

                    if (KeyWasJustPressed(Keys.C))
                    {
                        mcExplosionDebrisParticleSystem.ExplosionIntensity += 5;
                        mcExplosionDebrisParticleSystem.ExplosionIntensity = (mcExplosionDebrisParticleSystem.ExplosionIntensity > 200 ? 200 : mcExplosionDebrisParticleSystem.ExplosionIntensity);
                    }

                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcExplosionDebrisParticleSystem.ChangeExplosionColor();
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        mcExplosionDebrisParticleSystem.ExplosionParticleSize -= 5;
                        mcExplosionDebrisParticleSystem.ExplosionParticleSize = (mcExplosionDebrisParticleSystem.ExplosionParticleSize < 1 ? 1 : mcExplosionDebrisParticleSystem.ExplosionParticleSize);
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcExplosionDebrisParticleSystem.ExplosionParticleSize += 5;
                        mcExplosionDebrisParticleSystem.ExplosionParticleSize = (mcExplosionDebrisParticleSystem.ExplosionParticleSize > 100 ? 100 : mcExplosionDebrisParticleSystem.ExplosionParticleSize);
                    }
                break;

                case EPSEffects.ExplosionShockwave:
                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcExplosionShockwaveParticleSystem.ChangeExplosionColor();
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        mcExplosionShockwaveParticleSystem.ShockwaveSize -= 5;
                        mcExplosionShockwaveParticleSystem.ShockwaveSize = (mcExplosionShockwaveParticleSystem.ShockwaveSize < 100 ? 100 : mcExplosionShockwaveParticleSystem.ShockwaveSize);
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcExplosionShockwaveParticleSystem.ShockwaveSize += 5;
                        mcExplosionShockwaveParticleSystem.ShockwaveSize = (mcExplosionShockwaveParticleSystem.ShockwaveSize > 400 ? 400 : mcExplosionShockwaveParticleSystem.ShockwaveSize);
                    }
                break;

                case EPSEffects.Explosion:
                    if (KeyWasJustPressed(Keys.V))
                    {
                        mcExplosionParticleSystem.ChangeExplosionColor();
                    }

                    if (KeyWasJustPressed(Keys.B))
                    {
                        mcExplosionParticleSystem.ExplosionParticleSize -= 5;
                        mcExplosionParticleSystem.ExplosionParticleSize = (mcExplosionParticleSystem.ExplosionParticleSize < 1 ? 1 : mcExplosionParticleSystem.ExplosionParticleSize);
                    }

                    if (KeyWasJustPressed(Keys.N))
                    {
                        mcExplosionParticleSystem.ExplosionParticleSize += 5;
                        mcExplosionParticleSystem.ExplosionParticleSize = (mcExplosionParticleSystem.ExplosionParticleSize > 100 ? 100 : mcExplosionParticleSystem.ExplosionParticleSize);
                    }
                break;

                case EPSEffects.PixelParticleSystemTemplate: break;
                case EPSEffects.SpriteParticleSystemTemplate: break;
                case EPSEffects.PointSpriteParticleSystemTemplate: break;
                case EPSEffects.QuadParticleSystemTemplate: break;
                case EPSEffects.TexturedQuadParticleSystemTemplate: break;
                case EPSEffects.DefaultPixelParticleSystemTemplate: break;
                case EPSEffects.DefaultSpriteParticleSystemTemplate: break;
                case EPSEffects.DefaultPointSpriteParticleSystemTemplate: break;
                case EPSEffects.DefaultQuadParticleSystemTemplate: break;
                case EPSEffects.DefaultTexturedQuadParticleSystemTemplate: break;
            }
        }

        #endregion
    }

    #region Application Entry Point

    /// <summary>
    /// The main entry point for the application
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (GameMain Game = new GameMain())
            {
#if (!XBOX)
                // String to hold any prerequisites error messages
                string prerequisitesErrorMessage = "";

                // If XNA 3.1 is not installed
                Microsoft.Win32.RegistryKey xnaKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\XNA\\Framework\\v3.1");
                if (xnaKey == null || (int)xnaKey.GetValue("Installed") != 1)
                {
                    // Store the error message
                    prerequisitesErrorMessage += "XNA 3.1 must be installed to run this program. You can download the XNA 3.1 Redistributable from http://www.microsoft.com/downloads/ \n\n";
                }

                // If .NET 3.5 or 4 is not installed
                Microsoft.Win32.RegistryKey netKey35 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v3.5");
                Microsoft.Win32.RegistryKey netKey4 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Client");
                bool net35NotInstalled = (netKey35 == null || (int)netKey35.GetValue("Install") != 1);
                bool net4NotInstalled = (netKey4 == null || (int)netKey4.GetValue("Install") != 1);
                if (net35NotInstalled && net4NotInstalled)
                {
                    // Store the error message
                    prerequisitesErrorMessage += "The .NET Framework 3.5 or greater must be installed to run this program. You can download the .NET Framework from http://www.microsoft.com/downloads/ \n\n";
                }

                // If not all of the prerequisites are installed
                if (!string.IsNullOrEmpty(prerequisitesErrorMessage))
                {
                    // Add to the error message the option of trying to run the program anyways
                    prerequisitesErrorMessage += "Do you want to try and run the program anyways, even though not all of the prerequisites are installed?";

                    // Display the error message and exit
                    if (System.Windows.Forms.MessageBox.Show(prerequisitesErrorMessage, "Prerequisites Not Installed", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                }

                // If we are in Release Mode
                if (GameMain.mbRELEASE_MODE)
                {
                    try
                    {
                        Game.Run();     // Start the game
                    }
                    catch (Exception e)
                    {
                        // Display any error messages that occur
                        System.Windows.Forms.MessageBox.Show(e.ToString(), "Unhandled Exception");
                    }
                }
                // Else we are in debug mode, so allow Visual Studio to show us the error message
                else
                {
                    Game.Run();
                }
#else
                Game.Run();
#endif
            }
        }
    }

    #endregion
}
