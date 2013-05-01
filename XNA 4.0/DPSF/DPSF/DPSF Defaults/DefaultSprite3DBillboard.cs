#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF
{
    /// <summary>
    /// The Default Sprite 3D Billboard Particle System to inherit from, which uses Default Sprite 3D Billboard Particles
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
	public abstract class DefaultSprite3DBillboardParticleSystem : DPSFDefaultSprite3DBillboardParticleSystem<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DefaultSprite3DBillboardParticleSystem(Game cGame) : base(cGame) { }
    }

    /// <summary>
	/// Particle used by the Default Sprite 3D Billboard Particle System
	/// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    public class DefaultSprite3DBillboardParticle : DefaultSpriteParticle
    {
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
            DistanceFromCameraSquared = 0f;
        }

        /// <summary>
        /// Deep copy all of the Particle properties
        /// </summary>
        /// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
        public override void CopyFrom(DPSFParticle ParticleToCopy)
        {
            // Cast the Particle to the type it really is
            DefaultSprite3DBillboardParticle cParticleToCopy = (DefaultSprite3DBillboardParticle)ParticleToCopy;

            base.CopyFrom(cParticleToCopy);
            this.DistanceFromCameraSquared = cParticleToCopy.DistanceFromCameraSquared;
        }
    }

    /// <summary>
    /// The Default Sprite 3D Billboard Particle System class.
    /// This class just inherits from the Default Sprite Particle System class and overrides the DrawSprite()
    /// function to draw the sprites as Billboards in 3D space.
    /// </summary>
    /// <typeparam name="Particle">The Particle class to use.</typeparam>
    /// <typeparam name="Vertex">The Vertex Format to use.</typeparam>
#if (WINDOWS)
    [Serializable]
#endif
	public abstract class DPSFDefaultSprite3DBillboardParticleSystem<Particle, Vertex> : DPSFDefaultSpriteParticleSystem<Particle, Vertex>
        where Particle : DPSFParticle, new()
        where Vertex : struct, IDPSFParticleVertex
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DPSFDefaultSprite3DBillboardParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        /// <summary>
        /// Get / Set the Position of the Camera.
        /// <para>NOTE: 3D Billboard Sprite particles always face the camera by using the View Matrix, so this only needs to be Set (updated) every frame if you plan on 
        /// sorting these particles relative to one another by their distance from the camera, in order to give proper depth perception of the particles.</para>
        /// </summary>
        public Vector3 CameraPosition { get; set; }

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
            DefaultSprite3DBillboardParticle particle = (DefaultSprite3DBillboardParticle)Particle;

            // Calculate the sprites position in 3D space, and flip it vertically so that it is not upside-down.
            Vector3 viewSpacePosition = Vector3.Transform(particle.Position, this.View);

            // Get the Position of the Particle relative to the camera
            Vector2 destination = new Vector2(viewSpacePosition.X, viewSpacePosition.Y);

            // Get the Position and Dimensions from the Texture to use for this Sprite
            Rectangle sourceFromTexture = new Rectangle(0, 0, Texture.Width, Texture.Height);

            // Calculate how much to scale the sprite to get it to the desired Width and Height.
            // Use negative height in order to flip the texture to be right-side up.
            Vector2 scale = new Vector2(particle.Width / sourceFromTexture.Width, -particle.Height / sourceFromTexture.Height);

            // Make the Sprite rotate about its center
            Vector2 origin = new Vector2(sourceFromTexture.Width / 2, sourceFromTexture.Height / 2);

            // Draw the Sprite
            cSpriteBatch.Draw(Texture, destination, sourceFromTexture, particle.ColorAsPremultiplied, particle.Rotation, origin, scale, particle.FlipMode, viewSpacePosition.Z);
        }

        /// <summary>
        /// Function to setup the Render Properties (i.e. BlendState, DepthStencilState, RasterizerState, and SamplerState)
        /// which will be applied to the Graphics Device before drawing the Particle System's Particles.
        /// <para>This function is called when initializing the particle system.</para>
        /// </summary>
        protected override void InitializeRenderProperties()
        {
            // We use the Alpha Test Effect by default for 3D Sprites
            this.Effect = new AlphaTestEffect(this.GraphicsDevice);
            RenderProperties.RasterizerState = DPSFHelper.CloneRasterizerState(RasterizerState.CullNone);
        }

        /// <summary>
        /// Function to set the Shader's global variables before drawing
        /// </summary>
        protected override void SetEffectParameters()
        {
            // Use the AlphaTestEffect so we can take advantage of Alpha Testing automatically.
            AlphaTestEffect effect = this.Effect as AlphaTestEffect;
            if (effect != null)
            {
                effect.World = this.World;
                effect.View = Matrix.Identity;          // Don't set the Effect's View Matrix, as we'll manually apply the view matrix to the sprite in the DrawSprite() function.
                effect.Projection = this.Projection;
                effect.VertexColorEnabled = true;       // Enable tinting the texture's color.
            }
        }

        /// <summary>
        /// Sets the camera position, so that the particles know how far they are from the camera and can be properly sorted.
		/// <para>NOTE: 3D Billboard Sprite particles always face the camera, so this only needs to be Set (updated) every frame if you plan on 
		/// sorting these particles relative to one another by their distance from the camera, in order to give proper depth perception of the particles
		/// (i.e. by adding the Particle EveryTimeEvent UpdateParticleDistanceFromCameraSquared and the Particle System EveryTimeEvent UpdateParticleSystemToSortParticlesByDistanceFromCamera).</para>
        /// </summary>
        /// <param name="cameraPosition">The camera position.</param>
        public override void SetCameraPosition(Vector3 cameraPosition)
        {
            this.CameraPosition = cameraPosition;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        /// <summary>
        /// Updates the Particle's DistanceFromCameraSquared property to reflect how far this Particle is from the Camera.
        /// </summary>
        /// <param name="cParticle">The Particle to update.</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update.</param>
        protected void UpdateParticleDistanceFromCameraSquared(DefaultSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
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
                DefaultSprite3DBillboardParticle cParticle1 = (DefaultSprite3DBillboardParticle)(DPSFParticle)Particle1;
                DefaultSprite3DBillboardParticle cParticle2 = (DefaultSprite3DBillboardParticle)(DPSFParticle)Particle2;
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
}
