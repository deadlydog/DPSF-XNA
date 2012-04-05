#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion

namespace DPSF
{
    /// <summary>
    /// Class to hold a List of Animations and the texture coordintes of the Pictures used by the Animations.
    /// To start, Create Picture's of all images that will be used in any Animations. Then Create an Animation
    /// by specifying the order of the Picture IDs to go through, and the speed to flip through them at (i.e. frame-rate).
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    public class Animations
    {
        /// <summary>
        /// Structure to store an individual Picture's position and dimensions within a texture
        /// </summary>
#if (WINDOWS)
        [Serializable]
#endif
        struct SPicture
        {
            public int iID;                         // The Unique ID of this Picture (used as its Index in the Pictures List)
            public Rectangle sTextureCoordinates;   // The Position and Dimensions of this Picture in the Texture

            /// <summary>
            /// Explicit constructor
            /// </summary>
            /// <param name="iID">The ID of this Picture (this should be unique)</param>
            /// <param name="sTextureCoordinates">The top-left (x,y) position and (width,height) dimensions
            /// of the Picture within the texture</param>
            public SPicture(int iID, Rectangle sTextureCoordinates)
            {
                this.iID = iID;
                this.sTextureCoordinates = sTextureCoordinates;
            }
        }

        /// <summary>
        /// Class to hold a single Animation's (i.e. Walking, Running, Jumping, etc) sequence of 
        /// Pictures and how long to display each Picture in the Animation for
        /// </summary>
#if (WINDOWS)
        [Serializable]
#endif
        class Animation
        {
            public int miID;                        // The unique ID of this Animation (used as its Index position in the Animation List)
            public List<int> mcPictureRotationOrder;// The Order to Rotate through the Pictures to make the Animation
            public int miCurrentPictureIndex;       // The Index of the Current Picture in the Picture List
            public float mfPictureRotationTime;     // The length of Time to wait before changing to the next Picture in the Animation
            public int miNumberOfTimesToPlay;       // The number of times the Animation should Play (repeats when it reaches the end)
            public int miNumberOfTimesPlayed;       // The number of times the Animation has Played already

            /// <summary>
            /// Explicit Constructor
            /// </summary>
            /// <param name="iID">The ID of this Animation (this should be unique)</param>
            /// <param name="cPictureRotationOrder">A List of Picture ID's which tell the sequence of 
            /// Pictures that make up the Animation</param>
            /// <param name="fPictureRotationTime">How long (in seconds) to wait before switching to the
            /// next Picture in the Picture Rotation Order</param>
            /// <param name="iNumberOfTimesToPlay">The Number of Times the Animation should Play before stopping. A value
            /// of zero means the Animation should repeat forever.</param>
            public Animation(int iID, List<int> cPictureRotationOrder, float fPictureRotationTime, int iNumberOfTimesToPlay)
            {
                miID = iID;
                mcPictureRotationOrder = new List<int>(cPictureRotationOrder);
                miCurrentPictureIndex = 0;
                mfPictureRotationTime = fPictureRotationTime;
                miNumberOfTimesToPlay = iNumberOfTimesToPlay;
                miNumberOfTimesPlayed = 0;
            }

            /// <summary>
            /// Returns the Picture ID of the Current Picture being displayed.
            /// </summary>
            public int CurrentPicturesID
            {
                get { return mcPictureRotationOrder[miCurrentPictureIndex]; }
            }

            /// <summary>
            /// Moves the Current Picture Index to the next element in the Picture Rotation Order, and loops
            /// if it reaches the end of the Animation
            /// </summary>
            public void MoveToNextPictureInAnimation()
            {
                // Increment the position in the Picture Rotation Order
                miCurrentPictureIndex++;

                // If we have reached the end of the Animation
                if (miCurrentPictureIndex >= mcPictureRotationOrder.Count)
                {
                    // If the Animation should Repeat
                    if (miNumberOfTimesToPlay == 0 ||
                        miNumberOfTimesPlayed < miNumberOfTimesToPlay)
                    {
                        // Start back at the beginning of the Animation
                        miCurrentPictureIndex = 0;

                        // Increment the Number of Times this Animation has Played
                        miNumberOfTimesPlayed++;
                    }
                    // Else the Animation shouldn't Repeat
                    else
                    {
                        // Record that the Animation has finished Playing
                        miNumberOfTimesPlayed = miNumberOfTimesToPlay;

                        // NOTE: miCurrentPictureIndex will now be invalid, as it
                        // will be equal to mcPictureRotationOrder.Count
                        miCurrentPictureIndex = mcPictureRotationOrder.Count;
                    }
                }
            }

            /// <summary>
            /// Get if the Animation has finished Playing or not.
            /// NOTE: Animations with Number Of Times To Play == 0 will never end
            /// </summary>
            public bool AnimationHasEnded
            {
                get { return ((miNumberOfTimesPlayed == miNumberOfTimesToPlay) && miNumberOfTimesToPlay != 0); }
            }
        }

        List<SPicture> mcPictures = new List<SPicture>();       // Holds all of the Pictures
        List<Animation> mcAnimations = new List<Animation>();   // Holds all of the Animations
        int miCurrentAnimationID = -1;      // The Index of the Animation that is Current being used
        float mfAnimationFrameTimer = 0.0f; // Used to determine when to move to the next Picture (i.e. Frame) in the Animation
        bool mbPaused = false;              // Tells if the Animation is Paused or not

        /// <summary>
        /// Copies the given Animations data into this Animation
        /// </summary>
        /// <param name="cAnimationToCopy">The Animation to Copy from</param>
        public void CopyFrom(Animations cAnimationToCopy)
        {
            int iIndex = 0;

            // Copy the simple class information
            miCurrentAnimationID = cAnimationToCopy.miCurrentAnimationID;
            mfAnimationFrameTimer = cAnimationToCopy.mfAnimationFrameTimer;

            // Copy the Pictures List
            mcPictures = new List<SPicture>(cAnimationToCopy.mcPictures);

            // Deep Copy the Animations List info (since it contains a reference types)
            int iNumberOfAnimations = cAnimationToCopy.mcAnimations.Count;
            mcAnimations = new List<Animation>(iNumberOfAnimations);
            for (iIndex = 0; iIndex < iNumberOfAnimations; iIndex++)
            {
                Animation cAnimation = new Animation(cAnimationToCopy.mcAnimations[iIndex].miID,
                                                     cAnimationToCopy.mcAnimations[iIndex].mcPictureRotationOrder,
                                                     cAnimationToCopy.mcAnimations[iIndex].mfPictureRotationTime,
                                                     cAnimationToCopy.mcAnimations[iIndex].miNumberOfTimesToPlay);

                mcAnimations.Add(cAnimation);
                mcAnimations[iIndex].miCurrentPictureIndex = cAnimationToCopy.mcAnimations[iIndex].miCurrentPictureIndex;
                mcAnimations[iIndex].miNumberOfTimesPlayed = cAnimationToCopy.mcAnimations[iIndex].miNumberOfTimesPlayed;
            }
        }

        /// <summary>
        /// Creates a Picture that can be used in a Animation, and returns its unique ID. 
        /// A Picture can be used multiple times in an Animation.
        /// </summary>
        /// <param name="sTextureCoordinates">The top-left (x,y) position and (width,height) dimensions
        /// in the Texture that form this Picture</param>
        /// <returns>Returns the new Picture's unique ID.</returns>
        public int CreatePicture(Rectangle sTextureCoordinates)
        {
            // Find where this Picture will be placed in the Pictures List
            int iPictureIndex = mcPictures.Count;

            // Create the new Picture, using the Index as the Pictures ID
            SPicture sPicture = new SPicture(iPictureIndex, sTextureCoordinates);

            // Add the new Picture to the Picture List
            mcPictures.Add(sPicture);

            // Return the unique ID of the new Picture
            return iPictureIndex;
        }

        /// <summary>
        /// Automatically creates the specified Total Number Of Pictures. All pictures are assumed to have 
        /// the same width and height, as specified in the First Picture rectangle. Also, the First Picture
        /// is assumed to be at the top-left corner of the Tileset.
        /// <para>Pictures are created in left-to-right, top-to-bottom order. The ID of the first Picture created
        /// is returned, with each new Picture created incrementing the ID value, so the last Picture created
        /// will have an ID of (returned ID + (Total Number Of Pictures - 1)).</para>
        /// </summary>
        /// <param name="iTotalNumberOfPictures">The Total Number Of Pictures in the Tileset</param>
        /// <param name="iPicturesPerRow">How many Pictures are in a row in the texture</param>
        /// <param name="sFirstPicture">The Position of the top-left Picture in the Tileset, and the
        /// width and height of each Picture in the Tileset</param>
        /// <returns>The ID of the first Picture created
        /// is returned, with each new Picture created incrementing the ID value, so the last Picture created
        /// will have an ID of (returned ID + (Total Number Of Pictures - 1)).</returns>
        public int CreatePicturesFromTileSet(int iTotalNumberOfPictures, int iPicturesPerRow, Rectangle sFirstPicture)
        {
            int iIndex = 0;
            int iLastPictureID = 0;

            // Loop through and create each Picture
            for (iIndex = 0; iIndex < iTotalNumberOfPictures; iIndex++)
            {
                // Calculate which Row and Column this Picture is at within the texture
                int iRow = iIndex / iPicturesPerRow;
                int iColumn = iIndex % iPicturesPerRow;

                // Calculate this Pictures Texture Coordinates
                Rectangle sRect = new Rectangle(sFirstPicture.X + (sFirstPicture.Width * iColumn),
                                                sFirstPicture.Y + (sFirstPicture.Height * iRow),
                                                sFirstPicture.Width, sFirstPicture.Height);

                // Create the new Picture
                iLastPictureID = CreatePicture(sRect);
            }

            // Calculate and return the ID of the first Picture created
            return (iLastPictureID - (iTotalNumberOfPictures - 1));
        }

        /// <summary>
        /// Creates a new Animation and returns the Animation's unique ID.
        /// <para>NOTE: Returns -1 if an invalid Picture ID was specified in the PictureRotationOrder.</para>
        /// <para>NOTE: Be sure to Create the Pictures before creating the Animation.</para>
        /// </summary>
        /// <param name="cPictureRotationOrder">A List of Picture IDs that specifies the Order of Pictures
        /// to Rotate through in order to produce the Animation. A single Picture ID can be used many times.</param>
        /// <param name="fPictureRotationTime">How long (in seconds) to wait before switching to the
        /// next Picture in the Picture Rotation Order (i.e. The frame-rate of the Animation)</param>
        /// <param name="iNumberOfTimesToPlay">The number of times this Animation should be played 
        /// (it replays when the end of the Animation is reached). Specify a value of zero to have the 
        /// Animation repeat forever</param>
        /// <returns>Returns the new Animation's unique ID.</returns>
        public int CreateAnimation(List<int> cPictureRotationOrder, float fPictureRotationTime, int iNumberOfTimesToPlay)
        {
            // Loop through the Picture Rotation Order
            int iNumberOfPictures = cPictureRotationOrder.Count;
            for (int iIndex = 0; iIndex < iNumberOfPictures; iIndex++)
            {
                // Get the specified Picture ID
                int iPictureID = cPictureRotationOrder[iIndex];

                // If the Picture ID is invalid
                if (iPictureID < 0 || iPictureID > mcPictures.Count)
                {
                    // Return that an invalid Picture ID was specified
                    return -1;
                }
            }

            // Find where this Animation will be placed in the Animations List
            int iAnimationIndex = mcAnimations.Count;

            // Create the new Animation
            Animation cAnimation = new Animation(iAnimationIndex, cPictureRotationOrder, fPictureRotationTime, iNumberOfTimesToPlay);

            // Add the new Animation to the Animations List
            mcAnimations.Add(cAnimation);

            // Return the ID of the new Animation
            return iAnimationIndex;
        }

        /// <summary>
        /// Creates a new Animation and returns the Animation's unique ID.
        /// <para>NOTE: Returns -1 if an invalid Picture ID was specified in the PictureRotationOrder.</para>
        /// <para>NOTE: Be sure to Create the Pictures before creating the Animation.</para>
        /// </summary>
        /// <param name="iaPictureRotationOrder">An array of Picture IDs that specifies the Order of Pictures
        /// to Rotate through in order to produce the Animation</param>
        /// <param name="fPictureRotationTime">How long (in seconds) to wait before switching to the
        /// next Picture in the Picture Rotation Order (i.e. The next Frame in the Animation)</param>
        /// <param name="iNumberOfTimesToPlay">The number of times this Animation should be played 
        /// (it replays when the end of the Animation is reached). Specify a value of zero to have the 
        /// Animation repeat forever</param>
        /// <returns>Returns the new Animation's unique ID.
        /// NOTE: Returns -1 if an invalid Picture ID was specified in the PictureRotationOrder.</returns>
        public int CreateAnimation(int[] iaPictureRotationOrder, float fPictureRotationTime, int iNumberOfTimesToPlay)
        {
            int iIndex = 0;

            // Get the Number of elements in the Picture Rotation Order array
            int iSizeOfArray = iaPictureRotationOrder.Length;

            // List to hold all of the Picture Rotation Order values
            List<int> cPictureRotationOrder = new List<int>(iSizeOfArray);

            // Loop through each of the elements in the Picture Rotation Order array and store them in the List
            for (iIndex = 0; iIndex < iSizeOfArray; iIndex++)
            {
                cPictureRotationOrder.Add(iaPictureRotationOrder[iIndex]);
            }

            // Create the Animation
            return CreateAnimation(cPictureRotationOrder, fPictureRotationTime, iNumberOfTimesToPlay);
        }

        /// <summary>
        /// Returns true if the given Picture ID is valid (i.e. A Picture with the same ID exists).
        /// </summary>
        /// <param name="iPictureID">The Picture ID to look for</param>
        /// <returns>Returns true if the given Picture ID is valid (i.e. A Picture with the same ID exists).</returns>
        public bool PictureIDIsValid(int iPictureID)
        {
            return (iPictureID >= 0 && iPictureID < mcPictures.Count);
        }

        /// <summary>
        /// Returns true if the given Animation ID is valid (i.e. An Animation with the same ID exists).
        /// </summary>
        /// <param name="iAnimationID">The Animation ID to look for</param>
        /// <returns>Returns true if the given Animation ID is valid (i.e. An Animation with the same ID exists).</returns>
        public bool AnimationIDIsValid(int iAnimationID)
        {
            return (iAnimationID >= 0 && iAnimationID < mcAnimations.Count);
        }

        /// <summary>
        /// Get / Set the Current Animation being used. The Animation is started at its beginning.
        /// <para>NOTE: If an invalid Animiation ID is given when Setting, the Animation will not be changed.</para>
        /// <para>NOTE: If an Animation has not beeng set yet when Getting, -1 is returned.</para>
        /// </summary>
        public int CurrentAnimationID
        {
            get { return miCurrentAnimationID; }
            set
            {
                // Temporarily store the given Animation Index
                int iNewAnimationIndex = value;

                // If the given Animation Index is valid
                if (iNewAnimationIndex < mcAnimations.Count && iNewAnimationIndex >= 0)
                {
                    // Use the new Animation
                    miCurrentAnimationID = iNewAnimationIndex;

                    // Start from the beginning of the Animation
                    mcAnimations[miCurrentAnimationID].miCurrentPictureIndex = 0;
                    mcAnimations[miCurrentAnimationID].miNumberOfTimesPlayed = 0;

                    // Reset the Animation Timer
                    mfAnimationFrameTimer = 0.0f;
                }
            }
        }

        /// <summary>
        /// Sets the Current Animation being used, as well as what index in the Animation's Picture Rotation
        /// Order the Animation should start at. 
        /// <para>NOTE: If the specified Animiation to use is not valid, the Current Animation will not be 
        /// changed, and if the specified Picture Rotation Order Index is not valid, the Animation will 
        /// start from the beginning of the Animation.</para>
        /// </summary>
        /// <param name="iAnimationID">The ID of the Animation to use</param>
        /// <param name="iPictureRotationOrderIndex">The Index in the Animation's Picture Rotation Order
        /// that the Animation should begin playing from</param>
        public void SetCurrentAnimationAndPositionInAnimation(int iAnimationID, int iPictureRotationOrderIndex)
        {
            // If the given Animation Index is valid
            if (AnimationIDIsValid(iAnimationID))
            {
                // Use the new Animation
                miCurrentAnimationID = iAnimationID;

                // If the specified Picture Rotation Order Index is valid
                if (iPictureRotationOrderIndex >= 0 &&
                    iPictureRotationOrderIndex < mcAnimations[miCurrentAnimationID].mcPictureRotationOrder.Count)
                {
                    // Start the Animation at the specified position in the Picture Rotation Order
                    mcAnimations[miCurrentAnimationID].miCurrentPictureIndex = iPictureRotationOrderIndex;
                }
                // Else the specified Picture Rotation Order is not valid
                else
                {
                    // So just start at the beginning of the Animation
                    mcAnimations[miCurrentAnimationID].miCurrentPictureIndex = 0;
                }

                // Mark the Animation as not having Played at all yet
                mcAnimations[miCurrentAnimationID].miNumberOfTimesPlayed = 0;

                // Reset the Animation Timer
                mfAnimationFrameTimer = 0.0f;
            }
        }

        /// <summary>
        /// Returns how much Time (in seconds) should elapse before switching frames in the Animation.
        /// <para>NOTE: Returns zero if the specified Animation ID is not valid.</para>
        /// </summary>
        /// <param name="iAnimationID">The ID of the Animation containing the Picture Rotation Time to retrive</param>
        /// <returns>Returns how much Time (in seconds) should elapse before switching frames in the Animation.
        /// NOTE: Returns zero if the specified Animation ID is not valid.</returns>
        public float GetAnimationsPictureRotationTime(int iAnimationID)
        {
            // Variable to hold the Animation's Picture Rotation Time
            float fPictureRotationTime = 0.0f;

            // If the specified Animation ID is valid
            if (AnimationIDIsValid(iAnimationID))
            {
                // Get the Animations Picture Rotation Time
                fPictureRotationTime = mcAnimations[iAnimationID].mfPictureRotationTime;
            }

            // Return the Picture Rotation Time
            return fPictureRotationTime;
        }

        /// <summary>
        /// Sets how much Time should elapse before switching frames in the Animation
        /// </summary>
        /// <param name="iAnimationID">The ID of the Animation to update</param>
        /// <param name="fNewPictureRotationTime">The Time (in seconds) to wait before moving to the
        /// next Picture in the Animations Picture Rotation Order</param>
        public void SetAnimationsPictureRotationTime(int iAnimationID, float fNewPictureRotationTime)
        {
            // If the specified Animation ID is valid
            if (AnimationIDIsValid(iAnimationID))
            {
                // Set the Animations new Picture Rotation Time
                mcAnimations[iAnimationID].mfPictureRotationTime = fNewPictureRotationTime;
            }
        }

        /// <summary>
        /// Get / Set how much Time should elapsed before switching frames in the Current Animation. 
        /// <para>NOTE: If no Animation has been set yet, zero will be returned.</para>
        /// </summary>
        public float CurrentAnimationsPictureRotationTime
        {
            get { return GetAnimationsPictureRotationTime(miCurrentAnimationID); }
            set { SetAnimationsPictureRotationTime(miCurrentAnimationID, value); }
        }

        /// <summary>
        /// Get / Set the Current Index in the Current Animation's Picture Rotation Order. 
        /// <para>NOTE: If no Animation has been set yet, Get returns -1, and Set doesn't change anything 
        /// (as well as if the specified Index is invalid).</para>
        /// </summary>
        public int CurrentAnimationsPictureRotationOrderIndex
        {
            get
            {
                // If there is a Current Animation
                if (CurrentAnimationIsValid)
                {
                    // Return the Current position in the Picture Rotation Order
                    return mcAnimations[miCurrentAnimationID].miCurrentPictureIndex;
                }
                // Else no Animation has been set yet
                else
                {
                    return -1;
                }
            }

            set
            {
                // Temporarily store the given Index to use
                int iNewPictureRotationOrderIndex = value;

                // If there is a Current Animation
                if (CurrentAnimationIsValid)
                {
                    // If the new Index is valid
                    if (iNewPictureRotationOrderIndex >= 0 &&
                        iNewPictureRotationOrderIndex < mcAnimations[miCurrentAnimationID].mcPictureRotationOrder.Count)
                    {
                        // Set the New Current position in the Picture Rotation Order
                        mcAnimations[miCurrentAnimationID].miCurrentPictureIndex = iNewPictureRotationOrderIndex;

                        // Reset the Animation Timer
                        mfAnimationFrameTimer = 0.0f;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the Number of times the given Animation ID is set to Play.
        /// Zero means the Animation will repeat forever.
        /// <para>NOTE: If the given Animation ID is invalid, -1 is returned.</para>
        /// </summary>
        /// <param name="iAnimationID">The ID of the Animation to update</param>
        /// <returns>Returns the Number of times the given Animation ID is set to Play.
        /// Zero means the Animation will repeat forever.
        /// NOTE: If the given Animation ID is invalid, -1 is returned.</returns>
        public int GetAnimationsNumberOfTimesToPlay(int iAnimationID)
        {
            // Variable to hold the Animation's Number Of Times To Play
            int iNumberOfTimesToPlay = -1;

            // If the specified Animation ID is valid
            if (AnimationIDIsValid(iAnimationID))
            {
                // Get the Animations Number Of Times To Play
                iNumberOfTimesToPlay = mcAnimations[iAnimationID].miNumberOfTimesToPlay;
            }

            // Return the Number Of Times To Play
            return iNumberOfTimesToPlay;
        }

        /// <summary>
        /// Sets the Number of times the given Animation ID should Play
        /// (it replays when the end of the Animation is reached). 
        /// Specify a value of zero to have the Animation repeat forever.
        /// <para>NOTE: If the given Animation ID is invalid, no changes are made.</para>
        /// </summary>
        /// <param name="iAnimationID">The ID of the Animation to update</param>
        /// <param name="iNewNumberOfTimesToPlay">The New Number of times the Animation should Play</param>
        public void SetAnimationsNumberOfTimesToPlay(int iAnimationID, int iNewNumberOfTimesToPlay)
        {
            // If the specified Animation ID is valid
            if (AnimationIDIsValid(iAnimationID))
            {
                // Set the Animations Number Of Times To Play
                mcAnimations[iAnimationID].miNumberOfTimesToPlay = iNewNumberOfTimesToPlay;
            }
        }

        /// <summary>
        /// Get / Set the Number of times the Current Animation should Play
        /// (it replays when the end of the Animation is reached). 
        /// Specify a value of zero to have the Animation repeat forever.
        /// <para>NOTE: If no Animation has been set yet, no changes are made when
        /// Setting, and -1 is returned when Getting.</para>
        /// </summary>
        public int CurrentAnimationsNumberOfTimesToPlay
        {
            get { return GetAnimationsNumberOfTimesToPlay(miCurrentAnimationID); }
            set { SetAnimationsNumberOfTimesToPlay(miCurrentAnimationID, value); }
        }

        /// <summary>
        /// Get / Set the Number of times the Current Animation has Played already.
        /// <para>NOTE: If no Animation has been set yet, Get returns -1, and Set doesn't change anything.</para>
        /// </summary>
        public int CurrentAnimationsNumberOfTimesPlayed
        {
            get
            {
                // If there is a Current Animation
                if (CurrentAnimationIsValid)
                {
                    // Return the Current Animations Number of Times Played
                    return mcAnimations[miCurrentAnimationID].miNumberOfTimesPlayed;
                }
                // Else no Animation has been set yet
                else
                {
                    return -1;
                }
            }

            set
            {
                // Temporarily store the given Number of Times Played
                int iNewNumberOfTimesPlayed = value;

                // If there is a Current Animation
                if (CurrentAnimationIsValid)
                {
                    // Set the New Number Of Times the Current Animation has already Played
                    mcAnimations[miCurrentAnimationID].miNumberOfTimesPlayed = iNewNumberOfTimesPlayed;
                }
            }
        }

        /// <summary>
        /// Get if the Current Animation is Done Playing or not (i.e. Its Number Of Times Played is
        /// greater than or equal to its Number Of Times To Play). Returns true even if no
        /// Animation has been set to Play yet.
        /// </summary>
        public bool CurrentAnimationIsDonePlaying
        {
            get
            {
                // If there is a Current Animation
                if (CurrentAnimationIsValid)
                {
                    // Get how many Times the Current Animation should Play
                    int iNumberOfTimesToPlay = CurrentAnimationsNumberOfTimesToPlay;

                    // Return if the Current Animation has played the specified number of times already
                    // or not, and it's not set to play forever
                    return (iNumberOfTimesToPlay != 0 && CurrentAnimationsNumberOfTimesPlayed >= iNumberOfTimesToPlay);
                }
                // Else no Animation has been set yet
                else
                {
                    // So return that the Current Animatino is done Playing (since there isn't one)
                    return true;
                }
            }
        }

        /// <summary>
        /// Returns the amount of Time required to play the specified Animation.
        /// <para>NOTE: If an invalid AnimationID is specified, zero is returned.</para>
        /// </summary>
        /// <param name="iAnimationID">The ID of the Animation to check</param>
        /// <returns>Returns the amount of Time required to play the specified Animation.
        /// NOTE: If an invalid AnimationID is specified, zero is returned.</returns>
        public float TimeRequiredToPlayAnimation(int iAnimationID)
        {
            // If the specified Animation ID is valid
            if (AnimationIDIsValid(iAnimationID))
            {
                // Get a handle to the Animation to check
                Animation cAnimation = mcAnimations[iAnimationID];

                // Return how long it will take to Play the specified Animation
                return (cAnimation.mcPictureRotationOrder.Count * cAnimation.mfPictureRotationTime * cAnimation.miNumberOfTimesToPlay);
            }
            // Else the AnimationID was not valid, so return zero
            return 0.0f;
        }

        /// <summary>
        /// Gets the amount of Time (in seconds) required to play the Current Animation.
        /// <para>NOTE: If no Animation has been played yet, zero is returned.</para>
        /// </summary>
        public float TimeRequiredToPlayCurrentAnimation
        {
            get { return TimeRequiredToPlayAnimation(miCurrentAnimationID); }
        }

        /// <summary>
        /// Gets the amount of Time (in seconds) required to play the remainder of the Current Animation.
        /// <para>NOTE: If no Animation has been played yet, zero is returned.</para>
        /// </summary>
        public float TimeRequiredToPlayTheRestOfTheCurrentAnimation
        {
            get
            {
                // If the specified Animation ID is valid
                if (CurrentAnimationIsValid)
                {
                    // Get a handle to the Animation to check
                    Animation cAnimation = mcAnimations[miCurrentAnimationID];

                    // Calculate how much time is left in the current frame
                    float fFrameRemainderTime = cAnimation.mfPictureRotationTime - mfAnimationFrameTimer;

                    // Calculate how much time is left to finish the animation without repeats
                    float fNoRepeatRemainderTime = ((cAnimation.mcPictureRotationOrder.Count - (cAnimation.miCurrentPictureIndex + 1)) * cAnimation.mfPictureRotationTime) + fFrameRemainderTime;

                    // Calculate how much time it will take to play all of the repeats
                    float fRepeatsRemainingTime = (cAnimation.mcPictureRotationOrder.Count * cAnimation.mfPictureRotationTime) * (cAnimation.miNumberOfTimesToPlay - (cAnimation.miNumberOfTimesPlayed + 1));

                    // Return how long it will take to Play the remainder of the Animation
                    return (fNoRepeatRemainderTime + fRepeatsRemainingTime);
                }
                // Else no Animation has been set yet
                // So return zero
                return 0.0f;
            }
        }

        /// <summary>
        /// Returns the Rectangle representing the Texture Coordinates of the specified Picture.
        /// </summary>
        /// <param name="iPictureID">The Picture ID of the Picture whose Texture Coordinates 
        /// should be retrieved</param>
        /// <returns>Returns the Rectangle representing the Texture Coordinates of the specified Picture.</returns>
        public Rectangle GetPicturesTextureCoordinates(int iPictureID)
        {
            // The Rectangle containing the Texture Coordinates to be returned
            Rectangle cTextureCoordinates = new Rectangle();

            // If the Picture ID is valid
            if (iPictureID >= 0 && iPictureID < mcPictures.Count)
            {
                // Get the specified Picture's Texture Coordinates
                cTextureCoordinates = mcPictures[iPictureID].sTextureCoordinates;
            }

            // Return the Texture Coordinates
            return cTextureCoordinates;
        }

        /// <summary>
        /// Get the Rectangle representing the Texture Coordinates of the Picture 
        /// in the Animation that should be displayed at this point in time
        /// </summary>
        public Rectangle CurrentPicturesTextureCoordinates
        {
            get
            {
                // Temp variable to hold the Current Animation's Picture rectangle
                Rectangle sRect = new Rectangle();

                // If there is a Current Animation and it is not Done Playing yet
                if (CurrentAnimationIsValid && !CurrentAnimationIsDonePlaying)
                {
                    // Store the Rectangle 
                    sRect = mcPictures[mcAnimations[miCurrentAnimationID].CurrentPicturesID].sTextureCoordinates;
                }

                // Return the Rectangle containing the Texture Coordinates to use
                return sRect;
            }
        }

        /// <summary>
        /// Get / Set if the Animation should be Paused or not. If Paused, the Animation will
        /// not be Updated.
        /// </summary>
        public bool Paused
        {
            get { return mbPaused; }
            set { mbPaused = value; }
        }

        /// <summary>
        /// Updates the Animation according to how much time has elapsed
        /// </summary>
        /// <param name="fElapsedTime">The amount of Time (in seconds) since the last Update</param>
        public void Update(float fElapsedTime)
        {
            // If the Animation is Paused or no Animation has been set yet
            if (Paused || !CurrentAnimationIsValid)
            {
                // Do nothing
                return;
            }
            // Else an Animation should be playing

            // Add the Elapsed Time to the Animation Timer
            mfAnimationFrameTimer += fElapsedTime;

            // If it's time to move to the next Picture in the Animation
            if (mfAnimationFrameTimer >= mcAnimations[miCurrentAnimationID].mfPictureRotationTime)
            {
                // Subtract the Animation's Picture Rotation Time from the Animation Timer
                mfAnimationFrameTimer -= mcAnimations[miCurrentAnimationID].mfPictureRotationTime;

                // Move to the next Picture in the Animation
                mcAnimations[miCurrentAnimationID].MoveToNextPictureInAnimation();
            }
        }

        /// <summary>
        /// Get if the Current Animation has been set yet or not
        /// </summary>
        private bool CurrentAnimationIsValid
        {
            get { return AnimationIDIsValid(miCurrentAnimationID); }
        }
    }
}
