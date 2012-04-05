using System;

namespace DPSF
{
	/// <summary>
	/// Attribute used to mark fields/methods/properties that should be shown in the DPSF Viewer.
	/// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public class DPSFViewerParameterAttribute : System.Attribute
    {
		/// <summary>
		/// A user-friendly description of what the field/method/property does.
		/// This will be displayed to the user of the DPSF Viewer.
		/// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private string _description = string.Empty;

		/// <summary>
		/// The group of controls that this one should be grouped with in the DPSF Viewer.
		/// </summary>
		public string Group
		{
			get { return _group; }
			set { _group = value; }
		}
		private string _group = string.Empty;
    }
}
