#region File Description
//===================================================================
// FPS.cs
// 
// This file provides functions to keep track of the current and
// average number of Frames Per Second being achieved by the application.
//
// How To Use:
//  You do not need to create an instance of the FPS class since it
//  is a static class. Just call the Update() function every frame
//  (so likely from within the Draw() function), and check the
//  CurrentFPS and/or AverageFPS properties to read the Frames Per Second.
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
#endregion

namespace DPSF
{
    /// <summary>
    /// Static class used to display the current and average Frames Per Second. 
    /// The Update() function should be called every frame.
    /// </summary>
    public static class FPS
    {
        // Frames Per Second variables
        private static int miFPS = 0;           // The number of Frames achieved in the last second
        private static int miFPSCount = 0;      // The number of Frames achieved so far in this second
        private static float mfTimeSinceLastUpdateInSeconds = 0.0f;

        // Average Frames Per Second variables
        private static int miNumberOfSecondsToComputerAverageOver = 10;
        private static Queue<int> mcFPSQueue = new Queue<int>();
        private static int miFPSQueueSum = 0;   // Stores the Sum of all the FPS numbers in the FPS Queue
        private static float mfAvgFPS = 0.0f;   // The Average number of Frames achieved over the last miNumberOfSecondsToComputerAverageOver seconds


        /// <summary>
        /// This function should be called every Frame and is used to update how many FPS were achieved
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">The elapsed time since the last Frame was drawn, in seconds</param>
        public static void Update(float fElapsedTimeInSeconds)
        {
            // Increment the Frames Per Second Counter
            miFPSCount++;

            // Update the total time since the FPS was last Updated
            mfTimeSinceLastUpdateInSeconds += fElapsedTimeInSeconds;

            // If one second has passed since the last FPS Update
            if (mfTimeSinceLastUpdateInSeconds >= 1.0f)
            {
                // Update the number of FPS achieved so that it can be displayed
                miFPS = miFPSCount;

                // Subtract one second from the Time Since the Last FPS Update
                mfTimeSinceLastUpdateInSeconds -= 1.0f;

                // Reset the FPS Counter to zero
                miFPSCount = 0;

                // Store the last seconds FPS count in the FPS Queue
                mcFPSQueue.Enqueue(miFPS);

                // Add the last seconds FPS count to the FPS Queue Sum
                miFPSQueueSum += miFPS;

                // Only store the last miNumberOfSecondsToComputerAverageOver FPS counts
                if (mcFPSQueue.Count > miNumberOfSecondsToComputerAverageOver)
                {
                    // Remove the oldest FPS count from the FPS Queue and subtract it's value from the Queue Sum
                    miFPSQueueSum -= mcFPSQueue.Dequeue();
                }

                // Calculate the Average FPS
                mfAvgFPS = (float)miFPSQueueSum / (float)mcFPSQueue.Count;
            }
        }

        /// <summary>
        /// Get the current number of Frames Per Second being achieved
        /// </summary>
        public static int CurrentFPS
        {
            get { return miFPS; }
        }

        /// <summary>
        /// Get the Average number of Frames Per Second being achieved
        /// </summary>
        /// <returns></returns>
        public static float AverageFPS
        {
            get { return mfAvgFPS; }
        }

        /// <summary>
        /// Get / Set the Number of Seconds that the Average FPS should be computed over
        /// </summary>
        /// <param name="iSeconds">The Number of Seconds that the Average FPS should be computed over. 
        /// NOTE: This must be greater than 1</param>
        public static int NumberOfSecondsToComputeAverageOver
        {
            get { return miNumberOfSecondsToComputerAverageOver; }
            set
            {
                // Store the given value
                int iSeconds = value;

                // If a valid number has been specified
                if (iSeconds > 1)
                {
                    // Set the Number Of Seconds that the Average should be Computed Over
                    miNumberOfSecondsToComputerAverageOver = iSeconds;

                    // Resize the Queue if it is needed
                    while (mcFPSQueue.Count > miNumberOfSecondsToComputerAverageOver)
                    {
                        // Remove an item from the end of the FPS Queue and subtract it from the Queue's Sum
                        miFPSQueueSum -= mcFPSQueue.Dequeue();
                    }

                    // Calculate the new Average FPS
                    mfAvgFPS = (float)miFPSQueueSum / (float)mcFPSQueue.Count;
                }
            }
        }

        /// <summary>
        /// Function to erase the current FPS values being used to calculate the Average and to start over
        /// </summary>
        public static void ResetAverageFPS()
        {
            // Reset the Average FPS variables
            mfAvgFPS = 0.0f;
            miFPSQueueSum = 0;
            mcFPSQueue.Clear();
        }
    }

