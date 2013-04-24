using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF
{
    /// <summary>
    /// Class to hold all of the SpriteBatch-specific drawing Settings
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    public class SpriteBatchSettings
    {
        /// <summary>
        /// The Sort Mode to use in the SpriteBatch.Begin() function call.
        /// </summary>
        public SpriteSortMode SortMode = SpriteSortMode.Deferred;

        /// <summary>
        /// The Transformation Matrix used in the SpriteBatch.Begin() function call.
        /// </summary>
        public Matrix TransformationMatrix = Matrix.Identity;
    }
}
