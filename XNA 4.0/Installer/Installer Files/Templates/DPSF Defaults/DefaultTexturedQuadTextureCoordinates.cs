#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF
{
	/// <summary>
	/// The Default Textured Quad with Texture Coordinates Particle System to inherit from, which uses Default Textured Quad Texture Coordinates Particles
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DefaultTexturedQuadTextureCoordinatesParticleSystem : DPSFDefaultTexturedQuadTextureCoordinatesParticleSystem<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
		/// parameter if not using a Game object.</param>
		public DefaultTexturedQuadTextureCoordinatesParticleSystem(Game cGame) : base(cGame) { }
	}

	/// <summary>
	/// Particle used by the Default Textured Quad with Texture Coordinates Particle System
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class DefaultTextureQuadTextureCoordinatesParticle : DefaultTexturedQuadParticle
	{
		/// <summary>
		/// The Normalized (0.0 - 1.0) Top-Left Texture Coordinate to use for the Particle's image
		/// </summary>
		public Vector2 NormalizedTextureCoordinateLeftTop;

		/// <summary>
		/// The Normalized (0.0 - 1.0) Bottom-Right Texture Coordinate to use for the Particle's image
		/// </summary>
		public Vector2 NormalizedTextureCoordinateRightBottom;

		/// <summary>
		/// Sets the Normalized Texture Coordinates using the absolute (i.e. non-normalized) top-left coordinate and the dimensions of the Picture in the Texture
		/// </summary>
		/// <param name="sTextureCoordinates">The top-left Position and the Dimensions of the Picture in the Texture</param>
		/// <param name="iTextureWidth">The Width of the Texture that the Picture is in</param>
		/// <param name="iTextureHeight">The Height of the Texture that the Picture is in</param>
		public void SetTextureCoordinates(Rectangle sTextureCoordinates, int iTextureWidth, int iTextureHeight)
		{
			SetTextureCoordinates(sTextureCoordinates.Left, sTextureCoordinates.Top, sTextureCoordinates.Right, sTextureCoordinates.Bottom, iTextureWidth, iTextureHeight);
		}

		/// <summary>
		/// Sets the Normalized Texture Coordinates using the absolute (i.e. non-normalized) coordinates of the Picture in the Texture
		/// </summary>
		/// <param name="iLeft">The X position of the top-left corner of the Picture in the Texture</param>
		/// <param name="iTop">The Y position of the top-left corner of the Picture in the Texture</param>
		/// <param name="iRight">The X position of the bottom-right corner of the Picture in the Texture</param>
		/// <param name="iBottom">The Y position of the bottom-right corner of the Picture in the Texture</param>
		/// <param name="iTextureWidth">The Width of the Texture that the Picture is in</param>
		/// <param name="iTextureHeight">The Height of the Texture that the Picture is in</param>
		public void SetTextureCoordinates(int iLeft, int iTop, int iRight, int iBottom, int iTextureWidth, int iTextureHeight)
		{
			// Calculate and set the Normalized Texture Coordinates
			NormalizedTextureCoordinateLeftTop.X = (float)iLeft / (float)iTextureWidth;
			NormalizedTextureCoordinateLeftTop.Y = (float)iTop / (float)iTextureHeight;
			NormalizedTextureCoordinateRightBottom.X = (float)iRight / (float)iTextureHeight;
			NormalizedTextureCoordinateRightBottom.Y = (float)iBottom / (float)iTextureHeight;
		}

		/// <summary>
		/// Resets the Particle variables to their default values
		/// </summary>
		public override void Reset()
		{
			base.Reset();
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
			DefaultTextureQuadTextureCoordinatesParticle cParticleToCopy = (DefaultTextureQuadTextureCoordinatesParticle)ParticleToCopy;

			base.CopyFrom(cParticleToCopy);
			this.NormalizedTextureCoordinateLeftTop = cParticleToCopy.NormalizedTextureCoordinateLeftTop;
			this.NormalizedTextureCoordinateRightBottom = cParticleToCopy.NormalizedTextureCoordinateRightBottom;
		}
	}

	/// <summary>
	/// The Default Textured Quad with Texture Coordinates Particle System class
	/// </summary>
	/// <typeparam name="Particle">The Particle class to use</typeparam>
	/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
#if (WINDOWS)
	[Serializable]
