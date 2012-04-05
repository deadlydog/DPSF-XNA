using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF
{
	/// <summary>
	/// The Techniques provided by the DPSF Default Effect.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public enum DPSFDefaultEffectTechniques
	{
		/// <summary>
		/// The default technique used to display particles as sprites.
		/// </summary>
		Sprites = 0,

		/// <summary>
		/// The default technique used to display particles as colored quads.
		/// </summary>
		Quads = 1,

		/// <summary>
		/// The default technique used to display particles as textured quads.
		/// </summary>
		TexturedQuads = 2,

		/// <summary>
		/// An experimental technique used to display particles as textured quads, doing the color blending using premultiplied colors.
		/// </summary>
		TexturedQuadsExperimental = 3
	}

	/// <summary>
	/// The Default Effect provided by DPSF.
	/// </summary>
	public class DPSFDefaultEffect : Effect
	{
		/// <summary>
		/// The list of valid DPSF Default Effect configurations
		/// </summary>
		public enum DPSFDefaultEffectConfigurations
		{
			/// <summary>
			/// Windows HiDef configuration.
			/// </summary>
			WindowsHiDef = 0,

			/// <summary>
			/// Windows Reach configuration.
			/// </summary>
			WindowsReach = 1,

			/// <summary>
			/// Xbox 360 HiDef configuration.
			/// </summary>
			Xbox360HiDef = 2
		}

		#region Public Effect Parameters

		/// <summary>
		/// How much of the vertex Color should be blended in with the Texture's Color.
        /// <para>0.0 = use Texture's color, 1.0 = use specified color. Default is 0.5.</para>
		/// </summary>
		public float ColorBlendAmount
		{
			get { return _colorBlendAmountParameter.GetValueSingle(); }
			set
			{
				float colorBlendAmount = value;
				if (colorBlendAmount > 1) colorBlendAmount = 1;
				if (colorBlendAmount < 0) colorBlendAmount = 0;
				_colorBlendAmountParameter.SetValue(colorBlendAmount);
			}
		}

		/// <summary>
		/// The texture to use to draw the particles.
		/// </summary>
		public Texture2D Texture
		{
			get { return _textureParameter.GetValueTexture2D(); }
			set { _textureParameter.SetValue(value); }
		}

		/// <summary>
		/// The World matrix.
		/// </summary>
		public Matrix World
		{
			get { return _worldParameter.GetValueMatrix(); }
			set { _worldParameter.SetValue(value); }
		}

		/// <summary>
		/// The View matrix.
		/// </summary>
		public Matrix View
		{
			get { return _viewParameter.GetValueMatrix(); }
			set { _viewParameter.SetValue(value); }
		}

		/// <summary>
		/// The Projection matrix.
		/// </summary>
		public Matrix Projection
		{
			get { return _projectionParameter.GetValueMatrix(); }
			set { _projectionParameter.SetValue(value); }
		}

		#endregion

		// Shortcut accessors to the Effect parameters
		private EffectParameter _colorBlendAmountParameter;
		private EffectParameter _textureParameter;
		private EffectParameter _worldParameter;
		private EffectParameter _viewParameter;
		private EffectParameter _projectionParameter;

		/// <summary>
		/// Holds this effects configuration in case we need to clone it.
		/// </summary>
		private DPSFDefaultEffectConfigurations _configuration = DPSFDefaultEffectConfigurations.WindowsHiDef;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="graphicsDevice">The Graphics Device to load the effect with.</param>
		/// <param name="configuration">The effect configuration to load (i.e. Windows HiDef, Xbox 360 Reach, etc.)</param>
		public DPSFDefaultEffect(GraphicsDevice graphicsDevice, DPSFDefaultEffectConfigurations configuration)
			: base(graphicsDevice, configuration == DPSFDefaultEffectConfigurations.WindowsHiDef ? DPSFResources.DPSFDefaultEffectWindowsHiDef : 
									configuration == DPSFDefaultEffectConfigurations.WindowsReach ? DPSFResources.DPSFDefaultEffectWindowsReach :
									DPSFResources.DPSFDefaultEffectXbox360HiDef)
		{
			// Hookup the strongly typed accessors to the Effect parameters
			_colorBlendAmountParameter = Parameters["xColorBlendAmount"];
			_textureParameter = Parameters["xTexture"];
			_worldParameter = Parameters["xWorld"];
			_viewParameter = Parameters["xView"];
			_projectionParameter = Parameters["xProjection"];

			// Record what configuration was specified
			_configuration = configuration;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFDefaultEffect"/> class.
		/// </summary>
		/// <param name="effectToClone">The effect to clone.</param>
		public DPSFDefaultEffect(DPSFDefaultEffect effectToClone)
			: this(effectToClone.GraphicsDevice, effectToClone._configuration)
		{
			// Copy the Effect Parameters into this Effect
			this.ColorBlendAmount = effectToClone.ColorBlendAmount;
			this.Texture = effectToClone.Texture;
			this.World = effectToClone.World;
			this.View = effectToClone.View;
			this.Projection = effectToClone.Projection;
		}

		/// <summary>
		/// Creates and returns a clone of this DPSFDefaultEffect instance.
		/// </summary>
		public override Effect Clone()
		{
			return new DPSFDefaultEffect(this);
		}
	}
}
