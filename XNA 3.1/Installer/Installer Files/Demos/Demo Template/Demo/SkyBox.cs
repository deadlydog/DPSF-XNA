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

namespace Demo
{
    class SkyBox
    {
        // The Graphics Device and Effect used to draw the Terrain
        GraphicsDevice mcGraphicsDevice = null;
        public BasicEffect mcEffect = null;

        // Vertex and Index Buffer variables
        VertexBuffer mcVertexBuffer = null;
        VertexDeclaration mcVertexDeclaration = null;
        IndexBuffer mcIndexBuffer = null;

        // The Sky Box Textures
        Texture2D[] mcaTextures = new Texture2D[6];

        // The Length, Width, and Height of the Sky Box
        int miSize = 5000;
        int miHeightOffset = 0;

        public void Load(GraphicsDevice cGraphicsDevice, int iSize, int iHeightOffset, 
                Texture2D cBack, Texture2D cFront, Texture2D cBottom, Texture2D cTop, Texture2D cLeft, Texture2D cRight)
        {
            // Save a handle to the Graphics Device to draw to
            mcGraphicsDevice = cGraphicsDevice;

            // Create the Effect used to draw the Terrain
            mcEffect = new BasicEffect(mcGraphicsDevice, null);

            // Save the Size and Height Offset of the Sky Box
            miSize = iSize;
            miHeightOffset = iHeightOffset;

            // Save handles to the Sky Box Textures
            mcaTextures[0] = cBack;
            mcaTextures[1] = cFront;
            mcaTextures[2] = cBottom;
            mcaTextures[3] = cTop;
            mcaTextures[4] = cLeft;
            mcaTextures[5] = cRight;

            // Create the Sky Box and copy it to the Vertex and Index Buffers
            SetupVertices();
            SetupIndices();

            // Setup the Effect
            mcEffect.TextureEnabled = true;
        }

        public void Draw(Vector3 sCameraPosition, Matrix sWorld, Matrix sView, Matrix sProjection)
        {
            // If there is nothing to Draw
            if (mcVertexBuffer == null)
            {
                // Exit without Drawing anything
                return;
            }

            // Translate the Sky Box to keep it at a constant distance from the Camera
            sWorld *= Matrix.CreateTranslation(sCameraPosition);

            // Raise the Sky Box by the specified Hight Offset
            sWorld *= Matrix.CreateTranslation(0, miHeightOffset, 0);

            // Setup the World, View, and Projection Matrices
            mcEffect.World = sWorld;
            mcEffect.View = sView;
            mcEffect.Projection = sProjection;

            mcEffect.Begin();

            // Set the Vertex and Index Buffers to use
            mcGraphicsDevice.VertexDeclaration = mcVertexDeclaration;
            mcGraphicsDevice.Vertices[0].SetSource(mcVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
            mcGraphicsDevice.Indices = mcIndexBuffer;

            // Loop through all 6 sides of the Sky Box
            for (int iSide = 0; iSide < 6; iSide++)
            {
                // Set the propert Texture to use to draw this Side of the Sky Box
                mcEffect.Texture = mcaTextures[iSide];

                // Draw this Side of the Sky Box
                mcEffect.Techniques[0].Passes[0].Begin();
                    mcGraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, iSide * 4, 4, iSide * 6, 2);
                mcEffect.Techniques[0].Passes[0].End();
            }

            mcEffect.End();
        }

