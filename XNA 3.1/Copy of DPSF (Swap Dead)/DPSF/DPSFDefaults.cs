#region File Description
//===================================================================
// DPSFDefaults.cs
// 
// This file provides some Default Particle Systems that inherit from
// the DPSF class. These classes may be used as-is to create new 
// Particle Systems, or my be inherited by new Particle System classes
// to extend the existing functionality.
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF
{
    #region Default Particle System Type Declarations
    // Declare the Default Particle Systems as easy to use types

    /// <summary>
    /// The Default Pixel Particle System to inherit from, which uses Default Pixel Particles
    /// </summary>
    public class DefaultPixelParticleSystem : DPSFDefaultPixelParticleSystem<DefaultPixelParticle, DefaultPixelParticleVertex>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DefaultPixelParticleSystem(Game cGame) : base(cGame) { }
    }

    /// <summary>
    /// The Default Sprite Particle System to inherit from, which uses Default Sprite Particles
    /// </summary>
    public class DefaultSpriteParticleSystem : DPSFDefaultSpriteParticleSystem<DefaultSpriteParticle, DefaultSpriteParticleVertex>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DefaultSpriteParticleSystem(Game cGame) : base(cGame) { }
    }

    /// <summary>
    /// The Default Animated Sprite Particle Sytem to inherit from, which uses Default Animated Sprite Particles
    /// </summary>
    public class DefaultAnimatedSpriteParticleSystem : DPSFDefaultAnimatedSpriteParticleSystem<DefaultAnimatedSpriteParticle, DefaultSpriteParticleVertex>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DefaultAnimatedSpriteParticleSystem(Game cGame) : base(cGame) { }
    }

    /// <summary>
    /// The Default Point Sprite Particle System to inherit from, which uses Default Point Sprite Particles
    /// </summary>
    public class DefaultPointSpriteParticleSystem : DPSFDefaultPointSpriteParticleSystem<DefaultPointSpriteParticle, DefaultPointSpriteParticleVertex>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DefaultPointSpriteParticleSystem(Game cGame) : base(cGame) { }
    }

    /// <summary>
    /// The Default Quad Particle System to inherit from, which uses Default Quad Particles
    /// </summary>
    public class DefaultQuadParticleSystem : DPSFDefaultQuadParticleSystem<DefaultQuadParticle, DefaultQuadParticleVertex>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DefaultQuadParticleSystem(Game cGame) : base(cGame) { }
    }

    /// <summary>
    /// The Default Textured Quad Particle System to inherit from, which uses Default Textured Quad Particles
    /// </summary>
    public class DefaultTexturedQuadParticleSystem : DPSFDefaultTexturedQuadParticleSystem<DefaultTexturedQuadParticle, DefaultTexturedQuadParticleVertex>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DefaultTexturedQuadParticleSystem(Game cGame) : base(cGame) { }
    }

    /// <summary>
    /// The Default Animated Textured Quad Particle System to inherit from, which uses Default Animated Textured Quad Particles
    /// </summary>
    public class DefaultAnimatedTexturedQuadParticleSystem : DPSFDefaultAnimatedTexturedQuadParticleSystem<DefaultAnimatedTexturedQuadParticle, DefaultTexturedQuadParticleVertex>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DefaultAnimatedTexturedQuadParticleSystem(Game cGame) : base(cGame) { }
    }
    #endregion

    #region Default Particles

    /// <summary>
    /// The Base Particle class from which the Default Particle classes inherit from
    /// </summary>
    public class DPSFDefaultBaseParticle : DPSFParticle
    {
        // The Particle Properties were left as public variables as opposed to
        // public Properties for a few reasons: 1 - Properties take about 4x as
        // much code as using simple variables (unless using automatic properties),
        // 2 - Properties cannot be passed as ref or out, and 3 - You can't change
        // individual struct components when using properties. That is, you couldn't
        // write cParticle.Position.X = 1; you would have to set cParticle.Position 
        // to a whole new vector, which is less convenient.

        // Properties used to draw the Particle

        /// <summary>
        /// The Position of the Particle in 3D space.
        /// <para>NOTE: For 2D Pixel and Sprite Particles, the Z value can still be used to
        /// determine which Particles are drawn in front of others (0.0 = front, 
        /// 1.0 = back) when SpriteBatchOptions.eSortMode = SpriteSortMode.BackToFront
        /// or SpriteSortMode.FrontToBack</para>
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// The Color of the Particle, or if using a Texture, the Color to incorporate into the Particle's Texture.
        /// <para>NOTE: This Color's alpha value controls the transparency of the Particle's Texture.</para>
        /// </summary>
        public Color Color;

        // Additional Particle Properties

        /// <summary>
        /// The Particle's Velocity
        /// </summary>
        public Vector3 Velocity;

        /// <summary>
        /// The Particle's Acceleration
        /// </summary>
        public Vector3 Acceleration;

        /// <summary>
        /// An External Force that may be applied to the Particle
        /// </summary>
        public Vector3 ExternalForce;
        
        /// <summary>
        /// The Friction to apply to the Particle
        /// </summary>
        public float Friction;

        /// <summary>
        /// The Particle's Color when it is born
        /// </summary>
        public Color StartColor;

        /// <summary>
        /// The Particle's Color when it dies
        /// </summary>
        public Color EndColor;

        /// <summary>
        /// Resets the Particle variables to their default values
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Position = Vector3.Zero;
            Color = Color.White;

            Velocity = Acceleration = ExternalForce = Vector3.Zero;
            Friction = 0.0f;
            StartColor = EndColor = Color.White;
        }

        /// <summary>
        /// Deep copy all of the Particle properties
        /// </summary>
        /// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
        public override void CopyFrom(DPSFParticle ParticleToCopy)
        {
            // Cast the Particle to the type it really is
            DPSFDefaultBaseParticle cParticleToCopy = (DPSFDefaultBaseParticle)ParticleToCopy;

            base.CopyFrom(cParticleToCopy);
            this.Position = cParticleToCopy.Position;
            this.Color = cParticleToCopy.Color;

            this.Velocity = cParticleToCopy.Velocity;
            this.Acceleration = cParticleToCopy.Acceleration;
            this.ExternalForce = cParticleToCopy.ExternalForce;
            this.Friction = cParticleToCopy.Friction;
            this.StartColor = cParticleToCopy.StartColor;
            this.EndColor = cParticleToCopy.EndColor;
        }
    }

    /// <summary>
    /// Particle used by the Default Pixel Particle System
    /// </summary>
    public class DefaultPixelParticle : DPSFDefaultBaseParticle
    { }

    /// <summary>
    /// Particle used by the Default Sprite Particle System
    /// </summary>
    public class DefaultSpriteParticle : DPSFDefaultBaseParticle
    {
        // Properties used to draw the Particle

        /// <summary>
        /// How much the Particle should be Rotated
        /// </summary>
        public float Rotation;

        /// <summary>
        /// The Width of the Particle
        /// </summary>
        public float Width;

        /// <summary>
        /// The Height of the Particle
        /// </summary>
        public float Height;

        /// <summary>
        /// Tells if the Sprite should be flipped Horizontally or Vertically
        /// </summary>
        public SpriteEffects FlipMode;

        // Additional Particle Properties

        /// <summary>
        /// The Particle's Rotational Velocity
        /// </summary>
        public float RotationalVelocity;

        /// <summary>
        /// The Particle's Rotational Acceleration
        /// </summary>
        public float RotationalAcceleration;

        /// <summary>
        /// The Width of the Particle when it is born
        /// </summary>
        public float StartWidth;

        /// <summary>
        /// The Height of the Particle when it is born
        /// </summary>
        public float StartHeight;
        
        /// <summary>
        /// The Width of the Particle when it dies
        /// </summary>
        public float EndWidth;

        /// <summary>
        /// The Height of the Particle when it dies
        /// </summary>
        public float EndHeight;

        /// <summary>
        /// Resets the Particle variables to their default values
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Rotation = 0.0f;
            Width = Height = 10.0f;
            FlipMode = SpriteEffects.None;

            RotationalVelocity = RotationalAcceleration = 0.0f;
            StartWidth = StartHeight = EndWidth = EndHeight = 10.0f;
        }

        /// <summary>
        /// Deep copy all of the Particle properties
        /// </summary>
        /// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
        public override void CopyFrom(DPSFParticle ParticleToCopy)
        {
            // Cast the Particle to the type it really is
            DefaultSpriteParticle cParticleToCopy = (DefaultSpriteParticle)ParticleToCopy;

            base.CopyFrom(cParticleToCopy);
            this.Rotation = cParticleToCopy.Rotation;
            this.Width = cParticleToCopy.Width;
            this.Height = cParticleToCopy.Height;
            this.FlipMode = cParticleToCopy.FlipMode;

            this.RotationalVelocity = cParticleToCopy.RotationalVelocity;
            this.RotationalAcceleration = cParticleToCopy.RotationalAcceleration;

            this.StartWidth = cParticleToCopy.StartWidth;
            this.StartHeight = cParticleToCopy.StartHeight;
            this.EndWidth = cParticleToCopy.EndWidth;
            this.EndHeight = cParticleToCopy.EndHeight;
        }
    }

    /// <summary>
    /// Particle used by the Default Animated Sprite Particle System
    /// </summary>
    public class DefaultAnimatedSpriteParticle : DefaultSpriteParticle
    {
        /// <summary>
        /// Class to hold this Particle's Animation information
        /// </summary>
        public Animations Animation;

        /// <summary>
        /// The Position and Dimensions of this Picture in the Texture
        /// </summary>
        public Rectangle TextureCoordinates;

        /// <summary>
        /// Resets the Particle variables to their default values
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Animation = new Animations();
            TextureCoordinates = new Rectangle();
        }

        /// <summary>
        /// Deep copy all of the Particle properties
        /// </summary>
        /// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
        public override void CopyFrom(DPSFParticle ParticleToCopy)
        {
            // Cast the Particle to the type it really is
            DefaultAnimatedSpriteParticle cParticleToCopy = (DefaultAnimatedSpriteParticle)ParticleToCopy;

            base.CopyFrom(cParticleToCopy);
            Animation.CopyFrom(cParticleToCopy.Animation);
            this.TextureCoordinates = cParticleToCopy.TextureCoordinates;
        }
    }

    /// <summary>
    /// Particle used by the Default Point Sprite Particle System
    /// </summary>
    public class DefaultPointSpriteParticle : DPSFDefaultBaseParticle
    {
        // Properties used to draw the Particle
        
        /// <summary>
        /// How much the Particle should be Rotated
        /// </summary>
        public float Rotation;
        
        /// <summary>
        /// The Size (i.e. Width and Height) of the Particle.
        /// <para>NOTE: Smaller Particles are drawn MUCH quicker than larger Particles.</para>
        /// </summary>
        public float Size;
        
        // Additional Particle Properties

        /// <summary>
        /// The Particle's Rotational Velocity
        /// </summary>
        public float RotationalVelocity;

        /// <summary>
        /// The Particle's Rotational Acceleration
        /// </summary>
        public float RotationalAcceleration;
        
        /// <summary>
        /// The Particle's Size when it is born
        /// </summary>
        public float StartSize;
        
        /// <summary>
        /// The Particle's Size when it dies
        /// </summary>
        public float EndSize;

        /// <summary>
        /// Resets the Particle variables to their default values
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Rotation = 0.0f;
            Size = 10.0f;

            RotationalVelocity = RotationalAcceleration = 0.0f;
            StartSize = EndSize = 10.0f;
        }

        /// <summary>
        /// Deep copy all of the Particle properties
        /// </summary>
        /// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
        public override void CopyFrom(DPSFParticle ParticleToCopy)
        {
            // Cast the Particle to the type it really is
            DefaultPointSpriteParticle cParticleToCopy = (DefaultPointSpriteParticle)ParticleToCopy;

            base.CopyFrom(cParticleToCopy);
            this.Rotation = cParticleToCopy.Rotation;
            this.Size = cParticleToCopy.Size;

            this.RotationalVelocity = cParticleToCopy.RotationalVelocity;
            this.RotationalAcceleration = cParticleToCopy.RotationalAcceleration;
            this.StartSize = cParticleToCopy.StartSize;
            this.EndSize = cParticleToCopy.EndSize;
        }
    }

    /// <summary>
    /// Particle used by the Default Quad Particle System
    /// </summary>
    public class DefaultQuadParticle : DPSFDefaultBaseParticle
    {
        /// <summary>
        /// The Orientation of the Particle
        /// </summary>
        public Quaternion Orientation;

        /// <summary>
        /// The Rotational Velocity of the Particle.
        /// X = Pitch Velocity, Y = Yaw Velocity, Z = Roll Velocity in radians
        /// </summary>
        public Vector3 RotationalVelocity;
        /// <summary>
        /// The Rotational Acceleration of the Particle.
        /// X = Pitch Acceleration, Y = Yaw Acceleration, Z = Roll Acceleration in radians
        /// </summary>
        public Vector3 RotationalAcceleration;
        
        /// <summary>
        /// The Width of the Particle
        /// </summary>
        public float Width;

        /// <summary>
        /// The Height of the Particle
        /// </summary>
        public float Height;

        /// <summary>
        /// The Width of the Particle when it is born
        /// </summary>
        public float StartWidth;

        /// <summary>
        /// The Height of the Particle when it is born
        /// </summary>
        public float StartHeight;
        
        /// <summary>
        /// The Width of the Particle when it dies
        /// </summary>
        public float EndWidth;

        /// <summary>
        /// The Height of the Particle when it dies
        /// </summary>
        public float EndHeight;

        /// <summary>
        /// Get / Set the Normal (forward) direction of the Particle (i.e. which direction it is facing)
        /// </summary>
        public Vector3 Normal
        {
            get { return Orientation3D.GetNormalDirection(Orientation); }
            set { Orientation3D.SetNormalDirection(ref Orientation, value); }
        }

        /// <summary>
        /// Get / Set the Up direction of the Particle
        /// </summary>
        public Vector3 Up
        {
            get { return Orientation3D.GetUpDirection(Orientation); }
            set { Orientation3D.SetUpDirection(ref Orientation, value); }
        }

        /// <summary>
        /// Get / Set the Right direction of the Particle
        /// </summary>
        public Vector3 Right
        {
            get { return Orientation3D.GetRightDirection(Orientation); }
            set { Orientation3D.SetRightDirection(ref Orientation, value); }
        }

        /// <summary>
        /// Resets the Particle variables to their default values
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Orientation = Quaternion.Identity;
            RotationalVelocity = RotationalAcceleration = Vector3.Zero;
            Width = Height = 10.0f;
            StartWidth = StartHeight = EndWidth = EndHeight = 10.0f;
        }

        /// <summary>
        /// Deep copy all of the Particle properties
        /// </summary>
        /// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
        public override void CopyFrom(DPSFParticle ParticleToCopy)
        {
            // Cast the Particle to the type it really is
            DefaultQuadParticle cParticleToCopy = (DefaultQuadParticle)ParticleToCopy;

            base.CopyFrom(cParticleToCopy);
            this.Orientation = cParticleToCopy.Orientation;
            this.RotationalVelocity = cParticleToCopy.RotationalVelocity;
            this.RotationalAcceleration = cParticleToCopy.RotationalAcceleration;
            this.Width = cParticleToCopy.Width;
            this.Height = cParticleToCopy.Height;
            this.StartHeight = cParticleToCopy.StartHeight;
            this.StartWidth = cParticleToCopy.StartWidth;
            this.EndHeight = cParticleToCopy.EndHeight;
            this.EndWidth = cParticleToCopy.EndWidth;
        }
    }

    /// <summary>
    /// Particle used by the Default Textured Quad Particle System
    /// </summary>
    public class DefaultTexturedQuadParticle : DefaultQuadParticle
    { }

    /// <summary>
    /// Particle used by the Default Animated Quad Particle System
    /// </summary>
    public class DefaultAnimatedTexturedQuadParticle : DefaultTexturedQuadParticle
    {
        /// <summary>
        /// Class to hold this Particle's Animation information
        /// </summary>
        public Animations Animation;

        /// <summary>
        /// The Normalized (0.0 - 1.0) Top-Left Texture Coordinate to use for the Particle's image
        /// </summary>
        public Vector2 NormalizedTextureCoordinateLeftTop;

        /// <summary>
        /// The Normalized (0.0 - 1.0) Bottom-Right Texture Coordinate to use for the Particle's image
        /// </summary>
        public Vector2 NormalizedTextureCoordinateRightBottom;

        /// <summary>
        /// Resets the Particle variables to their default values
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Animation = new Animations();
            NormalizedTextureCoordinateLeftTop = new Vector2(0, 0);
            NormalizedTextureCoordinateRightBottom = new Vector2(1, 1);
        }

        /// <summary>
        /// Deep copy all of the Particle properties
        /// </summary>
        /// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
        public override void CopyFrom(DPSFParticle ParticleToCopy)
        {
            // Cast the Particle to the type it really is
            DefaultAnimatedTexturedQuadParticle cParticleToCopy = (DefaultAnimatedTexturedQuadParticle)ParticleToCopy;

            base.CopyFrom(ParticleToCopy);
            Animation.CopyFrom(cParticleToCopy.Animation);
            this.NormalizedTextureCoordinateLeftTop = cParticleToCopy.NormalizedTextureCoordinateLeftTop;
            this.NormalizedTextureCoordinateRightBottom = cParticleToCopy.NormalizedTextureCoordinateRightBottom;
        }
    }

    #endregion

    #region Default Particle Systems

    /// <summary>
    /// The Base Particle System class that the Default Particle System classes inherit from
    /// </summary>
    /// <typeparam name="Particle">The Particle class to use</typeparam>
    /// <typeparam name="Vertex">The Vertex Format to use</typeparam>
    public class DPSFDefaultBaseParticleSystem<Particle, Vertex> : DPSF<Particle, Vertex>
        where Particle : DPSFParticle, new()
        where Vertex : struct, IDPSFParticleVertex
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DPSFDefaultBaseParticleSystem(Game cGame) : base(cGame) { }
        
        //===========================================================
        // Structures and Variables
        //===========================================================

        /// <summary>
        /// Particle System Properties used to initialize a Particle's Properties.
        /// <para>NOTE: These are only applied to the Particle when the InitializeParticleUsingInitialProperties()
        /// function is set as the Particle Initialization Function.</para>
        /// </summary>
        public class CInitialProperties
        {
            // Min and Max properties used to set a Particle's initial properties
            public float LifetimeMin = 1.0f;
            public float LifetimeMax = 1.0f;
            public Vector3 PositionMin = Vector3.Zero;
            public Vector3 PositionMax = Vector3.Zero;
            public Vector3 VelocityMin = Vector3.Zero;
            public Vector3 VelocityMax = Vector3.Zero;
            public Vector3 AccelerationMin = Vector3.Zero;
            public Vector3 AccelerationMax = Vector3.Zero;
            public Vector3 ExternalForceMin = Vector3.Zero;
            public Vector3 ExternalForceMax = Vector3.Zero;
            public float FrictionMin = 0.0f;
            public float FrictionMax = 0.0f;
            public Color StartColorMin = Color.White;
            public Color StartColorMax = Color.White;
            public Color EndColorMin = Color.White;
            public Color EndColorMax = Color.White;

            /// <summary>
            /// If true the Position will be somewhere on the vector joining the Min Position to the Max Position.
            /// <para>If false each of the XYZ components will be randomly calculated individually between the Min and Max Position XYZ values.</para>
            /// <para>Default value is false.</para>
            /// </summary>
            public bool InterpolateBetweenMinAndMaxPosition = false;

            /// <summary>
            /// If true the Velocity will be somewhere on the vector joining the Min Velocity to the Max Velocity.
            /// <para>If false each of the XYZ components will be randomly calculated individually between the Min and Max Velocity XYZ values.</para>
            /// <para>Default value is false.</para>
            /// </summary>
            public bool InterpolateBetweenMinAndMaxVelocity = false;

            /// <summary>
            /// If true the Acceleration will be somewhere on the vector joining the Min Acceleration to the Max Acceleration.
            /// <para>If false each of the XYZ components will be randomly calculated individually between the Min and Max Acceleration XYZ values.</para>
            /// <para>Default value is false.</para>
            /// </summary>
            public bool InterpolateBetweenMinAndMaxAcceleration = false;

            /// <summary>
            /// If true the External Force will be somewhere on the vector joining the Min External Force to the Max External Force.
            /// <para>If false each of the XYZ components will be randomly calculated individually between the Min and Max External Force XYZ values.</para>
            /// <para>Default value is false.</para>
            /// </summary>
            public bool InterpolateBetweenMinAndMaxExternalForce = false;

            /// <summary>
            /// If true a Lerp'd value between the Min and Max Colors will be randomly chosen.
            /// <para>If false the RGBA component values will be randomly chosen individually between the Min and Max Color RGBA values.</para>
            /// <para>Default value is false.</para>
            /// </summary>
            public bool InterpolateBetweenMinAndMaxColors = false;

            /// <summary>
            /// If true the Emitter's Position will be added to the Particle's starting Position. For example, if the Particle is given
            /// an initial position of zero it will be placed wherever the Emitter currently is.
            /// <para>Default value is true.</para>
            /// </summary>
            public bool PositionIsAffectedByEmittersPosition = true;

            /// <summary>
            /// If true the Particle's Velocity direction will be adjusted according to the Emitter's Orientation. For example, if the
            /// Emitter is orientated to face backwards, the Particle's Velocity direction will be reversed.
            /// <para>Default value is true.</para>
            /// </summary>
            public bool VelocityIsAffectedByEmittersOrientation = true;
        }

        // The structure variable containing all Initial Properties
        private CInitialProperties mcInitialProperties = new CInitialProperties();

        // The Name of the Particle System
        private string msName = "Default";

        /// <summary>
        /// Get the Settings used to specify the Initial Properties of a new Particle.
        /// <para>NOTE: These are only applied to the Particle when the InitializeParticleUsingInitialProperties()
        /// function is set as the Particle Initialization Function.</para>
        /// </summary>
        public CInitialProperties InitialProperties
        {
            get { return mcInitialProperties; }
        }

        /// <summary>
        /// The Name of the Particle System
        /// </summary>
        public string Name
        {
            get { return msName; }
            set { msName = value; }
        }

        /// <summary>
        /// A list of Magnets that should affect this Particle System's Particles.
        /// <para>NOTE: You must add a UpdateParticleXAccordingToMagnets function to the Particle
        /// Events in order for these Magnets to affect the Particles.</para>
        /// </summary>
        public LinkedList<DefaultParticleSystemMagnet> MagnetList = new LinkedList<DefaultParticleSystemMagnet>();

        //===========================================================
        // Initialization Functions
        //===========================================================

        /// <summary>
        /// Function to Initialize a Default Particle with the Initial Settings
        /// </summary>
        /// <param name="Particle">The Particle to be Initialized</param>
        public virtual void InitializeParticleUsingInitialProperties(DPSFParticle Particle)
        {
            InitializeParticleUsingInitialProperties(Particle, mcInitialProperties);
        }

        /// <summary>
        /// Function to Initialize a Default Particle with the Initial Settings
        /// </summary>
        /// <param name="Particle">The Particle to be Initialized</param>
        /// <param name="cInitialProperties">The Initial Settings to use to Initialize the Particle</param>
        public void InitializeParticleUsingInitialProperties(DPSFParticle Particle, CInitialProperties cInitialProperties)
        {
            // Cast the Particle to the type it really is
            DPSFDefaultBaseParticle cParticle = (DPSFDefaultBaseParticle)Particle;

            // Initialize the Particle according to the values specified in the Initial Settings
            cParticle.Lifetime = DPSFHelper.RandomNumberBetween(cInitialProperties.LifetimeMin, cInitialProperties.LifetimeMax);

            // If the Position should be interpolated between the Min and Max Positions
            if (cInitialProperties.InterpolateBetweenMinAndMaxPosition)
            {
                cParticle.Position = Vector3.Lerp(cInitialProperties.PositionMin, cInitialProperties.PositionMax, RandomNumber.NextFloat());
            }
            // Else the Position XYZ values should each be calculated individually
            else
            {
                cParticle.Position = DPSFHelper.RandomVectorBetweenTwoVectors(cInitialProperties.PositionMin, cInitialProperties.PositionMax);
            }

            // If the Particle's Velocity should be affected by the Emitters Orientation
            if (cInitialProperties.VelocityIsAffectedByEmittersOrientation)
            {
                // Rotate the Particle around the Emitter according to the Emitters orientation
                cParticle.Position = Vector3.Transform(cParticle.Position, Emitter.OrientationData.Orientation);
            }

            // If the Particle should be affected by the Emitters Position
            if (cInitialProperties.PositionIsAffectedByEmittersPosition)
            {
                // Add the Emitter's Position to the Particle's Position
                cParticle.Position += Emitter.PositionData.Position;
            }

            // If the Velocity should be interpolated between the Min and Max Velocity
            if (cInitialProperties.InterpolateBetweenMinAndMaxVelocity)
            {
                cParticle.Velocity = Vector3.Lerp(cInitialProperties.VelocityMin, cInitialProperties.VelocityMax, RandomNumber.NextFloat());
            }
            // Else the Velocity XYZ values should each be calculated individually
            else
            {
                cParticle.Velocity = DPSFHelper.RandomVectorBetweenTwoVectors(cInitialProperties.VelocityMin, cInitialProperties.VelocityMax);
            }

            // Have the Emitters Rotation affect the Particle's starting Velocity
            cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);

            // If the Accleration should be interpolated between the Min and Max Accleration
            if (cInitialProperties.InterpolateBetweenMinAndMaxAcceleration)
            {
                cParticle.Acceleration = Vector3.Lerp(cInitialProperties.AccelerationMin, cInitialProperties.AccelerationMax, RandomNumber.NextFloat());
            }
            // Else the Accleration XYZ values should each be calculated individually
            else
            {
                cParticle.Acceleration = DPSFHelper.RandomVectorBetweenTwoVectors(cInitialProperties.AccelerationMin, cInitialProperties.AccelerationMax);
            }

            // If the External Force should be interpolated between the Min and Max External Force
            if (cInitialProperties.InterpolateBetweenMinAndMaxExternalForce)
            {
                cParticle.ExternalForce = Vector3.Lerp(cInitialProperties.ExternalForceMin, cInitialProperties.ExternalForceMax, RandomNumber.NextFloat());
            }
            // Else the External Force XYZ values should each be calculated individually
            else
            {
                cParticle.ExternalForce = DPSFHelper.RandomVectorBetweenTwoVectors(cInitialProperties.ExternalForceMin, cInitialProperties.ExternalForceMax);
            }

            // Calculate the amount of Friction to use
            cParticle.Friction = DPSFHelper.RandomNumberBetween(cInitialProperties.FrictionMin, cInitialProperties.FrictionMax);

            // If the new Color values should be somewhere between the interpolation of the Min and Max Colors
            if (cInitialProperties.InterpolateBetweenMinAndMaxColors)
            {
                cParticle.StartColor = DPSFHelper.LerpColor(cInitialProperties.StartColorMin, cInitialProperties.StartColorMax, RandomNumber.NextFloat());
                cParticle.EndColor = DPSFHelper.LerpColor(cInitialProperties.EndColorMin, cInitialProperties.EndColorMax, RandomNumber.NextFloat());
            }
            // Else the RGBA Color values should each be randomly calculated individually
            else
            {
                cParticle.StartColor = DPSFHelper.LerpColor(cInitialProperties.StartColorMin, cInitialProperties.StartColorMax, RandomNumber.NextFloat(), RandomNumber.NextFloat(), RandomNumber.NextFloat(), RandomNumber.NextFloat());
                cParticle.EndColor = DPSFHelper.LerpColor(cInitialProperties.EndColorMin, cInitialProperties.EndColorMax, RandomNumber.NextFloat(), RandomNumber.NextFloat(), RandomNumber.NextFloat(), RandomNumber.NextFloat());
            }
            cParticle.Color = cParticle.StartColor;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        /// <summary>
        /// Update a Particle's Position according to its Velocity
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticlePositionUsingVelocity(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Position according to its Velocity
            cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
        }

        /// <summary>
        /// Update a Particle's Velocity according to its Acceleration
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleVelocityUsingAcceleration(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Position according to its Velocity
            cParticle.Velocity += cParticle.Acceleration * fElapsedTimeInSeconds;
        }

        /// <summary>
        /// Updates a Particle's Velocity according to its Acceleration, and then the Position according
        /// to the new Velocity
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticlePositionAndVelocityUsingAcceleration(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle Velocity and Position according to Acceleration
            cParticle.Velocity += cParticle.Acceleration * fElapsedTimeInSeconds;
            cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
        }

        /// <summary>
        /// Applies the External Force to the Particle's Position
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticlePositionUsingExternalForce(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Apply the External Force to the Particle's Position
            cParticle.Position += (cParticle.ExternalForce * fElapsedTimeInSeconds);
        }

        /// <summary>
        /// Applies the External Force to the Particle's Velocity
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleVelocityUsingExternalForce(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Apply the External Force to the Particle's Velocity
            cParticle.Velocity += (cParticle.ExternalForce * fElapsedTimeInSeconds);
        }

        /// <summary>
        /// Applies the Particle's Friction to the its Velocity to slow the Particle down to a stop
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleVelocityUsingFriction(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
        {
            // If the Particle is still moving and there is Friction to apply
            if (cParticle.Velocity != Vector3.Zero && cParticle.Friction != 0.0f)
            {
                // Copy the Particle's Velocity
                Vector3 sNewVelocity = cParticle.Velocity;

                // Get the current Speed of the Particle and calculate what its new Speed should be
                float fSpeed = sNewVelocity.Length();
                fSpeed -= (cParticle.Friction * fElapsedTimeInSeconds);

                // If the Particle has slowed to a stop
                if (fSpeed <= 0.0f)
                {
                    // Stop it from moving
                    sNewVelocity = Vector3.Zero;
                }
                // Else the Particle should still be moving
                else
                {
                    // Calculate the Particle's new Velocity vector at the new Speed
                    sNewVelocity.Normalize();
                    sNewVelocity *= fSpeed;
                }

                // Make the Particle travel at the new Velocity
                cParticle.Velocity = sNewVelocity;
            }
        }

        /// <summary>
        /// Linearly interpolates the Particles Color between it's Start Color and End Color based on the 
        /// Particle's Normalized Elapsed Time
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleColorUsingLerp(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Color
            cParticle.Color = DPSFHelper.LerpColor(cParticle.StartColor, cParticle.EndColor, cParticle.NormalizedElapsedTime);
        }

        /// <summary>
        /// Quickly fades particle in when born and slowly fades it out as it gets closer to death
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Calculate how transparent the Particle should be and apply it
            byte bAlpha = DPSFHelper.FadeInQuicklyAndFadeOutSlowlyBasedOnLifetime(cParticle.NormalizedElapsedTime);
            cParticle.Color = new Color(cParticle.Color.R, cParticle.Color.G, cParticle.Color.B, bAlpha);
        }

        /// <summary>
        /// Linearly interpolates the Particles Transparency to fade out based on the Particle's Normalized Elapsed Time
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleTransparencyToFadeOutUsingLerp(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Calculate how transparent the Particle should be and apply it
            byte bAlpha = (byte)(255 - (cParticle.NormalizedElapsedTime * 255));
            cParticle.Color = new Color(cParticle.Color.R, cParticle.Color.G, cParticle.Color.B, bAlpha);
        }

        /// <summary>
        /// Linearly interpolates the Particles Transparency to fade in based on the Particle's Normalized Elapsed Time
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleTransparencyToFadeInUsingLerp(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Calculate how transparent the Particle should be and apply it
            byte bAlpha = (byte)(cParticle.NormalizedElapsedTime * 255);
            cParticle.Color = new Color(cParticle.Color.R, cParticle.Color.G, cParticle.Color.B, bAlpha);
        }

        /// <summary>
        /// Calculates how much affect each of the Particle System's Magnets should have on 
        /// this Particle and updates the Particle's Position accordingly.
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticlePositionAccordingToMagnets(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Temp handle to a Magnet
            DefaultParticleSystemMagnet cMagnet = null;

            // Loop through each of the Particle System's Magnets
            LinkedListNode<DefaultParticleSystemMagnet> cNode = MagnetList.First;
            while (cNode != null)
            {
                // Get a handle to this Magnet
                cMagnet = (DefaultParticleSystemMagnet)cNode.Value;

                // If this is not a custom user Magnet (i.e. it is Attracting or Repelling)
                if (cMagnet.Mode != DefaultParticleSystemMagnet.MagnetModes.Other)
                {
                    // Apply the Force to the Particle's Position
                    cParticle.Position += (CalculateForceMagnetShouldExertOnParticle(cMagnet, cParticle) * fElapsedTimeInSeconds);
                }

                // Move to the next Magnet in the list
                cNode = cNode.Next;
            }
        }

        /// <summary>
        /// Calculates how much affect each of the Particle System's Magnets should have on 
        /// this Particle and updates the Particle's Velocity accordingly.
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleVelocityAccordingToMagnets(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Temp handle to a Magnet
            DefaultParticleSystemMagnet cMagnet = null;

            // Loop through each of the Particle System's Magnets
            LinkedListNode<DefaultParticleSystemMagnet> cNode = MagnetList.First;
            while (cNode != null)
            {
                // Get a handle to this Magnet
                cMagnet = (DefaultParticleSystemMagnet)cNode.Value;

                // If this is not a custom user Magnet (i.e. it is Attracting or Repelling)
                if (cMagnet.Mode != DefaultParticleSystemMagnet.MagnetModes.Other)
                {
                    // Apply the Force to the Particle's Position
                    cParticle.Velocity += (CalculateForceMagnetShouldExertOnParticle(cMagnet, cParticle) * fElapsedTimeInSeconds);
                }

                // Move to the next Magnet in the list
                cNode = cNode.Next;
            }
        }

        /// <summary>
        /// Returns the vector force that a Magnet should exert on a Particle
        /// </summary>
        /// <param name="cMagnet">The Magnet affecting the Particle</param>
        /// <param name="cParticle">The Particle being affected by the Magnet</param>
        /// <returns>Returns the vector force that a Magnet should exert on a Particle</returns>
        private Vector3 CalculateForceMagnetShouldExertOnParticle(DefaultParticleSystemMagnet cMagnet, DPSFDefaultBaseParticle cParticle)
        {
            // Variable to store the Force to Exert on the Particle
            Vector3 sForceToExertOnParticle = Vector3.Zero;

            // Calculate which Direction to push the Particle
            Vector3 sDirectionToPushParticle;

            // If the Particle should be Attracted to the Magnet
            if (cMagnet.Mode == DefaultParticleSystemMagnet.MagnetModes.Attract)
            {
                sDirectionToPushParticle = cMagnet.PositionData.Position - cParticle.Position;
            }
            // Else the Particle should be Repelled away from the Magnet
            else
            {
                sDirectionToPushParticle = cParticle.Position - cMagnet.PositionData.Position;
            }

            // If the Direction To Push the Particle is not valid and we should be Repelling the Particle
            if (sDirectionToPushParticle == Vector3.Zero && cMagnet.Mode == DefaultParticleSystemMagnet.MagnetModes.Repel)
            {
                // Pick a random Direction vector with a very short length to repel the Particle with
                sDirectionToPushParticle = DPSFHelper.RandomNormalizedVector() * 0.00001f;
            }

            // Get how far away the Particle is from the Magnet
            float fDistanceFromMagnet = sDirectionToPushParticle.Length();
            
            // If the Particle is within range to be affected by the Magnet
            if (fDistanceFromMagnet >= cMagnet.MinDistance && fDistanceFromMagnet <= cMagnet.MaxDistance)
            {
                // If the Direction To Push the Particle is valid
                if (sDirectionToPushParticle != Vector3.Zero)
                {
                    // Normalize the Direction To Push the Particle
                    sDirectionToPushParticle.Normalize();
                }

                // Calculate the normalized distance from the Magnet that the Particle is
                float fLerpAmount = 0.0f;
                if (cMagnet.MaxDistance != cMagnet.MinDistance)
                {
                    fLerpAmount = (fDistanceFromMagnet - cMagnet.MinDistance) / (cMagnet.MaxDistance - cMagnet.MinDistance);
                }
                // Else the Max Distance equals the Min Distance
                else
                {
                    // So to avoid a divide by zero we just assume a full Lerp amount
                    fLerpAmount = 1.0f;
                }

                // Calculate how much of the Max Force to apply to the Particle
                float fNormalizedForce = 0.0f;
                switch (cMagnet.DistanceFunction)
                {
                    default:
                    case DefaultParticleSystemMagnet.DistanceFunctions.Constant:
                        fNormalizedForce = cMagnet.MaxForce;
                    break;

                    case DefaultParticleSystemMagnet.DistanceFunctions.Linear:
                        fNormalizedForce = MathHelper.Lerp(0, cMagnet.MaxForce, fLerpAmount);
                    break;

                    case DefaultParticleSystemMagnet.DistanceFunctions.Squared:
                        fNormalizedForce = MathHelper.Lerp(0, cMagnet.MaxForce, fLerpAmount * fLerpAmount);
                    break;

                    case DefaultParticleSystemMagnet.DistanceFunctions.Cubed:
                        fNormalizedForce = MathHelper.Lerp(0, cMagnet.MaxForce, fLerpAmount * fLerpAmount * fLerpAmount);
                    break;

                    case DefaultParticleSystemMagnet.DistanceFunctions.LinearInverse:
                        fNormalizedForce = MathHelper.Lerp(cMagnet.MaxForce, 0, fLerpAmount);
                    break;

                    case DefaultParticleSystemMagnet.DistanceFunctions.SquaredInverse:
                        fNormalizedForce = MathHelper.Lerp(cMagnet.MaxForce, 0, fLerpAmount * fLerpAmount);
                    break;

                    case DefaultParticleSystemMagnet.DistanceFunctions.CubedInverse:
                        fNormalizedForce = MathHelper.Lerp(cMagnet.MaxForce, 0, fLerpAmount * fLerpAmount * fLerpAmount);
                    break;
                }

                // Calculate how much Force should be Exerted on the Particle
                sForceToExertOnParticle = sDirectionToPushParticle * (fNormalizedForce * cMagnet.MaxForce);
            }

            // Return how much Force to Exert on the Particle
            return sForceToExertOnParticle;
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        /// <summary>
        /// Sets the Emitter to Emit Particles Automatically
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleSystemEmitParticlesAutomaticallyOn(float fElapsedTimeInSeconds)
        {
            Emitter.EmitParticlesAutomatically = true;
        }

        /// <summary>
        /// Sets the Emitter to not Emit Particles Automatically
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleSystemEmitParticlesAutomaticallyOff(float fElapsedTimeInSeconds)
        {
            Emitter.EmitParticlesAutomatically = false;
        }

        /// <summary>
        /// Enables the Emitter
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleSystemEnableEmitter(float fElapsedTimeInSeconds)
        {
            Emitter.Enabled = true;
        }

        /// <summary>
        /// Disables the Emitter
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleSystemDisableEmitter(float fElapsedTimeInSeconds)
        {
            Emitter.Enabled = false;
        }
    }

    /// <summary>
    /// The Default Sprite Particle System class
    /// </summary>
    /// <typeparam name="Particle">The Particle class to use</typeparam>
    /// <typeparam name="Vertex">The Vertex Format to use</typeparam>
    public class DPSFDefaultPixelParticleSystem<Particle, Vertex> : DPSFDefaultBaseParticleSystem<Particle, Vertex>
        where Particle : DPSFParticle, new()
        where Vertex : struct, IDPSFParticleVertex
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DPSFDefaultPixelParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================


        //===========================================================
        // Vertex Update and Overridden Particle System Functions
        //===========================================================

        /// <summary>
        /// Function to update the Vertex properties according to the Particle properties
        /// </summary>
        /// <param name="sVertexBuffer">The array containing the Vertices to be drawn</param>
        /// <param name="iIndex">The Index in the array where the Particle's Vertex info should be placed</param>
        /// <param name="Particle">The Particle to copy the information from</param>
        public virtual void UpdateVertexProperties(ref DefaultPixelParticleVertex[] sVertexBuffer, int iIndex, DPSFParticle Particle)
        {
            // Cast the Particle to the type it really is
            DefaultPixelParticle cParticle = (DefaultPixelParticle)Particle;

            // Copy this Particle's renderable Properties to the Vertex Buffer
            sVertexBuffer[iIndex].Position = cParticle.Position;
            sVertexBuffer[iIndex].Color = cParticle.Color;
        }

        /// <summary>
        /// Virtual function to Set the Effect's Parameters before drawing the Particles
        /// </summary>
        protected override void SetEffectParameters()
        {
            // Specify the World, View, and Projection Matrices
            Effect.Parameters["xWorld"].SetValue(World);
            Effect.Parameters["xView"].SetValue(View);
            Effect.Parameters["xProjection"].SetValue(Projection);
        }

        //===========================================================
        // Initialization Functions
        //===========================================================



        //===========================================================
        // Particle Update Functions
        //===========================================================

        /// <summary>
        /// Linearly interpolate the Particle's Position.Z value from 1.0 (back) to
        /// 0.0 (front) according to the Particle's Normalized Lifetime
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleDepthFromBackToFrontUsingLerp(DefaultPixelParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Depth Position
            cParticle.Position.Z = (1.0f - cParticle.NormalizedElapsedTime);
        }

        /// <summary>
        /// Linearly interpolate the Particle's Position.Z value from 0.0 (front) to
        /// 1.0 (back) according to the Particle's Normalized Lifetime
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleDepthFromFrontToBackUsingLerp(DefaultPixelParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Depth Position
            cParticle.Position.Z = (cParticle.NormalizedElapsedTime);
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================
    }

    /// <summary>
    /// The Default Sprite Particle System class
    /// </summary>
    /// <typeparam name="Particle">The Particle class to use</typeparam>
    /// <typeparam name="Vertex">The Vertex Format to use</typeparam>
    public class DPSFDefaultSpriteParticleSystem<Particle, Vertex> : DPSFDefaultBaseParticleSystem<Particle, Vertex>
        where Particle : DPSFParticle, new()
        where Vertex : struct, IDPSFParticleVertex
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DPSFDefaultSpriteParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        /// <summary>
        /// Particle System Properties used to initialize a Particle's Properties.
        /// <para>NOTE: These are only applied to the Particle when the InitializeParticleUsingInitialProperties()
        /// function is set as the Particle Initialization Function.</para>
        /// </summary>
        public class CInitialPropertiesForSprite : CInitialProperties
        {
            // Min and Max properties used to set a Particle's initial Rotation properties
            public float RotationMin = 0.0f;
            public float RotationMax = 0.0f;
            public float RotationalVelocityMin = 0.0f;
            public float RotationalVelocityMax = 0.0f;
            public float RotationalAccelerationMin = 0.0f;
            public float RotationalAccelerationMax = 0.0f;

            // Main and Max properties used to set a Particle's Width and Height properties
            public float StartWidthMin = 10.0f;
            public float StartWidthMax = 10.0f;
            public float StartHeightMin = 10.0f;
            public float StartHeightMax = 10.0f;
            public float EndWidthMin = 10.0f;
            public float EndWidthMax = 10.0f;
            public float EndHeightMin = 10.0f;
            public float EndHeightMax = 10.0f;
        }

        // The structure variable containing all Initial Properties
        private CInitialPropertiesForSprite mcInitialProperties = new CInitialPropertiesForSprite();

        /// <summary>
        /// Get the Settings used to specify the Initial Properties of a new Particle.
        /// <para>NOTE: These are only applied to the Particle when the InitializeParticleUsingInitialProperties()
        /// function is set as the Particle Initialization Function.</para>
        /// </summary>
        public new CInitialPropertiesForSprite InitialProperties
        {
            get { return mcInitialProperties; }
        }

        //===========================================================
        // Draw Sprite and Overridden Particle System Functions
        //===========================================================

        /// <summary>
        /// Function to draw a Sprite Particle. This function should be used to draw the given
        /// Particle with the provided SpriteBatch.
        /// </summary>
        /// <param name="Particle">The Particle Sprite to Draw</param>
        /// <param name="cSpriteBatch">The SpriteBatch to use to doing the Drawing</param>
        protected override void DrawSprite(DPSFParticle Particle, SpriteBatch cSpriteBatch)
        {
            // Cast the Particle to the type it really is
            DefaultSpriteParticle cParticle = (DefaultSpriteParticle)Particle;

            // Get the Position and Dimensions of the Particle in 2D space
            Rectangle sDestination = new Rectangle((int)cParticle.Position.X, (int)cParticle.Position.Y,
                                                   (int)cParticle.Width, (int)cParticle.Height);

            // Get the Depth (Z-Buffer value) of the Particle clamped in the range 0.0 - 1.0 (0.0 = front, 1.0 = back)
            float fNormalizedDepth = MathHelper.Clamp(cParticle.Position.Z, 0.0f, 1.0f);

            // Get the Position and Dimensions from the Texture to use for this Sprite
            Rectangle sSourceFromTexture = new Rectangle(0, 0, Texture.Width, Texture.Height);

            // Make the Sprite rotate about its center
            Vector2 sOrigin = new Vector2(sSourceFromTexture.Width / 2, sSourceFromTexture.Height / 2);

            // Draw the Sprite
            cSpriteBatch.Draw(Texture, sDestination, sSourceFromTexture, cParticle.Color, cParticle.Rotation, sOrigin, cParticle.FlipMode, fNormalizedDepth);
        }

        //===========================================================
        // Initialization Function
        //===========================================================

        /// <summary>
        /// Function to Initialize a Default Particle with default Properties
        /// </summary>
        /// <param name="Particle">The Particle to be Initialized</param>
        public override void InitializeParticleUsingInitialProperties(DPSFParticle Particle)
        {
            // Cast the Particle to the type it really is
            DefaultSpriteParticle cParticle = (DefaultSpriteParticle)Particle;

            // Initialize the Particle according to the values specified in the Initial Settings
            base.InitializeParticleUsingInitialProperties(cParticle, mcInitialProperties);

            // Calculate the Particle's Rotation values
            cParticle.Rotation = DPSFHelper.RandomNumberBetween(mcInitialProperties.RotationMin, mcInitialProperties.RotationMax);
            cParticle.RotationalVelocity = DPSFHelper.RandomNumberBetween(mcInitialProperties.RotationalVelocityMin, mcInitialProperties.RotationalVelocityMax);
            cParticle.RotationalAcceleration = DPSFHelper.RandomNumberBetween(mcInitialProperties.RotationalAccelerationMin, mcInitialProperties.RotationalAccelerationMax);

            // Calculate the Particle's Width and Height values
            cParticle.StartWidth = DPSFHelper.RandomNumberBetween(mcInitialProperties.StartWidthMin, mcInitialProperties.StartWidthMax);
            cParticle.EndWidth = DPSFHelper.RandomNumberBetween(mcInitialProperties.EndWidthMin, mcInitialProperties.EndWidthMax);
            cParticle.StartHeight = DPSFHelper.RandomNumberBetween(mcInitialProperties.StartHeightMin, mcInitialProperties.StartHeightMax);
            cParticle.EndHeight = DPSFHelper.RandomNumberBetween(mcInitialProperties.EndHeightMin, mcInitialProperties.EndHeightMax);
            cParticle.Width = cParticle.StartWidth;
            cParticle.Height = cParticle.StartHeight;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        /// <summary>
        /// Update a Particle's Rotation according to its Rotational Velocity
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleRotationUsingRotationalVelocity(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Rotation according to its Rotational Velocity
            cParticle.Rotation += cParticle.RotationalVelocity * fElapsedTimeInSeconds;
        }

        /// <summary>
        /// Update a Particle's Rotational Velocity according to its Rotational Acceleration
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleRotationalVelocityUsingRotationalAcceleration(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Rotation according to its Rotational Velocity
            cParticle.RotationalVelocity += cParticle.RotationalAcceleration * fElapsedTimeInSeconds;
        }

        /// <summary>
        /// Update a Particle's Rotation and Rotational Velocity according to its Rotational Acceleration
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Rotaional Velocity and Rotation according to its Rotational Acceleration
            cParticle.RotationalVelocity += cParticle.RotationalAcceleration * fElapsedTimeInSeconds;
            cParticle.Rotation += cParticle.RotationalVelocity * fElapsedTimeInSeconds;
        }

        /// <summary>
        /// Linearly interpolate the Particle's Width between the Start and End Width according
        /// to the Particle's Normalized Lifetime
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleWidthUsingLerp(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Calculate the Particle's new Width
            cParticle.Width = MathHelper.Lerp(cParticle.StartWidth, cParticle.EndWidth, cParticle.NormalizedElapsedTime);
        }

        /// <summary>
        /// Linearly interpolate the Particle's Height between the Start and End Height according
        /// to the Particle's Normalized Lifetime
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleHeightUsingLerp(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Calculate the Particle's new Height
            cParticle.Height = MathHelper.Lerp(cParticle.StartHeight, cParticle.EndHeight, cParticle.NormalizedElapsedTime);
        }

        /// <summary>
        /// Linearly interpolate the Particle's Width and Height between the Start and End values according
        /// to the Particle's Normalized Lifetime
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleWidthAndHeightUsingLerp(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Calculate the Particle's new Width and Height
            cParticle.Width = MathHelper.Lerp(cParticle.StartWidth, cParticle.EndWidth, cParticle.NormalizedElapsedTime);
            cParticle.Height = MathHelper.Lerp(cParticle.StartHeight, cParticle.EndHeight, cParticle.NormalizedElapsedTime);
        }

        /// <summary>
        /// Linearly interpolate the Particle's Position.Z value from 1.0 (back) to
        /// 0.0 (front) according to the Particle's Normalized Lifetime
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleDepthFromBackToFrontUsingLerp(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Depth Position
            cParticle.Position.Z = (1.0f - cParticle.NormalizedElapsedTime);
        }

        /// <summary>
        /// Linearly interpolate the Particle's Position.Z value from 0.0 (front) to
        /// 1.0 (back) according to the Particle's Normalized Lifetime
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleDepthFromFrontToBackUsingLerp(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Depth Position
            cParticle.Position.Z = (cParticle.NormalizedElapsedTime);
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        /// <summary>
        /// Sorts the Particle System's Active Particles so that the Particles at the back
        /// (i.e. Position.Z = 1.0) are drawn before the Particles at the front (i.e. 
        /// Position.Z = 0.0).
        /// <para>NOTE: This operation is very expensive and should only be used when you are
        /// using a Shader (i.e. Effect and Technique) and have the 
        /// SpriteBatchOptions.eSortMode set to SpriteSortMode.Immediate (as this is the
        /// only mode which allows a Shader to be used with SpriteBatch).</para>
        /// <para>If you are not using a Shader and want the Particles sorted by Depth, use SpriteSortMode.BackToFront.</para>
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleSystemToSortParticlesByDepth(float fElapsedTimeInSeconds)
        {
            Array.Sort<Particle>(Particles, 0, NumberOfActiveParticles, new ParticleComparerByDepth());
        }

        /// <summary>
        /// Class that implements the IComperer interface to compare two Particles by Depth
        /// </summary>
        private class ParticleComparerByDepth : IComparer<Particle>
        {
            /// <summary>
            /// Compares the Depth value (i.e. Position.Z value) of two DPSF Default Base Particles
            /// </summary>
            /// <param name="Particle1">The first Particle to compare</param>
            /// <param name="Particle2">The second Particle to compare</param>
            /// <returns>Returns an integer specifying if Particle1 is greater than Particle2 or not</returns>
            public int Compare(Particle Particle1, Particle Particle2)
            {
                DPSFDefaultBaseParticle cParticle1 = (DPSFDefaultBaseParticle)(DPSFParticle)Particle1;
                DPSFDefaultBaseParticle cParticle2 = (DPSFDefaultBaseParticle)(DPSFParticle)Particle2;
                return cParticle1.Position.Z.CompareTo(cParticle2.Position.Z);
            }
        }
    }

    /// <summary>
    /// The Default Animated Sprite Particle System class
    /// </summary>
    /// <typeparam name="Particle">The Particle class to use</typeparam>
    /// <typeparam name="Vertex">The Vertex Format to use</typeparam>
    public class DPSFDefaultAnimatedSpriteParticleSystem<Particle, Vertex> : DPSFDefaultSpriteParticleSystem<Particle, Vertex>
        where Particle : DPSFParticle, new()
        where Vertex : struct, IDPSFParticleVertex
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DPSFDefaultAnimatedSpriteParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Draw Sprite and Overridden Particle System Functions
        //===========================================================

        /// <summary>
        /// Function to draw a Sprite Particle. This function should be used to draw the given
        /// Particle with the provided SpriteBatch.
        /// </summary>
        /// <param name="Particle">The Particle Sprite to Draw</param>
        /// <param name="cSpriteBatch">The SpriteBatch to use to doing the Drawing</param>
        protected override void DrawSprite(DPSFParticle Particle, SpriteBatch cSpriteBatch)
        {
            // Cast the Particle to the type it really is
            DefaultAnimatedSpriteParticle cParticle = (DefaultAnimatedSpriteParticle)Particle;

            // Get the Position and Dimensions of the Particle in 2D space
            Rectangle sDestination = new Rectangle((int)cParticle.Position.X, (int)cParticle.Position.Y,
                                                   (int)cParticle.Width, (int)cParticle.Height);

            // Get the Depth (Z-Buffer value) of the Particle clamped in the range 0.0 - 1.0 (0.0 = front, 1.0 = back)
            float fNormalizedDepth = MathHelper.Clamp(cParticle.Position.Z, 0.0f, 1.0f);

            // Get the Position and Dimensions from the Texture to use for this Sprite
            Rectangle sSourceFromTexture = cParticle.TextureCoordinates;

            // Make the Sprite rotate about its center
            Vector2 sOrigin = new Vector2(sSourceFromTexture.Width / 2, sSourceFromTexture.Height / 2);

            // Draw the Sprite
            cSpriteBatch.Draw(Texture, sDestination, sSourceFromTexture, cParticle.Color, cParticle.Rotation, sOrigin, cParticle.FlipMode, fNormalizedDepth);
        }


        //===========================================================
        // Particle Update Functions
        //===========================================================

        /// <summary>
        /// Updates the Animation, as well as the Particle's Texture Coordinates to match the Animation
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleAnimationAndTextureCoordinates(DefaultAnimatedSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Animation
            cParticle.Animation.Update(fElapsedTimeInSeconds);

            // Get the Particle's Texture Coordinates to use
            cParticle.TextureCoordinates = cParticle.Animation.CurrentPicturesTextureCoordinates;
        }

        /// <summary>
        /// Updates the Particle to be removed from the Particle System once the Animation finishes Playing
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleToDieOnceAnimationFinishesPlaying(DefaultAnimatedSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // If the Animation has finished Playing
            if (cParticle.Animation.CurrentAnimationIsDonePlaying)
            {
                // Make sure the Lifetime is greater than zero
                // We make it a small value to try and keep from triggering any Timed Events by accident
                cParticle.Lifetime = 0.000001f;

                // Set the Particle to die
                cParticle.NormalizedElapsedTime = 1.0f;
            }
        }
    }

    /// <summary>
    /// The Default Point Sprite Particle System class
    /// </summary>
    /// <typeparam name="Particle">The Particle class to use</typeparam>
    /// <typeparam name="Vertex">The Vertex Format to use</typeparam>
    public class DPSFDefaultPointSpriteParticleSystem<Particle, Vertex> : DPSFDefaultBaseParticleSystem<Particle, Vertex> 
        where Particle : DPSFParticle, new()
        where Vertex : struct, IDPSFParticleVertex
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DPSFDefaultPointSpriteParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        /// <summary>
        /// Particle System Properties used to initialize a Particle's Properties.
        /// <para>NOTE: These are only applied to the Particle when the InitializeParticleUsingInitialProperties()
        /// function is set as the Particle Initialization Function.</para>
        /// </summary>
        public class CInitialPropertiesForPointSprite : CInitialProperties
        {
            // Min and Max properties used to set a Particle's initial properties
            public float RotationMin = 0.0f;
            public float RotationMax = 0.0f;
            public float RotationalVelocityMin = 0.0f;
            public float RotationalVelocityMax = 0.0f;
            public float RotationalAccelerationMin = 0.0f;
            public float RotationalAccelerationMax = 0.0f;
            public float StartSizeMin = 10.0f;
            public float StartSizeMax = 10.0f;
            public float EndSizeMin = 10.0f;
            public float EndSizeMax = 10.0f;
        }

        // The structure variable containing all Initial Properties
        private CInitialPropertiesForPointSprite mcInitialProperties = new CInitialPropertiesForPointSprite();

        /// <summary>
        /// Get the Settings used to specify the Initial Properties of a new Particle.
        /// <para>NOTE: These are only applied to the Particle when the InitializeParticleUsingInitialProperties()
        /// function is set as the Particle Initialization Function.</para>
        /// </summary>
        public new CInitialPropertiesForPointSprite InitialProperties
        {
            get { return mcInitialProperties; }
        }


        //===========================================================
        // Vertex Update and Overridden Particle System Functions
        //===========================================================

        /// <summary>
        /// Function to update the Vertex properties according to the Particle properties
        /// </summary>
        /// <param name="sVertexBuffer">The array containing the Vertices to be drawn</param>
        /// <param name="iIndex">The Index in the array where the Particle's Vertex info should be placed</param>
        /// <param name="Particle">The Particle to copy the information from</param>
        public virtual void UpdateVertexProperties(ref DefaultPointSpriteParticleVertex[] sVertexBuffer, int iIndex, DPSFParticle Particle)
        {
            // Cast the Particle to the type it really is
            DefaultPointSpriteParticle cParticle = (DefaultPointSpriteParticle)Particle;

            // Copy this Particle's renderable Properties to the Vertex Buffer
            sVertexBuffer[iIndex].Position = cParticle.Position;
            sVertexBuffer[iIndex].Size = cParticle.Size;
            sVertexBuffer[iIndex].Rotation = cParticle.Rotation;
            sVertexBuffer[iIndex].Color = cParticle.Color;
        }

        /// <summary>
        /// Virtual function to Set the Graphics Device properties for rendering the Particles
        /// </summary>
        /// <param name="cRenderState">The Render State of the Graphics Device to change</param>
        protected override void SetRenderState(RenderState cRenderState)
        {
            // Enable point sprites
            cRenderState.PointSpriteEnable = true;
            cRenderState.PointSizeMax = 256;

            // Set the alpha blend mode
            cRenderState.AlphaBlendEnable = true;
            cRenderState.AlphaBlendOperation = BlendFunction.Add;
            cRenderState.SourceBlend = Blend.SourceAlpha;
            cRenderState.DestinationBlend = Blend.InverseSourceAlpha;

            // Set the alpha test mode
            cRenderState.AlphaTestEnable = true;
            cRenderState.AlphaFunction = CompareFunction.Greater;
            cRenderState.ReferenceAlpha = 0;

            // Enable the depth buffer (so particles will not be visible through
            // solid objects like the ground plane), but disable depth writes
            // (so particles will not obscure other particles).
            cRenderState.DepthBufferEnable = true;
            cRenderState.DepthBufferWriteEnable = false;
        }

        /// <summary>
        /// Virtual function to Reset the Graphics Device properties after rendering the Particles
        /// </summary>
        /// <param name="cRenderState">The Render State of the Graphics Device to change</param>
        protected override void ResetRenderState(RenderState cRenderState)
        {
            // Reset the more unusual Render States that we changed,
            // so as not to mess up any other subsequent drawing
            cRenderState.PointSpriteEnable = false;
            cRenderState.AlphaBlendEnable = false;
            cRenderState.AlphaTestEnable = false;
            cRenderState.DepthBufferWriteEnable = true;
        }

        /// <summary>
        /// Virtual function to Set the Effect's Parameters before drawing the Particles
        /// </summary>
        protected override void SetEffectParameters()
        {
            // Specify the World, View, and Projection Matrices
            Effect.Parameters["xWorld"].SetValue(World);
            Effect.Parameters["xView"].SetValue(View);
            Effect.Parameters["xProjection"].SetValue(Projection);

            // Also specify the Viewport Height as it's used in determining a 
            // Point Sprite's Size relative to how far it is from the Camera
            Effect.Parameters["xViewportHeight"].SetValue(GraphicsDevice.Viewport.Height);

            // Specify the Texture to use
            Effect.Parameters["xTexture"].SetValue(Texture);
        }

        //===========================================================
        // Initialization Functions
        //===========================================================

        /// <summary>
        /// Function to Initialize a Default Particle with the Initial Properties
        /// </summary>
        /// <param name="Particle">The Particle to be Initialized</param>
        public override void InitializeParticleUsingInitialProperties(DPSFParticle Particle)
        {
            // Cast the Particle to the type it really is
            DefaultPointSpriteParticle cParticle = (DefaultPointSpriteParticle)Particle;

            // Initialize the Particle according to the values specified in the Initial Settings
            base.InitializeParticleUsingInitialProperties(cParticle, mcInitialProperties);

            cParticle.Rotation = DPSFHelper.RandomNumberBetween(mcInitialProperties.RotationMin, mcInitialProperties.RotationMax);
            cParticle.RotationalVelocity = DPSFHelper.RandomNumberBetween(mcInitialProperties.RotationalVelocityMin, mcInitialProperties.RotationalVelocityMax);
            cParticle.RotationalAcceleration = DPSFHelper.RandomNumberBetween(mcInitialProperties.RotationalAccelerationMin, mcInitialProperties.RotationalAccelerationMax);

            cParticle.StartSize = DPSFHelper.RandomNumberBetween(mcInitialProperties.StartSizeMin, mcInitialProperties.StartSizeMax);
            cParticle.EndSize = DPSFHelper.RandomNumberBetween(mcInitialProperties.EndSizeMin, mcInitialProperties.EndSizeMax);
            cParticle.Size = cParticle.StartSize;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        /// <summary>
        /// Update a Particle's Rotation according to its Rotational Velocity
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleRotationUsingRotationalVelocity(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Rotation according to its Rotational Velocity
            cParticle.Rotation += cParticle.RotationalVelocity * fElapsedTimeInSeconds;
        }

        /// <summary>
        /// Update a Particle's Rotational Velocity according to its Rotational Acceleration
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleRotationalVelocityUsingRotationalAcceleration(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Rotaional Velocity according to its Rotational Acceleration
            cParticle.RotationalVelocity += cParticle.RotationalAcceleration * fElapsedTimeInSeconds;
        }

        /// <summary>
        /// Update a Particle's Rotation and Rotational Velocity according to its Rotational Acceleration
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Rotaional Velocity and Rotation according to its Rotational Acceleration
            cParticle.RotationalVelocity += cParticle.RotationalAcceleration * fElapsedTimeInSeconds;
            cParticle.Rotation += cParticle.RotationalVelocity * fElapsedTimeInSeconds;
        }

        /// <summary>
        /// Linearly interpolates the Particles Size between it's Start Size and End Size based on the 
        /// Particle's Normalized Elapsed Time
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleSizeUsingLerp(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Size
            cParticle.Size = MathHelper.Lerp(cParticle.StartSize, cParticle.EndSize, cParticle.NormalizedElapsedTime);
        }
    }

    /// <summary>
    /// The Default Quad Particle System class
    /// </summary>
    /// <typeparam name="Particle">The Particle class to use</typeparam>
    /// <typeparam name="Vertex">The Vertex Format to use</typeparam>
    public class DPSFDefaultQuadParticleSystem<Particle, Vertex> : DPSFDefaultBaseParticleSystem<Particle, Vertex>
        where Particle : DPSFParticle, new()
        where Vertex : struct, IDPSFParticleVertex
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DPSFDefaultQuadParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        /// <summary>
        /// Particle System Properties used to initialize a Particle's Properties.
        /// <para>NOTE: These are only applied to the Particle when the InitializeParticleUsingInitialProperties()
        /// function is set as the Particle Initialization Function.</para>
        /// </summary>
        public class CInitialPropertiesForQuad : CInitialProperties
        {
            // Min and Max properties used to set a Particle's initial Rotation properties
            public Vector3 RotationMin = Vector3.Zero;
            public Vector3 RotationMax = Vector3.Zero;
            public Vector3 RotationalVelocityMin = Vector3.Zero;
            public Vector3 RotationalVelocityMax = Vector3.Zero;
            public Vector3 RotationalAccelerationMin = Vector3.Zero;
            public Vector3 RotationalAccelerationMax = Vector3.Zero;

            // Main and Max properties used to set a Particle's Width and Height properties
            public float StartWidthMin = 10.0f;
            public float StartWidthMax = 10.0f;
            public float StartHeightMin = 10.0f;
            public float StartHeightMax = 10.0f;
            public float EndWidthMin = 10.0f;
            public float EndWidthMax = 10.0f;
            public float EndHeightMin = 10.0f;
            public float EndHeightMax = 10.0f;

            /// <summary>
            /// If true, the Rotation will be somewhere on the vector joining the Min Rotation to the Max Rotation.
            /// <para>If false, each of the XYZ components will be randomly calculated individually between the Min and Max Rotation XYZ values.</para>
            /// <para>Default value is false.</para>
            /// </summary>
            public bool InterpolateBetweenMinAndMaxRotation = false;

            /// <summary>
            /// If true, the Rotational Velocity will be somewhere on the vector joining the Min Rotational Velocity to the Max Rotational Velocity.
            /// <para>If false, each of the XYZ components will be randomly calculated individually between the Min and Max Rotational Velocity XYZ values.</para>
            /// <para>Default value is false.</para>
            /// </summary>
            public bool InterpolateBetweenMinAndMaxRotationalVelocity = false;

            /// <summary>
            /// If true, the Rotational Acceleration will be somewhere on the vector joining the Min Rotational Acceleration to the Max Rotational Acceleration.
            /// <para>If false, each of the XYZ components will be randomly calculated individually between the Min and Max Rotational Acceleration XYZ values.</para>
            /// <para>Default value is false.</para>
            /// </summary>
            public bool InterpolateBetweenMinAndMaxRotationalAcceleration = false;
        }

        // The structure variable containing all Initial Properties
        private CInitialPropertiesForQuad mcInitialProperties = new CInitialPropertiesForQuad();

        // Holds the Position of the Camera, which is used to Billboard the Particles
        private Vector3 msCameraPosition = Vector3.Zero;

        /// <summary>
        /// Get the Settings used to specify the Initial Properties of a new Particle.
        /// <para>NOTE: These are only applied to the Particle when the InitializeParticleUsingInitialProperties()
        /// function is set as the Particle Initialization Function.</para>
        /// </summary>
        public new CInitialPropertiesForQuad InitialProperties
        {
            get { return mcInitialProperties; }
        }

        /// <summary>
        /// Get / Set the Position of the Camera.
        /// <para>NOTE: This should be Set (updated) every frame if Billboarding will be used 
        /// (i.e. Always have the Particles face the Camera).</para>
        /// </summary>
        public Vector3 CameraPosition
        {
            get { return msCameraPosition; }
            set { msCameraPosition = value; }
        }

        //===========================================================
        // Vertex Update and Overridden Particle System Functions
        //===========================================================

        /// <summary>
        /// Function to update the Vertex properties according to the Particle properties
        /// </summary>
        /// <param name="sVertexBuffer">The array containing the Vertices to be drawn</param>
        /// <param name="iIndex">The Index in the array where the Particle's Vertex info should be placed</param>
        /// <param name="Particle">The Particle to copy the information from</param>
        public virtual void UpdateVertexProperties(ref DefaultQuadParticleVertex[] sVertexBuffer, int iIndex, DPSFParticle Particle)
        {
            // Cast the Particle to the type it really is
            DefaultQuadParticle cParticle = (DefaultQuadParticle)Particle;

            // Calculate what half of the Quads Width and Height are
            int iHalfWidth = (int)(cParticle.Width / 2.0f);
            int iHalfHeight = (int)(cParticle.Height / 2.0f);

            // Calculate the Positions of the Quads corners around the origin
            Vector3 sTopLeft = new Vector3(-iHalfWidth, iHalfHeight, 0);
            Vector3 sTopRight = new Vector3(iHalfWidth, iHalfHeight, 0);
            Vector3 sBottomLeft = new Vector3(-iHalfWidth, -iHalfHeight, 0);
            Vector3 sBottomRight = new Vector3(iHalfWidth, -iHalfHeight, 0);

            // Rotate the Quad corners around the origin according to its Orientation, 
            // then calculate their final Positions
            sTopLeft = Vector3.Transform(sTopLeft, cParticle.Orientation) + cParticle.Position;
            sTopRight = Vector3.Transform(sTopRight, cParticle.Orientation) + cParticle.Position;
            sBottomLeft = Vector3.Transform(sBottomLeft, cParticle.Orientation) + cParticle.Position;
            sBottomRight = Vector3.Transform(sBottomRight, cParticle.Orientation) + cParticle.Position;

            // Copy this Particle's renderable Properties to the Vertex Buffer
            // This is a Quad so we must copy all 4 Vertices over
            sVertexBuffer[iIndex].Position = sBottomLeft;
            sVertexBuffer[iIndex].Color = cParticle.Color;

            sVertexBuffer[iIndex + 1].Position = sTopLeft;
            sVertexBuffer[iIndex + 1].Color = cParticle.Color;

            sVertexBuffer[iIndex + 2].Position = sBottomRight;
            sVertexBuffer[iIndex + 2].Color = cParticle.Color;

            sVertexBuffer[iIndex + 3].Position = sTopRight;
            sVertexBuffer[iIndex + 3].Color = cParticle.Color;

            // Fill in the Index Buffer for the newly added Vertices.
            // Specify the Vertices in Counter-Clockwise order.
            // It takes 6 Indexes to represent a quad (2 triangles = 6 corners).
            IndexBuffer[IndexBufferIndex++] = iIndex;
            IndexBuffer[IndexBufferIndex++] = iIndex + 2;
            IndexBuffer[IndexBufferIndex++] = iIndex + 1;
            IndexBuffer[IndexBufferIndex++] = iIndex + 2;
            IndexBuffer[IndexBufferIndex++] = iIndex + 3;
            IndexBuffer[IndexBufferIndex++] = iIndex + 1;
        }

        /// <summary>
        /// Virtual function to Set the Graphics Device properties for rendering the Particles
        /// </summary>
        /// <param name="cRenderState">The Render State of the Graphics Device to change</param>
        protected override void SetRenderState(RenderState cRenderState)
        {
            // Set the alpha blend mode
            cRenderState.AlphaBlendEnable = true;
            cRenderState.AlphaBlendOperation = BlendFunction.Add;
            cRenderState.SourceBlend = Blend.SourceAlpha;
            cRenderState.DestinationBlend = Blend.InverseSourceAlpha;

            // Set the alpha test mode
            cRenderState.AlphaTestEnable = true;
            cRenderState.AlphaFunction = CompareFunction.Greater;
            cRenderState.ReferenceAlpha = 0;

            // Enable the depth buffer (so particles will not be visible through
            // solid objects like the ground plane), but disable depth writes
            // (so particles will not obscure other particles).
            cRenderState.DepthBufferEnable = true;
            cRenderState.DepthBufferWriteEnable = false;
        }

        /// <summary>
        /// Virtual function to Reset the Graphics Device properties after rendering the Particles
        /// </summary>
        /// <param name="cRenderState">The Render State of the Graphics Device to change</param>
        protected override void ResetRenderState(RenderState cRenderState)
        {
            // Reset the more unusual Render States that we changed,
            // so as not to mess up any other subsequent drawing
            cRenderState.AlphaBlendEnable = false;
            cRenderState.AlphaTestEnable = false;
            cRenderState.DepthBufferWriteEnable = true;
        }

        /// <summary>
        /// Virtual function to Set the Effect's Parameters before drawing the Particles
        /// </summary>
        protected override void SetEffectParameters()
        {
            // Specify the World, View, and Projection Matrices
            Effect.Parameters["xWorld"].SetValue(World);
            Effect.Parameters["xView"].SetValue(View);
            Effect.Parameters["xProjection"].SetValue(Projection);
        }

        //===========================================================
        // Initialization Function
        //===========================================================

        /// <summary>
        /// Function to Initialize a Default Particle with default settings
        /// </summary>
        /// <param name="Particle">The Particle to be Initialized</param>
        public override void InitializeParticleUsingInitialProperties(DPSFParticle Particle)
        {
            // Cast the Particle to the type it really is
            DefaultQuadParticle cParticle = (DefaultQuadParticle)Particle;

            // Initialize the Particle according to the values specified in the Initial Settings
            base.InitializeParticleUsingInitialProperties(cParticle, mcInitialProperties);

            // If the Rotation should be interpolated between the Min and Max Rotation
            if (mcInitialProperties.InterpolateBetweenMinAndMaxRotation)
            {
                // Calculate the Particle's initial Rotational values
                Vector3 sRotation = Vector3.Lerp(mcInitialProperties.RotationMin, mcInitialProperties.RotationMax, RandomNumber.NextFloat());
                cParticle.Orientation = Quaternion.CreateFromYawPitchRoll(sRotation.Y, sRotation.X, sRotation.Z);
            }
            // Else the Rotation XYZ values should each be calculated individually
            else
            {
                // Calculate the Particle's initial Rotational values
                Vector3 sRotation = DPSFHelper.RandomVectorBetweenTwoVectors(mcInitialProperties.RotationMin, mcInitialProperties.RotationMax);
                cParticle.Orientation = Quaternion.CreateFromYawPitchRoll(sRotation.Y, sRotation.X, sRotation.Z);
            }

            // If the Rotational Velocity should be interpolated between the Min and Max Rotational Velocities
            if (mcInitialProperties.InterpolateBetweenMinAndMaxRotationalVelocity)
            {
                cParticle.RotationalVelocity = Vector3.Lerp(mcInitialProperties.RotationalVelocityMin, mcInitialProperties.RotationalVelocityMax, RandomNumber.NextFloat());
            }
            // Else the Rotational Velocity XYZ values should each be calculated individually
            else
            {
                cParticle.RotationalVelocity = DPSFHelper.RandomVectorBetweenTwoVectors(mcInitialProperties.RotationalVelocityMin, mcInitialProperties.RotationalVelocityMax);
            }

            // If the Rotational Acceleration should be interpolated between the Min and Max Rotational Acceleration
            if (mcInitialProperties.InterpolateBetweenMinAndMaxRotationalAcceleration)
            {
                cParticle.RotationalAcceleration = Vector3.Lerp(mcInitialProperties.RotationalAccelerationMin, mcInitialProperties.RotationalAccelerationMax, RandomNumber.NextFloat());
            }
            // Else the Rotational Acceleration XYZ values should each be calculated individually
            else
            {
                cParticle.RotationalAcceleration = DPSFHelper.RandomVectorBetweenTwoVectors(mcInitialProperties.RotationalAccelerationMin, mcInitialProperties.RotationalAccelerationMax);
            }

            // Calculate the Particle's Width and Height values
            cParticle.StartWidth = DPSFHelper.RandomNumberBetween(mcInitialProperties.StartWidthMin, mcInitialProperties.StartWidthMax);
            cParticle.EndWidth = DPSFHelper.RandomNumberBetween(mcInitialProperties.EndWidthMin, mcInitialProperties.EndWidthMax);
            cParticle.StartHeight = DPSFHelper.RandomNumberBetween(mcInitialProperties.StartHeightMin, mcInitialProperties.StartHeightMax);
            cParticle.EndHeight = DPSFHelper.RandomNumberBetween(mcInitialProperties.EndHeightMin, mcInitialProperties.EndHeightMax);
            cParticle.Width = cParticle.StartWidth;
            cParticle.Height = cParticle.StartHeight;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        /// <summary>
        /// Update a Particle's Rotation according to its Rotational Velocity
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleRotationUsingRotationalVelocity(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Rotation according to its Rotational Velocity
            // If Rotational Velocity is being used
            if (cParticle.RotationalVelocity != Vector3.Zero)
            {
                // Quaternion Rotation Formula: NewOrientation = OldNormalizedOrientation * Rotation(v*t) * 0.5,
                // where v is the angular(rotational) velocity vector, and t is the amount of time elapsed.
                // The 0.5 is used to scale the rotation so that Pi = 180 degree rotation
                cParticle.Orientation.Normalize();
                Quaternion sRotation = new Quaternion(cParticle.RotationalVelocity * (fElapsedTimeInSeconds * 0.5f), 0);
                cParticle.Orientation += cParticle.Orientation * sRotation;
            }
        }

        /// <summary>
        /// Update a Particle's Rotational Velocity according to its Rotational Acceleration
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleRotationalVelocityUsingRotationalAcceleration(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
        {
            // If Rotational Acceleration is being used
            if (cParticle.RotationalAcceleration != Vector3.Zero)
            {
                // Update the Particle's Rotaional Velocity according to its Rotational Acceleration
                cParticle.RotationalVelocity += cParticle.RotationalAcceleration * fElapsedTimeInSeconds;
            }
        }

        /// <summary>
        /// Update a Particle's Rotation and Rotational Velocity according to its Rotational Acceleration
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Particle's Rotaional Velocity and Rotation according to its Rotational Acceleration
            UpdateParticleRotationalVelocityUsingRotationalAcceleration(cParticle, fElapsedTimeInSeconds);
            UpdateParticleRotationUsingRotationalVelocity(cParticle, fElapsedTimeInSeconds);
        }

        /// <summary>
        /// Linearly interpolate the Particle's Width between the Start and End Width according
        /// to the Particle's Normalized Lifetime
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleWidthUsingLerp(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Calculate the Particle's new Width
            cParticle.Width = MathHelper.Lerp(cParticle.StartWidth, cParticle.EndWidth, cParticle.NormalizedElapsedTime);
        }

        /// <summary>
        /// Linearly interpolate the Particle's Height between the Start and End Height according
        /// to the Particle's Normalized Lifetime
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleHeightUsingLerp(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Calculate the Particle's new Height
            cParticle.Height = MathHelper.Lerp(cParticle.StartHeight, cParticle.EndHeight, cParticle.NormalizedElapsedTime);
        }

        /// <summary>
        /// Linearly interpolate the Particle's Width and Height between the Start and End values according
        /// to the Particle's Normalized Lifetime
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleWidthAndHeightUsingLerp(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Calculate the Particle's new Width and Height
            cParticle.Width = MathHelper.Lerp(cParticle.StartWidth, cParticle.EndWidth, cParticle.NormalizedElapsedTime);
            cParticle.Height = MathHelper.Lerp(cParticle.StartHeight, cParticle.EndHeight, cParticle.NormalizedElapsedTime);
        }

        /// <summary>
        /// Turns the Particle into a Billboard Particle (i.e. The Particle always faces the Camera).
        /// <para>NOTE: This Update function should be called after all other Update functions to ensure that 
        /// the Particle is orientated correctly.</<para>
        /// <para>NOTE: Update the Particle System's Camera Position every frame to ensure that this works correctly.</para>
        /// <para>NOTE: Only Roll Rotations (i.e. around the Z axis) will be visible when this is used.</para>
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleToFaceTheCamera(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Make the Particle face the Camera
            cParticle.Normal = CameraPosition - cParticle.Position;
        }
    }

    /// <summary>
    /// The Default Textured Quad Particle System class
    /// </summary>
    /// <typeparam name="Particle">The Particle class to use</typeparam>
    /// <typeparam name="Vertex">The Vertex Format to use</typeparam>
    public class DPSFDefaultTexturedQuadParticleSystem<Particle, Vertex> : DPSFDefaultQuadParticleSystem<Particle, Vertex>
        where Particle : DPSFParticle, new()
        where Vertex : struct, IDPSFParticleVertex
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DPSFDefaultTexturedQuadParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        //===========================================================
        // Vertex Update and Overridden Particle System Functions
        //===========================================================

        /// <summary>
        /// Function to update the Vertex properties according to the Particle properties
        /// </summary>
        /// <param name="sVertexBuffer">The array containing the Vertices to be drawn</param>
        /// <param name="iIndex">The Index in the array where the Particle's Vertex info should be placed</param>
        /// <param name="Particle">The Particle to copy the information from</param>
        public virtual void UpdateVertexProperties(ref DefaultTexturedQuadParticleVertex[] sVertexBuffer, int iIndex, DPSFParticle Particle)
        {
            // Cast the Particle to the type it really is
            DefaultTexturedQuadParticle cParticle = (DefaultTexturedQuadParticle)Particle;
            
            // Calculate what half of the Quads Width and Height are
            int iHalfWidth = (int)(cParticle.Width / 2.0f);
            int iHalfHeight = (int)(cParticle.Height / 2.0f);
            
            // Calculate the Positions of the Quads corners around the origin
            Vector3 sTopLeft = new Vector3(-iHalfWidth, iHalfHeight, 0);
            Vector3 sTopRight = new Vector3(iHalfWidth, iHalfHeight, 0);
            Vector3 sBottomLeft = new Vector3(-iHalfWidth, -iHalfHeight, 0);
            Vector3 sBottomRight = new Vector3(iHalfWidth, -iHalfHeight, 0);

            // Rotate the Quad corners around the origin according to its Orientation, 
            // then calculate their final Positions
            sTopLeft = Vector3.Transform(sTopLeft, cParticle.Orientation) + cParticle.Position;
            sTopRight = Vector3.Transform(sTopRight, cParticle.Orientation) + cParticle.Position;
            sBottomLeft = Vector3.Transform(sBottomLeft, cParticle.Orientation) + cParticle.Position;
            sBottomRight = Vector3.Transform(sBottomRight, cParticle.Orientation) + cParticle.Position;

            // Copy this Particle's renderable Properties to the Vertex Buffer
            // This is a Quad so we must copy all 4 Vertices over
            sVertexBuffer[iIndex].Position = sBottomLeft;
            sVertexBuffer[iIndex].TextureCoordinate = new Vector2(0, 1);
            sVertexBuffer[iIndex].Color = cParticle.Color;

            sVertexBuffer[iIndex + 1].Position = sTopLeft;
            sVertexBuffer[iIndex + 1].TextureCoordinate = new Vector2(0, 0);
            sVertexBuffer[iIndex + 1].Color = cParticle.Color;

            sVertexBuffer[iIndex + 2].Position = sBottomRight;
            sVertexBuffer[iIndex + 2].TextureCoordinate = new Vector2(1, 1);
            sVertexBuffer[iIndex + 2].Color = cParticle.Color;

            sVertexBuffer[iIndex + 3].Position = sTopRight;
            sVertexBuffer[iIndex + 3].TextureCoordinate = new Vector2(1, 0);
            sVertexBuffer[iIndex + 3].Color = cParticle.Color;

            // Fill in the Index Buffer for the newly added Vertices.
            // Specify the Vertices in Counter-Clockwise order.
            // It takes 6 Indexes to represent a quad (2 triangles = 6 corners).
            IndexBuffer[IndexBufferIndex++] = iIndex;
            IndexBuffer[IndexBufferIndex++] = iIndex + 2;
            IndexBuffer[IndexBufferIndex++] = iIndex + 1;
            IndexBuffer[IndexBufferIndex++] = iIndex + 2;
            IndexBuffer[IndexBufferIndex++] = iIndex + 3;
            IndexBuffer[IndexBufferIndex++] = iIndex + 1;            
        }

        /// <summary>
        /// Virtual function to Set the Graphics Device properties for rendering the Particles
        /// </summary>
        /// <param name="cRenderState">The Render State of the Graphics Device to change</param>
        protected override void SetRenderState(RenderState cRenderState)
        {
            // Set the alpha blend mode
            cRenderState.AlphaBlendEnable = true;
            cRenderState.AlphaBlendOperation = BlendFunction.Add;
            cRenderState.SourceBlend = Blend.SourceAlpha;
            cRenderState.DestinationBlend = Blend.InverseSourceAlpha;

            // Set the alpha test mode
            cRenderState.AlphaTestEnable = true;
            cRenderState.AlphaFunction = CompareFunction.Greater;
            cRenderState.ReferenceAlpha = 0;

            // Enable the depth buffer (so particles will not be visible through
            // solid objects like the ground plane), but disable depth writes
            // (so particles will not obscure other particles).
            cRenderState.DepthBufferEnable = true;
            cRenderState.DepthBufferWriteEnable = false;
        }

        /// <summary>
        /// Virtual function to Reset the Graphics Device properties after rendering the Particles
        /// </summary>
        /// <param name="cRenderState">The Render State of the Graphics Device to change</param>
        protected override void ResetRenderState(RenderState cRenderState)
        {
            // Reset the more unusual Render States that we changed,
            // so as not to mess up any other subsequent drawing
            cRenderState.AlphaBlendEnable = false;
            cRenderState.AlphaTestEnable = false;
            cRenderState.DepthBufferWriteEnable = true;
        }

        /// <summary>
        /// Virtual function to Set the Effect's Parameters before drawing the Particles
        /// </summary>
        protected override void SetEffectParameters()
        {
            // Specify the World, View, and Projection Matrices
            Effect.Parameters["xWorld"].SetValue(World);
            Effect.Parameters["xView"].SetValue(View);
            Effect.Parameters["xProjection"].SetValue(Projection);

            // Specify the Texture to use
            Effect.Parameters["xTexture"].SetValue(Texture);
        }

        //===========================================================
        // Initialization Function
        //===========================================================

        //===========================================================
        // Particle Update Functions
        //===========================================================
    }

    /// <summary>
    /// The Default Animated Textured Quad Particle System class
    /// </summary>
    /// <typeparam name="Particle">The Particle class to use</typeparam>
    /// <typeparam name="Vertex">The Vertex Format to use</typeparam>
    public class DPSFDefaultAnimatedTexturedQuadParticleSystem<Particle, Vertex> : DPSFDefaultTexturedQuadParticleSystem<Particle, Vertex>
        where Particle : DPSFParticle, new()
        where Vertex : struct, IDPSFParticleVertex
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DPSFDefaultAnimatedTexturedQuadParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Vertex Update and Overridden Particle System Functions
        //===========================================================

        /// <summary>
        /// Function to update the Vertex properties according to the Particle properties
        /// </summary>
        /// <param name="sVertexBuffer">The array containing the Vertices to be drawn</param>
        /// <param name="iIndex">The Index in the array where the Particle's Vertex info should be placed</param>
        /// <param name="Particle">The Particle to copy the information from</param>
        public override void UpdateVertexProperties(ref DefaultTexturedQuadParticleVertex[] sVertexBuffer, int iIndex, DPSFParticle Particle)
        {
            // Cast the Particle to the type it really is
            DefaultAnimatedTexturedQuadParticle cParticle = (DefaultAnimatedTexturedQuadParticle)Particle;

            // Calculate what half of the Quads Width and Height are
            int iHalfWidth = (int)(cParticle.Width / 2.0f);
            int iHalfHeight = (int)(cParticle.Height / 2.0f);

            // Calculate the Positions of the Quads corners around the origin
            Vector3 sTopLeft = new Vector3(-iHalfWidth, iHalfHeight, 0);
            Vector3 sTopRight = new Vector3(iHalfWidth, iHalfHeight, 0); ;
            Vector3 sBottomLeft = new Vector3(-iHalfWidth, -iHalfHeight, 0); ;
            Vector3 sBottomRight = new Vector3(iHalfWidth, -iHalfHeight, 0); ;

            // Rotate the Quad corners around the origin according to its Orientation, 
            // then calculate their final Positions
            sTopLeft = Vector3.Transform(sTopLeft, cParticle.Orientation) + cParticle.Position;
            sTopRight = Vector3.Transform(sTopRight, cParticle.Orientation) + cParticle.Position;
            sBottomLeft = Vector3.Transform(sBottomLeft, cParticle.Orientation) + cParticle.Position;
            sBottomRight = Vector3.Transform(sBottomRight, cParticle.Orientation) + cParticle.Position;

            // Copy this Particle's renderable Properties to the Vertex Buffer
            // This is a Quad so we must copy all 4 Vertices over
            sVertexBuffer[iIndex].Position = sBottomLeft;
            sVertexBuffer[iIndex].TextureCoordinate = new Vector2(cParticle.NormalizedTextureCoordinateLeftTop.X, cParticle.NormalizedTextureCoordinateRightBottom.Y);
            sVertexBuffer[iIndex].Color = cParticle.Color;

            sVertexBuffer[iIndex + 1].Position = sTopLeft;
            sVertexBuffer[iIndex + 1].TextureCoordinate = cParticle.NormalizedTextureCoordinateLeftTop;
            sVertexBuffer[iIndex + 1].Color = cParticle.Color;

            sVertexBuffer[iIndex + 2].Position = sBottomRight;
            sVertexBuffer[iIndex + 2].TextureCoordinate = cParticle.NormalizedTextureCoordinateRightBottom;
            sVertexBuffer[iIndex + 2].Color = cParticle.Color;

            sVertexBuffer[iIndex + 3].Position = sTopRight;
            sVertexBuffer[iIndex + 3].TextureCoordinate = new Vector2(cParticle.NormalizedTextureCoordinateRightBottom.X, cParticle.NormalizedTextureCoordinateLeftTop.Y);
            sVertexBuffer[iIndex + 3].Color = cParticle.Color;

            // Fill in the Index Buffer for the newly added Vertices.
            // Specify the Vertices in Counter-Clockwise order.
            // It takes 6 Indexes to representa quad, since 2 Vertices are
            // shared between the two triangles.
            IndexBuffer[IndexBufferIndex++] = iIndex;
            IndexBuffer[IndexBufferIndex++] = iIndex + 2;
            IndexBuffer[IndexBufferIndex++] = iIndex + 1;
            IndexBuffer[IndexBufferIndex++] = iIndex + 2;
            IndexBuffer[IndexBufferIndex++] = iIndex + 3;
            IndexBuffer[IndexBufferIndex++] = iIndex + 1;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        /// <summary>
        /// Updates the Animation, as well as the Particle's Texture Coordinates to match the Animation
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleAnimationAndTextureCoordinates(DefaultAnimatedTexturedQuadParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Update the Animation
            cParticle.Animation.Update(fElapsedTimeInSeconds);

            // Get the Particle's Texture Coordinates
            Rectangle sSourceRect = cParticle.Animation.CurrentPicturesTextureCoordinates;

            // Normalize the Particle's Texture Coordinates
            float fLeft = sSourceRect.Left / (float)Texture.Width;
            float fRight = sSourceRect.Right / (float)Texture.Width;
            float fTop = sSourceRect.Top / (float)Texture.Height;
            float fBottom = sSourceRect.Bottom / (float)Texture.Height;

            // Set the Particle's Texture Coordinates to use
            cParticle.NormalizedTextureCoordinateLeftTop = new Vector2(fLeft, fTop);
            cParticle.NormalizedTextureCoordinateRightBottom = new Vector2(fRight, fBottom);
        }

        /// <summary>
        /// Updates the Particle to be removed from the Particle System once the Animation finishes Playing
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleToDieOnceAnimationFinishesPlaying(DefaultAnimatedTexturedQuadParticle cParticle, float fElapsedTimeInSeconds)
        {
            // If the Animation has finished Playing
            if (cParticle.Animation.CurrentAnimationIsDonePlaying)
            {
                // Make sure the Lifetime is greater than zero
                // We make it a small value to try and keep from triggering any Timed Events by accident
                cParticle.Lifetime = 0.00001f;

                // Set the Particle to die
                cParticle.NormalizedElapsedTime = 1.0f;
            }
        }
    }

    #endregion

    #region Default Particle Vertex's

    /// <summary>
    /// Structure used to hold a Default Pixel Particle Vertex's properties used for drawing.
    /// This contains a Vector3 Position and a Color Color.
    /// </summary>
    public struct DefaultPixelParticleVertex : IDPSFParticleVertex
    {
        /// <summary>
        /// The Position of the Particle in 3D space
        /// </summary>
        public Vector3 Position;
        
        /// <summary>
        /// The Color of the Particle
        /// </summary>
        public Color Color;

        // Describe the vertex structure used to display a Particle
        private static readonly VertexElement[] msVertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.Position, 0),

            new VertexElement(0, 12, VertexElementFormat.Color,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.Color, 0),
        };

        // The size of the vertex structure in bytes
        private const int miSizeInBytes = 16;

        /// <summary>
        /// An array describing the attributes of each Vertex
        /// </summary>
        public VertexElement[] VertexElements
        {
            get { return DefaultPixelParticleVertex.msVertexElements; }
        }

        /// <summary>
        /// The Size of one Vertex Element in Bytes
        /// </summary>
        public int SizeInBytes
        {
            get { return DefaultPixelParticleVertex.miSizeInBytes; }
        }
    }

    /// <summary>
    /// Dummy structure used for the vertices of Default Sprites. Since Sprites are drawn using a 
    /// SpriteBatch, they do not have vertices, so this structure is empty.
    /// </summary>
    public struct DefaultSpriteParticleVertex : IDPSFParticleVertex
    {
        /// <summary>
        /// An array describing the attributes of each Vertex
        /// </summary>
        public VertexElement[] VertexElements
        {
            get { return null; }
        }

        /// <summary>
        /// The Size of one Vertex Element in Bytes
        /// </summary>
        public int SizeInBytes
        {
            get { return 0; }
        }
    }

    /// <summary>
    /// Structure used to hold a Default Point Sprite Particle Vertex's properties used for drawing.
    /// This contains a Vector3 Position, float Size, Color Color, and float Rotation.
    /// </summary>
    public struct DefaultPointSpriteParticleVertex : IDPSFParticleVertex
    {
        /// <summary>
        /// The Position of the Particle in 3D space
        /// </summary>
        public Vector3 Position;
        
        /// <summary>
        /// The Size (width and height) of the Particle
        /// </summary>
        public float Size;

        /// <summary>
        /// The Color to tint the Texture
        /// </summary>
        public Color Color;

        /// <summary>
        /// The 2D roll Rotation of the Particle
        /// </summary>
        public float Rotation;

        // Describe the vertex structure used to display a Particle
        private static readonly VertexElement[] msVertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.Position, 0),

            new VertexElement(0, 12, VertexElementFormat.Single, 
                                     VertexElementMethod.Default, 
                                     VertexElementUsage.PointSize, 0),

            new VertexElement(0, 16, VertexElementFormat.Color,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.Color, 0),

            new VertexElement(0, 20, VertexElementFormat.Single,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.TextureCoordinate, 0),
        };

        // The size of the vertex structure in bytes
        private const int miSizeInBytes = 24;

        /// <summary>
        /// An array describing the attributes of each Vertex
        /// </summary>
        public VertexElement[] VertexElements
        {
            get { return DefaultPointSpriteParticleVertex.msVertexElements; }
        }

        /// <summary>
        /// The Size of one Vertex Element in Bytes
        /// </summary>
        public int SizeInBytes
        {
            get { return DefaultPointSpriteParticleVertex.miSizeInBytes; }
        }
    }

    /// <summary>
    /// Structure used to hold a Default Quad Particle's Vertex's properties used for drawing.
    /// This contains a Vector3 Position and a Color Color.
    /// </summary>
    public struct DefaultQuadParticleVertex : IDPSFParticleVertex
    {
        /// <summary>
        /// The Position of the vertex in 3D space. The position of this vertex
        /// relative to the quads other three vertices determines the Particle's orientation.
        /// </summary>
        public Vector3 Position;
        
        /// <summary>
        /// The Color of the vertex
        /// </summary>
        public Color Color;

        // Describe the vertex structure used to display a Particle
        private static readonly VertexElement[] msVertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.Position, 0),

            new VertexElement(0, 12, VertexElementFormat.Color,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.Color, 0),
        };

        // The size of the vertex structure in bytes
        private const int miSizeInBytes = 16;

        /// <summary>
        /// An array describing the attributes of each Vertex
        /// </summary>
        public VertexElement[] VertexElements
        {
            get { return DefaultQuadParticleVertex.msVertexElements; }
        }

        /// <summary>
        /// The Size of one Vertex Element in Bytes
        /// </summary>
        public int SizeInBytes
        {
            get { return DefaultQuadParticleVertex.miSizeInBytes; }
        }
    }

    /// <summary>
    /// Structure used to hold a Default Textured Quad Particle's Vertex's properties used for drawing.
    /// This contains a Vector3 Position, Vector2 TextureCoordinate, and Color Color.
    /// </summary>
    public struct DefaultTexturedQuadParticleVertex : IDPSFParticleVertex
    {
        /// <summary>
        /// The Position of the vertex in 3D space. The position of this vertex
        /// relative to the quads other three vertices determines the Particle's orientation.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The Coordinate of the Texture that this Vertex corresponds to
        /// </summary>
        public Vector2 TextureCoordinate;
        
        /// <summary>
        /// The Color to tint the Texture
        /// </summary>
        public Color Color;

        // Describe the vertex structure used to display a Particle
        private static readonly VertexElement[] msVertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.Position, 0),

            new VertexElement(0, 12, VertexElementFormat.Vector2,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.TextureCoordinate, 0),

            new VertexElement(0, 20, VertexElementFormat.Color,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.Color, 0),
        };

        // The size of the vertex structure in bytes
        private const int miSizeInBytes = 24;

        /// <summary>
        /// An array describing the attributes of each Vertex
        /// </summary>
        public VertexElement[] VertexElements
        {
            get { return DefaultTexturedQuadParticleVertex.msVertexElements; }
        }

        /// <summary>
        /// The Size of one Vertex Element in Bytes
        /// </summary>
        public int SizeInBytes
        {
            get { return DefaultTexturedQuadParticleVertex.miSizeInBytes; }
        }
    }

    #endregion

    #region Default Objects

    /// <summary>
    /// Magnet of a Particle System to have an affect on its Paticles, such as
    /// attracting or repeling them
    /// </summary>
    public class DefaultParticleSystemMagnet
    {
        /// <summary>
        /// The Modes that the Magnet can be in
        /// </summary>
        public enum MagnetModes
        {
            /// <summary>
            /// Attract Particles to the Magnet
            /// </summary>
            Attract = 0,

            /// <summary>
            /// Repel Particles from the Magnet
            /// </summary>
            Repel = 1,

            /// <summary>
            /// Have some other custom effect on the Particles
            /// </summary>
            Other = 2
        }

        /// <summary>
        /// How much the Magnet should affect the Particles based on the Magnet's
        /// Max Distance and Strength
        /// </summary>
        public enum DistanceFunctions
        {
            /// <summary>
            /// As long as the Particle's distance from the Magnet is between the Min and Max Distance of 
            /// the Magnet, the Max Force will be applied to the Particle.
            /// <para>Function Logic: y = 1, where y is the normalized Force applied and 1 is the Particle's
            /// normalized distance between the Magnet's Min and Max Distances.</para>
            /// </summary>
            Constant = 0,

            /// <summary>
            /// The Force applied to the Particle will be Linearly interpolated from zero to Max Force
            /// based on the Particle's distance from the Magnet between the Magnet's Min and Max Distances. 
            /// So when the Particle is at the Min Distance from the Magnet no force will be applied, when 
            /// the Particle is at half of the Max Distance from the Magnet 1/2 of the Max Force will be 
            /// applied, and when the Particle is at the Max Distance from the Magnet the Max Force will be 
            /// applied to the Particle.
            /// <para>Function Logic: y = x, where y is the normalized Force applied and x is the Particle's 
            /// normalized distance between the Magnet's Min and Max Distances.</para>
            /// </summary>
            Linear = 1,

            /// <summary>
            /// The Force applied to the Particle will be Squared interpolated from zero to Max Force
            /// based on the Particle's distance from the Magnet between the Magnet's Min and Max Distances. 
            /// So when the Particle is at the Min Distance from the Magnet no force will be applied, when 
            /// the Particle is at half of the Max Distance from the Magnet 1/4 of the Max Force will be 
            /// applied, and when the Particle is at the Max Distance from the Magnet the Max Force will be 
            /// applied to the Particle.
            /// <para>Function Logic: y = x * x, where y is the normalized Force applied and x is the Particle's
            /// normalized distance between the Magnet's Min and Max Distances.</para>
            /// </summary>
            Squared = 2,

            /// <summary>
            /// The Force applied to the Particle will be Cubed interpolated from zero to Max Force
            /// based on the Particle's distance from the Magnet between the Magnet's Min and Max Distances.
            /// So when the Particle is at the Min Distance from the Magnet no force will be applied, when 
            /// the Particle is at half of the Max Distance from the Manget 1/8 of the Max Force will be 
            /// applied, and when the Particle is at the Max Distance from the Magnet the Max Force will be 
            /// applied to the Particle.
            /// <para>Function Logic: y = x * x * x, where y is the normalized Force applied and x is the Particle's
            /// normalized distance between the Magnet's Min and Max Distances.</para>
            /// </summary>
            Cubed = 3,

            /// <summary>
            /// The Inverse of the Linear function. That is, when the Particle is at the Min Distance from
            /// the Magnet the Max Force will be applied, when the Particle is at half of the Max Distance
            /// from the Magnet 1/2 of the Max Force will be applied, and when the Particle is at the Max Distance
            /// from the Magnet no force will be applied to the Particle.
            /// </summary>
            LinearInverse = 4,

            /// <summary>
            /// The Inverse of the Squared function. That is, when the Particle is at the Min Distance from
            /// the Magnet the Max Force will be applied, when the Particle is at half of the Max Distance
            /// from the Magnet 1/4 of the Max Force will be applied, and when the Particle is at the Max Distance
            /// from the Magnet no force will be applied to the Particle.
            /// </summary>
            SquaredInverse = 5,

            /// <summary>
            /// The Inverse of the Cubed function. That is, when the Particle is at the Min Distance from
            /// the Magnet the Max Force will be applied, when the Particle is at half of the Max Distance
            /// from the Magnet 1/8 of the Max Force will be applied, and when the Particle is at the Max Distance
            /// from the Magnet no force will be applied to the Particle.
            /// </summary>
            CubedInverse = 6
        }

        /// <summary>
        /// The current Mode that the Magnet is in
        /// </summary>
        public MagnetModes Mode = MagnetModes.Attract;

        /// <summary>
        /// The Function to use to determine how much a Particle should be affected by 
        /// the Magnet based on how far away from the Magnet it is
        /// </summary>
        public DistanceFunctions DistanceFunction = DistanceFunctions.Linear;

        /// <summary>
        /// The Position, Velocity, and Acceleration of the Magnet
        /// </summary>
        public Position3D PositionData = new Position3D();

        /// <summary>
        /// The Min Distance that the Magnet should be able to affect Particles at. If the
        /// Particle is closer to the Magnet than this distance, the Magnet will not affect
        /// the Particle.
        /// </summary>
        public float MinDistance = 0.0f;

        /// <summary>
        /// The Max Distance that the Magnet should be able to affect Particles at. If the
        /// Particle is further away from the Magnet tan this distance, the Manget will not
        /// affect the Particle.
        /// </summary>
        public float MaxDistance = 100;

        /// <summary>
        /// The Max Force that the Magnet is able to exert on a Particle
        /// </summary>
        public float MaxForce = 1;

        /// <summary>
        /// The Type of Magnet this is. This may be used in conjunction with the "Other" Magnet
        /// Mode to distinguish which type of custom user effect the Magnet should have on the Particles.
        /// </summary>
        public int Type = 0;

        // Static variable to assign unique IDs to the Magnets
        static private int SmiCounter = 0;

        // The unique ID of the Magnet
        private int miID = SmiCounter++;

        /// <summary>
        /// Get the unique ID of this Magnet
        /// </summary>
        public int ID
        {
            get { return miID; }
        }

        /// <summary>
        /// Explicit Constructor
        /// </summary>
        /// <param name="eMode">The Mode that the Magnet should be in</param>
        /// <param name="eDistanceFunction">The Function to use to determine how much a Particle should be affected by 
        /// the Magnet based on how far away from the Magnet it is</param>
        /// <param name="sPosition">The 3D Position of the Magnet</param>
        /// <param name="fMinDistance">The Min Distance that the Magnet should be able to affect Particles at. If the
        /// Particle is closer to the Magnet than this distance, the Magnet will not affect the Particle.</param>
        /// <param name="fMaxDistance">The Max Distance that the Magnet should be able to affect Particles at. If the
        /// Particle is further away from the Magnet tan this distance, the Manget will not affect the Particle.</param>
        /// <param name="fMaxForce">The Max Force that the Magnet is able to exert on a Particle</param>
        /// <param name="iType">The Type of Magnet this is. This may be used in conjunction with the "Other" Magnet
        /// Mode to distinguish which type of custom user effect the Magnet should have on the Particles.</param>
        public DefaultParticleSystemMagnet(MagnetModes eMode, DistanceFunctions eDistanceFunction, Vector3 sPosition,
                                           float fMinDistance, float fMaxDistance, float fMaxForce, int iType)
        {
            Mode = eMode;
            DistanceFunction = eDistanceFunction;
            PositionData.Position = sPosition;
            MinDistance = fMinDistance;
            MaxDistance = fMaxDistance;
            MaxForce = fMaxForce;
            Type = iType;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="cMagnetToCopy">The Magnet to copy from</param>
        public DefaultParticleSystemMagnet(DefaultParticleSystemMagnet cMagnetToCopy)
        {
            CopyFrom(cMagnetToCopy);
        }

        /// <summary>
        /// Copies the given Magnet's data into this Magnet's data
        /// </summary>
        /// <param name="cMagnetToCopy">The Magnet to copy from</param>
        public void CopyFrom(DefaultParticleSystemMagnet cMagnetToCopy)
        {
            Mode = cMagnetToCopy.Mode;
            DistanceFunction = cMagnetToCopy.DistanceFunction;
            PositionData.CopyFrom(cMagnetToCopy.PositionData);
            MinDistance = cMagnetToCopy.MinDistance;
            MaxDistance = cMagnetToCopy.MaxDistance;
            MaxForce = cMagnetToCopy.MaxForce;
            Type = cMagnetToCopy.Type;
        }
    }

    #endregion
}
