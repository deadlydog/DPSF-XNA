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
		/// A user-friendly name to display in the DPSF Viewer instead of the field/method/property name.
		/// If this is left empty the field/method/property name will be used.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		private string _name = string.Empty;

		/// <summary>
		/// A user-friendly description of what the field/method/property does.
		/// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private string _description = string.Empty;

		/// <summary>
		/// The group of controls that this one should be grouped with in the DPSF Viewer GUI.
		/// </summary>
		public string Group
		{
			get { return _group; }
			set { _group = value; }
		}
		private string _group = string.Empty;
    }
}
