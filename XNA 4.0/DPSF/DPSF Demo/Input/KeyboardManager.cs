using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Input
{
	public static class KeyboardManager
	{
		/// <summary>
		/// Used to control user input speed.
		/// </summary>
		private static TimeSpan _inputTimeSpan = TimeSpan.Zero;

		/// <summary>
		/// Gets the current state of the keyboard.
		/// </summary>
		public static KeyboardState CurrentKeyboardState { get { return _currentKeyboardState; } }
		private static KeyboardState _currentKeyboardState = new KeyboardState();

		/// <summary>
		/// Gets the previous state of the keyboard (i.e. the keyboard state last frame).
		/// </summary>
		public static KeyboardState PreviousKeyboardState { get { return _previousKeyboardState; } }
		private static KeyboardState _previousKeyboardState = new KeyboardState();

		/// <summary>
		/// Updates the keyboard state at this frame.
		/// <para>NOTE: This should be called every frame, and only once per frame.</para>
		/// <para>NOTE: This should be called before calling any other functions in this class.</para>
		/// </summary>
		/// <param name="timeElapsedSinceLastFrame">The time elapsed since last frame.</param>
		public static void UpdateKeyboardStateForThisFrame(TimeSpan timeElapsedSinceLastFrame)
		{
			// Backup the keyboard state before getting its new value.
			_previousKeyboardState = _currentKeyboardState;
			_currentKeyboardState = Keyboard.GetState();

			// Add how long it's been since the last user input was received.
			_inputTimeSpan += timeElapsedSinceLastFrame;
		}

		/// <summary>
		/// Returns true if the Key is being pressed down.
		/// </summary>
		/// <param name="cKey">The Key to check.</param>
		/// <returns>Returns true if the Key is being pressed down.</returns>
		public static bool KeyIsDown(Keys cKey)
		{
			return _currentKeyboardState.IsKeyDown(cKey);
		}

		/// <summary>
		/// Returns true if the Key is being pressed down, and no other input was received in the last TimeInSeconds seconds.
		/// </summary>
		/// <param name="cKey">The Key to check.</param>
		/// <param name="fTimeInSeconds">The amount of time in seconds that must have passed since the last input for this key to be considered pressed down.</param>
		/// <returns>Returns true if the Key is being pressed down, and no other input was received in the last TimeInSeconds seconds.</returns>
		public static bool KeyIsDown(Keys cKey, float fTimeInSeconds)
		{
			// If the Key is being pressed down.
			if (KeyIsDown(cKey))
			{
				// If the specified Time In Seconds has passed since any input was received.
				if (_inputTimeSpan.TotalSeconds >= fTimeInSeconds)
				{
					// Reset the Input Timer.
					_inputTimeSpan = TimeSpan.Zero;

					// Rerun that the specified amount of Time has elapsed since the last input was received.
					return true;
				}
			}

			// Return that the key is not being pressed, or that a key was hit sooner than the specified Time In Seconds.
			return false;
		}

		/// <summary>
		/// Returns true if the Key is not pressed down.
		/// </summary>
		/// <param name="cKey">The Key to check.</param>
		/// <returns>Returns true if the Key is not being pressed down.</returns>
		public static bool KeyIsUp(Keys cKey)
		{
			return _currentKeyboardState.IsKeyUp(cKey);
		}

		/// <summary>
		/// Returns true if the Key was just pressed down.
		/// </summary>
		/// <param name="cKey">The Key to check.</param>
		/// <returns>Returns true if the Key is being pressed now, but was not being pressed last frame.</returns>
		public static bool KeyWasJustPressed(Keys cKey)
		{
			return (_currentKeyboardState.IsKeyDown(cKey) && !_previousKeyboardState.IsKeyDown(cKey));
		}

		/// <summary>
		/// Returns true if the Key was just released.
		/// </summary>
		/// <param name="cKey">The Key to check.</param>
		/// <returns>Returns true if the Key is not being pressed now, but was being pressed last frame.</returns>
		public static bool KeyWasJustReleased(Keys cKey)
		{
			return (_currentKeyboardState.IsKeyUp(cKey) && !_previousKeyboardState.IsKeyUp(cKey));
		}
	}
}
