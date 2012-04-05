using System;

namespace DPSF
{
    /// <summary>
    /// Class that may be used to obtain random numbers. This class inherits the Random class
    /// and adds additional functionality.
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    public class RandomNumbers : Random
    {
        /// <summary>
        /// Returns a random number between the specified values
        /// </summary>
        /// <param name="fValue1">The first value</param>
        /// <param name="fValue2">The second value</param>
        /// <returns>Returns a random number between the specified values</returns>
        public float Between(float fValue1, float fValue2)
        {
            return fValue1 + ((float)NextDouble() * (fValue2 - fValue1));
        }

        /// <summary>
        /// Returns a random number between 0.0f and 1.0f.
        /// </summary>
        /// <returns>Returns a random number between 0.0f and 1.0f.</returns>
        public float NextFloat()
        {
            return (float)NextDouble();
        }
    }
}
