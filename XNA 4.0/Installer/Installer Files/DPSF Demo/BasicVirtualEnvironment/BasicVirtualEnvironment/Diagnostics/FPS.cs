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

#endregion

namespace BasicVirtualEnvironment.Diagnostics
{
    /// <summary>
    /// Static class used to display the current and average Frames Per Second. 
    /// The Update() function should be called every frame.
    /// </summary>
    public static class FPS
    {
        // Frames Per Second variables.
        private static int miFPS = 0;           // The number of Frames achieved in the last second.
        private static int miFPSCount = 0;      // The number of Frames achieved so far in this second.
        private static float mfTimeSinceLastUpdateInSeconds = 0.0f;

        // Average Frames Per Second variables.
        private static int miNumberOfSecondsToComputerAverageOver = 10;
        private static Queue<int> mcFPSQueue = new Queue<int>();
        private static int miFPSQueueSum = 0;   // Stores the Sum of all the FPS numbers in the FPS Queue.
        private static float mfAvgFPS = 0.0f;   // The Average number of Frames achieved over the last miNumberOfSecondsToComputerAverageOver seconds.

        /// <summary>
        /// Event handler that fires every second, directly after the CurrentFPS and AverageFPS have been updated.
        /// </summary>
        public static event EventHandler<FPSEventArgs> FPSUpdated = delegate { };

        /// <summary>
        /// Event args used to pass the FPS info to in an event handler.
        /// </summary>
        public class FPSEventArgs : EventArgs
        {
            /// <summary>
            /// The Frames Per Second achieved in the past second.
            /// </summary>
            public int FPS { get; set; }

            /// <summary>
            /// The Average Frame Per Second achieved.
            /// </summary>
            public float AverageFPS { get; set; }
        }

        // Event args passed into the event handler.
        // We use a static instance instead of creating a new instance each time for the sake of garbage collection.
        private static FPSEventArgs _fpsEventArgs = new FPSEventArgs();

        /// <summary>
        /// This function should be called every Frame and is used to update how many FPS were achieved.
        /// </summary>
        /// <param name="elapsedTimeInSeconds">The elapsed time since the last Frame was drawn, in seconds.</param>
        public static void Update(float elapsedTimeInSeconds)
        {
            // Increment the Frames Per Second Counter.
            miFPSCount++;

            // Update the total time since the FPS was last Updated.
            mfTimeSinceLastUpdateInSeconds += elapsedTimeInSeconds;

            // If one second has passed since the last FPS Update.
            if (mfTimeSinceLastUpdateInSeconds >= 1.0f)
            {
                // Update the number of FPS achieved so that it can be displayed.
                miFPS = miFPSCount;

                // Subtract one second from the Time Since the Last FPS Update.
                mfTimeSinceLastUpdateInSeconds -= 1.0f;

                // Reset the FPS Counter to zero.
                miFPSCount = 0;

                // Store the last seconds FPS count in the FPS Queue.
                mcFPSQueue.Enqueue(miFPS);

                // Add the last seconds FPS count to the FPS Queue Sum.
                miFPSQueueSum += miFPS;

                // Only store the last miNumberOfSecondsToComputerAverageOver FPS counts.
                if (mcFPSQueue.Count > miNumberOfSecondsToComputerAverageOver)
                {
                    // Remove the oldest FPS count from the FPS Queue and subtract it's value from the Queue Sum.
                    miFPSQueueSum -= mcFPSQueue.Dequeue();
                }

                // Calculate the Average FPS.
                mfAvgFPS = (float)miFPSQueueSum / (float)mcFPSQueue.Count;

                // Let any listeners know that the FPS and Average FPS have been updated.
                _fpsEventArgs.FPS = miFPS;
                _fpsEventArgs.AverageFPS = mfAvgFPS;
                FPSUpdated(null, _fpsEventArgs);
            }
        }

        /// <summary>
        /// Get the current number of Frames Per Second being achieved.
        /// </summary>
        public static int CurrentFPS
        {
            get { return miFPS; }
        }

        /// <summary>
        /// Get the Average number of Frames Per Second being achieved.
        /// </summary>
        /// <returns></returns>
        public static float AverageFPS
        {
            get { return mfAvgFPS; }
        }

        /// <summary>
        /// Get / Set the Number of Seconds that the Average FPS should be computed over.
        /// </summary>
        /// <param name="iSeconds">The Number of Seconds that the Average FPS should be computed over. 
        /// NOTE: This must be greater than 1.</param>
        public static int NumberOfSecondsToComputeAverageOver
        {
            get { return miNumberOfSecondsToComputerAverageOver; }
            set
            {
                // Store the given value.
                int iSeconds = value;

                // If a valid number has been specified.
                if (iSeconds > 1)
                {
                    // Set the Number Of Seconds that the Average should be Computed Over.
                    miNumberOfSecondsToComputerAverageOver = iSeconds;

                    // Resize the Queue if it is needed
                    while (mcFPSQueue.Count > miNumberOfSecondsToComputerAverageOver)
                    {
                        // Remove an item from the end of the FPS Queue and subtract it from the Queue's Sum.
                        miFPSQueueSum -= mcFPSQueue.Dequeue();
                    }

                    // Calculate the new Average FPS.
                    mfAvgFPS = (float)miFPSQueueSum / (float)mcFPSQueue.Count;
                }
            }
        }

        /// <summary>
        /// Function to erase the current FPS values being used to calculate the Average and to start over.
        /// </summary>
        public static void ResetAverageFPS()
        {
            // Reset the Average FPS variables
            mfAvgFPS = 0.0f;
            miFPSQueueSum = 0;
            mcFPSQueue.Clear();
        }
    }
}
