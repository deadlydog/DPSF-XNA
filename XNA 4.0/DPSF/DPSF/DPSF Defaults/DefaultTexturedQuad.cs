#region Using Statements
using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF
{
	/// <summary>
	/// The Default Textured Quad Particle System to inherit from, which uses Default Textured Quad Particles
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DefaultTexturedQuadParticleSystem : DPSFDefaultTexturedQuadParticleSystem<DefaultTexturedQuadParticle, DefaultTexturedQuadParticleVertex>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
		/// parameter if not using a Game object.</param>
		public DefaultTexturedQuadParticleSystem(Game cGame) : base(cGame) { }
	}

	/// <summary>
	/// Particle used by the Default Textured Quad Particle System
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class DefaultTexturedQuadParticle : DefaultQuadParticle
	{ }

	/// <summary>
	/// The Default Textured Quad Particle System class
	/// </summary>
	/// <typeparam name="Particle">The Particle class to use</typeparam>
	/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DPSFDefaultTexturedQuadParticleSystem<Particle, Vertex> : DPSFDefaultQuadParticleSystem<Particle, Vertex>
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
		protected virtual void UpdateVertexProperties(ref DefaultTexturedQuadParticleVertex[] sVertexBuffer, int iIndex, DPSFParticle Particle)
		{
			// Cast the Particle to the type it really is
			DefaultTexturedQuadParticle cParticle = (DefaultTexturedQuadParticle)Particle;

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

            // Effects expect a premultiplied color, so get the actual color to use.
            Color premultipliedColor = cParticle.ColorAsPremultiplied;

			// Copy this Particle's renderable Properties to the Vertex Buffer
			// This is a Quad so we must copy all 4 Vertices over
			sVertexBuffer[iIndex].Position = sBottomLeft;
			sVertexBuffer[iIndex].TextureCoordinate = new Vector2(0, 1);
            sVertexBuffer[iIndex].Color = premultipliedColor;

			sVertexBuffer[iIndex + 1].Position = sTopLeft;
			sVertexBuffer[iIndex + 1].TextureCoordinate = new Vector2(0, 0);
            sVertexBuffer[iIndex + 1].Color = premultipliedColor;

			sVertexBuffer[iIndex + 2].Position = sBottomRight;
			sVertexBuffer[iIndex + 2].TextureCoordinate = new Vector2(1, 1);
            sVertexBuffer[iIndex + 2].Color = premultipliedColor;

			sVertexBuffer[iIndex + 3].Position = sTopRight;
			sVertexBuffer[iIndex + 3].TextureCoordinate = new Vector2(1, 0);
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
            AlphaTestEffect effect = this.Effect as AlphaTestEffect;
            if (effect == null) return;

            // Specify the World, View, and Projection Matrices to use, as well as the Texture to use
            effect.World = this.World;
            effect.View = this.View;
            effect.Projection = this.Projection;
            effect.Texture = this.Texture;
            effect.VertexColorEnabled = true;       // Enable tinting the texture's color.
        }

		//===========================================================
		// Initialization Function
		//===========================================================

		//===========================================================
		// Particle Update Functions
		//===========================================================
	}

	/// <summary>
	/// Structure used to hold a Default Textured Quad Particle's Vertex's properties used for drawing.
	/// This contains a Vector3 Position, Vector2 TextureCoordinate, and Color Color.
	/// </summary>
#if (WINDOWS)
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
#endif
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
		private static readonly VertexDeclaration vertexDeclaration = new VertexDeclaration
		(
			new VertexElement(0, VertexElementFormat.Vector3,
									VertexElementUsage.Position, 0),

			new VertexElement(12, VertexElementFormat.Vector2,
									 VertexElementUsage.TextureCoordinate, 0),

			new VertexElement(20, VertexElementFormat.Color,
									 VertexElementUsage.Color, 0)
		);

		/// <summary>
		/// An array describing the attributes of each Vertex
		/// </summary>
		public VertexDeclaration VertexDeclaration
		{
			get { return DefaultTexturedQuadParticleVertex.vertexDeclaration; }
		}
	}
}
