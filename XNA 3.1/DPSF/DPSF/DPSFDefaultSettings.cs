using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DPSF
{
    /// <summary>
    /// Static class used to apply default settings to all DPSF particle systems when they are initialized.
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
        }

        /// <summary>
        /// Gets or sets the default particle system memory management settings.
        /// </summary>
        public static AutoMemoryManagerSettings AutoMemoryManagementSettings { get; private set; }

        /// <summary>
        /// Gets or sets the default particle system updates per second.
        /// </summary>
        public static int UpdatesPerSecond { get; set; }
    }
}
