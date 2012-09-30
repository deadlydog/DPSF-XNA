using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BasicVirtualEnvironment.Input
{
	public static class GamePadsManager
	{
		/// <summary>
		/// Used to control user input speeds.
		/// </summary>
		private static readonly Dictionary<PlayerIndex, TimeSpan> _inputTimeSpans = new Dictionary<PlayerIndex,TimeSpan>();

		/// <summary>
		/// Gets the current state of the game pads.
		/// </summary>
		public static Dictionary<PlayerIndex, GamePadState> CurrentGamePadStates { get { return _currentGamePadStates; } }
		private static readonly Dictionary<PlayerIndex, GamePadState> _currentGamePadStates = new Dictionary<PlayerIndex, GamePadState>();

		/// <summary>
		/// Gets the previous state of the game pads (i.e. their states at the last frame).
		/// </summary>
		public static Dictionary<PlayerIndex, GamePadState> PreviousGamePadStates { get { return _previousGamePadStates; } }
		private static readonly Dictionary<PlayerIndex, GamePadState> _previousGamePadStates = new Dictionary<PlayerIndex, GamePadState>();

		/// <summary>
		/// Initializes the <see cref="GamePadsManager"/> class.
		/// </summary>
		static GamePadsManager()
		{
			// Initialize the Previous Game Pad States.
			_previousGamePadStates[PlayerIndex.One] = new GamePadState();
			_previousGamePadStates[PlayerIndex.Two] = new GamePadState();
			_previousGamePadStates[PlayerIndex.Three] = new GamePadState();
			_previousGamePadStates[PlayerIndex.Four] = new GamePadState();

			// Initialize the Current Game Pad States.
			_currentGamePadStates[PlayerIndex.One] = new GamePadState();
			_currentGamePadStates[PlayerIndex.Two] = new GamePadState();
			_currentGamePadStates[PlayerIndex.Three] = new GamePadState();
			_currentGamePadStates[PlayerIndex.Four] = new GamePadState();

			// Initialize the Time Spans.
			_inputTimeSpans[PlayerIndex.One] = TimeSpan.Zero;
			_inputTimeSpans[PlayerIndex.Two] = TimeSpan.Zero;
			_inputTimeSpans[PlayerIndex.Three] = TimeSpan.Zero;
			_inputTimeSpans[PlayerIndex.Four] = TimeSpan.Zero;
		}

		/// <summary>
		/// Updates the Game Pad states at this frame.
		/// <para>NOTE: This should be called every frame, and only once per frame.</para>
		/// <para>NOTE: This should be called before calling any other functions in this class.</para>
		/// </summary>
		/// <param name="timeElapsedSinceLastFrame">The time elapsed since last frame.</param>
		public static void UpdateGamePadStatesForThisFrame(TimeSpan timeElapsedSinceLastFrame)
		{
			// Backup the Game Pad states.
			_previousGamePadStates[PlayerIndex.One] = _currentGamePadStates[PlayerIndex.One];
			_previousGamePadStates[PlayerIndex.Two] = _currentGamePadStates[PlayerIndex.Two];
			_previousGamePadStates[PlayerIndex.Three] = _currentGamePadStates[PlayerIndex.Three];
			_previousGamePadStates[PlayerIndex.Four] = _currentGamePadStates[PlayerIndex.Four];

			// Get the new Game Pad state values.
			_currentGamePadStates[PlayerIndex.One] = GamePad.GetState(PlayerIndex.One);
			_currentGamePadStates[PlayerIndex.Two] = GamePad.GetState(PlayerIndex.Two);
			_currentGamePadStates[PlayerIndex.Three] = GamePad.GetState(PlayerIndex.Three);
			_currentGamePadStates[PlayerIndex.Four] = GamePad.GetState(PlayerIndex.Four);

			// Add how long it's been since the last user input was received for each Game Pad.
			_inputTimeSpans[PlayerIndex.One] += timeElapsedSinceLastFrame;
			_inputTimeSpans[PlayerIndex.Two] += timeElapsedSinceLastFrame;
			_inputTimeSpans[PlayerIndex.Three] += timeElapsedSinceLastFrame;
			_inputTimeSpans[PlayerIndex.Four] += timeElapsedSinceLastFrame;
		}

		/// <summary>
		/// Returns true if the GamePad Button is down.
		/// </summary>
		/// <param name="player">The GamePad to check.</param>
		/// <param name="buttons">The Buttons to check.</param>
		/// <returns>Returns true if the GamePad Button is down, false if not.</returns>
		public static bool ButtonIsDown(PlayerIndex player, Buttons buttons)
		{
			return _currentGamePadStates[player].IsButtonDown(buttons);
		}

		/// <summary>
		/// Returns true if the Button is being pressed down, and no other input was received in the last TimeInSeconds seconds.
		/// </summary>
		/// <param name="player">The GamePad to check.</param>
		/// <param name="buttons">The Buttons to check</param>
		/// <param name="timeInSeconds">The amount of time in seconds that must have passed since the last input for the Button to be considered pressed down.</param>
		/// <returns>Returns true if the Button is being pressed down, and no other input was received in the last TimeInSeconds seconds.</returns>
		public static bool ButtonIsDown(PlayerIndex player, Buttons buttons, float timeInSeconds)
		{
			// If the Button is being pressed down
			if (ButtonIsDown(player, buttons))
			{
				// If the specified Time In Seconds has passed since any input was received
				if (_inputTimeSpans[player].TotalSeconds >= timeInSeconds)
				{
					// Reset the Input Timer
					_inputTimeSpans[player] = TimeSpan.Zero;

					// Rerun that the specified amount of Time has elapsed since the last input was received
					return true;
				}
			}

			// Return that the Button is not being pressed, or that a Button was hit sooner than 
			// the specified Time In Seconds
			return false;
		}

		/// <summary>
		/// Returns true if the GamePad Button is up.
		/// </summary>
		/// <param name="player">The GamePad to check.</param>
		/// <param name="buttons">The Buttons to check.</param>
		/// <returns>Returns true if the GamePad Button is up, false if not.</returns>
		public static bool ButtonIsUp(PlayerIndex player, Buttons buttons)
		{
			return _currentGamePadStates[player].IsButtonUp(buttons);
		}

		/// <summary>
		/// Returns true if the GamePad Button was just pressed
		/// </summary>
		/// <param name="player">The GamePad to check.</param>
		/// <param name="buttons">The Buttons to check.</param>
		/// <returns>Returns true if the GamePad Button was just pressed, false if not.</returns>
		public static bool ButtonWasJustPressed(PlayerIndex player, Buttons buttons)
		{
			return (_currentGamePadStates[player].IsButtonDown(buttons) && !_previousGamePadStates[player].IsButtonDown(buttons));
		}

		/// <summary>
		/// Returns true if the GamePad Button was just released.
		/// </summary>
		/// <param name="player">The GamePad to check.</param>
		/// <param name="buttons">The Buttons to check.</param>
		/// <returns>Returns true if the GamePad Button was just released, false if not.</returns>
		public static bool ButtonWasJustReleased(PlayerIndex player, Buttons buttons)
		{
			return (_currentGamePadStates[player].IsButtonUp(buttons) && !_previousGamePadStates[player].IsButtonUp(buttons));
		}
	}	
}
