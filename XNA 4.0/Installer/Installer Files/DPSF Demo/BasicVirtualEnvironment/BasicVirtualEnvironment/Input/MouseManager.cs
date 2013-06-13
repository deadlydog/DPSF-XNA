using System;
using Microsoft.Xna.Framework.Input;

namespace BasicVirtualEnvironment.Input
{
	/// <summary>
	/// The buttons on the mouse that may be pressed.
	/// </summary>
	public enum MouseButtons
	{
		/// <summary>
		/// The Left mouse button.
		/// </summary>
		Left = 0,

		/// <summary>
		/// The Right mouse button.
		/// </summary>
		Right = 1,

		/// <summary>
		/// The Middle mouse button.
		/// </summary>
		Middle = 2,

		/// <summary>
		/// The Forward mouse button.
		/// Corresponds to Microsoft.Xna.Framework.Input.MouseState.XButton1.
		/// </summary>
		Forward = 3,

		/// <summary>
		/// The Back mouse button.
		/// Corresponds to Microsoft.Xna.Framework.Input.MouseState.XButton2.
		/// </summary>
		Back = 4
	}

	public static class MouseManager
	{
		/// <summary>
		/// Used to control user input speed.
		/// </summary>
		private static TimeSpan _inputTimeSpan = TimeSpan.Zero;

		/// <summary>
		/// Gets the current state of the mouse.
		/// </summary>
		public static MouseState CurrentMouseState { get { return _currentMouseState; } }
		private static MouseState _currentMouseState = new MouseState();

		/// <summary>
		/// Gets the previous state of the mouse (i.e. the mouse state last frame).
		/// </summary>
		public static MouseState PreviousMouseState { get { return _previousMouseState; } }
		private static MouseState _previousMouseState = new MouseState();

		/// <summary>
		/// Updates the mouse state at this frame.
		/// <para>NOTE: This should be called every frame, and only once per frame.</para>
		/// <para>NOTE: This should be called before calling any other functions in this class.</para>
		/// </summary>
		/// <param name="timeElapsedSinceLastFrame">The time elapsed since last frame.</param>
		public static void UpdateMouseStateForThisFrame(TimeSpan timeElapsedSinceLastFrame)
		{
			// Backup the mouse state before getting its new value.
			_previousMouseState = _currentMouseState;
			_currentMouseState = Mouse.GetState();
			
			// Add how long it's been since the last user input was received.
			_inputTimeSpan += timeElapsedSinceLastFrame;
		}

		/// <summary>
		/// Returns true if the Mouse Button is being pressed down.
		/// </summary>
		/// <param name="cButton">The Mouse Button to check.</param>
		/// <returns>Returns true if the Mouse Button is being pressed down.</returns>
		public static bool ButtonIsDown(MouseButtons cButton)
		{
			return ButtonIsDown(_currentMouseState, cButton);
		}

		/// <summary>
		/// Returns true if the Mouse State's Mouse Button is being pressed down.
		/// </summary>
		/// <param name="mouseState">The Mouse State to check.</param>
		/// <param name="cButton">The Mouse Button to check.</param>
		/// <returns>Returns true if the Mouse State's Mouse Button is being pressed down.</returns>
		public static bool ButtonIsDown(MouseState mouseState, MouseButtons cButton)
		{
			// Return if the mouse button is being pressed or not.
			switch (cButton)
			{
				default:
				case MouseButtons.Left: return mouseState.LeftButton == ButtonState.Pressed;
				case MouseButtons.Right: return mouseState.RightButton == ButtonState.Pressed;
				case MouseButtons.Middle: return mouseState.MiddleButton == ButtonState.Pressed;
				case MouseButtons.Forward: return mouseState.XButton1 == ButtonState.Pressed;
				case MouseButtons.Back: return mouseState.XButton2 == ButtonState.Pressed;
			}
		}

		/// <summary>
		/// Returns true if the Mouse Button is being pressed down, and no other input was received in the last TimeInSeconds seconds.
		/// </summary>
		/// <param name="cButton">The Mouse Button to check.</param>
		/// <param name="fTimeInSeconds">The amount of time in seconds that must have passed since the last input for this mouse button to be considered pressed down.</param>
		/// <returns>Returns true if the Mouse Button is being pressed down, and no other input was received in the last TimeInSeconds seconds.</returns>
		public static bool ButtonIsDown(MouseButtons cButton, float fTimeInSeconds)
		{
			// If the Button is being pressed down.
			if (ButtonIsDown(cButton))
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

			// Return that the button is not being pressed, or that a button was hit sooner than the specified Time In Seconds.
			return false;
		}

		/// <summary>
		/// Returns true if the Mouse Button is not pressed down.
		/// </summary>
		/// <param name="cButton">The Mouse Button to check.</param>
		/// <returns>Returns true if the Mouse Button is not being pressed down.</returns>
		public static bool ButtonIsUp(MouseButtons cButton)
		{
			return ButtonIsUp(_currentMouseState, cButton);
		}

		/// <summary>
		/// Returns true if the Mouse State's Mouse Button is not pressed down.
		/// </summary>
		/// <param name="mouseState">The Mouse State to check.</param>
		/// <param name="cButton">The Mouse Button to check.</param>
		/// <returns>Returns true if the Mouse State's Mouse Button is not being pressed down.</returns>
		public static bool ButtonIsUp(MouseState mouseState, MouseButtons cButton)
		{
			switch (cButton)
			{
				default:
				case MouseButtons.Left: return _currentMouseState.LeftButton == ButtonState.Released;
				case MouseButtons.Right: return _currentMouseState.RightButton == ButtonState.Released;
				case MouseButtons.Middle: return _currentMouseState.MiddleButton == ButtonState.Released;
				case MouseButtons.Forward: return _currentMouseState.XButton1 == ButtonState.Released;
				case MouseButtons.Back: return _currentMouseState.XButton2 == ButtonState.Released;
			}
		}

		/// <summary>
		/// Returns true if the Mouse Button was just pressed down.
		/// </summary>
		/// <param name="cButton">The Mouse Button to check.</param>
		/// <returns>Returns true if the Mouse Button is being pressed now, but was not being pressed last frame.</returns>
		public static bool ButtonWasJustPressed(MouseButtons cButton)
		{
			return (ButtonIsDown(cButton) && !ButtonIsDown(_previousMouseState, cButton));
		}

		/// <summary>
		/// Returns true if the Button was just released.
		/// </summary>
		/// <param name="cButton">The Button to check.</param>
		/// <returns>Returns true if the Button is not being pressed now, but was being pressed last frame.</returns>
		public static bool ButtonWasJustReleased(MouseButtons cButton)
		{
			return (ButtonIsUp(cButton) && !ButtonIsUp(_previousMouseState, cButton));
		}
	}
}
