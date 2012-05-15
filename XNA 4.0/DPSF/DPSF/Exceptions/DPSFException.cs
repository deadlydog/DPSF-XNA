using System;

namespace DPSF.Exceptions
{
	/// <summary>
	/// An Exception thrown by a DPSF component.
	/// <para>NOTE: The other DSPF Exceptions do not inherit from this DPSFException. They each inherit from their non-DPSF equivalent exceptions.</para>
	/// </summary>
	public class DPSFException : Exception, IDPSFException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFException"/> class.
		/// </summary>
		public DPSFException() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		public DPSFException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public DPSFException(string message, Exception innerException) : base(message, innerException) { }
	}
}