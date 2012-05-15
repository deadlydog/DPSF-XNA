using System;

namespace DPSF.Exceptions
{
	/// <summary>
	/// An ArgumentNullException thrown by a DPSF component.
	/// </summary>
	public class DPSFArgumentNullException : ArgumentNullException, IDPSFException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFArgumentNullException"/> class.
		/// </summary>
		public DPSFArgumentNullException() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFArgumentNullException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		public DPSFArgumentNullException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFArgumentNullException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public DPSFArgumentNullException(string message, Exception innerException) : base(message, innerException) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFArgumentNullException"/> class.
		/// </summary>
		/// <param name="paramName">Name of the parameter that is null.</param>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		public DPSFArgumentNullException(string paramName, string message) : base(paramName, message) { }
	}
}