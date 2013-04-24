#region File Description
//===================================================================
// This files in the DPSF Defaults folder provide some Default Particle 
// Systems that inherit from the DPSF class. These classes may be used 
// as-is to create new Particle Systems, or my be inherited by new Particle 
// System classes to extend the existing functionality.
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion

namespace DPSF
{
	/// <summary>
	/// The Base Particle class from which the Default Particle classes inherit from
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
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
        /// <para>NOTE: This should be a Non-Premultipilied color.</para>
		/// </summary>
		public Color Color;

        /// <summary>
        /// Get the Color as a Premultiplied color (i.e. premultiplied alpha).
        /// </summary>
        public Color ColorAsPremultiplied { get { return Color.FromNonPremultiplied(this.Color.ToVector4()); } }

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
		/// The Particle's Color when it is born.
        /// <para>NOTE: This should be a Non-Premultipilied color.</para>
		/// </summary>
		public Color StartColor;

		/// <summary>
		/// The Particle's Color when it dies
        /// <para>NOTE: This should be a Non-Premultipilied color.</para>
		/// </summary>
		public Color EndColor;

        /// <summary>
        /// Get the Start Color as a Premultiplied color (i.e. premultiplied alpha).
        /// </summary>
        public Color StartColorAsPremultiplied { get { return Color.FromNonPremultiplied(this.StartColor.ToVector4()); } }

        /// <summary>
        /// Get the End Color as a Premultiplied color (i.e. premultiplied alpha).
        /// </summary>
        public Color EndColorAsPremultiplied { get { return Color.FromNonPremultiplied(this.EndColor.ToVector4()); } }

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
	/// The Base Particle System class that the Default Particle System classes inherit from
	/// </summary>
	/// <typeparam name="Particle">The Particle class to use</typeparam>
	/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DPSFDefaultBaseParticleSystem<Particle, Vertex> : DPSF<Particle, Vertex>
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
#if (WINDOWS)
	[Serializable]
#endif
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
		public List<DefaultParticleSystemMagnet> MagnetList = new List<DefaultParticleSystemMagnet>();

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

			// If the Acceleration should be interpolated between the Min and Max Acceleration
			if (cInitialProperties.InterpolateBetweenMinAndMaxAcceleration)
			{
				cParticle.Acceleration = Vector3.Lerp(cInitialProperties.AccelerationMin, cInitialProperties.AccelerationMax, RandomNumber.NextFloat());
			}
			// Else the Acceleration XYZ values should each be calculated individually
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
		protected void UpdateParticlePositionUsingVelocity(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Update the Particle's Position according to its Velocity
			cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
		}

		/// <summary>
		/// Update a Particle's Velocity according to its Acceleration
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleVelocityUsingAcceleration(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Update the Particle's Position according to its Velocity
			cParticle.Velocity += cParticle.Acceleration * fElapsedTimeInSeconds;
		}

