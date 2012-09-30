using System;
using System.IO;

namespace BasicVirtualEnvironment.Diagnostics
{
	/// <summary>
	/// Static class used to log the average FPS to a file at a specified interval.
	/// The Update() function should be called every frame.
	/// </summary>
	public static class FPSLogger
	{
		private static string msFilePath = "AverageFPSLog.txt"; // The File Path to write the Log to.
		private static float mfTimeIntervalBetweenLogging = 10; // How long (in seconds) between writing the Average FPS to the file.
		private static float mfTimeElapsedSinceLastLog = 0;     // How much Time (in seconds) has Elapsed since we last logged the Average FPS.

		private static float mfTimeToWaitBeforeStarting = 0;    // How much Time should pass before starting the Logging.
		private static bool mbInitialWaitTimeOver = false;      // Tells if the the Time To Wait Before Starting has elapsed or not.

		private static int miNumberOfTimesToLog = 0;            // How many times the Average FPS should be Logged.
		private static int miNumberOfTimesLogged = 0;           // How many times the Average FPS has been Logged.

		private static bool mbEnabled = true;                   // Tells if Logging should be performed or not.
		private static bool mbLogToFile = true;                 // Tells if the Logging should be written to the File or not.

		/// <summary>
		/// This function should be called every Frame and is used to log the Average FPS to the file. If this function
		/// writes to the Log file, the Average FPS that was written to the file is returned. Otherwise it returns zero.
		/// </summary>
		/// <param name="elapsedTimeInSeconds">The elapsed time since the last Frame was drawn, in seconds.</param>
		/// <returns>If this function writes to the Log file, the Average FPS that was written to the file is returned. 
		/// Otherwise it returns zero.</returns>
		public static float Update(float elapsedTimeInSeconds)
		{
			// Variable to hold the value to return.
			float fAverageFPSToReturn = 0f;

			// If Logging is not Enabled.
			if (!mbEnabled)
			{
				// Exit without doing anything.
				return fAverageFPSToReturn;
			}

			// Add the Elapsed Time to our Time Elapsed Since Last Logging the FPS.
			mfTimeElapsedSinceLastLog += elapsedTimeInSeconds;

			// If the Initial Wait Time has not elapsed yet.
			if (!mbInitialWaitTimeOver)
			{
				// If the Initial Wait Time has now elapsed.
				if (mfTimeElapsedSinceLastLog >= mfTimeToWaitBeforeStarting)
				{
					// Record that the Initial Wait Time has elapsed.
					mbInitialWaitTimeOver = true;

					// Subtract the Initial Wait Time from the Last Log Time.
					mfTimeElapsedSinceLastLog -= mfTimeToWaitBeforeStarting;
				}
				// Else the Initial Wait Time has not elapsed yet.
				else
				{
					// So exit the function without Logging anything.
					return fAverageFPSToReturn;
				}
			}

			// If we have not Logged the max amount of Logs yet.
			if (miNumberOfTimesLogged < miNumberOfTimesToLog || miNumberOfTimesToLog == 0)
			{
				// If enough Time has elapsed that a Log should be performed.
				if (mfTimeElapsedSinceLastLog >= mfTimeIntervalBetweenLogging)
				{
					// Subtract the Interval Time from our Last Log Time.
					mfTimeElapsedSinceLastLog -= mfTimeIntervalBetweenLogging;

					// Increment the Number Of Times the Average FPS was Logged.
					miNumberOfTimesLogged++;

					// Save the value that is going to be written to the Log.
					fAverageFPSToReturn = FPS.AverageFPS;

					// If the Average FPS should be Logged to the File.
					if (mbLogToFile)
					{
						TextWriter cFile = null;
						try
						{
							// Get a handle to the File to write to, and turn Appending on.
							cFile = new StreamWriter(msFilePath, true);

							// Write the Average FPS to the File
							string sLog = miNumberOfTimesLogged.ToString() + " - " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + fAverageFPSToReturn.ToString();
							cFile.WriteLine(sLog);
						}
						finally
						{
							// Close the handle to the File since we are done writing for now.
							if (cFile != null)
							{
								cFile.Close();
							}
						}
					}
				}
			}

			// Return the Average FPS if it was written to the file.
			return fAverageFPSToReturn;
		}

		/// <summary>
		/// Get / Set the File Path that the Average FPS should be logged to.
		/// </summary>
		/// <param name="sFilePathAndName"></param>
		public static string FilePath
		{
			get { return msFilePath; }
			set { msFilePath = value; }
		}

		/// <summary>
		/// Get / Set if the Average FPS should be written to the FilePath.
		/// NOTE: If this is false the Average FPS will not be written to the File, it will just be
		/// returned by the Update() function.
		/// </summary>
		public static bool LogToFile
		{
			get { return mbLogToFile; }
			set { mbLogToFile = value; }
		}

		/// <summary>
		/// Get / Set the Time Interval (in seconds) Between Logging the Average FPS to the file.
		/// NOTE: This value must be greater than 1
		/// </summary>
		public static float TimeIntervalBetweenLogging
		{
			get { return mfTimeIntervalBetweenLogging; }
			set
			{
				// Store the given value
				float fTimeInterval = value;

				// If the specific Time Interval is valid.
				if (fTimeInterval > 1.0f)
				{
					mfTimeIntervalBetweenLogging = fTimeInterval;
				}
			}
		}

		/// <summary>
		/// Get / Set the amount of Time (in seconds) to wait before starting to Log the Average FPS.
		/// </summary>
		public static float TimeToWaitBeforeStarting
		{
			get { return mfTimeToWaitBeforeStarting; }
			set { mfTimeToWaitBeforeStarting = value; }
		}

		/// <summary>
		/// Get / Set the Number Of Times the Average FPS should be Logged.
		/// NOTE: Setting this to zero means that the Average FPS should be Logged forever.
		/// </summary>
		public static int NumberOfTimesToLog
		{
			get { return miNumberOfTimesToLog; }
			set { miNumberOfTimesToLog = value; }
		}

		/// <summary>
		/// Get the Number Of Times that the Average FPS has been Logged.
		/// </summary>
		public static int NumberOfTimesLogged
		{
			get { return miNumberOfTimesLogged; }
		}

		/// <summary>
		/// Get / Set if Logging should be performed or not.
		/// </summary>
		public static bool Enabled
		{
			get { return mbEnabled; }
			set { mbEnabled = value; }
		}
	}
}