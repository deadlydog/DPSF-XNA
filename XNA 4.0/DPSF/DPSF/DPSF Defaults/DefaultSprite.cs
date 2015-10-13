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
	/// The Default Sprite Particle System to inherit from, which uses Default Sprite Particles
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DefaultSpriteParticleSystem : DPSFDefaultSpriteParticleSystem<DefaultSpriteParticle, DefaultSpriteParticleVertex>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
		/// parameter if not using a Game object.</param>
		public DefaultSpriteParticleSystem(Game cGame) : base(cGame) { }
	}

	/// <summary>
	/// Particle used by the Default Sprite Particle System
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class DefaultSpriteParticle : DPSFDefaultBaseParticle
	{
		//===========================================================
		// Properties used to draw the Particle
		//===========================================================

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


		//===========================================================
		// Additional Particle Properties
		//===========================================================

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
	/// The Default Sprite Particle System class
	/// </summary>
	/// <typeparam name="Particle">The Particle class to use</typeparam>
	/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DPSFDefaultSpriteParticleSystem<Particle, Vertex> : DPSFDefaultBaseParticleSystem<Particle, Vertex>
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
	#if (WINDOWS)
		[Serializable]
	#endif
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

		/// <summary>
		/// Function to setup the Render Properties (i.e. BlendState, DepthStencilState, RasterizerState, and SamplerState)
		/// which will be applied to the Graphics Device before drawing the Particle System's Particles.
		/// <para>This function is called when initializing the particle system.</para>
		/// </summary>
		protected override void InitializeRenderProperties()
		{
			// Use the NonPremultiplied BlendState by default for Sprite particles so that transparency is drawn.
			RenderProperties.BlendState = DPSFHelper.CloneBlendState(BlendState.NonPremultiplied);
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
		protected void UpdateParticleRotationUsingRotationalVelocity(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Update the Particle's Rotation according to its Rotational Velocity
			cParticle.Rotation += cParticle.RotationalVelocity * fElapsedTimeInSeconds;
		}

		/// <summary>
		/// Update a Particle's Rotational Velocity according to its Rotational Acceleration
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleRotationalVelocityUsingRotationalAcceleration(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Update the Particle's Rotation according to its Rotational Velocity
			cParticle.RotationalVelocity += cParticle.RotationalAcceleration * fElapsedTimeInSeconds;
		}

		/// <summary>
		/// Update a Particle's Rotation and Rotational Velocity according to its Rotational Acceleration
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Update the Particle's Rotational Velocity and Rotation according to its Rotational Acceleration
			cParticle.RotationalVelocity += cParticle.RotationalAcceleration * fElapsedTimeInSeconds;
			cParticle.Rotation += cParticle.RotationalVelocity * fElapsedTimeInSeconds;
		}

		/// <summary>
		/// Linearly interpolate the Particle's Width between the Start and End Width according
		/// to the Particle's Normalized Lifetime
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleWidthUsingLerp(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
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
		protected void UpdateParticleHeightUsingLerp(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
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
		protected void UpdateParticleWidthAndHeightUsingLerp(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
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
		protected void UpdateParticleDepthFromBackToFrontUsingLerp(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
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
		protected void UpdateParticleDepthFromFrontToBackUsingLerp(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
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
		/// using a Shader (i.e. Effect and Technique).</para>
		/// <para>If you are not using a Shader and want the Particles sorted by Depth, use SpriteSortMode.BackToFront.</para>
		/// <para>Merge Sort is the sorting algorithm used, as it tends to be best for linked lists.
		/// TODO - WHILE MERGE SORT SHOULD BE USED, DUE TO TIME CONSTRAINTS A (PROBABLY) SLOWER METHOD (QUICK-SORT)
		/// IS BEING USED INSTEAD. THIS FUNCTION NEEDS TO BE UPDATED TO USE MERGE SORT STILL.
		/// THE LINKED LIST MERGE SORT ALGORITHM CAN BE FOUND AT http://www.chiark.greenend.org.uk/~sgtatham/algorithms/listsort.html</para>
		/// </summary>
		/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
		protected void UpdateParticleSystemToSortParticlesByDepth(float fElapsedTimeInSeconds)
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
				DPSFDefaultBaseParticle cParticle1 = (DPSFDefaultBaseParticle)(DPSFParticle)Particle1;
				DPSFDefaultBaseParticle cParticle2 = (DPSFDefaultBaseParticle)(DPSFParticle)Particle2;
				return cParticle1.Position.Z.CompareTo(cParticle2.Position.Z);
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
	/// Dummy structure used for the vertices of Default Sprites. Since Sprites are drawn using a 
	/// SpriteBatch, they do not have vertices, so this structure is empty.
	/// </summary>
#if (WINDOWS)
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
#endif
	public struct DefaultSpriteParticleVertex : IDPSFParticleVertex
	{
		/// <summary>
		/// An array describing the attributes of each Vertex
		/// </summary>
		public VertexDeclaration VertexDeclaration
		{
			get { return null; }
		}
	}
}
