using DPSF_Demo_for_Windows_Phone_MonoGame.Resources;

namespace DPSF_Demo_for_Windows_Phone_MonoGame
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; } }
    }
}