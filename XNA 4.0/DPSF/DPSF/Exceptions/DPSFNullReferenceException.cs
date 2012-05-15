using System;

namespace DPSF.Exceptions
{
	/// <summary>
	/// A NullReferenceException thrown by a DPSF component.
	/// </summary>
	public class DPSFNullReferenceException : NullReferenceException, IDPSFException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFNullReferenceException"/> class.
		/// </summary>
		public DPSFNullReferenceException() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFNullReferenceException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		public DPSFNullReferenceException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFNullReferenceException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public DPSFNullReferenceException(string message, Exception innerException) : base(message, innerException) { }
	}
}