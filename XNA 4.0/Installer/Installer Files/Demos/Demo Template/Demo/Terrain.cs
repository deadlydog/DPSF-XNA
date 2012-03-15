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
    class Terrain
    {
        public struct VertexPositionNormalColored
        {
            public Vector3 Position;
            public Color Color;
            public Vector3 Normal;

            public static int SizeInBytes = 7 * 4;
            public static VertexElement[] VertexElements = new VertexElement[]
            {
                new VertexElement( 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0 ),
                new VertexElement( sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0 ),
                new VertexElement( sizeof(float) * 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0 ),
            };
        }

        // The Graphics Device and Effect used to draw the Terrain
        GraphicsDevice mcGraphicsDevice = null;
        public BasicEffect mcEffect = null;

        // Vertex and Index Buffer variables
        VertexPositionNormalTexture[] msaTerrainVertices;
        VertexBuffer mcVertexBuffer = null;
        VertexDeclaration mcVertexDeclaration = null;
        int[] miaTerrainIndices;
        IndexBuffer mcIndexBuffer = null;

        // The Terrain's Texture
        Texture2D mcTerrainTexture = null;
        int miNumberOfTextureTiles = 0;

        // The Width and Height of the Height Map Texture
        private int miTerrainTextureWidth = 0;
        private int miTerrainTextureHeight = 0;
        private float[,] mfaTerrainHeightData;

        // The Dimensions that the Terrain should be
        private int miTerrainWidth = 0;
        private int miTerrainLength = 0;
        private int miTerrainMaxHeight = 0;

        /// <summary>
        /// Creates the Terrain from the given Height Map Texture
        /// </summary>
        /// <param name="cGraphicsDevice">The Graphics Device to draw to</param>
        /// <param name="cHeightMapTexture">The Texture that should be used to generate the Terrain / Height Map with</param>
        /// <param name="iTerrainWidth">The desired Width of the Terrain</param>
        /// <param name="iTerrainLength">The desired Length of the Terrain</param>
        /// <param name="iTerrainMaxHeight">The Height that the largest Height Map Texture color value should be</param>
        /// <param name="cTexture">The Texture to tile the Terrain with</param>
        /// <param name="iNumberOfTextureTiles">How many times the Terrain Texture should be Tiled to cover the entire Terrain</param>
        public void Load(GraphicsDevice cGraphicsDevice, Texture2D cHeightMapTexture, int iTerrainWidth, int iTerrainLength, int iTerrainMaxHeight,
                            Texture2D cTexture, int iNumberOfTextureTiles)
        {
            // Save a handle to the Graphics Device to draw to
            mcGraphicsDevice = cGraphicsDevice;

            // Create the Effect used to draw the Terrain
            mcEffect = new BasicEffect(mcGraphicsDevice);
            
            // Save the Texture used to draw the Terrain with, and how many times it should be Tiled
            mcTerrainTexture = cTexture;
            miNumberOfTextureTiles = iNumberOfTextureTiles;

            // Save the Width and Height of the Height Map Texture used to generate the Terrain
            miTerrainTextureWidth = cHeightMapTexture.Width;
            miTerrainTextureHeight = cHeightMapTexture.Height;

            // Save the desired Terrain Width, Length, and Height
            miTerrainWidth = iTerrainWidth;
            miTerrainLength = iTerrainLength;
            miTerrainMaxHeight = iTerrainMaxHeight;

            // Create the Height Map Data
            SetupHeightMapData(cHeightMapTexture);

            // Create the Vertices and Indices
            CreateVertices();
            CreateIndices();
            CalculateNormals();

            // Copy the data to the Vertex Buffer and Index Buffer
            CopyToBuffers();

            // Setup the Effect
            mcEffect.VertexColorEnabled = false;
            mcEffect.TextureEnabled = true;
            mcEffect.Texture = mcTerrainTexture;
            mcEffect.LightingEnabled = true;
            mcEffect.EnableDefaultLighting();
        }

        /// <summary>
        /// Draws the Terrain to the Graphics Device
        /// </summary>
        /// <param name="sWorld">The World Matrix</param>
        /// <param name="sView">The View Matrix</param>
        /// <param name="sProjection">The Projection Matrix</param>
        public void Draw(Matrix sWorld, Matrix sView, Matrix sProjection)
        {
            // Set the Effect's World, View, and Projection matrices
            mcEffect.World = sWorld;
            mcEffect.View = sView;
            mcEffect.Projection = sProjection;

			mcGraphicsDevice.RasterizerState = RasterizerState.CullNone;
			mcGraphicsDevice.DepthStencilState = DepthStencilState.Default;

			// Turn on texture Mirroring so that we don't notice the Tiling of the Texture
			mcGraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

			mcGraphicsDevice.SetVertexBuffer(mcVertexBuffer);
			mcGraphicsDevice.Indices = mcIndexBuffer;

            foreach (EffectPass pass in mcEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                // Draw the Terrain
                mcGraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, msaTerrainVertices.Length, 0, miaTerrainIndices.Length / 3);
            }
        }

        /// <summary>
        /// Fills the Terrain Height Data array with the values from the Height Map Texture
        /// </summary>
        /// <param name="cHeightMapTexture">The Texture to create the Height Map from</param>
        private void SetupHeightMapData(Texture2D cHeightMapTexture)
        {
            // Get the Height Map Data from the Texture
            Color[] saHeightMapColors = new Color[miTerrainTextureWidth * miTerrainTextureHeight];
            cHeightMapTexture.GetData(saHeightMapColors);

            // Create the Terrain's Height Map Data
            mfaTerrainHeightData = new float[miTerrainTextureWidth, miTerrainTextureHeight];
            for (int x = 0; x < miTerrainTextureWidth; x++)
            {
                for (int y = 0; y < miTerrainTextureHeight; y++)
                {
                    mfaTerrainHeightData[x, y] = saHeightMapColors[x + y * miTerrainTextureWidth].R;
                }
            }
        }

        /// <summary>
        /// Creates the Vertices array and calculates each Vertex's Position and Texture Coordinates
        /// </summary>
        private void CreateVertices()
        {
            // Calculate the half dimensions of the Terrain so it can be centered around the origin
            int iHalfTerrainWidth = miTerrainWidth / 2;
            int iHalfTerrainLength = miTerrainLength / 2;
            int iHalfTerrainHeight = miTerrainMaxHeight / 2;

            // Find the local Min and Max height values of the Terrain Texture
            float fMinHeight = float.MaxValue;
            float fMaxHeight = float.MinValue;
            for (int x = 0; x < miTerrainTextureWidth; x++)
            {
                for (int y = 0; y < miTerrainTextureHeight; y++)
                {
                    if (mfaTerrainHeightData[x, y] < fMinHeight)
                    {
                        fMinHeight = mfaTerrainHeightData[x, y];
                    }

                    if (mfaTerrainHeightData[x, y] > fMaxHeight)
                    {
                        fMaxHeight = mfaTerrainHeightData[x, y];
                    }
                }
            }

            // Calculate how much we need to Scale in each dimension to obtain the desired Terrain dimensions
            float fWidthScale = (float)miTerrainWidth / (float)miTerrainTextureWidth;
            float fLengthScale = (float)miTerrainLength / (float)miTerrainTextureHeight;
            float fHeightScale = (float)miTerrainMaxHeight / (fMaxHeight - fMinHeight);

            // Calculate the desired Texture Tile Width
            float fTextureTileWidthOnTerrain = miTerrainWidth / miNumberOfTextureTiles;
            float fTextureTileLengthOnTerrain = miTerrainLength / miNumberOfTextureTiles;

            // Create the Vertices
            msaTerrainVertices = new VertexPositionNormalTexture[miTerrainTextureWidth * miTerrainTextureHeight];
            for (int x = 0; x < miTerrainTextureWidth; x++)
            {
                for (int z = 0; z < miTerrainTextureHeight; z++)
                {
                    // Calculate the position of this Vertex in 3D space
                    float fX = x * fWidthScale;
                    float fY = (mfaTerrainHeightData[x, z] - fMinHeight) * fHeightScale;
                    float fZ = z * fLengthScale;

                    // Calculate the (u,v) Texture coordinates of this Vertex
                    float fU = fX / fTextureTileWidthOnTerrain;
                    float fV = fZ / fTextureTileLengthOnTerrain;

                    // Set the Vertex's Position and Texture Coordinate
                    msaTerrainVertices[x + z * miTerrainTextureWidth].Position = new Vector3(fX - iHalfTerrainWidth, fY - iHalfTerrainHeight, fZ - iHalfTerrainLength);
                    msaTerrainVertices[x + z * miTerrainTextureWidth].TextureCoordinate = new Vector2(fU, fV);

/*
                    Color sVertexColor = Color.White;
                    if (mfaTerrainHeightData[x, z] < fMinHeight + (fMaxHeight - fMinHeight) / 4)
                        sVertexColor = Color.Blue;
                    else if (mfaTerrainHeightData[x, z] < fMinHeight + (fMaxHeight - fMinHeight) * 2 / 4)
                        sVertexColor = Color.Green;
                    else if (mfaTerrainHeightData[x, z] < fMinHeight + (fMaxHeight - fMinHeight) * 3 / 4)
                        sVertexColor = Color.Brown;
                    else
                        sVertexColor = Color.White;

                    msaTerrainVertices[x + z * miTerrainTextureWidth].Color = sVertexColor;
*/
                }
            }
        }

        /// <summary>
        /// Creates the Indices corresponding to the Vertices
        /// </summary>
        private void CreateIndices()
        {
            // Create the Indices array
            miaTerrainIndices = new int[(miTerrainTextureWidth - 1) * (miTerrainTextureHeight - 1) * 6];
            
            // Set the Terrain Index data for the Vertices
            int counter = 0;
            for (int y = 0; y < miTerrainTextureHeight - 1; y++)
            {
                for (int x = 0; x < miTerrainTextureWidth - 1; x++)
                {
                    int lowerLeft = x + y * miTerrainTextureWidth;
                    int lowerRight = (x + 1) + y * miTerrainTextureWidth;
                    int topLeft = x + (y + 1) * miTerrainTextureWidth;
                    int topRight = (x + 1) + (y + 1) * miTerrainTextureWidth;

                    miaTerrainIndices[counter++] = topLeft;
                    miaTerrainIndices[counter++] = lowerRight;
                    miaTerrainIndices[counter++] = lowerLeft;

                    miaTerrainIndices[counter++] = topLeft;
                    miaTerrainIndices[counter++] = topRight;
                    miaTerrainIndices[counter++] = lowerRight;
                }
            }
        }

        /// <summary>
        /// Calculates and assigns the Normal for each Vertex.
        /// <para>NOTE: This should be called after CreateVertices() and CreateIndices() as it relies on the
        /// Vertex and Index arrays.</para>
        /// </summary>
        private void CalculateNormals()
        {
            // Initialize all Normals
            for (int i = 0; i < msaTerrainVertices.Length; i++)
            {
                msaTerrainVertices[i].Normal = Vector3.Zero;
            }

            // Calculate and set the Normal for each Vertex
            for (int i = 0; i < miaTerrainIndices.Length / 3; i++)
            {
                int index1 = miaTerrainIndices[i * 3];
                int index2 = miaTerrainIndices[i * 3 + 1];
                int index3 = miaTerrainIndices[i * 3 + 2];

                Vector3 side1 = msaTerrainVertices[index1].Position - msaTerrainVertices[index3].Position;
                Vector3 side2 = msaTerrainVertices[index1].Position - msaTerrainVertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                msaTerrainVertices[index1].Normal += normal;
                msaTerrainVertices[index2].Normal += normal;
                msaTerrainVertices[index3].Normal += normal;
            }

            // Normalize all of the Vertices' Normals
            for (int i = 0; i < msaTerrainVertices.Length; i++)
            {
                msaTerrainVertices[i].Normal.Normalize();
            }
        }

        /// <summary>
        /// Copies the Vertex and Index arrays to the Vertex Buffer and Index Buffer
        /// </summary>
        private void CopyToBuffers()
        {
            // Copy the Vertex data to the Vertex Buffer
            mcVertexBuffer = new VertexBuffer(mcGraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, msaTerrainVertices.Length, BufferUsage.WriteOnly);
            mcVertexBuffer.SetData(msaTerrainVertices);

            // Copy the Index data to the Index Buffer
            mcIndexBuffer = new IndexBuffer(mcGraphicsDevice, typeof(int), miaTerrainIndices.Length, BufferUsage.WriteOnly);
            mcIndexBuffer.SetData(miaTerrainIndices);
        }
    }
}
