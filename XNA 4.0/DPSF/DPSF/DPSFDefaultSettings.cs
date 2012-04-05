
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
    }
}
