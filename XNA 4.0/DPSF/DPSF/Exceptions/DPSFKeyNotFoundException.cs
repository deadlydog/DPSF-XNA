using System;
using System.Collections.Generic;

namespace DPSF.Exceptions
{
	/// <summary>
	/// A KeyNotFoundException thrown by a DPSF exception.
	/// </summary>
	public class DPSFKeyNotFoundException : KeyNotFoundException, IDPSFException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFKeyNotFoundException"/> class.
		/// </summary>
		public DPSFKeyNotFoundException() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFKeyNotFoundException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		public DPSFKeyNotFoundException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DPSFKeyNotFoundException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public DPSFKeyNotFoundException(string message, Exception innerException) : base(message, innerException) { }
	}
}