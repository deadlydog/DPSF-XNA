using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DPSF
{
    /// <summary>
    /// The Type of Particles that the Particle Systems can draw. Different Particle Types are drawn in 
    /// different ways. For example, four vertices are required to draw a Quad, and only one is required 
    /// to draw a Point Sprite.
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    public enum ParticleTypes
    {
        /// <summary>
        /// This is the default settings when we don't know what Type of Particles are going to be used yet.
        /// A particle system is not considered Initialized until the Particle Type does not equal this.
        /// </summary>
        None = 0,

        /// <summary>
        /// Use this when you do not want to draw your particles to the screen (e.g. when you just want to collect 
        /// and analyze particle information without visualizing the particles), or when you want to draw particles
        /// to the screen using your own code (e.g. to draw models instead of textures).
        /// <para>No vertex or index buffer will be created, saving memory.</para>
        /// <para>The BeforeDraw() and AfterDraw() functions are still called when this Particle Type is used.
        /// Place your custom drawing code in one of these functions if necessary.</para>
        /// </summary>
        NoDisplay = 1,

        ///// <summary>
        ///// One vertex in 3D world coordinates. These particles are always only 1 pixel in size, cannot be 
        ///// rotated, and do not use a Texture.
        ///// </summary>
        //Pixel = 2,

        /// <summary>
        /// Texture in 2D screen coordinates. Drawn using a SpriteBatch object. Only allows for 2D roll
        /// rotations, always faces the camera, and must use a Texture.
        /// </summary>
        Sprite = 3,

        ///// <summary>
        ///// One vertex in 3D world coordinates. Only allows for 2D roll rotations, always faces the 
        ///// camera, is always a perfect square (i.e. cannot be stretched or skewed), and must use a Texture.
        ///// </summary>
        //PointSprite = 4,

        /// <summary>
        /// Four vertices in 3D world coordinates. Allows for rotations in all 3 dimensions, does
        /// not have to always face the camera, may be skewed into any quadrilateral, such as
        /// a square, rectangle, or trapezoid, and do not use a Texture.
        /// </summary>
        Quad = 5,

        /// <summary>
        /// Four vertices in 3D world coordinates. Allows for rotations in all 3 dimensions, does
        /// not have to always face the camera, may be skewed into any quadrilateral, such as
        /// a square, rectangle, or trapezoid, and must use a Texture.
        /// </summary>
        TexturedQuad = 6
    }
}
