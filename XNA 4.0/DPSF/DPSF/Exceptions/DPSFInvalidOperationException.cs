using System;

namespace DPSF.Exceptions
{
	/// <summary>
	/// An InvalidOperationException thrown by a DPSF component.
	/// </summary>
	public class DPSFInvalidOperationException : InvalidOperationException, IDPSFException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFInvalidOperationException"/> class.
		/// </summary>
		public DPSFInvalidOperationException() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFInvalidOperationException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		public DPSFInvalidOperationException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFInvalidOperationException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public DPSFInvalidOperationException(string message, Exception innerException) : base(message, innerException) { }
	}
}