    /// <summary>
    /// Static class used to log the average FPS to a file at a specified interval.
    /// The Update() function should be called every frame.
    /// </summary>
    public static class FPSLogger
    {
        private static string msFilePath = "AverageFPSLog.txt"; // The File Path to write the Log to
        private static float mfTimeIntervalBetweenLogging = 10; // How long (in seconds) between writing the Average FPS to the file
        private static float mfTimeElapsedSinceLastLog = 0;     // How much Time (in seconds) has Elapsed since we last logged the Average FPS

        private static float mfTimeToWaitBeforeStarting = 0;    // How much Time should pass before starting the Logging
        private static bool mbInitialWaitTimeOver = false;      // Tells if the the Time To Wait Before Starting has elapsed or not

        private static int miNumberOfTimesToLog = 0;            // How many times the Average FPS should be Logged
        private static int miNumberOfTimesLogged = 0;           // How many times the Average FPS has been Logged

        private static bool mbEnabled = true;                   // Tells if Logging should be performed or not
        private static bool mbLogToFile = true;                 // Tells if the Logging should be written to the File or not

        /// <summary>
        /// This function should be called every Frame and is used to log the Average FPS to the file. If this function
        /// writes to the Log file, the Average FPS that was written to the file is returned. Otherwise it returns zero.
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">The elapsed time since the last Frame was drawn, in seconds</param>
        /// <returns>If this function writes to the Log file, the Average FPS that was written to the file is returned. 
        /// Otherwise it returns zero.</returns>
        public static float Update(float fElapsedTimeInSeconds)
        {
            // Variable to hold the value to return
            float fAverageFPSToReturn = 0f;

            // If Logging is not Enabled
            if (!mbEnabled)
            {
                // Exit without doing anything
                return fAverageFPSToReturn;
            }

            // Add the Elapsed Time to our Time Elapsed Since Last Logging the FPS
            mfTimeElapsedSinceLastLog += fElapsedTimeInSeconds;

            // If the Inital Wait Time has not elapsed yet
            if (!mbInitialWaitTimeOver)
            {
                // If the Initial Wait Time has now elapsed
                if (mfTimeElapsedSinceLastLog >= mfTimeToWaitBeforeStarting)
                {
                    // Record that the Initial Wait Time has elapsed
                    mbInitialWaitTimeOver = true;

                    // Subtract the Initial Wait Time from the Last Log Time
                    mfTimeElapsedSinceLastLog -= mfTimeToWaitBeforeStarting;
                }
                // Else the Initial Wait Time has not elapsed yet
                else
                {
                    // So exit the function without Logging anything
                    return fAverageFPSToReturn;
                }
            }

            // If we have not Logged the max amount of Logs yet
            if (miNumberOfTimesLogged < miNumberOfTimesToLog || miNumberOfTimesToLog == 0)
            {
                // If enough Time has elapsed that a Log should be performed
                if (mfTimeElapsedSinceLastLog >= mfTimeIntervalBetweenLogging)
                {
                    // Subtract the Interval Time from our Last Log Time
                    mfTimeElapsedSinceLastLog -= mfTimeIntervalBetweenLogging;

                    // Increment the Number Of Times the Average FPS was Logged
                    miNumberOfTimesLogged++;

                    // Save the value that is going to be written to the Log
                    fAverageFPSToReturn = FPS.AverageFPS;

                    // If the Average FPS should be Logged to the File
                    if (mbLogToFile)
                    {
                        // Get a handle to the File to write to, and turn Appending on
                        TextWriter cFile = new StreamWriter(msFilePath, true);

                        // Write the Average FPS to the File
                        string sLog = miNumberOfTimesLogged.ToString() + " - " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + fAverageFPSToReturn.ToString();
                        cFile.WriteLine(sLog);

                        // Close the handle to the File since we are done writing for now
                        cFile.Close();
                    }
                }
            }

            // Return the Average FPS if it was written to the file
            return fAverageFPSToReturn;
        }

        /// <summary>
        /// Get / Set the File Path that the Average FPS should be logged to
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

                // If the specifiec Time Interval is valid
                if (fTimeInterval > 1.0f)
                {
                    mfTimeIntervalBetweenLogging = fTimeInterval;
                }
            }
        }

        /// <summary>
        /// Get / Set the amount of Time (in seconds) to wait before starting to Log the Average FPS
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
        /// Get the Number Of Times that the Average FPS has been Logged
        /// </summary>
        public static int NumberOfTimesLogged
        {
            get { return miNumberOfTimesLogged; }
        }

        /// <summary>
        /// Get / Set if Logging should be performed or not
        /// </summary>
        public static bool Enabled
        {
            get { return mbEnabled; }
            set { mbEnabled = value; }
        }
    }
}