        private void SetupVertices()
        {
            // Create array to hold the Vertices
            VertexPositionTexture[] saVertices = new VertexPositionTexture[4 * 6];

            // Calculate Half of the Sky Boxes Size
            Vector3 sHalfSkyBoxSize = new Vector3(miSize / 2, miSize / 2, miSize / 2);

            // Back of Sky Box
            saVertices[0].Position = new Vector3(sHalfSkyBoxSize.X, -sHalfSkyBoxSize.Y, -sHalfSkyBoxSize.Z);
            saVertices[0].TextureCoordinate.X = 1.0f; saVertices[0].TextureCoordinate.Y = 1.0f;
            saVertices[1].Position = new Vector3(sHalfSkyBoxSize.X, sHalfSkyBoxSize.Y, -sHalfSkyBoxSize.Z);
            saVertices[1].TextureCoordinate.X = 1.0f; saVertices[1].TextureCoordinate.Y = 0.0f;
            saVertices[2].Position = new Vector3(-sHalfSkyBoxSize.X, sHalfSkyBoxSize.Y, -sHalfSkyBoxSize.Z);
            saVertices[2].TextureCoordinate.X = 0.0f; saVertices[2].TextureCoordinate.Y = 0.0f;
            saVertices[3].Position = new Vector3(-sHalfSkyBoxSize.X, -sHalfSkyBoxSize.Y, -sHalfSkyBoxSize.Z);
            saVertices[3].TextureCoordinate.X = 0.0f; saVertices[3].TextureCoordinate.Y = 1.0f;

            // Front of Sky Box
            saVertices[4].Position = new Vector3(-sHalfSkyBoxSize.X, -sHalfSkyBoxSize.Y, sHalfSkyBoxSize.Z);
            saVertices[4].TextureCoordinate.X = 1.0f; saVertices[4].TextureCoordinate.Y = 1.0f;
            saVertices[5].Position = new Vector3(-sHalfSkyBoxSize.X, sHalfSkyBoxSize.Y, sHalfSkyBoxSize.Z);
            saVertices[5].TextureCoordinate.X = 1.0f; saVertices[5].TextureCoordinate.Y = 0.0f;
            saVertices[6].Position = new Vector3(sHalfSkyBoxSize.X, sHalfSkyBoxSize.Y, sHalfSkyBoxSize.Z);
            saVertices[6].TextureCoordinate.X = 0.0f; saVertices[6].TextureCoordinate.Y = 0.0f;
            saVertices[7].Position = new Vector3(sHalfSkyBoxSize.X, -sHalfSkyBoxSize.Y, sHalfSkyBoxSize.Z);
            saVertices[7].TextureCoordinate.X = 0.0f; saVertices[7].TextureCoordinate.Y = 1.0f;

            // Bottom of Sky Box
            saVertices[8].Position = new Vector3(-sHalfSkyBoxSize.X, -sHalfSkyBoxSize.Y, -sHalfSkyBoxSize.Z);
            saVertices[8].TextureCoordinate.X = 0.0f; saVertices[8].TextureCoordinate.Y = 0.0f;
            saVertices[9].Position = new Vector3(-sHalfSkyBoxSize.X, -sHalfSkyBoxSize.Y, sHalfSkyBoxSize.Z);
            saVertices[9].TextureCoordinate.X = 0.0f; saVertices[9].TextureCoordinate.Y = 1.0f;
            saVertices[10].Position = new Vector3(sHalfSkyBoxSize.X, -sHalfSkyBoxSize.Y, sHalfSkyBoxSize.Z);
            saVertices[10].TextureCoordinate.X = 1.0f; saVertices[10].TextureCoordinate.Y = 1.0f;
            saVertices[11].Position = new Vector3(sHalfSkyBoxSize.X, -sHalfSkyBoxSize.Y, -sHalfSkyBoxSize.Z);
            saVertices[11].TextureCoordinate.X = 1.0f; saVertices[11].TextureCoordinate.Y = 0.0f;

            // Top of Sky Box
            saVertices[12].Position = new Vector3(sHalfSkyBoxSize.X, sHalfSkyBoxSize.Y, -sHalfSkyBoxSize.Z);
            saVertices[12].TextureCoordinate.X = 1.0f; saVertices[12].TextureCoordinate.Y = 0.0f;
            saVertices[13].Position = new Vector3(sHalfSkyBoxSize.X, sHalfSkyBoxSize.Y, sHalfSkyBoxSize.Z);
            saVertices[13].TextureCoordinate.X = 1.0f; saVertices[13].TextureCoordinate.Y = 1.0f;
            saVertices[14].Position = new Vector3(-sHalfSkyBoxSize.X, sHalfSkyBoxSize.Y, sHalfSkyBoxSize.Z);
            saVertices[14].TextureCoordinate.X = 0.0f; saVertices[14].TextureCoordinate.Y = 1.0f;
            saVertices[15].Position = new Vector3(-sHalfSkyBoxSize.X, sHalfSkyBoxSize.Y, -sHalfSkyBoxSize.Z);
            saVertices[15].TextureCoordinate.X = 0.0f; saVertices[15].TextureCoordinate.Y = 0.0f;

            // Left side of Sky Box
            saVertices[16].Position = new Vector3(sHalfSkyBoxSize.X, -sHalfSkyBoxSize.Y, -sHalfSkyBoxSize.Z);
            saVertices[16].TextureCoordinate.X = 0.0f; saVertices[19].TextureCoordinate.Y = 0.0f;
            saVertices[17].Position = new Vector3(sHalfSkyBoxSize.X, -sHalfSkyBoxSize.Y, sHalfSkyBoxSize.Z);
            saVertices[17].TextureCoordinate.X = 1.0f; saVertices[18].TextureCoordinate.Y = 0.0f;
            saVertices[18].Position = new Vector3(sHalfSkyBoxSize.X, sHalfSkyBoxSize.Y, sHalfSkyBoxSize.Z);
            saVertices[18].TextureCoordinate.X = 1.0f; saVertices[17].TextureCoordinate.Y = 1.0f;
            saVertices[19].Position = new Vector3(sHalfSkyBoxSize.X, sHalfSkyBoxSize.Y, -sHalfSkyBoxSize.Z);
            saVertices[19].TextureCoordinate.X = 0.0f; saVertices[16].TextureCoordinate.Y = 1.0f;

            // Right side of Sky Box            
            saVertices[20].Position = new Vector3(-sHalfSkyBoxSize.X, sHalfSkyBoxSize.Y, -sHalfSkyBoxSize.Z);
            saVertices[20].TextureCoordinate.X = 1.0f; saVertices[23].TextureCoordinate.Y = 1.0f;
            saVertices[21].Position = new Vector3(-sHalfSkyBoxSize.X, sHalfSkyBoxSize.Y, sHalfSkyBoxSize.Z);
            saVertices[21].TextureCoordinate.X = 0.0f; saVertices[22].TextureCoordinate.Y = 1.0f;
            saVertices[22].Position = new Vector3(-sHalfSkyBoxSize.X, -sHalfSkyBoxSize.Y, sHalfSkyBoxSize.Z);
            saVertices[22].TextureCoordinate.X = 0.0f; saVertices[21].TextureCoordinate.Y = 0.0f;
            saVertices[23].Position = new Vector3(-sHalfSkyBoxSize.X, -sHalfSkyBoxSize.Y, -sHalfSkyBoxSize.Z);
            saVertices[23].TextureCoordinate.X = 1.0f; saVertices[20].TextureCoordinate.Y = 0.0f;

            // Copy the Vertex data to the Vertex Buffer
            mcVertexBuffer = new VertexBuffer(mcGraphicsDevice, saVertices.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
            mcVertexBuffer.SetData(saVertices);

            // Set the Vertex Declaration
            mcVertexDeclaration = new VertexDeclaration(mcGraphicsDevice, VertexPositionTexture.VertexElements);
        }

        private void SetupIndices()
        {
            // Create an array to hold the Indices
            int[] miaIndices = new int[6 * 6];

            for (int x = 0; x < 6; x++)
            {
                miaIndices[x * 6 + 0] = (short)(x * 4 + 0);
                miaIndices[x * 6 + 2] = (short)(x * 4 + 1);
                miaIndices[x * 6 + 1] = (short)(x * 4 + 2);

                miaIndices[x * 6 + 3] = (short)(x * 4 + 2);
                miaIndices[x * 6 + 5] = (short)(x * 4 + 3);
                miaIndices[x * 6 + 4] = (short)(x * 4 + 0);
            }

            // Copy the Index data to the Index Buffer
            mcIndexBuffer = new IndexBuffer(mcGraphicsDevice, typeof(int), miaIndices.Length, BufferUsage.WriteOnly);
            mcIndexBuffer.SetData(miaIndices);
        }
    }
}
