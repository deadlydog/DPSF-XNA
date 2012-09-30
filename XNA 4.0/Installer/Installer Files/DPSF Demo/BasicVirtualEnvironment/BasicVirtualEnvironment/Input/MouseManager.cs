using System;
using Microsoft.Xna.Framework.Input;

namespace BasicVirtualEnvironment.Input
{
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
	}
}
