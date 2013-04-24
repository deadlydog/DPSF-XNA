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
    /// Class to hold all of the drawing Settings
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    public class RenderProperties
    {
        /// <summary>
        /// Get / Set the BlendState to use when drawing the particles.
        /// <para>Default value is BlendState.AlphaBlend.</para>
        /// </summary>
        public BlendState BlendState { get; set; }

        /// <summary>
        /// Get / Set the DepthStencilState to use when drawing the particles.
        /// <para>Default value is DepthStencilState.DepthRead.</para>
        /// </summary>
        public DepthStencilState DepthStencilState { get; set; }

        /// <summary>
        /// Get / Set the RasterizerState to use when drawing the particles.
        /// <para>Default value is RasterizerState.CullCounterClockwise.</para>
        /// </summary>
        public RasterizerState RasterizerState { get; set; }

        /// <summary>
        /// Get / Set the SamplerState to use when drawing the particles.
        /// <para>Default value is SamplerState.LinearClamp.</para>
        /// </summary>
        public SamplerState SamplerState { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderProperties"/> class, setting each property to its default value.
        /// </summary>
        public RenderProperties()
        {
            ResetToDefaults();
        }

        /// <summary>
        /// Resets each of the render properties to their default values.
        /// </summary>
        public void ResetToDefaults()
        {
            // Clone the states instead of just setting to them directly so that they are not read-only and we can change their properties.
            this.BlendState = DPSFHelper.CloneBlendState(BlendState.AlphaBlend);
            this.DepthStencilState = DPSFHelper.CloneDepthStencilState(DepthStencilState.DepthRead);
            this.RasterizerState = DPSFHelper.CloneRasterizerState(RasterizerState.CullCounterClockwise);
            this.SamplerState = DPSFHelper.CloneSamplerState(SamplerState.LinearClamp);
        }
    }
}
