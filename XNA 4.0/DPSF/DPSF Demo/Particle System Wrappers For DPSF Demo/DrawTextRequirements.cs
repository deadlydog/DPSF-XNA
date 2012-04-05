using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
    public class DrawTextRequirements
    {
        /// <summary>
        /// The Sprite Batch used to draw text to the screen.
        /// </summary>
        public SpriteBatch TextWriter { get; set; }

        /// <summary>
        /// The Font to draw the text with.
        /// </summary>
        public SpriteFont Font { get; set; }

        /// <summary>
        /// The area that it is safe to draw text to so it will show up properly on screen.
        /// </summary>
        public Rectangle TextSafeArea { get; set; }

        /// <summary>
        /// The Color to draw Property text with.
        /// </summary>
        public Color PropertyTextColor { get; set; }

        /// <summary>
        /// The Color to draw Value text with.
        /// </summary>
        public Color ValueTextColor { get; set; }

        /// <summary>
        /// The Color to draw Control text with.
        /// </summary>
        public Color ControlTextColor { get; set; }
    }
}
