using System;

namespace DPSF.Exceptions
{
	/// <summary>
	/// A warning Exception thrown by a DPSF component when the user is not configuring DPSF ideally.
	/// These are useful for helping users ensure that they have DPFS configured for optimal performance.
	/// <para>NOTE: These exceptions can be disabled by setting the DPSFHelper.ThrowDebugWarningExceptions property to false.</para>
	/// </summary>
	public class DPSFDebugWarningException : Exception, IDPSFException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFDebugWarningException"/> class.
		/// </summary>
		public DPSFDebugWarningException() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFDebugWarningException" /> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		public DPSFDebugWarningException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFDebugWarningException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public DPSFDebugWarningException(string message, Exception innerException) : base(message, innerException) { }
	}
}