		/// <summary>
		/// Updates a Particle's Velocity according to its Acceleration, and then the Position according to the new Velocity
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticlePositionAndVelocityUsingAcceleration(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
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
		protected void UpdateParticlePositionUsingExternalForce(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Apply the External Force to the Particle's Position
			cParticle.Position += (cParticle.ExternalForce * fElapsedTimeInSeconds);
		}

		/// <summary>
		/// Applies the External Force to the Particle's Velocity
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleVelocityUsingExternalForce(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Apply the External Force to the Particle's Velocity
			cParticle.Velocity += (cParticle.ExternalForce * fElapsedTimeInSeconds);
		}

		/// <summary>
		/// Applies the Particle's Friction to the its Velocity to slow the Particle down to a stop
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleVelocityUsingFriction(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
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
		/// Linearly interpolates the Particle's Color between it's Start Color and End Color based on the Particle's Normalized Elapsed Time.
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleColorUsingLerp(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Update the Particle's Color
			cParticle.Color = DPSFHelper.LerpColor(cParticle.StartColor, cParticle.EndColor, cParticle.NormalizedElapsedTime);
		}

		/// <summary>
		/// Linearly interpolates the Particle's Transparency to fade out based on the Particle's Normalized Elapsed Time.
		/// <para>If you are also updating the Particle Color using an EveryTime Event, be sure to set the ExecutionOrder of the 
		/// event calling this function to be greater than that one, so that this function is called AFTER the color update function.</para>
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleTransparencyToFadeOutUsingLerp(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Calculate how transparent the Particle should be and apply it
			byte bAlpha = (byte)(255 - (cParticle.NormalizedElapsedTime * 255));
			cParticle.Color = new Color(cParticle.Color.R, cParticle.Color.G, cParticle.Color.B, bAlpha);
		}

		/// <summary>
		/// Linearly interpolates the Particle's Transparency to fade in based on the Particle's Normalized Elapsed Time.
		/// <para>If you are also updating the Particle Color using an EveryTime Event, be sure to set the ExecutionOrder of the 
		/// event calling this function to be greater than that one, so that this function is called AFTER the color update function.</para>
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleTransparencyToFadeInUsingLerp(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Calculate how transparent the Particle should be and apply it
			byte bAlpha = (byte)(cParticle.NormalizedElapsedTime * 255);
			cParticle.Color = new Color(cParticle.Color.R, cParticle.Color.G, cParticle.Color.B, bAlpha);
		}

		/// <summary>
		/// Quickly fades the particle in when born and slowly fades it out as it gets closer to death.
		/// <para>If you are also updating the Particle Color using an EveryTime Event, be sure to set the ExecutionOrder of the 
		/// event calling this function to be greater than that one, so that this function is called AFTER the color update function.</para>
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Calculate how transparent the Particle should be and apply it
			byte bAlpha = DPSFHelper.FadeInQuicklyAndFadeOutSlowlyBasedOnLifetime(cParticle.NormalizedElapsedTime);
			cParticle.Color = new Color(cParticle.Color.R, cParticle.Color.G, cParticle.Color.B, bAlpha);
		}

		/// <summary>
		/// Quickly fades the particle in when born and quickly fades it out as it approaches its death.
		/// <para>If you are also updating the Particle Color using an EveryTime Event, be sure to set the ExecutionOrder of the 
		/// event calling this function to be greater than that one, so that this function is called AFTER the color update function.</para>
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleTransparencyWithQuickFadeInAndQuickFadeOut(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Calculate how transparent the Particle should be and apply it
			byte bAlpha = DPSFHelper.FadeInQuicklyAndFadeOutQuicklyBasedOnLifetime(cParticle.NormalizedElapsedTime);
			cParticle.Color = new Color(cParticle.Color.R, cParticle.Color.G, cParticle.Color.B, bAlpha);
		}

		protected void UpdateParticleTransparencyWithQuickFadeIn(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Calculate how transparent the Particle should be and apply it
			byte bAlpha = DPSFHelper.FadeInQuicklyBasedOnLifetime(cParticle.NormalizedElapsedTime);
			cParticle.Color = new Color(cParticle.Color.R, cParticle.Color.G, cParticle.Color.B, bAlpha);
		}

		protected void UpdateParticleTransparencyWithQuickFadeOut(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Calculate how transparent the Particle should be and apply it
			byte bAlpha = DPSFHelper.FadeOutQuicklyBasedOnLifetime(cParticle.NormalizedElapsedTime);
			cParticle.Color = new Color(cParticle.Color.R, cParticle.Color.G, cParticle.Color.B, bAlpha);
		}

		/// <summary>
		/// Calculates how much affect each of the Particle System's Magnets should have on 
		/// this Particle and updates the Particle's Position accordingly.
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticlePositionAccordingToMagnets(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Loop through each of the Particle System's Magnets
			for (int index = 0; index < MagnetList.Count; index++)
			{
				DefaultParticleSystemMagnet magnet = MagnetList[index];

				// If this is not a custom user Magnet (i.e. it is Attracting or Repelling)
				if (magnet.Mode != DefaultParticleSystemMagnet.MagnetModes.Other)
				{
					// Apply the Force to the Particle's Position
					cParticle.Position += (CalculateForceMagnetShouldExertOnParticle(magnet, cParticle) * fElapsedTimeInSeconds);
				}
			}
		}

		/// <summary>
		/// Calculates how much affect each of the Particle System's Magnets should have on 
		/// this Particle and updates the Particle's Velocity accordingly.
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleVelocityAccordingToMagnets(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Loop through each of the Particle System's Magnets
			for (int index = 0; index < MagnetList.Count; index++)
			{
				DefaultParticleSystemMagnet magnet = MagnetList[index];

				// If this is not a custom user Magnet (i.e. it is Attracting or Repelling)
				if (magnet.Mode != DefaultParticleSystemMagnet.MagnetModes.Other)
				{
					// Apply the Force to the Particle's Position
					cParticle.Velocity += (CalculateForceMagnetShouldExertOnParticle(magnet, cParticle) * fElapsedTimeInSeconds);
				}
			}
		}

		/// <summary>
		/// Returns the vector force that a Magnet should exert on a Particle
		/// </summary>
		/// <param name="cMagnet">The Magnet affecting the Particle</param>
		/// <param name="cParticle">The Particle being affected by the Magnet</param>
		/// <returns>Returns the vector force that a Magnet should exert on a Particle</returns>
		protected Vector3 CalculateForceMagnetShouldExertOnParticle(DefaultParticleSystemMagnet cMagnet, DPSFDefaultBaseParticle cParticle)
		{
			// Variable to store the Force to Exert on the Particle
			Vector3 sForceToExertOnParticle = Vector3.Zero;

			// Calculate which Direction to push the Particle
			Vector3 sDirectionToPushParticle;

			// If this is a Point Magnet
			if (cMagnet.MagnetType == DefaultParticleSystemMagnet.MagnetTypes.PointMagnet)
			{
				// Cast the Magnet to the proper type
				MagnetPoint cPointMagnet = (MagnetPoint)cMagnet;

				// Calculate the direction to attract the Particle to the point in space where the Magnet is
				sDirectionToPushParticle = cPointMagnet.PositionData.Position - cParticle.Position;
			}
			// Else If this is a Line Magnet
			else if (cMagnet.MagnetType == DefaultParticleSystemMagnet.MagnetTypes.LineMagnet)
			{
				// Cast the Magnet to the proper type
				MagnetLine cLineMagnet = (MagnetLine)cMagnet;

				// Calculate the closest point on the Line to the Particle.
				// Equation taken from http://ozviz.wasp.uwa.edu.au/~pbourke/geometry/pointline/
				// Also explained at http://www.allegro.cc/forums/thread/589720

				// Calculate 2 points on the Line
				Vector3 sPosition1 = cLineMagnet.PositionOnLine;
				Vector3 sPosition2 = cLineMagnet.PositionOnLine + cLineMagnet.Direction;

				// Put calculations into temp variables for speed and easy readability
				float fA = cParticle.Position.X - sPosition1.X;
				float fB = cParticle.Position.Y - sPosition1.Y;
				float fC = cParticle.Position.Z - sPosition1.Z;
				float fD = sPosition2.X - sPosition1.X;
				float fE = sPosition2.Y - sPosition1.Y;
				float fF = sPosition2.Z - sPosition1.Z;

				// Next calculate the value of U.
				// NOTE: The Direction is normalized, so the distance between Position1 and Position2 is one, so we 
				// don't need to bother squaring and dividing by the length here.
				float fU = (fA * fD) + (fB * fE) + (fC * fF);

				// Calculate the closest point on the Line to the Particle
				Vector3 sClosestPointOnLine = new Vector3();
				sClosestPointOnLine.X = sPosition1.X + (fU * fD);
				sClosestPointOnLine.Y = sPosition1.Y + (fU * fE);
				sClosestPointOnLine.Z = sPosition1.Z + (fU * fF);

				// Calculate the direction to attract the Particle to the closest point on the Line
				sDirectionToPushParticle = sClosestPointOnLine - cParticle.Position;
			}
			// Else if the is a Line Segment Magnet
			else if (cMagnet.MagnetType == DefaultParticleSystemMagnet.MagnetTypes.LineSegmentMagnet)
			{
				// Cast the Magnet to the proper type
				MagnetLineSegment cLineSegmentMagnet = (MagnetLineSegment)cMagnet;

				// Calculate the closest point on the Line to the Particle.
				// Equation taken from http://ozviz.wasp.uwa.edu.au/~pbourke/geometry/pointline/
				// Also explained at http://www.allegro.cc/forums/thread/589720

				// Calculate 2 points on the Line
				Vector3 sPosition1 = cLineSegmentMagnet.EndPoint1;
				Vector3 sPosition2 = cLineSegmentMagnet.EndPoint2;

				// Put calculations into temp variables for speed and easy readability
				float fA = cParticle.Position.X - sPosition1.X;
				float fB = cParticle.Position.Y - sPosition1.Y;
				float fC = cParticle.Position.Z - sPosition1.Z;
				float fD = sPosition2.X - sPosition1.X;
				float fE = sPosition2.Y - sPosition1.Y;
				float fF = sPosition2.Z - sPosition1.Z;

				// Next calculate the value of U
				float fDot = (fA * fD) + (fB * fE) + (fC * fF);
				float fLengthSquared = (fD * fD) + (fE * fE) + (fF * fF);
				float fU = fDot / fLengthSquared;

				// Calculate the closest point on the Line to the Particle
				Vector3 sClosestPointOnLine = new Vector3();

				// If the Particle is closest to the first End Point
				if (fU < 0.0f)
				{
					sClosestPointOnLine = sPosition1;
				}
				// Else If the Particle is closest to the second End Point
				else if (fU > 1.0f)
				{
					sClosestPointOnLine = sPosition2;
				}
				// Else the Particle is closest to the Line Segment somewhere between the End Points
				else
				{
					// Calculate where in between the End Points the Particle is closest to
					sClosestPointOnLine.X = sPosition1.X + (fU * (sPosition2.X - sPosition1.X));
					sClosestPointOnLine.Y = sPosition1.Y + (fU * (sPosition2.Y - sPosition1.Y));
					sClosestPointOnLine.Z = sPosition1.Z + (fU * (sPosition2.Z - sPosition1.Z));
				}

				// Calculate the direction to attract the Particle to the closest point on the Line
				sDirectionToPushParticle = sClosestPointOnLine - cParticle.Position;
			}
			// Else If this is a Plane Magnet
			else if (cMagnet.MagnetType == DefaultParticleSystemMagnet.MagnetTypes.PlaneMagnet)
			{
				// Cast the Magnet to the proper type
				MagnetPlane cPlaneMagnet = (MagnetPlane)cMagnet;

				// Calculate the closest point on the Plane to the Particle.
				// Equation taken from http://ozviz.wasp.uwa.edu.au/~pbourke/geometry/pointline/

				// Calculate how far from the Plane the Particle is
				float fDistanceFromPlane = Vector3.Dot(cParticle.Position - cPlaneMagnet.PositionOnPlane, cPlaneMagnet.Normal);

				// Calculate the closest point on the Plane to the Particle
				Vector3 sClosestPointOnPlane = cParticle.Position + (-cPlaneMagnet.Normal * fDistanceFromPlane);

				// Calculate the direction to attract the Particle to the closest point on the Plane
				sDirectionToPushParticle = sClosestPointOnPlane - cParticle.Position;
			}
			// Else we don't know what kind of Magnet this is
			else
			{
				// So exit returning no force
				return Vector3.Zero;
			}

			// If the Particle should be Repelled away from the Magnet (instead of attracted to it)
			if (cMagnet.Mode == DefaultParticleSystemMagnet.MagnetModes.Repel)
			{
				// Reverse the direction we are going to push the Particle
				sDirectionToPushParticle *= -1;
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
		protected void UpdateParticleSystemEmitParticlesAutomaticallyOn(float fElapsedTimeInSeconds)
		{
			Emitter.EmitParticlesAutomatically = true;
		}

		/// <summary>
		/// Sets the Emitter to not Emit Particles Automatically
		/// </summary>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleSystemEmitParticlesAutomaticallyOff(float fElapsedTimeInSeconds)
		{
			Emitter.EmitParticlesAutomatically = false;
		}

		/// <summary>
		/// Enables the Emitter
		/// </summary>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleSystemEnableEmitter(float fElapsedTimeInSeconds)
		{
			Emitter.Enabled = true;
		}

		/// <summary>
		/// Disables the Emitter
		/// </summary>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleSystemDisableEmitter(float fElapsedTimeInSeconds)
		{
			Emitter.Enabled = false;
		}
	}
}