#endif
	public abstract class DPSFDefaultTexturedQuadTextureCoordinatesParticleSystem<Particle, Vertex> : DPSFDefaultTexturedQuadParticleSystem<Particle, Vertex>
		where Particle : DPSFParticle, new()
		where Vertex : struct, IDPSFParticleVertex
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
		/// parameter if not using a Game object.</param>
		public DPSFDefaultTexturedQuadTextureCoordinatesParticleSystem(Game cGame) : base(cGame) { }

		//===========================================================
		// Vertex Update and Overridden Particle System Functions
		//===========================================================

		/// <summary>
		/// Function to update the Vertex properties according to the Particle properties
		/// </summary>
		/// <param name="sVertexBuffer">The array containing the Vertices to be drawn</param>
		/// <param name="iIndex">The Index in the array where the Particle's Vertex info should be placed</param>
		/// <param name="Particle">The Particle to copy the information from</param>
		protected override void UpdateVertexProperties(ref DefaultTexturedQuadParticleVertex[] sVertexBuffer, int iIndex, DPSFParticle Particle)
		{
			// Cast the Particle to the type it really is
			DefaultTextureQuadTextureCoordinatesParticle cParticle = (DefaultTextureQuadTextureCoordinatesParticle)Particle;

			// Calculate what half of the Quads Width and Height are
			float fHalfWidth = cParticle.Width / 2.0f;
			float fHalfHeight = cParticle.Height / 2.0f;

			// Calculate the Positions of the Quads corners around the origin
			Vector3 sTopLeft = new Vector3(-fHalfWidth, fHalfHeight, 0);
			Vector3 sTopRight = new Vector3(fHalfWidth, fHalfHeight, 0); ;
			Vector3 sBottomLeft = new Vector3(-fHalfWidth, -fHalfHeight, 0); ;
			Vector3 sBottomRight = new Vector3(fHalfWidth, -fHalfHeight, 0); ;

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
			sVertexBuffer[iIndex].TextureCoordinate = new Vector2(cParticle.NormalizedTextureCoordinateLeftTop.X, cParticle.NormalizedTextureCoordinateRightBottom.Y);
            sVertexBuffer[iIndex].Color = premultipliedColor;

			sVertexBuffer[iIndex + 1].Position = sTopLeft;
			sVertexBuffer[iIndex + 1].TextureCoordinate = cParticle.NormalizedTextureCoordinateLeftTop;
            sVertexBuffer[iIndex + 1].Color = premultipliedColor;

			sVertexBuffer[iIndex + 2].Position = sBottomRight;
			sVertexBuffer[iIndex + 2].TextureCoordinate = cParticle.NormalizedTextureCoordinateRightBottom;
            sVertexBuffer[iIndex + 2].Color = premultipliedColor;

			sVertexBuffer[iIndex + 3].Position = sTopRight;
			sVertexBuffer[iIndex + 3].TextureCoordinate = new Vector2(cParticle.NormalizedTextureCoordinateRightBottom.X, cParticle.NormalizedTextureCoordinateLeftTop.Y);
            sVertexBuffer[iIndex + 3].Color = premultipliedColor;

            // Fill in the Index Buffer for the newly added Vertices.
            // Specify the Vertices in Counter-Clockwise order.
            // It takes 6 Indices to represent a quad (2 triangles = 6 corners).
            // If we should be using the 32-bit Integer Index Buffer, fill it.
			if (this.IsUsingIntegerIndexBuffer)
            {
                IndexBuffer[IndexBufferIndex++] = iIndex;
                IndexBuffer[IndexBufferIndex++] = iIndex + 2;
                IndexBuffer[IndexBufferIndex++] = iIndex + 1;
                IndexBuffer[IndexBufferIndex++] = iIndex + 2;
                IndexBuffer[IndexBufferIndex++] = iIndex + 3;
                IndexBuffer[IndexBufferIndex++] = iIndex + 1;
            }
            // Else we should be using the 16-bit Short Index Buffer.
            else
            {
                IndexBufferShort[IndexBufferIndex++] = (short)(iIndex);
                IndexBufferShort[IndexBufferIndex++] = (short)(iIndex + 2);
                IndexBufferShort[IndexBufferIndex++] = (short)(iIndex + 1);
                IndexBufferShort[IndexBufferIndex++] = (short)(iIndex + 2);
                IndexBufferShort[IndexBufferIndex++] = (short)(iIndex + 3);
                IndexBufferShort[IndexBufferIndex++] = (short)(iIndex + 1);
            }
		}
	}
}
