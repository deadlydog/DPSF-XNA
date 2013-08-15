using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace DPSF
{
    /// <summary>
    /// Static class used to apply default settings to all DPSF particle systems and particle system managers when they are initialized.
    /// <para>Note: These settings are applied during particle system initialization, and may be overwritten by particle system manager settings.</para>
    /// </summary>
    public static class DPSFDefaultSettings
    {
        /// <summary>
        /// Initializes the <see cref="DPSFDefaultSettings"/> class.
        /// This constructor cannot be explicitly called. It will be called the first time this class is accessed.
        /// </summary>
        static DPSFDefaultSettings()
        {
            ResetToDefaults();            
        }

        /// <summary>
        /// Resets all of the DPSFDefaultSettings' properties to their default values.
        /// </summary>
        public static void ResetToDefaults()
        {
            AutoMemoryManagementSettings = new AutoMemoryManagerSettings();
            UpdatesPerSecond = 0;
            PerformanceProfilingIsEnabled = false;
			UseSharedEffectForAllParticleSystems = false;
        }

        /// <summary>
        /// Gets / Sets the default particle system memory management settings.
        /// </summary>
        public static AutoMemoryManagerSettings AutoMemoryManagementSettings { get; private set; }

        /// <summary>
        /// Gets / Sets the default particle system updates per second.
        /// <para>NOTE: Zero means update as often as possible.</para>
        /// </summary>
        public static int UpdatesPerSecond { get; set; }

        /// <summary>
        /// Gets / Sets if particle system performance timings are Enabled by default or not.
        /// Performance profiling is not available on the Reach profile.
        /// </summary>
        public static bool PerformanceProfilingIsEnabled { get; set; }

		/// <summary>
		/// Gets / Sets if a common BasicEffect and AlphaTestEffect should be used for all particle systems, rather than each particle system creating their own.
		/// <para>NOTE: Using a shared effect will decrease the time it takes to Initialize() each particle system.</para>
		/// <para>NOTE: If using a shared Effect for all particle systems, if one particle system sets an Effect parameter, all other particle systems should also set that same
		/// parameter in their overridden SetEffectParameters() function so that each particle system guarantees that the Effect is using the parameters it expects it to be using.
		/// For example, if particle system 1 sets the Effect's Texture, that same Texture will be used for particle system 2 unless particle system 2 specifies the new Texture to use.</para>
		/// <para>NOTE: The Graphics Device of the first particle system to be initialized will be used when creating the shared Effect. If you later require a different Graphics Device
		/// to be used, you must call the SetGraphicsDeviceForSharedEffectsForAllParticleSystems() function.</para>
		/// </summary>
		public static bool UseSharedEffectForAllParticleSystems { get; set; }

		/// <summary>
		/// Reinitializes the shared BasicEffect and AlphaTestEffect using the GraphicsDevice provided.
		/// <para>NOTE: If you are using a Shared Effect For All Particle Systems and you change GraphicsDevices, you will need to call this function with the new Graphics Device.</para>
		/// </summary>
		public static void SetGraphicsDeviceForSharedEffectsForAllParticleSystems(GraphicsDevice graphicsDevice)
		{
			_basicEffect = new BasicEffect(graphicsDevice);
			_alphaTestEffect = new AlphaTestEffect(graphicsDevice);
		}

		/// <summary>
		/// Gets the BasicEffect shared by all particle systems.
		/// </summary>
		/// <param name="graphicsDevice">The Graphics Device used to initialize the BasicEffect.
		/// <para>NOTE: This is only used the first time that this function is called.</para></param>
		internal static BasicEffect GetSharedBasicEffect(GraphicsDevice graphicsDevice)
		{
			return _basicEffect ?? (_basicEffect = new BasicEffect(graphicsDevice));
		}
    	private static BasicEffect _basicEffect = null;

		/// <summary>
		/// Gets the AlphaTestEffect shared by all particle systems.
		/// </summary>
		/// <param name="graphicsDevice">The Graphics Device used to initialize the AlphaTestEffect.
		/// <para>NOTE: This is only used the first time that this function is called.</para></param>
		internal static AlphaTestEffect GetSharedAlphaTestEffect(GraphicsDevice graphicsDevice)
		{
			return _alphaTestEffect ?? (_alphaTestEffect = new AlphaTestEffect(graphicsDevice));
		}
    	private static AlphaTestEffect _alphaTestEffect = null;

	    /// <summary>
	    /// Get and set if DPSF Debug Warning Exceptions should be thrown or not.
	    /// <para>These exceptions are typically thrown as a warning to the user that they do not have DPSF configured ideally.</para>
	    /// <para>This will only return true (default) if we are running in Debug mode (i.e. the DEBUG compilation symbol is defined) and a debugger is attached. 
	    /// This is done to prevent these exceptions from being thrown when software compiled in Debug mode is released.</para>
	    /// </summary>
	    public static bool ThrowDebugWarningExceptions
	    {
		    get
		    {
			    // Not everybody knows that they should be compiling their software in Release mode before shipping it, so to avoid these
			    // Debug Warning Exceptions from being thrown on released software, make sure a debugger is attached.
			    // If we are running in Debug mode and a Debugger is attached, return the setting value specified by the user.
			    return DPSFHelper.IsRunningInDebugModeWithDebuggerAttached && _throwDebugWarningExceptions;
		    }
		    set { _throwDebugWarningExceptions = value; }
	    }

	    private static bool _throwDebugWarningExceptions = true;
    }
}
