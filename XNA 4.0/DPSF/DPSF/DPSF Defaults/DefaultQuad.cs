#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF
{
	/// <summary>
	/// The Default Quad Particle System to inherit from, which uses Default Quad Particles
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DefaultQuadParticleSystem : DPSFDefaultQuadParticleSystem<DefaultQuadParticle, DefaultQuadParticleVertex>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
		/// parameter if not using a Game object.</param>
		public DefaultQuadParticleSystem(Game cGame) : base(cGame) { }
	}

	/// <summary>
	/// Particle used by the Default Quad Particle System
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
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
		/// Sets the Width and Height properties to the given value.
		/// Gets the Width value, ignoring whether the Height value is the same or not.
		/// </summary>
		public float Size
		{
			get { return Width; }
			set { Width = value; Height = value; }
		}

		/// <summary>
		/// Sets the StartWidth and StartHeight properties to the given value.
		/// Gets the StartWidth value, ignoring whether the StartHeight value is the same or not.
		/// </summary>
		public float StartSize
		{
			get { return StartWidth; }
			set { StartWidth = value; StartHeight = value; }
		}

		/// <summary>
		/// Sets the EndWidth and EndHeight properties to the given value.
		/// Gets the EndWidth value, ignoring whether the EndHeight value is the same or not.
		/// </summary>
		public float EndSize
		{
			get { return EndWidth; }
			set { EndWidth = value; EndHeight = value; }
		}

		/// <summary>
		/// Scales the Width and Height by the given amount.
		/// </summary>
		/// <param name="scale">The amount to scale the Width and Height by.</param>
		public void Scale(float scale)
		{
			Width *= scale;
			Height *= scale;
		}

		/// <summary>
		/// Updates the Width to the given value and uniformly scales the Height to maintain the width-to-height ratio.
		/// </summary>
		/// <param name="newWidth">The Width the particle should have.</param>
		public void ScaleToWidth(float newWidth)
		{
			// If we won't be able to calculate the scale, just exit
			if (Width == 0.0f)
				return;

			float scale = newWidth / Width;
			Height *= scale;
			Width = newWidth;
		}

		/// <summary>
		/// Updates the Height to the given value and uniformly scales the Width to maintain the width-to-height ratio.
		/// </summary>
		/// <param name="newHeight">The Height the particle should have.</param>
		public void ScaleToHeight(float newHeight)
		{
			// If we won't be able to calculate the scale, just exit
			if (Height == 0.0f)
				return;

			float scale = newHeight / Height;
			Width *= scale;
			Height = newHeight;
		}

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
		/// The squared distance between this particle and the camera.
		/// <para>NOTE: This property is only used if you are sorting the particles based on their distance 
		/// from the camera, otherwise you can use this property for whatever you like.</para>
		/// </summary>
		public float DistanceFromCameraSquared;

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
            DistanceFromCameraSquared = 0f;
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
            this.DistanceFromCameraSquared = cParticleToCopy.DistanceFromCameraSquared;
		}
	}

	/// <summary>
	/// The Default Quad Particle System class
	/// </summary>
	/// <typeparam name="Particle">The Particle class to use</typeparam>
	/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DPSFDefaultQuadParticleSystem<Particle, Vertex> : DPSFDefaultBaseParticleSystem<Particle, Vertex>
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
	#if (WINDOWS)
		[Serializable]
	#endif
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
			/// The Min Start Size for the particle's StartWidth and StartHeight properties.
			/// <para>NOTE: If this is greater than zero, this will be used instead of 
			/// the StartWidthMin and StartHeightMin properties.</para>
			/// </summary>
			public float StartSizeMin = 0;

			/// <summary>
			/// The Max Start Size for the particle's StartWidth and StartHeight properties.
			/// <para>NOTE: If this is greater than zero, this will be used instead of 
			/// the StartWidthMax and StartHeightMax properties.</para>
			/// </summary>
			public float StartSizeMax = 0;

			/// <summary>
			/// The Min End Size for the particle's EndWidth and EndHeight properties.
			/// <para>NOTE: If this is greater than zero, this will be used instead of 
			/// the EndWidthMin and EndHeightMin properties.</para>
			/// </summary>
			public float EndSizeMin = 0;

			/// <summary>
			/// The Max End Size for the particle's EndWidth and EndHeight properties.
			/// <para>NOTE: If this is greater than zero, this will be used instead of 
			/// the EndWidthMax and EndHeightMax properties.</para>
			/// </summary>
			public float EndSizeMax = 0;

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
		/// <para>NOTE: This should be Set (updated) every frame if Billboarding will be used (i.e. Always have the Particles face the Camera).</para>
		/// </summary>
        public Vector3 CameraPosition { get; set; }

		//===========================================================
		// Vertex Update and Overridden Particle System Functions
		//===========================================================

		/// <summary>
		/// Function to update the Vertex properties according to the Particle properties
		/// </summary>
		/// <param name="sVertexBuffer">The array containing the Vertices to be drawn</param>
		/// <param name="iIndex">The Index in the array where the Particle's Vertex info should be placed</param>
		/// <param name="Particle">The Particle to copy the information from</param>
		protected virtual void UpdateVertexProperties(ref DefaultQuadParticleVertex[] sVertexBuffer, int iIndex, DPSFParticle Particle)
		{
			// Cast the Particle to the type it really is
			DefaultQuadParticle cParticle = (DefaultQuadParticle)Particle;

			// Calculate what half of the Quads Width and Height are
			float fHalfWidth = cParticle.Width / 2.0f;
			float fHalfHeight = cParticle.Height / 2.0f;

			// Calculate the Positions of the Quads corners around the origin
			Vector3 sTopLeft = new Vector3(-fHalfWidth, -fHalfHeight, 0);
			Vector3 sTopRight = new Vector3(fHalfWidth, -fHalfHeight, 0);
			Vector3 sBottomLeft = new Vector3(-fHalfWidth, fHalfHeight, 0);
			Vector3 sBottomRight = new Vector3(fHalfWidth, fHalfHeight, 0);

			// Rotate the Quad corners around the origin according to its Orientation, 
			// then calculate their final Positions
			sTopLeft = Vector3.Transform(sTopLeft, cParticle.Orientation) + cParticle.Position;
			sTopRight = Vector3.Transform(sTopRight, cParticle.Orientation) + cParticle.Position;
			sBottomLeft = Vector3.Transform(sBottomLeft, cParticle.Orientation) + cParticle.Position;
			sBottomRight = Vector3.Transform(sBottomRight, cParticle.Orientation) + cParticle.Position;

            // Effect expects a premultiplied color, so get the actual color to use.
            Color premultipliedColor = cParticle.ColorAsPremultiplied;

			// Copy this Particle's renderable Properties to the Vertex Buffer
			// This is a Quad so we must copy all 4 Vertices over
			sVertexBuffer[iIndex].Position = sBottomLeft;
			sVertexBuffer[iIndex].Color = premultipliedColor;

			sVertexBuffer[iIndex + 1].Position = sTopLeft;
            sVertexBuffer[iIndex + 1].Color = premultipliedColor;

			sVertexBuffer[iIndex + 2].Position = sBottomRight;
            sVertexBuffer[iIndex + 2].Color = premultipliedColor;

			sVertexBuffer[iIndex + 3].Position = sTopRight;
            sVertexBuffer[iIndex + 3].Color = premultipliedColor;

            // Fill in the Index Buffer for the newly added Vertices.
            // Specify the Vertices in Clockwise order.
            // It takes 6 Indices to represent a quad (2 triangles = 6 corners).
            // If we should be using the 32-bit Integer Index Buffer, fill it.
            if (this.IsUsingIntegerIndexBuffer)
            {
                IndexBuffer[IndexBufferIndex++] = iIndex + 1;
                IndexBuffer[IndexBufferIndex++] = iIndex + 2;
                IndexBuffer[IndexBufferIndex++] = iIndex;
                IndexBuffer[IndexBufferIndex++] = iIndex + 1;
                IndexBuffer[IndexBufferIndex++] = iIndex + 3;
                IndexBuffer[IndexBufferIndex++] = iIndex + 2;
            }
            // Else we should be using the 16-bit Short Index Buffer.
            else
            {
                IndexBufferShort[IndexBufferIndex++] = (short)(iIndex + 1);
                IndexBufferShort[IndexBufferIndex++] = (short)(iIndex + 2);
                IndexBufferShort[IndexBufferIndex++] = (short)(iIndex);
                IndexBufferShort[IndexBufferIndex++] = (short)(iIndex + 1);
                IndexBufferShort[IndexBufferIndex++] = (short)(iIndex + 3);
                IndexBufferShort[IndexBufferIndex++] = (short)(iIndex + 2);
            }
		}

		/// <summary>
		/// Virtual function to Set the Effect's Parameters before drawing the Particles
		/// </summary>
        protected override void SetEffectParameters()
        {
            // Specify the World, View, and Projection Matrices
            BasicEffect effect = this.Effect as BasicEffect;
            if (effect == null) return;

            // Specify the World, View, and Projection Matrices to use
            effect.World = this.World;
            effect.View = this.View;
            effect.Projection = this.Projection;
            effect.VertexColorEnabled = true;       // Enable tinting the texture's color.

            // Disable unused properties
            effect.TextureEnabled = false;
            effect.FogEnabled = false;
            effect.LightingEnabled = false;
        }

		/// <summary>
		/// Sets the camera position, so that the particles know how to make themselves face the camera if needed.
		/// (i.e. you add the Particle EveryTimeEvent UpdateParticleToFaceTheCamera).
		/// </summary>
		/// <param name="cameraPosition">The camera position.</param>
		public override void SetCameraPosition(Vector3 cameraPosition)
		{
			this.CameraPosition = cameraPosition;
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
			cParticle.StartWidth = DPSFHelper.RandomNumberBetween(mcInitialProperties.StartSizeMin > 0 ? mcInitialProperties.StartSizeMin : mcInitialProperties.StartWidthMin, mcInitialProperties.StartSizeMax > 0 ? mcInitialProperties.StartSizeMax : mcInitialProperties.StartWidthMax);
			cParticle.EndWidth = DPSFHelper.RandomNumberBetween(mcInitialProperties.EndSizeMin > 0 ? mcInitialProperties.EndSizeMin : mcInitialProperties.EndWidthMin, mcInitialProperties.EndSizeMax > 0 ? mcInitialProperties.EndSizeMax : mcInitialProperties.EndWidthMax);
			cParticle.StartHeight = DPSFHelper.RandomNumberBetween(mcInitialProperties.StartSizeMin > 0 ? mcInitialProperties.StartSizeMin : mcInitialProperties.StartHeightMin, mcInitialProperties.StartSizeMax > 0 ? mcInitialProperties.StartSizeMax : mcInitialProperties.StartHeightMax);
			cParticle.EndHeight = DPSFHelper.RandomNumberBetween(mcInitialProperties.EndSizeMin > 0 ? mcInitialProperties.EndSizeMin : mcInitialProperties.EndHeightMin, mcInitialProperties.EndSizeMax > 0 ? mcInitialProperties.EndSizeMax : mcInitialProperties.EndHeightMax);
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
		protected void UpdateParticleRotationUsingRotationalVelocity(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
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
		protected void UpdateParticleRotationalVelocityUsingRotationalAcceleration(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
		{
			// If Rotational Acceleration is being used
			if (cParticle.RotationalAcceleration != Vector3.Zero)
			{
				// Update the Particle's Rotational Velocity according to its Rotational Acceleration
				cParticle.RotationalVelocity += cParticle.RotationalAcceleration * fElapsedTimeInSeconds;
			}
		}

		/// <summary>
		/// Update a Particle's Rotation and Rotational Velocity according to its Rotational Acceleration
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Update the Particle's Rotational Velocity and Rotation according to its Rotational Acceleration
			UpdateParticleRotationalVelocityUsingRotationalAcceleration(cParticle, fElapsedTimeInSeconds);
			UpdateParticleRotationUsingRotationalVelocity(cParticle, fElapsedTimeInSeconds);
		}

		/// <summary>
		/// Linearly interpolate the Particle's Width between the Start and End Width according
		/// to the Particle's Normalized Lifetime
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleWidthUsingLerp(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
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
		protected void UpdateParticleHeightUsingLerp(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
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
		protected void UpdateParticleWidthAndHeightUsingLerp(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Calculate the Particle's new Width and Height
			cParticle.Width = MathHelper.Lerp(cParticle.StartWidth, cParticle.EndWidth, cParticle.NormalizedElapsedTime);
			cParticle.Height = MathHelper.Lerp(cParticle.StartHeight, cParticle.EndHeight, cParticle.NormalizedElapsedTime);
		}

		/// <summary>
		/// Turns the Particle into a Billboard Particle (i.e. The Particle always faces the Camera).
		/// <para>NOTE: This Update function should be called after all other Update functions to ensure that 
		/// the Particle is orientated correctly.</para>
		/// <para>NOTE: Update the Particle System's Camera Position every frame to ensure that this works correctly.</para>
		/// <para>NOTE: Only Roll Rotations (i.e. around the Z axis) will be visible when this is used.</para>
		/// </summary>
		/// <param name="cParticle">The Particle to update.</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update.</param>
		protected void UpdateParticleToFaceTheCamera(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Make the Particle face the Camera
			cParticle.Normal = CameraPosition - cParticle.Position;
		}

		/// <summary>
		/// Orientates the Particle to face the camera, but constrains the particle to always be perpendicular to the 
		/// Y-Z plane.
		/// </summary>
		/// <param name="cParticle">The Particle to update.</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update.</param>
		protected void UpdateParticleToBeConstrainedAroundXAxis(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
		{
			Vector3 newNormal = CameraPosition - cParticle.Position;
			newNormal.X = 0;
			cParticle.Normal = newNormal;
		}

		/// <summary>
		/// Orientates the Particle to face the camera, but constrains the particle to always be perpendicular to the 
		/// X-Z plane (i.e standing straight up).
		/// </summary>
		/// <param name="cParticle">The Particle to update.</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update.</param>
		protected void UpdateParticleToBeConstrainedAroundYAxis(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
		{
			Vector3 newNormal = CameraPosition - cParticle.Position;
			newNormal.Y = 0;
			cParticle.Normal = newNormal;
		}

		/// <summary>
		/// Orientates the Particle to face the camera, but constrains the particle to always be perpendicular to the 
		/// X-Y plane.
		/// </summary>
		/// <param name="cParticle">The Particle to update.</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update.</param>
		protected void UpdateParticleToBeConstrainedAroundZAxis(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
		{
			Vector3 newNormal = CameraPosition - cParticle.Position;
			newNormal.Z = 0;
			cParticle.Normal = newNormal;
		}

		/// <summary>
		/// Updates the Particle's DistanceFromCameraSquared property to reflect how far this Particle is from the Camera.
		/// </summary>
		/// <param name="cParticle">The Particle to update.</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update.</param>
		protected void UpdateParticleDistanceFromCameraSquared(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
		{
			cParticle.DistanceFromCameraSquared = Vector3.DistanceSquared(this.CameraPosition, cParticle.Position);
		}

		//===========================================================
		// Particle System Update Functions
		//===========================================================

		/// <summary>
		/// Sorts the particles to draw particles furthest from the camera first, in order to achieve proper depth perspective.
		/// 
		/// <para>NOTE: This operation is very expensive and should only be used when you are
		/// drawing particles with both opaque and semi-transparent portions, and not using additive blending.</para>
		/// <para>Merge Sort is the sorting algorithm used, as it tends to be best for linked lists.
		/// TODO - WHILE MERGE SORT SHOULD BE USED, DUE TO TIME CONSTRAINTS A (PROBABLY) SLOWER METHOD (QUICK-SORT)
		/// IS BEING USED INSTEAD. THIS FUNCTION NEEDS TO BE UPDATED TO USE MERGE SORT STILL.
		/// THE LINKED LIST MERGE SORT ALGORITHM CAN BE FOUND AT http://www.chiark.greenend.org.uk/~sgtatham/algorithms/listsort.html</para>
		/// </summary>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleSystemToSortParticlesByDistanceFromCamera(float fElapsedTimeInSeconds)
		{
			// Store the Number of Active Particles to sort
			int iNumberOfActiveParticles = ActiveParticles.Count;

			// If there is nothing to sort
			if (iNumberOfActiveParticles <= 1)
			{
				// Exit without sorting
				return;
			}

			// Create a List to put the Active Particles in to be sorted
			List<Particle> cActiveParticleList = new List<Particle>(iNumberOfActiveParticles);

			// Add all of the Particles to the List
			LinkedListNode<Particle> cNode = ActiveParticles.First;
			while (cNode != null)
			{
				// Copy this Particle into the Array
				cActiveParticleList.Add(cNode.Value);

				// Move to the next Active Particle
				cNode = cNode.Next;
			}

			// Now that the List is full, sort it
			cActiveParticleList.Sort(delegate(Particle Particle1, Particle Particle2)
			{
				DefaultQuadParticle cParticle1 = (DefaultQuadParticle)(DPSFParticle)Particle1;
				DefaultQuadParticle cParticle2 = (DefaultQuadParticle)(DPSFParticle)Particle2;
				return cParticle1.DistanceFromCameraSquared.CompareTo(cParticle2.DistanceFromCameraSquared);
			});
			
			// Now that the List is sorted, add the Particles into the Active Particles Linked List in sorted order
			ActiveParticles.Clear();
			for (int iIndex = 0; iIndex < iNumberOfActiveParticles; iIndex++)
			{
				// Add this Particle to the Active Particles Linked List.
				// List is sorted from smallest to largest, but we want
				// our Linked List sorted from largest to smallest, since
				// the Particles at the end of the Linked List are drawn last.
				ActiveParticles.AddFirst(cActiveParticleList[iIndex]);
			}
		}
	}

	/// <summary>
	/// Structure used to hold a Default Quad Particle's Vertex's properties used for drawing.
	/// This contains a Vector3 Position and a Color Color.
	/// </summary>
#if (WINDOWS)
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
#endif
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
		private static readonly VertexDeclaration vertexDeclaration = new VertexDeclaration
		(
			new VertexElement(0, VertexElementFormat.Vector3,
									VertexElementUsage.Position, 0),

			new VertexElement(12, VertexElementFormat.Color,
									 VertexElementUsage.Color, 0)
		);

		/// <summary>
		/// An array describing the attributes of each Vertex
		/// </summary>
		public VertexDeclaration VertexDeclaration
		{
			get { return DefaultQuadParticleVertex.vertexDeclaration; }
		}
	}
}
